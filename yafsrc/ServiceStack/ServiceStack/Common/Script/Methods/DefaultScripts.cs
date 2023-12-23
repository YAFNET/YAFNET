// ***********************************************************************
// <copyright file="DefaultScripts.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ServiceStack.Text;
using ServiceStack.Text.Json;

namespace ServiceStack.Script;

// ReSharper disable InconsistentNaming
/// <summary>
/// Class DefaultScripts.
/// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
/// Implements the <see cref="ServiceStack.Script.IConfigureScriptContext" />
/// </summary>
/// <seealso cref="ServiceStack.Script.ScriptMethods" />
/// <seealso cref="ServiceStack.Script.IConfigureScriptContext" />
public partial class DefaultScripts : ScriptMethods, IConfigureScriptContext
{
    /// <summary>
    /// The instance
    /// </summary>
    public readonly static DefaultScripts Instance = new();

    /// <summary>
    /// Gets the remove new lines for.
    /// </summary>
    /// <value>The remove new lines for.</value>
    public static List<string> RemoveNewLinesFor { get; } = [
        nameof(to),
        nameof(toGlobal),
        nameof(assignTo),
        nameof(assignToGlobal),
        nameof(assignError),
        nameof(addTo),
        nameof(addToGlobal),
        nameof(addToStart),
        nameof(addToStartGlobal),
        nameof(appendTo),
        nameof(appendToGlobal),
        nameof(prependTo),
        nameof(prependToGlobal),
        nameof(@do),
        nameof(end),
        nameof(@throw),
        nameof(ifThrow),
        nameof(throwIf),
        nameof(throwIf),
        nameof(ifThrowArgumentException),
        nameof(ifThrowArgumentNullException),
        nameof(throwArgumentNullExceptionIf),
        nameof(throwArgumentException),
        nameof(throwArgumentNullException),
        nameof(throwNotSupportedException),
        nameof(throwNotImplementedException),
        nameof(throwUnauthorizedAccessException),
        nameof(throwFileNotFoundException),
        nameof(throwOptimisticConcurrencyException),
        nameof(throwNotSupportedException),
        nameof(ifError),
        nameof(skipExecutingFiltersOnError),
        nameof(continueExecutingFiltersOnError)
    ];

    /// <summary>
    /// The evaluate when skipping filter execution
    /// </summary>
    public static List<string> EvaluateWhenSkippingFilterExecution = [
        nameof(ifError),
        nameof(lastError)
    ];

    /// <summary>
    /// Configures the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Configure(ScriptContext context)
    {
        RemoveNewLinesFor.Each(name => context.RemoveNewLineAfterFiltersNamed.Add(name));
        EvaluateWhenSkippingFilterExecution.Each(name => context.OnlyEvaluateFiltersWhenSkippingPageFilterExecution.Add(name));
    }
  
    /// <summary>
    /// Determines whether the specified value is even.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if the specified value is even; otherwise, <c>false</c>.</returns>
    public bool isEven(int value) => value % 2 == 0;

    /// <summary>
    /// Determines whether the specified target is true.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is true; otherwise, <c>false</c>.</returns>
    public static bool isTrue(object target) => target is true;

    /// <summary>
    /// Determines whether the specified target is falsy.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is falsy; otherwise, <c>false</c>.</returns>
    public static bool isFalsy(object target)
    {
        if (target == null || target == JsNull.Value)
            return true;
        if (target is string s)
            return string.IsNullOrEmpty(s);
        if (target is bool b)
            return !b;
        if (target is int i)
            return i == 0;
        if (target is long l)
            return l == 0;
        if (target is double d)
            return d == 0 || double.IsNaN(d);

        return false;
    }



    /// <summary>
    /// Determines whether the specified test is null.
    /// </summary>
    /// <param name="test">The test.</param>
    /// <returns><c>true</c> if the specified test is null; otherwise, <c>false</c>.</returns>
    public bool isNull(object test) => ViewUtils.IsNull(test);




    /// <summary>
    /// Determines whether [is na n] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if [is na n] [the specified value]; otherwise, <c>false</c>.</returns>
    public bool isNaN(double value) => double.IsNaN(value);
    /// <summary>
    /// Determines whether the specified value is infinity.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if the specified value is infinity; otherwise, <c>false</c>.</returns>
    public bool isInfinity(double value) => double.IsInfinity(value);

    /// <summary>
    /// Ifs the not exists.
    /// </summary>
    /// <param name="returnTarget">The return target.</param>
    /// <param name="test">The test.</param>
    /// <returns>System.Object.</returns>
    public object ifNotExists(object returnTarget, object test) => isNull(test) ? returnTarget : StopExecution.Value;
    /// <summary>
    /// Ifs the no.
    /// </summary>
    /// <param name="returnTarget">The return target.</param>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public object ifNo(object returnTarget, object target) => target == null ? returnTarget : StopExecution.Value;
    /// <summary>
    /// Ifs the not empty.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public object ifNotEmpty(object target) => isEmpty(target) ? StopExecution.Value : target;
    /// <summary>
    /// Ifs the not empty.
    /// </summary>
    /// <param name="returnTarget">The return target.</param>
    /// <param name="test">The test.</param>
    /// <returns>System.Object.</returns>
    public object ifNotEmpty(object returnTarget, object test) => isEmpty(test) ? StopExecution.Value : returnTarget;
    /// <summary>
    /// Ifs the empty.
    /// </summary>
    /// <param name="returnTarget">The return target.</param>
    /// <param name="test">The test.</param>
    /// <returns>System.Object.</returns>
    public object ifEmpty(object returnTarget, object test) => isEmpty(test) ? returnTarget : StopExecution.Value;
    /// <summary>
    /// Ifs the true.
    /// </summary>
    /// <param name="returnTarget">The return target.</param>
    /// <param name="test">The test.</param>
    /// <returns>System.Object.</returns>
    public object ifTrue(object returnTarget, object test) => isTrue(test) ? returnTarget : StopExecution.Value;
    /// <summary>
    /// Ifs the false.
    /// </summary>
    /// <param name="returnTarget">The return target.</param>
    /// <param name="test">The test.</param>
    /// <returns>System.Object.</returns>
    public object ifFalse(object returnTarget, object test) => !isTrue(test) ? returnTarget : StopExecution.Value;

    /// <summary>
    /// Determines whether the specified target is empty.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is empty; otherwise, <c>false</c>.</returns>
    public bool isEmpty(object target)
    {
        if (isNull(target))
            return true;

        if (target is string s)
            return s == string.Empty;

        if (target is IEnumerable e)
            return !e.GetEnumerator().MoveNext();

        return false;
    }


    /// <summary>
    /// Ends this instance.
    /// </summary>
    /// <returns>StopExecution.</returns>
    public StopExecution end() => StopExecution.Value;
    /// <summary>
    /// Ends the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="ignore">The ignore.</param>
    /// <returns>Task.</returns>
    public Task end(ScriptScopeContext scope, object ignore) => TypeConstants.EmptyTask;
    /// <summary>
    /// Ends the specified ignore.
    /// </summary>
    /// <param name="ignore">The ignore.</param>
    /// <returns>StopExecution.</returns>
    public StopExecution end(object ignore) => StopExecution.Value;

    /// <summary>
    /// Determines whether the specified target is long.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is long; otherwise, <c>false</c>.</returns>
    public bool isLong(object target) => target is long;
    /// <summary>
    /// Determines whether the specified target is integer.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is integer; otherwise, <c>false</c>.</returns>
    public bool isInteger(object target) => target?.GetType().IsIntegerType() == true;

    /// <summary>
    /// Determines whether the specified target is bool.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is bool; otherwise, <c>false</c>.</returns>
    public bool isBool(object target) => target is bool;
    /// <summary>
    /// Determines whether the specified target is list.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is list; otherwise, <c>false</c>.</returns>
    public bool isList(object target) => target is IEnumerable and not IDictionary and not string;
    /// <summary>
    /// Determines whether the specified target is enumerable.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is enumerable; otherwise, <c>false</c>.</returns>
    public bool isEnumerable(object target) => target is IEnumerable;
    /// <summary>
    /// Determines whether the specified target is dictionary.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is dictionary; otherwise, <c>false</c>.</returns>
    public bool isDictionary(object target) => target is IDictionary;
    /// <summary>
    /// Determines whether the specified target is character.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is character; otherwise, <c>false</c>.</returns>
    public bool isChar(object target) => target is char;
    /// <summary>
    /// Determines whether the specified target is chars.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is chars; otherwise, <c>false</c>.</returns>
    public bool isChars(object target) => target is char[];
    /// <summary>
    /// Determines whether the specified target is byte.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is byte; otherwise, <c>false</c>.</returns>
    public bool isByte(object target) => target is byte;
    /// <summary>
    /// Determines whether the specified target is bytes.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is bytes; otherwise, <c>false</c>.</returns>
    public bool isBytes(object target) => target is byte[];

    /// <summary>
    /// Determines whether the specified target is type.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="typeName">Name of the type.</param>
    /// <returns><c>true</c> if the specified target is type; otherwise, <c>false</c>.</returns>
    public bool isType(object target, string typeName) => typeName.EqualsIgnoreCase(target?.GetType().Name);
    /// <summary>
    /// Determines whether the specified target is number.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is number; otherwise, <c>false</c>.</returns>
    public bool isNumber(object target) => target?.GetType().IsNumericType() == true;

    /// <summary>
    /// Determines whether the specified target is enum.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is enum; otherwise, <c>false</c>.</returns>
    public bool isEnum(object target) => target?.GetType().IsEnum == true;
    /// <summary>
    /// Determines whether the specified target is array.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is array; otherwise, <c>false</c>.</returns>
    public bool isArray(object target) => target?.GetType().IsArray == true;
    /// <summary>
    /// Determines whether [is anon object] [the specified target].
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if [is anon object] [the specified target]; otherwise, <c>false</c>.</returns>
    public bool isAnonObject(object target) => target?.GetType().IsAnonymousType() == true;
    /// <summary>
    /// Determines whether the specified target is class.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is class; otherwise, <c>false</c>.</returns>
    public bool isClass(object target) => target?.GetType().IsClass == true;
    /// <summary>
    /// Determines whether [is value type] [the specified target].
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if [is value type] [the specified target]; otherwise, <c>false</c>.</returns>
    public bool isValueType(object target) => target?.GetType().IsValueType == true;
    /// <summary>
    /// Determines whether the specified target is dto.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is dto; otherwise, <c>false</c>.</returns>
    public bool isDto(object target) => target?.GetType().IsDto() == true;
    /// <summary>
    /// Determines whether the specified target is tuple.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target is tuple; otherwise, <c>false</c>.</returns>
    public bool isTuple(object target) => target?.GetType().IsTuple() == true;

    /// <summary>
    /// Instances the of.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public bool instanceOf(object target, object type)
    {
        if (target == null || type == null)
            return target == type;

        Type t = null;
        if (type is string typeName)
        {
            var protectedScripts = Context.ProtectedMethods;
            if (protectedScripts != null)
                t = protectedScripts.assertTypeOf(typeName);
            else
                return target.GetType().Name == typeName;
        }
        t ??= type as Type
              ?? throw new NotSupportedException(
                  $"{nameof(instanceOf)} expects Type or Type Name but was {type.GetType().Name}");

        return t.IsInstanceOfType(target);
    }

    /// <summary>
    /// Lengthes the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Int32.</returns>
    public int length(object target) => target is IEnumerable e ? e.Cast<object>().Count() : 0;

    /// <summary>
    /// Ors the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool OR(object lhs, object rhs) => isTrue(lhs) || isTrue(rhs);
    /// <summary>
    /// Ands the specified LHS.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool AND(object lhs, object rhs) => isTrue(lhs) && isTrue(rhs);

    /// <summary>
    /// Equalses the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool equals(object target, object other) =>
        target == null || other == null
            ? target == other
            : target.GetType() == other.GetType()
                ? target.Equals(other)
                : target.Equals(other.ConvertTo(target.GetType()));

    /// <summary>
    /// Nots the equals.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool notEquals(object target, object other) => !equals(target, other);

    /// <summary>
    /// Greaters the than.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool greaterThan(object target, object other) => compareTo(target, other, i => i > 0);
    /// <summary>
    /// Greaters the than equal.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool greaterThanEqual(object target, object other) => compareTo(target, other, i => i >= 0);
    /// <summary>
    /// Lesses the than.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool lessThan(object target, object other) => compareTo(target, other, i => i < 0);
    /// <summary>
    /// Lesses the than equal.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool lessThanEqual(object target, object other) => compareTo(target, other, i => i <= 0);

    //aliases
    /// <summary>
    /// Nots the specified target.
    /// </summary>
    /// <param name="target">if set to <c>true</c> [target].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool not(bool target) => !target;
    /// <summary>
    /// Eqs the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool eq(object target, object other) => equals(target, other);
    /// <summary>
    /// Nots the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool not(object target, object other) => notEquals(target, other);
    /// <summary>
    /// Gts the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool gt(object target, object other) => greaterThan(target, other);
    /// <summary>
    /// Gtes the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool gte(object target, object other) => greaterThanEqual(target, other);
    /// <summary>
    /// Lts the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool lt(object target, object other) => lessThan(target, other);
    /// <summary>
    /// Ltes the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool lte(object target, object other) => lessThanEqual(target, other);

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="other">The other.</param>
    /// <param name="fn">The function.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentNullException">nameof(target)</exception>
    /// <exception cref="ArgumentNullException">nameof(other)</exception>
    /// <exception cref="NotSupportedException">$"{target} is not IComparable</exception>
    /// <exception cref="System.ArgumentNullException">target</exception>
    /// <exception cref="System.ArgumentNullException">other</exception>
    /// <exception cref="System.NotSupportedException"></exception>
    static internal bool compareTo(object target, object other, Func<int, bool> fn)
    {
        if (target == null || target == JsNull.Value)
            throw new ArgumentNullException(nameof(target));
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (target is IComparable c)
        {
            return target.GetType() == other.GetType()
                       ? fn(c.CompareTo(other))
                       : fn(c.CompareTo(other.ConvertTo(target.GetType())));
        }

        throw new NotSupportedException($"{target} is not IComparable");
    }

    /// <summary>
    /// Echoes the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public object echo(object value) => value;
    /// <summary>
    /// Passes the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IRawString.</returns>
    public IRawString pass(string target) => ("{{ " + target + " }}").ToRawString();

    /// <summary>
    /// Joins the specified values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>System.String.</returns>
    public string join(IEnumerable<object> values) => join(values, ",");
    /// <summary>
    /// Joinlns the specified values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>System.String.</returns>
    public string joinln(IEnumerable<object> values) => join(values, "\n");
    /// <summary>
    /// Joins the specified values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns>System.String.</returns>
    public string join(IEnumerable<object> values, string delimiter) => values.Map(x => x.AsString()).Join(delimiter);

    /// <summary>
    /// Reverses the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="original">The original.</param>
    /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
    public IEnumerable<object> reverse(ScriptScopeContext scope, IEnumerable<object> original) => original.Reverse();

    /// <summary>
    /// Prepends to.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult prependTo(ScriptScopeContext scope, string value, object argExpr) =>
        prependToArgs(scope, nameof(prependTo), value, argExpr, scope.ScopedParams);

    /// <summary>
    /// Prepends to global.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult prependToGlobal(ScriptScopeContext scope, string value, object argExpr) =>
        prependToArgs(scope, nameof(prependToGlobal), value, argExpr, scope.PageResult.Args);

    /// <summary>
    /// Prepends to arguments.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>IgnoreResult.</returns>
    private IgnoreResult prependToArgs(ScriptScopeContext scope, string filterName, string value, object argExpr, IDictionary<string, object> args)
    {
        if (value == null)
            return IgnoreResult.Value;

        var varName = GetVarNameFromStringOrArrowExpression(filterName, argExpr);

        if (args.TryGetValue(varName, out object oString))
        {
            if (oString is string s)
            {
                args[varName] = value + s;
            }
        }
        else
        {
            args[varName] = value;
        }

        return IgnoreResult.Value;
    }

    /// <summary>
    /// Appends to.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult appendTo(ScriptScopeContext scope, string value, object argExpr) =>
        appendToArgs(scope, nameof(appendTo), value, argExpr, scope.ScopedParams);

    /// <summary>
    /// Appends to global.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult appendToGlobal(ScriptScopeContext scope, string value, object argExpr) =>
        appendToArgs(scope, nameof(appendToGlobal), value, argExpr, scope.PageResult.Args);

    /// <summary>
    /// Appends to arguments.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>IgnoreResult.</returns>
    private IgnoreResult appendToArgs(ScriptScopeContext scope, string filterName, string value, object argExpr, Dictionary<string, object> args)
    {
        if (value == null)
            return IgnoreResult.Value;

        var varName = GetVarNameFromStringOrArrowExpression(filterName, argExpr);

        if (args.TryGetValue(varName, out object oString))
        {
            if (oString is string s)
            {
                args[varName] = s + value;
            }
        }
        else
        {
            args[varName] = value;
        }

        return IgnoreResult.Value;
    }

    /// <summary>
    /// Adds to start.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult addToStart(ScriptScopeContext scope, object value, object argExpr) =>
        addToStartArgs(scope, nameof(addToStart), value, argExpr, scope.ScopedParams);

    /// <summary>
    /// Adds to start global.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult addToStartGlobal(ScriptScopeContext scope, object value, object argExpr) =>
        addToStartArgs(scope, nameof(addToStartGlobal), value, argExpr, scope.PageResult.Args);

    /// <summary>
    /// Adds to start arguments.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>IgnoreResult.</returns>
    /// <exception cref="NotSupportedException">nameof(addToStart) + " can only add to an IEnumerable not a " + collection.GetType().Name</exception>
    /// <exception cref="System.NotSupportedException">addToStart</exception>
    private IgnoreResult addToStartArgs(ScriptScopeContext scope, string filterName, object value, object argExpr, Dictionary<string, object> args)
    {
        if (value == null)
            return IgnoreResult.Value;

        var varName = GetVarNameFromStringOrArrowExpression(filterName, argExpr);

        if (args.TryGetValue(varName, out object collection))
        {
            if (collection is IList l)
            {
                l.Insert(0, value);
            }
            else if (collection is IEnumerable e and not string)
            {
                var to = new List<object> { value };
                foreach (var item in e)
                {
                    to.Add(item);
                }
                args[varName] = to;
            }
            else
            {
                throw new NotSupportedException(nameof(addToStart) + " can only add to an IEnumerable not a " + collection.GetType().Name);
            }
        }
        else
        {
            if (value is IEnumerable and not string)
                args[varName] = value;
            else
                args[varName] = new List<object> { value };
        }

        return IgnoreResult.Value;
    }

    /// <summary>
    /// Adds to.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult addTo(ScriptScopeContext scope, object value, object argExpr) =>
        addToArgs(scope, nameof(addTo), value, argExpr, scope.ScopedParams);

    /// <summary>
    /// Adds to global.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult addToGlobal(ScriptScopeContext scope, object value, object argExpr) =>
        addToArgs(scope, nameof(addToGlobal), value, argExpr, scope.PageResult.Args);

    /// <summary>
    /// Adds to arguments.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExprOrCollection">The argument expr or collection.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>IgnoreResult.</returns>
    /// <exception cref="NotSupportedException">filterName + " can only add to an IEnumerable not a " + collection.GetType().Name</exception>
    /// <exception cref="System.NotSupportedException"></exception>
    private IgnoreResult addToArgs(ScriptScopeContext scope, string filterName, object value, object argExprOrCollection, Dictionary<string, object> args)
    {
        if (value == null)
            return IgnoreResult.Value;

        var varName = GetVarNameFromStringOrArrowExpression(filterName, argExprOrCollection);
        if (args.TryGetValue(varName, out object collection))
        {
            if (TryAddToCollection(collection, value)) { }
            else if (collection is IEnumerable e and not string)
            {
                var to = new List<object>();
                foreach (var item in e)
                {
                    to.Add(item);
                }
                if (value is IEnumerable eValues and not string)
                {
                    foreach (var item in eValues)
                    {
                        to.Add(item);
                    }
                }
                else
                {
                    to.Add(value);
                }
                args[varName] = to;
            }
            else
            {
                throw new NotSupportedException(filterName + " can only add to an IEnumerable not a " + collection.GetType().Name);
            }
        }
        else
        {
            if (value is IEnumerable && !(value is string || value is IDictionary))
                args[varName] = value;
            else
                args[varName] = new List<object> { value };
        }

        return IgnoreResult.Value;
    }

    /// <summary>
    /// Adds the item.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotSupportedException">$"{nameof(addItem)} can only add to an ICollection not a '{collection.GetType().Name}'</exception>
    /// <exception cref="System.NotSupportedException"></exception>
    public object addItem(object collection, object value)
    {
        if (collection == null)
            return null;

        if (!TryAddToCollection(collection, value))
            throw new NotSupportedException($"{nameof(addItem)} can only add to an ICollection not a '{collection.GetType().Name}'");

        return collection;
    }

    /// <summary>
    /// Puts value in dictionary at key
    /// </summary>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>value</returns>
    public object putItem(IDictionary dictionary, object key, object value)
    {
        if (dictionary == null)
            return null;

        dictionary[key] = value;

        return value;
    }

    /// <summary>
    /// Tries the add to collection.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private static bool TryAddToCollection(object collection, object value)
    {
        if (collection is IList l)
        {
            if (value is IEnumerable e && !(value is string || value is IDictionary))
            {
                foreach (var item in e)
                {
                    l.Add(item);
                }
            }
            else
            {
                l.Add(value);
            }
        }
        else if (collection is IDictionary d)
        {
            if (value is KeyValuePair<string, object> kvp)
            {
                d[kvp.Key] = kvp.Value;
            }
            else if (value is IEnumerable<KeyValuePair<string, object>> kvps)
            {
                foreach (var entry in kvps)
                {
                    d[entry.Key] = entry.Value;
                }
            }
            else if (value is IDictionary dValue)
            {
                foreach (var key in dValue.Keys)
                {
                    d[key] = dValue[key];
                }
            }
        }
        else if (collection is NameValueCollection nvc)
        {
            if (value is KeyValuePair<string, object> kvp)
            {
                nvc[kvp.Key] = kvp.Value?.ToString();
            }
            else if (value is IEnumerable<KeyValuePair<string, object>> kvps)
            {
                foreach (var entry in kvps)
                {
                    nvc[entry.Key] = entry.Value?.ToString();
                }
            }
            else if (value is IDictionary dValue)
            {
                foreach (string key in dValue.Keys)
                {
                    nvc[key] = dValue[key]?.ToString();
                }
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Assigns the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public object assign(ScriptScopeContext scope, string argExpr, object value) =>
        assignArgs(scope, argExpr, value, scope.ScopedParams);

    /// <summary>
    /// Assigns the arguments.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <param name="value">The value.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotSupportedException">$"Cannot assign to non-existing '{targetName}' in {argExpr}</exception>
    /// <exception cref="System.NotSupportedException">Cannot assign to non-existing '{targetName}' in {argExpr}</exception>
    private object assignArgs(ScriptScopeContext scope, string argExpr, object value, Dictionary<string, object> args) //from filter
    {
        var targetEndPos = argExpr.IndexOfAny(['.', '[']);
        if (targetEndPos == -1)
        {
            args[argExpr] = value;
        }
        else
        {
            var targetName = argExpr.Substring(0, targetEndPos);
            if (!args.TryGetValue(targetName, out object target))
                throw new NotSupportedException($"Cannot assign to non-existing '{targetName}' in {argExpr}");

            scope.InvokeAssignExpression(argExpr, target, value);
        }

        return value;
    }

    // Shorter Alias for assignTo:
    /// <summary>
    /// To the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult to(ScriptScopeContext scope, object value, object argExpr)
    {
        var varName = GetVarNameFromStringOrArrowExpression(nameof(to), argExpr);

        scope.ScopedParams[varName] = value;
        return IgnoreResult.Value;
    }

    /// <summary>
    /// Assigns to.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult assignTo(ScriptScopeContext scope, object value, object argExpr)
    {
        var varName = GetVarNameFromStringOrArrowExpression(nameof(assignTo), argExpr);

        scope.ScopedParams[varName] = value;
        return IgnoreResult.Value;
    }

    // Shorter Alias for assignToGlobal:
    /// <summary>
    /// To the global.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult toGlobal(ScriptScopeContext scope, object value, object argExpr)
    {
        var varName = GetVarNameFromStringOrArrowExpression(nameof(toGlobal), argExpr);

        scope.PageResult.Args[varName] = value;
        return IgnoreResult.Value;
    }

    /// <summary>
    /// Assigns to global.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult assignToGlobal(ScriptScopeContext scope, object value, object argExpr)
    {
        var varName = GetVarNameFromStringOrArrowExpression(nameof(assignToGlobal), argExpr);

        scope.PageResult.Args[varName] = value;
        return IgnoreResult.Value;
    }

    // Shorter Alias for assignTo:
    /// <summary>
    /// To the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>Task.</returns>
    public Task to(ScriptScopeContext scope, object argExpr) =>
        assignToArgs(scope, nameof(to), argExpr, scope.ScopedParams);

    /// <summary>
    /// Assigns to.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>Task.</returns>
    public Task assignTo(ScriptScopeContext scope, object argExpr) =>
        assignToArgs(scope, nameof(assignTo), argExpr, scope.ScopedParams);

    // Shorter Alias for assignToGlobal:
    /// <summary>
    /// To the global.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>Task.</returns>
    public Task toGlobal(ScriptScopeContext scope, object argExpr) =>
        assignToArgs(scope, nameof(toGlobal), argExpr, scope.PageResult.Args);

    /// <summary>
    /// Assigns to global.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>Task.</returns>
    public Task assignToGlobal(ScriptScopeContext scope, object argExpr) =>
        assignToArgs(scope, nameof(assignToGlobal), argExpr, scope.PageResult.Args);

    /// <summary>
    /// Assigns to arguments.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>Task.</returns>
    private Task assignToArgs(ScriptScopeContext scope, string filterName, object argExpr, Dictionary<string, object> args) //from context filter
    {
        var varName = GetVarNameFromStringOrArrowExpression(nameof(assignToGlobal), argExpr);

        var ms = (MemoryStream)scope.OutputStream;
        var value = ms.ReadToEnd();
        scope.ScopedParams[varName] = value;
        ms.SetLength(0); //just capture output, don't write anything to the ResponseStream
        return TypeConstants.EmptyTask;
    }

    /// <summary>
    /// Gets the variable name from string or arrow expression.
    /// </summary>
    /// <param name="filterName">Name of the filter.</param>
    /// <param name="argExpr">The argument expr.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="ArgumentNullException">filterName</exception>
    /// <exception cref="NotSupportedException">$"{filterName} expression must return an identifer</exception>
    /// <exception cref="NotSupportedException">$"{filterName} requires a string or expression identifier but was instead '{argExpr.GetType().Name}'</exception>
    /// <exception cref="System.ArgumentNullException"></exception>
    /// <exception cref="System.NotSupportedException"></exception>
    /// <exception cref="System.NotSupportedException"></exception>
    public static string GetVarNameFromStringOrArrowExpression(string filterName, object argExpr)
    {
        if (argExpr == null)
            throw new ArgumentNullException(filterName);

        if (argExpr is JsArrowFunctionExpression arrowExpr)
        {
            if (arrowExpr.Body is not JsIdentifier identifier)
                throw new NotSupportedException($"{filterName} expression must return an identifer");

            return identifier.Name;
        }

        if (argExpr is string varName)
            return varName;

        throw new NotSupportedException($"{filterName} requires a string or expression identifier but was instead '{argExpr.GetType().Name}'");
    }

    /// <summary>
    /// Buffers the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>Task.</returns>
    public Task buffer(ScriptScopeContext scope, object target)
    {
        var ms = (MemoryStream)scope.OutputStream;
        return TypeConstants.EmptyTask;
    }

    /// <summary>
    /// Partials the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <returns>Task.</returns>
    public Task partial(ScriptScopeContext scope, object target) => partial(scope, target, null);
    /// <summary>
    /// Partials the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="scopedParams">The scoped parameters.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    /// <exception cref="FileNotFoundException">$"Partial was not found: '{pageName}'</exception>
    /// <exception cref="System.IO.FileNotFoundException">Partial was not found: '{pageName}'</exception>
    public async Task partial(ScriptScopeContext scope, object target, object scopedParams)
    {
        var pageName = target.ToString();
        var pageParams = scope.AssertOptions(nameof(partial), scopedParams);

        if (!scope.TryGetPage(pageName, out var page, out var codePage))
        {
            //Allow partials starting with '_{name}-partial' to be referenced without boilerplate
            if (pageName[0] != '_')
            {
                if (!scope.TryGetPage('_' + pageName + "-partial", out page, out codePage))
                    throw new FileNotFoundException($"Partial was not found: '{pageName}'");
            }
        }

        if (page != null)
            await page.Init();

        if (page is SharpPartialPage) // make partial block args available in scope
        {
            foreach (var pageArg in page.Args)
            {
                pageParams[pageArg.Key] = pageArg.Value;
            }
        }

        pageParams[ScriptConstants.It] = pageParams;
        pageParams[ScriptConstants.PartialArg] = page;

        await scope.WritePageAsync(page, codePage, pageParams);
    }

    /// <summary>
    /// Selects the each.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="items">The items.</param>
    /// <returns>Task.</returns>
    public Task selectEach(ScriptScopeContext scope, object target, object items) => selectEach(scope, target, items, null);
    /// <summary>
    /// Selects the each.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="items">The items.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    /// <exception cref="ArgumentException">$"{nameof(selectEach)} in '{scope.Page.VirtualPath}' requires an IEnumerable, but received a '{items.GetType().Name}' instead</exception>
    /// <exception cref="System.ArgumentException"></exception>
    public async Task selectEach(ScriptScopeContext scope, object target, object items, object scopeOptions)
    {
        if (items is IEnumerable objs)
        {
            var scopedParams = scope.GetParamsWithItemBinding(nameof(select), scopeOptions, out string itemBinding);

            var i = 0;
            var itemScope = scope.CreateScopedContext(target.ToString(), scopedParams);
            foreach (var item in objs)
            {
                itemScope.AddItemToScope(itemBinding, item, i++);
                await itemScope.WritePageAsync();
            }
        }
        else if (items != null)
        {
            throw new ArgumentException($"{nameof(selectEach)} in '{scope.Page.VirtualPath}' requires an IEnumerable, but received a '{items.GetType().Name}' instead");
        }
    }

    /// <summary>
    /// To the string.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public string toString(object target) => target?.ToString();
    /// <summary>
    /// Ases the string.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.String.</returns>
    public string asString(object target) => target.AsString();
    /// <summary>
    /// To the list.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>List&lt;System.Object&gt;.</returns>
    public List<object> toList(IEnumerable target) => target.Map(x => x);
    /// <summary>
    /// To the array.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object[].</returns>
    public object[] toArray(IEnumerable target) => target.Map(x => x).ToArray();

    /// <summary>
    /// Froms the character code.
    /// </summary>
    /// <param name="charCode">The character code.</param>
    /// <returns>System.Char.</returns>
    public char fromCharCode(int charCode) => Convert.ToChar(charCode);
    /// <summary>
    /// To the character.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Char.</returns>
    public char toChar(object target) => target is string {Length: 1} s ? s[0] : target.ConvertTo<char>();
    /// <summary>
    /// To the chars.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Char[].</returns>
    public char[] toChars(object target) => target is string s
                                                ? s.ToCharArray()
                                                : target is IEnumerable<object> objects
                                                    ? objects.Where(x => x != null).Select(x => x.ToString()[0]).ToArray()
                                                    : target.ConvertTo<char[]>();

    /// <summary>
    /// To the int.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Int32.</returns>
    public int toInt(object target) => target.ConvertTo<int>();
    /// <summary>
    /// To the long.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Int64.</returns>
    public long toLong(object target) => target.ConvertTo<long>();

    /// <summary>
    /// To the double.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Double.</returns>
    public double toDouble(object target) => target.ConvertTo<double>();
    /// <summary>
    /// To the decimal.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Decimal.</returns>
    public decimal toDecimal(object target) => target.ConvertTo<decimal>();
    /// <summary>
    /// To the bool.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool toBool(object target) => target.ConvertTo<bool>();
    /// <summary>
    /// To the date time.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>DateTime.</returns>
    public DateTime toDateTime(object target) => target.ConvertTo<DateTime>();
    /// <summary>
    /// Dates the specified year.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <returns>DateTime.</returns>
    public DateTime date(int year, int month, int day) => new(year, month, day);
    /// <summary>
    /// Dates the specified year.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="secs">The secs.</param>
    /// <returns>DateTime.</returns>
    public DateTime date(int year, int month, int day, int hour, int min, int secs) => new(year, month, day, hour, min, secs);
    /// <summary>
    /// To the time span.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>TimeSpan.</returns>
    public TimeSpan toTimeSpan(object target) => target.ConvertTo<TimeSpan>();
    /// <summary>
    /// Times the specified hours.
    /// </summary>
    /// <param name="hours">The hours.</param>
    /// <param name="mins">The mins.</param>
    /// <param name="secs">The secs.</param>
    /// <returns>TimeSpan.</returns>
    public TimeSpan time(int hours, int mins, int secs) => new(0, hours, mins, secs);
    /// <summary>
    /// Times the specified days.
    /// </summary>
    /// <param name="days">The days.</param>
    /// <param name="hours">The hours.</param>
    /// <param name="mins">The mins.</param>
    /// <param name="secs">The secs.</param>
    /// <returns>TimeSpan.</returns>
    public TimeSpan time(int days, int hours, int mins, int secs) => new(days, hours, mins, secs);

    /// <summary>
    /// Pairs the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>KeyValuePair&lt;System.String, System.Object&gt;.</returns>
    public KeyValuePair<string, object> pair(string key, object value) => new(key, value);

    /// <summary>
    /// To the keys.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    /// <exception cref="NotSupportedException">nameof(toKeys) + " expects an IDictionary or List of KeyValuePairs but received: " + target.GetType().Name</exception>
    /// <exception cref="System.NotSupportedException">toKeys</exception>
    public List<string> toKeys(object target)
    {
        if (target == null)
            return null;

        if (target is IDictionary<string, object> objDictionary)
            return objDictionary.Keys.ToList();
        if (target is IDictionary dictionary)
            return dictionary.Keys.Map(x => x.ToString());

        if (target is IEnumerable<KeyValuePair<string, object>> kvps)
        {
            var to = new List<string>();
            foreach (var kvp in kvps)
            {
                to.Add(kvp.Key);
            }
            return to;
        }
        if (target is IEnumerable<KeyValuePair<string, string>> stringKvps)
        {
            var to = new List<string>();
            foreach (var kvp in stringKvps)
            {
                to.Add(kvp.Key);
            }
            return to;
        }
        throw new NotSupportedException(nameof(toKeys) + " expects an IDictionary or List of KeyValuePairs but received: " + target.GetType().Name);
    }

    /// <summary>
    /// To the values.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>List&lt;System.Object&gt;.</returns>
    /// <exception cref="NotSupportedException">nameof(toValues) + " expects an IDictionary or List of KeyValuePairs but received: " + target.GetType().Name</exception>
    /// <exception cref="System.NotSupportedException">toValues</exception>
    public List<object> toValues(object target)
    {
        if (target == null)
            return null;

        if (target is IDictionary<string, object> objDictionary)
            return objDictionary.Values.ToList();
        if (target is IDictionary dictionary)
            return dictionary.Values.Map(x => x);

        if (target is IEnumerable<KeyValuePair<string, object>> kvps)
        {
            var to = new List<object>();
            foreach (var kvp in kvps)
            {
                to.Add(kvp.Value);
            }
            return to;
        }
        if (target is IEnumerable<KeyValuePair<string, string>> stringKvps)
        {
            var to = new List<object>();
            foreach (var kvp in stringKvps)
            {
                to.Add(kvp.Value);
            }
            return to;
        }
        throw new NotSupportedException(nameof(toValues) + " expects an IDictionary or List of KeyValuePairs but received: " + target.GetType().Name);
    }

    /// <summary>
    /// Asserts the within maximum quota.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="NotSupportedException">$"{value} exceeds Max Quota of {maxQuota}. \nMaxQuota can be changed in `ScriptContext.MaxQuota`.</exception>
    /// <exception cref="System.NotSupportedException"></exception>
    public int AssertWithinMaxQuota(int value)
    {
        var maxQuota = Context.MaxQuota;
        if (value > maxQuota)
            throw new NotSupportedException($"{value} exceeds Max Quota of {maxQuota}. \nMaxQuota can be changed in `ScriptContext.MaxQuota`.");

        return value;
    }

    /// <summary>
    /// To the dictionary.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>Dictionary&lt;System.Object, System.Object&gt;.</returns>
    public Dictionary<object, object> toDictionary(ScriptScopeContext scope, object target, object expression) => toDictionary(scope, target, expression, null);
    /// <summary>
    /// To the dictionary.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>Dictionary&lt;System.Object, System.Object&gt;.</returns>
    public Dictionary<object, object> toDictionary(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(toDictionary));
        var token = scope.AssertExpression(nameof(map), expression, scopeOptions, out var itemBinding);

        scope = scope.Clone();
        return items.ToDictionary(item => token.Evaluate(scope.AddItemToScope(itemBinding, item)));
    }

    /// <summary>
    /// Types the name.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>IRawString.</returns>
    public IRawString typeName(object target) => (target?.GetType().Name ?? "null").ToRawString();

    /// <summary>
    /// Ofs the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>IEnumerable.</returns>
    public IEnumerable of(ScriptScopeContext scope, IEnumerable target, object scopeOptions)
    {
        var items = target.AssertEnumerable(nameof(of));
        var scopedParams = scope.GetParamsWithItemBinding(nameof(of), scopeOptions, out string itemBinding);

        if (scopedParams.TryGetValue("type", out object oType))
        {
            if (oType is string typeName)
                return items.Where(x => x?.GetType().Name == typeName);
            if (oType is Type type)
                return items.Where(x => x?.GetType() == type);
        }

        return items;
    }

    /// <summary>
    /// Does the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Object.</returns>
    public object @do(ScriptScopeContext scope, object expression)
    {
        var token = scope.AssertExpression(nameof(@do), expression, scopeOptions: null, out var itemBinding);
        var result = token.Evaluate(scope);

        return IgnoreResult.Value;
    }

    /// <summary>
    /// Does the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>Task.</returns>
    public Task @do(ScriptScopeContext scope, object target, object expression) => @do(scope, target, expression, null);
    /// <summary>
    /// Does the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>Task.</returns>
    public Task @do(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        if (isNull(target) || target is false)
            return TypeConstants.EmptyTask;

        var token = scope.AssertExpression(nameof(@do), expression, scopeOptions, out var itemBinding);

        if (target is IEnumerable objs and not IDictionary and not string)
        {
            var items = target.AssertEnumerable(nameof(@do));

            var i = 0;
            var eagerItems = items.ToArray(); // assign on array expression can't be within enumerable
            foreach (var item in eagerItems)
            {
                scope.AddItemToScope(itemBinding, item, i++);
                var result = token.Evaluate(scope);
            }
        }
        else
        {
            scope.AddItemToScope(itemBinding, target);
            var result = token.Evaluate(scope);
        }

        return TypeConstants.EmptyTask;
    }

    /// <summary>
    /// Propses the specified o.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> props(object o)
    {
        if (o == null)
            return TypeConstants.EmptyStringList;

        var pis = propTypes(o);
        return pis.Map(x => x.Name).OrderBy(x => x).ToList();
    }

    /// <summary>
    /// Properties the types.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>PropertyInfo[].</returns>
    public PropertyInfo[] propTypes(object o)
    {
        if (o == null)
            return TypeConstants<PropertyInfo>.EmptyArray;

        var type = o as Type ?? o.GetType();

        return type.GetPublicProperties();
    }

    /// <summary>
    /// Fieldses the specified o.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> fields(object o)
    {
        if (o == null)
            return TypeConstants.EmptyStringList;

        var fis = fieldTypes(o);
        return fis.Map(x => x.Name).OrderBy(x => x).ToList();
    }

    /// <summary>
    /// Fields the types.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>FieldInfo[].</returns>
    public FieldInfo[] fieldTypes(object o)
    {
        if (o == null)
            return TypeConstants<FieldInfo>.EmptyArray;

        var type = o as Type ?? o.GetType();

        return type.GetPublicFields();
    }

    /// <summary>
    /// Statics the fields.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> staticFields(object o)
    {
        if (o == null)
            return TypeConstants.EmptyStringList;

        var fis = staticFieldTypes(o);
        return fis.Map(x => x.Name).OrderBy(x => x).ToList();
    }

    /// <summary>
    /// Statics the field types.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>FieldInfo[].</returns>
    public FieldInfo[] staticFieldTypes(object o)
    {
        if (o == null)
            return TypeConstants<FieldInfo>.EmptyArray;

        var type = o as Type ?? o.GetType();

        return type.GetFields(BindingFlags.Static | BindingFlags.Public);
    }

    /// <summary>
    /// Properties the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotSupportedException">$"There is no public Property '{propertyName}' on Type '{target.GetType().Name}'</exception>
    /// <exception cref="System.NotSupportedException">There is no public Property '{propertyName}' on Type '{target.GetType().Name}'</exception>
    public object property(object target, string propertyName)
    {
        if (isNull(target))
            return null;

        var props = TypeProperties.Get(target.GetType());
        var fn = props.GetPublicGetter(propertyName);
        if (fn == null)
            throw new NotSupportedException($"There is no public Property '{propertyName}' on Type '{target.GetType().Name}'");

        var value = fn(target);
        return value;
    }

    /// <summary>
    /// Fields the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotSupportedException">$"There is no public Field '{fieldName}' on Type '{target.GetType().Name}'</exception>
    /// <exception cref="System.NotSupportedException">There is no public Field '{fieldName}' on Type '{target.GetType().Name}'</exception>
    public object field(object target, string fieldName)
    {
        if (isNull(target))
            return null;

        var props = TypeFields.Get(target.GetType());
        var fn = props.GetPublicGetter(fieldName);
        if (fn == null)
            throw new NotSupportedException($"There is no public Field '{fieldName}' on Type '{target.GetType().Name}'");

        var value = fn(target);
        return value;
    }

    /// <summary>
    /// Maps the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="items">The items.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Object.</returns>
    public object map(ScriptScopeContext scope, object items, object expression) => map(scope, items, expression, null);
    /// <summary>
    /// Maps the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Object.</returns>
    public object map(ScriptScopeContext scope, object target, object expression, object scopeOptions)
    {
        var token = scope.AssertExpression(nameof(map), expression, scopeOptions, out var itemBinding);

        scope = scope.Clone();
        if (target is IEnumerable items and not IDictionary and not string)
        {
            var i = 0;
            return items.Map(item => token.Evaluate(scope.AddItemToScope(itemBinding, item, i++)));
        }

        var result = token.Evaluate(scope.AddItemToScope(itemBinding, target));
        return result;
    }

    /// <summary>
    /// Scopes the vars.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotSupportedException">$"'{nameof(scopeVars)}' expects a Dictionary but received a '{target.GetType().Name}'</exception>
    /// <exception cref="System.NotSupportedException">'{nameof(scopeVars)}' expects a Dictionary but received a '{target.GetType().Name}'</exception>
    public object scopeVars(object target)
    {
        if (isNull(target))
            return null;

        if (target is IDictionary<string, object> g)
            return new ScopeVars(g);

        if (target is IDictionary d)
        {
            var to = new ScopeVars();
            foreach (var key in d.Keys)
            {
                to[key.ToString()] = d[key];
            }
            return to;
        }

        if (target is IEnumerable<KeyValuePair<string, object>> kvps)
        {
            var to = new ScopeVars();
            foreach (var item in kvps)
            {
                to[item.Key] = item.Value;
            }
            return to;
        }

        if (target is IEnumerable e)
        {
            var to = new List<object>();

            foreach (var item in e)
            {
                var toItem = item is IDictionary
                                 ? scopeVars(item)
                                 : item;

                to.Add(toItem);
            }

            return to;
        }

        throw new NotSupportedException($"'{nameof(scopeVars)}' expects a Dictionary but received a '{target.GetType().Name}'");
    }

    /// <summary>
    /// Selects the fields.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="names">The names.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotSupportedException">nameof(selectFields) + " requires an IEnumerable, IDictionary or POCO Target, received instead: " + target.GetType().Name</exception>
    /// <exception cref="NotSupportedException">nameof(selectFields) + " requires a string or [string] or property names, received instead: " + names.GetType().Name</exception>
    /// <exception cref="System.NotSupportedException">selectFields</exception>
    /// <exception cref="System.NotSupportedException">selectFields</exception>
    public object selectFields(object target, object names)
    {
        if (target == null || names == null)
            return null;

        if (target is string || target.GetType().IsValueType)
            throw new NotSupportedException(nameof(selectFields) + " requires an IEnumerable, IDictionary or POCO Target, received instead: " + target.GetType().Name);

        var namesList = names is IEnumerable eKeys
                            ? eKeys.Map(x => x)
                            : null;

        var stringKey = names as string;
        var stringKeys = namesList?.OfType<string>().ToList();
        if (stringKeys.IsEmpty())
            stringKeys = null;

        if (stringKey == null && stringKeys == null)
            throw new NotSupportedException(nameof(selectFields) + " requires a string or [string] or property names, received instead: " + names.GetType().Name);

        if (stringKey?.IndexOf(',') >= 0)
        {
            stringKeys = stringKey.Split(',').Map(x => x.Trim());
            stringKey = null;
        }

        var stringsSet = stringKeys != null
                             ? new HashSet<string>(stringKeys, StringComparer.OrdinalIgnoreCase)
                             : [stringKey];

        var singleItem = target is IDictionary || target is not IEnumerable;
        if (singleItem)
        {
            var objDictionary = target.ToObjectDictionary();

            var to = new Dictionary<string, object>();
            foreach (var key in objDictionary.Keys)
            {
                if (stringsSet.Contains(key))
                    to[key] = objDictionary[key];
            }

            return to;
        }
        else
        {
            var to = new List<Dictionary<string, object>>();
            var e = (IEnumerable)target;
            foreach (var item in e)
            {
                var objDictionary = item.ToObjectDictionary();

                var row = new Dictionary<string, object>();
                foreach (var key in objDictionary.Keys)
                {
                    if (stringsSet.Contains(key))
                        row[key] = objDictionary[key];
                }
                to.Add(row);
            }
            return to;
        }
    }

    /// <summary>
    /// Selects the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="selectTemplate">The select template.</param>
    /// <returns>Task.</returns>
    public Task select(ScriptScopeContext scope, object target, object selectTemplate) => select(scope, target, selectTemplate, null);
    /// <summary>
    /// Selects the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="selectTemplate">The select template.</param>
    /// <param name="scopeOptions">The scope options.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    public async Task select(ScriptScopeContext scope, object target, object selectTemplate, object scopeOptions)
    {
        if (isNull(target))
            return;

        var scopedParams = scope.GetParamsWithItemBinding(nameof(select), scopeOptions, out string itemBinding);
        var template = JsonTypeSerializer.Unescape(selectTemplate.ToString(), removeQuotes: false);
        var itemScope = scope.CreateScopedContext(template, scopedParams);

        if (target is IEnumerable objs and not IDictionary and not string)
        {
            var i = 0;
            foreach (var item in objs)
            {
                itemScope.AddItemToScope(itemBinding, item, i++);
                await itemScope.WritePageAsync();
            }
        }
        else
        {
            itemScope.AddItemToScope(itemBinding, target);
            await itemScope.WritePageAsync();
        }
    }

    /// <summary>
    /// Selects the partial.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="pageName">Name of the page.</param>
    /// <returns>Task.</returns>
    public Task selectPartial(ScriptScopeContext scope, object target, string pageName) => selectPartial(scope, target, pageName, null);
    /// <summary>
    /// Selects the partial.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="target">The target.</param>
    /// <param name="pageName">Name of the page.</param>
    /// <param name="scopedParams">The scoped parameters.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    /// <exception cref="FileNotFoundException">$"Partial was not found: '{pageName}'</exception>
    /// <exception cref="System.IO.FileNotFoundException">Partial was not found: '{pageName}'</exception>
    public async Task selectPartial(ScriptScopeContext scope, object target, string pageName, object scopedParams)
    {
        if (isNull(target))
            return;

        if (!scope.TryGetPage(pageName, out var page, out var codePage))
        {
            //Allow partials starting with '_{name}-partial' to be referenced without boilerplate
            if (pageName[0] != '_')
            {
                if (!scope.TryGetPage('_' + pageName + "-partial", out page, out codePage))
                    throw new FileNotFoundException($"Partial was not found: '{pageName}'");
            }
        }

        if (page != null)
            await page.Init();

        var pageParams = scope.GetParamsWithItemBinding(nameof(selectPartial), page, scopedParams, out string itemBinding);

        if (page is SharpPartialPage) // make partial block args available in scope
        {
            foreach (var pageArg in page.Args)
            {
                pageParams[pageArg.Key] = pageArg.Value;
            }
        }

        pageParams[ScriptConstants.PartialArg] = page;

        scope = scope.Clone();
        if (target is IEnumerable objs and not IDictionary and not string)
        {
            var i = 0;
            foreach (var item in objs)
            {
                scope.AddItemToScope(itemBinding, item, i++);
                await scope.WritePageAsync(page, codePage, pageParams);
            }
        }
        else
        {
            scope.AddItemToScope(itemBinding, target);
            await scope.WritePageAsync(page, codePage, pageParams);
        }
    }

    /// <summary>
    /// Removes the key from dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="keyToRemove">The key to remove.</param>
    /// <returns>System.Object.</returns>
    public object removeKeyFromDictionary(IDictionary dictionary, object keyToRemove)
    {
        var removeKeys = keyToRemove is IEnumerable e and not string
                             ? e.Map(x => x)
                             : null;

        foreach (var key in EnumerableUtils.ToList(dictionary.Keys))
        {
            if (removeKeys != null)
            {
                foreach (var removeKey in removeKeys)
                {
                    if (Equals(key, removeKey))
                        dictionary.Remove(key);
                }
            }
            else if (Equals(key, keyToRemove))
            {
                dictionary.Remove(key);
            }
        }
        return dictionary;
    }

    /// <summary>
    /// Removes the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="keysToRemove">The keys to remove.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotSupportedException">nameof(remove) + " removes keys from a IDictionary or [IDictionary]</exception>
    /// <exception cref="System.NotSupportedException">remove</exception>
    /// <exception cref="System.NotSupportedException">remove</exception>
    public object remove(object target, object keysToRemove)
    {
        var removeKeys = keysToRemove is string s
                             ? (IEnumerable)new[] { s }
                             : keysToRemove is IEnumerable eKeys
                                 ? eKeys.Map(x => x)
                                 : null;

        var stringKeys = removeKeys?.OfType<string>().ToArray();
        if (stringKeys.IsEmpty())
        {
        }

        switch (target)
        {
            case IDictionary d:
                return removeKeyFromDictionary(d, removeKeys);
            case IEnumerable e:
            {
                object first = e.Cast<object>().FirstOrDefault(item => item != null);
                if (first == null)
                    return target;

                var itemType = first.GetType();
                var props = TypeProperties.Get(itemType);

                if (first is not IDictionary)
                    throw new NotSupportedException(nameof(remove) + " removes keys from a IDictionary or [IDictionary]");

                foreach (var item in e)
                {
                    switch (item)
                    {
                        case null:
                            continue;
                        case IDictionary ed:
                            removeKeyFromDictionary(ed, removeKeys);
                            break;
                    }
                }

                break;
            }
            default:
                throw new NotSupportedException(nameof(remove) + " removes keys from a IDictionary or [IDictionary]");
        }

        return target;
    }

    /// <summary>
    /// The internal keys
    /// </summary>
    private readonly static HashSet<string> InternalKeys = [ScriptConstants.It, ScriptConstants.PartialArg];

    /// <summary>
    /// Owns the props.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>System.Object.</returns>
    public object ownProps(IEnumerable<KeyValuePair<string, object>> target)
    {
        var to = new List<KeyValuePair<string, object>>();
        foreach (var entry in target)
        {
            if (InternalKeys.Contains(entry.Key))
                continue;
            to.Add(entry);
        }
        return to;
    }

    /// <summary>
    /// Withouts the keys.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="keys">The keys.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public object withoutKeys(IDictionary<string, object> target, object keys)
    {
        if (keys == null)
            return target;

        var strKeys = keys is string s
                          ? [s]
                          : keys is IEnumerable e
                              ? e.Map(x => x.ToString())
                              : throw new NotSupportedException($"{nameof(withoutKeys)} expects a collection of key names but received ${keys.GetType().Name}");

        var to = new Dictionary<string, object>();
        foreach (var entry in target)
        {
            if (strKeys.Contains(entry.Key))
                continue;

            to[entry.Key] = entry.Value;
        }
        return to;
    }

    /// <summary>
    /// Merges the specified sources.
    /// </summary>
    /// <param name="sources">The sources.</param>
    /// <returns>System.Object.</returns>
    public object merge(object sources) => merge(new Dictionary<string, object>(), sources);
    /// <summary>
    /// Merges the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="sources">The sources.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public object merge(IDictionary<string, object> target, object sources)
    {
        var srcArray = sources is IDictionary<string, object> d
                           ? [d]
                           : sources is List<IDictionary<string, object>> ld
                               ? ld.ToArray()
                               : sources is List<object> lo
                                   ? lo.ToArray()
                                   : sources as object[] ?? throw new NotSupportedException(
                                         $"{nameof(merge)} cannot merge objects of type ${sources.GetType().Name}");

        return target.MergeIntoObjectDictionary(srcArray);
    }

    /// <summary>
    /// Evals the script.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="source">The source.</param>
    /// <returns>Task&lt;System.Object&gt;.</returns>
    public Task<object> evalScript(ScriptScopeContext scope, string source) => evalScript(scope, source, null);
    /// <summary>
    /// Evals the script.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="source">The source.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public async Task<object> evalScript(ScriptScopeContext scope, string source, Dictionary<string, object> args)
    {
        if (string.IsNullOrEmpty(source))
            return null;

        var context = scope.CreateNewContext(args);

        using var ms = MemoryStreamFactory.GetStream();
        var pageResult = new PageResult(context.OneTimePage(source));
        if (args != null)
            pageResult.Args = args;

        await pageResult.WriteToAsync(ms);

        ms.Position = 0;
        var result = ms.ReadToEnd();

        return result;
    }

    /// <summary>
    /// Writes the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="value">The value.</param>
    /// <returns>IgnoreResult.</returns>
    public IgnoreResult write(ScriptScopeContext scope, object value)
    {
        if (value != null)
        {
            var s = value.ToString();
            MemoryProvider.Instance.Write(scope.OutputStream, s.AsMemory());
        }
        return IgnoreResult.Value;
    }

    /// <summary>
    /// Unwraps the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public object unwrap(object value)
    {
        if (value is Task t)
        {
            if (value is Task<object> taskObj)
                return taskObj.GetAwaiter().GetResult();
            return t.GetResult();
        }
        return value;
    }
}

/// <summary>
/// Class DefaultScripts.
/// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
/// Implements the <see cref="ServiceStack.Script.IConfigureScriptContext" />
/// </summary>
/// <seealso cref="ServiceStack.Script.ScriptMethods" />
/// <seealso cref="ServiceStack.Script.IConfigureScriptContext" />
public partial class DefaultScripts //Methods named after common keywords breaks intelli-sense when trying to use them
{
    /// <summary>
    /// Ifs the specified return target.
    /// </summary>
    /// <param name="returnTarget">The return target.</param>
    /// <param name="test">The test.</param>
    /// <returns>System.Object.</returns>
    public object @if(object returnTarget, object test) => test is true ? returnTarget : StopExecution.Value;

    /// <summary>
    /// Throws the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="message">The message.</param>
    /// <returns>System.Object.</returns>
    public object @throw(ScriptScopeContext scope, string message) => new Exception(message).InStopFilter(scope, null);
    /// <summary>
    /// Throws the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="message">The message.</param>
    /// <param name="options">The options.</param>
    /// <returns>System.Object.</returns>
    public object @throw(ScriptScopeContext scope, string message, object options) => new Exception(message).InStopFilter(scope, options);
}