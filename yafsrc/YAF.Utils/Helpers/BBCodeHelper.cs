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


namespace YAF.Utils.Helpers
{
  using System.Text.RegularExpressions;

  /// <summary>
  /// The bb code helper.
  /// </summary>
  public static class BBCodeHelper
  {
    /// <summary>
    /// The strip bb code.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The strip bb code.
    /// </returns>
    public static string StripBBCode(string text)
    {
      return Regex.Replace(text, @"\[(.|\n)*?\]", string.Empty);
    }

    /// <summary>
    /// Strip Quote BB Code Quotes including the quoted text
    /// </summary>
    /// <param name="text">Text to check
    /// </param>
    /// <returns>The Cleaned Text
    /// </returns>
    public static string StripBBCodeQuotes(string text)
    {
        return Regex.Replace(text, @"\[quote\b[^>]*](.|\n)*?\[/quote\]", string.Empty, RegexOptions.Multiline);
    }
  }
}