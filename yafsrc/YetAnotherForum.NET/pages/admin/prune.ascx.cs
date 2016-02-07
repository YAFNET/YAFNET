/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Tasks;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Summary description for prune.
    /// </summary>
    public partial class prune : AdminPage
    {
        #region Methods

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
            if (!this.IsPostBack)
            {
                this.PageLinks.AddRoot();
                this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
                this.PageLinks.AddLink(this.GetText("ADMIN_PRUNE", "TITLE"), string.Empty);

                this.Page.Header.Title = "{0} - {1}".FormatWith(
                      this.GetText("ADMIN_ADMIN", "Administration"),
                      this.GetText("ADMIN_PRUNE", "TITLE"));

                this.commit.Text = this.GetText("ADMIN_PRUNE", "PRUNE_START");

                this.days.Text = "60";
                this.BindData();
            }

            this.lblPruneInfo.Text = string.Empty;

            if (this.Get<ITaskModuleManager>().IsTaskRunning(PruneTopicTask.TaskName))
            {
                this.lblPruneInfo.Text = this.GetText("ADMIN_PRUNE", "PRUNE_INFO");
                this.commit.Enabled = false;
            }
        }

        /// <summary>
        /// The prune button_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PruneButton_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_PRUNE", "CONFIRM_PRUNE"));
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.forumlist.DataSource = LegacyDb.forum_listread(
                this.PageContext.PageBoardID, this.PageContext.PageUserID, null, null, false, false);

            this.forumlist.DataValueField = "ForumID";
            this.forumlist.DataTextField = "Forum";

            this.DataBind();

            this.forumlist.Items.Insert(0, new ListItem(this.GetText("ADMIN_PRUNE", "ALL_FORUMS"), "0"));
        }

        /// <summary>
        /// The commit_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void commit_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            PruneTopicTask.Start(
              this.PageContext.PageBoardID,
              this.forumlist.SelectedValue.ToType<int>(),
              this.days.Text.ToType<int>(),
              this.permDeleteChkBox.Checked);

            this.PageContext.AddLoadMessage(this.GetText("ADMIN_PRUNE", "MSG_TASK"));
        }

        #endregion
    }
}