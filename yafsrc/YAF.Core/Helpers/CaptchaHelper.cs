/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
namespace YAF.Core.Helpers;

using System;

using YAF.Core.Utilities.Captcha;
using YAF.Types.Attributes;

/// <summary>
/// The captcha helper.
/// </summary>
public static class CaptchaHelper
{
    /// <summary>
    /// Gets the Captcha Image as base64 String
    /// </summary>
    /// <returns>
    /// Returns the Captcha Image as base64 String
    /// </returns>
    public static string GetCaptcha()
    {
        var imgText = BoardContext.Current.Get<ISixLaborsCaptchaModule>().Generate(GetCaptchaText(true));

        return $"data:image/png;base64,{Convert.ToBase64String(imgText)}";
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
            BoardContext.Current.BoardSettings.CaptchaSize);
    }

    /// <summary>
    /// The get captcha text.
    /// </summary>
    /// <param name="forceNew">
    /// The force New.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string GetCaptchaText(
        bool forceNew)
    {
        var cache = BoardContext.Current.Get<IHttpContextAccessor>().HttpContext.Session;

        var cacheName = $"Session{cache.Id}CaptchaImageText";

        if (!forceNew && cache.GetString(cacheName).IsSet())
        {
            return cache.GetString(cacheName);
        }

        var text = GetCaptchaString();

        cache.SetString(cacheName, text);

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

        var text = GetCaptchaText(false);

        return string.Compare(text, captchaText, StringComparison.InvariantCultureIgnoreCase) == 0;
    }
}