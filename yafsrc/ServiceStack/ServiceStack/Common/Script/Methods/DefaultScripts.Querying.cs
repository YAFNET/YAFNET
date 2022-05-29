// ***********************************************************************
// <copyright file="DefaultScripts.Querying.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;

namespace ServiceStack.Script;

// ReSharper disable InconsistentNaming
/// <summary>
/// Class DefaultScripts.
/// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
/// Implements the <see cref="ServiceStack.Script.IConfigureScriptContext" />
/// </summary>
/// <seealso cref="ServiceStack.Script.ScriptMethods" />
/// <seealso cref="ServiceStack.Script.IConfigureScriptContext" />
public partial class DefaultScripts
{
    /// <summary>
    /// Steps the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>List&lt;System.Object&gt;.</returns>
    public List<object> step(IEnumerable target, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(step));

        var scopedParams = scopeOptions.AssertOptions(nameof(step));

        var from = scopedParams.TryGetValue("from", out object oFrom)
                       ? (int)oFrom
                       : 0;

        var by = scopedParams.TryGetValue("by", out object oBy)
                     ? (int)oBy
                     : 1;

        var to = new List<object>();
        var itemsArray = items.ToArray();
        for (var i = from; i < itemsArray.Length; i += by)
        {
            to.Add(itemsArray[i]);
        }

        return to;
    }

    /// <summary>
    /// Elements at.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="index">The index.</param>
    /// <returns>System.Object.</returns>
    public object elementAt(IEnumerable target, int index)
    {
        var items = target.AssertEnumerable(nameof(elementAt));

        var i = 0;
        foreach (var item in items)
        {
            if (i++ == index)
                return item;
        }

        return null;
    }

    /// <summary>
    /// Determines whether [contains] [the specified target].
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="needle">The needle.</param>
    /// <returns><c>true</c> if [contains] [the specified target]; otherwise, <c>false</c>.</returns>
    /// <exception cref="System.NotSupportedException">'{nameof(contains)}' requires a string or IEnumerable but received a '{target.GetType()?.Name}' instead</exception>
    public bool contains(object target, object needle)
    {
        if (isNull(needle))
            return false;

        if (target is string s)
        {
            if (needle is char c)
                return s.IndexOf(c) >= 0;
            return s.IndexOf(needle.ToString(), StringComparison.Ordinal) >= 0;
        }
        if (target is IEnumerable items)
        {
            foreach (var item in items)
            {
                if (Equals(item, needle))
                    return true;
            }
            return false;
        }
        throw new NotSupportedException($"'{nameof(contains)}' requires a string or IEnumerable but received a '{target.GetType().Name}' instead");
    }

    /// <summary>
    /// Sequences the equals.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool sequenceEquals(IEnumerable a, IEnumerable b) => a.Cast<object>().SequenceEqual(b.Cast<object>());

    /// <summary>
    /// Takes the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="original">The original.</param>
    /// <param name="countOrBinding">The count or binding.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> take(ScriptScopeContext scope, IEnumerable<object> original, object countOrBinding) =>
        original.Take(scope.GetValueOrEvaluateBinding<int>(countOrBinding));

    /// <summary>
    /// Skips the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="original">The original.</param>
    /// <param name="countOrBinding">The count or binding.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> skip(ScriptScopeContext scope, IEnumerable<object> original, object countOrBinding) =>
        original.Skip(scope.GetValueOrEvaluateBinding<int>(countOrBinding));

    /// <summary>
    /// Limits the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="original">The original.</param>
    /// <param name="skipOrBinding">The skip or binding.</param>
    /// <param name="takeOrBinding">The take or binding.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> limit(ScriptScopeContext scope, IEnumerable<object> original, object skipOrBinding, object takeOrBinding)
    {
        var skip = scope.GetValueOrEvaluateBinding<int>(skipOrBinding);
        var take = scope.GetValueOrEvaluateBinding<int>(takeOrBinding);
        return original.Skip(skip).Take(take);
    }

    /// <summary>
    /// Counts the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.Int32.</returns>
    public int count(ScriptScopeContext scope, object target) => target.AssertEnumerable(nameof(count)).Count();
    /// <summary>
    /// Counts the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Int32.</returns>
    public int count(ScriptScopeContext scope, object target, object expression) => count(scope, target, expression, null);
    /// <summary>
    /// Counts the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Int32.</returns>
    public int count(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(count));
        var expr = scope.AssertExpression(nameof(count), expression, scopeOptions, out var itemBinding);

        var total = 0;
        var i = 0;
        foreach (var item in items)
        {
            scope.AddItemToScope(itemBinding, item, i++);
            var result = expr.EvaluateToBool(scope);
            if (result)
                total++;
        }

        return total;
    }

    /// <summary>
    /// Sums the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public object sum(ScriptScopeContext scope, object target) => sum(scope, target, null, null);
    /// <summary>
    /// Sums the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Object.</returns>
    public object sum(ScriptScopeContext scope, object target, object expression) => sum(scope, target, expression, null);
    /// <summary>
    /// Sums the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Object.</returns>
    public object sum(ScriptScopeContext scope, object target, object expression, object scopeOptions) =>
        applyInternal(nameof(sum), scope, target, expression, scopeOptions, (a, b) => a + b);

    /// <summary>
    /// Minimums the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public object min(ScriptScopeContext scope, object target) => min(scope, target, null, null);
    /// <summary>
    /// Minimums the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Object.</returns>
    public object min(ScriptScopeContext scope, object target, object expression) => min(scope, target, expression, null);
    /// <summary>
    /// Minimums the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Object.</returns>
    public object min(ScriptScopeContext scope, object target, object expression, object scopeOptions) =>
        applyInternal(nameof(min), scope, target, expression, scopeOptions, (a, b) => b < a ? b : a);

    /// <summary>
    /// Maximums the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public object max(ScriptScopeContext scope, object target) => max(scope, target, null, null);
    /// <summary>
    /// Maximums the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Object.</returns>
    public object max(ScriptScopeContext scope, object target, object expression) => max(scope, target, expression, null);
    /// <summary>
    /// Maximums the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Object.</returns>
    public object max(ScriptScopeContext scope, object target, object expression, object scopeOptions) =>
        applyInternal(nameof(max), scope, target, expression, scopeOptions, (a, b) => b > a ? b : a);

    /// <summary>
    /// Averages the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.Double.</returns>
    public double average(ScriptScopeContext scope, object target) => average(scope, target, null, null);
    /// <summary>
    /// Averages the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Double.</returns>
    public double average(ScriptScopeContext scope, object target, object expression) => average(scope, target, expression, null);
    /// <summary>
    /// Averages the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Double.</returns>
    public double average(ScriptScopeContext scope, object target, object expression, object scopeOptions) =>
        applyInternal(nameof(average), scope, target, expression, scopeOptions, (a, b) => a + b).ConvertTo<double>() / target.AssertEnumerable(nameof(average)).Count();

    /// <summary>
    /// Applies the internal.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <param name="fn">The function.</param>
    /// <returns>System.Object.</returns>
    private object applyInternal(string filterName, ScriptScopeContext scope, object target, object expression, object scopeOptions,
                                 Func<double, double, double> fn)
    {
        if (target is double d)
            return fn(d, expression.ConvertTo<double>());
        if (target is int i)
            return (int)fn(i, expression.ConvertTo<double>());
        if (target is long l)
            return (long)fn(l, expression.ConvertTo<double>());

        var items = target.AssertEnumerable(filterName);
        var total = filterName == nameof(min)
                        ? double.MaxValue
                        : 0;
        Type itemType = null;
        if (expression != null)
        {
            scope = scope.Clone();
            var expr = scope.AssertExpression(filterName, expression, scopeOptions, out var itemBinding);

            foreach (var item in items)
            {
                if (item == null) continue;

                scope.AddItemToScope(itemBinding, item);
                var result = expr.Evaluate(scope);
                if (result == null) continue;
                itemType ??= result.GetType();

                total = fn(total, result.ConvertTo<double>());
            }
        }
        else
        {
            foreach (var item in items)
            {
                if (item == null) continue;
                itemType ??= item.GetType();
                total = fn(total, item.ConvertTo<double>());
            }
        }

        if (filterName == nameof(min) && itemType == null)
            return 0;

        if (expression == null && itemType == null)
            itemType = target.GetType().FirstGenericType()?.GetGenericArguments().FirstOrDefault();

        return itemType == null || itemType == typeof(double)
                   ? total
                   : total.ConvertTo(itemType);
    }

    /// <summary>
    /// Reduces the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Object.</returns>
    public object reduce(ScriptScopeContext scope, object target, object expression) => reduce(scope, target, expression, null);
    /// <summary>
    /// Reduces the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    /// <exception cref="System.NotSupportedException"></exception>
    public object reduce(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(reduce));
        Type itemType = null;

        if (expression is not JsArrowFunctionExpression arrowExpr)
            throw new NotSupportedException($"{nameof(reduce)} expects an arrow expression but was instead '{expression.GetType().Name}'");

        if (arrowExpr.Params.Length != 2)
            throw new NotSupportedException($"{nameof(reduce)} expects 2 params but was instead {arrowExpr.Params.Length}");

        var accumulatorBinding = arrowExpr.Params[0].Name;
        var itemBinding = arrowExpr.Params[1].Name;
        var expr = arrowExpr.Body;

        var scopedParams = scopeOptions as Dictionary<string, object> ?? new Dictionary<string, object>();

        var accumulator = scopedParams.TryGetValue("initialValue", out object initialValue)
                              ? initialValue.ConvertTo<double>()
                              : scopeOptions is not IDictionary
                                  ? scopeOptions.ConvertTo<double>()
                                  : 0;

        var i = 0;
        scope = scope.Clone();
        foreach (var item in items)
        {
            if (item == null) continue;

            scope.AddItemToScope(accumulatorBinding, accumulator);
            scope.AddItemToScope("index", i++);
            scope.AddItemToScope(itemBinding, item);

            var result = expr.Evaluate(scope);
            if (result == null) continue;
            itemType ??= result.GetType();

            accumulator = result.ConvertTo<double>();
        }

        return itemType == null || itemType == typeof(double)
                   ? accumulator
                   : accumulator.ConvertTo(itemType);
    }

    /// <summary>
    /// Zips the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="original">The original.</param>
    /// <param name="itemsOrBinding">The items or binding.</param>
    /// <returns>List&lt;System.Object[]&gt;.</returns>
    /// <exception cref="System.ArgumentException"></exception>
    public List<object[]> zip(ScriptScopeContext scope, IEnumerable original, object itemsOrBinding)
    {
        var to = new List<object[]>();
        string literal = itemsOrBinding as string;
        var arrowExpr = itemsOrBinding as JsArrowFunctionExpression;

        if (literal != null || arrowExpr != null)
        {
            var token = literal != null
                            ? literal.GetCachedJsExpression(scope)
                            : arrowExpr.Body;
            var binding = arrowExpr != null
                              ? arrowExpr.Params[0].Name
                              : "it";

            var i = 0;
            foreach (var a in original)
            {
                scope.AddItemToScope(binding, a, i++);
                var bindValue = token.Evaluate(scope);
                if (bindValue is IEnumerable current)
                {
                    foreach (var b in current)
                    {
                        to.Add(new[] { a, b });
                    }
                }
                else if (bindValue != null)
                    throw new ArgumentException($"{nameof(zip)} in '{scope.Page.VirtualPath}' requires '{literal}' to evaluate to an IEnumerable, but evaluated to a '{bindValue.GetType().Name}' instead");
            }
        }
        else if (itemsOrBinding is IEnumerable current)
        {
            var currentArray = current.Cast<object>().ToArray();
            foreach (var a in original)
            {
                foreach (var b in currentArray)
                {
                    to.Add(new[] { a, b });
                }
            }
        }

        return to;
    }

    /// <summary>
    /// Flattens the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>List&lt;System.Object&gt;.</returns>
    public List<object> flatten(object target) => flatten(target, int.MaxValue);
    /// <summary>
    /// Flattens the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="depth">The depth.</param>
    /// <returns>List&lt;System.Object&gt;.</returns>
    public List<object> flatten(object target, int depth)
    {
        var to = new List<object>();
        _flatten(to, target, depth);
        return to;
    }

    /// <summary>
    /// Flattens the specified to.
    /// </summary>
    /// <param name="to">To.</param>
    /// <param name="target">The target.</param>
    /// <param name="depth">The depth.</param>
    private void _flatten(ICollection<object> to, object target, int depth)
    {
        if (target != null)
        {
            if (target is not string && target is not IDictionary && target is IEnumerable objs)
            {
                foreach (var o in objs)
                {
                    if (depth > 0)
                        _flatten(to, o, depth - 1);
                    else
                        to.Add(o);
                }
            }
            else
            {
                to.Add(target);
            }
        }
    }

    /// <summary>
    /// Lets the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="scopeBindings">The scope bindings.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">'{nameof(let)}' in '{scope.Page.VirtualPath}' expects a string Expression for its value but received '{entry.Value}' instead</exception>
    /// <exception cref="System.NotSupportedException">'{nameof(let)}' in '{scope.Page.VirtualPath}' requires an IEnumerable but received a '{target.GetType()?.Name}' instead</exception>
    public object let(ScriptScopeContext scope, object target, object scopeBindings) //from filter
    {
        if (target is IEnumerable objs)
        {
            string itemBinding;
            Dictionary<string, object> scopedParams = null;

            var arrowExpr = scopeBindings as JsArrowFunctionExpression;
            if (arrowExpr != null)
            {
                itemBinding = arrowExpr.Params[0].Name;
            }
            else
            {
                scopedParams = scope.GetParamsWithItemBindingOnly(nameof(let), null, scopeBindings, out itemBinding);
            }

            var to = new List<ScopeVars>();
            var i = 0;
            foreach (var item in objs)
            {
                scope.ScopedParams[ScriptConstants.Index] = i++;
                scope.ScopedParams[itemBinding] = item;

                // Copy over previous let bindings into new let bindings
                var itemBindings = new ScopeVars();
                if (item is object[] tuple)
                {
                    foreach (var a in tuple)
                    {
                        if (a is IDictionary<string, object> aArgs)
                        {
                            foreach (var entry in aArgs)
                            {
                                itemBindings[entry.Key] = entry.Value;
                            }
                        }
                    }
                }

                if (arrowExpr != null)
                {
                    var value = arrowExpr.Body.Evaluate(scope);
                    if (value is Dictionary<string, object> bindingVars)
                    {
                        foreach (var bindingVar in bindingVars)
                        {
                            itemBindings[bindingVar.Key] = bindingVar.Value;
                        }
                    }
                }
                else
                {
                    foreach (var entry in scopedParams)
                    {
                        var bindTo = entry.Key;
                        if (entry.Value is not string bindToLiteral)
                            throw new NotSupportedException($"'{nameof(let)}' in '{scope.Page.VirtualPath}' expects a string Expression for its value but received '{entry.Value}' instead");

                        bindToLiteral.ParseJsExpression(out JsToken token);

                        try
                        {
                            var bindValue = token.Evaluate(scope);
                            scope.ScopedParams[bindTo] = bindValue;
                            itemBindings[bindTo] = bindValue;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }
                }

                to.Add(itemBindings);
            }

            return to;
        }
        if (target != null)
            throw new NotSupportedException($"'{nameof(let)}' in '{scope.Page.VirtualPath}' requires an IEnumerable but received a '{target.GetType().Name}' instead");

        return null;
    }

    /// <summary>
    /// Firsts the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public object first(ScriptScopeContext scope, object target) => target.AssertEnumerable(nameof(first)).FirstOrDefault();
    /// <summary>
    /// Firsts the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Object.</returns>
    public object first(ScriptScopeContext scope, object target, object expression) => first(scope, target, expression, null);
    /// <summary>
    /// Firsts the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Object.</returns>
    public object first(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(first));
        var expr = scope.AssertExpression(nameof(first), expression, scopeOptions, out var itemBinding);

        var i = 0;
        foreach (var item in items)
        {
            scope.AddItemToScope(itemBinding, item, i++);
            var result = expr.EvaluateToBool(scope);
            if (result)
                return item;
        }

        return null;
    }

    /// <summary>
    /// Lasts the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public object last(ScriptScopeContext scope, object target) => target.AssertEnumerable(nameof(last)).LastOrDefault();

    /// <summary>
    /// Anies the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool any(ScriptScopeContext scope, object target) => target.AssertEnumerable(nameof(any)).Any();
    /// <summary>
    /// Anies the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool any(ScriptScopeContext scope, object target, object expression) => any(scope, target, expression, null);
    /// <summary>
    /// Anies the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool any(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(any));
        var expr = scope.AssertExpression(nameof(any), expression, scopeOptions, out var itemBinding);

        var i = 0;
        foreach (var item in items)
        {
            scope.AddItemToScope(itemBinding, item, i++);
            var result = expr.EvaluateToBool(scope);
            if (result)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Alls the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool all(ScriptScopeContext scope, object target, object expression) => all(scope, target, expression, null);
    /// <summary>
    /// Alls the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool all(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(all));
        var expr = scope.AssertExpression(nameof(all), expression, scopeOptions, out var itemBinding);

        var i = 0;
        foreach (var item in items)
        {
            scope.AddItemToScope(itemBinding, item, i++);
            var result = expr.EvaluateToBool(scope);
            if (!result)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Wheres the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> where(ScriptScopeContext scope, object target, object expression) => where(scope, target, expression, null);
    /// <summary>
    /// Wheres the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> where(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(where));
        var expr = scope.AssertExpression(nameof(where), expression, scopeOptions, out var itemBinding);

        var to = new List<object>();
        var i = 0;
        foreach (var item in items)
        {
            scope.AddItemToScope(itemBinding, item, i++);
            var result = expr.EvaluateToBool(scope);
            if (result)
                to.Add(item);
        }

        return to;
    }

    /// <summary>
    /// Takes the while.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> takeWhile(ScriptScopeContext scope, object target, object expression) => takeWhile(scope, target, expression, null);
    /// <summary>
    /// Takes the while.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> takeWhile(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(takeWhile));
        var expr = scope.AssertExpression(nameof(takeWhile), expression, scopeOptions, out var itemBinding);

        var to = new List<object>();
        var i = 0;
        foreach (var item in items)
        {
            scope.AddItemToScope(itemBinding, item, i++);
            var result = expr.EvaluateToBool(scope);
            if (result)
                to.Add(item);
            else
                return to;
        }

        return to;
    }

    /// <summary>
    /// Skips the while.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> skipWhile(ScriptScopeContext scope, object target, object expression) => skipWhile(scope, target, expression, null);
    /// <summary>
    /// Skips the while.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> skipWhile(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(skipWhile));
        var expr = scope.AssertExpression(nameof(skipWhile), expression, scopeOptions, out var itemBinding);

        var to = new List<object>();
        var i = 0;
        var keepSkipping = true;
        foreach (var item in items)
        {
            scope.AddItemToScope(itemBinding, item, i++);
            var result = expr.EvaluateToBool(scope);
            if (!result)
                keepSkipping = false;

            if (!keepSkipping)
                to.Add(item);
        }

        return to;
    }

    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> orderBy(ScriptScopeContext scope, object target, object expression) => orderBy(scope, target, expression, null);
    /// <summary>
    /// Orders the by.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> orderBy(ScriptScopeContext scope, object target, object expression, object scopeOptions) =>
        orderByInternal(nameof(orderBy), scope, target, expression, scopeOptions);

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> orderByDescending(ScriptScopeContext scope, object target, object expression) => orderByDescending(scope, target, expression, null);
    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> orderByDescending(ScriptScopeContext scope, object target, object expression, object scopeOptions) =>
        orderByInternal(nameof(orderByDescending), scope, target, expression, scopeOptions);

    /// <summary>
    /// Orders the by desc.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> orderByDesc(ScriptScopeContext scope, object target, object expression) => orderByDesc(scope, target, expression, null);
    /// <summary>
    /// Orders the by desc.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> orderByDesc(ScriptScopeContext scope, object target, object expression, object scopeOptions) =>
        orderByInternal(nameof(orderByDescending), scope, target, expression, scopeOptions);

    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> thenBy(ScriptScopeContext scope, object target, object expression) => thenBy(scope, target, expression, null);
    /// <summary>
    /// Thens the by.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> thenBy(ScriptScopeContext scope, object target, object expression, object scopeOptions) =>
        thenByInternal(nameof(thenBy), scope, target, expression, scopeOptions);

    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> thenByDescending(ScriptScopeContext scope, object target, object expression) => thenByDescending(scope, target, expression, null);
    /// <summary>
    /// Thens the by descending.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> thenByDescending(ScriptScopeContext scope, object target, object expression, object scopeOptions) =>
        thenByInternal(nameof(thenByDescending), scope, target, expression, scopeOptions);

    /// <summary>
    /// Class ComparerWrapper.
    /// Implements the <see cref="object" />
    /// </summary>
    /// <seealso cref="object" />
    private class ComparerWrapper : IComparer<object>
    {
        /// <summary>
        /// The comparer
        /// </summary>
        private readonly IComparer comparer;
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerWrapper"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public ComparerWrapper(IComparer comparer)
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.
        /// Value
        /// Meaning
        /// Less than zero
        /// <paramref name="x" /> is less than <paramref name="y" />.
        /// Zero
        /// <paramref name="x" /> equals <paramref name="y" />.
        /// Greater than zero
        /// <paramref name="x" /> is greater than <paramref name="y" />.</returns>
        public int Compare(object x, object y) => comparer.Compare(x, y);
    }
    /// <summary>
    /// Class EqualityComparerWrapper.
    /// Implements the <see cref="object" />
    /// </summary>
    /// <seealso cref="object" />
    private class EqualityComparerWrapper : IEqualityComparer<object>
    {
        /// <summary>
        /// The comparer
        /// </summary>
        private readonly IEqualityComparer comparer;
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityComparerWrapper"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public EqualityComparerWrapper(IEqualityComparer comparer)
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns><see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
        public new bool Equals(object x, object y) => comparer.Equals(x, y);
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(object obj) => comparer.GetHashCode(obj);
    }
    /// <summary>
    /// Class EqualityComparerWrapper.
    /// Implements the <see cref="object" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="object" />
    private class EqualityComparerWrapper<T> : IEqualityComparer<object>
    {
        /// <summary>
        /// The comparer
        /// </summary>
        private readonly IEqualityComparer<T> comparer;
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityComparerWrapper{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public EqualityComparerWrapper(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns><see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
        public new bool Equals(object x, object y) => comparer.Equals((T)x, (T)y);
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(object obj) => comparer.GetHashCode((T)obj);
    }

    /// <summary>
    /// Gets the comparer.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="scopedParams">The scoped parameters.</param>
    /// <returns>IComparer&lt;System.Object&gt;.</returns>
    /// <exception cref="System.NotSupportedException">'{filterName}' in '{scope.Page.VirtualPath}' expects a IComparer but received a '{oComparer.GetType()?.Name}' instead</exception>
    private static IComparer<object> GetComparer(string filterName, ScriptScopeContext scope, IReadOnlyDictionary<string, object> scopedParams)
    {
        var comparer = (IComparer<object>)Comparer<object>.Default;
        if (!scopedParams.TryGetValue(ScriptConstants.Comparer, out object oComparer)) return comparer;
        if (oComparer is not IComparer nonGenericComparer)
            throw new NotSupportedException(
                $"'{filterName}' in '{scope.Page.VirtualPath}' expects a IComparer but received a '{oComparer.GetType().Name}' instead");
        comparer = new ComparerWrapper(nonGenericComparer);
        return comparer;
    }

    /// <summary>
    /// Orders the by internal.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public static IEnumerable<object> orderByInternal(string filterName, ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(filterName);
        var expr = scope.AssertExpression(filterName, expression, scopeOptions, out var itemBinding);

        var comparer = GetComparer(filterName, scope, scopeOptions as Dictionary<string, object> ?? new Dictionary<string, object>());

        var i = 0;
        var sorted = filterName == nameof(orderByDescending)
                         ? items.OrderByDescending(item => expr.Evaluate(scope.AddItemToScope(itemBinding, item, i++)), comparer)
                         : items.OrderBy(item => expr.Evaluate(scope.AddItemToScope(itemBinding, item, i++)), comparer);

        return sorted;
    }

    /// <summary>
    /// Thens the by internal.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    /// <exception cref="System.NotSupportedException">'{filterName}' in '{scope.Page.VirtualPath}' requires an IOrderedEnumerable but received a '{target?.GetType()?.Name}' instead</exception>
    public static IEnumerable<object> thenByInternal(string filterName, ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        if (target is not IOrderedEnumerable<object> items)
            throw new NotSupportedException($"'{filterName}' in '{scope.Page.VirtualPath}' requires an IOrderedEnumerable but received a '{target?.GetType().Name}' instead");

        var expr = scope.AssertExpression(filterName, expression, scopeOptions, out var itemBinding);

        var comparer = GetComparer(filterName, scope, scopeOptions as Dictionary<string, object> ?? new Dictionary<string, object>());
        var i = 0;

        var sorted = filterName == nameof(thenByDescending)
                         ? items.ThenByDescending(item => expr.Evaluate(scope.AddItemToScope(itemBinding, item, i++)), comparer)
                         : items.ThenBy(item => expr.Evaluate(scope.AddItemToScope(itemBinding, item, i++)), comparer);

        return sorted;
    }

    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="items">The items.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>IEnumerable&lt;IGrouping&lt;System.Object, System.Object&gt;&gt;.</returns>
    public IEnumerable<IGrouping<object, object>> groupBy(ScriptScopeContext scope, IEnumerable<object> items, object expression) => groupBy(scope, items, expression, null);
    /// <summary>
    /// Groups the by.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="items">The items.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable&lt;IGrouping&lt;System.Object, System.Object&gt;&gt;.</returns>
    /// <exception cref="System.NotSupportedException">'{nameof(groupBy)}' in '{scope.Page.VirtualPath}' expects a IEqualityComparer but received a '{oComparer.GetType()?.Name}' instead</exception>
    /// <exception cref="System.NotSupportedException">map expression in '{nameof(groupBy)}' must be a string or arrow expression</exception>
    public IEnumerable<IGrouping<object, object>> groupBy(ScriptScopeContext scope, IEnumerable<object> items, object expression, object scopeOptions)
    {
        if (items == null)
            return TypeConstants<IGrouping<object, object>>.EmptyArray;

        var expr = scope.AssertExpression(nameof(groupBy), expression, scopeOptions, out var itemBinding);

        var scopedParams = scopeOptions as Dictionary<string, object> ?? new Dictionary<string, object>();

        var comparer = (IEqualityComparer<object>)EqualityComparer<object>.Default;
        if (scopedParams.TryGetValue(ScriptConstants.Comparer, out object oComparer))
        {
            comparer = oComparer as IEqualityComparer<object>;
            if (oComparer is IEqualityComparer<string> stringComparer)
                comparer = new EqualityComparerWrapper<string>(stringComparer);
            else if (oComparer is IEqualityComparer<int> intComparer)
                comparer = new EqualityComparerWrapper<int>(intComparer);
            else if (oComparer is IEqualityComparer<long> longComparer)
                comparer = new EqualityComparerWrapper<long>(longComparer);
            else if (oComparer is IEqualityComparer<double> doubleComparer)
                comparer = new EqualityComparerWrapper<double>(doubleComparer);

            if (comparer == null)
            {
                if (oComparer is not IEqualityComparer nonGenericComparer)
                    throw new NotSupportedException(
                        $"'{nameof(groupBy)}' in '{scope.Page.VirtualPath}' expects a IEqualityComparer but received a '{oComparer.GetType().Name}' instead");
                comparer = new EqualityComparerWrapper(nonGenericComparer);
            }
        }

        if (scopedParams.TryGetValue(ScriptConstants.Map, out object map) && map != null)
        {
            var mapBinding = itemBinding;
            JsToken mapExpr;
            if (map is string mapStr)
            {
                mapStr.ParseJsExpression(out mapExpr);
            }
            else if (map is JsArrowFunctionExpression arrowExpr)
            {
                mapBinding = arrowExpr.Params[0].Name;
                mapExpr = arrowExpr.Body;
            }
            else
            {
                throw new NotSupportedException($"map expression in '{nameof(groupBy)}' must be a string or arrow expression");
            }

            scope = scope.Clone();
            var result = items.GroupBy(
                item => expr.Evaluate(scope.AddItemToScope(itemBinding, item)),
                item => mapExpr.Evaluate(scope.AddItemToScope(mapBinding, item)),
                comparer);
            return result;
        }
        else
        {
            var result = items.GroupBy(item => expr.Evaluate(scope.AddItemToScope(itemBinding, item)), comparer);
            return result;
        }
    }

    /// <summary>
    /// Equivalents to.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="items">The items.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool equivalentTo(IEnumerable<object> target, IEnumerable<object> items) => target.EquivalentTo(items);
    /// <summary>
    /// Distincts the specified items.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> distinct(IEnumerable<object> items) => items.Distinct();
    /// <summary>
    /// Unions the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="items">The items.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> union(IEnumerable<object> target, IEnumerable<object> items) => target.Union(items);
    /// <summary>
    /// Intersects the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="items">The items.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> intersect(IEnumerable<object> target, IEnumerable<object> items) => target.Intersect(items);
    /// <summary>
    /// Excepts the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="items">The items.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> except(IEnumerable<object> target, IEnumerable<object> items) => target.Except(items);
    /// <summary>
    /// Concats the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="items">The items.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> concat(IEnumerable<object> target, IEnumerable<object> items) => target.Concat(items);

    /// <summary>
    /// Concats the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public string concat(IEnumerable<string> target)
    {
        var sb = StringBuilderCache.Allocate();
        foreach (var str in target)
        {
            sb.Append(str);
        }
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Gets the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="key">The key.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Unknown key '{key}' on '{target.GetType().Name}'</exception>
    public object get(object target, object key)
    {
        if (isNull(target))
            return null;

        if (target is IDictionary d)
        {
            if (d.Contains(key))
                return d[key];
        }
        else if (target is object[] a)
        {
            var index = key.ConvertTo<int>();
            return index < a.Length
                       ? a[index]
                       : null;
        }
        else if (target is IList l)
        {
            var index = key.ConvertTo<int>();
            return index < l.Count
                       ? l[index]
                       : null;
        }
        else if (target is IEnumerable e && DynamicNumber.IsNumber(key.GetType())) // IGrouping<TKey,TElement> is IEnumerable & can ref 'Key' property
        {
            var index = key.ConvertTo<int>();
            var i = 0;
            foreach (var value in e)
            {
                if (i++ == index)
                    return value;
            }
        }
        else if (key is string fieldName)
        {
            var targetType = target.GetType();
            var memberFn = TypeProperties.Get(targetType).GetPublicGetter(fieldName)
                           ?? TypeFields.Get(targetType).GetPublicGetter(fieldName);
            if (memberFn != null)
                return memberFn(target);
        }
        else if (DynamicNumber.IsNumber(key.GetType()))
        {
            var indexerMethod = target.GetType().GetInstanceMethod("get_Item");
            if (indexerMethod != null)
            {
                var fn = indexerMethod.GetInvoker();
                var ret = fn(target, key);
                return ret ?? JsNull.Value;
            }
        }

        throw new NotSupportedException($"Unknown key '{key}' on '{target.GetType().Name}'");
    }
}