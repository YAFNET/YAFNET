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
  #region Using

  using System;
  using System.Data;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for bannedip.
  /// </summary>
  public partial class extensions : AdminPage
  {
    #region Methods

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
      ((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this Extension?')";
    }

    /// <summary>
    /// The extension title_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ExtensionTitle_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((Label)sender).Text = (this.PageContext.BoardSettings.FileExtensionAreAllowed ? "Allowed" : "Disallowed") +
                             " File Extensions";
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      list.ItemCommand += this.list_ItemCommand;

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
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
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);
        this.PageLinks.AddLink("File Extensions", string.Empty);

        this.BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.list.DataSource = LegacyDb.extension_list(this.PageContext.PageBoardID);
      this.DataBind();
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
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
    private void list_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
    {
      if (e.CommandName == "add")
      {
        YafBuildLink.Redirect(ForumPages.admin_extensions_edit);
      }
      else if (e.CommandName == "edit")
      {
        YafBuildLink.Redirect(ForumPages.admin_extensions_edit, "i={0}", e.CommandArgument);
      }
      else if (e.CommandName == "delete")
      {
        LegacyDb.extension_delete(e.CommandArgument);
        this.BindData();
      }
      else if (e.CommandName == "export")
      {
        // export this list as XML...
        DataTable extensionList = LegacyDb.extension_list(this.PageContext.PageBoardID);
        extensionList.DataSet.DataSetName = "YafExtensionList";
        extensionList.TableName = "YafExtension";
        extensionList.Columns.Remove("ExtensionID");
        extensionList.Columns.Remove("BoardID");

        this.Response.ContentType = "text/xml";
        this.Response.AppendHeader("Content-Disposition", "attachment; filename=YafExtensionExport.xml");
        extensionList.DataSet.WriteXml(this.Response.OutputStream);
        this.Response.End();
      }
      else if (e.CommandName == "import")
      {
        YafBuildLink.Redirect(ForumPages.admin_extensions_import);
      }
    }

    #endregion
  }
}