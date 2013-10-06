/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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