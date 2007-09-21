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

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for login.
	/// </summary>
	public partial class login : YAF.Classes.Base.ForumPage
	{

		public login()
			: base( "LOGIN" )
		{ 
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !CanLogin )
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.forum );

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( GetText( "title" ) );

				//Login1.CreateUserText = "Sign up for a new account.";
				//Login1.CreateUserUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.register );
				Login1.PasswordRecoveryText = GetText( "lostpassword" );
				Login1.PasswordRecoveryUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.recoverpassword );
				Login1.FailureText = GetText( "password_error" );

				if ( Request.QueryString ["ReturnUrl"] != null )
				{
					Login1.DestinationPageUrl = Server.UrlDecode( Request.QueryString ["ReturnUrl"] );
				}
				else
				{
					Login1.DestinationPageUrl = YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum );
				}			

				// localize controls
				CheckBox rememberMe = ( CheckBox )Login1.FindControl( "RememberMe" );
				TextBox userName = ( TextBox ) Login1.FindControl( "UserName" );
				Button forumLogin = ( Button ) Login1.FindControl( "LoginButton" );
				HyperLink passwordRecovery = ( HyperLink ) Login1.FindControl( "PasswordRecovery" );
				RequiredFieldValidator usernameRequired = ( RequiredFieldValidator ) Login1.FindControl( "UsernameRequired" );
				RequiredFieldValidator passwordRequired = ( RequiredFieldValidator ) Login1.FindControl( "PasswordRequired" );

				usernameRequired.ToolTip = usernameRequired.ErrorMessage = GetText( "REGISTER", "NEED_USERNAME" );
				passwordRequired.ToolTip = passwordRequired.ErrorMessage = GetText( "REGISTER", "NEED_PASSWORD" );

				rememberMe.Text = GetText( "auto" );
				forumLogin.Text = GetText( "forum_login" );
				passwordRecovery.Text = GetText( "lostpassword" );
				passwordRecovery.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.recoverpassword );

				// set the focus using Damien McGivern client-side focus class
				YAF.Classes.UI.ClientSideFocus.setFocus( userName );

				DataBind();
			}
		}
	}
}
