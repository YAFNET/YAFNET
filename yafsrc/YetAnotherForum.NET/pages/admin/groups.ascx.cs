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

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Primary administrator interface for groups/roles editing.
	/// </summary>
	public partial class groups : YAF.Classes.Core.AdminPage
	{
		#region Data Members

		/// <summary>
		/// Temporary storage of un-linked provider roles.
		/// </summary>
		private StringCollection _availableRoles = new StringCollection();

		#endregion


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
			// roles
			PageLinks.AddLink("Roles", "");
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

				// sync roles just in case...
				RoleMembershipHelper.SyncRoles(YafContext.Current.PageBoardID);

				// bind data
				BindData();
			}
		}


		/// <summary>
		/// Handles load event for delete button, adds confirmation dialog.
		/// </summary>
		protected void Delete_Load(object sender, System.EventArgs e)
		{
			ControlHelper.AddOnClickConfirmDialog(sender, "Delete this Role?");
		}


		/// <summary>
		/// Handles role editing/deletion buttons.
		/// </summary>
		protected void RoleListYaf_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			// detect which command are we handling
			switch (e.CommandName)
			{
				case "edit":
					// go to role editing page
					YafBuildLink.Redirect(ForumPages.admin_editgroup, "i={0}", e.CommandArgument);
					break;
				case "delete":
					// delete role
					DB.group_delete(e.CommandArgument);
					// remove cache of forum moderators
					PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));
					// re-bind data
					BindData();
					break;
			}
		}


		/// <summary>
		/// Handles provider roles additing/deletetion.
		/// </summary>
		protected void RoleListNet_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			// detect which command are we handling
			switch (e.CommandName)
			{
				case "add":
					// save role and get its ID
                    int _initialPMessages = 0;
                    long groupID = DB.group_save(DBNull.Value, PageContext.PageBoardID, e.CommandArgument.ToString(), false, false, false, false, 1, _initialPMessages);
					// redirect to newly created role
					YafBuildLink.Redirect(ForumPages.admin_editgroup, "i={0}", groupID);
					break;
				case "delete":
					// delete role from provider data
					RoleMembershipHelper.DeleteRole(e.CommandArgument.ToString(), false);
					// re-bind data
					BindData();
					break;
			}
		}


		/// <summary>
		/// Handles click on new role button
		/// </summary>
		protected void NewGroup_Click(object sender, System.EventArgs e)
		{
			// redirect to new role page
			YafBuildLink.Redirect(ForumPages.admin_editgroup);
		}

		#endregion


		#region Data Binding & Formatting

		/// <summary>
		/// Bind data for this control.
		/// </summary>
		private void BindData()
		{
			// list roles of this board
			DataTable dt = DB.group_list(PageContext.PageBoardID, null);

			// set repeater datasource
			RoleListYaf.DataSource = dt;
           
			// clear cached list of roles
			_availableRoles.Clear();

			// get all provider roles
			foreach ( string role in RoleMembershipHelper.GetAllRoles() )
			{
				// make filter string, we want to filer by role name
				string filter = string.Format("Name='{0}'", role.Replace("'", "''"));
				
                // get given role of YAF
				DataRow[] rows = dt.Select(filter);

				// if this role is not in YAF DB, add it to the list of provider roles for syncing
				if (rows.Length == 0)
				{
					// doesn't exist in the Yaf Groups
					_availableRoles.Add(role);
				}
			}

			// check if there are any roles for syncing
			if (_availableRoles.Count > 0)
			{
				//make it datasource
				RoleListNet.DataSource = _availableRoles;
			}
			else
			{
				// no datasource for provider roles
				RoleListNet.DataSource = null;
			}

			// bind data to controls
			DataBind();
		}


		/// <summary>
		/// Get status of provider role vs YAF roles.
		/// </summary>
		/// <param name="currentRow">Data row which contains data about role.</param>
		/// <returns>String "Linked" when role is linked to YAF roles, "Unlinkable" otherwise.</returns>
		protected string GetLinkedStatus(System.Data.DataRowView currentRow)
		{
			// check whether role is Guests role, which can't be linked
			if (General.BinaryAnd(currentRow["Flags"], 2))
				return "Unlinkable";
			else
				return "Linked";
		}

		#endregion
	}
}
