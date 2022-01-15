/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Core.Helpers
{
    #region Using

    using System;
    using System.IO;
    using System.Runtime.Caching;
    using System.Web;
    using System.Web.Caching;

    using YAF.Core.Context;
    using YAF.Core.Utilities.ImageUtils;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    
    #endregion

    /// <summary>
    /// The captcha helper.
    /// </summary>
    public static class CaptchaHelper
    {
        #region Public Methods

        /// <summary>
        /// Gets the Captcha Image as base64 String
        /// </summary>
        /// <returns>
        /// Returns the Captcha Image as base64 String
        /// </returns>
        public static string GetCaptcha()
        {
            using var stream = new MemoryStream();

            var captchaImage = new CaptchaImage(
                GetCaptchaText(BoardContext.Current.Get<HttpContextBase>().Session, MemoryCache.Default, true),
                250,
                50,
                "Century Schoolbook");

            captchaImage.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            return $"data:image/png;base64,{Convert.ToBase64String(stream.ToArray())}";
        }

        /// <summary>
        /// Gets the CaptchaString using the BoardSettings
        /// </summary>
        /// <returns>
        /// The get captcha string.
        /// </returns>
        public static string GetCaptchaString()
        {
            return StringExtensions.GenerateRandomString(
                BoardContext.Current.BoardSettings.CaptchaSize,
                "abcdefghijkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ123456789");
        }

        /// <summary>
        /// The get captcha text.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="forceNew">
        /// The force New.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCaptchaText(
            [NotNull] HttpSessionStateBase session,
            [NotNull] MemoryCache cache,
            bool forceNew)
        {
            CodeContracts.VerifyNotNull(session);

            var cacheName = $"Session{session.SessionID}CaptchaImageText";

            if (!forceNew && cache[cacheName] != null)
            {
                return cache[cacheName].ToString();
            }

            var text = GetCaptchaString();

            if (cache[cacheName] != null)
            {
                cache[cacheName] = text;
            }
            else
            {
                cache.Add(cacheName, text, Cache.NoAbsoluteExpiration);
            }

            return text;
        }

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="captchaText">
        /// The captcha text.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsValid([NotNull] string captchaText)
        {
            CodeContracts.VerifyNotNull(captchaText);

            var text = GetCaptchaText(BoardContext.Current.Get<HttpSessionStateBase>(), MemoryCache.Default, false);

            return string.Compare(text, captchaText, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        #endregion
    }
}