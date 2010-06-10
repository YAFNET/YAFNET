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
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Interface for creating or editing user roles/groups.
  /// </summary>
  public partial class editgroup : AdminPage
  {
    #region Construcotrs & Overridden Methods

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    protected override void CreatePageLinks()
    {
      // forum index
      this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

      // admin index
      this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));

      // roles index
      this.PageLinks.AddLink("Roles", YafBuildLink.GetLink(ForumPages.admin_groups));

      // edit role
      this.PageLinks.AddLink("Edit Role");
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles page load event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      // this needs to be done just once, not during postbacks
      if (!IsPostBack)
      {
        // create page links
        CreatePageLinks();

        // bind data
        BindData();

        // is this editing of existing role or creation of new one?
        if (Request.QueryString["i"] != null)
        {
          // we are not creating new role
          this.NewGroupRow.Visible = false;

          // get data about edited role
          using (DataTable dt = DB.group_list(PageContext.PageBoardID, Request.QueryString["i"]))
          {
            // get it as row
            DataRow row = dt.Rows[0];

            // get role flags
            var flags = new GroupFlags(row["Flags"]);

            // set controls to role values
            this.Name.Text = (string) row["Name"];
            this.IsGuestX.Checked = flags.IsGuest;
            this.IsAdminX.Checked = flags.IsAdmin;
            this.IsStartX.Checked = flags.IsStart;
            this.IsModeratorX.Checked = flags.IsModerator;
            this.PMLimit.Text = row["PMLimit"].ToString();
            this.StyleTextBox.Text = row["Style"].ToString();
            this.Priority.Text = row["SortOrder"].ToString();
            this.UsrAlbums.Text = row["UsrAlbums"].ToString();
            this.UsrAlbumImages.Text = row["UsrAlbumImages"].ToString();
            this.UsrSigChars.Text = row["UsrSigChars"].ToString();
            this.UsrSigBBCodes.Text = row["UsrSigBBCodes"].ToString();
            this.UsrSigHTMLTags.Text = row["UsrSigHTMLTags"].ToString();
            this.Description.Text = row["Description"].ToString();

            // IsGuest flag can be set for only one role. if it isn't for this, disable that row
            if (flags.IsGuest)
            {
              this.IsGuestTR.Visible = true;
            }
          }
        }
      }
    }


    /// <summary>
    /// Handles click on cancel button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
      // go back to roles administration
      YafBuildLink.Redirect(ForumPages.admin_groups);
    }


    /// <summary>
    /// Handles click on save button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Save_Click(object sender, EventArgs e)
    {
      if (!ValidationHelper.IsValidInt(this.PMLimit.Text.Trim()))
      {
        PageContext.AddLoadMessage("You should enter integer value for pmessage number.");
        return;
      }

      if (!ValidationHelper.IsValidInt(this.Priority.Text.Trim()))
      {
        PageContext.AddLoadMessage("You should enter integer value for priority.");
        return;
      }
      if (!ValidationHelper.IsValidInt(this.UsrAlbums.Text.Trim()))
      {
          PageContext.AddLoadMessage("You should enter integer value for the number of user albums.");
          return;
      }
      if (!ValidationHelper.IsValidInt(this.UsrSigChars.Text.Trim()))
      {
          PageContext.AddLoadMessage("You should enter integer value for the number of chars in user signature.");
          return;
      }
      if (!ValidationHelper.IsValidInt(this.UsrAlbumImages.Text.Trim()))
      {
          PageContext.AddLoadMessage("You should enter integer value for the total number of images in all albums.");
          return;
      }

      // Role
      long roleID = 0;

      // get role ID from page's parameter
      if (Request.QueryString["i"] != null)
      {
        roleID = long.Parse(Request.QueryString["i"]);
      }

      // get new and old name
      string roleName = this.Name.Text.Trim();
      string oldRoleName = string.Empty;

      // if we are editing exising role, get it's original name
      if (roleID != 0)
      {
        // get the current role name in the DB
        using (DataTable dt = DB.group_list(YafContext.Current.PageBoardID, roleID))
        {
          foreach (DataRow row in dt.Rows)
          {
            oldRoleName = row["Name"].ToString();
          }
        }
      }

      // save role and get its ID if it's new (if it's old role, we get it anyway)
      roleID = DB.group_save(
        roleID, 
        PageContext.PageBoardID, 
        roleName, 
        this.IsAdminX.Checked, 
        this.IsGuestX.Checked, 
        this.IsStartX.Checked, 
        this.IsModeratorX.Checked, 
        this.AccessMaskID.SelectedValue, 
        this.PMLimit.Text.Trim(), 
        this.StyleTextBox.Text.Trim(), 
        this.Priority.Text.Trim(),
        this.Description.Text,
        this.UsrSigChars.Text,
        this.UsrSigBBCodes.Text,
        this.UsrSigHTMLTags.Text,
        this.UsrAlbums.Text.Trim(),
        this.UsrAlbumImages.Text.Trim()
        );
     

      // see if need to rename an existing role...
      if (roleName != oldRoleName && RoleMembershipHelper.RoleExists(oldRoleName) && !RoleMembershipHelper.RoleExists(roleName) && !this.IsGuestX.Checked)
      {
        // transfer users in addition to changing the name of the role...
        string[] users = PageContext.CurrentRoles.GetUsersInRole(oldRoleName);

        // delete the old role...
        RoleMembershipHelper.DeleteRole(oldRoleName, false);

        // create new role...
        RoleMembershipHelper.CreateRole(roleName);

        if (users.Length > 0)
        {
          // put users into new role...
          PageContext.CurrentRoles.AddUsersToRoles(users, new[] { roleName });
        }
      }
        
        // if role doesn't exist in provider's data source, create it
      else if (!RoleMembershipHelper.RoleExists(roleName) && !this.IsGuestX.Checked)
      {
        // simply create it
        RoleMembershipHelper.CreateRole(roleName);
      }     
     
     
      // Access masks for a newly created or an existing role
      if (Request.QueryString["i"] != null)
      {
        // go trhough all forums
        for (int i = 0; i < this.AccessList.Items.Count; i++)
        {
          // get current repeater item
          RepeaterItem item = this.AccessList.Items[i];

          // get forum ID
          int forumID = int.Parse(((Label) item.FindControl("ForumID")).Text);

          // save forum access maks for this role
          DB.forumaccess_save(forumID, roleID, ((DropDownList) item.FindControl("AccessmaskID")).SelectedValue);
        }

        YafBuildLink.Redirect(ForumPages.admin_groups);
      }

      // remove caching in case something got updated...
      PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));
      
      // Clearing cache with old permissions data...
      this.PageContext.Cache.Remove((x) => x.StartsWith(YafCache.GetBoardCacheKey(Constants.Cache.ActiveUserLazyData)));

      // Done, redirect to role editing page
      YafBuildLink.Redirect(ForumPages.admin_editgroup, "i={0}", roleID);
    }


    /// <summary>
    /// Handles pre-render event of each forum's access mask dropdown.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SetDropDownIndex(object sender, EventArgs e)
    {
      // get dropdown which raised this event
      var list = (DropDownList) sender;

      // select value from the list
      ListItem item = list.Items.FindByValue(list.Attributes["value"]);

      // verify something was found...
      if (item != null)
      {
        item.Selected = true;
      }
    }


    /// <summary>
    /// Handles databinding event of initial access maks dropdown control.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BindData_AccessMaskID(object sender, EventArgs e)
    {
      // get sender object as dropdown list
      var c = (DropDownList) sender;

      // list all access masks as data source
      c.DataSource = DB.accessmask_list(PageContext.PageBoardID, null);

      // set value and text field names
      c.DataValueField = "AccessMaskID";
      c.DataTextField = "Name";
    }

    #endregion

    #region Data Binding & Formatting

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
      // set datasource of access list (list of forums and role's access masks) if we are editing existing mask
      if (Request.QueryString["i"] != null)
      {
        this.AccessList.DataSource = DB.forumaccess_group(Request.QueryString["i"]);
      }

      // bind data to controls
      DataBind();
    }

    #endregion
  }
}