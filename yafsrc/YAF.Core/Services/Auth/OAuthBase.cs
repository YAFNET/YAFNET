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
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Oauth implementation by shannon whitley
    /// </summary>
    public class OAuthBase
    {
        #region Constants and Fields

        /// <summary>
        /// The hmacsh a 1 signature type.
        /// </summary>
        protected const string HMACSHA1SignatureType = "HMAC-SHA1";

        /// <summary>
        /// The o auth callback key.
        /// </summary>
        protected const string OAuthCallbackKey = "oauth_callback";

        /// <summary>
        /// The o auth consumer key key.
        /// </summary>
        protected const string OAuthConsumerKeyKey = "oauth_consumer_key";

        /// <summary>
        /// The o auth nonce key.
        /// </summary>
        protected const string OAuthNonceKey = "oauth_nonce";

        /// <summary>
        /// The o auth parameter prefix.
        /// </summary>
        protected const string OAuthParameterPrefix = "oauth_";

        /// <summary>
        /// The o auth signature key.
        /// </summary>
        protected const string OAuthSignatureKey = "oauth_signature";

        /// <summary>
        /// The o auth signature method key.
        /// </summary>
        protected const string OAuthSignatureMethodKey = "oauth_signature_method";

        /// <summary>
        /// The o auth timestamp key.
        /// </summary>
        protected const string OAuthTimestampKey = "oauth_timestamp";

        /// <summary>
        /// The o auth token key.
        /// </summary>
        protected const string OAuthTokenKey = "oauth_token";

        /// <summary>
        /// The o auth token secret key.
        /// </summary>
        protected const string OAuthTokenSecretKey = "oauth_token_secret";

        /// <summary>
        /// The o auth verifier key.
        /// </summary>
        protected const string OAuthVerifierKey = "oauth_verifier";

        /// <summary>
        /// The o auth version.
        /// </summary>
        protected const string OAuthVersion = "1.0";

        /// <summary>
        /// The o auth version key.
        /// </summary>
        protected const string OAuthVersionKey = "oauth_version";

        /// <summary>
        /// The plain text signature type.
        /// </summary>
        protected const string PlainTextSignatureType = "PLAINTEXT";

        /// <summary>
        /// The rsash a 1 signature type.
        /// </summary>
        protected const string RSASHA1SignatureType = "RSA-SHA1";

        /// <summary>
        /// The random.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// The unreserved chars.
        /// </summary>
        private const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        #endregion

        #region Enums

        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            /// <summary>
            /// The hmacsh a 1.
            /// </summary>
            HMACSHA1,

            /// <summary>
            /// The plaintext.
            /// </summary>
            PLAINTEXT,

            /// <summary>
            /// The rsash a 1.
            /// </summary>
            RSASHA1
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns>
        /// The generate nonce.
        /// </returns>
        public virtual string GenerateNonce()
        {
            return this.random.Next(123400, 9999999).ToString();
        }

        /// <summary>
        /// Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>
        /// <param name="url">
        /// The full url that needs to be signed including its non OAuth url parameters
        /// </param>
        /// <param name="consumerKey">
        /// The consumer key
        /// </param>
        /// <param name="consumerSecret">
        /// The consumer seceret
        /// </param>
        /// <param name="token">
        /// The token, if available. If not available pass null or an empty string
        /// </param>
        /// <param name="tokenSecret">
        /// The token secret, if available. If not available pass null or an empty string
        /// </param>
        /// <param name="callBackUrl">
        /// The call Back Url.
        /// </param>
        /// <param name="httpMethod">
        /// The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)
        /// </param>
        /// <param name="timeStamp">
        /// The time Stamp.
        /// </param>
        /// <param name="nonce">
        /// The nonce.
        /// </param>
        /// <param name="pin">
        /// The PIN.
        /// </param>
        /// <param name="normalizedUrl">
        /// The normalized Url.
        /// </param>
        /// <param name="normalizedRequestParameters">
        /// The normalized Request Parameters.
        /// </param>
        /// <returns>
        /// A base64 string of the hash value
        /// </returns>
        public string GenerateSignature(
            Uri url,
            string consumerKey,
            string consumerSecret,
            string token,
            string tokenSecret,
            string callBackUrl,
            string httpMethod,
            string timeStamp,
            string nonce,
            string pin,
            out string normalizedUrl,
            out string normalizedRequestParameters)
        {
            return this.GenerateSignature(
                url,
                consumerKey,
                consumerSecret,
                token,
                tokenSecret,
                callBackUrl,
                httpMethod,
                timeStamp,
                nonce,
                pin,
                SignatureTypes.HMACSHA1,
                out normalizedUrl,
                out normalizedRequestParameters);
        }

        /// <summary>
        /// Generates a signature using the specified signatureType
        /// </summary>
        /// <param name="url">
        /// The full url that needs to be signed including its non OAuth url parameters
        /// </param>
        /// <param name="consumerKey">
        /// The consumer key
        /// </param>
        /// <param name="consumerSecret">
        /// The consumer seceret
        /// </param>
        /// <param name="token">
        /// The token, if available. If not available pass null or an empty string
        /// </param>
        /// <param name="tokenSecret">
        /// The token secret, if available. If not available pass null or an empty string
        /// </param>
        /// <param name="callBackUrl">
        /// The call Back Url.
        /// </param>
        /// <param name="httpMethod">
        /// The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)
        /// </param>
        /// <param name="timeStamp">
        /// The time Stamp.
        /// </param>
        /// <param name="nonce">
        /// The nonce.
        /// </param>
        /// <param name="pin">
        /// The PIN.
        /// </param>
        /// <param name="signatureType">
        /// The type of signature to use
        /// </param>
        /// <param name="normalizedUrl">
        /// The normalized Url.
        /// </param>
        /// <param name="normalizedRequestParameters">
        /// The normalized Request Parameters.
        /// </param>
        /// <returns>
        /// A base64 string of the hash value
        /// </returns>
        /// <exception cref="ArgumentException">Unknown signature type</exception>
        public string GenerateSignature(
            Uri url,
            string consumerKey,
            string consumerSecret,
            string token,
            string tokenSecret,
            string callBackUrl,
            string httpMethod,
            string timeStamp,
            string nonce,
            string pin,
            SignatureTypes signatureType,
            out string normalizedUrl,
            out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode($"{consumerSecret}&{tokenSecret}");
                case SignatureTypes.HMACSHA1:
                    var signatureBase = this.GenerateSignatureBase(
                        url,
                        consumerKey,
                        token,
                        tokenSecret,
                        callBackUrl,
                        httpMethod,
                        timeStamp,
                        nonce,
                        pin,
                        HMACSHA1SignatureType,
                        out normalizedUrl,
                        out normalizedRequestParameters);

                    var hmacsha1 = new HMACSHA1
                                        {
                                            Key =
                                                Encoding.ASCII.GetBytes(
                                                    $"{this.UrlEncode(consumerSecret)}&{(tokenSecret.IsNotSet() ? string.Empty : this.UrlEncode(tokenSecret))}")
                                        };

                    return this.GenerateSignatureUsingHash(signatureBase, hmacsha1);
                default:
                    throw new ArgumentException("Unknown signature type", nameof(signatureType));
            }
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">
        /// The full url that needs to be signed including its non OAuth url parameters
        /// </param>
        /// <param name="consumerKey">
        /// The consumer key
        /// </param>
        /// <param name="token">
        /// The token, if available. If not available pass null or an empty string
        /// </param>
        /// <param name="tokenSecret">
        /// The token secret, if available. If not available pass null or an empty string
        /// </param>
        /// <param name="callBackUrl">
        /// The call Back Url.
        /// </param>
        /// <param name="httpMethod">
        /// The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)
        /// </param>
        /// <param name="timeStamp">
        /// The time Stamp.
        /// </param>
        /// <param name="nonce">
        /// The nonce.
        /// </param>
        /// <param name="pin">
        /// The PIN.
        /// </param>
        /// <param name="signatureType">
        /// The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.
        /// </param>
        /// <param name="normalizedUrl">
        /// The normalized Url.
        /// </param>
        /// <param name="normalizedRequestParameters">
        /// The normalized Request Parameters.
        /// </param>
        /// <returns>
        /// The signature base
        /// </returns>
        /// <exception cref="ArgumentNullException">ConsumerKey Not found</exception>
        public string GenerateSignatureBase(
            Uri url,
            string consumerKey,
            string token,
            string tokenSecret,
            string callBackUrl,
            string httpMethod,
            string timeStamp,
            string nonce,
            string pin,
            string signatureType,
            out string normalizedUrl,
            out string normalizedRequestParameters)
        {
            token ??= string.Empty;

            if (consumerKey.IsNotSet())
            {
                throw new ArgumentNullException(nameof(consumerKey));
            }

            if (httpMethod.IsNotSet())
            {
                throw new ArgumentNullException(nameof(httpMethod));
            }

            if (signatureType.IsNotSet())
            {
                throw new ArgumentNullException(nameof(signatureType));
            }

            var parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));

            if (callBackUrl.IsSet())
            {
                parameters.Add(new QueryParameter(OAuthCallbackKey, this.UrlEncode(callBackUrl)));
            }

            if (token.IsSet())
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, token));
            }

            // Pin Based Authentication
            if (pin.IsSet())
            {
                parameters.Add(new QueryParameter(OAuthVerifierKey, pin));
            }

            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = $"{url.Scheme}://{url.Host}";
            if (!(url.Scheme == "http" && url.Port == 80 || url.Scheme == "https" && url.Port == 443))
            {
                normalizedUrl += $":{url.Port}";
            }

            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = this.NormalizeRequestParameters(parameters);

            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", this.UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", this.UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">
        /// The signature based as produced by the GenerateSignatureBase method or by any other means
        /// </param>
        /// <param name="hash">
        /// The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method
        /// </param>
        /// <returns>
        /// A base64 string of the hash value
        /// </returns>
        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        /// <summary>
        /// Generates timestamp for the signature
        /// </summary>
        /// <returns>
        /// The generate time stamp.
        /// </returns>
        public virtual string GenerateTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalSeconds.ToType<long>().ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">
        /// The list of parameters already sorted
        /// </param>
        /// <returns>
        /// a string representing the normalized parameters
        /// </returns>
        protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < parameters.Count; i++)
            {
                var p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        ///   While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">
        /// The value to Url encode
        /// </param>
        /// <returns>
        /// Returns a Url encoded string
        /// </returns>
        protected string UrlEncode(string value)
        {
            var result = new StringBuilder();

            foreach (var symbol in value)
            {
                if (UnreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.AppendFormat("{0}{1}", '%', $"{(int)symbol:X2}");
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">
        /// The hashing algorithm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function
        /// </param>
        /// <param name="data">
        /// The data to hash
        /// </param>
        /// <returns>
        /// a Base64 string of the hash value
        /// </returns>
        /// <exception cref="ArgumentNullException">hash Algorithm</exception>
        private static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException(nameof(hashAlgorithm));
            }

            if (data.IsNotSet())
            {
                throw new ArgumentNullException(nameof(data));
            }

            var dataBuffer = Encoding.ASCII.GetBytes(data);
            var hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not beginning with "oauth_")
        /// </summary>
        /// <param name="parameters">
        /// The query string part of the Url
        /// </param>
        /// <returns>
        /// A list of QueryParameter each containing the parameter name and value
        /// </returns>
        private static List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            var result = new List<QueryParameter>();

            if (parameters.IsNotSet())
            {
                return result;
            }

            var p = parameters.Split('&');
            foreach (var s in p.Where(s => s.IsSet() && !s.StartsWith(OAuthParameterPrefix)))
            {
                if (s.IndexOf('=') > -1)
                {
                    var temp = s.Split('=');
                    result.Add(new QueryParameter(temp[0], temp[1]));
                }
                else
                {
                    result.Add(new QueryParameter(s, string.Empty));
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Provides an internal structure to sort the query parameter
        /// </summary>
        protected class QueryParameter
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="QueryParameter"/> class.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            public QueryParameter(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets Name.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets Value.
            /// </summary>
            public string Value { get; }

            #endregion
        }

        /// <summary>
        /// Comparer class used to perform the sorting of the query parameters
        /// </summary>
        protected class QueryParameterComparer : IComparer<QueryParameter>
        {
            #region Implemented Interfaces

            #region IComparer<QueryParameter>

            /// <summary>
            /// Compares the specified x.
            /// </summary>
            /// <param name="x">
            /// The x.
            /// </param>
            /// <param name="y">
            /// The y.
            /// </param>
            /// <returns>
            /// The compare.
            /// </returns>
            public int Compare(QueryParameter x, QueryParameter y)
            {
                return x.Name == y.Name
                    ? string.CompareOrdinal(x.Value, y.Value)
                    : string.CompareOrdinal(x.Name, y.Name);
            }

            #endregion

            #endregion
        }
    }
}