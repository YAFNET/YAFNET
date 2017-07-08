// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter
{
    using System.Collections.Specialized;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;

    using Intelligencia.UrlRewriter.Utilities;

    /// <summary>
    /// Encapsulates all rewriting information about an individual rewrite request.  This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This class cannot be created directly.  It will be provided to actions and conditions
    /// by the framework.
    /// </remarks>
    public sealed class RewriteContext
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="engine">The rewriting engine.</param>
        /// <param name="rawUrl">The initial, raw URL.</param>
        /// <param name="httpMethod">The HTTP method used (GET, POST, ...)</param>
        /// <param name="mapPath">The method to use for mapping paths.</param>
        /// <param name="serverVariables">Collection of server variables.</param>
        /// <param name="headers">Collection of headers.</param>
        /// <param name="cookies">Collection of cookies.</param>
        internal RewriteContext(
            RewriterEngine engine,
            string rawUrl,
            string httpMethod,
            MapPath mapPath,
            NameValueCollection serverVariables,
            NameValueCollection headers,
            HttpCookieCollection cookies)
        {
            this._engine = engine;
            this._location = rawUrl;
            this._method = httpMethod;
            this._mapPath = mapPath;

            foreach (string key in serverVariables)
            {
                this._properties.Add(key, serverVariables[key]);
            }

            foreach (string key in headers)
            {
                this._properties.Add(key, headers[key]);
            }

            foreach (string key in cookies)
            {
                this._properties.Add(key, cookies[key].Value);
            }
        }

        /// <summary>
        /// Maps the given URL to the absolute local path.
        /// </summary>
        /// <param name="url">The URL to map.</param>
        /// <returns>The absolute local file path relating to the url.</returns>
        public string MapPath(string url)
        {
            return this._mapPath(url);
        }

        /// <summary>
        /// The current location being rewritten.
        /// </summary>
        /// <remarks>
        /// This property starts out as Request.RawUrl and is altered by various
        /// rewrite actions.
        /// </remarks>
        public string Location
        {
            get => this._location;
            set => this._location = value;
        }

        /// <summary>
        /// The request method (GET, PUT, POST, HEAD, DELETE).
        /// </summary>
        public string Method => this._method;

        /// <summary>
        /// The properties for the context, including headers and cookie values.
        /// </summary>
        public NameValueCollection Properties => this._properties;

        /// <summary>
        /// Output headers.
        /// </summary>
        /// <remarks>
        /// This collection is the collection of headers to add to the response.
        /// For the headers sent in the request, use the <see cref="RewriteContext.Properties">Properties</see> property.
        /// </remarks>
        public NameValueCollection Headers => this._headers;

        /// <summary>
        /// The status code to send in the response.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get => this._statusCode;
            set => this._statusCode = value;
        }

        /// <summary>
        /// Collection of output cookies.
        /// </summary>
        /// <remarks>
        /// This is the collection of cookies to send in the response.  For the cookies
        /// received in the request, use the <see cref="RewriteContext.Properties">Properties</see> property.
        /// </remarks>
        public HttpCookieCollection Cookies => this._cookies;

        /// <summary>
        /// Last matching pattern from a match (if any).
        /// </summary>
        public Match LastMatch
        {
            get => this._lastMatch;
            set => this._lastMatch = value;
        }

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

        private RewriterEngine _engine;

        private string _method = string.Empty;

        private HttpStatusCode _statusCode = HttpStatusCode.OK;

        private string _location;

        private NameValueCollection _properties = new NameValueCollection();

        private NameValueCollection _headers = new NameValueCollection();

        private HttpCookieCollection _cookies = new HttpCookieCollection();

        private Match _lastMatch;

        private MapPath _mapPath;
    }
}