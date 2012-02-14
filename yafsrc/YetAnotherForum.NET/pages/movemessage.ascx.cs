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

namespace YAF.Pages
{
    #region Using

    using System;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Summary description for movetopic.
    /// </summary>
    public partial class movemessage : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "movemessage" /> class.
        /// </summary>
        public movemessage()
            : base("MOVEMESSAGE")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create and move_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CreateAndMove_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.TopicSubject.Text != string.Empty)
            {
                long nTopicId = LegacyDb.topic_create_by_message(
                  this.Request.QueryString.GetFirstOrDefault("m"), this.ForumList.SelectedValue, this.TopicSubject.Text);
                LegacyDb.message_move(this.Request.QueryString.GetFirstOrDefault("m"), nTopicId, true);
                YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
            }
            else
            {
                this.PageContext.AddLoadMessage(this.GetText("Empty_Topic"));
            }
        }

        /// <summary>
        /// The forum list_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ForumList_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.TopicsList.DataSource = LegacyDb.topic_list(this.ForumList.SelectedValue, null, DateTime.MinValue.AddYears(1754),DateTime.UtcNow, 0, 32762, false, false, false);
            this.TopicsList.DataTextField = "Subject";
            this.TopicsList.DataValueField = "TopicID";
            this.TopicsList.DataBind();
            this.TopicsList_SelectedIndexChanged(this.ForumList, e);
            this.CreateAndMove.Enabled = this.ForumList.SelectedValue.ToType<int>() <= 0 ? false : true;
        }

        /// <summary>
        /// The move_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Move_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.TopicsList.SelectedValue.ToType<int>() != this.PageContext.PageTopicID)
            {
                LegacyDb.message_move(this.Request.QueryString.GetFirstOrDefault("m"), this.TopicsList.SelectedValue, true);
            }

            YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
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
            if (this.Request.QueryString.GetFirstOrDefault("m") == null || !this.PageContext.ForumModeratorAccess)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

            this.PageLinks.AddLink(
                this.PageContext.PageCategoryName,
                YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
            this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
            this.PageLinks.AddLink(
                this.PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID));

            this.PageLinks.AddLink(this.GetText("MOVE_MESSAGE"));

            this.Move.Text = this.GetText("MOVE_MESSAGE");
            this.Move.ToolTip = this.GetText("MOVE_TITLE");
            this.CreateAndMove.Text = this.GetText("CREATE_TOPIC");
            this.CreateAndMove.ToolTip = this.GetText("SPLIT_TITLE");

            this.ForumList.DataSource = LegacyDb.forum_listall_sorted(this.PageContext.PageBoardID, this.PageContext.PageUserID);
            this.ForumList.DataTextField = "Title";
            this.ForumList.DataValueField = "ForumID";
            this.DataBind();

            this.ForumList.Items.FindByValue(this.PageContext.PageForumID.ToString()).Selected = true;
            this.ForumList_SelectedIndexChanged(this.ForumList, e);
        }

        /// <summary>
        /// The topics list_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void TopicsList_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Move.Enabled = this.TopicsList.SelectedValue != string.Empty;
        }

        #endregion
    }
}