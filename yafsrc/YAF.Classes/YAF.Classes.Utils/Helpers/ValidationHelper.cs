/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System.Text.RegularExpressions;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// The validation helper.
  /// </summary>
  public static class ValidationHelper
  {
    /// <summary>
    /// The is valid email.
    /// </summary>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <returns>
    /// The is valid email.
    /// </returns>
    public static bool IsValidEmail(string email)
    {
      return Regex.IsMatch(email, @"^([0-9a-z]+[-._+&])*[0-9a-z]+@([-0-9a-z]+[.])+[a-z]{2,6}$", RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// The is valid url.
    /// </summary>
    /// <param name="url">
    /// The url.
    /// </param>
    /// <returns>
    /// The is valid url.
    /// </returns>
    public static bool IsValidURL(string url)
    {
      return Regex.IsMatch(url, @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&%\$#\=~])*[^\.\,\)\(\s]$");
    }

    /// <summary>
    /// The is valid int.
    /// </summary>
    /// <param name="intstr">
    /// The intstr.
    /// </param>
    /// <returns>
    /// The is valid int.
    /// </returns>
    public static bool IsValidInt(string intstr)
    {
      int value;
      return int.TryParse(intstr, out value);
    }
  }
}