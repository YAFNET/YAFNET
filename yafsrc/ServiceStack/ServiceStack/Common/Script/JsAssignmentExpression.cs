// ***********************************************************************
// <copyright file="JsAssignmentExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace ServiceStack.Script;

/// <summary>
/// Class JsAssignmentExpression.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsAssignmentExpression : JsExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsAssignmentExpression"/> class.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="operator">The operator.</param>
    /// <param name="right">The right.</param>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Left Expression missing in Binary Expression</exception>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Operator missing in Binary Expression</exception>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Right Expression missing in Binary Expression</exception>
    public JsAssignmentExpression(JsToken left, JsAssignment @operator, JsToken right)
    {
        Left = left ?? throw new SyntaxErrorException("Left Expression missing in Binary Expression");
        Operator = @operator ?? throw new SyntaxErrorException("Operator missing in Binary Expression");
        Right = right ?? throw new SyntaxErrorException("Right Expression missing in Binary Expression");
    }

    /// <summary>
    /// Gets or sets the operator.
    /// </summary>
    /// <value>The operator.</value>
    public JsAssignment Operator { get; set; }
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
    protected bool Equals(JsAssignmentExpression other) =>
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
        return Equals((JsAssignmentExpression)obj);
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
    /// The assign function
    /// </summary>
    private Action<ScriptScopeContext, object, object> assignFn;

    /// <summary>
    /// Evals the property.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="scope">The scope.</param>
    /// <returns>System.String.</returns>
    private string EvalProperty(JsToken token, ScriptScopeContext scope)
    {
        switch (token)
        {
            case JsIdentifier id:
                return id.Name;
            default:
                return JsonValue(token.Evaluate(scope));
        }
    }

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    /// <exception cref="System.NotSupportedException">Could not create assignment expression for '{memberExpr.ToRawString()}'</exception>
    /// <exception cref="System.NotSupportedException">Assignment Expression not supported: " + Left.ToRawString()</exception>
    public override object Evaluate(ScriptScopeContext scope)
    {
        var rhs = Right.Evaluate(scope);

        if (Left is JsIdentifier id)
        {
            if (scope.ScopedParams.ContainsKey(id.Name))
            {
                scope.ScopedParams[id.Name] = rhs;
            }
            else
            {
                scope.PageResult.Args[id.Name] = rhs;
            }
            return rhs;
        }
        else if (Left is JsMemberExpression memberExpr)
        {
            var target = memberExpr.Object.Evaluate(scope);
            if (target == null)
                throw new ArgumentNullException(memberExpr.Object.ToRawString());

            // Evaluate target then reduce to simple expression then compile assign expression using expression trees
            var assignTargetExpr = memberExpr.Computed
                                       ? "obj[" + EvalProperty(memberExpr.Property, scope) + "]"
                                       : "obj." + EvalProperty(memberExpr.Property, scope);

            if (assignFn == null)
            {
                assignFn = scope.Context.GetAssignExpression(target.GetType(), assignTargetExpr.AsMemory());
                if (assignFn == null)
                    throw new NotSupportedException($"Could not create assignment expression for '{memberExpr.ToRawString()}'");
            }

            if (assignFn != null)
            {
                assignFn(scope, target, rhs);
                return rhs;
            }
        }

        throw new NotSupportedException("Assignment Expression not supported: " + Left.ToRawString());
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