/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
  // YAF.Pages
  using System;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for movetopic.
  /// </summary>
  public partial class movetopic : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="movetopic"/> class.
    /// </summary>
    public movetopic()
      : base("MOVETOPIC")
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
      if (Request.QueryString["t"] == null || !PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied();
      }

      if (!IsPostBack)
      {
        if (PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));
        }

        this.PageLinks.AddForumLinks(PageContext.PageForumID);
        this.PageLinks.AddLink(PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", PageContext.PageTopicID));

        this.Move.Text = GetText("move");

        // Ederon : 7/14/2007 - by default, leave pointer is set on value defined on host level
        this.LeavePointer.Checked = PageContext.BoardSettings.ShowMoved;

        this.ForumList.DataSource = DB.forum_listall_sorted(PageContext.PageBoardID, PageContext.PageUserID);

        DataBind();

        ListItem pageItem = this.ForumList.Items.FindByValue(PageContext.PageForumID.ToString());
        if (pageItem != null)
        {
          pageItem.Selected = true;
        }
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
      if (Convert.ToInt32(this.ForumList.SelectedValue) <= 0)
      {
        PageContext.AddLoadMessage(GetText("CANNOT_MOVE_TO_CATEGORY"));
        return;
      }

      // only move if it's a destination is a different forum.
      if (Convert.ToInt32(this.ForumList.SelectedValue) != PageContext.PageForumID)
      {
        // Ederon : 7/14/2007
        DB.topic_move(PageContext.PageTopicID, this.ForumList.SelectedValue, this.LeavePointer.Checked);
      }

      YafBuildLink.Redirect(ForumPages.topics, "f={0}", PageContext.PageForumID);
    }

    #region Web Form Designer generated code

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    #endregion
  }
}