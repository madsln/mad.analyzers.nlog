# mad.analyzers.nlog

Roslyn analyzer package for NLog that catches common mistakes and code smells in structured log messages at compile time.
It covers the same ground as the log-message diagnostics from `Microsoft.CodeAnalysis.Analyzers` but is tailored specifically to the NLog API, and adds two additional rules that enforce best practices for exception logging and object serialization.
The following diagnostics are included: **MAD1727** (use PascalCase placeholders), **MAD2017** (parameter count mismatch), **MAD2023** (invalid braces in template), **MAD2253** (named placeholders must not be numeric), **MAD2254** (template must be a static expression), **MAD2255** (exceptions should be passed as the first parameter), and **MAD2256** (use NLog's value formatter instead of manual object serialization).

---

## Diagnostics

### MAD1727 — Use PascalCase for placeholders

Placeholder names in log message templates should use PascalCase to follow NLog conventions.

```csharp
// ❌ triggers MAD1727
_logger.Info("Processing value {value}", value);

// ✅ correct
_logger.Info("Processing value {Value}", value);
```

---

### MAD2017 — Parameter count mismatch

The number of arguments passed to the log call must match the number of placeholders in the template.

```csharp
// ❌ triggers MAD2017 — one placeholder, two arguments
_logger.Info("Message with {Placeholder1}", intArg, boolArg);

// ✅ correct
_logger.Info("Message with {Placeholder1}", intArg);
```

---

### MAD2023 — Invalid braces in template

Every placeholder must have both an opening and a closing brace. A missing brace indicates a typo in the template.

```csharp
// ❌ triggers MAD2023 — missing opening brace
_logger.Info("Doing something with value: Value}", value);

// ✅ correct
_logger.Info("Doing something with value: {Value}", value);
```

---

### MAD2253 — Named placeholders should not be numeric values

Numeric placeholder names (e.g. `{0}`, `{1}`) are a `string.Format`-style pattern and are not meaningful in structured logging.

```csharp
// ❌ triggers MAD2253
_logger.Info("Processing value {0}", value);

// ✅ correct
_logger.Info("Processing value {Value}", value);
```

---

### MAD2254 — Template should be a static expression

Log message templates must be compile-time constants or static strings. Interpolated strings defeat structured logging because they are eagerly evaluated.

```csharp
// ❌ triggers MAD2254 — interpolated string
_logger.Info($"Doing something with value: {value}");

// ✅ correct
_logger.Info("Doing something with value: {Value}", value);
```

---

### MAD2255 — Exceptions should be passed as first parameter

NLog reserves the first overload parameter for an `Exception` so that it is stored in the log event's `Exception` property. Passing an exception as a message argument instead loses this association.

```csharp
// ❌ triggers MAD2255
_logger.Error("Operation failed: {Ex}", ex);

// ✅ correct
_logger.Error(ex, "Operation failed");
```

---

### MAD2256 — Use value formatter instead of manual serialization

Manually serializing complex objects (e.g. with `JsonConvert.SerializeObject` or `.ToString()`) before passing them to the logger prevents NLog from applying its own value formatters and structured-logging renderers.

```csharp
// ❌ triggers MAD2256
_logger.Info("Processing: {Dto}", JsonConvert.SerializeObject(dto));

// ✅ correct — let NLog serialize the object
_logger.Info("Processing: {Dto}", dto);
```

---

## Installation

```xml
<PackageReference Include="mad.analyzers.nlog">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```
