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
  #region Using

  using System;
  using System.Data;
  using System.Linq;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Controls;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for moderate.
  /// </summary>
  public partial class moderate0 : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "moderate0" /> class.
    /// </summary>
    public moderate0()
      : base("MODERATE")
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
      var pds = new PagedDataSource { AllowPaging = true, PageSize = this.PagerTop.PageSize };

      DataTable dt = DB.topic_list(
        this.PageContext.PageForumID, 
        null, 
        -1, 
        null, 
        this.PagerTop.CurrentPageIndex * pds.PageSize, 
        pds.PageSize, 
        false, 
        true);
      DataView dv = dt.DefaultView;

      pds.DataSource = dv;

      pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;
      if (pds.CurrentPageIndex >= pds.PageCount)
      {
        pds.CurrentPageIndex = pds.PageCount - 1;
      }

      int rowCount = 0;
      if (dt.Rows.Count > 0)
      {
        rowCount = (int)dt.Rows[0]["RowCount"];
      }

      this.topiclist.DataSource = pds;
      this.UserList.DataSource = DB.userforum_list(null, this.PageContext.PageForumID);
      this.DataBind();

      this.PagerTop.Count = rowCount;
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

      list.ForEach(x => DB.topic_delete(x.TopicRowID));

      this.PageContext.AddLoadMessage(this.GetText("deleted"));
      this.BindData();
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
    protected void DeleteUser_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((LinkButton)sender).Attributes["onclick"] =
        "return confirm('{0}')".FormatWith("Remove this user from this forum?");
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
    protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((ThemeButton)sender).Attributes["onclick"] = "return confirm('{0}')".FormatWith(this.GetText("confirm_delete"));
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
      if (!this.PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied();
      }

      if (!this.IsPostBack)
      {
        this.AddUser.Text = this.GetText("INVITE");

        if (this.PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(
            this.PageContext.PageCategoryName, 
            YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
        }

        this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        this.PagerTop.PageSize = 25;
      }

      this.BindData();
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
    protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
      // rebind
      this.BindData();
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
    protected void UserList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":
          YafBuildLink.Redirect(
            ForumPages.mod_forumuser, "f={0}&u={1}", this.PageContext.PageForumID, e.CommandArgument);
          break;
        case "remove":
          DB.userforum_delete(e.CommandArgument, this.PageContext.PageForumID);
          this.BindData();

          // clear moderatorss cache
          this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));
          break;
      }
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
    protected void topiclist_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
    {
      if (e.CommandName == "delete")
      {
        DB.topic_delete(e.CommandArgument);
        this.PageContext.AddLoadMessage(this.GetText("deleted"));
        this.BindData();
      }
    }

    #endregion
  }
}