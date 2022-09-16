using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace GetTypeToNameOf
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class GetTypeToNameOfAnalyzer : DiagnosticAnalyzer
    {
        public const string PreferPropertyName = "prefer";

        public const string DiagnosticId = "GetTypeToNameOf";
        private const string Category = "Simplification";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            //context.RegisterSyntaxNodeAction(DoAnalyze, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(DoAnalyze, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void DoAnalyze(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is MemberAccessExpressionSyntax maes))
            {
                return;
            }

            if (maes.Name.Identifier.ValueText != nameof(System.Type.Name))
            {
                return;
            }

            //if (maes.OperatorToken == SyntaxToken.Dot)
            //{
            //    return;
            //}

            if (!(maes.Expression is InvocationExpressionSyntax ies))
            {
                return;
            }

            if (ies.ArgumentList.Arguments.Count != 0)
            {
                return;
            }

            {
                if (ies.Expression is MemberAccessExpressionSyntax getTypeExpression)
                {
                    Process(context, maes, ies, getTypeExpression);
                    return;
                }
            }
            {
                if (ies.Expression is IdentifierNameSyntax getTypeExpression)
                {
                    Process(context, maes, ies, getTypeExpression);
                    return;
                }
            }

            //miss! do nothing.
        }

        private static void Process(
            SyntaxNodeAnalysisContext context,
            MemberAccessExpressionSyntax maes,
            InvocationExpressionSyntax ies,
            IdentifierNameSyntax getTypeExpression
            )
        {
            if (getTypeExpression.Identifier.ValueText != nameof(System.Type.GetType))
            {
                return;
            }

            var parentTypeSymbol = TryGetParentTypeSymbol(context.SemanticModel, getTypeExpression);
            if (parentTypeSymbol == null)
            {
                return;
            }

            var prefer = $"nameof({parentTypeSymbol.Name})";

            context.ReportDiagnostic(
                CreateDiagnostic(maes, prefer)
                );
        }

        private static void Process(
            SyntaxNodeAnalysisContext context,
            MemberAccessExpressionSyntax maes,
            InvocationExpressionSyntax ies,
            MemberAccessExpressionSyntax getTypeExpression
            )
        {
            if (getTypeExpression.Name.Identifier.ValueText != nameof(System.Type.GetType))
            {
                return;
            }

            if (!(getTypeExpression.Expression is ThisExpressionSyntax thisExpression))
            {
                return;
            }

            var parentTypeSymbol = TryGetParentTypeSymbol(context.SemanticModel, thisExpression);
            if (parentTypeSymbol == null)
            {
                return;
            }

            var prefer = $"nameof({parentTypeSymbol.Name})";

            context.ReportDiagnostic(
                CreateDiagnostic(maes, prefer)
                );
        }

        private static Diagnostic CreateDiagnostic(MemberAccessExpressionSyntax maes, string prefer)
        {
            return
                Diagnostic.Create(
                    Rule,
                    maes.GetLocation(),
                    new Dictionary<string, string>
                    {
                        [PreferPropertyName] = prefer
                    }.ToImmutableDictionary(),
                    maes.ToFullString(),
                    prefer
                    );
        }

        private static INamedTypeSymbol TryGetParentTypeSymbol(SemanticModel semanticModel, SyntaxNode root)
        {
            var parentType = TryGetParentType(root);
            if (parentType == null)
            {
                return null;
            }

            var parentTypeSymbol = semanticModel.GetDeclaredSymbol(parentType);
            if (parentTypeSymbol == null)
            {
                return null;
            }

            if (!parentTypeSymbol.IsSealed)
            {
                return null;
            }

            if (parentTypeSymbol.IsGenericType)
            {
                return null;
            }

            //check for method with name "GetType" in the type and its accessors
            //if it is exists, it may interfere with System.Type.GetType
            //so skip such type
            var typeSymbol = parentTypeSymbol;
            while (typeSymbol.BaseType != null)
            {
                foreach (var member in typeSymbol.GetMembers())
                {
                    if (!(member is IMethodSymbol ms))
                    {
                        continue;
                    }

                    if (ms.Name == nameof(System.Type.GetType))
                    {
                        return null;
                    }
                }

                typeSymbol = typeSymbol.BaseType;
            }

            return parentTypeSymbol;
        }

        private static TypeDeclarationSyntax TryGetParentType(SyntaxNode root)
        {
            while (root.Parent != null && root.Parent != root)
            {
                if (root is ClassDeclarationSyntax cds)
                {
                    return cds;
                }
                if (root is RecordDeclarationSyntax rds)
                {
                    return rds;
                }
                if (root is StructDeclarationSyntax sds)
                {
                    return sds;
                }

                root = root.Parent;
            }

            return null;
        }
    }

}
