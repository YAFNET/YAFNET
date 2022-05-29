// ***********************************************************************
// <copyright file="JsBinaryExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;

namespace ServiceStack.Script;

/// <summary>
/// Class JsBinaryExpression.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsBinaryExpression : JsExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsBinaryExpression"/> class.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="operator">The operator.</param>
    /// <param name="right">The right.</param>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Left Expression missing in Binary Expression</exception>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Operator missing in Binary Expression</exception>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Right Expression missing in Binary Expression</exception>
    public JsBinaryExpression(JsToken left, JsBinaryOperator @operator, JsToken right)
    {
        Left = left ?? throw new SyntaxErrorException("Left Expression missing in Binary Expression");
        Operator = @operator ?? throw new SyntaxErrorException("Operator missing in Binary Expression");
        Right = right ?? throw new SyntaxErrorException("Right Expression missing in Binary Expression");
    }

    /// <summary>
    /// Gets or sets the operator.
    /// </summary>
    /// <value>The operator.</value>
    public JsBinaryOperator Operator { get; set; }
    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>The left.</value>
    public JsToken Left { get; set; }
    /// <summary>
    /// Gets or sets the right.
    /// </summary>
    /// <value>The right.</value>
    public JsToken Right { get; set; }
    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString() => "(" + JsonValue(Left) + Operator.Token + JsonValue(Right) + ")";

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsBinaryExpression other) =>
        Equals(Operator, other.Operator) && Equals(Left, other.Left) && Equals(Right, other.Right);

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
        return Equals((JsBinaryExpression)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Operator != null ? Operator.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (Left != null ? Left.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Right != null ? Right.GetHashCode() : 0);
            return hashCode;
        }
    }

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(ScriptScopeContext scope)
    {
        var lhs = Left.Evaluate(scope);
        var rhs = Right.Evaluate(scope);
        return Operator.Evaluate(lhs, rhs);
    }

    /// <summary>
    /// Converts to jsast.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public override Dictionary<string, object> ToJsAst()
    {
        var to = new Dictionary<string, object>
                     {
                         ["type"] = ToJsAstType(),
                         ["operator"] = Operator.Token,
                         ["left"] = Left.ToJsAst(),
                         ["right"] = Right.ToJsAst(),
                     };
        return to;
    }
}