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
        // Slow-compiled delegates retain their closure instance. Even when a captured
        // value has an immutable type, another invocation can supply a different
        // closure instance and value for the same canonical expression key.
        if (ClosureSafety.HasClosure(expr))
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
        protected override Expression VisitConstant(ConstantExpression node)
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

        protected override Expression VisitMember(MemberExpression node)
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

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            sb.Append("LAMBDA(");
            foreach (var p in node.Parameters)
            {
                sb.Append(p.Type.FullName + ";");
            }

            sb.Append(")=>");
            return base.VisitLambda(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            sb.Append($"BIN({node.NodeType})");
            return base.VisitBinary(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            sb.Append($"PARAM({node.Type.FullName})");
            return node;
        }
    }

    private sealed class CacheableExpressionVisitor : ExpressionVisitor
    {
        private bool Cacheable { get; set; } = true;

        public static bool IsCacheable(Expression expr)
        {
            var v = new CacheableExpressionVisitor();
            v.Visit(expr);
            return v.Cacheable;
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            // Cannot reliably cache invocation expressions
            this.Cacheable = false;
            return node;
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            this.Cacheable = false;
            return node;
        }

        protected override Expression VisitTry(TryExpression node)
        {
            // Safe but unusual — allow it
            return base.VisitTry(node);
        }
    }

    public static class ClosureSafety
    {
        public static bool HasClosure(Expression expr)
        {
            var detector = new ClosureDetector();
            detector.Visit(expr);
            return detector.Result;
        }

        public static bool HasMutableClosure(Expression expr)
        {
            var detector = new MutableClosureDetector();
            detector.Visit(expr);
            return detector.Result;
        }

        private sealed class ClosureDetector : ExpressionVisitor
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

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (!node.Type.Name.Contains("DisplayClass"))
                {
                    return base.VisitConstant(node);
                }

                this.Result = true;
                return node;
            }
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

            protected override Expression VisitMember(MemberExpression node)
            {
                if (this.Result)
                {
                    return node;
                }

                // Identify DisplayClass closure (C# compiler generated)
                if (node.Expression is not ConstantExpression { Type.IsNestedPrivate: true } c ||
                    !c.Type.Name.Contains("DisplayClass"))
                {
                    return base.VisitMember(node);
                }

                var capturedType = node.Type;

                if (IsMutableType(capturedType))
                {
                    this.Result = true;
                }

                return base.VisitMember(node);
            }

            private static bool IsMutableType(Type type)
            {
                while (true)
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
                    if (type == typeof(decimal) || type == typeof(DateTime) || type == typeof(Guid) || type == typeof(TimeSpan))
                    {
                        return false;
                    }

                    // Nullable<T> → check underlying type
                    if (Nullable.GetUnderlyingType(type) is not { } underlying)
                    {
                        return StructIsMutable(type);
                    }

                    type = underlying;
                    continue;

                    // Structs are mutable unless all fields are readonly, and those fields are immutable
                    break;
                }
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