/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Change Password Page.
    /// </summary>
    public partial class cp_changepassword : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="cp_changepassword"/> class.
        /// </summary>
        public cp_changepassword()
            : base("CP_CHANGEPASSWORD")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The cancel push button_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void CancelPushButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.cp_profile);
        }

        /// <summary>
        /// The continue push button_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ContinuePushButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.cp_profile);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (Config.IsDotNetNuke)
            {
                // Not accessbile...
                YafBuildLink.AccessDenied();
            } 

            if (!this.Get<YafBoardSettings>().AllowPasswordChange &&
                !(this.PageContext.IsAdmin || this.PageContext.IsForumModerator))
            {
                // Not accessbile...
                YafBuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.Get<YafBoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                YafBuildLink.GetLink(ForumPages.cp_profile));
            this.PageLinks.AddLink(this.GetText("TITLE"));

            var oldPasswordRequired =
                (RequiredFieldValidator)
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControl("CurrentPasswordRequired");
            var newPasswordRequired =
                (RequiredFieldValidator)
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControl("NewPasswordRequired");
            var confirmNewPasswordRequired =
                (RequiredFieldValidator)
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControl("ConfirmNewPasswordRequired");
            var passwordsEqual =
                (CompareValidator)this.ChangePassword1.ChangePasswordTemplateContainer.FindControl("NewPasswordCompare");
            var passwordsNotEqual =
                (CompareValidator)
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControl("NewOldPasswordCompare");

            oldPasswordRequired.ToolTip = oldPasswordRequired.ErrorMessage = this.GetText("NEED_OLD_PASSWORD");
            newPasswordRequired.ToolTip = newPasswordRequired.ErrorMessage = this.GetText("NEED_NEW_PASSWORD");
            confirmNewPasswordRequired.ToolTip =
                confirmNewPasswordRequired.ErrorMessage = this.GetText("NEED_NEW_CONFIRM_PASSWORD");
            passwordsEqual.ToolTip = passwordsEqual.ErrorMessage = this.GetText("NO_PASSWORD_MATCH");
            passwordsNotEqual.ToolTip = passwordsNotEqual.ErrorMessage = this.GetText("PASSWORD_NOT_NEW");

            ((Button)this.ChangePassword1.ChangePasswordTemplateContainer.FindControl("ChangePasswordPushButton")).Text
                = this.GetText("CHANGE_BUTTON");
            ((Button)this.ChangePassword1.ChangePasswordTemplateContainer.FindControl("CancelPushButton")).Text =
                this.GetText("CANCEL");
            ((Button)this.ChangePassword1.SuccessTemplateContainer.FindControl("ContinuePushButton")).Text =
                this.GetText("CONTINUE");

            // make failure text...
            // 1. Password incorrect or New Password invalid.
            // 2. New Password length minimum: {0}.
            // 3. Non-alphanumeric characters required: {1}.
            string failureText = this.GetText("PASSWORD_INCORRECT");
            failureText += "<br />{0}".FormatWith(this.GetText("PASSWORD_BAD_LENGTH"));
            failureText += "<br />{0}".FormatWith(this.GetText("PASSWORD_NOT_COMPLEX"));

            this.ChangePassword1.ChangePasswordFailureText = failureText;

            this.DataBind();
        }

        #endregion
    }
}