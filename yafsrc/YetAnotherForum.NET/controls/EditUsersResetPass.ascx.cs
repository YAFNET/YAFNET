/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Net.Mail;
  using System.Web.Security;
  using System.Web.UI.WebControls;

  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Types.Interfaces.Extensions;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The edit users reset pass.
  /// </summary>
  public partial class EditUsersResetPass : BaseUserControl
  {
    #region Properties

    /// <summary>
    ///   Gets CurrentUserID.
    /// </summary>
    public long? CurrentUserID
    {
      get
      {
        return this.PageContext.QueryIDs["u"];
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

        if (!this.PageContext.IsAdmin)
        {
            YafBuildLink.AccessDenied();
        }

        if (!this.IsPostBack)
        {
            this.rblPasswordResetFunction.Items.Add(new ListItem(this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "PASS_OPTION_RESET"), "reset", true));
            this.rblPasswordResetFunction.Items.Add(new ListItem(this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "PASS_OPTION_CHANGE"), "change"));

            this.rblPasswordResetFunction.SelectedIndex = 0;


            this.btnResetPassword.Text = this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "RESET_PASS");
            this.btnChangePassword.Text = this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "CHANGE_PASS");

            this.lblPassRequirements.Text =
                this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "PASS_REQUIREMENT").FormatWith(
                    this.Get<MembershipProvider>().MinRequiredPasswordLength,
                    this.Get<MembershipProvider>().MinRequiredNonAlphanumericCharacters);

            this.PasswordValidator.ErrorMessage = this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_NEW_PASS");
            this.RequiredFieldValidator1.ErrorMessage = this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_CONFIRM_PASS");
            this.CompareValidator1.ErrorMessage = this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_PASS_NOTMATCH");

            if (!this.Get<MembershipProvider>().EnablePasswordReset)
            {
                this.PasswordResetErrorHolder.Visible = true;
                this.btnResetPassword.Enabled = false;
                this.rblPasswordResetFunction.Enabled = false;
            }
        }

        this.BindData();
    }

      /// <summary>
    /// The toggle change pass ui enabled.
    /// </summary>
    /// <param name="status">
    /// The status.
    /// </param>
    protected void ToggleChangePassUIEnabled(bool status)
    {
      this.ChangePasswordHolder.Visible = status;
      this.btnChangePassword.Visible = status;
      this.btnResetPassword.Visible = !status;
    }

    /// <summary>
    /// The btn change password_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnChangePassword_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.Page.IsValid)
      {
        return;
      }

      // change password...
      try
      {
        MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID.Value);

        if (user != null)
        {
          // new password...
          string newPass = this.txtNewPassword.Text.Trim();

          // reset the password...
          user.UnlockUser();
          string tempPass = user.ResetPassword();

          // change to new password...
          user.ChangePassword(tempPass, newPass);

          if (this.chkEmailNotify.Checked)
          {
            // email a notification...
            var passwordRetrieval = new YafTemplateEmail("PASSWORDRETRIEVAL");

            string subject =
              this.Get<ILocalization>().GetText("RECOVER_PASSWORD", "PASSWORDRETRIEVAL_EMAIL_SUBJECT").FormatWith(
                this.PageContext.BoardSettings.Name);

            passwordRetrieval.TemplateParams["{username}"] = user.UserName;
            passwordRetrieval.TemplateParams["{password}"] = newPass;
            passwordRetrieval.TemplateParams["{forumname}"] = this.PageContext.BoardSettings.Name;
            passwordRetrieval.TemplateParams["{forumlink}"] = "{0}".FormatWith(YafForumInfo.ForumURL);

            passwordRetrieval.SendEmail(new MailAddress(user.Email, user.UserName), subject, true);

            this.PageContext.AddLoadMessage(this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_CHANGED_NOTI"));
          }
          else
          {
            this.PageContext.AddLoadMessage(this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_CHANGED"));
          }
        }
      }
      catch (Exception x)
      {
        this.PageContext.AddLoadMessage("Exception: {0}".FormatWith(x.Message));
      }
    }

    /// <summary>
    /// The btn reset password_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnResetPassword_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // reset password...
      try
      {
        MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID.Value);

        if (user != null)
        {
          // reset the password...
          user.UnlockUser();
          string newPassword = user.ResetPassword();

          // email a notification...
          var passwordRetrieval = new YafTemplateEmail("PASSWORDRETRIEVAL");

          string subject =
            this.Get<ILocalization>().GetText("RECOVER_PASSWORD", "PASSWORDRETRIEVAL_EMAIL_SUBJECT").FormatWith(
              this.PageContext.BoardSettings.Name);

          passwordRetrieval.TemplateParams["{username}"] = user.UserName;
          passwordRetrieval.TemplateParams["{password}"] = newPassword;
          passwordRetrieval.TemplateParams["{forumname}"] = this.PageContext.BoardSettings.Name;
          passwordRetrieval.TemplateParams["{forumlink}"] = "{0}".FormatWith(YafForumInfo.ForumURL);

          passwordRetrieval.SendEmail(new MailAddress(user.Email, user.UserName), subject, true);

          this.PageContext.AddLoadMessage(this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_RESET"));
        }
      }
      catch (Exception x)
      {
        this.PageContext.AddLoadMessage("Exception: {0}".FormatWith(x.Message));
      }
    }

    /// <summary>
    /// The rbl password reset function_ selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void rblPasswordResetFunction_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.ToggleChangePassUIEnabled(this.rblPasswordResetFunction.SelectedValue == "change");
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
    }

    #endregion
  }
}