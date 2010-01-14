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
  // YAF.Pages
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Controls;

  /// <summary>
  /// Summary description for moderate.
  /// </summary>
  public partial class moderate0 : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="moderate0"/> class.
    /// </summary>
    public moderate0()
      : base("MODERATE")
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
      if (!PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied();
      }

      if (!IsPostBack)
      {
        this.AddUser.Text = GetText("INVITE");

        if (PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));
        }

        this.PageLinks.AddForumLinks(PageContext.PageForumID);
        this.PageLinks.AddLink(GetText("TITLE"), string.Empty);

        this.PagerTop.PageSize = 25;
      }

      BindData();
    }

    /// <summary>
    /// The add user_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void AddUser_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.mod_forumuser, "f={0}", PageContext.PageForumID);
    }

    /// <summary>
    /// The delete_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Delete_Load(object sender, EventArgs e)
    {
      ((ThemeButton) sender).Attributes["onclick"] = String.Format("return confirm('{0}')", GetText("confirm_delete"));
    }

    /// <summary>
    /// The delete user_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteUser_Load(object sender, EventArgs e)
    {
      ((LinkButton) sender).Attributes["onclick"] = String.Format("return confirm('{0}')", "Remove this user from this forum?");
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      var pds = new PagedDataSource();
      pds.AllowPaging = true;
      pds.PageSize = this.PagerTop.PageSize;

      DataTable dt = DB.topic_list(PageContext.PageForumID, null, -1, null, 0, 999999,false);
      DataView dv = dt.DefaultView;

      this.PagerTop.Count = dv.Count;
      pds.DataSource = dv;

      pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;
      if (pds.CurrentPageIndex >= pds.PageCount)
      {
        pds.CurrentPageIndex = pds.PageCount - 1;
      }

      this.topiclist.DataSource = pds;
      this.UserList.DataSource = DB.userforum_list(null, PageContext.PageForumID);
      DataBind();
    }

    /// <summary>
    /// The topiclist_ item command.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void topiclist_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
      if (e.CommandName == "delete")
      {
        DB.topic_delete(e.CommandArgument);
        PageContext.AddLoadMessage(GetText("deleted"));
        BindData();
      }
    }

    /// <summary>
    /// The user list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void UserList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":
          YafBuildLink.Redirect(ForumPages.mod_forumuser, "f={0}&u={1}", PageContext.PageForumID, e.CommandArgument);
          break;
        case "remove":
          DB.userforum_delete(e.CommandArgument, PageContext.PageForumID);
          BindData();

          // clear moderatorss cache
          PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));
          break;
      }
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PagerTop_PageChange(object sender, EventArgs e)
    {
      // rebind
      BindData();
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
      topiclist.ItemCommand += new RepeaterCommandEventHandler(topiclist_ItemCommand);
      UserList.ItemCommand += new RepeaterCommandEventHandler(this.UserList_ItemCommand);
      AddUser.Click += new EventHandler(AddUser_Click);

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