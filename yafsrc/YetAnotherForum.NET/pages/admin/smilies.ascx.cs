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
  using YAF.Classes.Core.BBCode;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for smilies.
  /// </summary>
  public partial class smilies : AdminPage
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
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Smilies", string.Empty);

        BindData();
      }
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
      ((LinkButton) sender).Attributes["onclick"] = "return confirm('Delete this smiley?')";
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Pager_PageChange(object sender, EventArgs e)
    {
      BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.Pager.PageSize = 25;
      DataView dv = DB.smiley_list(PageContext.PageBoardID, null).DefaultView;
      this.Pager.Count = dv.Count;
      var pds = new PagedDataSource();
      pds.DataSource = dv;
      pds.AllowPaging = true;
      pds.CurrentPageIndex = this.Pager.CurrentPageIndex;
      pds.PageSize = this.Pager.PageSize;
      this.List.DataSource = pds;
      DataBind();
    }

    /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void List_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "add":
          YafBuildLink.Redirect(ForumPages.admin_smilies_edit);
          break;
        case "edit":
          YafBuildLink.Redirect(ForumPages.admin_smilies_edit, "s={0}", e.CommandArgument);
          break;
        case "moveup":
          DB.smiley_resort(PageContext.PageBoardID, e.CommandArgument, -1);

          // invalidate the cache...
          PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.Smilies));
          BindData();
          ReplaceRulesCreator.ClearCache();
          break;
        case "movedown":
          DB.smiley_resort(PageContext.PageBoardID, e.CommandArgument, 1);

          // invalidate the cache...
          PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.Smilies));
          BindData();
          ReplaceRulesCreator.ClearCache();
          break;
        case "delete":
          DB.smiley_delete(e.CommandArgument);

          // invalidate the cache...
          PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.Smilies));
          BindData();
          ReplaceRulesCreator.ClearCache();
          break;
        case "import":
          YafBuildLink.Redirect(ForumPages.admin_smilies_import);
          break;
      }
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
      this.Pager.PageChange += new EventHandler(Pager_PageChange);
      this.List.ItemCommand += new RepeaterCommandEventHandler(this.List_ItemCommand);

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