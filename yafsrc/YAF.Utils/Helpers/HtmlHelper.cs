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
namespace YAF.Utils.Helpers
{
  using System.Text.RegularExpressions;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// The html helper.
  /// </summary>
  public static class HtmlHelper
  {
    /// <summary>
    /// The strip html.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The strip html.
    /// </returns>
    public static string StripHtml(string text)
    {
      return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
    }

    /// <summary>
    /// The clean html string.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The clean html string.
    /// </returns>
    public static string CleanHtmlString(string text)
    {
      text = text.Replace("<br />", " ");
      text = text.Replace("&quot;", "\"");
      text = text.Replace("&nbsp;", " ");

      return text;
    }

    /// <summary>
    /// Validates an html tag against the allowedTags. Also check that
    /// it doesn't have any "extra" features such as javascript in it.
    /// </summary>
    /// <param name="tag">
    /// </param>
    /// <param name="allowedTags">
    /// </param>
    /// <returns>
    /// The is valid tag.
    /// </returns>
    public static bool IsValidTag(string tag, IEnumerable<string> allowedTags)
    {
      if (tag.IndexOf("javascript") >= 0)
      {
        return false;
      }

      if (tag.IndexOf("vbscript") >= 0)
      {
        return false;
      }

      if (tag.IndexOf("onclick") >= 0)
      {
        return false;
      }

      var endchars = new[]
        {
          ' ', '>', '/', '\t'
        };

      int pos = tag.IndexOfAny(endchars, 1);
      if (pos > 0)
      {
        tag = tag.Substring(0, pos);
      }

      if (tag[0] == '/')
      {
        tag = tag.Substring(1);
      }

      // check if it's a valid tag
      return allowedTags.Any(allowedTag => tag == allowedTag);
    }
  }
}