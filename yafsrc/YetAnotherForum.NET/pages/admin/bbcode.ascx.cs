/* Yet Another Forum.NET
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
  using System.Collections.Generic;
  using System.Data;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;

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
      ((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this YafBBCode Extension?')";
    }

    /// <summary>
    /// The get selected bb code i ds.
    /// </summary>
    /// <returns>
    /// </returns>
    [NotNull]
    protected List<int> GetSelectedBBCodeIDs()
    {
      var idList = new List<int>();

      // get checked items....
      foreach (RepeaterItem item in this.bbCodeList.Items)
      {
        var sel = (CheckBox)item.FindControl("chkSelected");
        if (sel.Checked)
        {
          var hiddenId = (HiddenField)item.FindControl("hiddenBBCodeID");

          idList.Add(Convert.ToInt32(hiddenId.Value));
        }
      }

      return idList;
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
       this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("YafBBCode Extensions", string.Empty);

        this.BindData();
      }
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
      if (e.CommandName == "add")
      {
        YafBuildLink.Redirect(ForumPages.admin_bbcode_edit);
      }
      else if (e.CommandName == "edit")
      {
        YafBuildLink.Redirect(ForumPages.admin_bbcode_edit, "b={0}", e.CommandArgument);
      }
      else if (e.CommandName == "delete")
      {
        LegacyDb.bbcode_delete(e.CommandArgument);
        this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.CustomBBCode));
        this.BindData();
      }
      else if (e.CommandName == "export")
      {
        List<int> bbCodeIds = this.GetSelectedBBCodeIDs();

        if (bbCodeIds.Count > 0)
        {
          // export this list as XML...
          DataTable dtBBCode = LegacyDb.bbcode_list(this.PageContext.PageBoardID, null);

          // remove all but required bbcodes...
          foreach (DataRow row in dtBBCode.Rows)
          {
            int id = Convert.ToInt32(row["BBCodeID"]);
            if (!bbCodeIds.Contains(id))
            {
              // remove from this table...
              row.Delete();
            }
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
          this.PageContext.AddLoadMessage("Nothing selected to export.");
        }
      }
      else if (e.CommandName == "import")
      {
        YafBuildLink.Redirect(ForumPages.admin_bbcode_import);
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