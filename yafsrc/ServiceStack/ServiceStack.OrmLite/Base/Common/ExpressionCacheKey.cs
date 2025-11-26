using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ServiceStack.ExpressionUtil;

public static class ExpressionCacheKey
{
    public static bool TryGetKey(LambdaExpression expr, out string key)
    {
        try
        {
            if (!CanCache(expr))
            {
                key = null;
                return false;
            }

            var sb = new StringBuilder();
            new CanonicalExpressionPrinter(sb).Visit(expr);
            key = sb.ToString();
            return true;
        }
        catch
        {
            key = null;
            return false;
        }
    }

    public static bool CanCache(Expression expr)
    {
        if (ClosureSafety.HasMutableClosure(expr))
        {
            return false;
        }

        return CacheableExpressionVisitor.IsCacheable(expr);
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }

    private sealed class CanonicalExpressionPrinter(StringBuilder sb) : ExpressionVisitor
    {
        override protected Expression VisitConstant(ConstantExpression node)
        {
            // Remove closure object identity
            if (node.Type.Name.Contains("DisplayClass"))
            {
                sb.Append($"CONST(closure:{node.Type.FullName})");
                return node;
            }

            // Normal constants
            sb.Append($"CONST({node.Value})");
            return node;
        }

        override protected Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ConstantExpression c &&
                c.Type.Name.Contains("DisplayClass"))
            {
                // Include the closure declaring type + member type to handle array/ref struct differences
                sb.Append("CLOSURE_MEMBER(");
                sb.Append(c.Type.FullName);
                sb.Append(".");
                sb.Append(node.Member.Name);
                sb.Append(":");
                sb.Append(node.Type.FullName);
                sb.Append(")");
                return node;
            }
            sb.Append($"MEMBER({node.Member.DeclaringType}.{node.Member.Name}:{node.Type.FullName})");
            return base.VisitMember(node);
        }

        override protected Expression VisitLambda<T>(Expression<T> node)
        {
            sb.Append("LAMBDA(");
            foreach (var p in node.Parameters)
            {
                sb.Append(p.Type.FullName + ";");
            }

            sb.Append(")=>");
            return base.VisitLambda(node);
        }

        override protected Expression VisitBinary(BinaryExpression node)
        {
            sb.Append($"BIN({node.NodeType})");
            return base.VisitBinary(node);
        }

        override protected Expression VisitParameter(ParameterExpression node)
        {
            sb.Append($"PARAM({node.Type.FullName})");
            return node;
        }
    }

    private sealed class CacheableExpressionVisitor : ExpressionVisitor
    {
        public bool Cacheable { get; private set; } = true;

        public static bool IsCacheable(Expression expr)
        {
            var v = new CacheableExpressionVisitor();
            v.Visit(expr);
            return v.Cacheable;
        }

        override protected Expression VisitInvocation(InvocationExpression node)
        {
            // Cannot reliably cache invocation expressions
            this.Cacheable = false;
            return node;
        }

        override protected Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            this.Cacheable = false;
            return node;
        }

        override protected Expression VisitTry(TryExpression node)
        {
            // Safe but unusual — allow it
            return base.VisitTry(node);
        }
    }

    public static class ClosureSafety
    {
        public static bool HasMutableClosure(Expression expr)
        {
            var detector = new MutableClosureDetector();
            detector.Visit(expr);
            return detector.Result;
        }

        private sealed class MutableClosureDetector : ExpressionVisitor
        {
            public bool Result { get; private set; }

            public override Expression Visit(Expression node)
            {
                if (this.Result || node is null)
                {
                    return node;
                }

                return base.Visit(node);
            }

            override protected Expression VisitMember(MemberExpression node)
            {
                if (this.Result)
                {
                    return node;
                }

                // Identify DisplayClass closure (C# compiler generated)
                if (node.Expression is ConstantExpression c &&
                    c.Type.IsNestedPrivate &&
                    c.Type.Name.Contains("DisplayClass"))
                {
                    var capturedType = node.Type;

                    if (IsMutableType(capturedType))
                    {
                        this.Result = true;
                    }
                }

                return base.VisitMember(node);
            }

            private static bool IsMutableType(Type type)
            {
                // Arrays are always mutable
                if (type.IsArray)
                {
                    return true;
                }

                // ref structs and Span<T> are stack-only, must never be cached
                if (type.IsRefStruct())
                {
                    return true;
                }

                // Classes are mutable unless proven otherwise
                if (!type.IsValueType)
                {
                    return true;
                }

                // Enums are immutable
                if (type.IsEnum)
                {
                    return false;
                }

                // Primitive value types are immutable
                if (type.IsPrimitive)
                {
                    return false;
                }

                // Decimal, DateTime, Guid are immutable structs
                if (type == typeof(decimal) ||
                    type == typeof(DateTime) ||
                    type == typeof(Guid) ||
                    type == typeof(TimeSpan))
                {
                    return false;
                }

                // Nullable<T> → check underlying type
                if (Nullable.GetUnderlyingType(type) is Type underlying)
                {
                    return IsMutableType(underlying);
                }

                // Structs are mutable unless all fields are readonly, and those fields are immutable
                return StructIsMutable(type);
            }

            private static bool StructIsMutable(Type type)
            {
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!field.IsInitOnly)
                    {
                        return true; // mutable field
                    }

                    if (IsMutableType(field.FieldType))
                    {
                        return true; // immutable wrapper around mutable type
                    }
                }

                return false; // readonly struct of immutable fields
            }
        }
    }

}