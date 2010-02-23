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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// Provides a image with themed src
  /// </summary>
  public class ThemeImage : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    /// The _alt.
    /// </summary>
    protected string _alt = string.Empty;

    /// <summary>
    /// The css Class.
    /// </summary>
    protected string _cssClass = string.Empty;

    /// <summary>
    /// The _enabled tag.
    /// </summary>
    protected bool _enabled = true;

    /// <summary>
    /// The _localized title page.
    /// </summary>
    protected string _localizedTitlePage = string.Empty;

    /// <summary>
    /// The _localized title tag.
    /// </summary>
    protected string _localizedTitleTag = string.Empty;

    /// <summary>
    /// The _style.
    /// </summary>
    protected string _style = string.Empty;

    /// <summary>
    /// The _theme page.
    /// </summary>
    protected string _themePage = "ICONS";

    /// <summary>
    /// The _theme tag.
    /// </summary>
    protected string _themeTag = string.Empty;

    /// <summary>
    /// The _use title for empty alt.
    /// </summary>
    protected bool _useTitleForEmptyAlt = true;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeImage"/> class.
    /// </summary>
    public ThemeImage()
      : base()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Alt.
    /// </summary>
    public string Alt
    {
      get
      {
        return this._alt;
      }

      set
      {
        this._alt = value;
      }
    }

    /// <summary>
    /// Gets or sets Style.
    /// </summary>
    public string CssClass
    {
      get
      {
        return this._cssClass;
      }

      set
      {
        this._cssClass = value;
      }
    }

    /// <summary>
    /// Gets or sets whether the control is Active .
    /// </summary>
    public bool Enabled
    {
      get
      {
        return this._enabled;
      }

      set
      {
        this._enabled = value;
      }
    }

    /// <summary>
    /// Gets or sets LocalizedTitlePage.
    /// </summary>
    public string LocalizedTitlePage
    {
      get
      {
        return this._localizedTitlePage;
      }

      set
      {
        this._localizedTitlePage = value;
      }
    }

    /// <summary>
    /// Gets or sets LocalizedTitleTag.
    /// </summary>
    public string LocalizedTitleTag
    {
      get
      {
        return this._localizedTitleTag;
      }

      set
      {
        this._localizedTitleTag = value;
      }
    }

    /// <summary>
    /// Gets or sets Style.
    /// </summary>
    public string Style
    {
      get
      {
        return this._style;
      }

      set
      {
        this._style = value;
      }
    }

    /// <summary>
    /// Set/Get the ThemePage -- Defaults to "ICONS"
    /// </summary>
    public string ThemePage
    {
      get
      {
        return this._themePage;
      }

      set
      {
        this._themePage = value;
      }
    }

    /// <summary>
    /// Set/Get the actual theme item
    /// </summary>
    public string ThemeTag
    {
      get
      {
        return this._themeTag;
      }

      set
      {
        this._themeTag = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether UseTitleForEmptyAlt.
    /// </summary>
    public bool UseTitleForEmptyAlt
    {
      get
      {
        return this._useTitleForEmptyAlt;
      }

      set
      {
        this._useTitleForEmptyAlt = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get current theme item.
    /// </summary>
    /// <returns>
    /// The get current theme item.
    /// </returns>
    protected string GetCurrentThemeItem()
    {
      if (!String.IsNullOrEmpty(this._themePage) && !String.IsNullOrEmpty(this._themeTag))
      {
        return this.PageContext.Theme.GetItem(this.ThemePage, this.ThemeTag, null);
      }

      return null;
    }

    /// <summary>
    /// The get current title item.
    /// </summary>
    /// <returns>
    /// The get current title item.
    /// </returns>
    protected string GetCurrentTitleItem()
    {
      if (!String.IsNullOrEmpty(this._localizedTitlePage) && !String.IsNullOrEmpty(this._localizedTitleTag))
      {
        return this.PageContext.Localization.GetText(this._localizedTitlePage, this._localizedTitleTag);
      }
      else if (!String.IsNullOrEmpty(this._localizedTitleTag))
      {
        return this.PageContext.Localization.GetText(this._localizedTitleTag);
      }

      return null;
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void Render(HtmlTextWriter output)
    {
      // vzrus: Don't render control if not enabled
      if (!this.Enabled)
      {
        return;
      }

      string src = this.GetCurrentThemeItem();
      string title = this.GetCurrentTitleItem();

      // might not be needed...
      if (String.IsNullOrEmpty(src))
      {
        return;
      }

      if (this.UseTitleForEmptyAlt && String.IsNullOrEmpty(this.Alt) && !String.IsNullOrEmpty(title))
      {
        this.Alt = title;
      }

      output.BeginRender();
      output.WriteBeginTag("img");
      output.WriteAttribute("id", this.ClientID);

      // this will output the src and alt attributes
      output.WriteAttribute("src", src);
      output.WriteAttribute("alt", this.Alt);

      if (!String.IsNullOrEmpty(this.Style))
      {
        output.WriteAttribute("style", this.Style);
      }

      if (!String.IsNullOrEmpty(this.CssClass))
      {
        output.WriteAttribute("class", this.CssClass);
      }

      if (!String.IsNullOrEmpty(title))
      {
        output.WriteAttribute("title", title);
      }

      // self closing end tag "/>"
      output.Write(HtmlTextWriter.SelfClosingTagEnd);

      output.EndRender();
    }

    #endregion
  }
}