/*
 * (Copyright (c) 2011, Shannon Whitley <swhitley@whitleymedia.com> http://voiceoftech.com/
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
    #region

    using System;
    using System.Web;

    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Twitter OAUTH implementation by shannon whitley
    /// </summary>
    public class OAuthTwitter : OAuthBase
    {
        #region Constants and Fields

        /// <summary>
        /// The access token URL.
        /// </summary>
        public const string ACCESSTOKEN = "https://api.twitter.com/oauth/access_token";

        /// <summary>
        /// The Authorize URL
        /// </summary>
        public const string AUTHORIZE = "https://api.twitter.com/oauth/authorize";

        /// <summary>
        /// The Request token URL
        /// </summary>
        public const string REQUESTTOKEN = "https://api.twitter.com/oauth/request_token";

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the call back URL.
        /// </summary>
        /// <value>
        ///   The call back URL.
        /// </value>
        public string CallBackUrl { get; set; } = "oob";

        /// <summary>
        ///   Gets or sets the Consumer Key
        /// </summary>
        public string ConsumerKey { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the Consumer Secret Key
        /// </summary>
        public string ConsumerSecret { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets OAUTH Token.
        /// </summary>
        public string OAuthToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets PIN.
        /// </summary>
        public string PIN { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Token.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets TokenSecret.
        /// </summary>
        public string TokenSecret { get; set; } = string.Empty;

        #endregion

        #region Public Methods

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">
        /// The OAUTH token is supplied by Twitter's authorization page following the callback.
        /// </param>
        /// <param name="pin">
        /// The PIN.
        /// </param>
        public void AccessTokenGet(string authToken, string pin)
        {
            this.Token = authToken;
            this.PIN = pin;

            var response = this.OAuthWebRequest(AuthUtilities.Method.GET, ACCESSTOKEN, string.Empty);

            if (response.Length <= 0)
            {
                return;
            }

            // Store the Token and Token Secret
            var qs = HttpUtility.ParseQueryString(response);
            if (qs["oauth_token"] != null)
            {
                this.Token = qs["oauth_token"];
            }

            if (qs["oauth_token_secret"] != null)
            {
                this.TokenSecret = qs["oauth_token_secret"];
            }
        }

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <returns>
        /// The url with a valid request token, or a null string.
        /// </returns>
        public string AuthorizationLinkGet()
        {
            // First let's get a REQUEST token.
            var response = this.OAuthWebRequest(AuthUtilities.Method.GET, REQUESTTOKEN, string.Empty);

            if (response.Length <= 0)
            {
                return null;
            }

            // response contains token and token secret.  We only need the token.
            var qs = HttpUtility.ParseQueryString(response);
            if (qs["oauth_token"] == null)
            {
                return null;
            }

            this.OAuthToken = qs["oauth_token"]; // tuck this away for later

            return $"{AUTHORIZE}?oauth_token={qs["oauth_token"]}";
        }

        /// <summary>
        /// Resets the state of the oAUTH Twitter object.
        /// </summary>
        public void Reset()
        {
            this.ConsumerKey =
                this.ConsumerSecret = this.OAuthToken = this.Token = this.TokenSecret = this.PIN = string.Empty;
        }

        /// <summary>
        /// Submit a web request using oAUTH.
        /// </summary>
        /// <param name="method">
        /// GET or POST
        /// </param>
        /// <param name="url">
        /// The full url, including the query string.
        /// </param>
        /// <param name="postData">
        /// Data to post (query string format)
        /// </param>
        /// <returns>
        /// The web server response.
        /// </returns>
        public string OAuthWebRequest(AuthUtilities.Method method, string url, string postData)
        {
            switch (method)
            {
                // Setup postData for signing.
                // Add the postData to the querystring.
                case AuthUtilities.Method.POST:
                {
                    if (postData.Length > 0)
                    {
                        // Decode the parameters and re-encode using the oAuth UrlEncode method.
                        var qs = HttpUtility.ParseQueryString(postData);
                        postData = string.Empty;

                        qs.AllKeys.ForEach(
                            key =>
                            {
                                if (postData.Length > 0)
                                {
                                    postData += "&";
                                }

                                qs[key] = HttpUtility.UrlDecode(qs[key]);
                                qs[key] = this.UrlEncode(qs[key]);
                                postData += $"{key}={qs[key]}";
                            });
                    
                        if (url.IndexOf("?", StringComparison.Ordinal) > 0)
                        {
                            url += "&";
                        }
                        else
                        {
                            url += "?";
                        }

                        url += postData;
                    }

                    break;
                }
                case AuthUtilities.Method.GET when postData.IsSet():
                    url += $"?{postData}";
                    break;
            }

            var uri = new Uri(url);

            var nonce = this.GenerateNonce();
            var timeStamp = this.GenerateTimeStamp();

            // Generate Signature
            var sig = this.GenerateSignature(
                uri,
                this.ConsumerKey,
                this.ConsumerSecret,
                this.Token,
                this.TokenSecret,
                this.CallBackUrl,
                method.ToString(),
                timeStamp,
                nonce,
                this.PIN,
                out var outUrl,
                out var querystring);

            querystring += $"&oauth_signature={HttpUtility.UrlEncode(sig)}";

            // Convert the querystring to postData
            if (method == AuthUtilities.Method.POST)
            {
                postData = querystring;
                querystring = string.Empty;
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            var ret = AuthUtilities.WebRequest(method, $"{outUrl}{querystring}", postData);

            return ret;
        }

        #endregion
    }
}