/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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

using YAF.Utils.Helpers;

namespace YAF.Pages
{
    // YAF.Pages

    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Forum Moderating Page.
    /// </summary>
    public partial class moderating : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "moderating" /> class.
        /// </summary>
        public moderating()
            : base("MODERATING")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add user_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void AddUser_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.mod_forumuser, "f={0}", this.PageContext.PageForumID);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        protected void BindData()
        {
            this.PagerTop.PageSize = this.Get<YafBoardSettings>().TopicsPerPage;
            int baseSize = this.Get<YafBoardSettings>().TopicsPerPage;
            int nCurrentPageIndex = this.PagerTop.CurrentPageIndex;
            DataTable dt = LegacyDb.topic_list(
                this.PageContext.PageForumID,
                null,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                nCurrentPageIndex,
                baseSize,
                false,
                true,
                false);

            this.topiclist.DataSource = dt;
            this.UserList.DataSource = LegacyDb.userforum_list(null, this.PageContext.PageForumID);
            this.DataBind();

            if (dt != null && dt.Rows.Count > 0)
            {
                this.PagerTop.Count = dt.AsEnumerable().First().Field<int>("TotalRows");
            }
        }

        /// <summary>
        /// The delete topics_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void DeleteTopics_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var list =
                this.topiclist.Controls.OfType<RepeaterItem>().SelectMany(x => x.Controls.OfType<TopicLine>()).Where(
                    x => x.IsSelected && x.TopicRowID.HasValue).ToList();

            list.ForEach(x => LegacyDb.topic_delete(x.TopicRowID));

            this.PageContext.AddLoadMessage(this.GetText("moderate", "deleted"));
            this.BindData();
        }

        /// <summary>
        /// The delete user_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteUser_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("moderate", "confirm_delete_user"));
        }

        /// <summary>
        /// The delete_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("moderate", "confirm_delete"));
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                YafBuildLink.AccessDenied();
            }

            if (!this.IsPostBack)
            {
                this.AddUser.Text = this.GetText("MODERATE", "INVITE");

                if (this.PageContext.Settings.LockedForum == 0)
                {
                    this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
                    this.PageLinks.AddLink(
                        this.PageContext.PageCategoryName,
                        YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
                }

                this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
                this.PageLinks.AddLink(this.GetText("MODERATE", "TITLE"), string.Empty);

                this.PagerTop.PageSize = 25;
            }

            this.BindData();
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// The user list_ item command.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void UserList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    YafBuildLink.Redirect(
                        ForumPages.mod_forumuser, "f={0}&u={1}", this.PageContext.PageForumID, e.CommandArgument);
                    break;
                case "remove":
                    LegacyDb.userforum_delete(e.CommandArgument, this.PageContext.PageForumID);
                    this.BindData();

                    // clear moderatorss cache
                    this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);
                    break;
            }
        }

        /// <summary>
        /// The topiclist_ item command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void topiclist_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "delete":
                    LegacyDb.topic_delete(e.CommandArgument);
                    this.PageContext.AddLoadMessage(this.GetText("deleted"));
                    this.BindData();
                    break;
            }
        }

        #endregion
    }
}