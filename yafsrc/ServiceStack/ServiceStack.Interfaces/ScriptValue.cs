// ***********************************************************************
// <copyright file="ScriptValue.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack
{
    /// <summary>
    /// Define a rich value that can either be a value, a constant JS Expression or a #Script Code script
    /// </summary>
    public interface IScriptValue
    {
        /// <summary>
        /// Use constant Value
        /// </summary>
        /// <value>The value.</value>
        object Value { get; set; }

        /// <summary>
        /// Create Value by Evaluating a #Script JS Expression. Lightweight, only evaluates an AST Token.
        /// Results are only evaluated *once* and cached globally in AppHost.ScriptContext.Cache
        /// </summary>
        /// <value>The expression.</value>
        string Expression { get; set; }

        /// <summary>
        /// Create Value by evaluating #Script Code, results of same expression are cached per request
        /// </summary>
        /// <value>The eval.</value>
        string Eval { get; set; }

        /// <summary>
        /// Whether to disable result caching for this Script Value
        /// </summary>
        /// <value><c>true</c> if [no cache]; otherwise, <c>false</c>.</value>
        bool NoCache { get; set; }
    }

    /// <summary>
    /// Struct ScriptValue
    /// Implements the <see cref="ServiceStack.IScriptValue" />
    /// </summary>
    /// <seealso cref="ServiceStack.IScriptValue" />
    public struct ScriptValue : IScriptValue
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }
        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public string Expression { get; set; }
        /// <summary>
        /// Gets or sets the eval.
        /// </summary>
        /// <value>The eval.</value>
        public string Eval { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [no cache].
        /// </summary>
        /// <value><c>true</c> if [no cache]; otherwise, <c>false</c>.</value>
        public bool NoCache { get; set; }
    }

    /// <summary>
    /// Class ScriptValueAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// Implements the <see cref="ServiceStack.IScriptValue" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    /// <seealso cref="ServiceStack.IScriptValue" />
    public abstract class ScriptValueAttribute : AttributeBase, IScriptValue
    {
        /// <summary>
        /// Use constant Value
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }
        /// <summary>
        /// Create Value by Evaluating a #Script JS Expression. Lightweight, only evaluates an AST Token.
        /// Results are only evaluated *once* and cached globally in AppHost.ScriptContext.Cache
        /// </summary>
        /// <value>The expression.</value>
        public string Expression { get; set; }
        /// <summary>
        /// Create Value by evaluating #Script Code, results of same expression are cached per request
        /// </summary>
        /// <value>The eval.</value>
        public string Eval { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [no cache].
        /// </summary>
        /// <value><c>true</c> if [no cache]; otherwise, <c>false</c>.</value>
        public bool NoCache { get; set; }
    }

}