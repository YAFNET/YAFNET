// ***********************************************************************
// <copyright file="JsUnaryExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;

namespace ServiceStack.Script;

/// <summary>
/// Class JsUnaryExpression.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsUnaryExpression : JsExpression
{
    /// <summary>
    /// Gets the operator.
    /// </summary>
    /// <value>The operator.</value>
    public JsUnaryOperator Operator { get; }
    /// <summary>
    /// Gets the argument.
    /// </summary>
    /// <value>The argument.</value>
    public JsToken Argument { get; }
    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString() => Operator.Token + JsonValue(Argument);

    /// <summary>
    /// Initializes a new instance of the <see cref="JsUnaryExpression"/> class.
    /// </summary>
    /// <param name="operator">The operator.</param>
    /// <param name="argument">The argument.</param>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Operator missing in Unary Expression</exception>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Argument missing in Unary Expression</exception>
    public JsUnaryExpression(JsUnaryOperator @operator, JsToken argument)
    {
        Operator = @operator ?? throw new SyntaxErrorException($"Operator missing in Unary Expression");
        Argument = argument ?? throw new SyntaxErrorException($"Argument missing in Unary Expression");
    }

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(ScriptScopeContext scope)
    {
        var result = Argument.Evaluate(scope);
        var afterUnary = Operator.Evaluate(result);
        return afterUnary;
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
                         ["argument"] = Argument.ToJsAst(),
                     };
        return to;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsUnaryExpression other) => Equals(Operator, other.Operator) && Equals(Argument, other.Argument);

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
        return Equals((JsUnaryExpression)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((Operator != null ? Operator.GetHashCode() : 0) * 397) ^ (Argument != null ? Argument.GetHashCode() : 0);
        }
    }
}