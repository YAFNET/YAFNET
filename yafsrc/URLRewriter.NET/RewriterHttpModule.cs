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
using System.Resources;
using System.Reflection;
using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter
{
	/// <summary>
	/// Rewrites urls based on patterns and conditions specified in the configuration file.  
	/// This class cannot be inherited.
	/// </summary>
	public sealed class RewriterHttpModule : IHttpModule
	{
		/// <summary>
		/// Initialises the module.
		/// </summary>
		/// <param name="context">The application context.</param>
		void IHttpModule.Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(BeginRequest);
		}

		/// <summary>
		/// Disposes of the module.
		/// </summary>
		void IHttpModule.Dispose()
		{
		}

		/// <summary>
		/// Configuration of the rewriter.
		/// </summary>
		public static RewriterConfiguration Configuration
		{
			get
			{
				return RewriterConfiguration.Current;
			}
		}

		/// <summary>
		/// Resolves an Application-path relative location
		/// </summary>
		/// <param name="location">The location</param>
		/// <returns>The absolute location.</returns>
		public static string ResolveLocation(string location)
		{
			return _rewriter.ResolveLocation(location);
		}

		/// <summary>
		/// The original query string.
		/// </summary>
		public static string OriginalQueryString
		{
			get
			{
				return _rewriter.OriginalQueryString;
			}
			set
			{
				_rewriter.OriginalQueryString = value;
			}
		}

        /// <summary>
        /// The final querystring, after rewriting.
        /// </summary>
        public static string QueryString
        {
            get
            {
                return _rewriter.QueryString;
            }
            set
            {
                _rewriter.QueryString = value;
            }
        }

        /// <summary>
        /// The raw url, before rewriting.
        /// </summary>
        public static string RawUrl
        {
            get
            {
                return _rewriter.RawUrl;
            }
            set
            {
                _rewriter.RawUrl = value;
            }
        }

        private void BeginRequest(object sender, EventArgs e)
		{
			// Add our PoweredBy header
			HttpContext.Current.Response.AddHeader(Constants.HeaderXPoweredBy, Configuration.XPoweredBy);

			_rewriter.Rewrite();
		}

		private static RewriterEngine _rewriter = new RewriterEngine(new HttpContextFacade(), RewriterConfiguration.Current);
	}
}
