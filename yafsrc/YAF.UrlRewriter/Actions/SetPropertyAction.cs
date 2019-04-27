// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Actions
{
    using System;

    /// <summary>
    /// Action that sets properties in the context.
    /// </summary>
    public class SetPropertyAction : IRewriteAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The name of the value.</param>
        public SetPropertyAction(string name, string value)
        {
            this.Name = name ?? throw new ArgumentNullException("name");
            this.Value = value ?? throw new ArgumentNullException("value");
        }

        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The rewrite context.</param>
        public RewriteProcessing Execute(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Properties.Set(this.Name, context.Expand(this.Value));

            return RewriteProcessing.ContinueProcessing;
        }
    }
}
