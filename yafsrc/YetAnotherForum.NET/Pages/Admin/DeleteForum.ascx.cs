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
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Tasks;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Administrative Page for the deleting of forum properties.
    /// </summary>
    public partial class DeleteForum : AdminPage
    {
        #region Methods

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlockStartup(
                "BlockUiFunctionJs",
                JavaScriptBlocks.BlockUiFunctionJs("DeleteForumMessage"));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.MoveTopics.CheckedChanged += this.MoveTopicsCheckedChanged;
            this.Delete.Click += this.SaveClick;
            this.Cancel.Click += Cancel_Click;

            this.Delete.ReturnConfirmText = this.GetText("ADMIN_FORUMS", "CONFIRM_DELETE");
            this.Delete.ReturnConfirmEvent = "blockUIMessage";

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

            this.BindData();

            if (!this.Get<HttpRequestBase>().QueryString.Exists("fa"))
            {
                BuildLink.Redirect(ForumPages.Admin_Forums);
            }

            var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa");

            var forum = this.GetRepository<Forum>().GetById(forumId.Value);

            if (forum == null)
            {
                BuildLink.Redirect(ForumPages.Admin_Forums);
            }
            else
            {
                this.IconHeader.Text = $"{this.GetText("HEADER1")}: <strong>{forum.Name}</strong>";

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
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), BuildLink.GetLink(ForumPages.Admin_Forums));
            this.PageLinks.AddLink(this.GetText("ADMIN_DELETEFORUM", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("TEAM", "FORUMS")} - {this.GetText("ADMIN_DELETEFORUM", "TITLE")}";
        }

        /// <summary>
        /// The update status timer_ tick.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UpdateStatusTimerTick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // see if the migration is done....
            if (this.Get<ITaskModuleManager>().TryGetTask(ForumDeleteTask.TaskName, out var task) && task.IsRunning)
            {
                // continue...
                return;
            }

            this.UpdateStatusTimer.Enabled = false;

            // rebind...
            this.BindData();

            // clear caches...
            this.ClearCaches();

            BuildLink.Redirect(ForumPages.Admin_Forums);
        }

        /// <summary>
        /// Cancel Deleting and Redirecting back to The Admin Forums Page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Admin_Forums);
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
            this.ForumList.DataSource = this.GetRepository<Forum>().List(this.PageContext.PageBoardID, null);

            this.ForumList.DataValueField = "ID";
            this.ForumList.DataTextField = "Name";
            this.ForumList.DataBind();

            this.ForumList.Items.Cast<ListItem>().ForEach(
                item => item.Attributes.Add(
                    "data-content",
                    $"<span class=\"select2-image-select-icon\"><i class=\"fas fa-comments fa-fw text-secondary\"></i><span><span>&nbsp;{item.Text}</span>"));
        }

        /// <summary>
        /// Handles the CheckedChanged event of the MoveTopics control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MoveTopicsCheckedChanged(object sender, EventArgs e)
        {
            this.ForumList.Enabled = this.MoveTopics.Checked;

            this.BindParentList();
        }

        /// <summary>
        /// Clears the caches.
        /// </summary>
        private void ClearCaches()
        {
            // clear moderators cache
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
                var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa");

                // schedule...
                ForumDeleteTask.Start(
                    this.PageContext.PageBoardID,
                    forumId.Value,
                    this.ForumList.SelectedValue.ToType<int>(),
                    out errorMessage);

                // enable timer...
                this.UpdateStatusTimer.Enabled = true;

                this.LocalizedLabel6.LocalizedTag = "DELETE_MOVE_TITLE";
            }
            else
            {
                // Simply Delete the Forum with all of its Content
                var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa");

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