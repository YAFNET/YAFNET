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