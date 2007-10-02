/* Yet Another Forum.NET
 * Copyright (C) 2006-2007 Jaben Cargman
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

namespace YAF.Controls
{
	public partial class EditUsersGroups : YAF.Classes.Base.BaseUserControl
	{
		protected void Page_Load( object sender, EventArgs e )
		{
			if ( !IsPostBack )
			{
				BindData();
			}
		}

    protected int ThisUserID
    {
      get
      {
        return int.Parse( Request.QueryString ["u"] );
      }
    }

		private void BindData()
		{
      UserGroups.DataSource = YAF.Classes.Data.DB.group_member( PageContext.PageBoardID, ThisUserID );
			DataBind();
		}

		protected bool IsMember( object o )
		{
			return long.Parse( o.ToString() ) > 0;
		}

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users );
		}

		protected void Save_Click( object sender, System.EventArgs e )
		{
			for ( int i = 0; i < UserGroups.Items.Count; i++ )
			{
				RepeaterItem item = UserGroups.Items [i];
				int GroupID = int.Parse( ( ( Label ) item.FindControl( "GroupID" ) ).Text );

				string roleName = string.Empty;
				using ( DataTable dt = YAF.Classes.Data.DB.group_list( PageContext.PageBoardID, GroupID ) )
				{
					foreach ( DataRow row in dt.Rows )
						roleName = ( string ) row ["Name"];
				}

				bool isChecked = ( ( CheckBox ) item.FindControl( "GroupMember" ) ).Checked;

				YAF.Classes.Data.DB.usergroup_save( ThisUserID, GroupID, isChecked );

        // update roles if this user isn't the guest
				if ( !UserMembershipHelper.IsGuestUser( ThisUserID ) )
				{
					string userName = YAF.Classes.Utils.UserMembershipHelper.GetUserNameFromID( ThisUserID );

					if ( isChecked && !Roles.IsUserInRole( userName, roleName ) )
						Roles.AddUserToRole( userName, roleName );
					else if ( !isChecked && Roles.IsUserInRole( userName, roleName ) )
						Roles.RemoveUserFromRole( userName, roleName );
				}
			}

			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users );
		}

	}
}