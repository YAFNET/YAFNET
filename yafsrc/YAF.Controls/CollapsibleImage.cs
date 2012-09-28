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

  using System;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using YAF.Core;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Types.Interfaces.Extensions;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The collapsible image.
  /// </summary>
  public class CollapsibleImage : ImageButton
  {
    #region Properties

    /// <summary>
    ///   Gets or sets AttachedControlID.
    /// </summary>
    [CanBeNull]
    public string AttachedControlID
    {
      get
      {
        if (this.ViewState["AttachedControlID"] != null)
        {
          return this.ViewState["AttachedControlID"].ToString();
        }

        return null;
      }

      set
      {
        this.ViewState["AttachedControlID"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets DefaultState.
    /// </summary>
    public CollapsiblePanelState DefaultState
    {
      get
      {
        CollapsiblePanelState defaultState = CollapsiblePanelState.Expanded;

        if (this.ViewState["DefaultState"] != null)
        {
          defaultState = (CollapsiblePanelState)this.ViewState["DefaultState"];
        }

        if (defaultState == CollapsiblePanelState.None)
        {
          defaultState = CollapsiblePanelState.Expanded;
        }

        return defaultState;
      }

      set
      {
        this.ViewState["DefaultState"] = value;
      }
    }

    /// <summary>
    ///   Gets PageContext.
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
    ///   Gets or sets PanelID.
    /// </summary>
    [CanBeNull]
    public string PanelID
    {
      get
      {
        if (this.ViewState["PanelID"] != null)
        {
          return this.ViewState["PanelID"].ToString();
        }

        return null;
      }

      set
      {
        this.ViewState["PanelID"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets PanelState.
    /// </summary>
    public CollapsiblePanelState PanelState
    {
      get
      {
        return YafContext.Current.Get<IYafSession>().PanelState[this.PanelID];
      }

      set
      {
        YafContext.Current.Get<IYafSession>().PanelState[this.PanelID] = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get attached control.
    /// </summary>
    /// <returns>
    /// </returns>
    protected Control GetAttachedControl()
    {
      Control control = null;

      if (this.AttachedControlID.IsSet())
      {
        // attempt to find this control...
        control = this.Parent.FindControl(this.AttachedControlID);
      }

      return control;
    }

    /// <summary>
    /// The on click.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnClick([NotNull] ImageClickEventArgs e)
    {
      // toggle the status...
      YafContext.Current.Get<IYafSession>().PanelState.TogglePanelState(this.PanelID, this.DefaultState);
      this.UpdateAttachedVisibility();

      base.OnClick(e);
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      // setup inital image state...
      this.ImageUrl = this.PageContext.Theme.GetCollapsiblePanelImageURL(this.PanelID, this.DefaultState);
      this.UpdateAttachedVisibility();

      base.OnPreRender(e);
    }

    /// <summary>
    /// The update attached visibility.
    /// </summary>
    protected void UpdateAttachedVisibility()
    {
      if (this.GetAttachedControl() != null)
      {
        // modify the visability depending on the status...
        this.GetAttachedControl().Visible = this.PanelState == CollapsiblePanelState.Expanded;
      }
    }

    #endregion
  }
}