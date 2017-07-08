// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Conditions
{
    using System;
    using System.IO;

    /// <summary>
    /// Condition that tests the existence of a file.
    /// </summary>
    public class ExistsCondition : IRewriteCondition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location"></param>
        public ExistsCondition(string location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            this._location = location;
        }

        /// <summary>
        /// Determines if the condition is matched.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        /// <returns>True if the condition is met.</returns>
        public bool IsMatch(RewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var filename = context.MapPath(context.Expand(this._location));
            return File.Exists(filename) || Directory.Exists(filename);
        }

        private string _location;
    }
}