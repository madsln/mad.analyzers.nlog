; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

| Rule ID | Category    | Severity      | Notes                                           |
| ------- | ----------- | ------------- | ----------------------------------------------- |
| MAD1727 | Naming      | IdeSuggestion | Use PascalCase for named placeholders           |
| MAD2017 | Reliability | BuildWarning  | Parameter count mismatch                        |
| MAD2023 | Reliability | BuildWarning  | Invalid braces in message template              |
| MAD2253 | Usage       | IdeSuggestion | Named placeholders should not be numeric values |
| MAD2254 | Usage       | IdeSuggestion | Template should be a static expression          |
| MAD2255 | Reliability | BuildWarning  | Exceptions should be passed first               |
| MAD2256 | Performance | IdeSuggestion | Avoid inline serialization in log message arguments |
