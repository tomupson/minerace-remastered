using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VContainerAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InjectAttributeSuppressor : DiagnosticSuppressor
    {
        private static readonly SuppressionDescriptor SuppressionDescriptor =
            new SuppressionDescriptor(
                id: "SPR0001",
                suppressedDiagnosticId: "CS0649",
                justification: "Field meets suppression criteria");

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => [SuppressionDescriptor];

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
            {
                IFieldSymbol? fieldSymbol = GetFieldSymbol(diagnostic, context);

                if (fieldSymbol != null && HasInjectAttribute(fieldSymbol))
                {
                    context.ReportSuppression(Suppression.Create(SuppressionDescriptor, diagnostic));
                }
            }
        }

        private static IFieldSymbol? GetFieldSymbol(Diagnostic diagnostic, SuppressionAnalysisContext context)
        {
            SyntaxNode? root = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken);
            SyntaxNode? node = root?.FindNode(diagnostic.Location.SourceSpan);

            if (node is VariableDeclaratorSyntax variable)
            {
                SemanticModel model = context.GetSemanticModel(diagnostic.Location.SourceTree!);
                return model.GetDeclaredSymbol(variable) as IFieldSymbol;
            }

            return null;
        }

        private static bool HasInjectAttribute(IFieldSymbol fieldSymbol)
        {
            return fieldSymbol.GetAttributes().Any(attr => attr.AttributeClass!.Name == "InjectAttribute");
        }
    }
}
