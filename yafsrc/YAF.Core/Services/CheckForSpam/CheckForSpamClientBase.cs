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
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Web;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.CheckForSpam;

    #endregion

    /// <summary>
    /// The client class used to communicate with the spam service.
    /// </summary>
    [Serializable]
    public abstract class CheckForSpamClientBase : ICheckForSpamClient
    {
        #region Constants and Fields

        /// <summary>
        ///   The http client.
        /// </summary>
        [NonSerialized]
        protected readonly HttpClient httpClient;

        /// <summary>
        ///   The api key.
        /// </summary>
        protected string apiKey;

        /// <summary>
        ///   The proxy.
        /// </summary>
        protected IWebProxy proxy;

        /// <summary>
        ///   The root url.
        /// </summary>
        protected Uri rootUrl;

        /// <summary>
        ///   The check url.
        /// </summary>
        protected Uri submitCheckUrl;

        /// <summary>
        ///   The submit ham url.
        /// </summary>
        protected Uri submitHamUrl;

        /// <summary>
        ///   The submit spam url.
        /// </summary>
        protected Uri submitSpamUrl;

        /// <summary>
        ///   The timeout.
        /// </summary>
        protected int timeout = 5000;

        /// <summary>
        ///   The user agent.
        /// </summary>
        protected string userAgent;

        /// <summary>
        ///   The verify url.
        /// </summary>
        protected Uri verifyUrl;

        /// <summary>
        ///   The version.
        /// </summary>
        private static readonly string version = typeof(HttpClient).Assembly.GetName().Version.ToString();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckForSpamClientBase"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor takes in all the dependencies to allow for 
        ///   dependency injection and unit testing. Seems like overkill, 
        ///   but it's worth it.
        /// </remarks>
        /// <param name="apiKey">
        /// The Akismet API key.
        /// </param>
        /// <param name="blogUrl">
        /// The root url of the blog.
        /// </param>
        /// <param name="httpClient">
        /// Client class used to make the underlying requests.
        /// </param>
        protected CheckForSpamClientBase(
            [NotNull] string apiKey,
            [NotNull] Uri blogUrl,
            [NotNull] HttpClient httpClient)
        {
            CodeContracts.VerifyNotNull(apiKey, "apiKey");
            CodeContracts.VerifyNotNull(blogUrl, "blogUrl");
            CodeContracts.VerifyNotNull(httpClient, "httpClient");

            this.apiKey = apiKey;
            this.rootUrl = blogUrl;
            this.httpClient = httpClient;

            this.SetServiceUrls();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckForSpamClientBase"/> class.
        /// </summary>
        /// <param name="apiKey">
        /// The Akismet API key.
        /// </param>
        /// <param name="rootUrl">
        /// The root url of the site using Akismet.
        /// </param>
        protected CheckForSpamClientBase([NotNull] string apiKey, [NotNull] Uri rootUrl)
            : this(apiKey, rootUrl, new HttpClient())
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the Akismet API key.
        /// </summary>
        /// <value>The API key.</value>
        [NotNull]
        public string ApiKey
        {
            get => this.apiKey ?? string.Empty;

            set
            {
                this.apiKey = value ?? string.Empty;
                this.SetServiceUrls();
            }
        }

        /// <summary>
        ///   Gets or sets the proxy to use.
        /// </summary>
        /// <value>The proxy.</value>
        public IWebProxy Proxy
        {
            get => this.proxy;

            set => this.proxy = value;
        }

        /// <summary>
        ///   Gets or sets the root URL to the blog.
        /// </summary>
        /// <value>The blog URL.</value>
        public Uri RootUrl
        {
            get => this.rootUrl;

            set => this.rootUrl = value;
        }

        /// <summary>
        ///   Gets or sets the timeout in milliseconds for the http request to Akismet. 
        ///   By default 5000 (5 seconds).
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout
        {
            get => this.timeout;

            set => this.timeout = value;
        }

        /// <summary>
        ///   Gets or sets the Usera Agent for the Akismet Client.  
        ///   Do not confuse this with the user agent for the comment 
        ///   being checked.
        /// </summary>
        /// <value>The API key.</value>
        [NotNull]
        public string UserAgent
        {
            get => this.userAgent ?? BuildUserAgent("Subkismet");

            set => this.userAgent = value;
        }

        /// <summary>
        /// Gets CheckUrlFormat.
        /// </summary>
        protected abstract string CheckUrlFormat { get; }

        /// <summary>
        /// Gets SubmitHamUrlFormat.
        /// </summary>
        protected abstract string SubmitHamUrlFormat { get; }

        /// <summary>
        /// Gets SubmitSpamUrlFormat.
        /// </summary>
        protected abstract string SubmitSpamUrlFormat { get; }

        /// <summary>
        /// Gets SubmitVerifyKeyFormat.
        /// </summary>
        protected abstract string SubmitVerifyKeyFormat { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Helper method for building a user agent string in the format 
        ///   preferred by Akismet.
        /// </summary>
        /// <param name="applicationName">
        /// Name of the application.
        /// </param>
        /// <returns>
        /// The build user agent.
        /// </returns>
        [NotNull]
        public static string BuildUserAgent([NotNull] string applicationName)
        {
            CodeContracts.VerifyNotNull(applicationName, "applicationName");

            return string.Format(CultureInfo.InvariantCulture, "{0}/{1} | Akismet/1.11", applicationName, version);
        }

        #endregion

        #region Implemented Interfaces

        #region ICheckForSpamClient

        /// <summary>
        /// Checks the comment and returns true if it is spam, otherwise false.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The check comment for spam.
        /// </returns>
        /// <exception cref="InvalidResponseException">Akismet returned an empty response
        /// or</exception>
        /// <exception cref="InvalidResponseException">Akismet returned an empty response</exception>
        public bool CheckCommentForSpam(IComment comment, out string result)
        {
            CodeContracts.VerifyNotNull(comment, "comment");

            result = this.SubmitComment(comment, this.submitCheckUrl);

            if (result.IsNotSet())
            {
                throw new InvalidResponseException("Akismet returned an empty response");
            }

            if (result != "true" && result != "false")
            {
                throw new InvalidResponseException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Received the response '{0}' from Akismet. Probably a bad API key.",
                        result));
            }

            return bool.Parse(result);
        }

        /// <summary>
        /// Submits a comment to Akismet that should not have been 
        ///   flagged as SPAM (a false positive).
        /// </summary>
        /// <param name="comment">
        /// </param>
        public void SubmitHam(IComment comment)
        {
            this.SubmitComment(comment, this.submitHamUrl);
        }

        /// <summary>
        /// Submits a comment to Akismet that should have been 
        ///   flagged as SPAM, but was not flagged by Akismet.
        /// </summary>
        /// <param name="comment">
        /// </param>
        public void SubmitSpam(IComment comment)
        {
            this.SubmitComment(comment, this.submitSpamUrl);
        }

        /// <summary>
        /// Verifies the API key.  You really only need to
        ///   call this once, perhaps at startup.
        /// </summary>
        /// <returns>
        /// The verify api key.
        /// </returns>
        /// <exception type="Sytsem.Web.WebException">
        /// If it cannot make a request of Akismet.
        /// </exception>
        /// <exception cref="InvalidResponseException">
        /// Akismet returned an empty response
        /// </exception>
        public bool VerifyApiKey()
        {
            var parameters =
                $"key={HttpUtility.UrlEncode(this.ApiKey)}&blog={HttpUtility.UrlEncode(this.RootUrl.ToString())}";
            var result = this.httpClient.PostRequest(
                this.verifyUrl,
                this.UserAgent,
                this.Timeout,
                parameters,
                this.proxy);

            if (result.IsNotSet())
            {
                throw new InvalidResponseException("Akismet returned an empty response");
            }

            return string.Equals("valid", result, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The set service urls.
        /// </summary>
        protected void SetServiceUrls()
        {
            this.submitHamUrl =
                new Uri(string.Format(CultureInfo.InvariantCulture, this.SubmitHamUrlFormat, this.apiKey));
            this.submitSpamUrl =
                new Uri(string.Format(CultureInfo.InvariantCulture, this.SubmitSpamUrlFormat, this.apiKey));
            this.submitCheckUrl = new Uri(string.Format(CultureInfo.InvariantCulture, this.CheckUrlFormat, this.apiKey));
            this.verifyUrl =
                new Uri(string.Format(CultureInfo.InvariantCulture, this.SubmitVerifyKeyFormat, this.apiKey));
        }

        /// <summary>
        /// The submit comment.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The submit comment.
        /// </returns>
        private string SubmitComment([NotNull] IComment comment, [NotNull] Uri url)
        {
            // Not too many concatenations.  Might not need a string builder.
            CodeContracts.VerifyNotNull(comment, "comment");
            CodeContracts.VerifyNotNull(url, "url");

            var parameters = new StringBuilder();

            parameters.AppendFormat(
                "blog={0}&user_ip={1}&user_agent={2}",
                HttpUtility.UrlEncode(this.rootUrl.ToString()),
                comment.IPAddress,
                HttpUtility.UrlEncode(comment.UserAgent));

            if (comment.Referrer.IsSet())
            {
                parameters.AppendFormat("&referer={0}", HttpUtility.UrlEncode(comment.Referrer));
            }

            if (comment.Permalink != null)
            {
                parameters.AppendFormat("&permalink={0}", HttpUtility.UrlEncode(comment.Permalink.ToString()));
            }

            if (comment.CommentType.IsSet())
            {
                parameters.AppendFormat("&comment_type={0}", HttpUtility.UrlEncode(comment.CommentType));
            }

            if (comment.Author.IsSet())
            {
                parameters.AppendFormat("&comment_author={0}", HttpUtility.UrlEncode(comment.Author));
            }

            if (comment.AuthorEmail.IsSet())
            {
                parameters.AppendFormat("&comment_author_email={0}", HttpUtility.UrlEncode(comment.AuthorEmail));
            }

            if (comment.AuthorUrl != null)
            {
                parameters.AppendFormat("&comment_author_url={0}", HttpUtility.UrlEncode(comment.AuthorUrl.ToString()));
            }

            if (comment.Content.IsSet())
            {
                parameters.AppendFormat("&comment_content={0}", HttpUtility.UrlEncode(comment.Content));
            }

            if (comment.ServerEnvironmentVariables != null)
            {
                foreach (string s in comment.ServerEnvironmentVariables)
                {
                    parameters.AppendFormat("&{0}={1}", s, HttpUtility.UrlEncode(comment.ServerEnvironmentVariables[s]));
                }
            }

            var response = this.httpClient.PostRequest(
                url,
                this.UserAgent,
                this.Timeout,
                parameters.ToString(),
                this.proxy);
            return response == null ? string.Empty : response.ToLower(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}