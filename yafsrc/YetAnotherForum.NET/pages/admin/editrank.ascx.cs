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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Add or Edit User Rank Page.
    /// </summary>
    public partial class editrank : AdminPage
    {
        #region Methods

        /// <summary>
        /// Cancel Edit and go Back to the Admin Ranks Page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_ranks);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.BindData();

                if (this.Request.QueryString.GetFirstOrDefault("r") != null)
                {
                    using (
                        var dt = LegacyDb.rank_list(
                            this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("r")))
                    {
                        var row = dt.Rows[0];
                        var flags = new RankFlags(row["Flags"]);
                        this.Name.Text = (string)row["Name"];
                        this.IsStart.Checked = flags.IsStart;
                        this.IsLadder.Checked = flags.IsLadder;
                        this.MinPosts.Text = row["MinPosts"].ToString();
                        this.PMLimit.Text = row["PMLimit"].ToString();
                        this.Style.Text = row["Style"].ToString();
                        this.RankPriority.Text = row["SortOrder"].ToString();
                        this.UsrAlbums.Text = row["UsrAlbums"].ToString();
                        this.UsrAlbumImages.Text = row["UsrAlbumImages"].ToString();
                        this.UsrSigChars.Text = row["UsrSigChars"].ToString();
                        this.UsrSigBBCodes.Text = row["UsrSigBBCodes"].ToString();
                        this.UsrSigHTMLTags.Text = row["UsrSigHTMLTags"].ToString();
                        this.Description.Text = row["Description"].ToString();

                        var item = this.RankImage.Items.FindByText(row["RankImage"].ToString());

                        if (item != null)
                        {
                            item.Selected = true;
                            this.Preview.Src = "{0}{1}/{2}".FormatWith(
                                YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Ranks, row["RankImage"]);
                        }
                        else
                        {
                            this.Preview.Src = YafForumInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry
                        }
                    }
                }
                else
                {
                    this.Preview.Src = YafForumInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry
                }
            }

            this.RankImage.Attributes["onchange"] =
                "getElementById('{2}').src='{0}{1}/' + this.value".FormatWith(
                    YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Ranks, this.Preview.ClientID);
        }



        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(
                this.GetText("ADMIN_RANKS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_ranks));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITRANK", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_RANKS", "TITLE"),
                this.GetText("ADMIN_EDITRANK", "TITLE"));
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
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_VALID_NUMBER"), MessageTypes.danger);
                return;
            }

            if (!ValidationHelper.IsValidInt(this.RankPriority.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITRANK", "MSG_RANK_INTEGER"), MessageTypes.danger);
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrAlbums.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_ALBUM_NUMBER"), MessageTypes.danger);
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrSigChars.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_SIG_NUMBER"), MessageTypes.danger);
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrAlbumImages.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_TOTAL_NUMBER"), MessageTypes.danger);
                return;
            }

            // Group
            var rankID = 0;
            if (this.Request.QueryString.GetFirstOrDefault("r") != null)
            {
                rankID = int.Parse(this.Request.QueryString.GetFirstOrDefault("r"));
            }

            object rankImage = null;
            if (this.RankImage.SelectedIndex > 0)
            {
                rankImage = this.RankImage.SelectedValue;
            }

            LegacyDb.rank_save(
                rankID,
                this.PageContext.PageBoardID,
                this.Name.Text,
                this.IsStart.Checked,
                this.IsLadder.Checked,
                this.MinPosts.Text,
                rankImage,
                this.PMLimit.Text.Trim().ToType<int>(),
                this.Style.Text.Trim(),
                this.RankPriority.Text.Trim(),
                this.Description.Text,
                this.UsrSigChars.Text.Trim().ToType<int>(),
                this.UsrSigBBCodes.Text.Trim(),
                this.UsrSigHTMLTags.Text.Trim(),
                this.UsrAlbums.Text.Trim().ToType<int>(),
                this.UsrAlbumImages.Text.Trim().ToType<int>());

            // Clearing cache with old permisssions data...
            this.Get<IDataCache>()
                .RemoveOf<object>(k => k.Key.StartsWith(Constants.Cache.ActiveUserLazyData.FormatWith(string.Empty)));

            // Clear Styling Caching
            this.Get<IDataCache>().Remove(Constants.Cache.GroupRankStyles);

            YafBuildLink.Redirect(ForumPages.admin_ranks);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            using (var dt = new DataTable("Files"))
            {
                dt.Columns.Add("FileID", typeof(long));
                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("Description", typeof(string));
                var dr = dt.NewRow();
                dr["FileID"] = 0;
                dr["FileName"] = YafForumInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry
                dr["Description"] = this.GetText("ADMIN_EDITRANK", "SELECT_IMAGE");
                dt.Rows.Add(dr);

                var dir =
                    new DirectoryInfo(
                        this.Request.MapPath(
                            "{0}{1}".FormatWith(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Ranks)));
                var files = dir.GetFiles("*.*");
                long nFileID = 1;

                foreach (var file in from file in files
                                          let sExt = file.Extension.ToLower()
                                          where sExt == ".png" || sExt == ".gif" || sExt == ".jpg"
                                          select file)
                {
                    dr = dt.NewRow();
                    dr["FileID"] = nFileID++;
                    dr["FileName"] = file.Name;
                    dr["Description"] = file.Name;
                    dt.Rows.Add(dr);
                }

                this.RankImage.DataSource = dt;
                this.RankImage.DataValueField = "FileName";
                this.RankImage.DataTextField = "Description";
            }

            this.DataBind();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}