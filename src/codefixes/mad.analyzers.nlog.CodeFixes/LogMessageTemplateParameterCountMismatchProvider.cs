using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mad.analyzers.nlog;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LogMessageTemplateParameterCountMismatchProvider)), Shared]
public class LogMessageTemplateParameterCountMismatchProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray<string>.Empty; }
    }

    public sealed override FixAllProvider? GetFixAllProvider()
    {
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        // return WellKnownFixAllProviders.BatchFixer;
        return null;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (root == null)
            return;

        // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic.
        var declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

        if (declaration == null)
            return;

        // Register a code action that will invoke the fix.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: CodeFixResources.CodeFixTitle,
                createChangedSolution: c => MakeUppercaseAsync(context.Document, declaration, c),
                equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
            diagnostic);
    }

    private async Task<Solution> MakeUppercaseAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
    {
        // Compute new uppercase name.
        var identifierToken = typeDecl.Identifier;
        var newName = identifierToken.Text.ToUpperInvariant();

        // Get the symbol representing the type to be renamed.
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

        if (typeSymbol == null)
        {
            // If we couldn't get the symbol, return the original solution.
            return document.Project.Solution;
        }

        // Produce a new solution that has all references to that type renamed, including the declaration.
        var originalSolution = document.Project.Solution;
        var optionSet = originalSolution.Workspace.Options;
        var renameOptions = GetRenameOptions(optionSet);
        var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, renameOptions, newName, cancellationToken).ConfigureAwait(false);

        // Return the new solution with the now-uppercase type name.
        return newSolution;
    }

#pragma warning disable CS0618 // Type or member is obsolete
    private static SymbolRenameOptions GetRenameOptions(OptionSet optionSet)
    {
        var renameOverloads = optionSet.GetOption(RenameOptions.RenameOverloads);
        var renameInStrings = optionSet.GetOption(RenameOptions.RenameInStrings);
        var renameInComments = optionSet.GetOption(RenameOptions.RenameInComments);
        return new SymbolRenameOptions(renameOverloads, renameInStrings, renameInComments);
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
