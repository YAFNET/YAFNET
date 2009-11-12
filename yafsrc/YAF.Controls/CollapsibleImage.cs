/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  /// <summary>
  /// The collapsible image.
  /// </summary>
  public class CollapsibleImage : ImageButton
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CollapsibleImage"/> class.
    /// </summary>
    public CollapsibleImage()
    {
    }

    /// <summary>
    /// Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        if (Site != null && Site.DesignMode == true)
        {
          // design-time, return null...
          return null;
        }

        return YafContext.Current;
      }
    }

    /// <summary>
    /// Gets or sets PanelID.
    /// </summary>
    public string PanelID
    {
      get
      {
        if (ViewState["PanelID"] != null)
        {
          return ViewState["PanelID"].ToString();
        }

        return null;
      }

      set
      {
        ViewState["PanelID"] = value;
      }
    }

    /// <summary>
    /// Gets or sets PanelState.
    /// </summary>
    public PanelSessionState.CollapsiblePanelState PanelState
    {
      get
      {
        return Mession.PanelState[PanelID];
      }

      set
      {
        Mession.PanelState[PanelID] = value;
      }
    }

    /// <summary>
    /// Gets or sets DefaultState.
    /// </summary>
    public PanelSessionState.CollapsiblePanelState DefaultState
    {
      get
      {
        PanelSessionState.CollapsiblePanelState defaultState = PanelSessionState.CollapsiblePanelState.Expanded;

        if (ViewState["DefaultState"] != null)
        {
          defaultState = (PanelSessionState.CollapsiblePanelState) ViewState["DefaultState"];
        }

        if (defaultState == PanelSessionState.CollapsiblePanelState.None)
        {
          defaultState = PanelSessionState.CollapsiblePanelState.Expanded;
        }

        return defaultState;
      }

      set
      {
        ViewState["DefaultState"] = value;
      }
    }

    /// <summary>
    /// Gets or sets AttachedControlID.
    /// </summary>
    public string AttachedControlID
    {
      get
      {
        if (ViewState["AttachedControlID"] != null)
        {
          return ViewState["AttachedControlID"].ToString();
        }

        return null;
      }

      set
      {
        ViewState["AttachedControlID"] = value;
      }
    }

    /// <summary>
    /// The on click.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnClick(ImageClickEventArgs e)
    {
      // toggle the status...
      Mession.PanelState.TogglePanelState(PanelID, DefaultState);
      UpdateAttachedVisibility();

      base.OnClick(e);
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender(EventArgs e)
    {
      // setup inital image state...
      ImageUrl = PageContext.Theme.GetCollapsiblePanelImageURL(PanelID, DefaultState);
      UpdateAttachedVisibility();

      base.OnPreRender(e);
    }

    /// <summary>
    /// The update attached visibility.
    /// </summary>
    protected void UpdateAttachedVisibility()
    {
      if (GetAttachedControl() != null)
      {
        // modify the visability depending on the status...
        GetAttachedControl().Visible = PanelState == PanelSessionState.CollapsiblePanelState.Expanded;
      }
    }

    /// <summary>
    /// The get attached control.
    /// </summary>
    /// <returns>
    /// </returns>
    protected Control GetAttachedControl()
    {
      Control control = null;

      if (!String.IsNullOrEmpty(AttachedControlID))
      {
        // attempt to find this control...
        control = Parent.FindControl(AttachedControlID) as Control;
      }

      return control;
    }
  }
}