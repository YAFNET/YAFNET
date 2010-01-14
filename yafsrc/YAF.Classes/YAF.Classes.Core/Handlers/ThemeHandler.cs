/* Yet Another Forum.NET
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
using System;

namespace YAF.Classes.Core
{
  /// <summary>
  /// The theme handler.
  /// </summary>
  public class ThemeHandler
  {
    /// <summary>
    /// The _init theme.
    /// </summary>
    private bool _initTheme = false;

    /// <summary>
    /// The _theme.
    /// </summary>
    private YafTheme _theme = null;

    /// <summary>
    /// Gets or sets Theme.
    /// </summary>
    public YafTheme Theme
    {
      get
      {
        if (!this._initTheme)
        {
          InitTheme();
        }

        return this._theme;
      }

      set
      {
        this._theme = value;
        this._initTheme = value != null;
      }
    }

    /// <summary>
    /// The before init.
    /// </summary>
    public event EventHandler<EventArgs> BeforeInit;

    /// <summary>
    /// The after init.
    /// </summary>
    public event EventHandler<EventArgs> AfterInit;

    /// <summary>
    /// Sets the theme class up for usage
    /// </summary>
    private void InitTheme()
    {
      if (!this._initTheme)
      {
        if (BeforeInit != null)
        {
          BeforeInit(this, new EventArgs());
        }

        string themeFile = null;

        if (YafContext.Current.Page != null && YafContext.Current.Page["ThemeFile"] != DBNull.Value && YafContext.Current.BoardSettings.AllowUserTheme)
        {
          // use user-selected theme
          themeFile = YafContext.Current.Page["ThemeFile"].ToString();
        }
        else if (YafContext.Current.Page != null && YafContext.Current.Page["ForumTheme"] != DBNull.Value)
        {
          themeFile = YafContext.Current.Page["ForumTheme"].ToString();
        }
        else
        {
          themeFile = YafContext.Current.BoardSettings.Theme;
        }

        if (!YafTheme.IsValidTheme(themeFile))
        {
            themeFile = StaticDataHelper.Themes().Rows[0][1].ToString();           
        }

        // create the theme class
        Theme = new YafTheme(themeFile);

        // make sure it's valid again...
        if (!YafTheme.IsValidTheme(Theme.ThemeFile))
        {
          // can't load a theme... throw an exception.
          throw new Exception(String.Format("Unable to find a theme to load. Last attempted to load \"{0}\" but failed.", themeFile));
        }

        if (AfterInit != null)
        {
          AfterInit(this, new EventArgs());
        }
      }
    }
  }
}