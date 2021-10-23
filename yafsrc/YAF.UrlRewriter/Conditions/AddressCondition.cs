// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Conditions
{
    using System;
    using System.Net;

    using YAF.UrlRewriter.Utilities;

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
                throw new ArgumentNullException(nameof(pattern));
            }

            this._range = IPRange.Parse(pattern);
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

            var ipAddress = context.Properties[Constants.RemoteAddressHeader];

            return ipAddress != null && this._range.InRange(IPAddress.Parse(ipAddress));
        }

        private readonly IPRange _range;
    }
}
