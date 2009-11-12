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
namespace YAF.Pages
{
  // YAF.Pages
  using System;
  using System.Data;
  using System.Net.Mail;
  using System.Web.Security;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The recoverpassword.
  /// </summary>
  public partial class recoverpassword : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="recoverpassword"/> class.
    /// </summary>
    public recoverpassword()
      : base("RECOVER_PASSWORD")
    {
    }

    /// <summary>
    /// Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        this.PasswordRecovery1.MembershipProvider = Config.MembershipProvider;

        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(GetText("TITLE"));

        // handle localization
        var usernameRequired = (RequiredFieldValidator) this.PasswordRecovery1.UserNameTemplateContainer.FindControl("UserNameRequired");
        var answerRequired = (RequiredFieldValidator) this.PasswordRecovery1.QuestionTemplateContainer.FindControl("AnswerRequired");

        usernameRequired.ToolTip = usernameRequired.ErrorMessage = GetText("REGISTER", "NEED_USERNAME");
        answerRequired.ToolTip = answerRequired.ErrorMessage = GetText("REGISTER", "NEED_ANSWER");

        ((Button) this.PasswordRecovery1.UserNameTemplateContainer.FindControl("SubmitButton")).Text = GetText("SUBMIT");
        ((Button) this.PasswordRecovery1.QuestionTemplateContainer.FindControl("SubmitButton")).Text = GetText("SUBMIT");
        ((Button) this.PasswordRecovery1.SuccessTemplateContainer.FindControl("SubmitButton")).Text = GetText("BACK");

        this.PasswordRecovery1.UserNameFailureText = GetText("USERNAME_FAILURE");
        this.PasswordRecovery1.GeneralFailureText = GetText("GENERAL_FAILURE");
        this.PasswordRecovery1.QuestionFailureText = GetText("QUESTION_FAILURE");

        DataBind();
      }
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
    protected void SubmitButton_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.login);
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
    protected void PasswordRecovery1_SendingMail(object sender, MailMessageEventArgs e)
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

      string subject = GetTextFormatted("PASSWORDRETRIEVAL_EMAIL_SUBJECT", PageContext.BoardSettings.Name);

      passwordRetrieval.TemplateParams["{username}"] = userName;
      passwordRetrieval.TemplateParams["{password}"] = password;
      passwordRetrieval.TemplateParams["{forumname}"] = PageContext.BoardSettings.Name;
      passwordRetrieval.TemplateParams["{forumlink}"] = String.Format("{0}", YafForumInfo.ForumURL);

      passwordRetrieval.SendEmail(e.Message.To[0], subject, true);

      // manually set to success...
      e.Cancel = true;
      this.PasswordRecovery1.TabIndex = 3;
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
    protected void PasswordRecovery1_SendMailError(object sender, SendMailErrorEventArgs e)
    {
      // it will fail to send the message...
      e.Handled = true;
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
    protected void PasswordRecovery1_VerifyingUser(object sender, LoginCancelEventArgs e)
    {
      MembershipUser user = PageContext.CurrentMembership.GetUser(this.PasswordRecovery1.UserName, false);

      if (user != null)
      {
        // verify the user is approved, etc...
        if (!user.IsApproved)
        {
          if (PageContext.BoardSettings.EmailVerification)
          {
            // get the hash from the db associated with this user...
            DataTable dt = DB.checkemail_list(user.Email);

            if (dt.Rows.Count > 0)
            {
              string hash = dt.Rows[0]["hash"].ToString();

              // re-send verification email instead of lost password...
              var verifyEmail = new YafTemplateEmail("VERIFYEMAIL");

              string subject = GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", PageContext.BoardSettings.Name);

              verifyEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.approve, true, "k={0}", hash);
              verifyEmail.TemplateParams["{key}"] = hash;
              verifyEmail.TemplateParams["{forumname}"] = PageContext.BoardSettings.Name;
              verifyEmail.TemplateParams["{forumlink}"] = String.Format("{0}", YafForumInfo.ForumURL);

              verifyEmail.SendEmail(new MailAddress(user.Email, user.UserName), subject, true);

              PageContext.LoadMessage.AddSession(PageContext.Localization.GetTextFormatted("ACCOUNT_NOT_APPROVED_VERIFICATION", user.Email));
            }
          }
          else
          {
            // explain they are not approved yet...
            PageContext.LoadMessage.AddSession(PageContext.Localization.GetText("ACCOUNT_NOT_APPROVED"));
          }

          // just in case cancel the verification...
          e.Cancel = true;

          // nothing they can do here... redirect to login...
          YafBuildLink.Redirect(ForumPages.login);
        }
      }
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
    protected void PasswordRecovery1_VerifyingAnswer(object sender, LoginCancelEventArgs e)
    {
      // needed to handle event
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
    protected void PasswordRecovery1_AnswerLookupError(object sender, EventArgs e)
    {
      PageContext.LoadMessage.AddSession(GetText("QUESTION_FAILURE"));
    }
  }
}