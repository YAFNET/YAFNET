// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Net;
using System.Text.RegularExpressions;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Conditions
{
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
            _range = IPRange.Parse(pattern);
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
            string ipAddress = context.Properties[Constants.RemoteAddressHeader];
			if (ipAddress != null)
			{
				return _range.InRange(IPAddress.Parse(ipAddress));
			}
			else
			{
				return false;
			}
		}

		private IPRange _range;
	}
}
