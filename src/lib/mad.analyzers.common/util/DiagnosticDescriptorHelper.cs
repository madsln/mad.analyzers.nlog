/*
This code was copied from https://github.com/dotnet/sdk and maybe slightly modified.
Code was originally distributed under the MIT license, see LICENSE.TXT in the project root for license information.
*/

using mad.analyzers.common.extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace mad.analyzers.common.util
{
    public static partial class DiagnosticDescriptorHelper
    {
        public static DiagnosticDescriptor Create(
            string id,
            LocalizableString title,
            LocalizableString messageFormat,
            string category,
            RuleLevel ruleLevel,
            LocalizableString? description,
            bool isEnabledByDefaultInAggressiveMode = true,
            params string[] additionalCustomTags)
        {
            // Ensure 'isEnabledByDefaultInAggressiveMode' is not false for enabled rules in default mode
            Debug.Assert(isEnabledByDefaultInAggressiveMode || ruleLevel == RuleLevel.Disabled || ruleLevel == RuleLevel.CandidateForRemoval);

            var (defaultSeverity, enabledByDefault) = GetDefaultSeverityAndEnabledByDefault(ruleLevel);

#pragma warning disable CA1308 // Normalize strings to uppercase - use lower case ID in help link
            var helpLink = $"https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/{id.ToLowerInvariant()}";
#pragma warning restore CA1308 // Normalize strings to uppercase

            var customTags = GetDefaultCustomTags(isEnabledByDefaultInAggressiveMode);

            if (additionalCustomTags.Length > 0)
            {
                customTags = customTags.Concat(additionalCustomTags).ToArray();
            }

#pragma warning disable RS0030 // The symbol 'DiagnosticDescriptor.DiagnosticDescriptor.#ctor' is banned in this project: Use 'DiagnosticDescriptorHelper.Create' instead
            return new DiagnosticDescriptor(id, title, messageFormat, category, defaultSeverity, enabledByDefault, description, helpLink, customTags);
#pragma warning restore RS0030

            static (DiagnosticSeverity defaultSeverity, bool enabledByDefault) GetDefaultSeverityAndEnabledByDefault(RuleLevel ruleLevel)
            {
                return ruleLevel switch
                {
                    RuleLevel.BuildWarning => (DiagnosticSeverity.Warning, true),
                    RuleLevel.IdeSuggestion => (DiagnosticSeverity.Info, true),
                    RuleLevel.IdeHidden_BulkConfigurable => (DiagnosticSeverity.Hidden, true),
                    RuleLevel.Disabled => (DiagnosticSeverity.Warning, false),
                    RuleLevel.CandidateForRemoval => (DiagnosticSeverity.Warning, false),
                    RuleLevel.BuildError => (DiagnosticSeverity.Error, true),
                    _ => throw new NotImplementedException(),
                };
            }

            static string[] GetDefaultCustomTags(
                bool isEnabledByDefaultInAggressiveMode)
            {
                if (isEnabledByDefaultInAggressiveMode)
                {
                    return WellKnownDiagnosticTagsExtensions.TelemetryEnabledInAggressiveMode;
                }
                else
                {
                    return WellKnownDiagnosticTagsExtensions.Telemetry;
                }
            }
        }
    }
}
