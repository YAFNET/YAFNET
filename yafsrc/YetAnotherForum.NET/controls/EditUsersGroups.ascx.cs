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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The edit users groups.
  /// </summary>
  public partial class EditUsersGroups : BaseUserControl
  {
    #region Properties

    /// <summary>
    /// Gets user ID of edited user.
    /// </summary>
    protected int CurrentUserID
    {
      get
      {
        return (int)this.PageContext.QueryIDs["u"];
      }
    }

    #endregion

    #region Methods

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
      // redurect to user admin page.
      YafBuildLink.Redirect(ForumPages.admin_users);
    }

    /// <summary>
    /// Checks if user is memeber of role or not depending on value of parameter.
    /// </summary>
    /// <param name="o">
    /// Parameter if 0, user is not member of a role.
    /// </param>
    /// <returns>
    /// True if user is member of role (o &gt; 0), false otherwise.
    /// </returns>
    protected bool IsMember(object o)
    {
      return long.Parse(o.ToString()) > 0;
    }

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
      this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

      // this needs to be done just once, not during postbacks
      if (!this.IsPostBack)
      {
        // bind data
        this.BindData();
      }
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
      // go through all roles displayed on page
      for (int i = 0; i < this.UserGroups.Items.Count; i++)
      {
        // get current item
        RepeaterItem item = this.UserGroups.Items[i];

        // get role ID from it
        int roleID = int.Parse(((Label)item.FindControl("GroupID")).Text);

        // get role name
        string roleName = string.Empty;
        using (DataTable dt = DB.group_list(this.PageContext.PageBoardID, roleID))
        {
          foreach (DataRow row in dt.Rows)
          {
            roleName = (string)row["Name"];
          }
        }

        // is user supposed to be in that role?
        bool isChecked = ((CheckBox)item.FindControl("GroupMember")).Checked;

        // save user in role
        DB.usergroup_save(this.CurrentUserID, roleID, isChecked);

        // update roles if this user isn't the guest
        if (!UserMembershipHelper.IsGuestUser(this.CurrentUserID))
        {
          // get user's name
          string userName = UserMembershipHelper.GetUserNameFromID(this.CurrentUserID);

          // add/remove user from roles in membership provider
          if (isChecked && !RoleMembershipHelper.IsUserInRole(userName, roleName))
          {
            RoleMembershipHelper.AddUserToRole(userName, roleName);
          }
          else if (!isChecked && RoleMembershipHelper.IsUserInRole(userName, roleName))
          {
            RoleMembershipHelper.RemoveUserFromRole(userName, roleName);
          }

          // Clearing cache with old permisssions data...
          this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ActiveUserLazyData.FormatWith(this.CurrentUserID)));
        }
      }

      // update forum moderators cache just in case something was changed...
      this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));

      // clear the cache for this user...
      UserMembershipHelper.ClearCacheForUserId(this.CurrentUserID);

      this.BindData();
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
      // get user roles
      this.UserGroups.DataSource = DB.group_member(this.PageContext.PageBoardID, this.CurrentUserID);

      // bind data to controls
      this.DataBind();
    }

    #endregion
  }
}