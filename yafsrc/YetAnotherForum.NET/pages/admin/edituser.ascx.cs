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
using System.Web.Security;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for edituser.
	/// </summary>
	public partial class edituser : YAF.Classes.Base.AdminPage
	{
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

		protected void Page_Load( object sender, System.EventArgs e )
		{
			// we're in the admin section...
			ProfileEditControl.InAdminPages = true;
			SignatureEditControl.InAdminPages = true;
			AvatarEditControl.InAdminPages = true;

			PageContext.QueryIDs = new QueryStringIDHelper( "u", true );

			DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, CurrentUserID, null );

			if ( dt.Rows.Count == 1 )
			{
				DataRow userRow = dt.Rows [0];

				// do admin permission check...
				if ( !PageContext.IsHostAdmin && IsUserHostAdmin( userRow ) )
				{
					// user is not host admin and is attempted to edit host admin account...
					YafBuildLink.AccessDenied();
				}

				if ( !IsPostBack )
				{
					PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
					PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
					PageLinks.AddLink( "Users", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_users ) );
					PageLinks.AddLink( String.Format( "Edit User \"{0}\"", userRow ["Name"].ToString() ) );

					// do a quick user membership sync...
					MembershipUser user = UserMembershipHelper.GetMembershipUser( CurrentUserID );
					RoleMembershipHelper.UpdateForumUser( user, PageContext.PageBoardID );
				}
			}
		}

		protected bool IsUserHostAdmin( DataRow userRow )
		{
			UserFlags userFlags = new UserFlags( userRow ["Flags"] );
			return userFlags.IsHostAdmin;
		}
	}
}
