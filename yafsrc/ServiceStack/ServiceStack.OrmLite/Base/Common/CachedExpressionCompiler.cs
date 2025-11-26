// ***********************************************************************
// <copyright file="CachedExpressionCompiler.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ServiceStack
{
    // The caching expression tree compiler was copied from MVC core to MVC Futures so that Futures code could benefit
    // from it and so that it could be exposed as a public API. This is the only public entry point into the system.
    // See the comments in the ExpressionUtil namespace for more information.
    //
    // The unit tests for the ExpressionUtil.* types are in the System.Web.Mvc.Test project.
    public static class CachedExpressionCompiler
    {
        private readonly static ParameterExpression _unusedParameterExpr =
            Expression.Parameter(typeof(object), "_unused");

        // Implements caching around LambdaExpression.Compile() so that equivalent expression trees only have to be
        // compiled once.
        public static Func<TModel, TValue> Compile<TModel, TValue>(
            this Expression<Func<TModel, TValue>> lambdaExpression)
        {
            ArgumentNullException.ThrowIfNull(lambdaExpression);

            return ExpressionUtil.CachedExpressionCompiler.Process(lambdaExpression);
        }

        // Evaluates an expression (not a LambdaExpression), e.g. 2 + 2.
        public static object Evaluate(Expression arg)
        {
            ArgumentNullException.ThrowIfNull(arg);

            var func = Wrap(arg);
            return func(null);
        }

        private static Func<object, object> Wrap(Expression arg)
        {
            var lambdaExpr =
                Expression.Lambda<Func<object, object>>(Expression.Convert(arg, typeof(object)), _unusedParameterExpr);
            return ExpressionUtil.CachedExpressionCompiler.Process(lambdaExpr);
        }
    }
}

namespace ServiceStack.ExpressionUtil
{
    // BinaryExpression fingerprint class
    // Useful for things like array[index]

    static internal class CachedExpressionCompiler
    {
        // This is the entry point to the cached expression compilation system. The system
        // will try to turn the expression into an actual delegate as quickly as possible,
        // relying on cache lookups and other techniques to save time if appropriate.
        // If the provided expression is particularly obscure and the system doesn't know
        // how to handle it, we'll just compile the expression as normal.
        public static Func<TModel, TValue> Process<TModel, TValue>(Expression<Func<TModel, TValue>> lambdaExpression)
        {
            return Compiler<TModel, TValue>.Compile(lambdaExpression);
        }

        private static class Compiler<TIn, TOut>
        {
            private static Func<TIn, TOut> _identityFunc;
            private readonly static ConcurrentDictionary<MemberInfo, Func<TIn, TOut>> _simpleMemberAccessDict = new();
            private readonly static ConcurrentDictionary<MemberInfo, Func<object, TOut>> _constMemberAccessDict = new();

            private readonly static ConcurrentDictionary<ExpressionFingerprintChain, Hoisted<TIn, TOut>>
                _fingerprintedCache = new();

            private readonly static ConcurrentDictionary<string, Func<TIn, TOut>> _slowCompileCache = new();

            public static Func<TIn, TOut> Compile(Expression<Func<TIn, TOut>> expr)
            {
                return CompileFromIdentityFunc(expr)
                       ?? CompileFromConstLookup(expr)
                       ?? CompileFromMemberAccess(expr)
                       ?? CompileFromFingerprint(expr)
                       ?? CachedCompileSlow(expr);
            }

            private static Func<TIn, TOut> CompileFromConstLookup(Expression<Func<TIn, TOut>> expr)
            {
                if (expr.Body is ConstantExpression constExpr)
                {
                    // model => {const}

                    var constantValue = (TOut)constExpr.Value;
                    return _ => constantValue;
                }

                return null;
            }

            private static Func<TIn, TOut> CompileFromIdentityFunc(Expression<Func<TIn, TOut>> expr)
            {
                if (expr.Body == expr.Parameters[0])
                {
                    // model => model

                    // don't need to lock, as all identity funcs are identical
                    if (_identityFunc == null)
                    {
                        _identityFunc = expr.Compile();
                    }

                    return _identityFunc;
                }

                return null;
            }

            private static Func<TIn, TOut> CompileFromFingerprint(Expression<Func<TIn, TOut>> expr)
            {
                var fingerprint =
                    FingerprintingExpressionVisitor.GetFingerprintChain(expr, out var capturedConstants);

                if (fingerprint != null)
                {
                    var del = _fingerprintedCache.GetOrAdd(fingerprint, _ =>
                    {
                        // Fingerprinting succeeded, but there was a cache miss. Rewrite the expression
                        // and add the rewritten expression to the cache.

                        var hoistedExpr = HoistingExpressionVisitor<TIn, TOut>.Hoist(expr);
                        return hoistedExpr.Compile();
                    });
                    return model => del(model, capturedConstants);
                }

                // couldn't be fingerprinted
                return null;
            }

            private static Func<TIn, TOut> CompileFromMemberAccess(Expression<Func<TIn, TOut>> expr)
            {
                // Performance tests show that on the x64 platform, special-casing static member and
                // captured local variable accesses is faster than letting the fingerprinting system
                // handle them. On the x86 platform, the fingerprinting system is faster, but only
                // by around one microsecond, so it's not worth it to complicate the logic here with
                // an architecture check.

                if (expr.Body is MemberExpression memberExpr)
                {
                    if (memberExpr.Expression == expr.Parameters[0] || memberExpr.Expression == null)
                    {
                        // model => model.Member or model => StaticMember
                        return _simpleMemberAccessDict.GetOrAdd(memberExpr.Member, _ => expr.Compile());
                    }

                    if (memberExpr.Expression is ConstantExpression constExpr)
                    {
                        // model => {const}.Member (captured local variable)
                        var del = _constMemberAccessDict.GetOrAdd(memberExpr.Member, _ =>
                        {
                            // rewrite as capturedLocal => ((TDeclaringType)capturedLocal).Member
                            var constParamExpr = Expression.Parameter(typeof(object), "capturedLocal");
                            var constCastExpr = Expression.Convert(constParamExpr, memberExpr.Member.DeclaringType);
                            var newMemberAccessExpr = memberExpr.Update(constCastExpr);
                            var newLambdaExpr =
                                Expression.Lambda<Func<object, TOut>>(newMemberAccessExpr, constParamExpr);
                            return newLambdaExpr.Compile();
                        });

                        var capturedLocal = constExpr.Value;
                        return _ => del(capturedLocal);
                    }
                }

                return null;
            }

            private static Func<TIn, TOut> CachedCompileSlow(Expression<Func<TIn, TOut>> expr)
            {
                if (ExpressionCacheKey.TryGetKey(expr, out var key))
                {
                    return _slowCompileCache.GetOrAdd(key, _ => CompileSlow(expr));
                }
                return CompileSlow(expr);
            }

            private static Func<TIn, TOut> CompileSlow(Expression<Func<TIn, TOut>> expr)
            {
#if DEBUG
                Console.WriteLine("SlowCompile");
#endif

                // fallback compilation system - just compile the expression directly
                try
                {
                    // Try to handle invalid ref struct boxing (e.g. Span<T> implicit conversion)
                    if (expr.Body is UnaryExpression { NodeType: ExpressionType.Convert } u
                        && u.Type == typeof(object)
                        && u.Operand is MethodCallExpression { Method.Name: "op_Implicit" } m)
                    {
                        var returnType = m.Method.ReturnType;
                        if (returnType.IsRefStruct())
                        {
                            try
                            {
                                // Strip the implicit conversion and try compiling the inner expression
                                var inner = m.Arguments[0];
                                var newBody = Expression.Convert(inner, typeof(object));
                                var newLambda = Expression.Lambda<Func<TIn, TOut>>(newBody, expr.Parameters);
                                return newLambda.Compile();
                            }
                            catch
                            {
                                // If recovery fails, ignore and throw original
                            }
                        }
                    }

                    return expr.Compile();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }

    internal sealed class ConditionalExpressionFingerprint(ExpressionType nodeType, Type type)
        : ExpressionFingerprint(nodeType, type)
    {
        // There are no properties on ConditionalExpression that are worth including in
        // the fingerprint.

        public override bool Equals(object obj)
        {
            var other = obj as ConditionalExpressionFingerprint;
            return (other != null)
                   && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal sealed class ConstantExpressionFingerprint(ExpressionType nodeType, Type type)
        : ExpressionFingerprint(nodeType, type)
    {
        // There are no properties on ConstantExpression that are worth including in
        // the fingerprint.

        public override bool Equals(object obj)
        {
            var other = obj as ConstantExpressionFingerprint;
            return (other != null)
                   && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal sealed class DefaultExpressionFingerprint(ExpressionType nodeType, Type type)
        : ExpressionFingerprint(nodeType, type)
    {
        // There are no properties on DefaultExpression that are worth including in
        // the fingerprint.

        public override bool Equals(object obj)
        {
            var other = obj as DefaultExpressionFingerprint;
            return (other != null)
                   && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal abstract class ExpressionFingerprint(ExpressionType nodeType, Type type)
    {
        // the type of expression node, e.g. OP_ADD, MEMBER_ACCESS, etc.
        public ExpressionType NodeType { get; private set; } = nodeType;

        // the CLR type resulting from this expression, e.g. int, string, etc.
        public Type Type { get; private set; } = type;

        internal virtual void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddInt32((int)NodeType);
            combiner.AddObject(Type);
        }

        protected bool Equals(ExpressionFingerprint other)
        {
            return (other != null)
                   && (this.NodeType == other.NodeType)
                   && Equals(this.Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExpressionFingerprint);
        }

        public override int GetHashCode()
        {
            var combiner = new HashCodeCombiner();
            AddToHashCodeCombiner(combiner);
            return combiner.CombinedHash;
        }
    }

    internal sealed class ExpressionFingerprintChain : IEquatable<ExpressionFingerprintChain>
    {
        public readonly List<ExpressionFingerprint> Elements = new();
        public bool Equals(ExpressionFingerprintChain other)
        {
            // Two chains are considered equal if two elements appearing in the same index in
            // each chain are equal (value equality, not referential equality).
            if (other == null)
            {
                return false;
            }

            if (this.Elements.Count != other.Elements.Count)
            {
                return false;
            }

            for (var i = 0; i < this.Elements.Count; i++)
            {
                if (!Equals(this.Elements[i], other.Elements[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExpressionFingerprintChain);
        }

        public override int GetHashCode()
        {
            var combiner = new HashCodeCombiner();
            Elements.ForEach(combiner.AddFingerprint);
            return combiner.CombinedHash;
        }
    }

    internal sealed class FingerprintingExpressionVisitor : ExpressionVisitor
    {
        private readonly List<object> _seenConstants = new();
        private readonly List<ParameterExpression> _seenParameters = new();
        private readonly ExpressionFingerprintChain _currentChain = new();
        private bool _gaveUp;

        private FingerprintingExpressionVisitor()
        {
        }

        private T GiveUp<T>(T node)
        {
            // We don't understand this node, so just quit.

            _gaveUp = true;
            return node;
        }

        // Returns the fingerprint chain + captured constants list for this expression, or null
        // if the expression couldn't be fingerprinted.
        public static ExpressionFingerprintChain GetFingerprintChain(Expression expr,
            out List<object> capturedConstants)
        {
            var visitor = new FingerprintingExpressionVisitor();
            visitor.Visit(expr);

            if (visitor._gaveUp)
            {
                capturedConstants = null;
                return null;
            }
            else
            {
                capturedConstants = visitor._seenConstants;
                return visitor._currentChain;
            }
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
            {
                _currentChain.Elements.Add(null);
                return null;
            }
            else
            {
                return base.Visit(node);
            }
        }

        override protected Expression VisitBinary(BinaryExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            _currentChain.Elements.Add(new BinaryExpressionFingerprint(node.NodeType, node.Type, node.Method));
            return base.VisitBinary(node);
        }

        override protected Expression VisitBlock(BlockExpression node)
        {
            return GiveUp(node);
        }

        override protected CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitConditional(ConditionalExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            _currentChain.Elements.Add(new ConditionalExpressionFingerprint(node.NodeType, node.Type));
            return base.VisitConditional(node);
        }

        override protected Expression VisitConstant(ConstantExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            _seenConstants.Add(node.Value);
            _currentChain.Elements.Add(new ConstantExpressionFingerprint(node.NodeType, node.Type));
            return base.VisitConstant(node);
        }

        override protected Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitDefault(DefaultExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            _currentChain.Elements.Add(new DefaultExpressionFingerprint(node.NodeType, node.Type));
            return base.VisitDefault(node);
        }

        override protected Expression VisitDynamic(DynamicExpression node)
        {
            return GiveUp(node);
        }

        override protected ElementInit VisitElementInit(ElementInit node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitExtension(Expression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitGoto(GotoExpression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitIndex(IndexExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            _currentChain.Elements.Add(new IndexExpressionFingerprint(node.NodeType, node.Type, node.Indexer));
            return base.VisitIndex(node);
        }

        override protected Expression VisitInvocation(InvocationExpression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitLabel(LabelExpression node)
        {
            return GiveUp(node);
        }

        override protected LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitLambda<T>(Expression<T> node)
        {
            if (_gaveUp)
            {
                return node;
            }

            _currentChain.Elements.Add(new LambdaExpressionFingerprint(node.NodeType, node.Type));
            return base.VisitLambda(node);
        }

        override protected Expression VisitListInit(ListInitExpression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitLoop(LoopExpression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitMember(MemberExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            _currentChain.Elements.Add(new MemberExpressionFingerprint(node.NodeType, node.Type, node.Member));
            return base.VisitMember(node);
        }

        override protected MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return GiveUp(node);
        }

        override protected MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitMemberInit(MemberInitExpression node)
        {
            return GiveUp(node);
        }

        override protected MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return GiveUp(node);
        }

        override protected MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitMethodCall(MethodCallExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            if (node.Type.IsRefStruct())
            {
                return GiveUp(node);
            }

            _currentChain.Elements.Add(new MethodCallExpressionFingerprint(node.NodeType, node.Type, node.Method));
            return base.VisitMethodCall(node);
        }

        override protected Expression VisitNew(NewExpression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitNewArray(NewArrayExpression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitParameter(ParameterExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            var parameterIndex = _seenParameters.IndexOf(node);
            if (parameterIndex < 0)
            {
                // first time seeing this parameter
                parameterIndex = _seenParameters.Count;
                _seenParameters.Add(node);
            }

            _currentChain.Elements.Add(new ParameterExpressionFingerprint(node.NodeType, node.Type, parameterIndex));
            return base.VisitParameter(node);
        }

        override protected Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitSwitch(SwitchExpression node)
        {
            return GiveUp(node);
        }

        override protected SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitTry(TryExpression node)
        {
            return GiveUp(node);
        }

        override protected Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            _currentChain.Elements.Add(new TypeBinaryExpressionFingerprint(node.NodeType, node.Type, node.TypeOperand));
            return base.VisitTypeBinary(node);
        }

        override protected Expression VisitUnary(UnaryExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            if (node.Type.IsRefStruct())
            {
                return GiveUp(node);
            }

            _currentChain.Elements.Add(new UnaryExpressionFingerprint(node.NodeType, node.Type, node.Method));
            return base.VisitUnary(node);
        }
    }

    internal class HashCodeCombiner
    {
        private long _combinedHash64 = 0x1505L;
        public int CombinedHash => _combinedHash64.GetHashCode();

        public void AddFingerprint(ExpressionFingerprint fingerprint)
        {
            if (fingerprint != null)
            {
                fingerprint.AddToHashCodeCombiner(this);
            }
            else
            {
                AddInt32(0);
            }
        }

        public void AddEnumerable(IEnumerable e)
        {
            if (e == null)
            {
                AddInt32(0);
            }
            else
            {
                var count = 0;
                foreach (var o in e)
                {
                    AddObject(o);
                    count++;
                }

                AddInt32(count);
            }
        }

        public void AddInt32(int i)
        {
            _combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;
        }

        public void AddObject(object o)
        {
            var hashCode = (o != null) ? o.GetHashCode() : 0;
            AddInt32(hashCode);
        }
    }

    internal delegate TValue Hoisted<in TModel, out TValue>(TModel model, List<object> capturedConstants);

    internal sealed class HoistingExpressionVisitor<TIn, TOut> : ExpressionVisitor
    {
        private readonly static ParameterExpression _hoistedConstantsParamExpr =
            Expression.Parameter(typeof(List<object>), "hoistedConstants");

        private int _numConstantsProcessed;

        // factory will create instance
        private HoistingExpressionVisitor()
        {
        }

        public static Expression<Hoisted<TIn, TOut>> Hoist(Expression<Func<TIn, TOut>> expr)
        {
            // rewrite Expression<Func<TIn, TOut>> as Expression<Hoisted<TIn, TOut>>

            var visitor = new HoistingExpressionVisitor<TIn, TOut>();
            var rewrittenBodyExpr = visitor.Visit(expr.Body);
            var rewrittenLambdaExpr =
                Expression.Lambda<Hoisted<TIn, TOut>>(rewrittenBodyExpr, expr.Parameters[0],
                    _hoistedConstantsParamExpr);
            return rewrittenLambdaExpr;
        }

        override protected Expression VisitConstant(ConstantExpression node)
        {
            // rewrite the constant expression as (TConst)hoistedConstants[i];
            return Expression.Convert(
                Expression.Property(_hoistedConstantsParamExpr, "Item", Expression.Constant(_numConstantsProcessed++)),
                node.Type);
        }
    }

    internal sealed class IndexExpressionFingerprint : ExpressionFingerprint
    {
        public IndexExpressionFingerprint(ExpressionType nodeType, Type type, PropertyInfo indexer)
            : base(nodeType, type)
        {
            // Other properties on IndexExpression (like the argument count) are simply derived
            // from Type and Indexer, so they're not necessary for inclusion in the fingerprint.

            Indexer = indexer;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.indexexpression.indexer.aspx
        public PropertyInfo Indexer { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as IndexExpressionFingerprint;
            return (other != null)
                   && Equals(this.Indexer, other.Indexer)
                   && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        override internal void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(Indexer);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    internal sealed class LambdaExpressionFingerprint : ExpressionFingerprint
    {
        public LambdaExpressionFingerprint(ExpressionType nodeType, Type type)
            : base(nodeType, type)
        {
            // There are no properties on LambdaExpression that are worth including in
            // the fingerprint.
        }

        public override bool Equals(object obj)
        {
            var other = obj as LambdaExpressionFingerprint;
            return (other != null)
                   && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal sealed class MemberExpressionFingerprint : ExpressionFingerprint
    {
        public MemberExpressionFingerprint(ExpressionType nodeType, Type type, MemberInfo member)
            : base(nodeType, type)
        {
            Member = member;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.memberexpression.member.aspx
        public MemberInfo Member { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as MemberExpressionFingerprint;
            return (other != null)
                   && Equals(this.Member, other.Member)
                   && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        override internal void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(Member);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    internal sealed class MethodCallExpressionFingerprint : ExpressionFingerprint
    {
        public MethodCallExpressionFingerprint(ExpressionType nodeType, Type type, MethodInfo method)
            : base(nodeType, type)
        {
            // Other properties on MethodCallExpression (like the argument count) are simply derived
            // from Type and Indexer, so they're not necessary for inclusion in the fingerprint.

            Method = method;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.methodcallexpression.method.aspx
        public MethodInfo Method { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as MethodCallExpressionFingerprint;
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
            combiner.AddObject(Method);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    internal sealed class ParameterExpressionFingerprint : ExpressionFingerprint
    {
        public ParameterExpressionFingerprint(ExpressionType nodeType, Type type, int parameterIndex)
            : base(nodeType, type)
        {
            ParameterIndex = parameterIndex;
        }

        // Parameter position within the overall expression, used to maintain alpha equivalence.
        public int ParameterIndex { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as ParameterExpressionFingerprint;
            return (other != null)
                   && (this.ParameterIndex == other.ParameterIndex)
                   && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        override internal void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddInt32(ParameterIndex);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    internal sealed class TypeBinaryExpressionFingerprint : ExpressionFingerprint
    {
        public TypeBinaryExpressionFingerprint(ExpressionType nodeType, Type type, Type typeOperand)
            : base(nodeType, type)
        {
            TypeOperand = typeOperand;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.typebinaryexpression.typeoperand.aspx
        public Type TypeOperand { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as TypeBinaryExpressionFingerprint;
            return (other != null)
                   && Equals(this.TypeOperand, other.TypeOperand)
                   && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        override internal void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(TypeOperand);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    internal sealed class UnaryExpressionFingerprint : ExpressionFingerprint
    {
        public UnaryExpressionFingerprint(ExpressionType nodeType, Type type, MethodInfo method)
            : base(nodeType, type)
        {
            // Other properties on UnaryExpression (like IsLifted / IsLiftedToNull) are simply derived
            // from Type and NodeType, so they're not necessary for inclusion in the fingerprint.

            Method = method;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.unaryexpression.method.aspx
        public MethodInfo Method { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as UnaryExpressionFingerprint;
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
            combiner.AddObject(Method);
            base.AddToHashCodeCombiner(combiner);
        }
    }
}