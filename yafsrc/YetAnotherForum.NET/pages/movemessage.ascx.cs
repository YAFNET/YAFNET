/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for movetopic.
  /// </summary>
  public partial class movemessage : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="movemessage"/> class.
    /// </summary>
    public movemessage()
      : base("MOVEMESSAGE")
    {
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
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request.QueryString.GetFirstOrDefault("m") == null || !PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied();
      }

      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));
        this.PageLinks.AddForumLinks(PageContext.PageForumID);
        this.PageLinks.AddLink(PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", PageContext.PageTopicID));

        this.Move.Text = GetText("MOVE_MESSAGE");
        this.CreateAndMove.Text = GetText("CREATE_TOPIC");

        this.ForumList.DataSource = DB.forum_listall_sorted(PageContext.PageBoardID, PageContext.PageUserID);
        this.ForumList.DataTextField = "Title";
        this.ForumList.DataValueField = "ForumID";
        DataBind();
        this.ForumList.Items.FindByValue(PageContext.PageForumID.ToString()).Selected = true;
        ForumList_SelectedIndexChanged(this.ForumList, e);
      }
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
    protected void Move_Click(object sender, EventArgs e)
    {
      if (Convert.ToInt32(this.TopicsList.SelectedValue) != PageContext.PageTopicID)
      {
        DB.message_move(Request.QueryString.GetFirstOrDefault("m"), this.TopicsList.SelectedValue, true);
      }

      YafBuildLink.Redirect(ForumPages.topics, "f={0}", PageContext.PageForumID);
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
    protected void ForumList_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.TopicsList.DataSource = DB.topic_list(this.ForumList.SelectedValue, null, 0, null, 0, 32762,false);
      this.TopicsList.DataTextField = "Subject";
      this.TopicsList.DataValueField = "TopicID";
      this.TopicsList.DataBind();
      TopicsList_SelectedIndexChanged(this.ForumList, e);
      this.CreateAndMove.Enabled = Convert.ToInt32(this.ForumList.SelectedValue) <= 0 ? false : true;
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
    protected void TopicsList_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.TopicsList.SelectedValue == string.Empty)
      {
        this.Move.Enabled = false;
      }
      else
      {
        this.Move.Enabled = true;
      }
    }

    /// <summary>
    /// The create and move_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CreateAndMove_Click(object sender, EventArgs e)
    {
      if (this.TopicSubject.Text != string.Empty)
      {
        long nTopicId = DB.topic_create_by_message(Request.QueryString.GetFirstOrDefault("m"), this.ForumList.SelectedValue, this.TopicSubject.Text);
        DB.message_move(Request.QueryString.GetFirstOrDefault("m"), nTopicId, true);
        YafBuildLink.Redirect(ForumPages.topics, "f={0}", PageContext.PageForumID);
      }
      else
      {
        PageContext.AddLoadMessage(GetText("EmptyTopic"));
      }
    }
  }
}