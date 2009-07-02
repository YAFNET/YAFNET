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
using System.Web.UI.WebControls;
using YAF.Classes.Utils;

namespace YAF.Pages
{
  /// <summary>
  /// Summary description for cp_subscriptions.
  /// </summary>
  public partial class cp_changepassword : YAF.Classes.Base.ForumPage
  {

    public cp_changepassword()
      : base( "CP_CHANGEPASSWORD" )
    {
    }

    private void Page_Load( object sender, System.EventArgs e )
    {
      if ( User == null )
      {
				RedirectNoAccess();
      }

			if ( !PageContext.BoardSettings.AllowPasswordChange && !( PageContext.IsAdmin || PageContext.IsForumModerator ) )
			{
				// Not accessbile...
				YafBuildLink.AccessDenied();
			}

      if ( !IsPostBack )
      {
        PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
        PageLinks.AddLink( PageContext.PageUserName, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_profile ) );
        PageLinks.AddLink( GetText( "TITLE" ) );

        RequiredFieldValidator oldPasswordRequired = ( RequiredFieldValidator ) ChangePassword1.ChangePasswordTemplateContainer.FindControl( "CurrentPasswordRequired" );
        RequiredFieldValidator newPasswordRequired = ( RequiredFieldValidator ) ChangePassword1.ChangePasswordTemplateContainer.FindControl( "NewPasswordRequired" );
        RequiredFieldValidator confirmNewPasswordRequired = ( RequiredFieldValidator ) ChangePassword1.ChangePasswordTemplateContainer.FindControl( "ConfirmNewPasswordRequired" );
        CompareValidator passwordsEqual = ( CompareValidator ) ChangePassword1.ChangePasswordTemplateContainer.FindControl( "NewPasswordCompare" );

        oldPasswordRequired.ToolTip = oldPasswordRequired.ErrorMessage = GetText( "NEED_OLD_PASSWORD" );
        newPasswordRequired.ToolTip = newPasswordRequired.ErrorMessage = GetText( "NEED_NEW_PASSWORD" );
        confirmNewPasswordRequired.ToolTip = confirmNewPasswordRequired.ErrorMessage = GetText( "NEED_NEW_CONFIRM_PASSWORD" );
        passwordsEqual.ToolTip = passwordsEqual.ErrorMessage = GetText( "NO_PASSWORD_MATCH" );

        ( ( Button ) ChangePassword1.ChangePasswordTemplateContainer.FindControl( "ChangePasswordPushButton" ) ).Text = GetText( "CHANGE_BUTTON" );
        ( ( Button ) ChangePassword1.ChangePasswordTemplateContainer.FindControl( "CancelPushButton" ) ).Text = GetText( "CANCEL" );
        ( ( Button ) ChangePassword1.SuccessTemplateContainer.FindControl( "ContinuePushButton" ) ).Text = GetText( "CONTINUE" );

        // make failure text...
        // 1. Password incorrect or New Password invalid.
        // 2. New Password length minimum: {0}.
        // 3. Non-alphanumeric characters required: {1}.
        string failureText = GetText( "PASSWORD_INCORRECT" );
        failureText += "<br/>" + GetText( "PASSWORD_BAD_LENGTH" );
        failureText += "<br/>" + GetText( "PASSWORD_NOT_COMPLEX" );

        ChangePassword1.ChangePasswordFailureText = failureText;

        DataBind();
      }
    }

    protected void CancelPushButton_Click( object sender, EventArgs e )
    {
      YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
    }

    protected void ContinuePushButton_Click( object sender, EventArgs e )
    {
      YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
    }
  }
}
