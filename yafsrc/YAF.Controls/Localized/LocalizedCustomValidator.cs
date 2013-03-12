/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Web.UI.WebControls;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;

  #endregion

  /// <summary>
  /// The localized custom validator.
  /// </summary>
  public class LocalizedCustomValidator : CustomValidator, ILocalizationSupport
  {
    #region Constants and Fields

    /// <summary>
    /// The _enable bb code.
    /// </summary>
    private bool _enableBbCode;

    /// <summary>
    /// The _localized page.
    /// </summary>
    private string _localizedPage;

    /// <summary>
    /// The _localized tag.
    /// </summary>
    private string _localizedTag;

    /// <summary>
    /// The _param 0.
    /// </summary>
    private string _param0;

    /// <summary>
    /// The _param 1.
    /// </summary>
    private string _param1;

    /// <summary>
    /// The _param 2.
    /// </summary>
    private string _param2;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether EnableBBCode.
    /// </summary>
    public bool EnableBBCode
    {
      get
      {
        return this._enableBbCode;
      }

      set
      {
        this._enableBbCode = value;
      }
    }

    /// <summary>
    /// Gets or sets LocalizedPage.
    /// </summary>
    public string LocalizedPage
    {
      get
      {
        return this._localizedPage;
      }

      set
      {
        this._localizedPage = value;
      }
    }

    /// <summary>
    /// Gets or sets LocalizedTag.
    /// </summary>
    public string LocalizedTag
    {
      get
      {
        return this._localizedTag;
      }

      set
      {
        this._localizedTag = value;
      }
    }

    /// <summary>
    /// Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        if (this.Site != null && this.Site.DesignMode)
        {
          // design-time, return null...
          return null;
        }

        return YafContext.Current;
      }
    }

    /// <summary>
    /// Gets or sets Param0.
    /// </summary>
    public string Param0
    {
      get
      {
        return this._param0;
      }

      set
      {
        this._param0 = value;
      }
    }

    /// <summary>
    /// Gets or sets Param1.
    /// </summary>
    public string Param1
    {
      get
      {
        return this._param1;
      }

      set
      {
        this._param1 = value;
      }
    }

    /// <summary>
    /// Gets or sets Param2.
    /// </summary>
    public string Param2
    {
      get
      {
        return this._param2;
      }

      set
      {
        this._param2 = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on load.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      // localize ErrorMessage, ToolTip
      this.ErrorMessage = this.Localize(this);
      this.ToolTip = this.Localize(this);
    }

    #endregion
  }
}