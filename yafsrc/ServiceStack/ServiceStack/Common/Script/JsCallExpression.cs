// ***********************************************************************
// <copyright file="JsCallExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    using ServiceStack.Extensions;

    /// <summary>
    /// Class JsCallExpression.
    /// Implements the <see cref="ServiceStack.Script.JsExpression" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.JsExpression" />
    public class JsCallExpression : JsExpression
    {
        /// <summary>
        /// Gets the callee.
        /// </summary>
        /// <value>The callee.</value>
        public JsToken Callee { get; }
        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public JsToken[] Arguments { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsCallExpression"/> class.
        /// </summary>
        /// <param name="callee">The callee.</param>
        /// <param name="arguments">The arguments.</param>
        public JsCallExpression(JsToken callee, params JsToken[] arguments)
        {
            Callee = callee;
            Arguments = arguments;
        }

        /// <summary>
        /// The name string
        /// </summary>
        private string nameString;
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => nameString ??= Callee is JsIdentifier identifier ? identifier.Name : null;

        /// <summary>
        /// Invokes the delegate.
        /// </summary>
        /// <param name="fn">The function.</param>
        /// <param name="target">The target.</param>
        /// <param name="isMemberExpr">if set to <c>true</c> [is member expr].</param>
        /// <param name="fnArgValues">The function argument values.</param>
        /// <returns>System.Object.</returns>
        public static object InvokeDelegate(Delegate fn, object target, bool isMemberExpr, List<object> fnArgValues)
        {
#if DEBUG
            try
#endif
            {
                if (fn is MethodInvoker methodInvoker)
                {
                    if (target == null && !isMemberExpr && fnArgValues.Count > 0)
                    {
                        target = fnArgValues[0];
                        fnArgValues.RemoveAt(0);
                    }

                    return methodInvoker(target, fnArgValues.ToArray());
                }
                if (fn is ActionInvoker actionInvoker)
                {
                    if (target == null && !isMemberExpr && fnArgValues.Count > 0)
                    {
                        target = fnArgValues[0];
                        fnArgValues.RemoveAt(0);
                    }

                    actionInvoker(target, fnArgValues.ToArray());
                    return IgnoreResult.Value;
                }
                if (fn is StaticMethodInvoker staticMethodInvoker)
                {
                    if (isMemberExpr)
                        fnArgValues.Insert(0, target);

                    return staticMethodInvoker(fnArgValues.ToArray());
                }
                if (fn is StaticActionInvoker staticActionInvoker)
                {
                    if (isMemberExpr)
                        fnArgValues.Insert(0, target);

                    staticActionInvoker(fnArgValues.ToArray());
                    return IgnoreResult.Value;
                }
                if (fn is ObjectActivator activator)
                {
                    if (isMemberExpr)
                        fnArgValues.Insert(0, target);

                    return activator(fnArgValues.ToArray());
                }

                var delegateInvoker = fn.Method.GetInvokerDelegate();

                if (target != null && isMemberExpr)
                {
                    fnArgValues.Insert(0, target);
                }

                var isStaticDelegate = delegateInvoker is StaticMethodInvoker || delegateInvoker is StaticActionInvoker;
                target = isStaticDelegate
                    ? null
                    : fn.Method.DeclaringType.CreateInstance();
                if (isStaticDelegate)
                    isMemberExpr = false;

                return InvokeDelegate(delegateInvoker,
                    target,
                    isMemberExpr, fnArgValues);
            }
#if DEBUG
            catch (Exception e)
            {
                var ex = e.GetInnerMostException().UnwrapIfSingleException().GetInnerMostException();
                Logging.LogManager.GetLogger(typeof(JsCallExpression)).Error(ex.Message + "\n" + ex.StackTrace, ex);
                throw;
            }
#endif
        }

        /// <summary>
        /// Evaluates the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.StackOverflowException">Exceeded MaxStackDepth of " + scope.Context.MaxStackDepth</exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public override object Evaluate(ScriptScopeContext scope)
        {
            string ResolveMethodName(JsToken token)
            {
                if (token is JsIdentifier identifier)
                    return identifier.Name;

                if (token is JsMemberExpression memberExpr)
                {
                    if (!memberExpr.Computed)
                        return JsObjectExpression.GetKey(memberExpr.Property);

                    var propValue = memberExpr.Property.Evaluate(scope);
                    if (propValue is not string s)
                        throw new NotSupportedException($"Expected string method name but was '{propValue?.GetType().Name ?? "null"}'");
                    return s;
                }

                return null;
            }


            MethodInvoker invoker;
            ScriptMethods filter;
            object value;

            var name = ResolveMethodName(Callee);
            var result = scope.PageResult;

            if (result.StackDepth > scope.Context.MaxStackDepth)
                throw new StackOverflowException("Exceeded MaxStackDepth of " + scope.Context.MaxStackDepth);

            var expr = Callee as JsMemberExpression;

            if (Arguments.Length == 0)
            {
                var argFn = ResolveDelegate(scope, name);
                if (argFn is Delegate fn)
                {
                    var ret = InvokeDelegate(fn, expr?.Object.Evaluate(scope), expr != null, new List<object>());
                    return ret;
                }

                if (expr != null)
                {
                    invoker = result.GetFilterInvoker(name, 1, out filter);
                    if (invoker != null)
                    {
                        var targetValue = expr.Object.Evaluate(scope);
                        if (targetValue == StopExecution.Value)
                            return targetValue;

                        value = result.InvokeFilter(invoker, filter, new[] { targetValue }, name);
                        return value;
                    }

                    invoker = result.GetContextFilterInvoker(name, 2, out filter);
                    if (invoker != null)
                    {
                        var targetValue = expr.Object.Evaluate(scope);
                        if (targetValue == StopExecution.Value)
                            return targetValue;

                        value = result.InvokeFilter(invoker, filter, new[] { scope, targetValue }, name);
                        return value;
                    }
                }

                value = Callee.Evaluate(scope);
                return value;
            }
            else
            {
                var fnArgValuesCount = Arguments.Length;
                foreach (var arg in Arguments)
                {
                    if (arg is JsSpreadElement) // ...[1,2] Arguments.Length = 1 / fnArgValues.Length = 2
                    {
                        var expandedArgs = EvaluateArgumentValues(scope, Arguments);
                        fnArgValuesCount = expandedArgs.Count;
                        break;
                    }
                }

                object argFn = null;
                argFn = ResolveDelegate(scope, name);

                if (argFn is Delegate fn)
                {
                    var fnArgValues = EvaluateArgumentValues(scope, Arguments);
                    var ret = InvokeDelegate(fn, expr?.Object.Evaluate(scope), expr != null, fnArgValues);
                    return ret;
                }

                if (expr != null)
                {
                    invoker = result.GetFilterInvoker(name, fnArgValuesCount + 1, out filter);
                    if (invoker != null)
                    {
                        var targetValue = expr.Object.Evaluate(scope);
                        if (targetValue == StopExecution.Value)
                            return targetValue;

                        var fnArgValues = EvaluateArgumentValues(scope, Arguments);
                        fnArgValues.Insert(0, targetValue);

                        value = result.InvokeFilter(invoker, filter, fnArgValues.ToArray(), name);
                        return value;
                    }

                    invoker = result.GetContextFilterInvoker(name, fnArgValuesCount + 2, out filter);
                    if (invoker != null)
                    {
                        var targetValue = expr.Object.Evaluate(scope);
                        if (targetValue == StopExecution.Value)
                            return targetValue;

                        var fnArgValues = EvaluateArgumentValues(scope, Arguments);
                        fnArgValues.InsertRange(0, new[] { scope, targetValue });
                        value = result.InvokeFilter(invoker, filter, fnArgValues.ToArray(), name);
                        return value;
                    }
                }
                else
                {
                    name = Name;

                    invoker = result.GetFilterInvoker(name, fnArgValuesCount, out filter);
                    if (invoker != null)
                    {
                        var fnArgValues = EvaluateArgumentValues(scope, Arguments);
                        value = result.InvokeFilter(invoker, filter, fnArgValues.ToArray(), name);
                        return value;
                    }

                    invoker = result.GetContextFilterInvoker(name, fnArgValuesCount + 1, out filter);
                    if (invoker != null)
                    {
                        var fnArgValues = EvaluateArgumentValues(scope, Arguments);
                        fnArgValues.Insert(0, scope);
                        value = result.InvokeFilter(invoker, filter, fnArgValues.ToArray(), name);
                        return value;
                    }
                }

                throw new NotSupportedException(result.CreateMissingFilterErrorMessage(name.LeftPart('(')));
            }
        }

        /// <summary>
        /// Resolves the delegate.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Object.</returns>
        private object ResolveDelegate(ScriptScopeContext scope, string name)
        {
            if (name == null)
            {
                if (Callee is JsCallExpression)
                    return Callee.Evaluate(scope);
            }
            else
                return scope.GetArgument(name);

            return null;
        }

        /// <summary>
        /// Evaluates the argument values.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>List&lt;System.Object&gt;.</returns>
        public static List<object> EvaluateArgumentValues(ScriptScopeContext scope, JsToken[] args)
        {
            var fnArgValues = new List<object>(args.Length + 2); //max size of args without spread args
            foreach (var arg in args)
            {
                if (arg is JsSpreadElement spread)
                {
                    if (spread.Argument.Evaluate(scope) is not IEnumerable spreadValues)
                        continue;
                    foreach (var argValue in spreadValues)
                    {
                        fnArgValues.Add(argValue);
                    }
                }
                else
                {
                    fnArgValues.Add(arg.Evaluate(scope));
                }
            }

            return fnArgValues;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => ToRawString();

        /// <summary>
        /// Converts to rawstring.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string ToRawString()
        {
            var sb = StringBuilderCacheAlt.Allocate();
            foreach (var arg in Arguments)
            {
                if (sb.Length > 0)
                    sb.Append(',');
                sb.Append(arg.ToRawString());
            }

            return $"{Callee.ToRawString()}({StringBuilderCacheAlt.ReturnAndFree(sb)})";
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetDisplayName() => (Name ?? "").Replace('′', '"');

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool Equals(JsCallExpression other)
        {
            return Equals(Callee, other.Callee) &&
                   Arguments.EquivalentTo(other.Arguments);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JsCallExpression)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Callee != null ? Callee.GetHashCode() : 0) * 397) ^
                       (Arguments != null ? Arguments.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Converts to jsast.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public override Dictionary<string, object> ToJsAst()
        {
            var arguments = new List<object>();
            var to = new Dictionary<string, object>
            {
                ["type"] = ToJsAstType(),
                ["callee"] = Callee.ToJsAst(),
                ["arguments"] = arguments,
            };

            foreach (var argument in Arguments)
            {
                arguments.Add(argument.ToJsAst());
            }

            return to;
        }
    }
}