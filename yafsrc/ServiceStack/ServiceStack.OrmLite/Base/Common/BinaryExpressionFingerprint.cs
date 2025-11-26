using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ServiceStack.ExpressionUtil;

internal sealed class BinaryExpressionFingerprint : ExpressionFingerprint
{
    public BinaryExpressionFingerprint(ExpressionType nodeType, Type type, MethodInfo method)
        : base(nodeType, type)
    {
        // Other properties on BinaryExpression (like IsLifted / IsLiftedToNull) are simply derived
        // from Type and NodeType, so they're not necessary for inclusion in the fingerprint.

        this.Method = method;
    }

    // http://msdn.microsoft.com/en-us/library/system.linq.expressions.binaryexpression.method.aspx
    public MethodInfo Method { get; private set; }

    public override bool Equals(object obj)
    {
        var other = obj as BinaryExpressionFingerprint;
        return (other != null)
               && Equals(this.Method, other.Method)
               && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    override internal void AddToHashCodeCombiner(HashCodeCombiner combiner)
    {
        combiner.AddObject(this.Method);
        base.AddToHashCodeCombiner(combiner);
    }
}