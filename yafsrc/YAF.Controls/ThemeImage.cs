/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

    using System.Web;
    using System.Web.UI;

  using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces; using YAF.Types.Constants;
    using YAF.Types.Interfaces.Extensions;
    using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Provides a image with themed src
  /// </summary>
  public class ThemeImage : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _alt.
    /// </summary>
    protected string _alt = string.Empty;

    /// <summary>
    ///   The css Class.
    /// </summary>
    protected string _cssClass = string.Empty;

    /// <summary>
    ///   The _enabled tag.
    /// </summary>
    protected bool _enabled = true;

    /// <summary>
    ///   The _localized title ready.
    /// </summary>
    protected string _localizedTitle = string.Empty;

    /// <summary>
    ///   The _localized title page.
    /// </summary>
    protected string _localizedTitlePage = string.Empty;

    /// <summary>
    ///   The _localized title tag.
    /// </summary>
    protected string _localizedTitleTag = string.Empty;

    /// <summary>
    ///   The _style.
    /// </summary>
    protected string _style = string.Empty;

    /// <summary>
    ///   The _theme page.
    /// </summary>
    protected string _themePage = "ICONS";

    /// <summary>
    ///   The _theme tag.
    /// </summary>
    protected string _themeTag = string.Empty;

    /// <summary>
    ///   The _use title for empty alt.
    /// </summary>
    protected bool _useTitleForEmptyAlt = true;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets Alt.
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
    ///   Gets or sets Style.
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
    ///   Gets or sets a value indicating whether the control is Active .
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
    ///   Gets or sets LocalizedTitle.
    /// </summary>
    public string LocalizedTitle
    {
      get
      {
        return this._localizedTitle;
      }

      set
      {
        this._localizedTitle = value;
      }
    }

    /// <summary>
    ///   Gets or sets LocalizedTitlePage.
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
    ///   Gets or sets LocalizedTitleTag.
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
    ///   Gets or sets Style.
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
    ///   Gets or sets the ThemePage -- Defaults to "ICONS"
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
    ///   Gets or sets the actual theme item
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
    ///   Gets or sets a value indicating whether UseTitleForEmptyAlt.
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
    /// Current image alt value
    /// </returns>
    protected string GetCurrentThemeItem()
    {
      if (this._themePage.IsSet() && this._themeTag.IsSet())
      {
        return this.PageContext.Get<ITheme>().GetItem(this.ThemePage, this.ThemeTag, null);
      }

      return null;
    }

    /// <summary>
    /// The get current title item.
    /// </summary>
    /// <returns>
    /// The current title item string.
    /// </returns>
    protected string GetCurrentTitleItem()
    {
      if (this._localizedTitlePage.IsSet() && this._localizedTitleTag.IsSet())
      {
        return this.GetText(this._localizedTitlePage, this._localizedTitleTag);
      }

        return this._localizedTitleTag.IsSet() ? this.GetText(this._localizedTitleTag) : null;
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter output)
    {
      // vzrus: Don't render control if not enabled
      if (!this.Enabled)
      {
        return;
      }

      string src = this.GetCurrentThemeItem();
        string title = string.IsNullOrEmpty(this.LocalizedTitle.Trim()) ? this.GetCurrentTitleItem() : this.LocalizedTitle;

      // might not be needed...
      if (src.IsNotSet())
      {
        return;
      }

      if (this.UseTitleForEmptyAlt && this.Alt.IsNotSet() && title.IsSet())
      {
        this.Alt = title;
      }

      output.BeginRender();
      output.WriteBeginTag("img");
      output.WriteAttribute("id", this.ClientID);

      // this will output the src and alt attributes
      output.WriteAttribute("src", src);
      output.WriteAttribute("alt", HttpUtility.HtmlEncode(this.Alt));

      if (this.Style.IsSet())
      {
        output.WriteAttribute("style", this.Style);
      }

      if (this.CssClass.IsSet())
      {
        output.WriteAttribute("class", this.CssClass);
      }

      if (title.IsSet())
      {
        output.WriteAttribute("title", HttpUtility.HtmlEncode(title));
      }

      // self closing end tag "/>"
      output.Write(HtmlTextWriter.SelfClosingTagEnd);

      output.EndRender();
    }

    #endregion
  }
}