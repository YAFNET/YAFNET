// ***********************************************************************
// <copyright file="JsExpression.cs" company="ServiceStack, Inc.">
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

/// <summary>
/// Class JsExpression.
/// Implements the <see cref="ServiceStack.Script.JsToken" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsToken" />
public abstract class JsExpression : JsToken
{
    /// <summary>
    /// Converts to jsast.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public abstract Dictionary<string, object> ToJsAst();

    /// <summary>
    /// Converts to jsasttype.
    /// </summary>
    /// <returns>System.String.</returns>
    public virtual string ToJsAstType() => GetType().ToJsAstType();
}

/// <summary>
/// Class JsIdentifier.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsIdentifier : JsExpression
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsIdentifier"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public JsIdentifier(string name) => Name = name;
    /// <summary>
    /// Initializes a new instance of the <see cref="JsIdentifier"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public JsIdentifier(ReadOnlySpan<char> name) => Name = name.Value();
    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString() => ":" + Name;

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(ScriptScopeContext scope)
    {
        var ret = scope.PageResult.GetValue(Name, scope);
        return ret;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsIdentifier other) => string.Equals(Name, other.Name);

    /// <summary>
    /// Converts to jsast.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public override Dictionary<string, object> ToJsAst() => new()
                                                                {
                                                                    ["type"] = ToJsAstType(),
                                                                    ["name"] = Name,
                                                                };

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => Name.GetHashCode();

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
        return Equals((JsIdentifier)obj);
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => ToRawString();
}

/// <summary>
/// Class JsLiteral.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsLiteral : JsExpression
{
    /// <summary>
    /// The true
    /// </summary>
    public static JsLiteral True = new(true);
    /// <summary>
    /// The false
    /// </summary>
    public static JsLiteral False = new(false);

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public object Value { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="JsLiteral"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsLiteral(object value) => Value = value;
    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString() => JsonValue(Value);

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsLiteral other) => Equals(Value, other.Value);
    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((JsLiteral)obj);
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => ToRawString();

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(ScriptScopeContext scope) => Value;

    /// <summary>
    /// Converts to jsast.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public override Dictionary<string, object> ToJsAst() => new()
                                                                {
                                                                    ["type"] = ToJsAstType(),
                                                                    ["value"] = Value,
                                                                    ["raw"] = JsonValue(Value),
                                                                };
}

/// <summary>
/// Class JsArrayExpression.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsArrayExpression : JsExpression
{
    /// <summary>
    /// Gets the elements.
    /// </summary>
    /// <value>The elements.</value>
    public JsToken[] Elements { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsArrayExpression"/> class.
    /// </summary>
    /// <param name="elements">The elements.</param>
    public JsArrayExpression(params JsToken[] elements) => Elements = elements.ToArray();
    /// <summary>
    /// Initializes a new instance of the <see cref="JsArrayExpression"/> class.
    /// </summary>
    /// <param name="elements">The elements.</param>
    public JsArrayExpression(IEnumerable<JsToken> elements) : this(elements.ToArray()) { }

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(ScriptScopeContext scope)
    {
        var to = new List<object>();
        foreach (var element in Elements)
        {
            if (element is JsSpreadElement spread)
            {
                if (spread.Argument.Evaluate(scope) is not IEnumerable arr)
                    continue;

                foreach (var value in arr)
                {
                    to.Add(value);
                }
            }
            else
            {
                var value = element.Evaluate(scope);
                to.Add(value);
            }
        }
        return to;
    }

    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString()
    {
        var sb = StringBuilderCache.Allocate();
        sb.Append("[");
        for (var i = 0; i < Elements.Length; i++)
        {
            if (i > 0)
                sb.Append(",");

            var element = Elements[i];
            sb.Append(element.ToRawString());
        }
        sb.Append("]");
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Converts to jsast.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public override Dictionary<string, object> ToJsAst()
    {
        var elements = new List<object>();
        var to = new Dictionary<string, object>
                     {
                         ["type"] = ToJsAstType(),
                         ["elements"] = elements
                     };

        foreach (var element in Elements)
        {
            elements.Add(element.ToJsAst());
        }

        return to;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsArrayExpression other)
    {
        return Elements.EquivalentTo(other.Elements);
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
        return Equals((JsArrayExpression)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return Elements != null ? Elements.GetHashCode() : 0;
    }
}

/// <summary>
/// Class JsObjectExpression.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsObjectExpression : JsExpression
{
    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <value>The properties.</value>
    public JsProperty[] Properties { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsObjectExpression"/> class.
    /// </summary>
    /// <param name="properties">The properties.</param>
    public JsObjectExpression(params JsProperty[] properties) => Properties = properties;
    /// <summary>
    /// Initializes a new instance of the <see cref="JsObjectExpression"/> class.
    /// </summary>
    /// <param name="properties">The properties.</param>
    public JsObjectExpression(IEnumerable<JsProperty> properties) : this(properties.ToArray()) { }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Invalid Key. Expected a Literal or Identifier but was {token.DebugToken()}</exception>
    public static string GetKey(JsToken token)
    {
        if (token is JsLiteral literalKey)
            return literalKey.Value.ToString();
        if (token is JsIdentifier identifierKey)
            return identifierKey.Name;
        if (token is JsMemberExpression memberExpr && memberExpr.Property is JsIdentifier prop)
            return prop.Name;

        throw new SyntaxErrorException($"Invalid Key. Expected a Literal or Identifier but was {token.DebugToken()}");
    }

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.NotSupportedException">Object Expressions does not have a key</exception>
    public override object Evaluate(ScriptScopeContext scope)
    {
        var to = new Dictionary<string, object>();
        foreach (var prop in Properties)
        {
            if (prop.Key == null)
            {
                if (prop.Value is JsSpreadElement spread)
                {
                    var value = spread.Argument.Evaluate(scope);
                    var obj = value.ToObjectDictionary();
                    if (obj != null)
                    {
                        foreach (var entry in obj)
                        {
                            to[entry.Key] = entry.Value;
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException("Object Expressions does not have a key");
                }
            }
            else
            {
                var keyString = GetKey(prop.Key);
                var value = prop.Value.Evaluate(scope);
                to[keyString] = value;
            }
        }
        return to;
    }

    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString()
    {
        var sb = StringBuilderCache.Allocate();
        sb.Append("{");
        for (var i = 0; i < Properties.Length; i++)
        {
            if (i > 0)
                sb.Append(",");

            var prop = Properties[i];
            if (prop.Key != null)
            {
                sb.Append(prop.Key.ToRawString());
                if (!prop.Shorthand)
                {
                    sb.Append(":");
                    sb.Append(prop.Value.ToRawString());
                }
            }
            else //.... spread operator
            {
                sb.Append(prop.Value.ToRawString());
            }
        }
        sb.Append("}");
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Converts to jsast.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public override Dictionary<string, object> ToJsAst()
    {
        var properties = new List<object>();
        var to = new Dictionary<string, object>
                     {
                         ["type"] = ToJsAstType(),
                         ["properties"] = properties
                     };

        var propType = typeof(JsProperty).ToJsAstType();
        foreach (var prop in Properties)
        {
            properties.Add(new Dictionary<string, object>
                               {
                                   ["type"] = propType,
                                   ["key"] = prop.Key?.ToJsAst(),
                                   ["computed"] = false, //syntax not supported: { ["a" + 1]: 2 }
                                   ["value"] = prop.Value.ToJsAst(),
                                   ["kind"] = "init",
                                   ["method"] = false,
                                   ["shorthand"] = prop.Shorthand,
                               });
        }

        return to;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsObjectExpression other)
    {
        return Properties.EquivalentTo(other.Properties);
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
        return Equals((JsObjectExpression)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return Properties != null ? Properties.GetHashCode() : 0;
    }
}

/// <summary>
/// Class JsProperty.
/// </summary>
public class JsProperty
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The key.</value>
    public JsToken Key { get; }
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public JsToken Value { get; }
    /// <summary>
    /// Gets a value indicating whether this <see cref="JsProperty"/> is shorthand.
    /// </summary>
    /// <value><c>true</c> if shorthand; otherwise, <c>false</c>.</value>
    public bool Shorthand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsProperty"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public JsProperty(JsToken key, JsToken value) : this(key, value, shorthand: false) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="JsProperty"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="shorthand">if set to <c>true</c> [shorthand].</param>
    public JsProperty(JsToken key, JsToken value, bool shorthand)
    {
        Key = key;
        Value = value;
        Shorthand = shorthand;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsProperty other)
    {
        return Equals(Key, other.Key) &&
               Equals(Value, other.Value) &&
               Shorthand == other.Shorthand;
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
        return Equals((JsProperty)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Key != null ? Key.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Shorthand.GetHashCode();
            return hashCode;
        }
    }
}

/// <summary>
/// Class JsSpreadElement.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsSpreadElement : JsExpression
{
    /// <summary>
    /// Gets the argument.
    /// </summary>
    /// <value>The argument.</value>
    public JsToken Argument { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="JsSpreadElement"/> class.
    /// </summary>
    /// <param name="argument">The argument.</param>
    public JsSpreadElement(JsToken argument)
    {
        Argument = argument;
    }

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(ScriptScopeContext scope)
    {
        return Argument.Evaluate(scope);
    }

    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString()
    {
        return "..." + Argument.ToRawString();
    }

    /// <summary>
    /// Converts to jsast.
    /// </summary>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public override Dictionary<string, object> ToJsAst() => new()
                                                                {
                                                                    ["type"] = ToJsAstType(),
                                                                    ["argument"] = Argument.ToJsAst()
                                                                };

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsSpreadElement other)
    {
        return Equals(Argument, other.Argument);
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
        return Equals((JsSpreadElement)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return Argument != null ? Argument.GetHashCode() : 0;
    }
}

/// <summary>
/// Class JsArrowFunctionExpression.
/// Implements the <see cref="ServiceStack.Script.JsExpression" />
/// </summary>
/// <seealso cref="ServiceStack.Script.JsExpression" />
public class JsArrowFunctionExpression : JsExpression
{
    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <value>The parameters.</value>
    public JsIdentifier[] Params { get; }
    /// <summary>
    /// Gets the body.
    /// </summary>
    /// <value>The body.</value>
    public JsToken Body { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsArrowFunctionExpression"/> class.
    /// </summary>
    /// <param name="param">The parameter.</param>
    /// <param name="body">The body.</param>
    public JsArrowFunctionExpression(JsIdentifier param, JsToken body) : this(new[] { param }, body) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="JsArrowFunctionExpression"/> class.
    /// </summary>
    /// <param name="params">The parameters.</param>
    /// <param name="body">The body.</param>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Params missing in Arrow Function Expression</exception>
    /// <exception cref="ServiceStack.Script.SyntaxErrorException">Body missing in Arrow Function Expression</exception>
    public JsArrowFunctionExpression(JsIdentifier[] @params, JsToken body)
    {
        Params = @params ?? throw new SyntaxErrorException($"Params missing in Arrow Function Expression");
        Body = body ?? throw new SyntaxErrorException($"Body missing in Arrow Function Expression");
    }

    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToRawString()
    {
        var sb = StringBuilderCache.Allocate();

        sb.Append("(");
        for (var i = 0; i < Params.Length; i++)
        {
            var identifier = Params[i];
            if (i > 0)
                sb.Append(",");

            sb.Append(identifier.Name);
        }
        sb.Append(") => ");

        sb.Append(Body.ToRawString());
        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public override object Evaluate(ScriptScopeContext scope) => this;

    /// <summary>
    /// Invokes the specified parameters.
    /// </summary>
    /// <param name="params">The parameters.</param>
    /// <returns>System.Object.</returns>
    public object Invoke(params object[] @params) => Invoke(JS.CreateScope(), @params);
    /// <summary>
    /// Invokes the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="params">The parameters.</param>
    /// <returns>System.Object.</returns>
    public object Invoke(ScriptScopeContext scope, params object[] @params)
    {
        var args = new Dictionary<string, object>();
        for (var i = 0; i < Params.Length; i++)
        {
            if (@params.Length < i)
                break;

            var param = Params[i];
            args[param.Name] = @params[i];
        }

        var exprScope = scope.ScopeWithParams(args);
        var ret = Body.Evaluate(exprScope);
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
                         ["id"] = null,
                     };
        var args = new List<object>();
        to["params"] = args;

        foreach (var param in Params)
        {
            args.Add(param.ToJsAst());
        }

        to["body"] = Body.ToJsAst();
        return to;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(JsArrowFunctionExpression other)
    {
        return Params.EquivalentTo(other.Params) && Equals(Body, other.Body);
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
        return Equals((JsArrowFunctionExpression)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((Params != null ? Params.GetHashCode() : 0) * 397) ^ (Body != null ? Body.GetHashCode() : 0);
        }
    }
}