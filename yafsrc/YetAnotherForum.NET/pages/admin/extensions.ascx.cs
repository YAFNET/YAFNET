/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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
  using YAF.Core.Extensions;
  using YAF.Core.Model;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Types.Models;
  using YAF.Utils;
  using YAF.Utils.Helpers;

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
        ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_EXTENSIONS", "CONFIRM_DELETE"));
    }

    /// <summary>
    /// Add Localized Text to Button
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void addLoad(object sender, EventArgs e)
    {
        var add = (Button)sender;
        add.Text = this.GetText("ADMIN_EXTENSIONS", "ADD");
    }

    /// <summary>
    /// Add Localized Text to Button
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void exportLoad(object sender, EventArgs e)
    {
        var export = (Button)sender;
        export.Text = this.GetText("ADMIN_EXTENSIONS", "EXPORT");
    }

    /// <summary>
    /// Add Localized Text to Button
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void importLoad(object sender, EventArgs e)
    {
        var import = (Button)sender;
        import.Text = this.GetText("ADMIN_EXTENSIONS", "IMPORT");
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
        ((Label)sender).Text = "{0} {1}".FormatWith(
            this.PageContext.BoardSettings.FileExtensionAreAllowed
                ? this.GetText("COMMON", "ALLOWED")
                : this.GetText("COMMON", "DISALLOWED"),
                this.GetText("ADMIN_EXTENSIONS", "TITLE"));
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this.list.ItemCommand += this.list_ItemCommand;

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      this.InitializeComponent();
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
        if (this.IsPostBack)
        {
            return;
        }

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_EXTENSIONS", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1}".FormatWith(
              this.GetText("ADMIN_ADMIN", "Administration"),
              this.GetText("ADMIN_EXTENSIONS", "TITLE"));

        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.list.DataSource = this.GetRepository<FileExtension>().List();
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
        switch (e.CommandName)
        {
            case "add":
                YafBuildLink.Redirect(ForumPages.admin_extensions_edit);
                break;
            case "edit":
                YafBuildLink.Redirect(ForumPages.admin_extensions_edit, "i={0}", e.CommandArgument);
                break;
            case "delete":
                this.GetRepository<FileExtension>().DeleteByID(e.CommandArgument.ToType<int>());
                this.BindData();
                break;
            case "export":
                {
                    // export this list as XML...
                    var extensionList = this.GetRepository<FileExtension>().List();
                    extensionList.DataSet.DataSetName = "YafExtensionList";
                    extensionList.TableName = "YafExtension";
                    extensionList.Columns.Remove("ExtensionID");
                    extensionList.Columns.Remove("BoardID");

                    this.Response.ContentType = "text/xml";
                    this.Response.AppendHeader("Content-Disposition", "attachment; filename=YafExtensionExport.xml");
                    extensionList.DataSet.WriteXml(this.Response.OutputStream);
                    this.Response.End();
                }

                break;
            case "import":
                YafBuildLink.Redirect(ForumPages.admin_extensions_import);
                break;
        }
    }

      #endregion
  }
}