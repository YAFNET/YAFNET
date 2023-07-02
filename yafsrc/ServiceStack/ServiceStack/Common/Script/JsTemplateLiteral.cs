// ***********************************************************************
// <copyright file="JsTemplateLiteral.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;
using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Class JsTemplateLiteral.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsTemplateLiteral : JsExpression
{
    /// <summary>
    /// Gets the quasis.
    /// </summary>
    /// <value>The quasis.</value>
    public JsTemplateElement[] Quasis { get; }
    /// <summary>
    /// Gets the expressions.
    /// </summary>
    /// <value>The expressions.</value>
    public JsToken[] Expressions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsTemplateLiteral" /> class.
    /// </summary>
    /// <param name="cooked">The cooked.</param>
    public JsTemplateLiteral(string cooked)
        : this(new[] { new JsTemplateElement(cooked, cooked, tail: true) }) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsTemplateLiteral" /> class.
    /// </summary>
    /// <param name="quasis">The quasis.</param>
    /// <param name="expressions">The expressions.</param>
    public JsTemplateLiteral(JsTemplateElement[] quasis = null, JsToken[] expressions = null)
    {
        Quasis = quasis ?? TypeConstants<JsTemplateElement>.EmptyArray;
        Expressions = expressions ?? TypeConstants<JsToken>.EmptyArray;
    }

    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString()
    {
        var sb = StringBuilderCache.Allocate();

        sb.Append("`");

        for (int i = 0; i < Quasis.Length; i++)
        {
            var quasi = Quasis[i];
            sb.Append(quasi.Value.Raw);
            if (quasi.Tail)
                break;

            var expr = Expressions[i];
            sb.Append("${");
            sb.Append(expr.ToRawString());
            sb.Append("}");
        }

        sb.Append("`");

        var ret = StringBuilderCache.ReturnAndFree(sb);
        return ret;
    }

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(ScriptScopeContext scope)
    {
        var sb = StringBuilderCache.Allocate();

        for (int i = 0; i < Quasis.Length; i++)
        {
            var quasi = Quasis[i];
            sb.Append(quasi.Value.Cooked);
            if (quasi.Tail)
                break;

            var expr = Expressions[i];
            var value = expr.Evaluate(scope);
            if (value is IRawString rs)
                sb.Append(rs.ToRawString());
            else
                sb.Append(value);
        }

        var ret = StringBuilderCache.ReturnAndFree(sb);
        return ret;
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
                     };

        var quasiType = typeof(JsTemplateElement).ToJsAstType();
        var quasis = new List<object>();
        foreach (var quasi in Quasis)
        {
            quasis.Add(new Dictionary<string, object>
                           {
                               ["type"] = quasiType,
                               ["value"] = new Dictionary<string, object>
                                               {
                                                   ["raw"] = quasi.Value.Raw,
                                                   ["cooked"] = quasi.Value.Cooked,
                                               },
                               ["tail"] = quasi.Tail,
                           });
        }
        to["quasis"] = quasis;

        var expressions = new List<object>();
        foreach (var expression in Expressions)
        {
            expressions.Add(expression.ToJsAst());
        }
        to["expressions"] = expressions;

        return to;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsTemplateLiteral other)
    {
        return Quasis.EquivalentTo(other.Quasis) &&
               Expressions.EquivalentTo(other.Expressions);
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
        return Equals((JsTemplateLiteral)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((Quasis != null ? Quasis.GetHashCode() : 0) * 397) ^ (Expressions != null ? Expressions.GetHashCode() : 0);
        }
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => ToRawString();
}

/// <summary>
/// Class JsTemplateElement.
/// </summary>
public class JsTemplateElement
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public JsTemplateElementValue Value { get; }
    /// <summary>
    /// Gets a value indicating whether this <see cref="JsTemplateElement" /> is tail.
    /// </summary>
    /// <value><c>true</c> if tail; otherwise, <c>false</c>.</value>
    public bool Tail { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsTemplateElement" /> class.
    /// </summary>
    /// <param name="raw">The raw.</param>
    /// <param name="cooked">The cooked.</param>
    /// <param name="tail">if set to <c>true</c> [tail].</param>
    public JsTemplateElement(string raw, string cooked, bool tail = false) :
        this(new JsTemplateElementValue(raw, cooked), tail)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsTemplateElement" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="tail">if set to <c>true</c> [tail].</param>
    public JsTemplateElement(JsTemplateElementValue value, bool tail)
    {
        Value = value;
        Tail = tail;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsTemplateElement other)
    {
        return Equals(Value, other.Value) && Tail == other.Tail;
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
        return Equals((JsTemplateElement)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((Value != null ? Value.GetHashCode() : 0) * 397) ^ Tail.GetHashCode();
        }
    }
}

/// <summary>
/// Class JsTemplateElementValue.
/// </summary>
public class JsTemplateElementValue
{
    /// <summary>
    /// Gets the raw.
    /// </summary>
    /// <value>The raw.</value>
    public string Raw { get; }
    /// <summary>
    /// Gets the cooked.
    /// </summary>
    /// <value>The cooked.</value>
    public string Cooked { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsTemplateElementValue" /> class.
    /// </summary>
    /// <param name="raw">The raw.</param>
    /// <param name="cooked">The cooked.</param>
    public JsTemplateElementValue(string raw, string cooked)
    {
        Raw = raw;
        Cooked = cooked;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsTemplateElementValue other)
    {
        return Raw.Equals(other.Raw) && Cooked.Equals(other.Cooked);
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
        return Equals((JsTemplateElementValue)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return (Raw.GetHashCode() * 397) ^ Cooked.GetHashCode();
        }
    }
}