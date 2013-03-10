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

namespace YAF.Core.Services.Twitter
{
    #region

    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Web;

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

        /// <summary>
        /// The _call back url.
        /// </summary>
        private string _callBackUrl = "oob";

        /// <summary>
        /// The _consumer key.
        /// </summary>
        private string _consumerKey = string.Empty;

        /// <summary>
        /// The _consumer secret.
        /// </summary>
        private string _consumerSecret = string.Empty;

        /// <summary>
        /// The _oauth token.
        /// </summary>
        private string _oauthToken = string.Empty;

        /// <summary>
        /// The _pin.
        /// </summary>
        private string _pin = string.Empty;

        /// <summary>
        /// The _token.
        /// </summary>
        private string _token = string.Empty;

        /// <summary>
        /// The _token secret.
        /// </summary>
        private string _tokenSecret = string.Empty;

        #endregion

        #region Enums

        /// <summary>
        /// The Http method.
        /// </summary>
        public enum Method
        {
            /// <summary>
            /// The get.
            /// </summary>
            GET, 

            /// <summary>
            /// The post.
            /// </summary>
            POST
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the call back URL.
        /// </summary>
        /// <value>
        ///   The call back URL.
        /// </value>
        public string CallBackUrl
        {
            get
            {
                return this._callBackUrl;
            }

            set
            {
                this._callBackUrl = value;
            }
        }

        /// <summary>
        ///   Gets or sets the Consumer Key
        /// </summary>
        public string ConsumerKey
        {
            get
            {
                return this._consumerKey;
            }

            set
            {
                this._consumerKey = value;
            }
        }

        /// <summary>
        ///   Gets or sets the Consumer Secret Key
        /// </summary>
        public string ConsumerSecret
        {
            get
            {
                return this._consumerSecret;
            }

            set
            {
                this._consumerSecret = value;
            }
        }

        /// <summary>
        /// Gets or sets OAuthToken.
        /// </summary>
        public string OAuthToken
        {
            get
            {
                return this._oauthToken;
            }

            set
            {
                this._oauthToken = value;
            }
        }

        /// <summary>
        /// Gets or sets PIN.
        /// </summary>
        public string PIN
        {
            get
            {
                return this._pin;
            }

            set
            {
                this._pin = value;
            }
        }

        /// <summary>
        /// Gets or sets Token.
        /// </summary>
        public string Token
        {
            get
            {
                return this._token;
            }

            set
            {
                this._token = value;
            }
        }

        /// <summary>
        /// Gets or sets TokenSecret.
        /// </summary>
        public string TokenSecret
        {
            get
            {
                return this._tokenSecret;
            }

            set
            {
                this._tokenSecret = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">
        /// The oauth_token is supplied by Twitter's authorization page following the callback.
        /// </param>
        /// <param name="pin">
        /// The PIN.
        /// </param>
        public void AccessTokenGet(string authToken, string pin)
        {
            this.Token = authToken;
            this._pin = pin;

            string response = this.OAuthWebRequest(Method.GET, ACCESSTOKEN, string.Empty);

            if (response.Length <= 0)
            {
                return;
            }

            // Store the Token and Token Secret
            NameValueCollection qs = HttpUtility.ParseQueryString(response);
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
            string ret = null;

            // First let's get a REQUEST token.
            string response = this.OAuthWebRequest(Method.GET, REQUESTTOKEN, string.Empty);
            if (response.Length > 0)
            {
                // response contains token and token secret.  We only need the token.
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    this.OAuthToken = qs["oauth_token"]; // tuck this away for later
                    ret = AUTHORIZE + "?oauth_token=" + qs["oauth_token"]; // +"&oauth_callback=oob";
                }
            }

            return ret;
        }

        /// <summary>
        /// Resets the state of the oAuthTwitter object.
        /// </summary>
        public void Reset()
        {
            this.ConsumerKey =
                this.ConsumerSecret = this.OAuthToken = this.Token = this.TokenSecret = this.PIN = string.Empty;
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
        /// Data to post in querystring format
        /// </param>
        /// <returns>
        /// The web server response.
        /// </returns>
        public string WebRequest(Method method, string url, string postData)
        {
            HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;

            // webRequest.UserAgent  = "Identify your application please.";
            // webRequest.Timeout = 20000;

            if (method == Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";
                
                // POST the data.
                StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                finally
                {
                    requestWriter.Close();
                }
            }

            string responseData = this.WebResponseGet(webRequest);

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
        public string WebResponseGet(HttpWebRequest webRequest)
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

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">
        /// GET or POST
        /// </param>
        /// <param name="url">
        /// The full url, including the querystring.
        /// </param>
        /// <param name="postData">
        /// Data to post (querystring format)
        /// </param>
        /// <returns>
        /// The web server response.
        /// </returns>
        public string OAuthWebRequest(Method method, string url, string postData)
        {
            string outUrl;
            string querystring;

            // Setup postData for signing.
            // Add the postData to the querystring.
            if (method == Method.POST)
            {
                if (postData.Length > 0)
                {
                    // Decode the parameters and re-encode using the oAuth UrlEncode method.
                    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                    postData = string.Empty;
                    foreach (string key in qs.AllKeys)
                    {
                        if (postData.Length > 0)
                        {
                            postData += "&";
                        }

                        qs[key] = HttpUtility.UrlDecode(qs[key]);
                        qs[key] = this.UrlEncode(qs[key]);
                        postData += key + "=" + qs[key];
                    }

                    if (url.IndexOf("?") > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }

                    url += postData;
                }
            }
            else if (method == Method.GET && !string.IsNullOrEmpty(postData))
            {
                url += "?" + postData;
            }

            Uri uri = new Uri(url);

            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            // Generate Signature
            string sig = this.GenerateSignature(
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
                out outUrl, 
                out querystring);

            querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

            // Convert the querystring to postData
            if (method == Method.POST)
            {
                postData = querystring;
                querystring = string.Empty;
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            string ret = this.WebRequest(method, outUrl + querystring, postData);

            return ret;
        }

        #endregion
    }
}