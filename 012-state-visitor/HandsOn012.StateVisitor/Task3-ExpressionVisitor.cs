// =============================================================================
// Hands-On 012 — Task 3: ExpressionVisitor Rewriting  (starter)
//
// Scenario: you have a LINQ predicate   p => p.Name == "widget"
//           that performs a case-SENSITIVE string comparison.
//           You want to rewrite it so the equality check becomes
//           string.Equals(left, right, StringComparison.OrdinalIgnoreCase)
//           without changing the original lambda.
//
// Your task — implement CaseInsensitiveStringVisitor:
//
//   Override VisitBinary(BinaryExpression node):
//     1. Check node.NodeType == ExpressionType.Equal
//        AND node.Left.Type  == typeof(string)
//        AND node.Right.Type == typeof(string)
//     2. If all three match, rewrite the node to:
//          Expression.Call(StringEquals, node.Left, node.Right,
//              Expression.Constant(StringComparison.OrdinalIgnoreCase))
//        where StringEquals is the MethodInfo for
//          string.Equals(string value, StringComparison comparisonType)
//     3. Otherwise delegate to base.VisitBinary(node)
//
// ExpressionVisitor reminder:
//   • ExpressionVisitor   = base class (provided by .NET — System.Linq.Expressions)
//   • VisitBinary         = override to intercept BinaryExpression nodes
//   • base.VisitBinary    = recurse into child expressions (always call for non-matched nodes)
//   • Visit()             = entry point — pass the root expression to transform
// =============================================================================

using System.Linq.Expressions;
using System.Reflection;

namespace HandsOn.StateVisitor;

// ─── ExpressionVisitor rewriter (your task) ───────────────────────────────────

public sealed class CaseInsensitiveStringVisitor : ExpressionVisitor
{
    private static readonly MethodInfo StringEquals =
        typeof(string).GetMethod(nameof(string.Equals),
            [typeof(string), typeof(string), typeof(StringComparison)])!;

    protected override Expression VisitBinary(BinaryExpression node)
    {
        // TODO: if node is a string == string comparison, rewrite it to
        //       string.Equals(left, right, StringComparison.OrdinalIgnoreCase)
        //       otherwise: return base.VisitBinary(node)
        throw new NotImplementedException();
    }
}
