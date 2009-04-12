/* Yet Another Forum.NET
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
using System.Web.Security;
using System.Globalization;
using System.Collections.Specialized;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class EditUsersResetPass : YAF.Classes.Base.BaseUserControl
	{
		public long CurrentUserID
		{
			get
			{
				return this.PageContext.QueryIDs ["u"];
			}
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			PageContext.QueryIDs = new QueryStringIDHelper( "u", true );

			if ( !PageContext.IsAdmin )
			{
				YafBuildLink.AccessDenied();
			}

			if ( !IsPostBack )
			{
				if ( !Membership.EnablePasswordReset )
				{
					PasswordResetErrorHolder.Visible = true;
					Reset.Enabled = false;
				}
			}

			BindData();
		}

		private void BindData()
		{

		}

		protected void Reset_Click( object sender, EventArgs e )
		{
			if ( !Page.IsValid )
			{
				return;
			}

			// reset password...
			try
			{
				MembershipUser user = YAF.Classes.Utils.UserMembershipHelper.GetMembershipUser( CurrentUserID );

				if ( user != null )
				{
					// new password...
					string newPass = txtNewPassword.Text.Trim();
					// reset the password...
					user.UnlockUser();
					string tempPass = user.ResetPassword();
					// change to new password...
					user.ChangePassword( tempPass, newPass );

					if ( chkEmailNotify.Checked )
					{
						// email a notification...
						YafTemplateEmail passwordRetrieval = new YafTemplateEmail( "PASSWORDRETRIEVAL" );

						string subject = String.Format( PageContext.Localization.GetText( "RECOVER_PASSWORD", "PASSWORDRETRIEVAL_EMAIL_SUBJECT" ), PageContext.BoardSettings.Name );

						passwordRetrieval.TemplateParams ["{username}"] = user.UserName;
						passwordRetrieval.TemplateParams ["{password}"] = newPass;
						passwordRetrieval.TemplateParams ["{forumname}"] = PageContext.BoardSettings.Name;
						passwordRetrieval.TemplateParams ["{forumlink}"] = String.Format( "{0}", YafForumInfo.ForumURL );

						passwordRetrieval.SendEmail( new System.Net.Mail.MailAddress( user.Email, user.UserName ), subject, true );

						PageContext.AddLoadMessage( "User Password Reset and Notification Email Sent" );
					}
					else
					{
						PageContext.AddLoadMessage( "User Password Reset" );
					}
				}
			}
			catch ( Exception x )
			{
				PageContext.AddLoadMessage( "Exception: " + x.Message );
			}
		}
	}
}