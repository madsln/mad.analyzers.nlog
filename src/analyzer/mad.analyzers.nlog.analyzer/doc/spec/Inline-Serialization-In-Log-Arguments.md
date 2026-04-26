# MAD2256 — Avoid inline serialization in log message arguments

## Background

NLog's structured logging is built around the **message template** paradigm. Each placeholder in the template corresponds to a positional argument that NLog captures as a structured value. NLog ships two mechanisms to control how that capture happens:

### 1. Destructuring operator `{@Property}`

Prefixing a placeholder name with `@` tells NLog to **destructure** the value — i.e. capture all public properties of the object into the structured log event instead of calling `.ToString()`:

```csharp
logger.Info("Order created: {@Order}", order);
```

The `LogEventInfo` then contains a rich, queryable object instead of a flat string. Targets such as Seq, Elastic, or custom JSON layouts can render it properly.

### 2. Stringification operator `{$Property}`

Prefixing with `$` forces NLog to call `.ToString()` on the value explicitly, regardless of the configured `ValueFormatter`:

```csharp
logger.Info("Status is {$Status}", status);
```

### 3. Custom `IValueFormatter` / `IJsonConverter`

For types that require custom serialization logic (e.g. `System.Text.Json`, `Newtonsoft.Json`), NLog provides the `NLog.Config.IValueFormatter` interface. Registering a formatter makes it apply globally to structured capture — no inline serialization needed in individual log calls.

---

When a developer **serializes inline** — calling `JsonSerializer.Serialize(...)`, `.ToString()`, or any other explicit serialization method as the argument to a logger call — several problems arise:

1. **Structured data is lost.** The log target receives a flat string; no downstream system can query individual fields.
2. **Performance degradation.** Serialization always runs, even when the log level is disabled. NLog's deferred evaluation is bypassed entirely.
3. **Double-serialization risk.** If the target is already a structured/JSON target, the escaped JSON string is nested inside the outer JSON document.
4. **Inconsistent representation.** Each call site may serialize differently (different options, formatters, cultures), making log data unreliable.

---

## Rule ID

`MAD2256`

- **Category:** Performance / Usage  
  → proposed: `DiagnosticCategory.Performance` (primary) with a secondary note in the description mentioning structured-data loss
- **Severity:** `IdeSuggestion`  
  (non-breaking, but clearly bad practice; elevating to `BuildWarning` can be considered in a future version)
- **Analyzer file:** `LogMessageTemplateParameterCountMismatchAnalyzer.cs` (extend existing, same pattern as MAD2255)

---

## Diagnostic Cases

### Case 1 — `JsonSerializer.Serialize` / `JsonConvert.SerializeObject` passed as argument

The most explicit form: the developer calls a serializer directly inside the log call.

```csharp
// ❌ MAD2256
logger.Info("Payload: {Payload}", JsonSerializer.Serialize(myDto));
logger.Info("Payload: {Payload}", JsonConvert.SerializeObject(myDto));

// ✅ Use destructuring operator
logger.Info("Payload: {@Payload}", myDto);
```

Detection:
- Argument is an `IInvocationOperation`
- `TargetMethod.Name` is `"Serialize"` or `"SerializeObject"`
- Containing type is one of:
  - `System.Text.Json.JsonSerializer`
  - `Newtonsoft.Json.JsonConvert`

---

### Case 2 — `.ToString()` called on a non-primitive, non-string argument

Calling `.ToString()` on a complex object is a strong signal of missed structured logging. For primitive types (`int`, `bool`, `Guid`, `DateTime`, enums) it is acceptable and **should not** trigger.

```csharp
// ❌ MAD2256
logger.Info("Item: {Item}", myComplexObject.ToString());

// ✅ Use destructuring or stringification operator
logger.Info("Item: {@Item}", myComplexObject);   // capture structure
logger.Info("Item: {$Item}", myComplexObject);   // explicit ToString via NLog
```

Detection:
- Argument is an `IInvocationOperation`
- `TargetMethod.Name == "ToString"` and `Arguments.Length == 0`
- Receiver type is **not** in the exempt set (see § Exemptions)

---

### Case 3 — Manual string concatenation / interpolation with a complex object

String interpolation (`$"..."`) or concatenation (`+`) involving a complex object produces the same flat-string problem. This case overlaps with MAD2254 (non-static template) but the diagnostic motivation differs: MAD2256 is about the argument, MAD2254 is about the template.

> **Note:** Case 3 has significant overlap with MAD2254. Consider whether MAD2256 should only fire when the template itself is static (i.e. MAD2254 has not fired). This avoids duplicate diagnostics on the same call site.

```csharp
// ❌ MAD2256 (argument) / MAD2254 (template)
logger.Info($"Item: {myObject}");

// ✅
logger.Info("Item: {@Item}", myObject);
```

Detection:
- Argument is an interpolated string or binary `+` concatenation
- At least one sub-expression resolves to a non-exempt complex type

---

## Exemptions (no diagnostic)

The following types / situations must **not** trigger MAD2256:

| Condition | Reason |
|---|---|
| Receiver / argument type is `string` | Already a string — no serialization happening |
| Receiver type is a primitive: `bool`, `byte`, `short`, `int`, `long`, `float`, `double`, `decimal`, `char` | `.ToString()` on primitives is idiomatic |
| Receiver type is `Guid`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `DateOnly`, `TimeOnly` | Value types with well-defined string representations |
| Receiver type is an `enum` | Same rationale |
| Argument is a `const` or literal | No runtime cost, no structured data to lose |
| Placeholder already uses `{@…}` or `{$…}` operator | Developer explicitly opted into NLog's own handling — `.ToString()` inside still degrades, but the intent is explicit. Still emit if Case 1 (explicit serializer call) is detected. |

---

## Detection Strategy

### Step 1 — Identify NLog log-method invocations

Reuse existing type-resolution in `LogMessageTemplateParameterCountMismatchAnalyzer`:
- `NLog.Logger`
- `NLog.ILogger`
- `NLog.ILoggerBase` (if present)
- `NLog.ILoggerExtensions`

### Step 2 — Collect template arguments

Iterate `invocation.Arguments`, identify those that map to a `message`-format-parameter slot (i.e. not the leading `Exception` parameter, not the `message` parameter itself). These are the `argumentN` / `args` parameters.

For `params object[]` arguments, inspect each element individually (reuse `GetParamElements` helper already present in the analyzer).

### Step 3 — For each argument, check for inline serialization

```
argument
  ├─ IInvocationOperation
  │    ├─ TargetMethod.Name ∈ { "Serialize", "SerializeObject" }
  │    │    AND ContainingType ∈ { JsonSerializer, JsonConvert }  → Case 1 ✅
  │    └─ TargetMethod.Name == "ToString" AND Arguments.IsEmpty
  │         AND ReceiverType NOT in ExemptTypes                    → Case 2 ✅
  └─ (future) interpolated string / concatenation containing
       a complex type                                              → Case 3 (v2)
```

### Step 4 — Parse corresponding placeholder (optional enrichment)

If the message template is statically resolvable (i.e. MAD2254 does not fire), correlate the argument index to the corresponding `ValueName` in `LogValuesFormatter.ValueNames`. This allows the fix provider to suggest the correct `{@PlaceholderName}` or `{$PlaceholderName}` rename.

### Step 5 — Report diagnostic

Report on the **argument expression** (not the invocation) so the squiggle is precise. Include the argument expression in the diagnostic properties bag for the code-fix provider.

---

## Resource Strings (Resources.resx)

```xml
<data name="MAD2256_Title" xml:space="preserve">
  <value>Avoid inline serialization in log message arguments</value>
</data>
<data name="MAD2256_Message" xml:space="preserve">
  <value>Argument '{0}' is serialized inline. Pass the original object and use the '{{@Property}}' destructuring syntax or register a custom NLog IValueFormatter instead.</value>
</data>
<data name="MAD2256_Description" xml:space="preserve">
  <value>
    Calling JsonSerializer.Serialize(), JsonConvert.SerializeObject(), or .ToString() on a complex object
    before passing it to a NLog logging method produces a flat string and bypasses NLog's structured
    capture pipeline. Use the '{@Property}' placeholder prefix to let NLog destructure the object, or
    the '{$Property}' prefix to force explicit stringification. For consistent serialization across all
    call sites, register a custom IValueFormatter with NLog's configuration.
    Serializing inline also incurs the serialization cost even when the log level is disabled.
  </value>
</data>
```

---

## AnalyzerReleases.Shipped.md entry

```markdown
| MAD2256 | Performance | IdeSuggestion | Avoid inline serialization in log message arguments |
```

---

## Demo Files

### `diagnostics/MAD2256/`

```
MAD2256/
  logger/
    Logger_Log_JsonSerializerSerializePassedAsArgument.cs
    Logger_Log_JsonConvertSerializeObjectPassedAsArgument.cs
    Logger_Log_ToStringOnComplexObjectPassedAsArgument.cs
  ilogger/
    ILogger_Log_JsonSerializerSerializePassedAsArgument.cs
    ILogger_Log_ToStringOnComplexObjectPassedAsArgument.cs
  iloggerextensions/
    ILoggerExtensions_Log_JsonSerializerSerializePassedAsArgument.cs
    ILoggerExtensions_Log_ToStringOnComplexObjectPassedAsArgument.cs
```

### `nodiag/MAD2256/`

```
MAD2256/
  Logger_Log_PrimitiveToStringNodiag.cs          // int.ToString() → no diag
  Logger_Log_StringArgumentNodiag.cs             // string argument → no diag
  Logger_Log_GuidToStringNodiag.cs               // Guid.ToString() → no diag
  Logger_Log_EnumToStringNodiag.cs               // enum.ToString() → no diag
  Logger_Log_DestructuringOperatorNodiag.cs      // {@Prop} without inline serialization → no diag
```

### Example diagnostic demo file

```csharp
// MAD2256=14:58
using NLog;
using System.Text.Json;

namespace mad.analyzers.nlog.demo.diagnostics.MAD2256.logger;

public class Logger_Log_JsonSerializerSerializePassedAsArgument
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldRaiseDiagnostic_MAD2256(MyDto dto)
    {
        _logger.Info("Processing: {Dto}", JsonSerializer.Serialize(dto));
    }
}
```

### Example no-diagnostic demo file

```csharp
// no diagnostic expected
using NLog;

namespace mad.analyzers.nlog.demo.nodiag.MAD2256.logger;

public class Logger_Log_DestructuringOperatorNodiag
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ShouldNotRaiseDiagnostic(MyDto dto)
    {
        _logger.Info("Processing: {@Dto}", dto);  // ✅ correct
    }
}
```

---

## Test Cases

### Diagnostic tests (generated by TestDataGen)

| Test name | Expected diagnostic |
|---|---|
| `Logger_Log_JsonSerializerSerializePassedAsArgument` | `MAD2256` at argument position |
| `Logger_Log_JsonConvertSerializeObjectPassedAsArgument` | `MAD2256` at argument position |
| `Logger_Log_ToStringOnComplexObjectPassedAsArgument` | `MAD2256` at argument position |

### No-diagnostic tests

| Test name | Expected |
|---|---|
| `Logger_Log_PrimitiveToStringNodiag` | no diagnostic |
| `Logger_Log_GuidToStringNodiag` | no diagnostic |
| `Logger_Log_StringArgumentNodiag` | no diagnostic |
| `Logger_Log_EnumToStringNodiag` | no diagnostic |

### Code-fix tests (manual, in `CodeFixes` test project)

- Fix 1: strips outer serialization call, placeholder unchanged
- Fix 2: strips outer serialization call + rewrites placeholder to `{@...}`

---

## Open Questions / Follow-up

| # | Question | Recommendation |
|---|---|---|
| 1 | Should Case 3 (interpolation/concatenation) be part of MAD2256 or a separate MAD2257? | Start with MAD2256 only covering Cases 1 & 2; add Case 3 later to avoid scope creep. |
| 2 | Should `{$Property}` (explicit stringify) suppress the `.ToString()` Case 2? | Yes — if the developer uses `{$…}` they are consciously requesting NLog's stringify path; suppress the diagnostic. |
| 3 | Severity: `IdeSuggestion` vs `BuildWarning`? | Ship as `IdeSuggestion` first. If performance impact is confirmed via benchmarks, promote to `BuildWarning` in a minor version. |
| 4 | Detection of custom serializers (e.g. `System.Xml.XmlSerializer`, `MessagePack`)? | Out of scope for v1; can be extended via an `editorconfig` allow-list of method FQNs. |
| 5 | Interaction with MAD2254 (non-static template)? | Only fire MAD2256 when the template is static (MAD2254 did not fire for the same call) to avoid confusing dual-diagnostic situations. |
