// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Intelligencia.UrlRewriter.Configuration;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter
{
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
            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            if (configurationManager == null)
            {
                throw new ArgumentNullException("configurationManager");
            }

            _engine = engine;
            _configurationManager = configurationManager;
            _httpContext = httpContext;
            _location = rawUrl;

            // Initialise the Properties collection from all the server variables, headers and cookies.
            foreach (string key in httpContext.ServerVariables.AllKeys)
            {
                _properties.Add(key, httpContext.ServerVariables[key]);
            }
            foreach (string key in httpContext.RequestHeaders.AllKeys)
            {
                _properties.Add(key, httpContext.RequestHeaders[key]);
            }
            foreach (string key in httpContext.RequestCookies.AllKeys)
            {
                _properties.Add(key, httpContext.RequestCookies[key].Value);
            }
        }

        /// <summary>
        /// The configuration manager.
        /// </summary>
        public IConfigurationManager ConfigurationManager
        {
            get { return _configurationManager; }
        }

        /// <summary>
        /// The current HTTP context.
        /// </summary>
        public IHttpContext HttpContext
        {
            get { return _httpContext; }
        }

        /// <summary>
        /// The current location being rewritten.
        /// </summary>
        /// <remarks>
        /// This property starts out as Request.RawUrl and is altered by various rewrite actions.
        /// </remarks>
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        /// <summary>
        /// The properties for the context, including headers and cookie values.
        /// </summary>
        public NameValueCollection Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Output response headers.
        /// </summary>
        /// <remarks>
        /// This collection is the collection of headers to add to the response.
        /// For the headers sent in the request, use the <see cref="RewriteContext.Properties">Properties</see> property.
        /// </remarks>
        public NameValueCollection ResponseHeaders
        {
            get { return _responseHeaders; }
        }

        /// <summary>
        /// The status code to send in the response.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        /// <summary>
        /// Collection of output cookies.
        /// </summary>
        /// <remarks>
        /// This is the collection of cookies to send in the response.  For the cookies
        /// received in the request, use the <see cref="RewriteContext.Properties">Properties</see> property.
        /// </remarks>
        public HttpCookieCollection ResponseCookies
        {
            get { return _responseCookies; }
        }

        /// <summary>
        /// Last matching pattern from a match (if any).
        /// </summary>
        public Match LastMatch
        {
            get { return _lastMatch; }
            set { _lastMatch = value; }
        }

        /// <summary>
        /// Expands the given input using the last match, properties, maps and transforms.
        /// </summary>
        /// <param name="input">The input to expand.</param>
        /// <returns>The expanded form of the input.</returns>
        public string Expand(string input)
        {
            return _engine.Expand(this, input);
        }

        /// <summary>
        /// Resolves the location to an absolute reference.
        /// </summary>
        /// <param name="location">The application-referenced location.</param>
        /// <returns>The absolute location.</returns>
        public string ResolveLocation(string location)
        {
            return _engine.ResolveLocation(location);
        }

        private RewriterEngine _engine;
        private IConfigurationManager _configurationManager;
        private IHttpContext _httpContext;
        private HttpStatusCode _statusCode = HttpStatusCode.OK;
        private string _location;
        private NameValueCollection _properties = new NameValueCollection();
        private NameValueCollection _responseHeaders = new NameValueCollection();
        private HttpCookieCollection _responseCookies = new HttpCookieCollection();
        private Match _lastMatch;
    }
}
