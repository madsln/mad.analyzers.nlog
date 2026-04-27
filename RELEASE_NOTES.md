# Version 1.0.1
- Fixed missing DLL in NuGet package

# Version 1.0.0
- Added `MAD2256`: detect inline serialization of complex types in logging calls
- Added `MAD2255`: detect exception misplacement in logging calls
- Added `MAD1727`: detect incorrect use of splat parameters
- Added support for .NET 10 and .NET 8 spans
- Added ports for Microsoft analyzer diagnostics
  - `CA1727` alias `MAD1727`: use pascal case placeholders
  - `CA2017` alias `MAD2017`: parameter count mismatch
  - `CA2023` alias `MAD2023`: invalid braces in template
  - `CA2253` alias `MAD2253`: named placeholders should not be numeric values
  - `CA2254` alias `MAD2254`: template should be a static expression