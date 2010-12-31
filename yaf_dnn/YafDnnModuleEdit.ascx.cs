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

namespace YAF.DotNetNuke
{
  #region Using

  using System;
  using System.Data;
    using System.Web.UI.WebControls;

  using global::DotNetNuke.Entities.Modules;
  using global::DotNetNuke.Framework;

    using YAF.Classes.Data;
  using YAF.Types.Constants;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for DotNetNukeModule.
  /// </summary>
  public partial class YafDnnModuleEdit : PortalModuleBase
  {
    // protected DropDownList    BoardID, CategoryID;
    // protected LinkButton    update, cancel, create;
    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      this.Load += this.DotNetNukeModuleEdit_Load;
      this.update.Click += this.UpdateClick;
      this.cancel.Click += CancelClick;
      this.create.Click += CreateClick;
      this.BoardID.SelectedIndexChanged += this.BoardIdSelectedIndexChanged;
      base.OnInit(e);
    }

    /// <summary>
    /// The cancel click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private static void CancelClick(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.forum);
    }

    /// <summary>
    /// The create click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private static void CreateClick(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_editboard);
    }

    /// <summary>
    /// The bind categories.
    /// </summary>
    private void BindCategories()
    {
      using (DataTable dt = LegacyDb.category_list(this.BoardID.SelectedValue, DBNull.Value))
      {
        DataRow row = dt.NewRow();
        row["Name"] = "[All Categories]";
        row["CategoryID"] = DBNull.Value;
        dt.Rows.InsertAt(row, 0);

        this.CategoryID.DataSource = dt;
        this.CategoryID.DataTextField = "Name";
        this.CategoryID.DataValueField = "CategoryID";
        this.CategoryID.DataBind();

          if (this.Settings["forumcategoryid"] == null)
          {
              return;
          }

          ListItem item = this.CategoryID.Items.FindByValue(this.Settings["forumcategoryid"].ToString());
          if (item != null)
          {
              item.Selected = true;
          }
      }
    }

    /// <summary>
    /// The board id selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void BoardIdSelectedIndexChanged(object sender, EventArgs e)
    {
      this.BindCategories();
    }

    /// <summary>
    /// The dot net nuke module edit_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void DotNetNukeModuleEdit_Load(object sender, EventArgs e)
    {
        ((CDefault)this.Page).AddStyleSheet("YafDefaultTheme", this.ResolveUrl("themes/standard/theme.css"));

        this.update.Text = "Update";
        this.cancel.Text = "Cancel";
        this.create.Text = "Create New Board";

        this.update.Visible = this.IsEditable;
        this.create.Visible = this.IsEditable;

        if (this.IsPostBack)
        {
            return;
        }

        using (DataTable dt = LegacyDb.board_list(DBNull.Value))
        {
            this.BoardID.DataSource = dt;
            this.BoardID.DataTextField = "Name";
            this.BoardID.DataValueField = "BoardID";
            this.BoardID.DataBind();
            if (this.Settings["forumboardid"] != null)
            {
                ListItem item = this.BoardID.Items.FindByValue(this.Settings["forumboardid"].ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        this.BindCategories();

        bool ineritDnnLang;
        bool.TryParse((string)this.Settings["InheritDnnLanguage"], out ineritDnnLang);

        this.InheritDnnLanguage.Checked = ineritDnnLang;
    }

      /// <summary>
    /// The update click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void UpdateClick(object sender, EventArgs e)
    {
      var objModules = new ModuleController();

      objModules.UpdateModuleSetting(this.ModuleId, "forumboardid", this.BoardID.SelectedValue);
      objModules.UpdateModuleSetting(this.ModuleId, "forumcategoryid", this.CategoryID.SelectedValue);
      objModules.UpdateModuleSetting(this.ModuleId, "InheritDnnLanguage", this.InheritDnnLanguage.Checked.ToString());

      YafBuildLink.Redirect(ForumPages.forum);
    }

    #endregion
  }
}