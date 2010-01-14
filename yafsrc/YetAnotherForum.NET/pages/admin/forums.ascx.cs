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

namespace YAF.Pages.Admin
{
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Utilities;

  /// <summary>
  /// Summary description for forums.
  /// </summary>
  public partial class forums : AdminPage
  {
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
      PageContext.PageElements.RegisterJQuery();
      PageContext.PageElements.RegisterJsResourceInclude("blockUIJs", "js/jquery.blockUI.js");

      if (!IsPostBack)
      {
        this.LoadingImage.ImageUrl = YafForumInfo.GetURLToResource("images/loading-white.gif");

        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Forums", string.Empty);

        BindData();
      }
    }

    /// <summary>
    /// The delete category_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteCategory_Load(object sender, EventArgs e)
    {
      ((LinkButton) sender).Attributes["onclick"] = "return confirm('Delete this category?')";
    }

    /// <summary>
    /// The delete forum_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteForum_Load(object sender, EventArgs e)
    {
      ((LinkButton) sender).Attributes["onclick"] =
        "return (confirm('Permanently delete this Forum including ALL topics, polls, attachments and messages associated with it?') && confirm('Are you POSITIVE?'));";
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      using (DataSet ds = DB.ds_forumadmin(PageContext.PageBoardID))
      {
        this.CategoryList.DataSource = ds.Tables[YafDBAccess.GetObjectName("Category")];
      }

      DataBind();
    }

    /// <summary>
    /// The forum list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForumList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":
          YafBuildLink.Redirect(ForumPages.admin_editforum, "f={0}", e.CommandArgument);
          break;
        case "delete":

          // schedule...
          ForumDeleteTask.Start(PageContext.PageBoardID, Convert.ToInt32(e.CommandArgument));

          // enable timer...
          this.UpdateStatusTimer.Enabled = true;

          // show blocking ui...
          PageContext.PageElements.RegisterJsBlockStartup("BlockUIExecuteJs", JavaScriptBlocks.BlockUIExecuteJs("DeleteForumMessage "));
          break;
      }
    }

    /// <summary>
    /// The clear caches.
    /// </summary>
    private void ClearCaches()
    {
      // clear moderatorss cache
      PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));

      // clear category cache...
      PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumCategory));

      // clear active discussions cache..
      PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumActiveDiscussions));
    }

    /// <summary>
    /// The new forum_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void NewForum_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_editforum);
    }

    /// <summary>
    /// The category list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void CategoryList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":
          YafBuildLink.Redirect(ForumPages.admin_editcategory, "c={0}", e.CommandArgument);
          break;
        case "delete":
          if (DB.category_delete(e.CommandArgument))
          {
            BindData();
            ClearCaches();
          }
          else
          {
            PageContext.AddLoadMessage(
              "You cannot delete this Category as it has at least one forum assigned to it.\nTo move forums click on \"Edit\" and change the category the forum is assigned to.");
          }

          break;
      }
    }

    /// <summary>
    /// The new category_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void NewCategory_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_editcategory);
    }

    /// <summary>
    /// The update status timer_ tick.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UpdateStatusTimer_Tick(object sender, EventArgs e)
    {
      // see if the migration is done....
      if (YafTaskModule.Current.TaskManager.ContainsKey(ForumDeleteTask.TaskName) && YafTaskModule.Current.TaskManager[ForumDeleteTask.TaskName].IsRunning)
      {
        // continue...
        return;
      }

      this.UpdateStatusTimer.Enabled = false;

      // rebind...
      BindData();

      // clear caches...
      ClearCaches();

      // done here...
      YafBuildLink.Redirect(ForumPages.admin_forums);
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
      this.CategoryList.ItemCommand += new RepeaterCommandEventHandler(this.CategoryList_ItemCommand);
    }

    #endregion
  }
}