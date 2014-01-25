// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Conditions
{
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
            _location = location;
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
            string filename = context.MapPath(context.Expand(_location));
			return File.Exists(filename) || Directory.Exists(filename);
		}

		private string _location;
	}
}
