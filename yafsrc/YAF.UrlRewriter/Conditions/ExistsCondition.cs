// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Conditions
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
        /// <param name="location">The file location</param>
        public ExistsCondition(string location)
        {
            this._location = location ?? throw new ArgumentNullException(nameof(location));
        }

        /// <summary>
        /// Determines if the condition is matched.
        /// </summary>
        /// <param name="context">The rewriting context.</param>
        /// <returns>True if the condition is met.</returns>
        public bool IsMatch(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                var filename = context.HttpContext.MapPath(context.Expand(this._location));
                return File.Exists(filename) || Directory.Exists(filename);
            }
            catch
            {
                // An HTTP exception or an I/O exception indicates that the file definitely
                // does not exist.
                return false;
            }
        }

        private readonly string _location;
    }
}
