using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GetTypeToNameOf
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(GetTypeToNameOfCodeFixProvider)), Shared]
    public sealed class GetTypeToNameOfCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(GetTypeToNameOfAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Id != GetTypeToNameOfAnalyzer.DiagnosticId)
                {
                    continue;
                }

                var diagnosticSpan = diagnostic.Location.SourceSpan;

                // Find the type declaration identified by the diagnostic.
                SyntaxNode declaration = root
                    .FindNode(diagnosticSpan)
                    .DescendantNodesAndSelf()
                    .OfType<MemberAccessExpressionSyntax>()
                    .FirstOrDefault();

                if (declaration == null)
                {
                    continue;
                }

                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixResources.CodeFixTitle,
                        createChangedSolution: c => DoFixAsync(diagnostic, context.Document, declaration, c),
                        equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                    diagnostic);
            }
        }

        private async Task<Solution> DoFixAsync(
            Diagnostic diagnostic,
            Document document,
            SyntaxNode subjectNode,
            CancellationToken cancellationToken)
        {
            var prefer = diagnostic.Properties[GetTypeToNameOfAnalyzer.PreferPropertyName];
            var toReplaceNode = SyntaxFactory.ParseExpression(prefer);

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot.ReplaceNode(subjectNode, toReplaceNode);

            // Return document with transformed tree.
            return document.WithSyntaxRoot(newRoot).Project.Solution;
        }
    }
}
