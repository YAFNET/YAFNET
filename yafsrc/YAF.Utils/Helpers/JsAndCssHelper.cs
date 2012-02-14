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
    using System;

    using YAF.Utils.Helpers.MinifyUtils;

    /// <summary>
  /// The js and css helper.
  /// </summary>
  public static class JsAndCssHelper
  {
    /// <summary>
    /// Compresses JavaScript
    /// </summary>
    /// <param name="javaScript">
    /// The Uncompressed Input JS
    /// </param>
    /// <returns>
    /// The compressed java script.
    /// </returns>
    public static string CompressJavaScript(string javaScript)
    {
        try
        {
           return JSMinify.Minify(javaScript);
        }
        catch (Exception)
        {
            return javaScript;
        }
    }

    /// <summary>
    /// Compresses CSS
    /// </summary>
    /// <param name="css">
    /// The Uncompressd Input CSS
    /// </param>
    /// <returns>
    /// The compressed css output.
    /// </returns>
    public static string CompressCss(string css)
    {
        try
        {
            return JSMinify.Minify(css);
        }
        catch (Exception)
        {
            return css;
        }
    }
  }
}