﻿using System;
using System.Collections.Generic;

namespace Kusto.Language.Editor
{
    using Syntax;
    using Utils;

    /// <summary>
    /// Detects expressions that use 'contains' operator
    /// </summary>
    internal class AvoidUsingContainsAnalyzer : KustoAnalyzer
    {
        private static readonly Diagnostic _diagnostic =
            new Diagnostic(
                "KS500",
                DiagnosticCategory.Performance,
                DiagnosticSeverity.Suggestion,
                description: "Avoid using contains operator",
                message: $"Avoid using the 'contains' operator as it has a high compute price." + Environment.NewLine +
                         $"Use the 'has' operator in cases when full term match is desired (see: https://aka.ms/kusto.stringterms).");

        protected override IEnumerable<Diagnostic> GetDiagnostics()
        {
            return new[] { _diagnostic };
        }

        public override void Analyze(KustoCode code, List<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            foreach (var node in code.Syntax.GetDescendants<BinaryExpression>())
            {
                if (node.Kind == SyntaxKind.ContainsExpression ||
                    node.Kind == SyntaxKind.NotContainsExpression ||
                    node.Kind == SyntaxKind.ContainsCsExpression ||
                    node.Kind == SyntaxKind.NotContainsCsExpression)
                {
                    diagnostics.Add(_diagnostic.WithLocation(node.Operator));
                }
            }
        }
    }
}