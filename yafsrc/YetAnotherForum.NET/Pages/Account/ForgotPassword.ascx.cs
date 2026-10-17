/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Account;

using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The recover Password Page.
/// </summary>
public partial class ForgotPassword : AccountPage
{
    /// <summary>
    /// How many reset emails an account gets in the time window.
    /// </summary>
    private const int MaxEmailsPerWindow = 3;

    /// <summary>
    /// The time window of the email limit.
    /// </summary>
    private static readonly TimeSpan EmailWindow = TimeSpan.FromHours(1);

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ForgotPassword" /> class.
    /// </summary>
    public ForgotPassword()
        : base("RECOVER_PASSWORD", ForumPages.Account_ForgotPassword)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FormValidatorJs),
            JavaScriptBlocks.FormValidatorJs(this.Forgot.ClientID));

        this.DataBind();

        this.UserName.Focus();
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"));
    }

    /// <summary>
    /// The forgot password click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForgotPasswordClick(object sender, EventArgs e)
    {
        var user = this.UserName.Text.Contains("@")
                       ? this.Get<IAspNetUsersHelper>().GetUserByEmail(this.UserName.Text)
                       : this.Get<IAspNetUsersHelper>().GetUserByName(this.UserName.Text);

        if (user == null)
        {
            this.PageBoardContext.Notify(this.GetText("USERNAME_FAILURE"), MessageTypes.danger);
            return;
        }

        // verify the user is approved, etc...
        if (!user.IsApproved)
        {
            // the verification e-mail is sent again, only while the verification is pending (accounts waiting
            // for the approval of an admin get no e-mail) and within the send limit
            this.ResendVerificationEmail(user);

            // explain they are not approved yet...
            this.PageBoardContext.LoadMessage.AddSession(
                this.GetTextFormatted("ACCOUNT_NOT_APPROVED_VERIFICATION", this.HtmlEncode(MaskEmail(user.Email))),
                MessageTypes.warning);

            return;
        }

        // over the limit nothing is sent (otherwise the inbox of the user could be flooded), the page shows the same
        if (!SendLimitHelper.TryCount($"AccountEmail_{user.Id}", MaxEmailsPerWindow, EmailWindow))
        {
            this.PageBoardContext.LoadMessage.AddSession(this.GetText("SUCCESS"), MessageTypes.success);

            this.Get<LinkBuilder>().Redirect(ForumPages.Board);
            return;
        }

        var code = HttpUtility.UrlEncode(this.Get<IAspNetUsersHelper>().GeneratePasswordResetToken(user.Id));

        try
        {
            this.Get<ISendNotification>().SendPasswordReset(user, code);
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Error(x, $"Password reset mail to {user.Email} failed");

            this.PageBoardContext.Notify(this.GetTextFormatted("failed", x.Message), MessageTypes.danger);

            return;
        }

        this.PageBoardContext.LoadMessage.AddSession(this.GetText("SUCCESS"), MessageTypes.success);

        this.Get<LinkBuilder>().Redirect(ForumPages.Board);
    }

    /// <summary>
    /// Sends the verification e-mail again when the e-mail verification of the account is pending.
    /// </summary>
    /// <param name="user">The user.</param>
    private void ResendVerificationEmail(AspNetUsers user)
    {
        var yafUser = this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(user.Id);

        if (yafUser == null || yafUser.UserFlags.IsDeleted)
        {
            return;
        }

        var checkMail = this.GetRepository<CheckEmail>().GetSingle(m => m.UserID == yafUser.ID);

        if (checkMail == null || !SendLimitHelper.TryCount($"AccountEmail_{user.Id}", MaxEmailsPerWindow, EmailWindow))
        {
            return;
        }

        // only a hash of the key is stored: the mail gets a new key
        this.GetRepository<CheckEmail>().DeleteById(checkMail.ID);

        try
        {
            this.Get<ISendNotification>().SendVerificationEmail(user, checkMail.Email, checkMail.UserID);
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Error(x, $"Verification mail to {checkMail.Email} failed");
        }
    }

    /// <summary>
    /// Masks the e-mail address: anybody can enter a user name on this page, the address must not be shown
    /// (e.g. "j***@example.com").
    /// </summary>
    /// <param name="email">The e-mail address.</param>
    /// <returns>The masked address.</returns>
    private static string MaskEmail(string email)
    {
        if (email.IsNotSet())
        {
            return string.Empty;
        }

        var at = email.IndexOf('@');

        return at <= 0 ? "***" : $"{email[0]}***{email.Substring(at)}";
    }
}
