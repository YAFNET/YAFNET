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

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The User Account Verification Page.
/// </summary>
public partial class Approve : AccountPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "Approve" /> class.
    /// </summary>
    public Approve()
        : base("APPROVE", ForumPages.Account_Approve)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Handles the Click event of the ValidateKey control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="EventArgs"/> instance containing the event data.
    /// </param>
    public void ValidateKey_Click(object sender, EventArgs e)
    {
        var userEmail = this.GetRepository<CheckEmail>().Update(HttpUtility.UrlEncode(this.key.Text));

        if (userEmail == null)
        {
            this.Approved.Visible = false;
            this.ErrorAlert.Visible = true;
            this.ErrorMessage.Text = this.GetText("email_verify_failed");

            return;
        }

        // the confirmation of a new e-mail address (Edit Settings): the key belongs to an approved user with
        // another address
        var forumUser = this.GetRepository<User>().GetById(userEmail.UserID);
        var accountUser = forumUser?.ProviderUserKey == null
                              ? null
                              : this.Get<IAspNetUsersHelper>().GetUser(forumUser.ProviderUserKey);

        if (accountUser is { IsApproved: true }
            && !userEmail.Email.Equals(accountUser.Email, StringComparison.OrdinalIgnoreCase))
        {
            this.ChangeEmail(accountUser, userEmail);
            return;
        }

        var user = this.Get<IAspNetUsersHelper>().GetUserByEmail(userEmail.Email);

        if (user == null)
        {
            this.Approved.Visible = false;
            this.ErrorAlert.Visible = true;
            this.ErrorMessage.Text = this.GetText("email_verify_failed");

            return;
        }

        var result = this.Get<IAspNetUsersHelper>().ConfirmEmail(user.Id, this.key.Text);

        if (result.Succeeded)
        {
            this.Approved.Visible = true;
            this.ErrorAlert.Visible = false;

            user.IsApproved = true;
            user.EmailConfirmed = true;

            // tell the provider to update...
            this.Get<IAspNetUsersHelper>().Update(user);

            this.GetRepository<User>().Approve(userEmail.UserID);

            this.GetRepository<CheckEmail>().DeleteById(userEmail.ID);

            this.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userEmail.UserID));

            // automatically log in created users
            this.Get<IAspNetUsersHelper>().SignIn(user);

            // now redirect to main site...
            this.PageBoardContext.LoadMessage.AddSession(this.GetText("EMAIL_VERIFIED"), MessageTypes.info);

            // default redirect -- because if may not want to redirect to login.
            this.Get<LinkBuilder>().Redirect(ForumPages.Board);
        }
        else
        {
            this.Approved.Visible = false;
            this.ErrorAlert.Visible = true;

            this.ErrorMessage.Text = result.Errors.FirstOrDefault();
        }
    }

    /// <summary>
    /// Changes the e-mail address of the user after the confirmation link of the new address was opened.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="checkEmail">The confirmation record with the new address.</param>
    private void ChangeEmail(AspNetUsers user, CheckEmail checkEmail)
    {
        var result = this.Get<IAspNetUsersHelper>().ConfirmEmail(user.Id, this.key.Text);

        // the address could have been taken by another account in the meantime
        var otherUser = this.Get<IAspNetUsersHelper>().GetUserByEmail(checkEmail.Email);

        if (!result.Succeeded || otherUser != null && otherUser.Id != user.Id)
        {
            this.Approved.Visible = false;
            this.ErrorAlert.Visible = true;
            this.ErrorMessage.Text = result.Succeeded
                                         ? this.GetText("EDIT_PROFILE", "DUPLICATED_EMAIL")
                                         : result.Errors.FirstOrDefault();

            return;
        }

        this.Get<IAspNetUsersHelper>().UpdateEmail(user, checkEmail.Email);

        this.GetRepository<CheckEmail>().DeleteById(checkEmail.ID);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(checkEmail.UserID));

        this.PageBoardContext.LoadMessage.AddSession(this.GetText("APPROVE", "EMAIL_CHANGED"), MessageTypes.success);

        this.Get<LinkBuilder>().Redirect(ForumPages.Board);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        if (this.Get<HttpRequestBase>().QueryString.Exists("code"))
        {
            this.key.Text = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("code");
            this.ValidateKey_Click(sender, e);
        }
        else
        {
            this.Approved.Visible = false;
            this.ErrorAlert.Visible = true;
            this.ErrorMessage.Text = this.GetText("email_verify_failed");
        }
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }
}