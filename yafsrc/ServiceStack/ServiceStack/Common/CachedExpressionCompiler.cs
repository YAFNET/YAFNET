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
    /// <summary>
    /// Class CachedExpressionCompiler.
    /// </summary>
    public static class CachedExpressionCompiler
    {
        /// <summary>
        /// The unused parameter expr
        /// </summary>
        private static readonly ParameterExpression _unusedParameterExpr = Expression.Parameter(typeof(object), "_unused");

        // Implements caching around LambdaExpression.Compile() so that equivalent expression trees only have to be
        // compiled once.
        /// <summary>
        /// Compiles the specified lambda expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="lambdaExpression">The lambda expression.</param>
        /// <returns>Func&lt;TModel, TValue&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">lambdaExpression</exception>
        public static Func<TModel, TValue> Compile<TModel, TValue>(this Expression<Func<TModel, TValue>> lambdaExpression)
        {
            if (lambdaExpression == null)
                throw new ArgumentNullException(nameof(lambdaExpression));

            return ExpressionUtil.CachedExpressionCompiler.Process(lambdaExpression);
        }

        // Evaluates an expression (not a LambdaExpression), e.g. 2 + 2.
        /// <summary>
        /// Evaluates the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.ArgumentNullException">arg</exception>
        public static object Evaluate(Expression arg)
        {
            if (arg == null)
                throw new ArgumentNullException(nameof(arg));

            Func<object, object> func = Wrap(arg);
            return func(null);
        }

        /// <summary>
        /// Wraps the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>Func&lt;System.Object, System.Object&gt;.</returns>
        private static Func<object, object> Wrap(Expression arg)
        {
            Expression<Func<object, object>> lambdaExpr = Expression.Lambda<Func<object, object>>(Expression.Convert(arg, typeof(object)), _unusedParameterExpr);
            return ExpressionUtil.CachedExpressionCompiler.Process(lambdaExpr);
        }
    }
}

namespace ServiceStack.ExpressionUtil
{

    // BinaryExpression fingerprint class
    // Useful for things like array[index]

    /// <summary>
    /// Class BinaryExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class BinaryExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        /// <param name="method">The method.</param>
        public BinaryExpressionFingerprint(ExpressionType nodeType, Type type, MethodInfo method)
            : base(nodeType, type)
        {
            // Other properties on BinaryExpression (like IsLifted / IsLiftedToNull) are simply derived
            // from Type and NodeType, so they're not necessary for inclusion in the fingerprint.

            Method = method;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.binaryexpression.method.aspx
        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method.</value>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is BinaryExpressionFingerprint other
                   && Equals(this.Method, other.Method)
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Adds to hash code combiner.
        /// </summary>
        /// <param name="combiner">The combiner.</param>
        internal override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(Method);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    /// <summary>
    /// Class CachedExpressionCompiler.
    /// </summary>
    internal static class CachedExpressionCompiler
    {
        // This is the entry point to the cached expression compilation system. The system
        // will try to turn the expression into an actual delegate as quickly as possible,
        // relying on cache lookups and other techniques to save time if appropriate.
        // If the provided expression is particularly obscure and the system doesn't know
        // how to handle it, we'll just compile the expression as normal.
        /// <summary>
        /// Processes the specified lambda expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="lambdaExpression">The lambda expression.</param>
        /// <returns>Func&lt;TModel, TValue&gt;.</returns>
        public static Func<TModel, TValue> Process<TModel, TValue>(Expression<Func<TModel, TValue>> lambdaExpression)
        {
            return Compiler<TModel, TValue>.Compile(lambdaExpression);
        }

        /// <summary>
        /// Class Compiler.
        /// </summary>
        /// <typeparam name="TIn">The type of the t in.</typeparam>
        /// <typeparam name="TOut">The type of the t out.</typeparam>
        private static class Compiler<TIn, TOut>
        {
            /// <summary>
            /// The identity function
            /// </summary>
            private static Func<TIn, TOut> _identityFunc;

            /// <summary>
            /// The simple member access dictionary
            /// </summary>
            private static readonly ConcurrentDictionary<MemberInfo, Func<TIn, TOut>> _simpleMemberAccessDict =
                new ConcurrentDictionary<MemberInfo, Func<TIn, TOut>>();

            /// <summary>
            /// The constant member access dictionary
            /// </summary>
            private static readonly ConcurrentDictionary<MemberInfo, Func<object, TOut>> _constMemberAccessDict =
                new ConcurrentDictionary<MemberInfo, Func<object, TOut>>();

            /// <summary>
            /// The fingerprinted cache
            /// </summary>
            private static readonly ConcurrentDictionary<ExpressionFingerprintChain, Hoisted<TIn, TOut>> _fingerprintedCache =
                new ConcurrentDictionary<ExpressionFingerprintChain, Hoisted<TIn, TOut>>();

            /// <summary>
            /// Compiles the specified expr.
            /// </summary>
            /// <param name="expr">The expr.</param>
            /// <returns>Func&lt;TIn, TOut&gt;.</returns>
            public static Func<TIn, TOut> Compile(Expression<Func<TIn, TOut>> expr)
            {
                return CompileFromIdentityFunc(expr)
                       ?? CompileFromConstLookup(expr)
                       ?? CompileFromMemberAccess(expr)
                       ?? CompileFromFingerprint(expr)
                       ?? CompileSlow(expr);
            }

            /// <summary>
            /// Compiles from constant lookup.
            /// </summary>
            /// <param name="expr">The expr.</param>
            /// <returns>Func&lt;TIn, TOut&gt;.</returns>
            private static Func<TIn, TOut> CompileFromConstLookup(Expression<Func<TIn, TOut>> expr)
            {
                if (expr.Body is ConstantExpression constExpr)
                {
                    // model => {const}

                    TOut constantValue = (TOut)constExpr.Value;
                    return _ => constantValue;
                }

                return null;
            }

            /// <summary>
            /// Compiles from identity function.
            /// </summary>
            /// <param name="expr">The expr.</param>
            /// <returns>Func&lt;TIn, TOut&gt;.</returns>
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

            /// <summary>
            /// Compiles from fingerprint.
            /// </summary>
            /// <param name="expr">The expr.</param>
            /// <returns>Func&lt;TIn, TOut&gt;.</returns>
            private static Func<TIn, TOut> CompileFromFingerprint(Expression<Func<TIn, TOut>> expr)
            {
                List<object> capturedConstants;
                ExpressionFingerprintChain fingerprint = FingerprintingExpressionVisitor.GetFingerprintChain(expr, out capturedConstants);

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

            /// <summary>
            /// Compiles from member access.
            /// </summary>
            /// <param name="expr">The expr.</param>
            /// <returns>Func&lt;TIn, TOut&gt;.</returns>
            private static Func<TIn, TOut> CompileFromMemberAccess(Expression<Func<TIn, TOut>> expr)
            {
                // Performance tests show that on the x64 platform, special-casing static member and
                // captured local variable accesses is faster than letting the fingerprinting system
                // handle them. On the x86 platform, the fingerprinting system is faster, but only
                // by around one microsecond, so it's not worth it to complicate the logic here with
                // an architecture check.

                MemberExpression memberExpr = expr.Body as MemberExpression;
                if (memberExpr != null)
                {
                    if (memberExpr.Expression == expr.Parameters[0] || memberExpr.Expression == null)
                    {
                        // model => model.Member or model => StaticMember
                        return _simpleMemberAccessDict.GetOrAdd(memberExpr.Member, _ => expr.Compile());
                    }

                    ConstantExpression constExpr = memberExpr.Expression as ConstantExpression;
                    if (constExpr != null)
                    {
                        // model => {const}.Member (captured local variable)
                        var del = _constMemberAccessDict.GetOrAdd(memberExpr.Member, _ =>
                        {
                            // rewrite as capturedLocal => ((TDeclaringType)capturedLocal).Member
                            var constParamExpr = Expression.Parameter(typeof(object), "capturedLocal");
                            var constCastExpr = Expression.Convert(constParamExpr, memberExpr.Member.DeclaringType);
                            var newMemberAccessExpr = memberExpr.Update(constCastExpr);
                            var newLambdaExpr = Expression.Lambda<Func<object, TOut>>(newMemberAccessExpr, constParamExpr);
                            return newLambdaExpr.Compile();
                        });

                        object capturedLocal = constExpr.Value;
                        return _ => del(capturedLocal);
                    }
                }

                return null;
            }

            /// <summary>
            /// Compiles the slow.
            /// </summary>
            /// <param name="expr">The expr.</param>
            /// <returns>Func&lt;TIn, TOut&gt;.</returns>
            private static Func<TIn, TOut> CompileSlow(Expression<Func<TIn, TOut>> expr)
            {
                // fallback compilation system - just compile the expression directly
                return expr.Compile();
            }
        }
    }

    /// <summary>
    /// Class ConditionalExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class ConditionalExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        public ConditionalExpressionFingerprint(ExpressionType nodeType, Type type)
            : base(nodeType, type)
        {
            // There are no properties on ConditionalExpression that are worth including in
            // the fingerprint.
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            ConditionalExpressionFingerprint other = obj as ConditionalExpressionFingerprint;
            return other != null
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Class ConstantExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class ConstantExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        public ConstantExpressionFingerprint(ExpressionType nodeType, Type type)
            : base(nodeType, type)
        {
            // There are no properties on ConstantExpression that are worth including in
            // the fingerprint.
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            ConstantExpressionFingerprint other = obj as ConstantExpressionFingerprint;
            return other != null
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Class DefaultExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class DefaultExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        public DefaultExpressionFingerprint(ExpressionType nodeType, Type type)
            : base(nodeType, type)
        {
            // There are no properties on DefaultExpression that are worth including in
            // the fingerprint.
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            DefaultExpressionFingerprint other = obj as DefaultExpressionFingerprint;
            return other != null
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Class ExpressionFingerprint.
    /// </summary>
    internal abstract class ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        protected ExpressionFingerprint(ExpressionType nodeType, Type type)
        {
            NodeType = nodeType;
            Type = type;
        }

        // the type of expression node, e.g. OP_ADD, MEMBER_ACCESS, etc.
        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        public ExpressionType NodeType { get; private set; }

        // the CLR type resulting from this expression, e.g. int, string, etc.
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>
        /// Adds to hash code combiner.
        /// </summary>
        /// <param name="combiner">The combiner.</param>
        internal virtual void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddInt32((int)NodeType);
            combiner.AddObject(Type);
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool Equals(ExpressionFingerprint other)
        {
            return other != null
                   && this.NodeType == other.NodeType
                   && this.Type == other.Type;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ExpressionFingerprint);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            HashCodeCombiner combiner = new HashCodeCombiner();
            AddToHashCodeCombiner(combiner);
            return combiner.CombinedHash;
        }
    }

    /// <summary>
    /// Class ExpressionFingerprintChain. This class cannot be inherited.
    /// Implements the <see cref="ExpressionFingerprintChain" />
    /// </summary>
    /// <seealso cref="ExpressionFingerprintChain" />
    internal sealed class ExpressionFingerprintChain : IEquatable<ExpressionFingerprintChain>
    {
        /// <summary>
        /// The elements
        /// </summary>
        public readonly List<ExpressionFingerprint> Elements = new List<ExpressionFingerprint>();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
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

            for (int i = 0; i < this.Elements.Count; i++)
            {
                if (!Equals(this.Elements[i], other.Elements[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ExpressionFingerprintChain);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            HashCodeCombiner combiner = new HashCodeCombiner();
            Elements.ForEach(combiner.AddFingerprint);

            return combiner.CombinedHash;
        }
    }

    /// <summary>
    /// Class FingerprintingExpressionVisitor. This class cannot be inherited.
    /// Implements the <see cref="System.Linq.Expressions.ExpressionVisitor" />
    /// </summary>
    /// <seealso cref="System.Linq.Expressions.ExpressionVisitor" />
    internal sealed class FingerprintingExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// The seen constants
        /// </summary>
        private readonly List<object> _seenConstants = new List<object>();
        /// <summary>
        /// The seen parameters
        /// </summary>
        private readonly List<ParameterExpression> _seenParameters = new List<ParameterExpression>();
        /// <summary>
        /// The current chain
        /// </summary>
        private readonly ExpressionFingerprintChain _currentChain = new ExpressionFingerprintChain();
        /// <summary>
        /// The gave up
        /// </summary>
        private bool _gaveUp;

        /// <summary>
        /// Prevents a default instance of the <see cref="FingerprintingExpressionVisitor"/> class from being created.
        /// </summary>
        private FingerprintingExpressionVisitor()
        {
        }

        /// <summary>
        /// Gives up.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">The node.</param>
        /// <returns>T.</returns>
        private T GiveUp<T>(T node)
        {
            // We don't understand this node, so just quit.

            _gaveUp = true;
            return node;
        }

        // Returns the fingerprint chain + captured constants list for this expression, or null
        // if the expression couldn't be fingerprinted.
        /// <summary>
        /// Gets the fingerprint chain.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <param name="capturedConstants">The captured constants.</param>
        /// <returns>ExpressionFingerprintChain.</returns>
        public static ExpressionFingerprintChain GetFingerprintChain(Expression expr, out List<object> capturedConstants)
        {
            FingerprintingExpressionVisitor visitor = new FingerprintingExpressionVisitor();
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

        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
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

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }
            _currentChain.Elements.Add(new BinaryExpressionFingerprint(node.NodeType, node.Type, node.Method));
            return base.VisitBinary(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.BlockExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitBlock(BlockExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.CatchBlock" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.ConditionalExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }
            _currentChain.Elements.Add(new ConditionalExpressionFingerprint(node.NodeType, node.Type));
            return base.VisitConditional(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            _seenConstants.Add(node.Value);
            _currentChain.Elements.Add(new ConstantExpressionFingerprint(node.NodeType, node.Type));
            return base.VisitConstant(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.DebugInfoExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.DefaultExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitDefault(DefaultExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }
            _currentChain.Elements.Add(new DefaultExpressionFingerprint(node.NodeType, node.Type));
            return base.VisitDefault(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.DynamicExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitDynamic(DynamicExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.ElementInit" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override ElementInit VisitElementInit(ElementInit node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the extension expression.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitExtension(Expression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.GotoExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitGoto(GotoExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.IndexExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitIndex(IndexExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }
            _currentChain.Elements.Add(new IndexExpressionFingerprint(node.NodeType, node.Type, node.Indexer));
            return base.VisitIndex(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.InvocationExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.LabelExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitLabel(LabelExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.LabelTarget" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1" />.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (_gaveUp)
            {
                return node;
            }
            _currentChain.Elements.Add(new LambdaExpressionFingerprint(node.NodeType, node.Type));
            return base.VisitLambda(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.ListInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitListInit(ListInitExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.LoopExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitLoop(LoopExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }
            _currentChain.Elements.Add(new MemberExpressionFingerprint(node.NodeType, node.Type, node.Member));
            return base.VisitMember(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberAssignment" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberListBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberMemberBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }
            _currentChain.Elements.Add(new MethodCallExpressionFingerprint(node.NodeType, node.Type, node.Method));
            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitNew(NewExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.NewArrayExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }

            int parameterIndex = _seenParameters.IndexOf(node);
            if (parameterIndex < 0)
            {
                // first time seeing this parameter
                parameterIndex = _seenParameters.Count;
                _seenParameters.Add(node);
            }

            _currentChain.Elements.Add(new ParameterExpressionFingerprint(node.NodeType, node.Type, parameterIndex));
            return base.VisitParameter(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitSwitch(SwitchExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchCase" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.TryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitTry(TryExpression node)
        {
            return GiveUp(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.TypeBinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }
            _currentChain.Elements.Add(new TypeBinaryExpressionFingerprint(node.NodeType, node.Type, node.TypeOperand));
            return base.VisitTypeBinary(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (_gaveUp)
            {
                return node;
            }
            _currentChain.Elements.Add(new UnaryExpressionFingerprint(node.NodeType, node.Type, node.Method));
            return base.VisitUnary(node);
        }
    }

    /// <summary>
    /// Class HashCodeCombiner.
    /// </summary>
    internal class HashCodeCombiner
    {
        /// <summary>
        /// The combined hash64
        /// </summary>
        private long _combinedHash64 = 0x1505L;

        /// <summary>
        /// Gets the combined hash.
        /// </summary>
        /// <value>The combined hash.</value>
        public int CombinedHash
        {
            get { return _combinedHash64.GetHashCode(); }
        }

        /// <summary>
        /// Adds the fingerprint.
        /// </summary>
        /// <param name="fingerprint">The fingerprint.</param>
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

        /// <summary>
        /// Adds the enumerable.
        /// </summary>
        /// <param name="e">The e.</param>
        public void AddEnumerable(IEnumerable e)
        {
            if (e == null)
            {
                AddInt32(0);
            }
            else
            {
                int count = 0;
                foreach (object o in e)
                {
                    AddObject(o);
                    count++;
                }
                AddInt32(count);
            }
        }

        /// <summary>
        /// Adds the int32.
        /// </summary>
        /// <param name="i">The i.</param>
        public void AddInt32(int i)
        {
            _combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;
        }

        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="o">The o.</param>
        public void AddObject(object o)
        {
            int hashCode = o != null ? o.GetHashCode() : 0;
            AddInt32(hashCode);
        }
    }

    /// <summary>
    /// Delegate Hoisted
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="model">The model.</param>
    /// <param name="capturedConstants">The captured constants.</param>
    /// <returns>TValue.</returns>
    internal delegate TValue Hoisted<in TModel, out TValue>(TModel model, List<object> capturedConstants);

    /// <summary>
    /// Class HoistingExpressionVisitor. This class cannot be inherited.
    /// Implements the <see cref="System.Linq.Expressions.ExpressionVisitor" />
    /// </summary>
    /// <typeparam name="TIn">The type of the t in.</typeparam>
    /// <typeparam name="TOut">The type of the t out.</typeparam>
    /// <seealso cref="System.Linq.Expressions.ExpressionVisitor" />
    internal sealed class HoistingExpressionVisitor<TIn, TOut> : ExpressionVisitor
    {
        /// <summary>
        /// The hoisted constants parameter expr
        /// </summary>
        private static readonly ParameterExpression _hoistedConstantsParamExpr = Expression.Parameter(typeof(List<object>), "hoistedConstants");
        /// <summary>
        /// The number constants processed
        /// </summary>
        private int _numConstantsProcessed;

        // factory will create instance
        /// <summary>
        /// Prevents a default instance of the <see cref="HoistingExpressionVisitor{TIn, TOut}"/> class from being created.
        /// </summary>
        private HoistingExpressionVisitor()
        {
        }

        /// <summary>
        /// Hoists the specified expr.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns>Expression&lt;Hoisted&lt;TIn, TOut&gt;&gt;.</returns>
        public static Expression<Hoisted<TIn, TOut>> Hoist(Expression<Func<TIn, TOut>> expr)
        {
            // rewrite Expression<Func<TIn, TOut>> as Expression<Hoisted<TIn, TOut>>

            var visitor = new HoistingExpressionVisitor<TIn, TOut>();
            var rewrittenBodyExpr = visitor.Visit(expr.Body);
            var rewrittenLambdaExpr = Expression.Lambda<Hoisted<TIn, TOut>>(rewrittenBodyExpr, expr.Parameters[0], _hoistedConstantsParamExpr);
            return rewrittenLambdaExpr;
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            // rewrite the constant expression as (TConst)hoistedConstants[i];
            return Expression.Convert(Expression.Property(_hoistedConstantsParamExpr, "Item", Expression.Constant(_numConstantsProcessed++)), node.Type);
        }
    }

    /// <summary>
    /// Class IndexExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class IndexExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        /// <param name="indexer">The indexer.</param>
        public IndexExpressionFingerprint(ExpressionType nodeType, Type type, PropertyInfo indexer)
            : base(nodeType, type)
        {
            // Other properties on IndexExpression (like the argument count) are simply derived
            // from Type and Indexer, so they're not necessary for inclusion in the fingerprint.

            Indexer = indexer;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.indexexpression.indexer.aspx
        /// <summary>
        /// Gets the indexer.
        /// </summary>
        /// <value>The indexer.</value>
        public PropertyInfo Indexer { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            IndexExpressionFingerprint other = obj as IndexExpressionFingerprint;
            return other != null
                   && Equals(this.Indexer, other.Indexer)
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Adds to hash code combiner.
        /// </summary>
        /// <param name="combiner">The combiner.</param>
        internal override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(Indexer);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    /// <summary>
    /// Class LambdaExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class LambdaExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        public LambdaExpressionFingerprint(ExpressionType nodeType, Type type)
            : base(nodeType, type)
        {
            // There are no properties on LambdaExpression that are worth including in
            // the fingerprint.
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            LambdaExpressionFingerprint other = obj as LambdaExpressionFingerprint;
            return other != null
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Class MemberExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class MemberExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        /// <param name="member">The member.</param>
        public MemberExpressionFingerprint(ExpressionType nodeType, Type type, MemberInfo member)
            : base(nodeType, type)
        {
            Member = member;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.memberexpression.member.aspx
        /// <summary>
        /// Gets the member.
        /// </summary>
        /// <value>The member.</value>
        public MemberInfo Member { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            MemberExpressionFingerprint other = obj as MemberExpressionFingerprint;
            return other != null
                   && Equals(this.Member, other.Member)
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Adds to hash code combiner.
        /// </summary>
        /// <param name="combiner">The combiner.</param>
        internal override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(Member);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    /// <summary>
    /// Class MethodCallExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class MethodCallExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        /// <param name="method">The method.</param>
        public MethodCallExpressionFingerprint(ExpressionType nodeType, Type type, MethodInfo method)
            : base(nodeType, type)
        {
            // Other properties on MethodCallExpression (like the argument count) are simply derived
            // from Type and Indexer, so they're not necessary for inclusion in the fingerprint.

            Method = method;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.methodcallexpression.method.aspx
        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method.</value>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            MethodCallExpressionFingerprint other = obj as MethodCallExpressionFingerprint;
            return other != null
                   && Equals(this.Method, other.Method)
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Adds to hash code combiner.
        /// </summary>
        /// <param name="combiner">The combiner.</param>
        internal override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(Method);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    /// <summary>
    /// Class ParameterExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class ParameterExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        /// <param name="parameterIndex">Index of the parameter.</param>
        public ParameterExpressionFingerprint(ExpressionType nodeType, Type type, int parameterIndex)
            : base(nodeType, type)
        {
            ParameterIndex = parameterIndex;
        }

        // Parameter position within the overall expression, used to maintain alpha equivalence.
        /// <summary>
        /// Gets the index of the parameter.
        /// </summary>
        /// <value>The index of the parameter.</value>
        public int ParameterIndex { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            ParameterExpressionFingerprint other = obj as ParameterExpressionFingerprint;
            return other != null
                   && this.ParameterIndex == other.ParameterIndex
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Adds to hash code combiner.
        /// </summary>
        /// <param name="combiner">The combiner.</param>
        internal override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddInt32(ParameterIndex);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    /// <summary>
    /// Class TypeBinaryExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class TypeBinaryExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeBinaryExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        /// <param name="typeOperand">The type operand.</param>
        public TypeBinaryExpressionFingerprint(ExpressionType nodeType, Type type, Type typeOperand)
            : base(nodeType, type)
        {
            TypeOperand = typeOperand;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.typebinaryexpression.typeoperand.aspx
        /// <summary>
        /// Gets the type operand.
        /// </summary>
        /// <value>The type operand.</value>
        public Type TypeOperand { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            TypeBinaryExpressionFingerprint other = obj as TypeBinaryExpressionFingerprint;
            return other != null
                   && Equals(this.TypeOperand, other.TypeOperand)
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Adds to hash code combiner.
        /// </summary>
        /// <param name="combiner">The combiner.</param>
        internal override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(TypeOperand);
            base.AddToHashCodeCombiner(combiner);
        }
    }

    /// <summary>
    /// Class UnaryExpressionFingerprint. This class cannot be inherited.
    /// Implements the <see cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    /// </summary>
    /// <seealso cref="ServiceStack.ExpressionUtil.ExpressionFingerprint" />
    internal sealed class UnaryExpressionFingerprint : ExpressionFingerprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryExpressionFingerprint"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        /// <param name="method">The method.</param>
        public UnaryExpressionFingerprint(ExpressionType nodeType, Type type, MethodInfo method)
            : base(nodeType, type)
        {
            // Other properties on UnaryExpression (like IsLifted / IsLiftedToNull) are simply derived
            // from Type and NodeType, so they're not necessary for inclusion in the fingerprint.

            Method = method;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.unaryexpression.method.aspx
        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method.</value>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            UnaryExpressionFingerprint other = obj as UnaryExpressionFingerprint;
            return other != null
                   && Equals(this.Method, other.Method)
                   && this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Adds to hash code combiner.
        /// </summary>
        /// <param name="combiner">The combiner.</param>
        internal override void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddObject(Method);
            base.AddToHashCodeCombiner(combiner);
        }
    }
}