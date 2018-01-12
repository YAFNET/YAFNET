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

namespace YAF.Dialogs
{
    #region Using

    using System;

    using YAF.Classes.Data;
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
    /// The Nntp Forum Add/Edit Dialog.
    /// </summary>
    public partial class NntpForumEdit : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets or sets the forum identifier.
        /// </summary>
        /// <value>
        /// The forum identifier.
        /// </value>
        public int? ForumId
        {
            get
            {
                return this.ViewState["ForumId"].ToType<int?>();
            }

            set
            {
                this.ViewState["ForumId"] = value;
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="forumId">The forum identifier.</param>
        public void BindData(int? forumId)
        {
            this.NntpServerID.DataSource = LegacyDb.nntpserver_list(this.PageContext.PageBoardID, null);
            this.NntpServerID.DataValueField = "NntpServerID";
            this.NntpServerID.DataTextField = "Name";
            this.NntpServerID.DataBind();

            this.ForumID.DataSource = LegacyDb.forum_listall_sorted(
                this.PageContext.PageBoardID, this.PageContext.PageUserID);
            this.ForumID.DataValueField = "ForumID";
            this.ForumID.DataTextField = "Title";
            this.ForumID.DataBind();

            this.ForumId = forumId;

            this.Title.LocalizedPage = "ADMIN_EDITNNTPSERVER";
            this.Save.TextLocalizedPage = "ADMIN_NNTPFORUMS";

            if (this.ForumId.HasValue)
            {
                // Edit
                var forum = this.GetRepository<NntpForum>().GetById(this.ForumId.Value);

                if (forum != null)
                {
                    this.NntpServerID.Items.FindByValue(forum.NntpServerID.ToString()).Selected = true;
                    this.GroupName.Text = forum.GroupName;
                    this.ForumID.Items.FindByValue(forum.ForumID.ToString()).Selected = true;
                    this.Active.Checked = forum.Active;
                    this.DateCutOff.Text = forum.DateCutOff.ToString();
                }

                this.Title.LocalizedTag = "TITLE_EDIT";
                this.Save.TextLocalizedTag = "SAVE";
            }
            else
            {
                // Add
                this.GroupName.Text = string.Empty;
                this.Active.Checked = false;
                this.DateCutOff.Text = string.Empty;

                this.Title.LocalizedTag = "TITLE";
                this.Save.TextLocalizedTag = "NEW_FORUM";
            }
        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.GroupName.Text.Trim().IsNotSet())
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITNNTPFORUM", "MSG_VALID_GROUP"), MessageTypes.warning);
                return;
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
                this.ForumId,
                this.NntpServerID.SelectedValue,
                this.GroupName.Text,
                this.ForumID.SelectedValue,
                this.Active.Checked,
                dateCutOff == DateTime.MinValue ? null : (DateTime?)dateCutOff);

            YafBuildLink.Redirect(ForumPages.admin_nntpforums);
        }

        #endregion
    }
}