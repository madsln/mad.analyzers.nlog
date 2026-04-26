# Contributing to mad.analyzers

Thank you for your interest in contributing! This document covers how to report bugs, request new features, and how the demo/test pipeline works — including the **demo file header format** that drives automated test generation.

---

## Table of Contents

- [Contributing to mad.analyzers](#contributing-to-madanalyzers)
  - [Table of Contents](#table-of-contents)
  - [Reporting Bugs](#reporting-bugs)
  - [Requesting Features](#requesting-features)
  - [Demo File Header Format](#demo-file-header-format)
    - [Diagnostic files (`diagnostics/`)](#diagnostic-files-diagnostics)
    - [Non-diagnostic files (`nodiag/`)](#non-diagnostic-files-nodiag)
    - [Header syntax reference](#header-syntax-reference)
    - [Examples](#examples)

---

## Reporting Bugs

Use the GitHub **Issues** tab and choose the **Bug report** template.

Please provide:

| Field | Expected content |
|---|---|
| **Analyzer ID** | e.g. `MAD2017`, `MAD2256` |
| **Summary** | One sentence describing the wrong behaviour |
| **Minimal reproduction** | A self-contained C# snippet that reproduces the issue |
| **Expected behaviour** | What the analyzer *should* report (or not report) |
| **Actual behaviour** | What it *actually* reports |
| **Environment** | .NET SDK version, IDE, NLog version |

> ℹ️ If the bug involves a false positive or a missed diagnostic, attach or inline the relevant C# code directly in the issue. The shorter and more focused the snippet, the faster the turnaround.

---

## Requesting Features

Use the GitHub **Issues** tab and choose the **Feature request** template.

Please provide:

| Field | Expected content |
|---|---|
| **Motivation** | Which NLog misuse / code smell should be caught? |
| **Proposed diagnostic ID** | Suggest an ID in the `MADxxxx` range or reference an existing `CAxxxx` rule |
| **Triggering example** | C# code that *should* raise a diagnostic |
| **Non-triggering example** | C# code that *should not* raise a diagnostic |
| **Proposed message** | Human-readable diagnostic message text |
| **Severity** | `Error`, `Warning`, or `Info` |

For larger changes (new analyzer, new code-fix) it is recommended to open an issue *before* sending a pull request so the design can be discussed first.

---

## Demo File Header Format

Every `.cs` file inside `src/demo/…/diagnostics/` carries a structured comment header at the start of the document describing all contained diagnostics. This header is the **single source of truth** for the automated test generator: it tells `TestDataGen` which diagnostics to expect, where they are located inside the (whitespace-stripped) source, and which diagnostic arguments to assert.

### Diagnostic files (`diagnostics/`)

Files that are expected to raise one or more diagnostics **must** start with one header line per diagnostic:

```
// <DiagnosticId>=<StartOffset>:<MessageLength>:<SpanLength>[;<ArgType>=<ArgValue>...]
```

Multiple diagnostics in the same file are expressed as multiple consecutive header lines:

```csharp
// MAD2255=13:62:2
// MAD2017=13:22:27;int=1;int=2
```

### Non-diagnostic files (`nodiag/`)

Files that must *not* raise any diagnostic carry **no header** at all. The absence of a header line is itself the signal to the test generator that `VerifyAnalyzerAsync` should be called with zero expected diagnostics.

### Header syntax reference

```
// {Id}={Line}:{Column}:{Length}[;{Type}={Value}]*
```

| Token | Type | Description |
|---|---|---|
| `Id` | string | Diagnostic identifier, e.g. `MAD2017` |
| `Line` | int | Line number counted after header ends |
| `Column` | int | One-based column number of the first character of the diagnostics span |
| `Length` | int | Length of the full **diagnostic span** (characters) |
| `Type=Value` | typed pair | One entry per diagnostic argument; `int` values are emitted as integer literals, everything else as a verbatim string |

**Offset semantics:** All offsets and lengths are measured on the source text *after* stripping comment lines and blank lines. This matches the text the Roslyn analyzer test harness receives.

### Examples

**Single diagnostic, two `int` arguments (MAD2017 — parameter count mismatch)**

```csharp
// MAD2017=13:22:29;int=1;int=2
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2017.logger;

public class Logger_Log_1PlaceholderWith2Args
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2017()
    {
        var intArg  = 42;
        var boolArg = true;
        _logger.Info("Message with {Placeholder1}", intArg, boolArg);
    }
}
```

`int=1` → 1 placeholder in the template  
`int=2` → 2 arguments passed → mismatch → `MAD2017` is raised

---

**Single diagnostic, one `string` argument (MAD2256 — serialised object passed as log argument)**

```csharp
// MAD2256=11:43:14;string=dto.ToString()
using NLog;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2256.logger;

public class Logger_Log_ToStringOnComplexObjectPassedAsArgument
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2256(Dto dto)
    {
        _logger.Info("Processing: {Dto}", dto.ToString());
    }
}
```

`string=dto.ToString()` → the offending argument expression, reported verbatim in the diagnostic message.

---

**Two diagnostics in one file (MAD2255 + MAD2017)**

```csharp
// MAD2255=13:62:2
// MAD2017=13:22:27;int=1;int=2
using NLog;
…
```

The test generator produces one `expected` variable per header line, in declaration order.

---

**No-diagnostic file (MAD2256 — destructuring operator is fine)**

```csharp
using NLog;

namespace mad.analyzers.nlog.demo.nodiag.MAD2256;

public class Logger_Log_DestructuringOperatorNodiag
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldNotRaiseDiagnostic(Dto dto)
    {
        _logger.Info("Processing: {@Dto}", dto);  // ✅ correct — NLog destructures the object
    }
}
```

No header → the generated test calls `VerifyAnalyzerAsync(source)` with no `expected` arguments.