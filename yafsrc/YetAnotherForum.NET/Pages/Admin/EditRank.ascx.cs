﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Pages.Admin;

using YAF.Types.Models;

/// <summary>
/// Add or Edit User Rank Page.
/// </summary>
public partial class EditRank : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditRank"/> class. 
    /// </summary>
    public EditRank()
        : base("ADMIN_EDITRANK", ForumPages.Admin_EditRank)
    {
    }

    /// <summary>
    /// Cancel Edit and go Back to the Admin Ranks Page.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Ranks);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FormValidatorJs),
            JavaScriptBlocks.FormValidatorJs(this.Save.ClientID));

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

        if (rank == null)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.Name.Text = rank.Name;
        this.IsStart.Checked = rank.RankFlags.IsStart;
        this.IsLadder.Checked = rank.RankFlags.IsLadder;
        this.MinPosts.Text = rank.MinPosts.ToString();
        this.PMLimit.Text = rank.PMLimit.ToString();
        this.Style.Text = rank.Style;
        this.RankPriority.Text = rank.SortOrder.ToString();
        this.UsrAlbums.Text = rank.UsrAlbums.ToString();
        this.UsrAlbumImages.Text = rank.UsrAlbumImages.ToString();
        this.UsrSigChars.Text = rank.UsrSigChars.ToString();
        this.UsrSigBBCodes.Text = rank.UsrSigBBCodes;
        this.Description.Text = rank.Description;

        this.DataBind();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_RANKS", "TITLE"), this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Ranks));

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITRANK", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Save (New) Rank
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void Save_Click(object sender, EventArgs e)
    {
        if (!ValidationHelper.IsValidInt(this.PMLimit.Text.Trim()))
        {
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_EDITGROUP", "MSG_VALID_NUMBER"),
                MessageTypes.danger);
            return;
        }

        if (!ValidationHelper.IsValidInt(this.RankPriority.Text.Trim()))
        {
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_EDITRANK", "MSG_RANK_INTEGER"),
                MessageTypes.danger);
            return;
        }

        if (!ValidationHelper.IsValidInt(this.UsrAlbums.Text.Trim()))
        {
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_EDITGROUP", "MSG_ALBUM_NUMBER"),
                MessageTypes.danger);
            return;
        }

        if (!ValidationHelper.IsValidInt(this.UsrSigChars.Text.Trim()))
        {
            this.PageBoardContext.Notify(this.GetText("ADMIN_EDITGROUP", "MSG_SIG_NUMBER"), MessageTypes.danger);
            return;
        }

        if (!ValidationHelper.IsValidInt(this.UsrAlbumImages.Text.Trim()))
        {
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_EDITGROUP", "MSG_TOTAL_NUMBER"),
                MessageTypes.danger);
            return;
        }

        // Rank
        int? rankId = null;
        if (this.Get<HttpRequestBase>().QueryString.Exists("r"))
        {
            rankId = this.Request.QueryString.GetFirstOrDefaultAs<int>("r");
        }

        this.GetRepository<Rank>().Save(
            rankId,
            this.PageBoardContext.PageBoardID,
            this.Name.Text,
            new RankFlags { IsStart = this.IsStart.Checked, IsLadder = this.IsLadder.Checked },
            this.MinPosts.Text.ToType<int>(),
            this.PMLimit.Text.Trim().ToType<int>(),
            this.Style.Text.Trim(),
            this.RankPriority.Text.Trim().ToType<short>(),
            this.Description.Text,
            this.UsrSigChars.Text.Trim().ToType<int>(),
            this.UsrSigBBCodes.Text.Trim(),
            this.UsrAlbums.Text.Trim().ToType<int>(),
            this.UsrAlbumImages.Text.Trim().ToType<int>());

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Ranks);
    }
}