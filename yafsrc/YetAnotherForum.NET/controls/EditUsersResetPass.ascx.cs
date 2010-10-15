/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System.Web.Security;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class EditUsersResetPass : YAF.Classes.Core.BaseUserControl
	{
		public long? CurrentUserID
		{
			get
			{
				return PageContext.QueryIDs ["u"];
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
				lblPassRequirements.Text = "{0} minimum length. {1} minimum non-alphanumeric characters ($#@!).".FormatWith(this.PageContext.CurrentMembership.MinRequiredPasswordLength, this.PageContext.CurrentMembership.MinRequiredNonAlphanumericCharacters);

				if ( !PageContext.CurrentMembership.EnablePasswordReset )
				{
					PasswordResetErrorHolder.Visible = true;
					btnResetPassword.Enabled = false;
					rblPasswordResetFunction.Enabled = false;
				}
			}

			BindData();
		}

		private void BindData()
		{

		}

		protected void btnChangePassword_Click( object sender, EventArgs e )
		{
			if (!Page.IsValid)
			{
				return;
			}

			// change password...
			try
			{
				MembershipUser user = UserMembershipHelper.GetMembershipUserById(CurrentUserID.Value);

				if (user != null)
				{
					// new password...
					string newPass = txtNewPassword.Text.Trim();
					// reset the password...
					user.UnlockUser();
					string tempPass = user.ResetPassword();
					// change to new password...
					user.ChangePassword(tempPass, newPass);

					if (chkEmailNotify.Checked)
					{
						// email a notification...
						YafTemplateEmail passwordRetrieval = new YafTemplateEmail("PASSWORDRETRIEVAL");

						string subject = this.PageContext.Localization.GetText("RECOVER_PASSWORD", "PASSWORDRETRIEVAL_EMAIL_SUBJECT").FormatWith(this.PageContext.BoardSettings.Name);

						passwordRetrieval.TemplateParams["{username}"] = user.UserName;
						passwordRetrieval.TemplateParams["{password}"] = newPass;
						passwordRetrieval.TemplateParams["{forumname}"] = PageContext.BoardSettings.Name;
						passwordRetrieval.TemplateParams["{forumlink}"] = "{0}".FormatWith(YafForumInfo.ForumURL);

						passwordRetrieval.SendEmail(new System.Net.Mail.MailAddress(user.Email, user.UserName), subject, true);

						PageContext.AddLoadMessage("User Password Changed and Notification Email Sent");
					}
					else
					{
						PageContext.AddLoadMessage("User Password Changed");
					}
				}
			}
			catch (Exception x)
			{
				PageContext.AddLoadMessage("Exception: " + x.Message);
			}			
		}

		protected void btnResetPassword_Click( object sender, EventArgs e )
		{
			// reset password...
			try
			{
				MembershipUser user = UserMembershipHelper.GetMembershipUserById(CurrentUserID.Value);

				if (user != null)
				{
					// reset the password...
					user.UnlockUser();
					string newPassword = user.ResetPassword();

					// email a notification...
					YafTemplateEmail passwordRetrieval = new YafTemplateEmail("PASSWORDRETRIEVAL");

					string subject = this.PageContext.Localization.GetText("RECOVER_PASSWORD", "PASSWORDRETRIEVAL_EMAIL_SUBJECT").FormatWith(this.PageContext.BoardSettings.Name);

					passwordRetrieval.TemplateParams["{username}"] = user.UserName;
					passwordRetrieval.TemplateParams["{password}"] = newPassword;
					passwordRetrieval.TemplateParams["{forumname}"] = PageContext.BoardSettings.Name;
					passwordRetrieval.TemplateParams["{forumlink}"] = "{0}".FormatWith(YafForumInfo.ForumURL);

					passwordRetrieval.SendEmail(new System.Net.Mail.MailAddress(user.Email, user.UserName), subject, true);

					PageContext.AddLoadMessage("User Password Reset and Notification Email Sent");
				}
			}
			catch (Exception x)
			{
				PageContext.AddLoadMessage("Exception: " + x.Message);
			}				
		}

		protected void rblPasswordResetFunction_SelectedIndexChanged( object sender, EventArgs e )
		{
			ToggleChangePassUIEnabled( rblPasswordResetFunction.SelectedValue == "change" );
		}

		protected void ToggleChangePassUIEnabled( bool status )
		{
			ChangePasswordHolder.Visible = status;
			btnChangePassword.Visible = status;
			btnResetPassword.Visible = !status;
		}
	}
}