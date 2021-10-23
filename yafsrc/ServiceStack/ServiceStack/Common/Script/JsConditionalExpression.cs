// ***********************************************************************
// <copyright file="JsConditionalExpression.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    /// <summary>
    /// Class JsConditionalExpression.
    /// Implements the <see cref="ServiceStack.Script.JsExpression" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.JsExpression" />
    public class JsConditionalExpression : JsExpression
    {
        /// <summary>
        /// Gets the test.
        /// </summary>
        /// <value>The test.</value>
        public JsToken Test { get; }

        /// <summary>
        /// Gets the consequent.
        /// </summary>
        /// <value>The consequent.</value>
        public JsToken Consequent { get; }

        /// <summary>
        /// Gets the alternate.
        /// </summary>
        /// <value>The alternate.</value>
        public JsToken Alternate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsConditionalExpression"/> class.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="consequent">The consequent.</param>
        /// <param name="alternate">The alternate.</param>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Test Expression missing in Conditional Expression</exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Consequent Expression missing in Conditional Expression</exception>
        /// <exception cref="ServiceStack.Script.SyntaxErrorException">Alternate Expression missing in Conditional Expression</exception>
        public JsConditionalExpression(JsToken test, JsToken consequent, JsToken alternate)
        {
            Test = test ?? throw new SyntaxErrorException($"Test Expression missing in Conditional Expression");
            Consequent = consequent ?? throw new SyntaxErrorException($"Consequent Expression missing in Conditional Expression");
            Alternate = alternate ?? throw new SyntaxErrorException($"Alternate Expression missing in Conditional Expression");
        }

        /// <summary>
        /// Converts to rawstring.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string ToRawString()
        {
            var sb = StringBuilderCache.Allocate();
            sb.Append(Test.ToRawString());
            sb.Append(" ? ");
            sb.Append(Consequent.ToRawString());
            sb.Append(" : ");
            sb.Append(Alternate.ToRawString());
            return StringBuilderCache.ReturnAndFree(sb);
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
                ["test"] = Test.ToJsAst(),
                ["consequent"] = Consequent.ToJsAst(),
                ["alternate"] = Alternate.ToJsAst(),
            };
            return to;
        }

        /// <summary>
        /// Evaluates the specified scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>System.Object.</returns>
        public override object Evaluate(ScriptScopeContext scope)
        {
            var test = Test.EvaluateToBool(scope);
            var value = test
                ? Consequent.Evaluate(scope)
                : Alternate.Evaluate(scope);
            return value;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool Equals(JsConditionalExpression other)
        {
            return Equals(Test, other.Test) &&
                   Equals(Consequent, other.Consequent) &&
                   Equals(Alternate, other.Alternate);
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
            return Equals((JsConditionalExpression)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Test != null ? Test.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Consequent != null ? Consequent.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Alternate != null ? Alternate.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}