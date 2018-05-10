// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Conditions
{
    using System;
    using System.Net;

    using Intelligencia.UrlRewriter.Utilities;

    /// <summary>
    /// Matches on the current remote IP address.
    /// </summary>
    public sealed class AddressCondition : IRewriteCondition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pattern"></param>
        public AddressCondition(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            this._range = IPRange.Parse(pattern);
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

            var ipAddress = context.Properties[Constants.RemoteAddressHeader];
            if (ipAddress != null)
            {
                return this._range.InRange(IPAddress.Parse(ipAddress));
            }

            return false;
        }

        private IPRange _range;
    }
}