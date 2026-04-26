using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace mad.analyzers.nlog.Test.Verifiers
{
    public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic()"/>
        public static DiagnosticResult Diagnostic()
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, MSTestVerifier>.Diagnostic();

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(string)"/>
        public static DiagnosticResult Diagnostic(string diagnosticId)
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, MSTestVerifier>.Diagnostic(diagnosticId);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
        public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, MSTestVerifier>.Diagnostic(descriptor);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
        public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
            => await VerifyAnalyzerAsync(source, Defaults.DotNetVersion, expected);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
        public static async Task VerifyAnalyzerAsync(string source, string netVersion, params DiagnosticResult[] expected)
        {
            var test = new Test
            {
                TestCode = source,
                SolutionTransforms =
                {
                    (solution, projectId) =>
                    {
                        var project = solution.GetProject(projectId)!;
                        var compilationOptions = project.CompilationOptions!;
                        compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                            compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                        solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                        var parseOptions = (CSharpParseOptions)project.ParseOptions!;
                        parseOptions = parseOptions.WithLanguageVersion(LanguageVersion.Latest);
                        solution = solution.WithProjectParseOptions(projectId, parseOptions);

                        return solution;
                    },
                },

                ReferenceAssemblies = GetReferenceAssembliesForDotNetVersion(netVersion)
                                        .AddPackages(ImmutableArray.Create(
                                            new PackageIdentity(
                                                Defaults.NLogLoggingExtensionsPackageName,
                                                Defaults.NLogLoggingExtensionsPackageVersion),
                                            new PackageIdentity(
                                                Defaults.NewtonsoftJsonPackageName,
                                                Defaults.NewtonsoftJsonPackageVersion)
                                        )),

            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, string)"/>
        public static async Task VerifyCodeFixAsync(string source, string fixedSource)
            => await VerifyCodeFixAsync(source, DiagnosticResult.EmptyDiagnosticResults, fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult, string)"/>
        public static async Task VerifyCodeFixAsync(string source, DiagnosticResult expected, string fixedSource)
            => await VerifyCodeFixAsync(source, new[] { expected }, fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult[], string)"/>
        public static async Task VerifyCodeFixAsync(string source, DiagnosticResult[] expected, string fixedSource)
            => await VerifyCodeFixAsync(source, Defaults.DotNetVersion, expected, fixedSource);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult[], string)"/>
        public static async Task VerifyCodeFixAsync(string source, string netVersion, DiagnosticResult[] expected, string fixedSource)
        {
            var test = new Test
            {
                TestCode = source,
                FixedCode = fixedSource,
                ReferenceAssemblies = GetReferenceAssembliesForDotNetVersion(netVersion)
                                        .AddPackages(ImmutableArray.Create(
                                            new PackageIdentity(
                                                Defaults.NLogLoggingExtensionsPackageName,
                                                Defaults.NLogLoggingExtensionsPackageVersion)
                                        ))
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }

        private static ReferenceAssemblies GetReferenceAssembliesForDotNetVersion(string netVersion)
        {
            return netVersion switch
            {
                "net10.0" => ReferenceAssemblies.Net.Net100,
                "net8.0" => ReferenceAssemblies.Net.Net80,
                "netstandard2.0" => ReferenceAssemblies.NetStandard.NetStandard20,
                "netstandard2.1" => ReferenceAssemblies.NetStandard.NetStandard21,
                _ => throw new ArgumentException($"Unsupported .NET version: {netVersion}", nameof(netVersion)),
            };
        }
    }
}
