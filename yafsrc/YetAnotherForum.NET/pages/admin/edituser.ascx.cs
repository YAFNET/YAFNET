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
  using System.Web.Security;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for edituser.
  /// </summary>
  public partial class edituser : AdminPage
  {
    /// <summary>
    /// Gets user ID of edited user.
    /// </summary>
    protected int CurrentUserID
    {
      get
      {
        return (int) PageContext.QueryIDs["u"];
      }
    }

    protected bool IsGuestUser
    {
      get
      {
        return UserMembershipHelper.IsGuestUser(CurrentUserID);
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
      // we're in the admin section...
      ProfileEditControl.InAdminPages = true;
      SignatureEditControl.InAdminPages = true;
      AvatarEditControl.InAdminPages = true;

      PageContext.QueryIDs = new QueryStringIDHelper("u", true);

      DataTable dt = DB.user_list(PageContext.PageBoardID, CurrentUserID, null);

      if (dt.Rows.Count == 1)
      {
        DataRow userRow = dt.Rows[0];

        // do admin permission check...
        if (!PageContext.IsHostAdmin && IsUserHostAdmin(userRow))
        {
          // user is not host admin and is attempted to edit host admin account...
          YafBuildLink.AccessDenied();
        }

        if (!IsPostBack)
        {
          this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
          this.PageLinks.AddLink("Users", YafBuildLink.GetLink(ForumPages.admin_users));
          this.PageLinks.AddLink(String.Format("Edit User \"{0}\"", userRow["Name"].ToString()));

          // do a quick user membership sync...
          MembershipUser user = UserMembershipHelper.GetMembershipUserById(CurrentUserID);

          // update if the user is not Guest
          if (!IsGuestUser)
          {
            RoleMembershipHelper.UpdateForumUser(user, PageContext.PageBoardID);
          }

          EditUserTabs.DataBind();
        }
      }
    }

    /// <summary>
    /// The is user host admin.
    /// </summary>
    /// <param name="userRow">
    /// The user row.
    /// </param>
    /// <returns>
    /// The is user host admin.
    /// </returns>
    protected bool IsUserHostAdmin(DataRow userRow)
    {
      var userFlags = new UserFlags(userRow["Flags"]);
      return userFlags.IsHostAdmin;
    }
  }
}