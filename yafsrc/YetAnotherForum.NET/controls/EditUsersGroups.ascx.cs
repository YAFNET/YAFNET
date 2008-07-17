/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Controls
{
	public partial class EditUsersGroups : YAF.Classes.Base.BaseUserControl
	{
		#region Properties

		/// <summary>
		/// Gets user ID of edited user.
		/// </summary>
		protected int CurrentUserID
		{
			get
			{
				return ( int )this.PageContext.QueryIDs ["u"];
			}
		}
		
		#endregion


		#region Event Handlers

		/// <summary>
		/// Handles page load event.
		/// </summary>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			PageContext.QueryIDs = new QueryStringIDHelper( "u", true );

			// this needs to be done just once, not during postbacks
			if (!IsPostBack)
			{
				// bind data
				BindData();
			}
		}


		/// <summary>
		/// Handles click on cancel button.
		/// </summary>
		protected void Cancel_Click(object sender, System.EventArgs e)
		{
			// redurect to user admin page.
			YafBuildLink.Redirect(ForumPages.admin_users);
		}


		/// <summary>
		/// Handles click on save button.
		/// </summary>
		protected void Save_Click(object sender, System.EventArgs e)
		{
			// go through all roles displayed on page
			for (int i = 0; i < UserGroups.Items.Count; i++)
			{
				// get current item
				RepeaterItem item = UserGroups.Items[i];
				// get role ID from it
				int roleID = int.Parse(((Label)item.FindControl("GroupID")).Text);

				// get role name
				string roleName = string.Empty;
				using (DataTable dt = DB.group_list(PageContext.PageBoardID, roleID))
				{
					foreach (DataRow row in dt.Rows) roleName = (string)row["Name"];
				}

				// is user supposed to be in that role?
				bool isChecked = ((CheckBox)item.FindControl("GroupMember")).Checked;

				// save user in role
				DB.usergroup_save(CurrentUserID, roleID, isChecked);

				// update roles if this user isn't the guest
				if (!UserMembershipHelper.IsGuestUser(CurrentUserID))
				{
					// get user's name
					string userName = UserMembershipHelper.GetUserNameFromID(CurrentUserID);

					// add/remove user from roles in membership provider
					if (isChecked && !Roles.IsUserInRole(userName, roleName))
						Roles.AddUserToRole(userName, roleName);
					else if (!isChecked && Roles.IsUserInRole(userName, roleName))
						Roles.RemoveUserFromRole(userName, roleName);
				}
			}

			// update forum moderators cache just in case something was changed...
			YafCache.Current.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));

			BindData();
		}

		#endregion


		#region Data Binding & Formatting

		/// <summary>
		/// Bind data for this control.
		/// </summary>
		private void BindData()
		{
			// get user roles
			UserGroups.DataSource = DB.group_member(PageContext.PageBoardID, CurrentUserID);

			// bind data to controls
			DataBind();
		}


		/// <summary>
		/// Checks if user is memeber of role or not depending on value of parameter.
		/// </summary>
		/// <param name="o">Parameter if 0, user is not member of a role.</param>
		/// <returns>True if user is member of role (o > 0), false otherwise.</returns>
		protected bool IsMember(object o)
		{
			return long.Parse(o.ToString()) > 0;
		}

		#endregion
	}
}