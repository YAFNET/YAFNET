/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Interface for creating or editing user roles/groups.
	/// </summary>
	public partial class editgroup : YAF.Classes.Base.AdminPage
	{
		#region Construcotrs & Overridden Methods

		/// <summary>
		/// Creates page links for this page.
		/// </summary>
		protected override void CreatePageLinks()
		{
			// forum index
			PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
			// admin index
			PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
			// roles index
			PageLinks.AddLink("Roles", YafBuildLink.GetLink(ForumPages.admin_groups));
			// edit role
			PageLinks.AddLink("Edit Role");
		}

		#endregion


		#region Event Handlers

		/// <summary>
		/// Handles page load event.
		/// </summary>
		protected void Page_Load(object sender, System.EventArgs e)
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
					NewGroupRow.Visible = false;

					// get data about edited role
					using (DataTable dt = DB.group_list(PageContext.PageBoardID, Request.QueryString["i"]))
					{
						// get it as row
						DataRow row = dt.Rows[0];
						// get role flags
						GroupFlags flags = new GroupFlags(row["Flags"]);

						// set controls to role values
						Name.Text = (string)row["Name"];
						IsGuestX.Checked = flags.IsGuest;
						IsAdminX.Checked = flags.IsAdmin;
						IsStartX.Checked = flags.IsStart;
						IsModeratorX.Checked = flags.IsModerator;

						// IsGuest flag can be set for only one role. if it isn't for this, disable that row
						if (flags.IsGuest) IsGuestTR.Visible = true;
					}
				}
			}
		}


		/// <summary>
		/// Handles click on cancel button.
		/// </summary>
		protected void Cancel_Click(object sender, System.EventArgs e)
		{
			// go back to roles administration
			YafBuildLink.Redirect(ForumPages.admin_groups);
		}


		/// <summary>
		/// Handles click on save button.
		/// </summary>
		protected void Save_Click(object sender, System.EventArgs e)
		{
			// Role
			long roleID = 0;
			// get role ID from page's parameter
			if (Request.QueryString["i"] != null) roleID = long.Parse(Request.QueryString["i"]);

			// get new and old name
			string roleName = Name.Text.Trim();
			string oldRoleName = string.Empty;

			// if we are editing exising role, get it's original name
			if (roleID != 0)
			{
				// get the current role name in the DB
				using (DataTable dt = DB.group_list(YafContext.Current.PageBoardID, roleID))
				{
					foreach (DataRow row in dt.Rows) oldRoleName = row["Name"].ToString();
				}
			}

			// save role and get its ID if it's new (if it's old role, we get it anyway)
			roleID = DB.group_save(roleID, PageContext.PageBoardID, roleName, IsAdminX.Checked, IsGuestX.Checked, IsStartX.Checked, IsModeratorX.Checked, AccessMaskID.SelectedValue);

			// if role doesn't exist in provider's data source, create it or rename it
			if (!System.Web.Security.Roles.RoleExists(roleName))
			{
				if (oldRoleName != string.Empty || IsGuestX.Checked)
				{
					// delete and re-create (if not guest role)
					System.Web.Security.Roles.DeleteRole(oldRoleName, false);
				}

				if (!IsGuestX.Checked)
				{
					// simply create it
					System.Web.Security.Roles.CreateRole(roleName);
				}
			}

			// Access masks for newly existing role
			if (Request.QueryString["i"] != null)
			{
				// go trhough all forums
				for (int i = 0; i < AccessList.Items.Count; i++)
				{
					// get current repeater item
					RepeaterItem item = AccessList.Items[i];

					// get forum ID
					int ForumID = int.Parse(((Label)item.FindControl("ForumID")).Text);

					// save forum access maks for this role
					DB.forumaccess_save(ForumID, roleID, ((DropDownList)item.FindControl("AccessmaskID")).SelectedValue);
				}
				YafBuildLink.Redirect(ForumPages.admin_groups);
			}

			// remove caching in case something got updated...
			YafCache.Current.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));

			// Done, redirect to role editing page
			YafBuildLink.Redirect(ForumPages.admin_editgroup, "i={0}", roleID);
		}


		/// <summary>
		/// Handles pre-render event of each forum's access mask dropdown.
		/// </summary>
		protected void SetDropDownIndex(object sender, System.EventArgs e)
		{
			// get dropdown which raised this event
			DropDownList list = (DropDownList)sender;

			// select value from the list
			ListItem item = list.Items.FindByValue(list.Attributes["value"]);

			// verify something was found...
			if ( item != null ) item.Selected = true;
		}


		/// <summary>
		/// Handles databinding event of initial access maks dropdown control.
		/// </summary>
		protected void BindData_AccessMaskID(object sender, System.EventArgs e)
		{
			// get sender object as dropdown list
			DropDownList c = (DropDownList)sender;

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
				AccessList.DataSource = DB.forumaccess_group(Request.QueryString["i"]);

			// bind data to controls
			DataBind();
		}

		#endregion
	}
}
