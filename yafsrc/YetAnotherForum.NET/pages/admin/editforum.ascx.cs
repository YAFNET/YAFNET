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
  /// Administrative Page for the editting of forum properties.
  /// </summary>
  public partial class editforum : AdminPage
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
        this.PageLinks.AddLink("Forums", string.Empty);

        BindData();
        if (Request.QueryString["f"] != null)
        {
          using (DataTable dt = DB.forum_list(PageContext.PageBoardID, Request.QueryString["f"]))
          {
            DataRow row = dt.Rows[0];
            var flags = new ForumFlags(row["Flags"]);
            this.Name.Text = (string) row["Name"];
            this.Description.Text = (string) row["Description"];
            this.SortOrder.Text = row["SortOrder"].ToString();
            this.HideNoAccess.Checked = flags.IsHidden;
            this.Locked.Checked = flags.IsLocked;
            this.IsTest.Checked = flags.IsTest;
            this.ForumNameTitle.Text = this.Name.Text;
            this.Moderated.Checked = flags.IsModerated;

            this.CategoryList.SelectedValue = row["CategoryID"].ToString();

            // populate parent forums list with forums according to selected category
            BindParentList();

            if (!row.IsNull("ParentID"))
            {
              this.ParentList.SelectedValue = row["ParentID"].ToString();
            }

            if (!row.IsNull("ThemeURL"))
            {
              this.ThemeList.SelectedValue = row["ThemeURL"].ToString();
            }

            this.remoteurl.Text = row["RemoteURL"].ToString();
          }

          this.NewGroupRow.Visible = false;
        }
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      int ForumID = 0;
      this.CategoryList.DataSource = DB.category_list(PageContext.PageBoardID, null);
      this.CategoryList.DataBind();

      if (Request.QueryString["f"] != null)
      {
        ForumID = Convert.ToInt32(Request.QueryString["f"]);
        this.AccessList.DataSource = DB.forumaccess_list(ForumID);
        this.AccessList.DataBind();
      }

      // Load forum's combo
      BindParentList();

      // Load forum's themes
      var listheader = new ListItem();
      listheader.Text = "Choose a theme";
      listheader.Value = string.Empty;

      this.AccessMaskID.DataBind();

      this.ThemeList.DataSource = StaticDataHelper.Themes();
      this.ThemeList.DataTextField = "Theme";
      this.ThemeList.DataValueField = "FileName";
      this.ThemeList.DataBind();
      this.ThemeList.Items.Insert(0, listheader);
    }

    /// <summary>
    /// The bind parent list.
    /// </summary>
    private void BindParentList()
    {
      this.ParentList.DataSource = DB.forum_listall_fromCat(PageContext.PageBoardID, this.CategoryList.SelectedValue);
      this.ParentList.DataValueField = "ForumID";
      this.ParentList.DataTextField = "Title";
      this.ParentList.DataBind();
    }

    /// <summary>
    /// The category_ change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void Category_Change(object sender, EventArgs e)
    {
      BindParentList();
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      this.CategoryList.AutoPostBack = true;
      this.Save.Click += new EventHandler(Save_Click);
      this.Cancel.Click += new EventHandler(Cancel_Click);
      base.OnInit(e);
    }

    /// <summary>
    /// The save_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Save_Click(object sender, EventArgs e)
    {
      if (this.CategoryList.SelectedValue.Trim().Length == 0)
      {
        PageContext.AddLoadMessage("You must select a category for the forum.");
        return;
      }

      if (this.Name.Text.Trim().Length == 0)
      {
        PageContext.AddLoadMessage("You must enter a name for the forum.");
        return;
      }

      if (this.Description.Text.Trim().Length == 0)
      {
        PageContext.AddLoadMessage("You must enter a description for the forum.");
        return;
      }

      if (this.SortOrder.Text.Trim().Length == 0)
      {
        PageContext.AddLoadMessage("You must enter a value for sort order.");
        return;
      }

      int sortOrder = 0;
      if (!int.TryParse(this.SortOrder.Text.Trim(), out sortOrder))
      {
        PageContext.AddLoadMessage("You must enter an number value for sort order.");
        return;
      }

      // Forum
      long ForumID = 0;
      if (Request.QueryString["f"] != null)
      {
        ForumID = long.Parse(Request.QueryString["f"]);
      }
      else if (this.AccessMaskID.SelectedValue.Length == 0)
      {
        PageContext.AddLoadMessage("You must select an initial access mask for the forum.");
        return;
      }

      object parentID = null;
      if (this.ParentList.SelectedValue.Length > 0)
      {
        parentID = this.ParentList.SelectedValue;
      }

      if (parentID != null && parentID.ToString() == Request.QueryString["f"])
      {
        PageContext.AddLoadMessage("Forum cannot be parent of self.");
        return;
      }

      // The picked forum cannot be child forum as it's a parent
      // If we update a forum ForumID > 0 
      if (ForumID > 0 && parentID != null)
      {
          int dependency = DB.forum_save_parentschecker(ForumID, parentID);
          if (dependency > 0)
          {
              PageContext.AddLoadMessage("The choosen forum cannot be child forum as it's a parent.");
              return;
          }
      }

      object themeURL = null;
      if (this.ThemeList.SelectedValue.Length > 0)
      {
        themeURL = this.ThemeList.SelectedValue;
      }

      ForumID = DB.forum_save(
        ForumID, 
        this.CategoryList.SelectedValue, 
        parentID, 
        this.Name.Text, 
        this.Description.Text, 
        sortOrder, 
        this.Locked.Checked, 
        this.HideNoAccess.Checked, 
        this.IsTest.Checked, 
        this.Moderated.Checked, 
        this.AccessMaskID.SelectedValue, 
        IsNull(this.remoteurl.Text), 
        themeURL, 
        false);

      // Access
      if (Request.QueryString["f"] != null)
      {
        for (int i = 0; i < this.AccessList.Items.Count; i++)
        {
          RepeaterItem item = this.AccessList.Items[i];
          int GroupID = int.Parse(((Label) item.FindControl("GroupID")).Text);
          DB.forumaccess_save(ForumID, GroupID, ((DropDownList) item.FindControl("AccessmaskID")).SelectedValue);
        }

        ClearCaches();
        YafBuildLink.Redirect(ForumPages.admin_forums);
      }

      ClearCaches();

      // Done
      YafBuildLink.Redirect(ForumPages.admin_editforum, "f={0}", ForumID);
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
    private void Cancel_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_forums);
    }

    /// <summary>
    /// The bind data_ access mask id.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BindData_AccessMaskID(object sender, EventArgs e)
    {
      ((DropDownList) sender).DataSource = DB.accessmask_list(PageContext.PageBoardID, null);
      ((DropDownList) sender).DataValueField = "AccessMaskID";
      ((DropDownList) sender).DataTextField = "Name";
    }

    /// <summary>
    /// The initialize component.
    /// </summary>
    private void InitializeComponent()
    {
    }

    /// <summary>
    /// The set drop down index.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SetDropDownIndex(object sender, EventArgs e)
    {
      try
      {
        var list = (DropDownList) sender;
        list.Items.FindByValue(list.Attributes["value"]).Selected = true;
      }
      catch (Exception)
      {
      }
    }
  }
}