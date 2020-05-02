// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="RewriteContext.cs">
//   Copyright (c)2011 Seth Yates
//   Author Seth Yates
//   Author Stewart Rae
// </copyright>
// <summary>
// Forked Version for YAF.NET
// Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.UrlRewriter
{
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;

    using YAF.Types.Extensions;
    using YAF.UrlRewriter.Configuration;
    using YAF.UrlRewriter.Utilities;

    /// <summary>
    /// Encapsulates all rewriting information about an individual rewrite request.
    /// This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This class cannot be created directly.
    /// It will be provided to actions and conditions by the framework.
    /// </remarks>
    internal class RewriteContext : IRewriteContext
    {
        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="engine">The rewriting engine.</param>
        /// <param name="rawUrl">The initial, raw URL.</param>
        /// <param name="httpContext">The HTTP context facade.</param>
        /// <param name="configurationManager">The configuration manager facade.</param>
        internal RewriteContext(
            RewriterEngine engine,
            string rawUrl,
            IHttpContext httpContext,
            IConfigurationManager configurationManager)
        {
            this._engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.ConfigurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            this.HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            this.Location = rawUrl;

            // Initialise the Properties collection from all the server variables, headers and cookies.
            httpContext.ServerVariables.AllKeys.ForEach(key => this.Properties.Add(key, httpContext.ServerVariables[key]));

            httpContext.RequestHeaders.AllKeys.ForEach(
                key => this.Properties.Add(key, httpContext.RequestHeaders[key]));

            httpContext.RequestCookies.AllKeys.ForEach(
                key => this.Properties.Add(key, httpContext.RequestCookies[key].Value));
        }

        /// <summary>
        /// The configuration manager.
        /// </summary>
        public IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// The current HTTP context.
        /// </summary>
        public IHttpContext HttpContext { get; }

        /// <summary>
        /// The current location being rewritten.
        /// </summary>
        /// <remarks>
        /// This property starts out as Request.RawUrl and is altered by various rewrite actions.
        /// </remarks>
        public string Location { get; set; }

        /// <summary>
        /// The properties for the context, including headers and cookie values.
        /// </summary>
        public NameValueCollection Properties { get; } = new NameValueCollection();

        /// <summary>
        /// Output response headers.
        /// </summary>
        /// <remarks>
        /// This collection is the collection of headers to add to the response.
        /// For the headers sent in the request, use the <see cref="RewriteContext.Properties">Properties</see> property.
        /// </remarks>
        public NameValueCollection ResponseHeaders { get; } = new NameValueCollection();

        /// <summary>
        /// The status code to send in the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// Collection of output cookies.
        /// </summary>
        /// <remarks>
        /// This is the collection of cookies to send in the response.  For the cookies
        /// received in the request, use the <see cref="RewriteContext.Properties">Properties</see> property.
        /// </remarks>
        public HttpCookieCollection ResponseCookies { get; } = new HttpCookieCollection();

        /// <summary>
        /// Last matching pattern from a match (if any).
        /// </summary>
        public Match LastMatch { get; set; }

        /// <summary>
        /// Expands the given input using the last match, properties, maps and transforms.
        /// </summary>
        /// <param name="input">The input to expand.</param>
        /// <returns>The expanded form of the input.</returns>
        public string Expand(string input)
        {
            return this._engine.Expand(this, input);
        }

        /// <summary>
        /// Resolves the location to an absolute reference.
        /// </summary>
        /// <param name="location">The application-referenced location.</param>
        /// <returns>The absolute location.</returns>
        public string ResolveLocation(string location)
        {
            return this._engine.ResolveLocation(location);
        }

        private readonly RewriterEngine _engine;
    }
}
