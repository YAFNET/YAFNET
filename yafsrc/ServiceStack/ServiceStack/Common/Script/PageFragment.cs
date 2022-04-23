// ***********************************************************************
// <copyright file="PageFragment.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Class PageFragment.
/// </summary>
public abstract class PageFragment { }

/// <summary>
/// Class PageVariableFragment.
/// Implements the <see cref="ServiceStack.Script.PageFragment" />
/// </summary>
/// <seealso cref="ServiceStack.Script.PageFragment" />
public class PageVariableFragment : PageFragment
{
    /// <summary>
    /// Gets or sets the original text.
    /// </summary>
    /// <value>The original text.</value>
    public ReadOnlyMemory<char> OriginalText { get; set; }

    /// <summary>
    /// The original text UTF8
    /// </summary>
    private ReadOnlyMemory<byte> originalTextUtf8;
    /// <summary>
    /// Gets the original text UTF8.
    /// </summary>
    /// <value>The original text UTF8.</value>
    public ReadOnlyMemory<byte> OriginalTextUtf8 => originalTextUtf8.IsEmpty ? originalTextUtf8 = OriginalText.ToUtf8() : OriginalTextUtf8;

    /// <summary>
    /// Gets the expression.
    /// </summary>
    /// <value>The expression.</value>
    public JsToken Expression { get; }

    /// <summary>
    /// Gets the binding.
    /// </summary>
    /// <value>The binding.</value>
    public string Binding { get; }

    /// <summary>
    /// Gets the initial value.
    /// </summary>
    /// <value>The initial value.</value>
    public object InitialValue { get; }
    /// <summary>
    /// Gets the initial expression.
    /// </summary>
    /// <value>The initial expression.</value>
    public JsCallExpression InitialExpression { get; }

    /// <summary>
    /// Gets the filter expressions.
    /// </summary>
    /// <value>The filter expressions.</value>
    public JsCallExpression[] FilterExpressions { get; }

    /// <summary>
    /// Gets the last name of the filter.
    /// </summary>
    /// <value>The last name of the filter.</value>
    internal string LastFilterName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageVariableFragment"/> class.
    /// </summary>
    /// <param name="originalText">The original text.</param>
    /// <param name="expr">The expr.</param>
    /// <param name="filterCommands">The filter commands.</param>
    public PageVariableFragment(ReadOnlyMemory<char> originalText, JsToken expr, List<JsCallExpression> filterCommands)
    {
        OriginalText = originalText;
        Expression = expr;
        FilterExpressions = filterCommands?.ToArray() ?? TypeConstants<JsCallExpression>.EmptyArray;

        if (expr is JsLiteral initialValue)
        {
            InitialValue = initialValue.Value;
        }
        else if (ReferenceEquals(expr, JsNull.Value))
        {
            InitialValue = expr;
        }
        else if (expr is JsCallExpression initialExpr)
        {
            InitialExpression = initialExpr;
        }
        else if (expr is JsIdentifier initialBinding)
        {
            Binding = initialBinding.Name;
        }

        if (FilterExpressions.Length > 0)
        {
            LastFilterName = FilterExpressions[FilterExpressions.Length - 1].Name;
        }
    }

    /// <summary>
    /// Evaluates the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object Evaluate(ScriptScopeContext scope)
    {
        if (ReferenceEquals(Expression, JsNull.Value))
            return Expression;

        return Expression.Evaluate(scope);
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(PageVariableFragment other)
    {
        return OriginalText.Span.SequenceEqual(other.OriginalText.Span)
               && Equals(Expression, other.Expression)
               && FilterExpressions.EquivalentTo(other.FilterExpressions);
    }

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
        return Equals((PageVariableFragment)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = OriginalText.GetHashCode();
            hashCode = (hashCode * 397) ^ (Expression != null ? Expression.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (FilterExpressions != null ? FilterExpressions.GetHashCode() : 0);
            return hashCode;
        }
    }
}

/// <summary>
/// Class PageStringFragment.
/// Implements the <see cref="ServiceStack.Script.PageFragment" />
/// </summary>
/// <seealso cref="ServiceStack.Script.PageFragment" />
public class PageStringFragment : PageFragment
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public ReadOnlyMemory<char> Value { get; set; }

    /// <summary>
    /// The value string
    /// </summary>
    private string valueString;
    /// <summary>
    /// Gets the value string.
    /// </summary>
    /// <value>The value string.</value>
    public string ValueString => valueString ?? (valueString = Value.ToString());

    /// <summary>
    /// The value UTF8
    /// </summary>
    private ReadOnlyMemory<byte> valueUtf8;
    /// <summary>
    /// Gets the value UTF8.
    /// </summary>
    /// <value>The value UTF8.</value>
    public ReadOnlyMemory<byte> ValueUtf8 => valueUtf8.IsEmpty ? valueUtf8 = Value.ToUtf8() : valueUtf8;

    /// <summary>
    /// Initializes a new instance of the <see cref="PageStringFragment"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public PageStringFragment(string value) : this(value.AsMemory()) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PageStringFragment"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public PageStringFragment(ReadOnlyMemory<char> value) => Value = value;

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(PageStringFragment other)
    {
        return Value.Span.SequenceEqual(other.Value.Span);
    }

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
        return Equals((PageStringFragment)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

/// <summary>
/// Class PageBlockFragment.
/// Implements the <see cref="ServiceStack.Script.PageFragment" />
/// </summary>
/// <seealso cref="ServiceStack.Script.PageFragment" />
public class PageBlockFragment : PageFragment
{
    /// <summary>
    /// Gets the original text.
    /// </summary>
    /// <value>The original text.</value>
    public ReadOnlyMemory<char> OriginalText { get; internal set; }
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>
    /// Gets the argument.
    /// </summary>
    /// <value>The argument.</value>
    public ReadOnlyMemory<char> Argument { get; }
    /// <summary>
    /// The argument string
    /// </summary>
    private string argumentString;
    /// <summary>
    /// Gets the argument string.
    /// </summary>
    /// <value>The argument string.</value>
    public string ArgumentString => argumentString ?? (argumentString = Argument.ToString());

    /// <summary>
    /// Gets the body.
    /// </summary>
    /// <value>The body.</value>
    public PageFragment[] Body { get; }
    /// <summary>
    /// Gets the else blocks.
    /// </summary>
    /// <value>The else blocks.</value>
    public PageElseBlock[] ElseBlocks { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageBlockFragment"/> class.
    /// </summary>
    /// <param name="originalText">The original text.</param>
    /// <param name="name">The name.</param>
    /// <param name="argument">The argument.</param>
    /// <param name="body">The body.</param>
    /// <param name="elseStatements">The else statements.</param>
    public PageBlockFragment(string originalText, string name, string argument,
                             JsStatement body, IEnumerable<PageElseBlock> elseStatements = null)
        : this(originalText.AsMemory(), name, argument.AsMemory(), new PageFragment[] { new PageJsBlockStatementFragment(new JsBlockStatement(body)) }, elseStatements) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PageBlockFragment"/> class.
    /// </summary>
    /// <param name="originalText">The original text.</param>
    /// <param name="name">The name.</param>
    /// <param name="argument">The argument.</param>
    /// <param name="body">The body.</param>
    /// <param name="elseStatements">The else statements.</param>
    public PageBlockFragment(string originalText, string name, string argument,
                             JsBlockStatement body, IEnumerable<PageElseBlock> elseStatements = null)
        : this(originalText.AsMemory(), name, argument.AsMemory(), new PageFragment[] { new PageJsBlockStatementFragment(body) }, elseStatements) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageBlockFragment"/> class.
    /// </summary>
    /// <param name="originalText">The original text.</param>
    /// <param name="name">The name.</param>
    /// <param name="argument">The argument.</param>
    /// <param name="body">The body.</param>
    /// <param name="elseStatements">The else statements.</param>
    public PageBlockFragment(string originalText, string name, string argument,
                             List<PageFragment> body, List<PageElseBlock> elseStatements = null)
        : this(originalText.AsMemory(), name, argument.AsMemory(), body, elseStatements) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PageBlockFragment"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="argument">The argument.</param>
    /// <param name="body">The body.</param>
    /// <param name="elseStatements">The else statements.</param>
    public PageBlockFragment(string name, ReadOnlyMemory<char> argument,
                             List<PageFragment> body, List<PageElseBlock> elseStatements = null)
        : this(default, name, argument, body, elseStatements) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageBlockFragment"/> class.
    /// </summary>
    /// <param name="originalText">The original text.</param>
    /// <param name="name">The name.</param>
    /// <param name="argument">The argument.</param>
    /// <param name="body">The body.</param>
    /// <param name="elseStatements">The else statements.</param>
    public PageBlockFragment(ReadOnlyMemory<char> originalText, string name, ReadOnlyMemory<char> argument,
                             IEnumerable<PageFragment> body, IEnumerable<PageElseBlock> elseStatements = null)
    {
        OriginalText = originalText;
        Name = name;
        Argument = argument;
        Body = body.ToArray();
        ElseBlocks = elseStatements?.ToArray() ?? TypeConstants<PageElseBlock>.EmptyArray;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(PageBlockFragment other)
    {
        return Name == other.Name &&
               Argument.Span.SequenceEqual(other.Argument.Span) &&
               Body.EquivalentTo(other.Body) &&
               ElseBlocks.EquivalentTo(other.ElseBlocks);
    }

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
        return Equals((PageBlockFragment)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name != null ? Name.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ Argument.GetHashCode();
            hashCode = (hashCode * 397) ^ (Body != null ? Body.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ElseBlocks != null ? ElseBlocks.GetHashCode() : 0);
            return hashCode;
        }
    }
}

/// <summary>
/// Class PageElseBlock.
/// Implements the <see cref="ServiceStack.Script.PageFragment" />
/// </summary>
/// <seealso cref="ServiceStack.Script.PageFragment" />
public class PageElseBlock : PageFragment
{
    /// <summary>
    /// Gets the argument.
    /// </summary>
    /// <value>The argument.</value>
    public ReadOnlyMemory<char> Argument { get; }
    /// <summary>
    /// Gets the body.
    /// </summary>
    /// <value>The body.</value>
    public PageFragment[] Body { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageElseBlock"/> class.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="body">The body.</param>
    public PageElseBlock(string argument, List<PageFragment> body) : this(argument.AsMemory(), body) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PageElseBlock"/> class.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="statement">The statement.</param>
    public PageElseBlock(string argument, JsStatement statement)
        : this(argument, new JsBlockStatement(statement)) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PageElseBlock"/> class.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="block">The block.</param>
    public PageElseBlock(string argument, JsBlockStatement block)
        : this(argument.AsMemory(), new PageFragment[] { new PageJsBlockStatementFragment(block) }) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PageElseBlock"/> class.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="body">The body.</param>
    public PageElseBlock(ReadOnlyMemory<char> argument, IEnumerable<PageFragment> body) : this(argument, body.ToArray()) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PageElseBlock"/> class.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="body">The body.</param>
    public PageElseBlock(ReadOnlyMemory<char> argument, PageFragment[] body)
    {
        Argument = argument;
        Body = body;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(PageElseBlock other)
    {
        return Argument.Span.SequenceEqual(other.Argument.Span) &&
               Body.EquivalentTo(other.Body);
    }

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
        return Equals((PageElseBlock)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Argument.GetHashCode();
            hashCode = (hashCode * 397) ^ (Body != null ? Body.GetHashCode() : 0);
            return hashCode;
        }
    }
}

/// <summary>
/// Class PageJsBlockStatementFragment.
/// Implements the <see cref="ServiceStack.Script.PageFragment" />
/// </summary>
/// <seealso cref="ServiceStack.Script.PageFragment" />
public class PageJsBlockStatementFragment : PageFragment
{
    /// <summary>
    /// Gets the block.
    /// </summary>
    /// <value>The block.</value>
    public JsBlockStatement Block { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="PageJsBlockStatementFragment"/> is quiet.
    /// </summary>
    /// <value><c>true</c> if quiet; otherwise, <c>false</c>.</value>
    public bool Quiet { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageJsBlockStatementFragment"/> class.
    /// </summary>
    /// <param name="statement">The statement.</param>
    public PageJsBlockStatementFragment(JsBlockStatement statement)
    {
        Block = statement;
    }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(PageJsBlockStatementFragment other)
    {
        return Equals(Block, other.Block);
    }

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
        return Equals((PageJsBlockStatementFragment)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return Block != null ? Block.GetHashCode() : 0;
    }
}