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
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Control handling user invitations to forum (i.e. granting permissions by admin/moderator).
	/// </summary>
	public partial class mod_forumuser : YAF.Classes.Core.ForumPage
	{
		/* Construction & Destruction */
		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public mod_forumuser() : base("MOD_FORUMUSER") { }

		#endregion


		/* Overriden Base Methods */
		#region MyRegion

		/// <summary>
		/// Creates page links for this page.
		/// </summary>
		protected override void CreatePageLinks()
		{
			if (PageContext.Settings.LockedForum == 0)
			{
				// forum index
				PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
				// category
				PageLinks.AddLink(PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));
			}
			// forum page
			PageLinks.AddForumLinks(PageContext.PageForumID);
			// currect page
			PageLinks.AddLink(GetText("TITLE"), "");
		}

		#endregion


		/* Event Handlers */
		#region Page Events

		/// <summary>
		/// Handles page load event.
		/// </summary>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// only moderators/admins are allowed in
			if (!PageContext.ForumModeratorAccess)
				YafBuildLink.AccessDenied();

			// do not repeat on postbact
			if (!IsPostBack)
			{
				// create page links
				CreatePageLinks();

				// load localized texts for buttons
				FindUsers.Text = GetText("FIND");
				Update.Text = GetText("UPDATE");
				Cancel.Text = GetText("CANCEL");

				// bind data
				DataBind();

				// if there is concrete user being handled
				if (Request.QueryString["u"] != null)
				{
					using (DataTable dt = YAF.Classes.Data.DB.userforum_list(Request.QueryString["u"], PageContext.PageForumID))
					{
						foreach (DataRow row in dt.Rows)
						{
							// set username and disable its editing
							UserName.Text = row["Name"].ToString();
							UserName.Enabled = false;
							// we don't need to find users now
							FindUsers.Visible = false;
							// get access mask for this user
							AccessMaskID.Items.FindByValue(row["AccessMaskID"].ToString()).Selected = true;
						}
					}
				}
			}
		}

		#endregion

		#region Button Click Events

		/// <summary>
		/// Handles FindUsers button click event.
		/// </summary>
		protected void FindUsers_Click(object sender, System.EventArgs e)
		{
			// we need at least two characters to search user by
			if (UserName.Text.Length < 2) return;

			// get found users
			using (DataTable dt = YAF.Classes.Data.DB.user_find(PageContext.PageBoardID, true, UserName.Text, null))
			{
				// have we found anyone?
				if (dt.Rows.Count > 0)
				{
					// set and enable user dropdown, disable text box
					ToList.DataSource = dt;
					ToList.DataValueField = "UserID";
					ToList.DataTextField = "Name";
					//ToList.SelectedIndex = 0;
					ToList.Visible = true;
					UserName.Visible = false;
					FindUsers.Visible = false;
				}
				// bind data (is this necessary?)
				base.DataBind();
			}
		}

		/// <summary>
		/// Handles click event of Update button.
		/// </summary>
		protected void Update_Click(object sender, System.EventArgs e)
		{
			// no user was specified
			if (UserName.Text.Length <= 0)
			{
				PageContext.AddLoadMessage(GetText("NO_SUCH_USER"));
				return;
			}

			// if we choose user from drop down, set selected value to text box
			if (ToList.Visible)
				UserName.Text = ToList.SelectedItem.Text;

			// we need to verify user exists
			using (DataTable dt = YAF.Classes.Data.DB.user_find(PageContext.PageBoardID, false, UserName.Text, null))
			{
				// there is no such user or reference is ambiugous
				if (dt.Rows.Count != 1)
				{
					PageContext.AddLoadMessage(GetText("NO_SUCH_USER"));
					return;
				}
				// we cannot use guest user here
				else if (SqlDataLayerConverter.VerifyInt32(dt.Rows[0]["IsGuest"]) > 0)
				{
					PageContext.AddLoadMessage(GetText("NOT_GUEST"));
					return;
				}

				// save permission
				YAF.Classes.Data.DB.userforum_save(dt.Rows[0]["UserID"], PageContext.PageForumID, AccessMaskID.SelectedValue);
				// redirect to forum moderation page
				YafBuildLink.Redirect(ForumPages.moderate, "f={0}", PageContext.PageForumID);

				// clear moderatorss cache
				YafCache.Current.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));
			}
		}

		/// <summary>
		/// Handles click event of cancel button.
		/// </summary>
		protected void Cancel_Click(object sender, System.EventArgs e)
		{
			// redirect to forum moderation page
			YafBuildLink.Redirect(ForumPages.moderate, "f={0}", PageContext.PageForumID);
		}

		#endregion


		/* Data Bidining & Formatting */
		#region Data Bidining

		public override void DataBind()
		{
			// load data
			DataTable dt;
			
			// only admin can assign all access masks
			if (!PageContext.IsAdmin)
			{
				// do not include access masks with this flags set
				int flags = (int)AccessFlags.Flags.ModeratorAccess;

				// non-admins cannot assign moderation access masks
				dt = YAF.Classes.Data.DB.accessmask_list(PageContext.PageBoardID, null, flags);
			}
			else
			{
				dt = YAF.Classes.Data.DB.accessmask_list(PageContext.PageBoardID, null);
			}

			// setup datasource for access masks dropdown
			AccessMaskID.DataSource = dt;
			AccessMaskID.DataValueField = "AccessMaskID";
			AccessMaskID.DataTextField = "Name";

			base.DataBind();
		}

		#endregion

	}
}
