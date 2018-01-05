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
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Admin Edit NNTP Forum Page.
    /// </summary>
    public partial class editnntpforum : AdminPage
    {
        #region Methods

        /// <summary>
        /// Cancel Import and Return Back to Previous Page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_nntpforums);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_NNTPFORUMS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_nntpforums));
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITNNTPFORUM", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_NNTPFORUMS", "TITLE"),
                this.GetText("ADMIN_EDITNNTPFORUM", "TITLE"));

            this.Save.Text = "<i class=\"fa fa-save fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("SAVE"));
            this.Cancel.Text = "<i class=\"fa fa-times fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("CANCEL"));

            this.BindData();

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("s") == null)
            {
                return;
            }

            var forum = this.GetRepository<NntpForum>().GetById(
                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("s").ToType<int>());

            if (forum != null)
            {
                this.NntpServerID.Items.FindByValue(forum.NntpServerID.ToString()).Selected = true;
                this.GroupName.Text = forum.GroupName;
                this.ForumID.Items.FindByValue(forum.ForumID.ToString()).Selected = true;
                this.Active.Checked = forum.Active;
                this.DateCutOff.Text = forum.DateCutOff.ToString();
            }
        }

        /// <summary>
        /// The save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.GroupName.Text.Trim().IsNotSet())
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITNNTPFORUM", "MSG_VALID_GROUP"), MessageTypes.warning);
                return;
            }

            object nntpForumID = null;
            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("s") != null)
            {
                nntpForumID = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("s");
            }

            if (this.ForumID.SelectedValue.ToType<int>() <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITNNTPFORUM", "MSG_SELECT_FORUM"));
                return;
            }

            DateTime dateCutOff;

            if (!DateTime.TryParse(this.DateCutOff.Text, out dateCutOff))
            {
                dateCutOff = DateTime.MinValue;
            }

            LegacyDb.nntpforum_save(
                nntpForumID,
                this.NntpServerID.SelectedValue,
                this.GroupName.Text,
                this.ForumID.SelectedValue,
                this.Active.Checked,
                dateCutOff == DateTime.MinValue ? null : (DateTime?)dateCutOff);

            YafBuildLink.Redirect(ForumPages.admin_nntpforums);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.NntpServerID.DataSource = LegacyDb.nntpserver_list(this.PageContext.PageBoardID, null);
            this.NntpServerID.DataValueField = "NntpServerID";
            this.NntpServerID.DataTextField = "Name";
            this.ForumID.DataSource = LegacyDb.forum_listall_sorted(
                this.PageContext.PageBoardID, this.PageContext.PageUserID);
            this.ForumID.DataValueField = "ForumID";
            this.ForumID.DataTextField = "Title";
            this.DataBind();
        }

        #endregion
    }
}