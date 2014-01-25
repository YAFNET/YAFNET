/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Web;

    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Constants;
    using YAF.Classes.Data;
    using YAF.Utils;
    using YAF.Types;

    #endregion

    /// <summary>
    /// The recaptcha validator.
    /// </summary>
    public class RecaptchaValidator
    {
        #region Constants and Fields

        /// <summary>
        ///   The verify url.
        /// </summary>
        private const string VerifyUrl = "http://api-verify.recaptcha.net/verify";

        /// <summary>
        ///   The challenge.
        /// </summary>
        private string challenge;

        /// <summary>
        ///   The remote ip.
        /// </summary>
        private string remoteIp;

        /// <summary>
        ///   The response.
        /// </summary>
        private string response;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Challenge.
        /// </summary>
        public string Challenge
        {
            get
            {
                return this.challenge;
            }

            set
            {
                this.challenge = value;
            }
        }

        /// <summary>
        ///   Gets or sets PrivateKey.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        ///   Gets or sets RemoteIP.
        /// </summary>
        /// <exception cref = "ArgumentException">
        /// </exception>
        public string RemoteIP
        {
            get
            {
                return this.remoteIp;
            }

            set
            {
                IPAddress address = IPAddress.Parse(value);
                if ((address == null) ||
                    ((address.AddressFamily != AddressFamily.InterNetwork) &&
                     (address.AddressFamily != AddressFamily.InterNetworkV6)))
                {
                    throw new ArgumentException("Expecting an IP address, got " + address);
                }

                this.remoteIp = address.ToString();
            }
        }

        /// <summary>
        ///   Gets or sets Response.
        /// </summary>
        public string Response
        {
            get
            {
                return this.response;
            }

            set
            {
                this.response = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The validate.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="InvalidProgramException">
        /// </exception>
        public RecaptchaResponse Validate()
        {
            string[] strArray;
            this.CheckNotNull(this.PrivateKey, "PrivateKey");
            this.CheckNotNull(this.RemoteIP, "RemoteIp");
            this.CheckNotNull(this.Challenge, "Challenge");
            this.CheckNotNull(this.Response, "Response");
            if ((this.challenge == string.Empty) || (this.response == string.Empty))
            {
                return RecaptchaResponse.InvalidSolution;
            }

            var request = (HttpWebRequest)WebRequest.Create("http://api-verify.recaptcha.net/verify");
            request.ProtocolVersion = HttpVersion.Version10;
            request.Timeout = 0x7530;
            request.Method = "POST";
            request.UserAgent = "reCAPTCHA/ASP.NET";
            request.ContentType = "application/x-www-form-urlencoded";
            string s =
              "privatekey={0}&remoteip={1}&challenge={2}&response={3}".FormatWith(
                new object[]
            {
              HttpUtility.UrlEncode(this.PrivateKey), HttpUtility.UrlEncode(this.RemoteIP), 
              HttpUtility.UrlEncode(this.Challenge), HttpUtility.UrlEncode(this.Response)
            });
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (TextReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        strArray = reader.ReadToEnd().Split(new char[0]);
                    }
                }
            }
            catch (WebException exception)
            {
                YafContext.Current.Get<ILogger>().Log(YafContext.Current.PageUserID, this.GetType().Name, exception.ToString(), EventLogTypes.Error);

                return RecaptchaResponse.RecaptchaNotReachable;
            }

            switch (strArray[0])
            {
                case "true":
                    return RecaptchaResponse.Valid;

                case "false":
                    return new RecaptchaResponse(false, strArray[1]);
            }

            throw new InvalidProgramException("Unknown status response.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The check not null.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        private void CheckNotNull([NotNull] object obj, [NotNull] string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        #endregion
    }
}