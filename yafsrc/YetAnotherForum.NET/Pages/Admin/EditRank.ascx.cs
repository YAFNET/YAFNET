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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Add or Edit User Rank Page.
    /// </summary>
    public partial class EditRank : AdminPage
    {
        #region Methods

        /// <summary>
        /// Cancel Edit and go Back to the Admin Ranks Page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Admin_Ranks);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            if (!this.Request.QueryString.Exists("r"))
            {
                return;
            }

            var rankId = this.Request.QueryString.GetFirstOrDefaultAs<int>("r");
            var rank = this.GetRepository<Rank>().GetById(rankId);

            var flags = new RankFlags(rank.Flags);
            this.Name.Text = rank.Name;
            this.IsStart.Checked = flags.IsStart;
            this.IsLadder.Checked = flags.IsLadder;
            this.MinPosts.Text = rank.MinPosts.ToString();
            this.PMLimit.Text = rank.PMLimit.ToString();
            this.Style.Text = rank.Style;
            this.RankPriority.Text = rank.SortOrder.ToString();
            this.UsrAlbums.Text = rank.UsrAlbums.ToString();
            this.UsrAlbumImages.Text = rank.UsrAlbumImages.ToString();
            this.UsrSigChars.Text = rank.UsrSigChars.ToString();
            this.UsrSigBBCodes.Text = rank.UsrSigBBCodes;
            this.UsrSigHTMLTags.Text = rank.UsrSigHTMLTags;
            this.Description.Text = rank.Description;

            this.DataBind();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_RANKS", "TITLE"), BuildLink.GetLink(ForumPages.Admin_Ranks));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITRANK", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_RANKS", "TITLE")} - {this.GetText("ADMIN_EDITRANK", "TITLE")}";
        }

        /// <summary>
        /// Save (New) Rank
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!ValidationHelper.IsValidInt(this.PMLimit.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITGROUP", "MSG_VALID_NUMBER"),
                    MessageTypes.danger);
                return;
            }

            if (!ValidationHelper.IsValidInt(this.RankPriority.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITRANK", "MSG_RANK_INTEGER"),
                    MessageTypes.danger);
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrAlbums.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITGROUP", "MSG_ALBUM_NUMBER"),
                    MessageTypes.danger);
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrSigChars.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_SIG_NUMBER"), MessageTypes.danger);
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrAlbumImages.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITGROUP", "MSG_TOTAL_NUMBER"),
                    MessageTypes.danger);
                return;
            }

            // Group
            var rankID = 0;
            if (this.Get<HttpRequestBase>().QueryString.Exists("r"))
            {
                rankID = int.Parse(this.Request.QueryString.GetFirstOrDefault("r"));
            }

            this.GetRepository<Rank>().Save(
                rankID,
                this.PageContext.PageBoardID,
                this.Name.Text,
                this.IsStart.Checked,
                this.IsLadder.Checked,
                this.MinPosts.Text,
                this.PMLimit.Text.Trim().ToType<int>(),
                this.Style.Text.Trim(),
                this.RankPriority.Text.Trim(),
                this.Description.Text,
                this.UsrSigChars.Text.Trim().ToType<int>(),
                this.UsrSigBBCodes.Text.Trim(),
                this.UsrSigHTMLTags.Text.Trim(),
                this.UsrAlbums.Text.Trim().ToType<int>(),
                this.UsrAlbumImages.Text.Trim().ToType<int>());

            // Clearing cache with old permissions data...
            this.Get<IDataCache>().RemoveOf<object>(
                k => k.Key.StartsWith(string.Format(Constants.Cache.ActiveUserLazyData, string.Empty)));

            // Clear Styling Caching
            this.Get<IDataCache>().Remove(Constants.Cache.GroupRankStyles);

            BuildLink.Redirect(ForumPages.Admin_Ranks);
        }

        #endregion
    }
}