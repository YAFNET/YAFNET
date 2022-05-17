// ***********************************************************************
// <copyright file="JsStatement.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace ServiceStack.Script;

/// <summary>
/// Class JsStatement.
/// </summary>
public abstract class JsStatement
{
}

/// <summary>
/// Class JsFilterExpressionStatement.
/// Implements the <see cref="ServiceStack.Script.JsStatement" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsStatement" />
public class JsFilterExpressionStatement : JsStatement
{
    /// <summary>
    /// Gets the filter expression.
    /// </summary>
    /// <value>The filter expression.</value>
    public PageVariableFragment FilterExpression { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="JsFilterExpressionStatement"/> class.
    /// </summary>
    /// <param name="originalText">The original text.</param>
    /// <param name="expr">The expr.</param>
    /// <param name="filters">The filters.</param>
    public JsFilterExpressionStatement(ReadOnlyMemory<char> originalText, JsToken expr, List<JsCallExpression> filters)
    {
        FilterExpression = new PageVariableFragment(originalText, expr, filters);
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="JsFilterExpressionStatement"/> class.
    /// </summary>
    /// <param name="originalText">The original text.</param>
    /// <param name="expr">The expr.</param>
    /// <param name="filters">The filters.</param>
    public JsFilterExpressionStatement(string originalText, JsToken expr, params JsCallExpression[] filters)
    {
        FilterExpression = new PageVariableFragment(originalText.AsMemory(), expr, new List<JsCallExpression>(filters));
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsFilterExpressionStatement other) => Equals(FilterExpression, other.FilterExpression);

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((JsFilterExpressionStatement)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => FilterExpression != null ? FilterExpression.GetHashCode() : 0;
}

/// <summary>
/// Class JsBlockStatement.
/// Implements the <see cref="ServiceStack.Script.JsStatement" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsStatement" />
public class JsBlockStatement : JsStatement
{
    /// <summary>
    /// Gets the statements.
    /// </summary>
    /// <value>The statements.</value>
    public JsStatement[] Statements { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="JsBlockStatement"/> class.
    /// </summary>
    /// <param name="statements">The statements.</param>
    public JsBlockStatement(JsStatement[] statements)
    {
        Statements = statements;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsBlockStatement"/> class.
    /// </summary>
    /// <param name="statement">The statement.</param>
    public JsBlockStatement(JsStatement statement)
    {
        Statements = new[] {statement};
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsBlockStatement other) => Statements.EquivalentTo(other.Statements);

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((JsBlockStatement)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => Statements != null ? Statements.GetHashCode() : 0;
}

/// <summary>
/// Class JsExpressionStatement.
/// Implements the <see cref="ServiceStack.Script.JsStatement" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsStatement" />
public class JsExpressionStatement : JsStatement
{
    /// <summary>
    /// Gets the expression.
    /// </summary>
    /// <value>The expression.</value>
    public JsToken Expression { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="JsExpressionStatement"/> class.
    /// </summary>
    /// <param name="expression">The expression.</param>
    public JsExpressionStatement(JsToken expression)
    {
        Expression = expression;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsExpressionStatement other) => Equals(Expression, other.Expression);

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((JsExpressionStatement)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => Expression != null ? Expression.GetHashCode() : 0;
}

/// <summary>
/// Class JsPageBlockFragmentStatement.
/// Implements the <see cref="ServiceStack.Script.JsStatement" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsStatement" />
public class JsPageBlockFragmentStatement : JsStatement
{
    /// <summary>
    /// Gets the block.
    /// </summary>
    /// <value>The block.</value>
    public PageBlockFragment Block { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="JsPageBlockFragmentStatement"/> class.
    /// </summary>
    /// <param name="block">The block.</param>
    public JsPageBlockFragmentStatement(PageBlockFragment block)
    {
        Block = block;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsPageBlockFragmentStatement other) => Equals(Block, other.Block);

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((JsPageBlockFragmentStatement)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => Block != null ? Block.GetHashCode() : 0;
}