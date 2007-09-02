/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
		protected void Page_Load( object sender, System.EventArgs e )
		{
			// we're in the admin section...
			ProfileEditControl.InAdminPages = true;
			SignatureEditControl.InAdminPages = true;
			AvatarEditControl.InAdminPages = true;

			if ( !IsPostBack )
			{
				if ( Request.QueryString ["u"] != null )
					// TODO: write the IsUserHostAdmin function
					if ( !PageContext.IsHostAdmin ) // && IsUserHostAdmin( Convert.ToInt32( Request.QueryString ["u"] ) ) ) 
					{
						yaf_BuildLink.AccessDenied();
					}

				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "Users", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_users ) );
				PageLinks.AddLink( "Edit", "" );

				BasicEditLink.Text = "User Details";
				BasicEditLink.CommandArgument = "QuickEditView";

				GroupLink.Text = "User Groups";
				GroupLink.CommandArgument = "GroupEditControl";

				SignatureLink.Text = "Signature Edit";
				SignatureLink.CommandArgument = "SignatureEditControl";

				ProfileLink.Text = "Profile Edit";
				ProfileLink.CommandArgument = "ProfileEditControl";

				SuspendLink.Text = "Suspend User";
				SuspendLink.CommandArgument = "SuspendUserControl";

				PointsLink.Text = "User Points";
				PointsLink.CommandArgument = "UserPointsView";

				AvatarLink.Text = "Avatar Edit";
				AvatarLink.CommandArgument = "AvatarEditView";

				if ( Request.QueryString ["av"] != null )
				{
					// show the avatar section...
					UserAdminMultiView.SetActiveView( AvatarEditView );
				}
			}
		}

		protected void Edit1_Click( object sender, System.EventArgs e )
		{
			UserAdminMultiView.SetActiveView( QuickEditView );
		}
		protected void Edit2_Click( object sender, System.EventArgs e )
		{
			UserAdminMultiView.SetActiveView( GroupsEditView );
		}
		protected void Edit3_Click( object sender, System.EventArgs e )
		{
			UserAdminMultiView.SetActiveView( ProfileEditView );
		}
		protected void Edit4_Click( object sender, System.EventArgs e )
		{
			UserAdminMultiView.SetActiveView( SignatureEditView );
		}
		protected void Edit5_Click( object sender, System.EventArgs e )
		{
			UserAdminMultiView.SetActiveView( SuspendUserView );
		}
		protected void Edit6_Click( object sender, System.EventArgs e )
		{
			UserAdminMultiView.SetActiveView( UserPointsView );
		}
		protected void Edit7_Click( object sender, System.EventArgs e )
		{
			UserAdminMultiView.SetActiveView( AvatarEditView );
		}
	}
}
