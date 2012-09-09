/* Yet Another Forum.NET
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
namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

    #endregion

  /// <summary>
  /// The bbcode.
  /// </summary>
  public partial class bbcode : AdminPage
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
        ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_BBCODE", "CONFIRM_DELETE"));
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
        add.Text = this.GetText("ADMIN_BBCODE", "ADD");
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
        export.Text = this.GetText("ADMIN_BBCODE", "EXPORT");
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
        import.Text = this.GetText("ADMIN_BBCODE", "IMPORT");
    }

    /// <summary>
    /// The get selected bb code i ds.
    /// </summary>
    /// <returns>
    /// The Id of the BB Code
    /// </returns>
    [NotNull]
    protected List<int> GetSelectedBBCodeIDs()
    {
        // get checked items....
        return (from RepeaterItem item in this.bbCodeList.Items
                let sel = (CheckBox)item.FindControl("chkSelected")
                where sel.Checked
                select (HiddenField)item.FindControl("hiddenBBCodeID") into hiddenId 
                select hiddenId.Value.ToType<int>()).ToList();
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
          this.PageLinks.AddLink(this.GetText("ADMIN_BBCODE", "TITLE"), string.Empty);

          this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_BBCODE", "TITLE"));

          this.BindData();
    }

    /// <summary>
    /// The bb code list_ item command.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void bbCodeList_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "add":
                YafBuildLink.Redirect(ForumPages.admin_bbcode_edit);
                break;
            case "edit":
                YafBuildLink.Redirect(ForumPages.admin_bbcode_edit, "b={0}", e.CommandArgument);
                break;
            case "delete":
                LegacyDb.bbcode_delete(e.CommandArgument);
                this.Get<IDataCache>().Remove(Constants.Cache.CustomBBCode);
                this.BindData();
                break;
            case "export":
                {
                    List<int> bbCodeIds = this.GetSelectedBBCodeIDs();

                    if (bbCodeIds.Count > 0)
                    {
                        // export this list as XML...
                        DataTable dtBBCode = LegacyDb.bbcode_list(this.PageContext.PageBoardID, null);

                        // remove all but required bbcodes...
                        foreach (DataRow row in
                            from DataRow row in dtBBCode.Rows
                            let id = row["BBCodeID"].ToType<int>()
                            where !bbCodeIds.Contains(id)
                            select row)
                        {
                            // remove from this table...
                            row.Delete();
                        }

                        // store delete changes...
                        dtBBCode.AcceptChanges();

                        // export...
                        dtBBCode.DataSet.DataSetName = "YafBBCodeList";
                        dtBBCode.TableName = "YafBBCode";
                        dtBBCode.Columns.Remove("BBCodeID");
                        dtBBCode.Columns.Remove("BoardID");

                        this.Response.ContentType = "text/xml";
                        this.Response.AppendHeader("Content-Disposition", "attachment; filename=YafBBCodeExport.xml");
                        dtBBCode.DataSet.WriteXml(this.Response.OutputStream);
                        this.Response.End();
                    }
                    else
                    {
                        this.PageContext.AddLoadMessage(this.GetText("ADMIN_BBCODE", "MSG_NOTHING_SELECTED"));
                    }
                }

                break;
            case "import":
                YafBuildLink.Redirect(ForumPages.admin_bbcode_import);
                break;
        }
    }

      /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.bbCodeList.DataSource = LegacyDb.bbcode_list(this.PageContext.PageBoardID, null);
      this.DataBind();
    }

    #endregion
  }
}