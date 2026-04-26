namespace TestDataGen.Config;

/// <summary>
/// A single test-case describing a message-template and its associated argument names.
/// </summary>
/// <param name="Name">
///   Human-readable test-case name (matches the Python reference dictionary key, e.g.
///   <c>"no placeholder with 2 args"</c>).
/// </param>
/// <param name="MessageTemplate">
///   The quoted C# string literal for the message template, e.g.
///   <c>"\"Message with {Placeholder1}\""</c>.
/// </param>
/// <param name="ArgumentNames">
///   Ordered list of argument names passed to the logging call.
///   Empty list means no arguments.
/// </param>
internal record TestCase(
    string Name,
    string MessageTemplate,
    IReadOnlyList<string> ArgumentNames);

/// <summary>
/// Static configuration for all test cases.
/// 1:1 port of the <c>testcases</c> dictionary from the Python reference script
/// <c>scripts/gen_MAD2017_demo_files.py</c>.
/// </summary>
internal static class TestCaseConfig
{
    public static readonly IReadOnlyList<TestCase> TestCases =
    [
        new("no placeholder with 2 args",
            "\"Message without placeholder\"",
            ["intArg", "boolArg"]),

        new("no placeholder with 5 params",
            "\"Message without placeholder\"",
            ["intArg", "boolArg", "stringArg", "doubleArg", "objectArg"]),

        new("1 placeholder without args",
            "\"Message with {Placeholder1}\"",
            []),

        new("1 placeholder with 2 args",
            "\"Message with {Placeholder1}\"",
            ["intArg", "boolArg"]),

        new("1 placeholder with 5 params",
            "\"Message with {Placeholder1}\"",
            ["intArg", "boolArg", "stringArg", "doubleArg", "objectArg"]),

        new("2 placeholder2 with 3 args",
            "\"Message with {Placeholder1}{Placeholder2}\"",
            ["intArg", "boolArg", "stringArg"]),

        new("3 placeholders with 2 args",
            "\"Message with {Placeholder1}{Placeholder2}{Placeholder3}\"",
            ["intArg", "boolArg"]),

        new("3 placeholders with 4 args",
            "\"Message with {Placeholder1}{Placeholder2}{Placeholder3}\"",
            ["intArg", "boolArg", "stringArg", "doubleArg"]),

        new("3 placeholders with 5 params",
            "\"Message with {Placeholder1}{Placeholder2}{Placeholder3}\"",
            ["intArg", "boolArg", "stringArg", "doubleArg", "objectArg"]),

        new("6 placeholders with no args",
            "\"Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}\"",
            []),

        new("6 placeholders with 3 args",
            "\"Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}\"",
            ["intArg", "boolArg", "stringArg"]),

        new("6 placeholders with 5 params",
            "\"Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}\"",
            ["intArg", "boolArg", "stringArg", "doubleArg", "objectArg"]),

        new("6 placeholders with 7 params",
            "\"Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}\"",
            ["intArg", "boolArg", "stringArg", "doubleArg", "objectArg", "decimalArg", "longArg"]),
    ];
}
