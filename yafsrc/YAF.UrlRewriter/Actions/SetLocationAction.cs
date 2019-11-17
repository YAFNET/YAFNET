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
    /// Sets the Location.
    /// </summary>
    public abstract class SetLocationAction : IRewriteAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="location">The location (pattern) to set.</param>
        protected SetLocationAction(string location)
        {
            this.Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        /// <summary>
        /// The location to set.  This can include replacements referencing the matched pattern,
        /// for example $1, $2, ... $n and ${group} as well as ${ServerVariable} and mapping, e.g., 
        /// ${MapName:$1}.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        public virtual RewriteProcessing Execute(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Location = context.ResolveLocation(context.Expand(this.Location));

            return RewriteProcessing.StopProcessing;
        }
    }
}
