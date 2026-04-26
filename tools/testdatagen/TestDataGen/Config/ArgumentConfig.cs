namespace TestDataGen.Config;

/// <summary>
/// Static configuration for local variable declarations used in generated demo methods.
/// </summary>
internal static class ArgumentConfig
{
    /// <summary>
    /// Maps each argument name to the C# local-variable declaration string used in the
    /// generated demo method body.
    /// 1:1 port of <c>args_declaration_map</c> from <c>scripts/gen_MAD2017_demo_files.py</c>.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> LocalVarDeclarations =
        new Dictionary<string, string>
        {
            ["intArg"]     = "var intArg = 42;",
            ["boolArg"]    = "var boolArg = true;",
            ["stringArg"]  = "var stringArg = \"foo\";",
            ["doubleArg"]  = "var doubleArg = 3.14;",
            ["objectArg"]  = "var objectArg = new object();",
            ["decimalArg"] = "var decimalArg = 4.20m;",
            ["longArg"]    = "var longArg = 8008135L;",
        };

    /// <summary>
    /// Declaration for the exception variable used in call-templates with an exception parameter.
    /// </summary>
    public const string ExceptionDeclaration =
        "var exception = new InvalidOperationException(\"Demo exception\");";
}
