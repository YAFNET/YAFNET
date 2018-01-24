/* Based on BlogSpam.net API http://blogspamnetapi.codeplex.com/
 * 
 * The MIT License (MIT)
 * -------------------------------------
 * Copyright (c) 2011 Code Gecko
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace YAF.Core.Services.CheckForSpam
{
    #region

    using System;

    using CookComputing.XmlRpc;

    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The blog spam.
    /// </summary>
    public class BlogSpamNet
    {
        #region Constants and Fields

        /// <summary>
        /// The _url.
        /// </summary>
        private const string _Url = "http://test.blogspam.net:8888/";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogSpamNet"/> class.
        /// </summary>
        public BlogSpamNet()
        {
            Url = new Uri(_Url);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogSpamNet"/> class.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        public BlogSpamNet(string url)
        {
            Url = new Uri(url);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Url.
        /// </summary>
        private static Uri Url { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Classify a Comment as SPAM true or false
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <param name="isSpam">
        /// The is spam.
        /// </param>
        /// <returns>
        /// The classify comment.
        /// </returns>
        public static string ClassifyComment(string comment, bool isSpam)
        {
            return GetProxy().classifyComment(new TrainComment { comment = comment, train = isSpam ? "spam" : "ok" });
        }

        /// <summary>
        /// Get stats for an Url
        /// </summary>
        /// <param name="siteUrl">
        /// The site url.
        /// </param>
        /// <returns>
        /// Returns the Stats
        /// </returns>
        public static Stats GetStats(string siteUrl)
        {
            return GetProxy().getStats(siteUrl);
        }

        /// <summary>
        /// Test a Comment for SPAM
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="ignoreInternalIp">Ignore Internal Ip</param>
        /// <param name="answer">The answer.</param>
        /// <returns>
        /// Returns if Comment is SPAM
        /// </returns>
        public static bool CommentIsSpam(BlogSpamComment comment, bool ignoreInternalIp, out string answer)
        {
            answer = GetProxy().testComment(comment);

            var result = answer;

            // Handle interal Ips not as spam
            if (answer.Equals("SPAM:Internal Only IP") && ignoreInternalIp)
            {
                return false;
            }

            // Handle interal Ips not as spam
            if (answer.Equals("SPAM:No reverse DNS entry for ::1") && ignoreInternalIp)
            {
                return false;
            }

            if (result.Contains(":"))
            {
                result = result.Remove(result.IndexOf(":"));
            }

            switch (result)
            {
                case "OK":
                    return false;
                case "SPAM":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Test a Comment for SPAM
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <returns>
        /// The test comment.
        /// </returns>
        public static string TestComment(BlogSpamComment comment)
        {
            return GetProxy().testComment(comment);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get proxy.
        /// </summary>
        /// <returns>
        /// The Proxy
        /// </returns>
        internal static IBlogSpamNet GetProxy()
        {
            var _proxy = (IBlogSpamNet)XmlRpcProxyGen.Create(typeof(IBlogSpamNet));
            var _server = (XmlRpcClientProtocol)_proxy;
            _server.Url = (Url == null) ? _Url : Url.ToString();
            return _proxy;
        }

        #endregion
    }
}