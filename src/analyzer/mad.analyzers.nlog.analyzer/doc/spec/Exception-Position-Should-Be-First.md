# MAD2255 — Exception should be passed as the first argument to the log method

## Background

NLog logging methods provide overloads where an `Exception` is accepted as the **first** parameter, before the message template:

```csharp
logger.Error(exception, "Something went wrong with {Value}", value);
```

Only when the exception is passed this way does NLog attach it to the `LogEventInfo.Exception` property, which enables:
- Full stack trace capture in structured logging targets
- Proper rendering by layout renderers such as `${exception}` and `${onexception}`
- Correlation with exception-aware log appenders and monitoring systems

When an exception is passed as a **template argument** instead — or only its `.Message` / `.ToString()` is passed — the stack trace is silently lost, making root-cause analysis extremely difficult.

---

## Rule ID

`MAD2255`

- **Category:** Reliability
- **Severity:** BuildWarning
- **Analyzer file:** `LogMessageTemplateParameterCountMismatchAnalyzer.cs` (extend existing) or a new `ExceptionPositionAnalyzer.cs`

---

## Diagnostic Cases

### Case 1 — Exception passed as a template argument (wrong overload)

The exception occupies a `{Placeholder}` slot in the message template instead of being the leading parameter.

```csharp
// ❌ MAD2255
catch (Exception ex)
{
    _logger.Error("Operation failed: {Ex}", ex);
}

// ✅ Correct
catch (Exception ex)
{
    _logger.Error(ex, "Operation failed");
}
```

NLog will format `ex.ToString()` into the message string but the `LogEventInfo.Exception` property will be `null`.

---

### Case 2 — Only `Exception.Message` is passed as a template argument

A very common mistake: the developer consciously extracts only the message string, discarding the type and stack trace entirely.

```csharp
// ❌ MAD2255
catch (Exception ex)
{
    _logger.Error("Operation failed: {ErrorMessage}", ex.Message);
}

// ✅ Correct
catch (Exception ex)
{
    _logger.Error(ex, "Operation failed");
}
```

Detection: argument is a member-access expression where
- the receiver resolves to a type derived from `System.Exception`, **and**
- the accessed member is the `Message` property.

---

### Case 3 — `Exception.ToString()` passed as a template argument

Similar to Case 2 but explicit. The developer calls `.ToString()` on the exception.

```csharp
// ❌ MAD2255
catch (Exception ex)
{
    _logger.Warn("Unexpected issue: {Details}", ex.ToString());
}

// ✅ Correct
catch (Exception ex)
{
    _logger.Warn(ex, "Unexpected issue");
}
```

Detection: argument is an invocation of `ToString()` on a receiver that resolves to `System.Exception` or a derived type.

---

### Case 4 — Exception passed as a non-first positional argument (without a first-parameter exception overload being used)

The exception is passed as a trailing `object` argument while the log call uses the no-exception overload.

```csharp
// ❌ MAD2255
catch (Exception ex)
{
    _logger.Info("Done. Exception was: {Ex}", someValue, ex);
}

// ✅ Correct — use exception-first overload, keep only meaningful template args
catch (Exception ex)
{
    _logger.Info(ex, "Done with {Value}", someValue);
}
```

---

## Detection Strategy

### Step 1 — Identify NLog log-method invocations

Reuse the existing type-resolution logic in `LogMessageTemplateParameterCountMismatchAnalyzer`:
- `NLog.Logger`
- `NLog.ILogger`
- `NLog.ILoggerExtensions`

Target methods: `Trace`, `Debug`, `Info`, `Warn`, `Error`, `Fatal`, `Log` — and their `*Exception` variants.

### Step 2 — Determine whether the exception-first overload is already used

Check whether the **first resolved argument** maps to a parameter of type `System.Exception` (or a derived type). If yes → no diagnostic, the call is correct.

### Step 3 — Inspect remaining arguments for misplaced exceptions

For each argument in the invocation that does **not** map to the leading exception parameter, check:

| Argument shape | Triggers MAD2255? |
|---|---|
| Type is `System.Exception` or derived | ✅ Yes — Case 1 |
| Member access `.Message` on an `Exception` receiver | ✅ Yes — Case 2 |
| Invocation of `.ToString()` on an `Exception` receiver | ✅ Yes — Case 3 |

The check should be performed on the **semantic type** of the argument, not just syntactic patterns, so it catches subclasses (`IOException`, `HttpRequestException`, custom exceptions, etc.).

### Step 4 — Report diagnostic

Report on the **offending argument expression** (not the entire invocation) to make the squiggle precise and actionable.

---

## Suppression / Edge Cases

| Scenario | Expected behaviour |
|---|---|
| Exception intentionally formatted into message for a non-NLog call | Out of scope — rule is scoped to NLog types only |
| `AggregateException.InnerExceptions` iterated in a message | No diagnostic — not a direct `Exception` argument |
| Exception already passed first AND also mentioned in template (double-log) | Separate concern — out of scope for this rule |
| Log call outside a `catch` block | Rule still applies — exceptions can be captured and logged anywhere |
| Conditional (ternary) expression where one branch is an `Exception` | Warn conservatively — flag if the expression type resolves to `Exception` |

---

## Related Rules

| Rule | Relation |
|---|---|
| MAD2017 | Parameter count mismatch — removing an exception from template args may also fix a MAD2017 violation |
| CA2254 | Microsoft's template-should-be-static rule — orthogonal |

---

## Resources

- [NLog Wiki — Exception Logging](https://github.com/NLog/NLog/wiki/Exception-Layout-Renderer)
- NLog source: `LoggerImpl.Write(Type, TargetWithFilterChain[], LogEventInfo, LogFactory)` — `LogEventInfo.Exception` is only populated from the dedicated overload parameter.