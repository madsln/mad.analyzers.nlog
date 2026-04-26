// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using System;

namespace mad.analyzers.common
{
    public enum SymbolVisibility
    {
        Public = 0,
        Internal = 1,
        Private = 2,
        Friend = Internal,
    }

    /// <summary>
    /// Extensions for <see cref="SymbolVisibility"/>.
    /// </summary>
    public static class SymbolVisibilityExtensions
    {
        /// <summary>
        /// Determines whether <paramref name="typeVisibility"/> is at least as visible as <paramref name="comparisonVisibility"/>.
        /// </summary>
        /// <param name="typeVisibility">The visibility to compare against.</param>
        /// <param name="comparisonVisibility">The visibility to compare with.</param>
        /// <returns>True if one can say that <paramref name="typeVisibility"/> is at least as visible as <paramref name="comparisonVisibility"/>.</returns>
        /// <remarks>
        /// For example, <see cref="SymbolVisibility.Public"/> is at least as visible as <see cref="SymbolVisibility.Internal"/>, but <see cref="SymbolVisibility.Private"/> is not as visible as <see cref="SymbolVisibility.Public"/>.
        /// </remarks>
        public static bool IsAtLeastAsVisibleAs(this SymbolVisibility typeVisibility, SymbolVisibility comparisonVisibility)
        {
            return typeVisibility switch
            {
                SymbolVisibility.Public => true,
                SymbolVisibility.Internal => comparisonVisibility != SymbolVisibility.Public,
                SymbolVisibility.Private => comparisonVisibility == SymbolVisibility.Private,
                _ => throw new ArgumentOutOfRangeException(nameof(typeVisibility), typeVisibility, null),
            };
        }
    }

    /// <summary>
    /// Describes different kinds of Dispose-like methods.
    /// </summary>
    public enum DisposeMethodKind
    {
        /// <summary>
        /// Not a dispose-like method.
        /// </summary>
        None,

        /// <summary>
        /// An override of <see cref="IDisposable.Dispose"/>.
        /// </summary>
        Dispose,

        /// <summary>
        /// A virtual method named Dispose that takes a single Boolean parameter, as
        /// is used when implementing the standard Dispose pattern.
        /// </summary>
        DisposeBool,

        /// <summary>
        /// A method named DisposeAsync that has no parameters and returns Task.
        /// </summary>
        DisposeAsync,

        /// <summary>
        /// An overridden method named DisposeCoreAsync that takes a single Boolean parameter and returns Task, as
        /// is used when implementing the standard DisposeAsync pattern.
        /// </summary>
        DisposeCoreAsync,

        /// <summary>
        /// A method named Close on a type that implements <see cref="IDisposable"/>.
        /// </summary>
        Close,

        /// <summary>
        /// A method named CloseAsync that has no parameters and returns Task.
        /// </summary>
        CloseAsync,
    }

    public static class MethodKindEx
    {
        public const MethodKind LocalFunction = (MethodKind)17;

#if HAS_IOPERATION
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable CA1823 // Remove unused private members
        /// <summary>
        /// This will only compile if <see cref="LocalFunction"/> and <see cref="MethodKind.LocalFunction"/> have the
        /// same value.
        /// </summary>
        /// <remarks>
        /// <para>The subtraction in <see cref="LocalFunctionValueAssertion1"/> will overflow if <see cref="MethodKind.LocalFunction"/> is greater, and the conversion
        /// to an unsigned value after negation in <see cref="LocalFunctionValueAssertion2"/> will overflow if <see cref="LocalFunction"/> is greater.</para>
        /// </remarks>
        private const uint LocalFunctionValueAssertion1 = LocalFunction - MethodKind.LocalFunction,
            LocalFunctionValueAssertion2 = -(LocalFunction - MethodKind.LocalFunction);
#endif
    }

    public enum RuleLevel
    {
        /// <summary>
        /// Correctness rule which prevents the compiler from producing a well-defined output binary, and must have
        /// <b>no false positives</b>. Violations of this rule must be fixed by users before any other testing work can
        /// continue. This rule will be <b>enabled in CI and IDE live analysis</b> by default with severity
        /// <see cref="DiagnosticSeverity.Error"/>.
        /// </summary>
        /// <remarks>
        /// Since analyzers cannot directly influence output binaries, this value is typically only valid in the
        /// implementation of source generators. Rare exceptions may occur at the request of a director in coordination
        /// with the core compiler team.
        /// </remarks>
        BuildError = 1,

        /// <summary>
        /// Correctness rule which should have <b>no false positives</b>, and is extremely likely to be fixed by users.
        /// This rule will be <b>enabled in CI and IDE live analysis</b> by default with severity <see cref="DiagnosticSeverity.Warning"/>.
        /// </summary>
        BuildWarning = 2,

        /// <summary>
        /// Correctness rule which should have <b>no false positives</b>, and is extremely likely to be fixed by users.
        /// This rule is a candidate to be turned into a <see cref="BuildWarning"/>.
        /// Until then, this rule will be an <see cref="IdeSuggestion"/>
        /// </summary>
        BuildWarningCandidate = IdeSuggestion,

        /// <summary>
        /// Rule which should have <b>no false positives</b>, and is a valuable IDE live analysis suggestion for opportunistic improvement, but not something to be enforced in CI.
        /// This rule will be <b>enabled by default as an IDE-only suggestion</b> with severity <see cref="DiagnosticSeverity.Info"/> which will be shown in "Messages" tab in Error list.
        /// </summary>
        IdeSuggestion = 3,

        /// <summary>
        /// Rule which <b>may have some false positives</b> and hence would be noisy to be enabled by default as a suggestion or a warning in IDE live analysis or CI.
        /// This rule will be enabled by default with <see cref="DiagnosticSeverity.Hidden"/> severity, so it will be <b>effectively disabled in both IDE live analysis and CI</b>.
        /// However, hidden severity ensures that this rule can be <i>enabled using the category based bulk configuration</i> (TODO: add documentation link on category based bulk configuration).
        /// </summary>
        IdeHidden_BulkConfigurable = 4,

        /// <summary>
        /// <b>Disabled by default rule.</b>
        /// Users would need to explicitly enable this rule with an ID-based severity configuration entry for the rule ID.
        /// This rule <i>cannot be enabled using the category based bulk configuration</i> (TODO: add documentation link on category based bulk configuration).
        /// </summary>
        Disabled = 5,

        /// <summary>
        /// <b>Disabled by default rule</b>, which is a candidate for <b>deprecation</b>.
        /// </summary>
        CandidateForRemoval = 6,

    }
}