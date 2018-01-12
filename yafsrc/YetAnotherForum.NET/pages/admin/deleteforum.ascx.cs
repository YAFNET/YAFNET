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

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Tasks;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Administrative Page for the deleting of forum properties.
    /// </summary>
    public partial class deleteforum : AdminPage
    {
        #region Methods

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlockStartup(
                "BlockUIExecuteJs",
                JavaScriptBlocks.BlockUIExecuteJs("DeleteForumMessage", this.Delete.ClientID));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Get query string as integer.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The get query string as integer
        /// </returns>
        protected int? GetQueryStringAsInt([NotNull] string name)
        {
            int value;

            if (this.Request.QueryString.GetFirstOrDefault(name) != null
                && int.TryParse(this.Request.QueryString.GetFirstOrDefault(name), out value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.MoveTopics.CheckedChanged += this.MoveTopicsCheckedChanged;
            this.Delete.Click += this.SaveClick;
            this.Cancel.Click += this.Cancel_Click;

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.LoadingImage.ImageUrl = YafForumInfo.GetURLToContent("images/loader.gif");

            this.BindData();

            var forumId = this.GetQueryStringAsInt("fa");

            using (var dt = LegacyDb.forum_list(this.PageContext.PageBoardID, forumId.Value))
            {
                var row = dt.Rows[0];

                this.ForumNameTitle.Text = (string)row["Name"];

                // populate parent forums list with forums according to selected category
                this.BindParentList();
            }
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), YafBuildLink.GetLink(ForumPages.admin_forums));
            this.PageLinks.AddLink(this.GetText("ADMIN_DELETEFORUM", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("TEAM", "FORUMS"),
                this.GetText("ADMIN_DELETEFORUM", "TITLE"));
        }

        /// <summary>
        /// The update status timer_ tick.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UpdateStatusTimerTick([NotNull] object sender, [NotNull] EventArgs e)
        {
            IBackgroundTask task;

            // see if the migration is done....
            if (this.Get<ITaskModuleManager>().TryGetTask(ForumDeleteTask.TaskName, out task) && task.IsRunning)
            {
                // continue...
                return;
            }

            this.UpdateStatusTimer.Enabled = false;

            // rebind...
            this.BindData();

            // clear caches...
            this.ClearCaches();

            YafBuildLink.Redirect(ForumPages.admin_forums);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            // Load forum's combo
            this.BindParentList();
        }

        /// <summary>
        /// Binds the parent list.
        /// </summary>
        private void BindParentList()
        {
            this.ForumList.DataSource = LegacyDb.forum_listall(
                this.PageContext.PageBoardID, this.PageContext.PageUserID);

            this.ForumList.DataValueField = "ForumID";
            this.ForumList.DataTextField = "Forum";
            this.ForumList.DataBind();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the MoveTopics control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MoveTopicsCheckedChanged(object sender, EventArgs e)
        {
            if (this.MoveTopics.Checked)
            {
                this.Delete.Attributes.Remove("onclick");
            }
            else
            {
                this.Delete.Attributes["onclick"] =
                    "return (confirm('{0}') && confirm('{1}'));".FormatWith(
                        this.GetText("ADMIN_FORUMS", "CONFIRM_DELETE"),
                        this.GetText("ADMIN_FORUMS", "CONFIRM_DELETE_POSITIVE"));
            }

            this.ForumList.Enabled = this.MoveTopics.Checked;
        }

        /// <summary>
        /// Cancel Deleting and Redirecting back to The Admin Forums Page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_forums);
        }

        /// <summary>
        /// Clears the caches.
        /// </summary>
        private void ClearCaches()
        {
            // clear moderatorss cache
            this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);

            // clear category cache...
            this.Get<IDataCache>().Remove(Constants.Cache.ForumCategory);

            // clear active discussions cache..
            this.Get<IDataCache>().Remove(Constants.Cache.ForumActiveDiscussions);
        }

        /// <summary>
        /// Delete The Forum
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            string errorMessage;

            if (this.MoveTopics.Checked)
            {
                // Simply Delete the Forum with all of its Content
                var forumId = this.GetQueryStringAsInt("fa");

                // schedule...
                ForumDeleteTask.Start(
                    this.PageContext.PageBoardID, forumId.Value, this.ForumList.SelectedValue.ToType<int>(), out errorMessage);

                // enable timer...
                this.UpdateStatusTimer.Enabled = true;

                this.LocalizedLabel6.LocalizedTag = "DELETE_MOVE_TITLE";
            }
            else
            {
                // Simply Delete the Forum with all of its Content
                var forumId = this.GetQueryStringAsInt("fa");

                // schedule...
                ForumDeleteTask.Start(this.PageContext.PageBoardID, forumId.Value, out errorMessage);

                // enable timer...
                this.UpdateStatusTimer.Enabled = true;

                this.LocalizedLabel6.LocalizedTag = "DELETE_TITLE";
            }

            if (errorMessage.IsSet())
            {
                this.PageContext.AddLoadMessage(errorMessage, MessageTypes.danger);
            }
        }

        #endregion
    }
}