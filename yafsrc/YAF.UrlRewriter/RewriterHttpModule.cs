// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="RewriterHttpModule.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace YAF.UrlRewriter
{
    using System;
    using System.Web;

    using YAF.Configuration;
    using YAF.UrlRewriter.Configuration;
    using YAF.UrlRewriter.Utilities;

    /// <summary>
    /// Main HTTP Module for the URL Rewriter.
    /// Rewrites URL's based on patterns and conditions specified in the configuration file.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class RewriterHttpModule : IHttpModule
    {
        /// <summary>
        /// Initializes the module.
        /// </summary>
        /// <param name="context">The application context.</param>
        void IHttpModule.Init(HttpApplication context)
        {
            if (!Config.EnableURLRewriting)
            {
                return;
            }

            context.BeginRequest += BeginRequest;
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
        public static string RawUrl => _rewriter.RawUrl;

        /// <summary>
        /// Event handler for the "BeginRequest" event.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="args"></param>
        private static void BeginRequest(object sender, EventArgs args)
        {
            // Add our PoweredBy header
            // HttpContext.Current.Response.AddHeader(Constants.HeaderXPoweredBy, Configuration.XPoweredBy);

            // Allow a bypass to be set up by the using application
            var context = HttpContext.Current;
            if (context.Items.Contains(@"Intelligencia.UrlRewriter.Bypass") && 
                context.Items[@"Intelligencia.UrlRewriter.Bypass"] is bool && 
                (bool)context.Items[@"Intelligencia.UrlRewriter.Bypass"])
            {
                // A bypass is set!
                return;
            }

            _rewriter.Rewrite();
        }

        /// <summary>
        /// The _rewriter.
        /// </summary>
        private static readonly RewriterEngine _rewriter = new RewriterEngine(
            new HttpContextFacade(),
            new ConfigurationManagerFacade(),
            new RewriterConfiguration());
    }
}
