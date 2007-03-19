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
using System.Globalization;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for register.
	/// </summary>
	public partial class register : YAF.Classes.Base.ForumPage
	{

		public register()
			: base( "REGISTER" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !CanLogin || PageContext.BoardSettings.DisableRegistrations )
				yaf_BuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( GetText("TITLE") );

				Control createUserTemplateRef = CreateUserWizard1.CreateUserStep.ContentTemplateContainer;

				CompareValidator passwordNoMatch = ( CompareValidator ) createUserTemplateRef.FindControl( "PasswordCompare" );
				RequiredFieldValidator usernameRequired = ( RequiredFieldValidator ) createUserTemplateRef.FindControl( "UserNameRequired" );
				RequiredFieldValidator passwordRequired = ( RequiredFieldValidator ) createUserTemplateRef.FindControl( "PasswordRequired" );
				RequiredFieldValidator confirmPasswordRequired = ( RequiredFieldValidator ) createUserTemplateRef.FindControl( "ConfirmPasswordRequired" );
				RequiredFieldValidator emailRequired = ( RequiredFieldValidator ) createUserTemplateRef.FindControl( "EmailRequired" );
				RequiredFieldValidator questionRequired = ( RequiredFieldValidator ) createUserTemplateRef.FindControl( "QuestionRequired" );
				RequiredFieldValidator answerRequired = ( RequiredFieldValidator ) createUserTemplateRef.FindControl( "AnswerRequired" );

				usernameRequired.ToolTip = usernameRequired.ErrorMessage = GetText( "NEED_USERNAME" );
				passwordRequired.ToolTip = passwordRequired.ErrorMessage = GetText( "NEED_PASSWORD" );
				confirmPasswordRequired.ToolTip = confirmPasswordRequired.ErrorMessage = GetText( "RETYPE_PASSWORD" );
				passwordNoMatch.ToolTip = passwordNoMatch.ErrorMessage = GetText( "NEED_MATCH" );
				emailRequired.ToolTip = emailRequired.ErrorMessage = GetText( "NEED_EMAIL" );
				questionRequired.ToolTip = questionRequired.ErrorMessage = GetText( "NEED_QUESTION" );
				answerRequired.ToolTip = answerRequired.ErrorMessage = GetText( "NEED_ANSWER" );

				DataBind();				
			}
		}

		static public string CreatePassword( int length )
		{
			string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#%&()@${[]}";
			string res = "";
			Random rnd = new Random();
			while ( 0 < length-- )
				res += valid [rnd.Next( valid.Length )];
			return res;
		}

		protected void CreateUserWizard1_PreviousButtonClick( object sender, WizardNavigationEventArgs e )
		{
			// if they clicked declined, redirect to the main page
			if ( e.CurrentStepIndex == 0 )
			{
				yaf_BuildLink.Redirect( ForumPages.forum );
			}
		}

		protected void CreateUserWizard1_CreateUserError( object sender, CreateUserErrorEventArgs e )
		{
			string createUserError = "";
			// find the type of error
			switch ( e.CreateUserError )
			{
				case MembershipCreateStatus.DuplicateEmail:
					createUserError = GetText( "ALREADY_REGISTERED" );
					break;
				case MembershipCreateStatus.DuplicateUserName:
					createUserError = GetText( "ALREADY_REGISTERED" );
					break;
				case MembershipCreateStatus.InvalidEmail:
					createUserError = GetText( "BAD_EMAIL" );
					break;
				case MembershipCreateStatus.InvalidPassword:
					createUserError = GetText( "BAD_PASSWORD" );
					break;
				case MembershipCreateStatus.InvalidQuestion:
					createUserError = GetText( "INVALID_QUESTION" );
					break;
				case MembershipCreateStatus.InvalidUserName:
					createUserError = GetText( "INVALID_USERNAME" );
					break;
				case MembershipCreateStatus.InvalidAnswer:
					createUserError = GetText( "INVALID_ANSWER" );
					break;
				case MembershipCreateStatus.InvalidProviderUserKey:
					createUserError = "Invalid provider user key.";
					break;
				case MembershipCreateStatus.DuplicateProviderUserKey:
					createUserError = "Duplicate provider user key.";
					break;
				case MembershipCreateStatus.ProviderError:
					createUserError = "Provider Error";
					break;
				case MembershipCreateStatus.UserRejected:
					createUserError = "User creation failed: Reason is defined by the provider.";
					break;
			}
			//Display the failure message in a client-side alert box
			Page.ClientScript.RegisterStartupScript( Page.GetType(), "CreateUserError", String.Format( "alert('{0}');", createUserError.Replace( "'", "\'" ) ), true );

		}
	}
}
