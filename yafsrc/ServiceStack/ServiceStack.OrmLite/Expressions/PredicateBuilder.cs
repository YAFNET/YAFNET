// ***********************************************************************
// <copyright file="PredicateBuilder.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite;

using System;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Enables the efficient, dynamic composition of query predicates.
/// </summary>
public static class PredicateBuilder
{
    /// <summary>
    /// Creates a predicate that evaluates to true.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
    public static Expression<Func<T, bool>> True<T>()
    {
        return param => true;
    }

    /// <summary>
    /// Creates a predicate that evaluates to false.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
    public static Expression<Func<T, bool>> False<T>()
    {
        return param => false;
    }

    /// <summary>
    /// Creates a predicate expression from the specified lambda expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
    public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
    {
        return predicate;
    }

    /// <param name="first">The first.</param>
    /// <typeparam name="T"></typeparam>
    extension<T>(Expression<Func<T, bool>> first)
    {
        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        /// <param name="second">The second.</param>
        /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
        public Expression<Func<T, bool>> And(Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        /// <param name="second">The second.</param>
        /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
        public Expression<Func<T, bool>> Or(Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// Negates the predicate.
        /// </summary>
        /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
        public Expression<Func<T, bool>> Not()
        {
            var negated = Expression.Not(first.Body);
            return Expression.Lambda<Func<T, bool>>(negated, first.Parameters);
        }
    }

    /// <summary>
    /// Combines the first expression with the second using the specified merge function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="first">The first.</param>
    /// <param name="second">The second.</param>
    /// <param name="merge">The merge.</param>
    /// <returns>Expression&lt;T&gt;.</returns>
    private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
                                            Func<Expression, Expression, Expression> merge)
    {
        // zip parameters (map from parameters of second to parameters of first)
        var map = first.Parameters
            .Select((f, i) => new { f, s = second.Parameters[i] })
            .ToDictionary(p => p.s, p => p.f);

        // replace parameters in the second lambda expression with the parameters in the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        // create a merged lambda expression with parameters from the first expression
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }
}