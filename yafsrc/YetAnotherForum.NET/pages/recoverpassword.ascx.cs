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
using YAF.Classes.Data;
using YAF.Controls;

namespace YAF.Pages // YAF.Pages
{
  public partial class recoverpassword : YAF.Classes.Base.ForumPage
  {
    public recoverpassword()
      : base( "RECOVER_PASSWORD" )
    {
    }

    protected void Page_Load( object sender, EventArgs e )
    {
		// Ederon : guess we don't need this if anymore
		//if ( !CanLogin )
        //YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.forum );

      if ( !IsPostBack )
      {
        PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
        PageLinks.AddLink( GetText( "TITLE" ) );

        // handle localization
        RequiredFieldValidator usernameRequired = ( RequiredFieldValidator ) PasswordRecovery1.UserNameTemplateContainer.FindControl( "UserNameRequired" );
        RequiredFieldValidator answerRequired = ( RequiredFieldValidator ) PasswordRecovery1.QuestionTemplateContainer.FindControl( "AnswerRequired" );

        usernameRequired.ToolTip = usernameRequired.ErrorMessage = GetText( "REGISTER", "NEED_USERNAME" );
        answerRequired.ToolTip = answerRequired.ErrorMessage = GetText( "REGISTER", "NEED_ANSWER" );

        ( ( Button ) PasswordRecovery1.UserNameTemplateContainer.FindControl( "SubmitButton" ) ).Text = GetText( "SUBMIT" );
        ( ( Button ) PasswordRecovery1.QuestionTemplateContainer.FindControl( "SubmitButton" ) ).Text = GetText( "SUBMIT" );
        ( ( Button ) PasswordRecovery1.SuccessTemplateContainer.FindControl( "SubmitButton" ) ).Text = GetText( "BACK" );

        PasswordRecovery1.UserNameFailureText = GetText( "USERNAME_FAILURE" );
        PasswordRecovery1.GeneralFailureText = GetText( "GENERAL_FAILURE" );
        PasswordRecovery1.QuestionFailureText = GetText( "QUESTION_FAILURE" );

        DataBind();
      }
    }

    protected void SubmitButton_Click( object sender, EventArgs e )
    {
      YafBuildLink.Redirect( ForumPages.login );
    }

    protected void PasswordRecovery1_SendingMail( object sender, MailMessageEventArgs e )
    {
      // we are going to handle the sending of this e-mail
      e.Cancel = true;

      // get the username and password from the body
      string body = e.Message.Body;

      // remove first line...
      body = body.Remove( 0, body.IndexOf( '\n' ) + 1 );
      // remove "Username: "
      body = body.Remove( 0, body.IndexOf( ": " ) + 2 );
      // get first line which is the username
      string userName = body.Substring( 0, body.IndexOf( '\n' ) );
      // delete that same line...
      body = body.Remove( 0, body.IndexOf( '\n' ) + 1 );
      // remove the "Password: " part
      body = body.Remove( 0, body.IndexOf( ": " ) + 2 );
      // the rest is the password...
      string password = body.Substring( 0, body.IndexOf( '\n' ) );
      
      // get the e-mail ready from the real template.
      body = General.ReadTemplate( "passwordretrieval.txt" );
      string subject = String.Format( GetText( "PASSWORDRETRIEVAL_EMAIL_SUBJECT" ), PageContext.BoardSettings.Name );

      body = body.Replace( "{username}", userName );
      body = body.Replace( "{password}", password );
      body = body.Replace( "{forumname}", PageContext.BoardSettings.Name );
      body = body.Replace( "{forumlink}", String.Format( "{0}", YafForumInfo.ForumURL ) );

      General.SendMail( new System.Net.Mail.MailAddress( PageContext.BoardSettings.ForumEmail, PageContext.BoardSettings.Name ),
                        e.Message.To[0], subject, body );
    }
  }
}