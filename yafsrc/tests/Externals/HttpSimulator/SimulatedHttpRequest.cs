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
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Used to simulate an HttpRequest.
    /// </summary>
    public class SimulatedHttpRequest : SimpleWorkerRequest
    {
        /// <summary>
        /// The host.
        /// </summary>
        private readonly string host;

        /// <summary>
        /// The _physical file path.
        /// </summary>
        private readonly string physicalFilePath;

        /// <summary>
        /// The port.
        /// </summary>
        private readonly int port;

        /// <summary>
        /// The verb.
        /// </summary>
        private readonly string verb;

        /// <summary>
        /// The referer.
        /// </summary>
        private Uri referer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedHttpRequest" /> class.
        /// Creates a new <see cref="SimulatedHttpRequest" /> instance.
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
        /// <exception cref="System.ArgumentNullException">
        /// host;Host cannot be null.
        /// or
        /// applicationPath;Can't create a request with a null application path. Try empty string.
        /// </exception>
        /// <exception cref="System.ArgumentException">Host cannot be empty.;host</exception>
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
                throw new ArgumentNullException(nameof(host), "Host cannot be null.");
            }

            if (host.Length == 0)
            {
                throw new ArgumentException("Host cannot be empty.", nameof(host));
            }

            if (applicationPath == null)
            {
                throw new ArgumentNullException(
                    nameof(applicationPath), "Can't create a request with a null application path. Try empty string.");
            }

            this.host = host;
            this.verb = verb;
            this.port = port;
            this.physicalFilePath = physicalFilePath;

            //this.browser = new HttpBrowserCapabilities();
        }

        /// <summary>
        ///   Gets the format exception.
        /// </summary>
        /// <value> The format exception. </value>
        public NameValueCollection Form { get; } = new NameValueCollection();

        /// <summary>
        ///   Gets the headers.
        /// </summary>
        /// <value> The headers. </value>
        public NameValueCollection Headers { get; } = new NameValueCollection();

        /// <summary>
        /// Returns the virtual path to the currently executing server application.
        /// </summary>
        /// <returns>
        /// The virtual path of the current application. 
        /// </returns>
        public override string GetAppPath()
        {
            var appPath = base.GetAppPath();
            return appPath;
        }

        /// <summary>
        /// The get app path translated.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string GetAppPathTranslated()
        {
            var path = base.GetAppPathTranslated();
            return path;
        }

        /// <summary>
        /// The get file path translated.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string GetFilePathTranslated()
        {
            return this.physicalFilePath;
        }

        /// <summary>
        /// Returns the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The HTTP verb returned in the request header. 
        /// </returns>
        public override string GetHttpVerbName()
        {
            return this.verb;
        }

        /// <summary>
        /// The get known request header.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string GetKnownRequestHeader(int index)
        {
            switch (index)
            {
                case 0x24:
                    return this.referer == null ? string.Empty : this.referer.ToString();
                case 12 when this.verb == "POST":
                    return "application/x-www-form-urlencoded";
                default:
                    return base.GetKnownRequestHeader(index);
            }
        }

        /// <summary>
        /// The get local port.
        /// </summary>
        /// <returns>
        /// The get local port.
        /// </returns>
        public override int GetLocalPort()
        {
            return this.port;
        }

        /// <summary>
        /// Reads request data from the client (when not preloaded).
        /// </summary>
        /// <returns>
        /// The number of bytes read. 
        /// </returns>
        public override byte[] GetPreloadedEntityBody()
        {
            var formText = this.Form.Keys.Cast<string>().Aggregate(
                string.Empty, 
                (current, key) => $"{current}{key}={this.Form[key]}&");

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
            return this.host;
        }

        /// <summary>
        /// Get all nonstandard HTTP header name-value pairs.
        /// </summary>
        /// <returns>
        /// An array of header name-value pairs. 
        /// </returns>
        public override string[][] GetUnknownRequestHeaders()
        {
            if (this.Headers == null || this.Headers.Count == 0)
            {
                return null;
            }

            var headersArray = new string[this.Headers.Count][];
            for (var i = 0; i < this.Headers.Count; i++)
            {
                headersArray[i] = new string[2];
                headersArray[i][0] = this.Headers.Keys[i];
                headersArray[i][1] = this.Headers[i];
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
            var uriPath = base.GetUriPath();
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

        /// <summary>
        /// The set referer.
        /// </summary>
        /// <param name="referer">
        /// The referer.
        /// </param>
        internal void SetReferer(Uri referer)
        {
            this.referer = referer;
        }
    }
}