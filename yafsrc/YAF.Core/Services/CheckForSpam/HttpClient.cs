/* Based on "Subkismet - The Cure For Comment Spam" v1.0: http://subkismet.codeplex.com/
 * 
 * License: New BSD License
 * -------------------------------------
 * Copyright (c) 2007-2008, Phil Haack
 * All rights reserved. 
 * Modified by Jaben Cargman for YAF in 2010 
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * 
 * Neither the name of Subkismet nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.* 
*/

namespace YAF.Core.Services.CheckForSpam
{
    #region Using

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;

    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Class used to make the actual HTTP requests.
    /// </summary>
    public class HttpClient
    {
        #region Public Methods

        /// <summary>
        /// Gets the content of a web page.
        /// </summary>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <param name="timeout">
        /// The timeout in miliseconds.
        /// </param>
        /// <param name="proxy">
        /// The proxy.
        /// </param>
        /// <returns>
        /// The string value of requested web page.
        /// </returns>
        /// <exception cref="InvalidResponseException"><c>InvalidResponseException</c>.</exception>
        [NotNull]
        public virtual string GetRequest(
            [NotNull] Uri url,
            [NotNull] string userAgent,
            int timeout,
            [NotNull] IWebProxy proxy)
        {
            CodeContracts.VerifyNotNull(url, "url");
            CodeContracts.VerifyNotNull(userAgent, "userAgent");
            CodeContracts.VerifyNotNull(proxy, "proxy");

            ServicePointManager.Expect100Continue = false;
            var request = WebRequest.Create(url) as HttpWebRequest;

            Debug.Assert(
                request != null,
                "HttpWebRequest should not be null",
                "Calling WebRequest.Create(url) produced a null HttpWebRequest instance for the URL '{0}'".FormatWith(
                    url));

            if (proxy != null)
            {
                request.Proxy = proxy;
            }

            request.UserAgent = userAgent;
            request.Timeout = timeout;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            request.KeepAlive = true;

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode < HttpStatusCode.OK && response.StatusCode >= HttpStatusCode.Ambiguous)
            {
                throw new InvalidResponseException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The service was not able to handle our request. Http Status '{0}'.",
                        response.StatusCode),
                    response.StatusCode);
            }

            string responseText;
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
            {
                // They only return "true" or "false"
                responseText = reader.ReadToEnd();
            }

            return responseText;
        }

        /// <summary>
        /// Posts the request and returns a text response.  
        ///   This is all that is needed for Akismet.
        /// </summary>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <param name="formParameters">
        /// The properly formatted parameters.
        /// </param>
        /// <returns>
        /// The post request.
        /// </returns>
        public virtual string PostRequest(
            [NotNull] Uri url,
            [NotNull] string userAgent,
            int timeout,
            [NotNull] string formParameters)
        {
            return this.PostRequest(url, userAgent, timeout, formParameters, null);
        }

        /// <summary>
        /// Posts the request.
        /// </summary>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <param name="userAgent">
        /// The user agent.
        /// </param>
        /// <param name="timeout">
        /// The timeout in milliseconds.
        /// </param>
        /// <param name="formParameters">
        /// The form parameters.
        /// </param>
        /// <param name="proxy">
        /// The proxy.
        /// </param>
        /// <returns>
        /// The post request.
        /// </returns>
        /// <exception cref="InvalidResponseException"><c>InvalidResponseException</c>.</exception>
        [NotNull]
        public virtual string PostRequest(
            [NotNull] Uri url,
            [NotNull] string userAgent,
            int timeout,
            [NotNull] string formParameters,
            [CanBeNull] IWebProxy proxy)
        {
            CodeContracts.VerifyNotNull(url, "url");
            CodeContracts.VerifyNotNull(userAgent, "userAgent");
            CodeContracts.VerifyNotNull(formParameters, "formParameters");

            ServicePointManager.Expect100Continue = false;
            var request = WebRequest.Create(url) as HttpWebRequest;

            Debug.Assert(
                request != null,
                "HttpWebRequest should not be null",
                "Calling WebRequest.Create(url) produced a null HttpWebRequest instance for the URL '{0}'".FormatWith(
                    url));

            if (proxy != null)
            {
                request.Proxy = proxy;
            }

            request.UserAgent = userAgent;
            request.Timeout = timeout;
            request.Method = "POST";
            request.ContentLength = formParameters.Length;
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            request.KeepAlive = true;

            using (var myWriter = new StreamWriter(request.GetRequestStream()))
            {
                myWriter.Write(formParameters);
            }

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode < HttpStatusCode.OK && response.StatusCode >= HttpStatusCode.Ambiguous)
            {
                throw new InvalidResponseException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The service was not able to handle our request. Http Status '{0}'.",
                        response.StatusCode),
                    response.StatusCode);
            }

            string responseText;
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
            {
                // They only return "true" or "false"
                responseText = reader.ReadToEnd();
            }

            return responseText;
        }

        #endregion
    }
}