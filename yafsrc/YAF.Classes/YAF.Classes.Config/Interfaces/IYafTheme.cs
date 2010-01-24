/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Classes.Interfaces
{
  /// <summary>
  /// The yaf theme interface
  /// </summary>
  public interface IYafTheme
  {
    /// <summary>
    /// Gets or sets the current Theme File
    /// </summary>
    string ThemeFile
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether LogMissingThemeItem.
    /// </summary>
    bool LogMissingThemeItem
    {
      get;
      set;
    }

    /// <summary>
    /// Gets ThemeDir.
    /// </summary>
    string ThemeDir
    {
      get;
    }

    /// <summary>
    /// The get item.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get item.
    /// </returns>
    string GetItem(string page, string tag);

    /// <summary>
    /// The get item.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <returns>
    /// The get item.
    /// </returns>
    string GetItem(string page, string tag, string defaultValue);

    /// <summary>
    /// Gets full path to the given theme file.
    /// </summary>
    /// <param name="filename">
    /// Short name of theme file.
    /// </param>
    /// <returns>
    /// The build theme path.
    /// </returns>
    string BuildThemePath(string filename);
  }
}