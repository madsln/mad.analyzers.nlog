using System.Text.RegularExpressions;
using TestDataGen.Config;

namespace TestDataGen.Demo;

/// <summary>
/// Builds up to two <see cref="DemoVariant"/> instances per combination of
/// logger type × call template × test case:
/// <list type="bullet">
///   <item><description>Direct – arguments passed directly in the logging call.</description></item>
///   <item><description>ObjectArray – arguments collected into <c>object[]</c>.</description></item>
/// </list>
/// <remarks>
/// ReadOnlySpan variants have been removed per ADR-011: the explicit
/// <c>ReadOnlySpan&lt;object&gt;</c> pattern does not occur in NLog production code,
/// and the transparent C# 13 <c>params ReadOnlySpan&lt;T&gt;</c> compiler path is
/// invisible to Roslyn and already covered by the Direct variants.
/// </remarks>
/// </summary>
internal static class VariantBuilder
{
    // Matches {PlaceholderN} patterns to count placeholders.
    private static readonly Regex PlaceholderRegex = new(@"\{\w+\}", RegexOptions.Compiled);

    /// <summary>
    /// Builds all applicable variants for the given combination.
    /// </summary>
    public static IEnumerable<DemoVariant> Build(
        string loggerType,
        CallTemplate template,
        TestCase testCase)
    {
        var argNames = testCase.ArgumentNames;
        var argsStr = string.Join(", ", argNames);
        var message = testCase.MessageTemplate;

        int placeholderCount = PlaceholderRegex.Matches(message).Count;
        int argCount = argNames.Count;

        // Using directives: NLog always; System.Globalization when CultureInfo is used.
        var usingDirectives = new List<string> { "using NLog;" };
        if (template.HasCultureInfo)
            usingDirectives.Add("using System.Globalization;");

        var ns = $"mad.analyzers.nlog.demo.generated.diagnostics.MAD2017.{loggerType.ToLower()}";
        var loggerDecl = LoggerConfig.FieldDeclarations[loggerType];

        // Base variable declarations: exception first (if needed), then arg declarations.
        var baseDecls = new List<string>();
        if (template.HasException)
            baseDecls.Add(ArgumentConfig.ExceptionDeclaration);
        foreach (var arg in argNames)
            if (ArgumentConfig.LocalVarDeclarations.TryGetValue(arg, out var decl))
                baseDecls.Add(decl);

        // Helper: resolve a full logging call line from the template.
        string ResolveCall(string messageAndArgs) =>
            template.Pattern
                    .Replace("%LOGLEVEL%", "Info")
                    .Replace("{MESSAGE_AND_ARGS}", messageAndArgs);

        // --- Variant 1: Direct ---
        var directMsgAndArgs = argCount > 0 ? $"{message}, {argsStr}" : message;
        yield return new DemoVariant(
            ClassName:            ClassNameBuilder.Build(loggerType, template.HasException, testCase.Name, ""),
            LoggerType:           loggerType,
            Namespace:            ns,
            LoggerDeclaration:    loggerDecl,
            UsingDirectives:      usingDirectives,
            LocalVarDeclarations: baseDecls,
            LoggingCallLine:      ResolveCall(directMsgAndArgs),
            MessageTemplate:      message,
            PlaceholderCount:     placeholderCount,
            ArgumentCount:        argCount);

        // --- Variant 2: ObjectArray ---
        // Python: "var objectArray = new object[] { {CONTENT} };" with CONTENT = message_args (or "")
        var arrayDecl = $"var objectArray = new object[] {{ {argsStr} }};";
        var arrayDecls = new List<string>(baseDecls) { arrayDecl };
        yield return new DemoVariant(
            ClassName:            ClassNameBuilder.Build(loggerType, template.HasException, testCase.Name, "ObjectArray"),
            LoggerType:           loggerType,
            Namespace:            ns,
            LoggerDeclaration:    loggerDecl,
            UsingDirectives:      usingDirectives,
            LocalVarDeclarations: arrayDecls,
            LoggingCallLine:      ResolveCall($"{message}, objectArray"),
            MessageTemplate:      message,
            PlaceholderCount:     placeholderCount,
            ArgumentCount:        argCount);

    }
}
