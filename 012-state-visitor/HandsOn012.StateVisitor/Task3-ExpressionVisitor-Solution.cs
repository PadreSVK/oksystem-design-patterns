// =============================================================================
// Hands-On 012 — Task 3: ExpressionVisitor Rewriting  (solution)
// =============================================================================

using System.Linq.Expressions;
using System.Reflection;

namespace HandsOn.StateVisitor;

// ✅ Rewrites every string == string binary node to a case-insensitive call:
//    string.Equals(left, right, StringComparison.OrdinalIgnoreCase)
public sealed class CaseInsensitiveStringVisitor : ExpressionVisitor
{
    private static readonly MethodInfo StringEquals =
        typeof(string).GetMethod(nameof(string.Equals),
            [typeof(string), typeof(string), typeof(StringComparison)])!;

    protected override Expression VisitBinary(BinaryExpression node)
    {
        if (node.NodeType == ExpressionType.Equal &&
            node.Left.Type  == typeof(string) &&
            node.Right.Type == typeof(string))
        {
            // ✅ rewrite:  left == right  →  string.Equals(left, right, OrdinalIgnoreCase)
            return Expression.Call(
                StringEquals,
                node.Left,
                node.Right,
                Expression.Constant(StringComparison.OrdinalIgnoreCase));
        }

        return base.VisitBinary(node);   // ✅ recurse into unrelated sub-expressions
    }
}
