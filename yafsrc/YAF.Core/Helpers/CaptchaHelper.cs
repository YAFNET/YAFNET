/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Web;
  using System.Web.Caching;

  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The captcha helper.
  /// </summary>
  public static class CaptchaHelper
  {
    #region Public Methods

    /// <summary>
    /// Gets the CaptchaString using the BoardSettings
    /// </summary>
    /// <returns>
    /// The get captcha string.
    /// </returns>
    public static string GetCaptchaString()
    {
      return StringExtensions.GenerateRandomString(
        YafContext.Current.BoardSettings.CaptchaSize, "abcdefghijkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ123456789");
    }

    /// <summary>
    /// The get captcha text.
    /// </summary>
    /// <param name="session">
    /// </param>
    /// <param name="cache">
    /// The cache.
    /// </param>
    /// <param name="forceNew">
    /// The force New.
    /// </param>
    /// <returns>
    /// The get captcha text.
    /// </returns>
    public static string GetCaptchaText([NotNull] HttpSessionStateBase session, [NotNull] System.Web.Caching.Cache cache, bool forceNew)
    {
      CodeContracts.VerifyNotNull(session, "session");

      var cacheName = "Session{0}CaptchaImageText".FormatWith(session.SessionID);

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
        cache.Add(
          cacheName, text, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), CacheItemPriority.Low, null);
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
    /// The is valid.
    /// </returns>
    public static bool IsValid([NotNull] string captchaText)
    {
      CodeContracts.VerifyNotNull(captchaText, "captchaText");

      var text = GetCaptchaText(YafContext.Current.Get<HttpSessionStateBase>(), HttpRuntime.Cache, false);

      return String.Compare(text, captchaText, StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    #endregion
  }
}