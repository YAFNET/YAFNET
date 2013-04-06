/* httpcontext-simulator 
 * a simulator used to simulate http context during integration testing
 *
 * Copyright (C) Phil Haack 
 * http://code.google.com/p/httpcontext-simulator/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
*/

namespace HttpSimulator
{
    #region

    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Hosting;

    #endregion

    /// <summary>
    /// Used to simulate an HttpRequest.
    /// </summary>
    public class SimulatedHttpRequest : SimpleWorkerRequest
    {
        #region Constants and Fields

        /// <summary>
        /// The _host.
        /// </summary>
        private readonly string _host;

        /// <summary>
        /// The _physical file path.
        /// </summary>
        private readonly string _physicalFilePath;

        /// <summary>
        /// The _port.
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// The _verb.
        /// </summary>
        private readonly string _verb;

        /// <summary>
        /// The form variables.
        /// </summary>
        private readonly NameValueCollection formVariables = new NameValueCollection();

        /// <summary>
        /// The headers.
        /// </summary>
        private readonly NameValueCollection headers = new NameValueCollection();

        /// <summary>
        /// The _browser.
        /// </summary>
        private HttpBrowserCapabilities _browser;

        /// <summary>
        /// The _referer.
        /// </summary>
        private Uri _referer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedHttpRequest"/> class.
        /// Creates a new <see cref="SimulatedHttpRequest"/> instance.
        /// </summary>
        /// <param name="applicationPath">App virtual dir.</param>
        /// <param name="physicalAppPath">Physical Path to the app.</param>
        /// <param name="physicalFilePath">Physical Path to the file.</param>
        /// <param name="page">The Part of the URL after the application.</param>
        /// <param name="query">Query.</param>
        /// <param name="output">Output.</param>
        /// <param name="host">Host.</param>
        /// <param name="port">Port to request.</param>
        /// <param name="verb">The HTTP Verb to use.</param>
        public SimulatedHttpRequest(
            string applicationPath, 
            string physicalAppPath, 
            string physicalFilePath, 
            string page, 
            string query, 
            TextWriter output, 
            string host, 
            int port, 
            string verb)
            : base(applicationPath, physicalAppPath, page, query, output)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host", "Host cannot be null.");
            }

            if (host.Length == 0)
            {
                throw new ArgumentException("Host cannot be empty.", "host");
            }

            if (applicationPath == null)
            {
                throw new ArgumentNullException(
                    "applicationPath", "Can't create a request with a null application path. Try empty string.");
            }

            this._host = host;
            this._verb = verb;
            this._port = port;
            this._physicalFilePath = physicalFilePath;

            this._browser = new HttpBrowserCapabilities();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the format exception.
        /// </summary>
        /// <value> The format exception. </value>
        public NameValueCollection Form
        {
            get
            {
                return this.formVariables;
            }
        }

        /// <summary>
        ///   Gets the headers.
        /// </summary>
        /// <value> The headers. </value>
        public NameValueCollection Headers
        {
            get
            {
                return this.headers;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the virtual path to the currently executing server application.
        /// </summary>
        /// <returns>
        /// The virtual path of the current application. 
        /// </returns>
        public override string GetAppPath()
        {
            string appPath = base.GetAppPath();
            return appPath;
        }

        /// <summary>
        /// The get app path translated.
        /// </summary>
        /// <returns>
        /// The get app path translated.
        /// </returns>
        public override string GetAppPathTranslated()
        {
            string path = base.GetAppPathTranslated();
            return path;
        }

        /// <summary>
        /// The get file path translated.
        /// </summary>
        /// <returns>
        /// The get file path translated.
        /// </returns>
        public override string GetFilePathTranslated()
        {
            return this._physicalFilePath;
        }

        /// <summary>
        /// Returns the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The HTTP verb returned in the request header. 
        /// </returns>
        public override string GetHttpVerbName()
        {
            return this._verb;
        }

        /// <summary>
        /// The get known request header.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The get known request header.
        /// </returns>
        public override string GetKnownRequestHeader(int index)
        {
            if (index == 0x24)
            {
                return this._referer == null ? string.Empty : this._referer.ToString();
            }

            if (index == 12 && this._verb == "POST")
            {
                return "application/x-www-form-urlencoded";
            }

            return base.GetKnownRequestHeader(index);
        }

        /// <summary>
        /// The get local port.
        /// </summary>
        /// <returns>
        /// The get local port.
        /// </returns>
        public override int GetLocalPort()
        {
            return this._port;
        }

        /// <summary>
        /// Reads request data from the client (when not preloaded).
        /// </summary>
        /// <returns>
        /// The number of bytes read. 
        /// </returns>
        public override byte[] GetPreloadedEntityBody()
        {
            string formText = this.formVariables.Keys.Cast<string>().Aggregate(
                string.Empty, 
                (current, key) =>
                string.Format("{0}{1}", current, string.Format("{0}={1}&", key, this.formVariables[key])));

            return Encoding.UTF8.GetBytes(formText);
        }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        /// <returns>
        /// The get server name.
        /// </returns>
        public override string GetServerName()
        {
            return this._host;
        }

        /// <summary>
        /// Get all nonstandard HTTP header name-value pairs.
        /// </summary>
        /// <returns>
        /// An array of header name-value pairs. 
        /// </returns>
        public override string[][] GetUnknownRequestHeaders()
        {
            if (this.headers == null || this.headers.Count == 0)
            {
                return null;
            }

            string[][] headersArray = new string[this.headers.Count][];
            for (int i = 0; i < this.headers.Count; i++)
            {
                headersArray[i] = new string[2];
                headersArray[i][0] = this.headers.Keys[i];
                headersArray[i][1] = this.headers[i];
            }

            return headersArray;
        }

        /// <summary>
        /// The get uri path.
        /// </summary>
        /// <returns>
        /// The get uri path.
        /// </returns>
        public override string GetUriPath()
        {
            string uriPath = base.GetUriPath();
            return uriPath;
        }

        /// <summary>
        /// Returns a value indicating whether all request data is available and no further reads from the client are required.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if all request data is available; otherwise, <see langword="false"/> . 
        /// </returns>
        public override bool IsEntireEntityBodyIsPreloaded()
        {
            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The set referer.
        /// </summary>
        /// <param name="referer">
        /// The referer.
        /// </param>
        internal void SetReferer(Uri referer)
        {
            this._referer = referer;
        }

        #endregion
    }
}