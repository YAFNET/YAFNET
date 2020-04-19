/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Web.ReCAPTCHA
{
    #region Using

    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Web;

    using ServiceStack;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The reCAPTCHA validator.
    /// </summary>
    public class RecaptchaValidator
    {
        #region Constants and Fields

        /// <summary>
        ///   The verify url.
        /// </summary>
        private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        /// <summary>
        ///   The remote IP.
        /// </summary>
        private string remoteIp;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets SecretKey.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        ///   Gets or sets RemoteIP.
        /// </summary>
        public string RemoteIP
        {
            get => this.remoteIp;

            set
            {
                var address = IPAddress.Parse(value);
                if (address == null ||
                    address.AddressFamily != AddressFamily.InterNetwork &&
                    address.AddressFamily != AddressFamily.InterNetworkV6)
                {
                    throw new ArgumentException($"Expecting an IP address, got {address}");
                }

                this.remoteIp = address.ToString();
            }
        }

        /// <summary>
        ///   Gets or sets Response.
        /// </summary>
        public string Response { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates the reCAPTCHA Response
        /// </summary>
        /// <returns>Returns if the reCaptcha is valid or not</returns>
        /// <exception cref="System.InvalidProgramException">Unknown status response.</exception>
        public RecaptchaResponse Validate()
        {
            RecaptchaJson responseJson;
            CheckNotNull(this.SecretKey, "SecretKey");
            CheckNotNull(this.RemoteIP, "RemoteIp");
            CheckNotNull(this.Response, "Response");
            
            if (this.Response.IsNotSet())
            {
                return RecaptchaResponse.InvalidSolution;
            }

            var request = (HttpWebRequest)WebRequest.Create(VerifyUrl);

            request.ProtocolVersion = HttpVersion.Version10;
            request.Timeout = 0x7530;
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.16) Gecko/20110319 Firefox/3.6.16";
            request.ContentType = "application/x-www-form-urlencoded";

            var s =
                $"secret={HttpUtility.UrlEncode(this.SecretKey)}&remoteip={HttpUtility.UrlEncode(this.RemoteIP)}&response={HttpUtility.UrlEncode(this.Response)}";
            
            var bytes = Encoding.ASCII.GetBytes(s);
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (var webResponse = request.GetResponse())
                {
                    using (TextReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        responseJson = reader.ReadToEnd().FromJson<RecaptchaJson>();
                    }
                }
            }
            catch (WebException exception)
            {
                BoardContext.Current.Get<ILogger>().Log(BoardContext.Current.PageUserID, this.GetType().Name, exception.ToString());

                return RecaptchaResponse.RecaptchaNotReachable;
            }

            return responseJson.Success
                       ? RecaptchaResponse.Valid
                       : new RecaptchaResponse(false, responseJson.ErrorCodes);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The check not null.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        private static void CheckNotNull([NotNull] object obj, [NotNull] string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        #endregion
    }
}