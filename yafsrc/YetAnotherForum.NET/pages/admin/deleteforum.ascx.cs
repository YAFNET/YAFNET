/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;

    using YAF.Classes;
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
            this.MoveTopics.CheckedChanged += this.MoveTopics_CheckedChanged;
            this.Delete.Click += this.Save_Click;
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
            this.PageContext.PageElements.RegisterJQuery();
            this.PageContext.PageElements.RegisterJsResourceInclude("blockUIJs", "js/jquery.blockUI.js");

            if (this.IsPostBack)
            {
                return;
            }

            this.LoadingImage.ImageUrl = YafForumInfo.GetURLToResource("images/loader.gif");

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), YafBuildLink.GetLink(ForumPages.admin_forums));
            this.PageLinks.AddLink(this.GetText("ADMIN_DELETEFORUM", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("TEAM", "FORUMS"),
                this.GetText("ADMIN_DELETEFORUM", "TITLE"));

            this.Delete.Text = this.GetText("ADMIN_DELETEFORUM", "DELETE_FORUM");
            this.Cancel.Text = this.GetText("CANCEL");

            this.Delete.Attributes["onclick"] =
                "return (confirm('{0}') && confirm('{1}'));".FormatWith(
                    this.GetText("ADMIN_FORUMS", "CONFIRM_DELETE"),
                    this.GetText("ADMIN_FORUMS", "CONFIRM_DELETE_POSITIVE"));

            this.BindData();

            var forumId = this.GetQueryStringAsInt("fa");

            using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, forumId.Value))
            {
                DataRow row = dt.Rows[0];

                this.ForumNameTitle.Text = (string)row["Name"];

                // populate parent forums list with forums according to selected category
                this.BindParentList();
            }
        }

        /// <summary>
        /// The update status timer_ tick.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UpdateStatusTimer_Tick([NotNull] object sender, [NotNull] EventArgs e)
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
        private void MoveTopics_CheckedChanged(object sender, EventArgs e)
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
        private void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var errorMessage = string.Empty;

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

                // show blocking ui...
                this.PageContext.PageElements.RegisterJsBlockStartup(
                    "BlockUIExecuteJs", JavaScriptBlocks.BlockUIExecuteJs("DeleteForumMessage"));
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

                // show blocking ui...
                this.PageContext.PageElements.RegisterJsBlockStartup(
                    "BlockUIExecuteJs", JavaScriptBlocks.BlockUIExecuteJs("DeleteForumMessage"));
            }

            // show blocking ui...
            this.PageContext.PageElements.RegisterJsBlockStartup(
                "BlockUIExecuteJs", JavaScriptBlocks.BlockUIExecuteJs("DeleteForumMessage"));

            // TODO : Handle Error Message?!
        }

        #endregion
    }
}