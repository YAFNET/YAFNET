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
  using System.Data;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.BBCode;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The bbcode_edit.
  /// </summary>
  public partial class bbcode_edit : AdminPage
  {
    #region Constants and Fields

    /// <summary>
    ///   The _bbcode id.
    /// </summary>
    private int? _bbcodeId;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets BBCodeID.
    /// </summary>
    protected int? BBCodeID
    {
      get
      {
        if (this._bbcodeId != null)
        {
          return this._bbcodeId;
        }
        else if (this.Request.QueryString.GetFirstOrDefault("b") != null)
        {
          int id;
          if (int.TryParse(this.Request.QueryString.GetFirstOrDefault("b"), out id))
          {
            this._bbcodeId = id;
            return id;
          }
        }

        return null;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The add_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Add_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      short sortOrder = 0;

      if (!ValidationHelper.IsValidPosShort(this.txtExecOrder.Text.Trim()))
      {
        this.PageContext.AddLoadMessage("The sort order value should be a positive integer from 0 to 32767.");
        return;
      }

      if (!short.TryParse(this.txtExecOrder.Text.Trim(), out sortOrder))
      {
        this.PageContext.AddLoadMessage("You must enter an number value from 0 to 32767 for sort order.");
        return;
      }

      DB.bbcode_save(
        this.BBCodeID, 
        this.PageContext.PageBoardID, 
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
        sortOrder);
      this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.CustomBBCode));
      ReplaceRulesCreator.ClearCache();
      YafBuildLink.Redirect(ForumPages.admin_bbcode);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    protected void BindData()
    {
      if (this.BBCodeID != null)
      {
        DataRow row = DB.bbcode_list(this.PageContext.PageBoardID, this.BBCodeID.Value).Rows[0];

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
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_bbcode);
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
      string strAddEdit = (this.BBCodeID == null) ? "Add" : "Edit";

      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(strAddEdit + " YafBBCode Extensions", string.Empty);

        this.BindData();
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

    #endregion
  }
}