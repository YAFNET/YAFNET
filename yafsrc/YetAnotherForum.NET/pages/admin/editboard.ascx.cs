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
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class editboard : YAF.Classes.Base.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "Boards", "" );

				BindData();
				if ( Request.QueryString ["b"] != null )
				{
					AdminInfo.Visible = false;
					using ( DataTable dt = YAF.Classes.Data.DB.board_list( Request.QueryString ["b"] ) )
					{
						DataRow row = dt.Rows [0];
						Name.Text = ( string ) row ["Name"];
						AllowThreaded.Checked = ( bool ) row ["AllowThreaded"];
					}
				}
				else
				{
					UserName.Text = User.UserName;
					UserEmail.Text = User.Email;
				}
			}
		}

		private void BindData()
		{
			DataBind();
		}

		protected void Save_Click( object sender, System.EventArgs e )
		{
			if ( Name.Text.Trim().Length == 0 )
			{
				PageContext.AddLoadMessage( "You must enter a name for the board." );
				return;
			}
			if ( Request.QueryString ["b"] == null )
			{
				if ( UserName.Text.Trim().Length == 0 )
				{
					PageContext.AddLoadMessage( "You must enter the name of a administrator user." );
					return;
				}
				if ( UserEmail.Text.Trim().Length == 0 )
				{
					PageContext.AddLoadMessage( "You must enter the email address of the administrator user." );
					return;
				}
				if ( UserPass1.Text.Trim().Length == 0 )
				{
					PageContext.AddLoadMessage( "You must enter a password for the administrator user." );
					return;
				}
				if ( UserPass1.Text != UserPass2.Text )
				{
					PageContext.AddLoadMessage( "The passwords don't match." );
					return;
				}
			}

			if ( Request.QueryString ["b"] != null )
			{
                // Save current board settings
				YAF.Classes.Data.DB.board_save( Request.QueryString ["b"], Name.Text, AllowThreaded.Checked );
			}
			else
			{
                // Create board
                // MEK says : Purposefully set MembershipAppName without including RolesAppName yet, as the current providers don't support different Appnames.
                this.CreateBoard(UserName.Text, UserPass1.Text,UserEmail.Text, UserPasswordQuestion.Text, UserPasswordAnswer.Text, Name.Text, BoardMembershipAppName.Text, BoardMembershipAppName.Text);
			}

			// Done
			PageContext.BoardSettings = null;
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_boards );
		}

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_boards );
		}

		protected void BindData_AccessMaskID( object sender, System.EventArgs e )
		{
			( ( DropDownList ) sender ).DataSource = YAF.Classes.Data.DB.accessmask_list( PageContext.PageBoardID, null );
			( ( DropDownList ) sender ).DataValueField = "AccessMaskID";
			( ( DropDownList ) sender ).DataTextField = "Name";
		}

		protected void SetDropDownIndex( object sender, System.EventArgs e )
		{
			try
			{
				DropDownList list = ( DropDownList ) sender;
				list.Items.FindByValue( list.Attributes ["value"] ).Selected = true;
			}
			catch ( Exception )
			{
			}
		}

        protected void CreateBoard(string adminName, string adminPassword, string adminEmail, string adminPasswordQuestion, string adminPasswordAnswer, string boardName, string boardMembershipAppName, string boardRolesAppName)
        {
            // Store current App Names
            string currentMembershipAppName = Membership.ApplicationName;
            string currentRolesAppName = Roles.ApplicationName;

            // Change App Names for new board
            Membership.ApplicationName = boardMembershipAppName;
            Roles.ApplicationName = boardRolesAppName;

            // Create new admin users
            MembershipCreateStatus createStatus;
            MembershipUser newAdmin = Membership.CreateUser(adminName, adminPassword, adminEmail, adminPasswordQuestion, adminPasswordAnswer, true, out createStatus);
            if (createStatus != MembershipCreateStatus.Success)
                throw new ApplicationException(string.Format("Create User Failed: {0}", GetMembershipErrorMessage(createStatus)));

            // Create groups required for the new board
            Roles.CreateRole("Administrators");
            Roles.CreateRole("Registered");

            // Add new admin users to group
            Roles.AddUserToRole(newAdmin.UserName, "Administrators");

            // Create Board
            YAF.Classes.Data.DB.board_create(newAdmin.UserName, newAdmin.ProviderUserKey, boardName, boardMembershipAppName, boardRolesAppName);

            // Return application name to as they were before.
            Membership.ApplicationName = currentMembershipAppName;
            Roles.ApplicationName = currentRolesAppName;

        }

        protected string GetMembershipErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
	}
}
