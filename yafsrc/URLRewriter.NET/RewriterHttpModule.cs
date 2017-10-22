// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace Intelligencia.UrlRewriter
{
    using System;
    using System.Web;

    using Intelligencia.UrlRewriter.Configuration;
    using Intelligencia.UrlRewriter.Utilities;

    using YAF.Classes;

    /// <summary>
    /// Main HTTP Module for the URL Rewriter.
    /// Rewrites URL's based on patterns and conditions specified in the configuration file.
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
        /// The raw URL for the current request, before any rewriting.
        /// </summary>
        public static string RawUrl
        {
            get { return _rewriter.RawUrl; }
        }

        /// <summary>
        /// Event handler for the "BeginRequest" event.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event args</param>
        private void BeginRequest(object sender, EventArgs args)
        {
            // Add our PoweredBy header
            // HttpContext.Current.Response.AddHeader(Constants.HeaderXPoweredBy, Configuration.XPoweredBy);
            
            // Allow a bypass to be set up by the using application
            HttpContext context = HttpContext.Current;
            if (context.Items.Contains(@"Intelligencia.UrlRewriter.Bypass") && 
                context.Items[@"Intelligencia.UrlRewriter.Bypass"] is bool && 
                ((bool)context.Items[@"Intelligencia.UrlRewriter.Bypass"]))
            {
                // A bypass is set!
                return;
            }

            _rewriter.Rewrite();
        }

        private static RewriterEngine _rewriter = new RewriterEngine(
            new HttpContextFacade(),
            new ConfigurationManagerFacade(),
            new RewriterConfiguration());
    }
}
