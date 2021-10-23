// ***********************************************************************
// <copyright file="JsVariableDeclaration.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    /// <summary>
    /// Class JsVariableDeclaration.
    /// Implements the <see cref="ServiceStack.Script.JsExpression" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.JsExpression" />
    public class JsVariableDeclaration : JsExpression
    {
        /// <summary>
        /// Gets the kind.
        /// </summary>
        /// <value>The kind.</value>
        public JsVariableDeclarationKind Kind { get; }
        /// <summary>
        /// Gets the declarations.
        /// </summary>
        /// <value>The declarations.</value>
        public JsDeclaration[] Declarations { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsVariableDeclaration"/> class.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="declarations">The declarations.</param>
        public JsVariableDeclaration(JsVariableDeclarationKind kind, params JsDeclaration[] declarations)
        {
            Kind = kind;
            Declarations = declarations;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool Equals(JsVariableDeclaration other) =>
            Kind == other.Kind && Declarations.SequenceEqual(other.Declarations);

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
            return Equals((JsVariableDeclaration)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Kind * 397) ^ (Declarations != null ? Declarations.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Converts to rawstring.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string ToRawString()
        {
            var sb = StringBuilderCache.Allocate();
            sb.Append(Kind.ToString().ToLower()).Append(" ");

            for (var i = 0; i < Declarations.Length; i++)
            {
                var dec = Declarations[i];
                if (i > 0)
                    sb.Append(", ");
                sb.Append(dec.ToRawString());
            }
            return StringBuilderCache.ReturnAndFree(sb);
        }

        /// <summary>
        /// Evaluates the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public override object Evaluate(ScriptScopeContext scope)
        {
            foreach (var declaration in Declarations)
            {
                scope.ScopedParams[declaration.Id.Name] = declaration.Init?.Evaluate(scope);
            }
            return JsNull.Value;
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
                ["declarations"] = Declarations.Map(x => (object)x.ToJsAst()),
                ["kind"] = Kind.ToString().ToLower(),
            };
            return to;
        }
    }


    /// <summary>
    /// Enum JsVariableDeclarationKind
    /// </summary>
    public enum JsVariableDeclarationKind
    {
        /// <summary>
        /// The variable
        /// </summary>
        Var,
        /// <summary>
        /// The let
        /// </summary>
        Let,
        /// <summary>
        /// The constant
        /// </summary>
        Const,
    }

    /// <summary>
    /// Class JsDeclaration.
    /// Implements the <see cref="ServiceStack.Script.JsExpression" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.JsExpression" />
    public class JsDeclaration : JsExpression
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public JsIdentifier Id { get; set; }

        /// <summary>
        /// Gets or sets the initialize.
        /// </summary>
        /// <value>The initialize.</value>
        public JsToken Init { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsDeclaration"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="init">The initialize.</param>
        public JsDeclaration(JsIdentifier id, JsToken init)
        {
            Id = id;
            Init = init;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool Equals(JsDeclaration other) => Equals(Id, other.Id) && Equals(Init, other.Init);

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
            return Equals((JsDeclaration)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (Init != null ? Init.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Converts to rawstring.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string ToRawString() => Init != null
            ? $"{Id.Name} = {Init.ToRawString()}"
            : Id.Name;

        /// <summary>
        /// Evaluates the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object Evaluate(ScriptScopeContext scope)
        {
            throw new System.NotImplementedException();
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
                ["id"] = Id.Name,
                ["init"] = Init?.ToJsAst(),
            };
            return to;
        }
    }
}