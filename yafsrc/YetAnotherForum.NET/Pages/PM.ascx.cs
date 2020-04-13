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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Private Message Page
    /// </summary>
    public partial class PM : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PM" /> class.
        /// </summary>
        public PM()
            : base("PM")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets View.
        /// </summary>
        protected PmView View { get; private set; }

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
            BoardContext.Current.PageElements.RegisterJsBlock(
                "yafPmTabsJs",
                JavaScriptBlocks.BootstrapTabsLoadJs(this.PmTabs.ClientID, this.hidLastTab.ClientID));

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
            if (!this.Get<BoardSettings>().AllowPrivateMessages)
            {
                BuildLink.RedirectInfoPage(InfoMessage.Disabled);
            }

            if (this.IsPostBack)
            {
                return;
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v").IsSet())
            {
                this.View = PmViewConverter.FromQueryString(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v"));

                this.hidLastTab.Value = $"View{(int)this.View}";
            }

            this.NewPM.NavigateUrl = BuildLink.GetLinkNotEscaped(ForumPages.PostPrivateMessage);
            this.NewPM2.NavigateUrl = this.NewPM.NavigateUrl;
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.Get<BoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                BuildLink.GetLink(ForumPages.Account));
            this.PageLinks.AddLink(this.GetText("TITLE"));
        }

        #endregion
    }
}