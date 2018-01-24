/*
 * Base oAuth Class for Twitter and LinkedIn
 * Author: Eran Sandler 
 * Code Url: http://oauth.net/code/
 * Author Url: http://eran.sandler.co.il/
 *
 * Some modifications by Shannon Whitley
 * Author Url: http://voiceoftech.com/
 * 
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted 
 * provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of conditions 
 * and the following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions
 * and the following disclaimer in the documentation and/or other materials provided with the distribution.

 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR 
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND 
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS 
 * BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace YAF.Core.Services.Auth
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    /// <summary>
    /// AUTH Utilities
    /// </summary>
    public static class AuthUtilities
    {
        /// <summary>
        /// Request Methods
        /// </summary>
        public enum Method
        {
            /// <summary>
            /// The GET
            /// </summary>
            GET,

            /// <summary>
            /// The POST
            /// </summary>
            POST,

            /// <summary>
            /// The PUT
            /// </summary>
            PUT,

            /// <summary>
            /// The DELETE
            /// </summary>
            DELETE
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">
        /// Http Method
        /// </param>
        /// <param name="url">
        /// Full url to the web resource
        /// </param>
        /// <param name="postData">
        /// Data to post in query string format
        /// </param>
        /// <returns>
        /// The web server response.
        /// </returns>
        public static string WebRequest(Method method, string url, string postData)
        {
            return WebRequest(method, url, postData, null);
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in query string format</param>
        /// <param name="headers">The headers.</param>
        /// <returns>
        /// The web server response.
        /// </returns>
        public static string WebRequest(Method method, string url, string postData, List<KeyValuePair<string, string>> headers)
        {
            var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    webRequest.Headers.Add(header.Key, header.Value);
                }
            }

            if (method == Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

                // POST the data.
                var requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                finally
                {
                    requestWriter.Close();
                }
            }

            var responseData = WebResponseGet(webRequest);

            return responseData;
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">
        /// The request object.
        /// </param>
        /// <returns>
        /// The response data.
        /// </returns>
        public static string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData;

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();

                if (responseReader != null)
                {
                    responseReader.Close();
                }
            }

            return responseData;
        }
    }
}