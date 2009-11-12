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

namespace YAF.Pages.Admin
{
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for bannedip.
  /// </summary>
  public partial class replacewords : AdminPage
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
        this.PageLinks.AddLink("Replace Words", string.Empty);

        BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.list.DataSource = DB.replace_words_list(PageContext.PageBoardID, null);
      DataBind();
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
      ((LinkButton) sender).Attributes["onclick"] = "return confirm('Delete this Word Replacement?')";
    }

    /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void list_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
      if (e.CommandName == "add")
      {
        YafBuildLink.Redirect(ForumPages.admin_replacewords_edit);
      }
      else if (e.CommandName == "edit")
      {
        YafBuildLink.Redirect(ForumPages.admin_replacewords_edit, "i={0}", e.CommandArgument);
      }
      else if (e.CommandName == "delete")
      {
        DB.replace_words_delete(e.CommandArgument);
        PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ReplaceWords));
        BindData();
      }
      else if (e.CommandName == "export")
      {
        DataTable replaceDT = DB.replace_words_list(PageContext.PageBoardID, null);
        replaceDT.DataSet.DataSetName = "YafReplaceWordsList";
        replaceDT.TableName = "YafReplaceWords";
        replaceDT.Columns.Remove("ID");
        replaceDT.Columns.Remove("BoardID");

        Response.ContentType = "text/xml";
        Response.AppendHeader("Content-Disposition", "attachment; filename=YafReplaceWordsExport.xml");
        replaceDT.DataSet.WriteXml(Response.OutputStream);
        Response.End();
      }
      else if (e.CommandName == "import")
      {
        YafBuildLink.Redirect(ForumPages.admin_replacewords_import);
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
      list.ItemCommand += new RepeaterCommandEventHandler(list_ItemCommand);

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