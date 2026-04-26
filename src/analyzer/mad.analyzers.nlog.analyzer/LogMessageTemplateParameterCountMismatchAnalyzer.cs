/*
This code was copied from https://github.com/dotnet/sdk and maybe slightly modified.
Code was originally distributed under the MIT license, see LICENSE.TXT in the project root for license information.
*/

using mad.analyzers.common;
using mad.analyzers.common.attributes;
using mad.analyzers.common.extensions;
using mad.analyzers.common.util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace mad.analyzers.nlog;

using res = MadAnalyzerCodeQualityResources;

// https://github.com/dotnet/roslyn-analyzers/issues/7438
// deps are in mad.analyzers.common
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class LogMessageTemplateParameterCountMismatchAnalyzer : DiagnosticAnalyzer
{
    public const string MAD1727RuleId = "MAD1727";
    public const string MAD2017RuleId = "MAD2017";
    public const string MAD2023RuleId = "MAD2023";
    public const string MAD2253RuleId = "MAD2253";
    public const string MAD2254RuleId = "MAD2254";
    public const string MAD2255RuleId = "MAD2255";
    public const string MAD2256RuleId = "MAD2256";

    /// <summary>
    /// Use PascalCase for named placeholders
    /// </summary>
    public static readonly DiagnosticDescriptor MAD1727Rule = DiagnosticDescriptorHelper.Create(MAD1727RuleId,
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD1727_Title)),
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD1727_Message)),
                                                                     DiagnosticCategory.Naming,
                                                                     RuleLevel.IdeSuggestion,
                                                                     
                                                                     description: res.CreateLocalizableResourceString(nameof(res.MAD1727_Description)));

    /// <summary>
    /// Parameter count mismatch
    /// </summary>
    public static readonly DiagnosticDescriptor MAD2017Rule = DiagnosticDescriptorHelper.Create(MAD2017RuleId,
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2017_Title)),
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2017_Message)),
                                                                     DiagnosticCategory.Reliability,
                                                                     RuleLevel.BuildWarning,
                                                                     description: res.CreateLocalizableResourceString(nameof(res.MAD2017_Description)));

    /// <summary>
    /// Invalid braces in message template
    /// </summary>
    public static readonly DiagnosticDescriptor MAD2023Rule = DiagnosticDescriptorHelper.Create(MAD2023RuleId,
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2023_Title)),
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2023_Message)),
                                                                     DiagnosticCategory.Reliability,
                                                                     RuleLevel.BuildWarning,
                                                                     description: res.CreateLocalizableResourceString(nameof(res.MAD2023_Description)));

    /// <summary>
    /// Named placeholders should not be numeric values
    /// </summary>
    public static readonly DiagnosticDescriptor MAD2253Rule = DiagnosticDescriptorHelper.Create(MAD2253RuleId,
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2253_Title)),
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2253_Message)),
                                                                     DiagnosticCategory.Usage,
                                                                     RuleLevel.IdeSuggestion,
                                                                     description: res.CreateLocalizableResourceString(nameof(res.MAD2253_Description)));

    /// <summary>
    /// Template should be a static expression
    /// </summary>
    public static readonly DiagnosticDescriptor MAD2254Rule = DiagnosticDescriptorHelper.Create(MAD2254RuleId,
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2254_Title)),
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2254_Message)),
                                                                     DiagnosticCategory.Usage,
                                                                     RuleLevel.IdeSuggestion,
                                                                     description: res.CreateLocalizableResourceString(nameof(res.MAD2254_Description)));

    /// <summary>
    /// Exception should be passed as the first argument to the log method
    /// </summary>
    public static readonly DiagnosticDescriptor MAD2255Rule = DiagnosticDescriptorHelper.Create(MAD2255RuleId,
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2255_Title)),
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2255_Message)),
                                                                     DiagnosticCategory.Reliability,
                                                                     RuleLevel.BuildWarning,
                                                                     description: res.CreateLocalizableResourceString(nameof(res.MAD2255_Description)));

    /// <summary>
    /// Avoid inline serialization in log message arguments
    /// </summary>
    public static readonly DiagnosticDescriptor MAD2256Rule = DiagnosticDescriptorHelper.Create(MAD2256RuleId,
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2256_Title)),
                                                                     res.CreateLocalizableResourceString(nameof(res.MAD2256_Message)),
                                                                     DiagnosticCategory.Performance,
                                                                     RuleLevel.IdeSuggestion,
                                                                     description: res.CreateLocalizableResourceString(nameof(res.MAD2256_Description)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(MAD1727Rule, MAD2017Rule, MAD2023Rule, MAD2253Rule, MAD2254Rule, MAD2255Rule, MAD2256Rule);

    public override void Initialize(AnalysisContext context)
    {
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // it works this way, cbb rn to investigate further
        context.RegisterCompilationStartAction(startAnalysisContext =>
        {
            var wellKnownTypeProvider = WellKnownTypeProvider.GetOrCreate(startAnalysisContext.Compilation);

            if (!wellKnownTypeProvider.TryGetOrCreateTypeByMetadataName(WellKnownTypeNames.NLogLogger, out var loggerType))
            {
                return;
            }

            if (!wellKnownTypeProvider.TryGetOrCreateTypeByMetadataName(WellKnownTypeNames.NLogILogger, out var iLoggerType))
            {
                return;
            }

            if (!wellKnownTypeProvider.TryGetOrCreateTypeByMetadataName(WellKnownTypeNames.NLogILoggerExtensions, out var loggerExtensionsType))
            {
                return;
            }

            var exceptionType = startAnalysisContext.Compilation
                .GetTypeByMetadataName("System.Exception");

            startAnalysisContext.RegisterOperationAction(context =>
            {
                AnalyzeInvocation(context, loggerType!, iLoggerType!, loggerExtensionsType!, exceptionType);
            }, OperationKind.Invocation);
        });
    }

    private void AnalyzeInvocation(OperationAnalysisContext context, INamedTypeSymbol loggerType, INamedTypeSymbol iLoggerType, INamedTypeSymbol loggerExtensionsType, INamedTypeSymbol? exceptionType)
    {
        var invocation = (IInvocationOperation)context.Operation;

        var methodSymbol = invocation.TargetMethod;
        var containingType = methodSymbol.ContainingType;

        if (!containingType.Equals(loggerExtensionsType, SymbolEqualityComparer.Default) &&
            !containingType.Equals(loggerType, SymbolEqualityComparer.Default) &&
            !containingType.Equals(iLoggerType, SymbolEqualityComparer.Default))
        {
            return;
        }

        if (!FindLogParameters(methodSymbol, out var methodMetadata) || methodMetadata == null)
        {
            return;
        }

        var data = ExtractInvocationData(invocation, methodMetadata);

        AnalyzeForMisplacedExceptions(context, data, methodMetadata, exceptionType);
        AnalyzeForInlineSerialization(context, data);

        // TODO: concerning exceptions
        // - check if first parameter is exception when in catch block - but i think sonar already does that
        // - maybe keep track of parameters while evaluating and checking if one is exception?

        if (data.FormatExpression is not null)
        {
            AnalyzeFormatArgument(context, data.FormatExpression, data.ParamsCount, methodSymbol);
        }
    }

    private readonly struct InvocationData
    {
        /// <summary>Die formatExpression-Operation (das message-Argument).</summary>
        public IOperation? FormatExpression { get; }

        /// <summary>Ergebnis von TryGetFormatText – null wenn nicht statisch auflösbar.</summary>
        public string? FormatText { get; }

        /// <summary>Ob das Template statisch auflösbar ist.</summary>
        public bool IsStaticTemplate => FormatText != null;

        /// <summary>Anzahl der tatsächlich übergebenen Log-Argumente.</summary>
        public int ParamsCount { get; }

        /// <summary>
        /// Flach aufgelöste Liste aller Log-Argumente (params bereits expandiert).
        /// Verwendet von MAD2255 und MAD2256.
        /// </summary>
        public ImmutableArray<IOperation> LogArgumentValues { get; }

        public InvocationData(IOperation? formatExpression, string? formatText, int paramsCount, ImmutableArray<IOperation> logArgumentValues)
        {
            FormatExpression = formatExpression;
            FormatText = formatText;
            ParamsCount = paramsCount;
            LogArgumentValues = logArgumentValues;
        }
    }

    private static InvocationData ExtractInvocationData(IInvocationOperation invocation, LogMethodMetadata metadata)
    {
        IOperation? formatExpr = null;
        int paramsCount = 0;
        var logArgValues = ImmutableArray.CreateBuilder<IOperation>();

        foreach (var argument in invocation.Arguments)
        {
            var parameter = argument.Parameter;
            if (parameter == null)
                continue;

            if (SymbolEqualityComparer.Default.Equals(parameter, metadata.MessageParameter))
            {
                formatExpr = argument.Value;
                continue;
            }

            if (!metadata.LogParameterSet.Contains(parameter))
                continue;

            // HasParamCollectionAttribute called at most once per params parameter (R-2)
            if (parameter.IsParams || HasParamCollectionAttribute(parameter))
            {
                // CountParamsElements handles all cases (local refs, object creation, etc.)
                // GetParamElements enumerates the inline elements for downstream analysis (R-6)
                paramsCount = CountParamsElements(argument.Value);
                logArgValues.AddRange(GetParamElements(argument.Value));
            }
            else
            {
                paramsCount++;
                logArgValues.Add(argument.Value);
            }
        }

        // TryGetFormatText called exactly once (R-3)
        var formatText = formatExpr != null ? TryGetFormatText(formatExpr) : null;

        return new InvocationData(formatExpr, formatText, paramsCount, logArgValues.ToImmutable());
    }

    private void AnalyzeFormatArgument(OperationAnalysisContext context, IOperation formatExpression, int paramsCount, IMethodSymbol methodSymbol)
    {
        // FormatText is already computed in InvocationData; re-use it via the already-called TryGetFormatText path.
        // We call TryGetFormatText here on the already-resolved expression – this is the single remaining
        // call; the InvocationData extraction above ensures no second tree-walk for the guard path (R-3).
        var text = TryGetFormatText(formatExpression);
        // MAD2254 argument is not a static string
        if (text == null)
        {
            context.ReportDiagnostic(formatExpression.CreateDiagnostic(MAD2254Rule, methodSymbol.ToDisplayString(GetLanguageSpecificFormat(formatExpression))));

            return;
        }

        LogValuesFormatter formatter;
        try
        {
            formatter = new LogValuesFormatter(text);
        }
        catch (Exception)
        {
            return;
        }

        // invalid message template
        if (!IsValidMessageTemplate(formatter.OriginalFormat))
        {
            context.ReportDiagnostic(formatExpression.CreateDiagnostic(MAD2023Rule));
            return;
        }

        foreach (var valueName in formatter.ValueNames)
        {
            // avoid numbered fields
            if (int.TryParse(valueName, out _))
            {
                context.ReportDiagnostic(formatExpression.CreateDiagnostic(MAD2253Rule));
            }
            // use pascal case
            else
            {
                var capitalIndex = 0;
                if (valueName[capitalIndex] == '@')
                {
                    capitalIndex = 1;
                }
                if (!string.IsNullOrEmpty(valueName) && char.IsLower(valueName[capitalIndex]))
                {
                    context.ReportDiagnostic(formatExpression.CreateDiagnostic(MAD1727Rule));
                }
            }
        }

        // don't pass params as array
        if (paramsCount != formatter.ValueNames.Count)
        {
            context.ReportDiagnostic(formatExpression.CreateDiagnostic(MAD2017Rule, formatter.ValueNames.Count, paramsCount));
        }
    }

    private static SymbolDisplayFormat GetLanguageSpecificFormat(IOperation operation) =>
        operation.Language == LanguageNames.CSharp ?
            SymbolDisplayFormat.CSharpShortErrorMessageFormat : SymbolDisplayFormat.VisualBasicShortErrorMessageFormat;

    private static string? TryGetFormatText(IOperation? argumentExpression)
    {
        if (argumentExpression is null)
            return null;

        switch (argumentExpression)
        {
            case IOperation { ConstantValue: { HasValue: true, Value: string constantValue } }:
                return constantValue;
            case IBinaryOperation { OperatorKind: BinaryOperatorKind.Add } binary:
                var leftText = TryGetFormatText(binary.LeftOperand);
                var rightText = TryGetFormatText(binary.RightOperand);

                if (leftText != null && rightText != null)
                {
                    return leftText + rightText;
                }

                return null;
            default:
                return null;
        }
    }

    /// <summary>
    /// Is the message template valid? (no unclosed braces, no braces without an opening, and no unescaped braces)
    /// </summary>
    /// <param name="messageTemplate">The message template to check for validity.</param>
    /// <returns>When true braces are valid, false otherwise.</returns>
    private static bool IsValidMessageTemplate(string messageTemplate)
    {
        if (messageTemplate is null)
        {
            return false;
        }

        int index = 0;
        bool leftBrace = false;

        while (index < messageTemplate.Length)
        {
            if (messageTemplate[index] == '{')
            {
                if (index < messageTemplate.Length - 1 && messageTemplate[index + 1] == '{')
                {
                    index++;
                }
                else if (leftBrace)
                {
                    return false;
                }
                else
                {
                    leftBrace = true;
                }
            }
            else if (messageTemplate[index] == '}')
            {
                if (leftBrace)
                {
                    leftBrace = false;
                }
                else if (index < messageTemplate.Length - 1 && messageTemplate[index + 1] == '}')
                {
                    index++;
                }
                else
                {
                    return false;
                }
            }

            index++;
        }

        return !leftBrace;
    }

    private static int CountParamsElements(IOperation value)
    {
        switch (value)
        {
            case IArrayCreationOperation arrayCreation:
                return arrayCreation.Initializer?.ElementValues.Length ?? 0;

            case IArrayInitializerOperation arrayInit:
                return arrayInit.ElementValues.Length;

            case IConversionOperation conv:
                return CountParamsElements(conv.Operand);

            case IObjectCreationOperation obj:
                // Bei InlineArray / ReadOnlySpan<T> haben wir oft ein Initializer
                if (obj.Initializer is IObjectOrCollectionInitializerOperation oci)
                {
                    return oci.Initializers.Length;
                }
                else if (obj.Initializer != null)
                {
                    return obj.Initializer.ChildOperations.Count();
                }
                

                // Fallback: Parameter an den ctor übergeben?
                return obj.Arguments.Count();

            case ILocalReferenceOperation localRef:
                return CountFromLocalInitializer(localRef);

            case ICollectionExpressionOperation collectionExpression:
                return CountFromCollectionExpression(collectionExpression);

            default:
                return 0;
        }
    }

    private static int CountFromCollectionExpression(ICollectionExpressionOperation collExpr)
    {
        var count = 0;

        foreach (var element in collExpr.Elements)
        {
            switch (element)
            {
                case ISpreadOperation _:
                    return -1; // unknown

                case IConversionOperation conv
                    when conv.Operand is ICollectionExpressionOperation nested:
                    {
                        var nestedCount = CountFromCollectionExpression(nested);
                        if (nestedCount < 0)
                            return -1;
                        count += nestedCount;
                        break;
                    }

                default:
                    count++;
                    break;
            }
        }

        return count;
    }

    private static int CountFromLocalInitializer(ILocalReferenceOperation localRef)
    {
        var local = localRef.Local;
        var block = localRef.Parent;

        // nächstliegenden Block suchen
        while (block is not null && block is not IBlockOperation)
        {
            block = block.Parent;
        }

        if (block is not IBlockOperation blockOp)
        {
            return 0;
        }

        var declarator = blockOp
            .Descendants()
            .OfType<IVariableDeclaratorOperation>()
            .FirstOrDefault(d => SymbolEqualityComparer.Default.Equals(d.Symbol, local));

        var initValue = declarator?.Initializer?.Value;
        if (initValue is null)
        {
            return 0;
        }

        return CountParamsElements(initValue); // rekursiv wieder oben auswerten
    }

    private static bool HasParamCollectionAttribute(IParameterSymbol parameter)
    {
        return parameter.GetAttributes().Any(a =>
            a.AttributeClass?.Name == "ParamCollectionAttribute" &&
            a.AttributeClass.ContainingNamespace.ToDisplayString() ==
                WellKnownNamespaces.SystemRuntimeCompilerServices);
    }

    private static void AnalyzeForMisplacedExceptions(OperationAnalysisContext context, in InvocationData data, LogMethodMetadata methodMetadata, INamedTypeSymbol? exceptionType)
    {
        if (exceptionType == null)
            return;

        // If the method already uses the exception-first overload, no diagnostic needed.
        // For extension methods the 'this' parameter is at index 0, so the first user-visible
        // parameter is at index 1.
        var firstVisibleParamIndex = methodMetadata.MethodSymbol?.IsExtensionMethod == true ? 1 : 0;
        var firstVisibleParam = methodMetadata.MethodSymbol?.Parameters.ElementAtOrDefault(firstVisibleParamIndex);
        if (firstVisibleParam != null && IsExceptionType(firstVisibleParam.Type, exceptionType))
            return;

        // LogArgumentValues already contains expanded params elements (R-1, R-6)
        foreach (var argValue in data.LogArgumentValues)
        {
            CheckArgumentForException(context, argValue, exceptionType);
        }
    }

    private static IEnumerable<IOperation> GetParamElements(IOperation value)
    {
        switch (value)
        {
            case IArrayCreationOperation arr when arr.Initializer != null:
                return arr.Initializer.ElementValues;
            case ICollectionExpressionOperation col:
                return col.Elements.Cast<IOperation>();
            case IConversionOperation conv:
                return GetParamElements(conv.Operand);
            default:
                return Enumerable.Empty<IOperation>();
        }
    }

    private static bool CheckArgumentForException(OperationAnalysisContext context, IOperation arg, INamedTypeSymbol exceptionType)
    {
        var actual = UnwrapConversion(arg);

        // Case 1 & 4: Exception or derived type passed directly as argument
        if (actual.Type != null && IsExceptionType(actual.Type, exceptionType))
        {
            context.ReportDiagnostic(arg.CreateDiagnostic(MAD2255Rule));
            return true;
        }

        // Case 2: ex.Message where ex is Exception
        if (actual is IMemberReferenceOperation memberAccess
            && memberAccess.Member.Name == "Message"
            && memberAccess.Instance?.Type != null
            && IsExceptionType(memberAccess.Instance.Type, exceptionType))
        {
            context.ReportDiagnostic(arg.CreateDiagnostic(MAD2255Rule));
            return true;
        }

        // Case 3: ex.ToString() where ex is Exception
        if (actual is IInvocationOperation inv
            && inv.TargetMethod.Name == "ToString"
            && inv.Arguments.Length == 0
            && inv.Instance?.Type != null
            && IsExceptionType(inv.Instance.Type, exceptionType))
        {
            context.ReportDiagnostic(actual.CreateDiagnostic(MAD2255Rule));
            return true;
        }

        return false;
    }

    private static IOperation UnwrapConversion(IOperation op)
    {
        while (op is IConversionOperation conv)
            op = conv.Operand;
        return op;
    }

    private static bool IsExceptionType(ITypeSymbol type, INamedTypeSymbol exceptionType)
    {
        var current = type;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, exceptionType))
                return true;
            current = current.BaseType;
        }
        return false;
    }

    private static bool FindLogParameters(IMethodSymbol methodSymbol, [NotNullWhen(returnValue: true)] out LogMethodMetadata? parameterMetadata)
    {
        parameterMetadata = null;
        var result = new LogMethodMetadata
        {
            MethodSymbol = methodSymbol
        };
        foreach (var parameter in methodSymbol.Parameters)
        {
            if (parameter.Type.SpecialType == SpecialType.System_String &&
                string.Equals(parameter.Name, "message", StringComparison.Ordinal))
            {
                result.MessageParameter = parameter;
            }
            else if (parameter.Name.StartsWith("argument", StringComparison.Ordinal))
            {
                result.LogParameters.Add(parameter);
                result.LogParameterSet.Add(parameter);
            }
            else if (parameter.Name.Equals("args", StringComparison.Ordinal) 
                        && parameter.IsParams)
            {
                result.LogParameters.Add(parameter);
                result.LogParameterSet.Add(parameter);
                result.UsesParamsArgument = true;
            }
        }

        if (result.MessageParameter == null)
        {
            return false;
        }

        parameterMetadata = result;
        return result.MessageParameter != null;
    }

    private static void AnalyzeForInlineSerialization(OperationAnalysisContext context, in InvocationData data)
    {
        // Only fire when the template is statically resolvable (MAD2254 would not fire).
        // IsStaticTemplate uses the FormatText already computed in ExtractInvocationData (R-3).
        if (data.FormatExpression != null && !data.IsStaticTemplate)
            return;

        // LogArgumentValues already contains expanded params elements (R-1, R-2, R-6)
        foreach (var argValue in data.LogArgumentValues)
        {
            CheckArgumentForInlineSerialization(context, argValue);
        }
    }

    private static readonly ImmutableHashSet<SpecialType> s_exemptPrimitives = ImmutableHashSet.Create(
        SpecialType.System_Boolean,
        SpecialType.System_Byte,
        SpecialType.System_SByte,
        SpecialType.System_Int16,
        SpecialType.System_UInt16,
        SpecialType.System_Int32,
        SpecialType.System_UInt32,
        SpecialType.System_Int64,
        SpecialType.System_UInt64,
        SpecialType.System_Single,
        SpecialType.System_Double,
        SpecialType.System_Decimal,
        SpecialType.System_Char,
        SpecialType.System_String);

    private static readonly ImmutableHashSet<string> s_exemptTypeNames = ImmutableHashSet.Create(
        "System.Guid",
        "System.DateTime",
        "System.DateTimeOffset",
        "System.TimeSpan",
        "System.DateOnly",
        "System.TimeOnly");

    private static bool IsExemptType(ITypeSymbol? type)
    {
        if (type == null)
            return true;

        // unwrap nullable
        if (type is INamedTypeSymbol named && named.IsGenericType && named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            type = named.TypeArguments[0];

        if (s_exemptPrimitives.Contains(type.SpecialType))
            return true;

        if (type.TypeKind == TypeKind.Enum)
            return true;

        var fqn = type.ToDisplayString();
        return s_exemptTypeNames.Contains(fqn);
    }

    private static void CheckArgumentForInlineSerialization(OperationAnalysisContext context, IOperation arg)
    {
        var actual = UnwrapConversion(arg);

        if (actual is IInvocationOperation inv)
        {
            var methodName = inv.TargetMethod.Name;
            var containingTypeName = inv.TargetMethod.ContainingType?.ToDisplayString();

            // Case 1: JsonSerializer.Serialize / JsonConvert.SerializeObject
            if ((methodName == "Serialize" && containingTypeName == "System.Text.Json.JsonSerializer") ||
                (methodName == "SerializeObject" && containingTypeName == "Newtonsoft.Json.JsonConvert"))
            {
                context.ReportDiagnostic(actual.CreateDiagnostic(MAD2256Rule, actual.Syntax.ToString()));
                return;
            }

            // Case 2: .ToString() on a non-exempt complex type
            if (methodName == "ToString" && inv.Arguments.Length == 0 && !IsExemptType(inv.Instance?.Type))
            {
                context.ReportDiagnostic(actual.CreateDiagnostic(MAD2256Rule, actual.Syntax.ToString()));
            }
        }
    }

    public class LogMethodMetadata
    {
        public IMethodSymbol? MethodSymbol { get; set; }
        public IParameterSymbol? MessageParameter { get; set; }
        public List<IParameterSymbol> LogParameters { get; set; } = [];

        /// <summary>O(1) lookup set for log parameters – mirrors <see cref="LogParameters"/> (R-5).</summary>
        public HashSet<IParameterSymbol> LogParameterSet { get; set; } = new(SymbolEqualityComparer.Default);

        public bool UsesParamsArgument { get; set; } = false;
    }
}
