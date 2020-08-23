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
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Private Message Page
    /// </summary>
    public partial class MyMessages : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MyMessages" /> class.
        /// </summary>
        public MyMessages()
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
            this.PageContext.PageElements.RegisterJsBlock(
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

            this.NewPM.NavigateUrl = BuildLink.GetLink(ForumPages.PostPrivateMessage);
            this.NewPM2.NavigateUrl = this.NewPM.NavigateUrl;

            // Renew PM Statistics
            var dt = this.GetRepository<PMessage>().UserMessageCount(this.PageContext.PageUserID);
            if (dt.HasRows())
            {
                this.InfoInbox.Text = this.InfoArchive.Text = this.InfoOutbox.Text = this.GetPMessageText(
                    "PMLIMIT_ALL",
                    dt.Rows[0]["NumberTotal"],
                    dt.Rows[0]["NumberIn"],
                    dt.Rows[0]["NumberOut"],
                    dt.Rows[0]["NumberArchived"],
                    dt.Rows[0]["NumberAllowed"]);
            }
        }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="total">The total.</param>
        /// <param name="inbox">The inbox.</param>
        /// <param name="outbox">The outbox.</param>
        /// <param name="archive">The archive.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Returns the Message Text</returns>
        protected string GetPMessageText(
            [NotNull] string text,
            [NotNull] object total,
            [NotNull] object inbox,
            [NotNull] object outbox,
            [NotNull] object archive,
            [NotNull] object limit)
        {
            object percentage = 0;
            if (limit.ToType<int>() != 0)
            {
                percentage = decimal.Round(total.ToType<decimal>() / limit.ToType<decimal>() * 100, 2);
            }

            if (!this.PageContext.IsAdmin)
            {
                return this.HtmlEncode(this.GetTextFormatted(text, total, inbox, outbox, archive, limit, percentage));
            }

            limit = "\u221E";
            percentage = 0;

            return this.HtmlEncode(this.GetTextFormatted(text, total, inbox, outbox, archive, limit, percentage));
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.PageContext.User.DisplayOrUserName(), BuildLink.GetLink(ForumPages.MyAccount));
            this.PageLinks.AddLink(this.GetText("TITLE"));
        }

        #endregion
    }
}