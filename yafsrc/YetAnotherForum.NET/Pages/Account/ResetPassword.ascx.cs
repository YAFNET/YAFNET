/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

/// <summary>
/// The Reset Password Page.
/// </summary>
public partial class ResetPassword : AccountPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "ResetPassword" /> class.
    /// </summary>
    public ResetPassword()
        : base("ACCOUNT_RESEST_PASSWORD", ForumPages.Account_ResetPassword)
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            this.ContentBody.CssClass = "card-body was-validated";
            return;
        }

        if (!this.Get<HttpRequestBase>().QueryString.Exists("code"))
        {
            this.Get<LinkBuilder>().AccessDenied();
            return;
        }

        this.DataBind();
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
    /// Reset Password for the user
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ResetClick(object sender, EventArgs e)
    {
        if (!this.Page.IsValid)
        {
            return;
        }

        var user = this.Get<IAspNetUsersHelper>().GetUserByEmail(this.Email.Text);

        if (user == null)
        {
            this.PageBoardContext.Notify(this.GetText("USERNAME_FAILURE"), MessageTypes.danger);
            return;
        }

        var code = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("code");

        var result = this.Get<IAspNetUsersHelper>().ResetPassword(user.Id, code, this.Password.Text);

        if (result.Succeeded)
        {
            // Get User again to get updated Password Hash
            user = this.Get<IAspNetUsersHelper>().GetUser(user.Id);

            this.Get<IAspNetUsersHelper>().SignIn(user);

            this.Get<LinkBuilder>().Redirect(ForumPages.Board);

            return;
        }

        this.PageBoardContext.Notify(result.Errors.FirstOrDefault(), MessageTypes.danger);
    }
}