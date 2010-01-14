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
  using System;
  using System.Data;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;

  /// <summary>
  /// The bbcode_edit.
  /// </summary>
  public partial class bbcode_edit : AdminPage
  {
    /// <summary>
    /// The _bbcode id.
    /// </summary>
    private int? _bbcodeId = null;

    /// <summary>
    /// Gets BBCodeID.
    /// </summary>
    protected int? BBCodeID
    {
      get
      {
        if (this._bbcodeId != null)
        {
          return this._bbcodeId;
        }
        else if (Request.QueryString["b"] != null)
        {
          int id;
          if (int.TryParse(Request.QueryString["b"], out id))
          {
            this._bbcodeId = id;
            return id;
          }
        }

        return null;
      }
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
      string strAddEdit = (BBCodeID == null) ? "Add" : "Edit";

      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(strAddEdit + " YafBBCode Extensions", string.Empty);

        BindData();
      }

      this.txtName.Attributes.Add("style", "width:100%");
      this.txtDescription.Attributes.Add("style", "width:100%;height:75px;");
      this.txtOnClickJS.Attributes.Add("style", "width:100%;height:75px;");
      this.txtDisplayJS.Attributes.Add("style", "width:100%;height:75px;");
      this.txtEditJS.Attributes.Add("style", "width:100%;height:75px;");
      this.txtDisplayCSS.Attributes.Add("style", "width:100%;height:75px;");
      this.txtSearchRegEx.Attributes.Add("style", "width:100%;height:75px;");
      this.txtReplaceRegEx.Attributes.Add("style", "width:100%;height:75px;");
      this.txtVariables.Attributes.Add("style", "width:100%;height:75px;");
      this.txtModuleClass.Attributes.Add("style", "width:100%");
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    protected void BindData()
    {
      if (BBCodeID != null)
      {
        DataRow row = DB.bbcode_list(PageContext.PageBoardID, BBCodeID.Value).Rows[0];

        // fill the control values...
        this.txtName.Text = row["Name"].ToString();
        this.txtExecOrder.Text = row["ExecOrder"].ToString();
        this.txtDescription.Text = row["Description"].ToString();
        this.txtOnClickJS.Text = row["OnClickJS"].ToString();
        this.txtDisplayJS.Text = row["DisplayJS"].ToString();
        this.txtEditJS.Text = row["EditJS"].ToString();
        this.txtDisplayCSS.Text = row["DisplayCSS"].ToString();
        this.txtSearchRegEx.Text = row["SearchRegex"].ToString();
        this.txtReplaceRegEx.Text = row["ReplaceRegex"].ToString();
        this.txtVariables.Text = row["Variables"].ToString();
        this.txtModuleClass.Text = row["ModuleClass"].ToString();
        this.chkUseModule.Checked = Convert.ToBoolean((row["UseModule"] == DBNull.Value) ? false : row["UseModule"]);
      }
    }

    /// <summary>
    /// The add_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Add_Click(object sender, EventArgs e)
    {
      DB.bbcode_save(
        BBCodeID, 
        PageContext.PageBoardID, 
        this.txtName.Text.Trim(), 
        this.txtDescription.Text, 
        this.txtOnClickJS.Text, 
        this.txtDisplayJS.Text, 
        this.txtEditJS.Text, 
        this.txtDisplayCSS.Text, 
        this.txtSearchRegEx.Text, 
        this.txtReplaceRegEx.Text, 
        this.txtVariables.Text, 
        this.chkUseModule.Checked, 
        this.txtModuleClass.Text, 
        int.Parse(this.txtExecOrder.Text));
      PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.CustomBBCode));
      ReplaceRulesCreator.ClearCache();
      YafBuildLink.Redirect(ForumPages.admin_bbcode);
    }

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_bbcode);
    }
  }
}