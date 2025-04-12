// ***********************************************************************
// <copyright file="ExpressionUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ServiceStack.Logging;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack;

/// <summary>
/// Class ExpressionUtils.
/// </summary>
public static class ExpressionUtils
{
    /// <summary>
    /// The log
    /// </summary>
    private readonly static ILog Log = LogManager.GetLogger(typeof(ExpressionUtils));

    /// <summary>
    /// Converts to propertyinfo.
    /// </summary>
    /// <param name="fieldExpr">The field expr.</param>
    /// <returns>PropertyInfo.</returns>
    public static PropertyInfo ToPropertyInfo(this Expression fieldExpr)
    {
        return ToPropertyInfo(fieldExpr as LambdaExpression)
               ?? ToPropertyInfo(fieldExpr as MemberExpression);
    }

    /// <summary>
    /// Converts to propertyinfo.
    /// </summary>
    /// <param name="lambda">The lambda.</param>
    /// <returns>PropertyInfo.</returns>
    public static PropertyInfo ToPropertyInfo(LambdaExpression lambda)
    {
        return lambda?.Body.NodeType == ExpressionType.MemberAccess
                   ? ToPropertyInfo(lambda.Body as MemberExpression)
                   : null;
    }

    /// <summary>
    /// Converts to propertyinfo.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>PropertyInfo.</returns>
    public static PropertyInfo ToPropertyInfo(MemberExpression m)
    {
        var pi = m?.Member as PropertyInfo;
        return pi;
    }

    /// <summary>
    /// Gets the name of the member.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldExpr">The field expr.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.NotSupportedException">Expected Property Expression</exception>
    public static string GetMemberName<T>(Expression<Func<T, object>> fieldExpr)
    {
        var m = GetMemberExpression(fieldExpr);
        if (m != null)
        {
            return m.Member.Name;
        }

        throw new NotSupportedException("Expected Property Expression");
    }

    /// <summary>
    /// Gets the member expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expr">The expr.</param>
    /// <returns>MemberExpression.</returns>
    public static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> expr)
    {
        var member = expr.Body as MemberExpression;
        var unary = expr.Body as UnaryExpression;
        return member ?? unary?.Operand as MemberExpression;
    }

    /// <summary>
    /// Assigneds the values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expr">The expr.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public static Dictionary<string, object> AssignedValues<T>(this Expression<Func<T>> expr)
    {
        if (expr?.Body is not MemberInitExpression initExpr)
        {
            return null;
        }

        var to = new Dictionary<string, object>();
        foreach (var binding in initExpr.Bindings)
        {
            to[binding.Member.Name] = binding.GetValue();
        }
        return to;
    }

    /// <summary>
    /// Gets the field names.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expr">The expr.</param>
    /// <returns>System.String[].</returns>
    /// <exception cref="System.ArgumentException">Invalid Fields List Expression: " + expr</exception>
    public static string[] GetFieldNames<T>(this Expression<Func<T, object>> expr)
    {
        if (expr.Body is MemberExpression member)
        {
            if (member.Member.DeclaringType.IsAssignableFrom(typeof(T)))
            {
                return [member.Member.Name];
            }

            var array = CachedExpressionCompiler.Evaluate(member);
            if (array is IEnumerable<string> strEnum)
            {
                return [.. strEnum];
            }
        }

        if (expr.Body is NewExpression newExpr)
        {
            return [.. newExpr.Arguments.OfType<MemberExpression>().Select(x => x.Member.Name)];
        }

        if (expr.Body is MemberInitExpression init)
        {
            return [.. init.Bindings.Select(x => x.Member.Name)];
        }

        if (expr.Body is NewArrayExpression newArray)
        {
            var constantExprs = newArray.Expressions.OfType<ConstantExpression>().ToList();
            if (newArray.Expressions.Count == constantExprs.Count)
            {
                return [.. constantExprs.Select(x => x.Value.ToString())];
            }

            var array = CachedExpressionCompiler.Evaluate(newArray);
            if (array is string[] strArray)
            {
                return strArray;
            }

            return array.ConvertTo<string[]>();
        }

        if (expr.Body is UnaryExpression unary)
        {
            member = unary.Operand as MemberExpression;
            if (member != null)
            {
                return [member.Member.Name];
            }
        }

        throw new ArgumentException("Invalid Fields List Expression: " + expr);
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <returns>System.Object.</returns>
    public static object GetValue(this MemberBinding binding)
    {
        switch (binding.BindingType)
        {
            case MemberBindingType.Assignment:
                var assign = (MemberAssignment)binding;
                if (assign.Expression is ConstantExpression constant)
                {
                    return constant.Value;
                }

                try
                {
                    return CachedExpressionCompiler.Evaluate(assign.Expression);
                }
                catch (Exception ex)
                {
                    Log.Error("Error compiling expression in MemberBinding.GetValue()", ex);

                    //Fallback to compile and execute
                    var member = Expression.Convert(assign.Expression, typeof(object));
                    var lambda = Expression.Lambda<Func<object>>(member);
                    var getter = lambda.Compile();
                    return getter();
                }
        }
        return null;
    }
}