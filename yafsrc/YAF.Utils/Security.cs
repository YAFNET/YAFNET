/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Utils
{
  using System;
  using System.Web;

  /// <summary>
  /// The security.
  /// </summary>
  public static class Security
  {
    /// <summary>
    /// Function that verifies a string is an integer value or it redirects to invalid "info" page.
    /// Used as a security feature against invalid values submitted to the page.
    /// </summary>
    /// <param name="longValue">
    /// The string value to test
    /// </param>
    /// <returns>
    /// The converted long value
    /// </returns>
    public static long StringToLongOrRedirect(string longValue)
    {
      long value;

      if (!long.TryParse(longValue, out value))
      {
        // it's an invalid request. Redirect to the info page on invalid requests.
        YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
      }

      return value;
    }

    /// <summary>
    /// The create password.
    /// </summary>
    /// <param name="length">
    /// The length.
    /// </param>
    /// <returns>
    /// The create password.
    /// </returns>
    public static string CreatePassword(int length)
    {
      string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#%&()@${[]}";
      string res = string.Empty;
      var rnd = new Random();
      while (0 < length--)
      {
        res += valid[rnd.Next(valid.Length)];
      }

      return res;
    }

    /// <summary>
    /// This method validates request whether it comes from same server in case it's HTTP POST.
    /// </summary>
    /// <param name="request">
    /// Request to validate.
    /// </param>
    public static void CheckRequestValidity(HttpRequest request)
    {
      // ip with 
      // deny access if POST request comes from other server
      if (request.HttpMethod == "POST" && request.UrlReferrer != null && request.Url.Host != null && request.UrlReferrer.Host != request.Url.Host)
      {
        YafBuildLink.AccessDenied();
      }
    }
  }
}