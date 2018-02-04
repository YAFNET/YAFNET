/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
          return this.ViewState["AttachedControlID"]?.ToString();
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
        var defaultState = CollapsiblePanelState.Expanded;

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
      this.ImageUrl = this.PageContext.Get<ITheme>().GetCollapsiblePanelImageURL(this.PanelID, this.DefaultState);
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