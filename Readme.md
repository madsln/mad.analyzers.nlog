# NLog LogMessage Analyzer

This analyzer is similar to the log message analyzer provided by the `Microsoft.CodeAnalysis.Analyzers` package.

It provides analyzing capabilities to find common mistakes and code smells when writing log messages.

Supported ports are listed below
- `CA1727` alias `MAD1727`: use pascal case placeholders
- `CA2017` alias `MAD2017`: parameter count mismatch
- `CA2023` alias `MAD2023`: invalid braces in template
- `CA2253` alias `MAD2253`: named placeholders should not be numeric values
- `CA2254` alias `MAD2254`: template should be a static expression

Added diagnostics
- `MAD2255`: exceptions should be passed as first parameter
- `MAD2256`: use valueformatter instead of manual serialization of complex objects

A lot of the code to kickstart this project was taken from the `dotnet/sdk` repository from github.
It was modified to fit the `NLog` logging API.

Code fixes are currently not supported nor planned.

## Usage of AI

This project heavily relied on the use of Github Copilot along with the model Claude Sonnet 4.6.
It was used as a playground to using copilot but also learning how to develop a dotnet analyzer.

The repository features a lot of definition, adr and journal documents which were part of my approach of using AI effectively here.
From my point of view it seemed to have an impact on the stability of quality and results during development.

Improvements for the future would most likely be to also create some basic rules and instructions files for copilot to achieve a more consistent code style.
And also establish some coding-conventions.

## Demo / Test Pipeline

The project uses a two-stage pipeline to keep demo code and unit tests in sync. Demo files act as **living documentation** (they are compilable, IDE-navigable C# files that trigger the analyzer in real-time) and as the **data source** for the test suite.

```
┌───────────────────────────────────────────────────────────┐
│  src/demo/mad.analyzers.nlog.demo/                 │
│    diagnostics/<MADxxxx>/<loggertype>/<ClassName>.cs      │  ← human-authored or generated
│    nodiag/<MADxxxx>/<ClassName>.cs                        │
│    usings/<Symbol>.cs                                     │
└───────────────────────┬───────────────────────────────────┘
                        │  TestDataGen  (tools/testdatagen)
                        ▼
┌───────────────────────────────────────────────────────────┐
│  src/test/mad.analyzers.nlog.Test/generated/       │
│    singlediag/<Namespace_ClassName>_Tests.cs              │  ← auto-generated, do not edit
│    nodiag/<Namespace_ClassName>_Tests.cs                  │
└───────────────────────┬───────────────────────────────────┘
                        │  dotnet test
                        ▼
                   MSTest + Roslyn Verifier
```

### Directory layout

| Path | Purpose |
|---|---|
| `diagnostics/<MADxxxx>/<loggertype>/` | Demo files that **must** produce a diagnostic |
| `nodiag/<MADxxxx>/` | Demo files that **must not** produce any diagnostic |
| `usings/<Symbol>.cs` | Shared type definitions / preambles (e.g. `Dto`, `IDisposable` stubs) referenced by demo files |
| `tools/testdatagen/` | Source of the `TestDataGen` CLI tool |
| `src/test/…/generated/` | Auto-generated test files — committed to source control, but never edited by hand |

### Step 1 — Write or generate a demo file

1. Choose the correct sub-folder:
   - `diagnostics/<MADxxxx>/<loggertype>/` if the code **should** trigger the analyzer.
   - `nodiag/<MADxxxx>/` if the code **should not** trigger the analyzer.
2. Name the file following the convention:  
   `{LoggerType}_Log_[WithException_]{PascalCaseDescription}[_{ObjectArray|ReadOnlySpan}].cs`
3. Add the header (diagnostic files only) as described in [Demo File Header Format](#demo-file-header-format).
4. Keep the file minimal — one method, one logging call per file.

### Step 2 — Run TestDataGen to produce unit-test sources

```powershell
.\build\bin\Debug\TestDataGen\net10.0\TestDataGen.exe test `
    --source .\src\demo\mad.analyzers.nlog.demo `
    --out    .\src\test\mad.analyzers.nlog.Test\generated\
```

`TestDataGen` performs the following transformations:

1. **Loads shared preambles** from `usings/` (keyed by filename without extension).
2. **Strips** comment lines and blank lines from every demo file.
3. **Replaces** the concrete log-level identifier (e.g. `Info`) with the placeholder `%LOGLEVEL%` so that a single parameterised test method covers all six NLog log levels (`Trace`, `Debug`, `Info`, `Warn`, `Error`, `Fatal`).
4. **Injects preambles** where referenced symbols are detected.
5. **For `diagnostics/` files:** parses the header, computes span positions in the stripped text, and wraps the flagged token in `{|#N:…|}` Roslyn marker syntax.
6. **Wraps** the code in a standard `namespace analyzer.test { … }` skeleton.
7. **Escapes** double-quotes for embedding in C# verbatim strings.
8. **Emits** one `[DataTestMethod]` per demo file, bundled into one `[TestClass]` per sub-directory.

### Step 3 — Build and run the test project

```powershell
dotnet test .\src\test\mad.analyzers.nlog.Test\
```

Each generated test method:
- Substitutes `%LOGLEVEL%` with the actual level from the `[DataRow]` attribute.
- Calls `VerifyCS.VerifyAnalyzerAsync(source)` — without expected diagnostics for `nodiag`, or with one `VerifyCS.Diagnostic(…).WithLocation(N).WithArguments(…)` per header line for `diagnostics`.

> ℹ️ The generated files under `generated/` are committed to source control. Re-running `TestDataGen` after modifying a demo file must be done before opening a pull request so that tests and demo files stay in sync.
