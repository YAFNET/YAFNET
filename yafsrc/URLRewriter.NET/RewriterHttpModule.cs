/* UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
*/ 

namespace Intelligencia.UrlRewriter
{
    using System;
    using System.Web;

    using Intelligencia.UrlRewriter.Configuration;
    using Intelligencia.UrlRewriter.Utilities;

    using YAF.Classes;

    /// <summary>
    /// Rewrites URLs based on patterns and conditions specified in the configuration file.  
    /// This class cannot be inherited.
    /// </summary>
    public sealed class RewriterHttpModule : IHttpModule
    {
        /// <summary>
        /// The _rewriter
        /// </summary>
        private static readonly RewriterEngine _Rewriter = new RewriterEngine(
            new HttpContextFacade(),
            RewriterConfiguration.Current);

        /// <summary>
        /// Gets or sets the original query string.
        /// </summary>
        public static string OriginalQueryString
        {
            get
            {
                return _Rewriter.OriginalQueryString;
            }

            set
            {
                _Rewriter.OriginalQueryString = value;
            }
        }

        /// <summary>
        /// Gets or sets the final query string, after rewriting.
        /// </summary>
        public static string QueryString
        {
            get
            {
                return _Rewriter.QueryString;
            }

            set
            {
                _Rewriter.QueryString = value;
            }
        }

        /// <summary>
        /// Gets or sets the raw url, before rewriting.
        /// </summary>
        public static string RawUrl
        {
            get
            {
                return _Rewriter.RawUrl;
            }

            set
            {
                _Rewriter.RawUrl = value;
            }
        }

        /// <summary>
        /// Gets the Configuration of the rewriter.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
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
            return _Rewriter.ResolveLocation(location);
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">The application context.</param>
        void IHttpModule.Init(HttpApplication context)
        {
            if (!Config.EnableURLRewriting)
            {
                return;
            }

            context.BeginRequest += this.BeginRequest;
        }

        /// <summary>
        /// Disposes of the module.
        /// </summary>
        void IHttpModule.Dispose()
        {
        }

        /// <summary>
        /// Begins the request.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BeginRequest(object sender, EventArgs e)
        {
            // Add our PoweredBy header
            HttpContext.Current.Response.AddHeader(Constants.HeaderXPoweredBy, Configuration.XPoweredBy);

            _Rewriter.Rewrite();
        }
    }
}