/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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

namespace YAF.Pages
{
    #region Using

    using System;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Private Message Page
    /// </summary>
    public partial class cp_pm : ForumPageRegistered
    {
        #region Constants and Fields

        /// <summary>
        ///   The _view.
        /// </summary>
        private PMView _view;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "cp_pm" /> class.
        /// </summary>
        public cp_pm()
            : base("CP_PM")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets View.
        /// </summary>
        protected PMView View
        {
            get
            {
                return this._view;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and Jquery Ui Tabs.
            YafContext.Current.PageElements.RegisterJsBlock(
                "yafPmTabsJs",
                JavaScriptBlocks.JqueryUITabsLoadJs(
                    this.PmTabs.ClientID,
                    this.hidLastTab.ClientID,
                    this.hidLastTabId.ClientID,
                    false));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // check if this feature is disabled
            if (!this.Get<YafBoardSettings>().AllowPrivateMessages)
            {
                YafBuildLink.RedirectInfoPage(InfoMessage.Disabled);
            }

            if (this.IsPostBack)
            {
                return;
            }

            if (this.Request.QueryString.GetFirstOrDefault("v").IsSet())
            {
                this._view = PMViewConverter.FromQueryString(this.Request.QueryString.GetFirstOrDefault("v"));

                this.hidLastTab.Value = ((int)this._view).ToString();
            }

            // if (_view == PMView.Inbox)
            // this.PMTabs.ActiveTab = this.InboxTab;
            // else if (_view == PMView.Outbox)
            // this.PMTabs.ActiveTab = this.OutboxTab;
            // else if (_view == PMView.Archive)
            // this.PMTabs.ActiveTab = this.ArchiveTab;
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.Get<YafBoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                YafBuildLink.GetLink(ForumPages.cp_profile));
            this.PageLinks.AddLink(this.GetText("TITLE"));

            // InboxTab.HeaderText = GetText("INBOX");
            // OutboxTab.HeaderText = GetText("SENTITEMS");
            // ArchiveTab.HeaderText = GetText("ARCHIVE");
            this.NewPM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage);
            this.NewPM2.NavigateUrl = this.NewPM.NavigateUrl;

            // inbox tab
            // ScriptManager.RegisterClientScriptBlock(InboxTabUpdatePanel, typeof(UpdatePanel), "InboxTabRefresh", String.Format("function InboxTabRefresh() {1}\n__doPostBack('{0}', '');\n{2}", InboxTabUpdatePanel.ClientID, '{', '}'), true);
            // sent tab
            // ScriptManager.RegisterClientScriptBlock(SentTabUpdatePanel, typeof(UpdatePanel), "SentTabRefresh", String.Format("function SentTabRefresh() {1}\n__doPostBack('{0}', '');\n{2}", SentTabUpdatePanel.ClientID, '{', '}'), true);
            // archive tab
            // ScriptManager.RegisterClientScriptBlock(ArchiveTabUpdatePanel, typeof(UpdatePanel), "ArchiveTabRefresh", String.Format("function ArchiveTabRefresh() {1}\n__doPostBack('{0}', '');\n{2}", ArchiveTabUpdatePanel.ClientID, '{', '}'), true);
        }

        #endregion
    }
}