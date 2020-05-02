/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * https://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Web.Controls
{
    #region Using

    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The collapsible image.
    /// </summary>
    public class CollapseButton : LinkButton
    {
        #region Properties

        /// <summary>
        ///   Gets or sets AttachedControlID.
        /// </summary>
        [CanBeNull]
        public string AttachedControlID
        {
            get => this.ViewState["AttachedControlID"]?.ToString();

            set => this.ViewState["AttachedControlID"] = value;
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

            set => this.ViewState["DefaultState"] = value;
        }

        /// <summary>
        ///   Gets PageContext.
        /// </summary>
        public BoardContext PageContext
        {
            get
            {
                if (this.Site != null && this.Site.DesignMode)
                {
                    // design-time, return null...
                    return null;
                }

                return BoardContext.Current;
            }
        }

        /// <summary>
        ///   Gets or sets PanelID.
        /// </summary>
        [CanBeNull]
        public string PanelID
        {
            get => this.ViewState["PanelID"]?.ToString();

            set => this.ViewState["PanelID"] = value;
        }

        /// <summary>
        ///   Gets or sets PanelState.
        /// </summary>
        public CollapsiblePanelState PanelState
        {
            get => BoardContext.Current.Get<ISession>().PanelState[this.PanelID];

            set => BoardContext.Current.Get<ISession>().PanelState[this.PanelID] = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get attached control.
        /// </summary>
        /// <returns>
        /// Returns the Attached Control
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
        /// The on pre render.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup initial image state...
            this.Text = new Icon
                            {
                                IconName = GetCollapsiblePanelIcon(this.PanelID, this.DefaultState),
                                IconType = "text-primary"
                            }.RenderToString();

            this.CssClass = "btn-collapse px-0";

            this.Attributes.Add("aria-label", "collapse button");

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
                // modify the visibility depending on the status...
                this.GetAttachedControl().Visible = this.PanelState == CollapsiblePanelState.Expanded;
            }
        }

        /// <summary>
        /// The on click.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnClick(EventArgs e)
        {
            // toggle the status...
            BoardContext.Current.Get<ISession>().PanelState.TogglePanelState(this.PanelID, this.DefaultState);
            this.UpdateAttachedVisibility();

            base.OnClick(e);
        }

        /// <summary>
        /// Gets the collapsible panel image url (expanded or collapsed).
        /// </summary>
        /// <param name="panelId">
        /// ID of collapsible panel
        /// </param>
        /// <param name="defaultState">
        /// Default Panel State
        /// </param>
        /// <returns>
        /// Image URL
        /// </returns>
        private static string GetCollapsiblePanelIcon([NotNull] string panelId, CollapsiblePanelState defaultState)
        {
            CodeContracts.VerifyNotNull(panelId, "panelID");

            var stateValue = BoardContext.Current.Get<ISession>().PanelState[panelId];

            if (stateValue != CollapsiblePanelState.None)
            {
                return stateValue == CollapsiblePanelState.Expanded ? "minus-square" : "plus-square";
            }

            stateValue = defaultState;
            BoardContext.Current.Get<ISession>().PanelState[panelId] = defaultState;

            return stateValue == CollapsiblePanelState.Expanded ? "minus-square" : "plus-square";
        }

        #endregion
    }
}