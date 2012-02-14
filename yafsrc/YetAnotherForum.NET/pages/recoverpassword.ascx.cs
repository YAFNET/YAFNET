/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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
namespace YAF.Pages
{
    // YAF.Pages
    #region Using

    using System;
    using System.Data;
    using System.Net.Mail;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The recover Password Page Class
    /// </summary>
    public partial class recoverpassword : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "recoverpassword" /> class.
        /// </summary>
        public recoverpassword()
            : base("RECOVER_PASSWORD")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether IsProtected.
        /// </summary>
        public override bool IsProtected
        {
            get
            {
                return false;
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
            if (this.IsPostBack)
            {
                return;
            }

            this.PasswordRecovery1.MembershipProvider = Config.MembershipProvider;

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("TITLE"));

            // handle localization
            var usernameRequired =
                (RequiredFieldValidator)this.PasswordRecovery1.UserNameTemplateContainer.FindControl("UserNameRequired");
            var answerRequired =
                (RequiredFieldValidator)this.PasswordRecovery1.QuestionTemplateContainer.FindControl("AnswerRequired");

            usernameRequired.ToolTip = usernameRequired.ErrorMessage = this.GetText("REGISTER", "NEED_USERNAME");
            answerRequired.ToolTip = answerRequired.ErrorMessage = this.GetText("REGISTER", "NEED_ANSWER");

            ((Button)this.PasswordRecovery1.UserNameTemplateContainer.FindControl("SubmitButton")).Text =
                this.GetText("SUBMIT");
            ((Button)this.PasswordRecovery1.QuestionTemplateContainer.FindControl("SubmitButton")).Text =
                this.GetText("SUBMIT");
            ((Button)this.PasswordRecovery1.SuccessTemplateContainer.FindControl("SubmitButton")).Text = this.GetText(
                "BACK");

            this.PasswordRecovery1.UserNameFailureText = this.GetText("USERNAME_FAILURE");
            this.PasswordRecovery1.GeneralFailureText = this.GetText("GENERAL_FAILURE");
            this.PasswordRecovery1.QuestionFailureText = this.GetText("QUESTION_FAILURE");

            this.DataBind();
        }

        /// <summary>
        /// The password recovery 1_ answer lookup error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PasswordRecovery1_AnswerLookupError([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.LoadMessage.AddSession(this.GetText("QUESTION_FAILURE"));
        }

        /// <summary>
        /// The password recovery 1_ send mail error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PasswordRecovery1_SendMailError([NotNull] object sender, [NotNull] SendMailErrorEventArgs e)
        {
            // it will fail to send the message...
            e.Handled = true;
        }

        /// <summary>
        /// The password recovery 1_ sending mail.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PasswordRecovery1_SendingMail([NotNull] object sender, [NotNull] MailMessageEventArgs e)
        {
            // get the username and password from the body
            string body = e.Message.Body;

            // remove first line...
            body = body.Remove(0, body.IndexOf('\n') + 1);

            // remove "Username: "
            body = body.Remove(0, body.IndexOf(": ") + 2);

            // get first line which is the username
            string userName = body.Substring(0, body.IndexOf('\n'));

            // delete that same line...
            body = body.Remove(0, body.IndexOf('\n') + 1);

            // remove the "Password: " part
            body = body.Remove(0, body.IndexOf(": ") + 2);

            // the rest is the password...
            string password = body.Substring(0, body.IndexOf('\n'));

            // get the e-mail ready from the real template.
            var passwordRetrieval = new YafTemplateEmail("PASSWORDRETRIEVAL");

            string subject = this.GetTextFormatted("PASSWORDRETRIEVAL_EMAIL_SUBJECT", this.Get<YafBoardSettings>().Name);

            passwordRetrieval.TemplateParams["{username}"] = userName;
            passwordRetrieval.TemplateParams["{password}"] = password;
            passwordRetrieval.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;
            passwordRetrieval.TemplateParams["{forumlink}"] = "{0}".FormatWith(YafForumInfo.ForumURL);

            passwordRetrieval.SendEmail(e.Message.To[0], subject, true);

            // manually set to success...
            e.Cancel = true;
            this.PasswordRecovery1.TabIndex = 3;
        }

        /// <summary>
        /// The password recovery 1_ verifying answer.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PasswordRecovery1_VerifyingAnswer([NotNull] object sender, [NotNull] LoginCancelEventArgs e)
        {
            // needed to handle event
        }

        /// <summary>
        /// The password recovery 1_ verifying user.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PasswordRecovery1_VerifyingUser([NotNull] object sender, [NotNull] LoginCancelEventArgs e)
        {
            MembershipUser user = null;

            if (this.PasswordRecovery1.UserName.Contains("@") && this.Get<MembershipProvider>().RequiresUniqueEmail)
            {
                // Email Login
                var username = this.Get<MembershipProvider>().GetUserNameByEmail(this.PasswordRecovery1.UserName);
                if (username != null)
                {
                    user = this.Get<MembershipProvider>().GetUser(username, false);

                    // update the username
                    this.PasswordRecovery1.UserName = username;
                }
            }
            else
            {
                // Standard user name login
                if (this.Get<YafBoardSettings>().EnableDisplayName)
                {
                    // Display name login
                    var id = this.Get<IUserDisplayName>().GetId(this.PasswordRecovery1.UserName);

                    if (id.HasValue)
                    {
                        // get the username associated with this id...
                        var username = UserMembershipHelper.GetUserNameFromID(id.Value);

                        // update the username
                        this.PasswordRecovery1.UserName = username;
                    }

                    user = this.Get<MembershipProvider>().GetUser(this.PasswordRecovery1.UserName, false);
                }
            }

            if (user == null)
            {
                return;
            }

            // verify the user is approved, etc...
            if (user.IsApproved)
            {
                return;
            }

            if (this.Get<YafBoardSettings>().EmailVerification)
            {
                // get the hash from the db associated with this user...
                DataTable dt = LegacyDb.checkemail_list(user.Email);

                if (dt.Rows.Count > 0)
                {
                    string hash = dt.Rows[0]["hash"].ToString();

                    // re-send verification email instead of lost password...
                    var verifyEmail = new YafTemplateEmail("VERIFYEMAIL");

                    string subject = this.GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", this.Get<YafBoardSettings>().Name);

                    verifyEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(
                        ForumPages.approve, true, "k={0}", hash);
                    verifyEmail.TemplateParams["{key}"] = hash;
                    verifyEmail.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;
                    verifyEmail.TemplateParams["{forumlink}"] = "{0}".FormatWith(YafForumInfo.ForumURL);

                    verifyEmail.SendEmail(new MailAddress(user.Email, user.UserName), subject, true);

                    this.PageContext.LoadMessage.AddSession(
                        this.GetTextFormatted("ACCOUNT_NOT_APPROVED_VERIFICATION", user.Email));
                }
            }
            else
            {
                // explain they are not approved yet...
                this.PageContext.LoadMessage.AddSession(this.GetText("ACCOUNT_NOT_APPROVED"));
            }

            // just in case cancel the verification...
            e.Cancel = true;

            // nothing they can do here... redirect to login...
            YafBuildLink.Redirect(ForumPages.login);
        }

        /// <summary>
        /// The submit button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void SubmitButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.login);
        }

        #endregion
    }
}