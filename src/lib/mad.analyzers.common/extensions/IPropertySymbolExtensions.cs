/*
This code was copied from https://github.com/dotnet/sdk and maybe slightly modified.
Code was originally distributed under the MIT license, see LICENSE.TXT in the project root for license information.
*/

using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using mad.analyzers.common.extensions;
using mad.analyzers.common.attributes;

namespace mad.analyzers.common.extensions
{
    public static class IPropertySymbolExtensions
    {
        /// <summary>
        /// Check if a property is an auto-property.
        /// TODO: Remove this helper when https://github.com/dotnet/roslyn/issues/46682 is handled.
        /// </summary>
        public static bool IsAutoProperty(this IPropertySymbol propertySymbol)
            => propertySymbol.ContainingType.GetMembers().OfType<IFieldSymbol>().Any(f => f.IsImplicitlyDeclared && SymbolEqualityComparer.Default.Equals(propertySymbol, f.AssociatedSymbol));

        public static bool IsIsCompletedFromAwaiterPattern(
            [NotNullWhen(true)] this IPropertySymbol? property,
            [NotNullWhen(true)] INamedTypeSymbol? inotifyCompletionType,
            [NotNullWhen(true)] INamedTypeSymbol? icriticalNotifyCompletionType)
        {
            if (property is null
                || !property.Name.Equals("IsCompleted", StringComparison.Ordinal)
                || property.Type?.SpecialType != SpecialType.System_Boolean)
            {
                return false;
            }

            var containingType = property.ContainingType?.OriginalDefinition;
            return containingType.DerivesFrom(inotifyCompletionType)
                || containingType.DerivesFrom(icriticalNotifyCompletionType);
        }

        public static ImmutableArray<IPropertySymbol> GetOriginalDefinitions(this IPropertySymbol propertySymbol)
        {
            ImmutableArray<IPropertySymbol>.Builder originalDefinitionsBuilder = ImmutableArray.CreateBuilder<IPropertySymbol>();

            if (propertySymbol.IsOverride && propertySymbol.OverriddenProperty != null)
            {
                originalDefinitionsBuilder.Add(propertySymbol.OverriddenProperty);
            }

            if (!propertySymbol.ExplicitInterfaceImplementations.IsEmpty)
            {
                originalDefinitionsBuilder.AddRange(propertySymbol.ExplicitInterfaceImplementations);
            }

            var typeSymbol = propertySymbol.ContainingType;
            var methodSymbolName = propertySymbol.Name;

            originalDefinitionsBuilder.AddRange(typeSymbol.AllInterfaces
                .SelectMany(m => m.GetMembers(methodSymbolName))
                .OfType<IPropertySymbol>()
                .Where(m => propertySymbol.Parameters.Length == m.Parameters.Length
                            && propertySymbol.IsIndexer == m.IsIndexer
                            && typeSymbol.FindImplementationForInterfaceMember(m) != null));

            return originalDefinitionsBuilder.ToImmutable();
        }
    }
}
