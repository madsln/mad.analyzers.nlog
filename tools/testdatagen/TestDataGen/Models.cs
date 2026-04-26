namespace TestDataGen;

/// <summary>
/// Represents a fully resolved demo class variant, ready for code generation.
/// </summary>
internal record DemoVariant(
    string ClassName,
    string LoggerType,
    string Namespace,
    string LoggerDeclaration,
    IReadOnlyList<string> UsingDirectives,
    IReadOnlyList<string> LocalVarDeclarations,
    string LoggingCallLine,
    string MessageTemplate,
    int PlaceholderCount,
    int ArgumentCount
);

/// <summary>
/// Represents a single Roslyn diagnostic descriptor extracted from a demo file header.
/// </summary>
internal record DiagnosticDescriptor(
    string Id,
    int Line,
    int Column,
    int Length,
    IReadOnlyList<(string Type, string Value)> Parameters
);

/// <summary>
/// Represents a single generated test method with its processed source and expected diagnostics.
/// </summary>
internal record TestMethodSpec(
    string MethodName,
    string ProcessedCode,
    IReadOnlyList<DiagnosticDescriptor> Diagnostics
);

/// <summary>
/// Represents a generated test class (one .cs file) containing multiple test methods.
/// </summary>
internal record TestClassSpec(
    string ClassName,
    string OutputPath,
    IReadOnlyList<TestMethodSpec> Methods
);
