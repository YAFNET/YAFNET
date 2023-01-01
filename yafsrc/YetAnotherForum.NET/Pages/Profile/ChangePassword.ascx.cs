/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Pages.Profile;

/// <summary>
/// The Change Password Page.
/// </summary>
public partial class ChangePassword : ProfilePage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChangePassword"/> class.
    /// </summary>
    public ChangePassword()
        : base("CHANGE_PASSWORD", ForumPages.Profile_ChangePassword)
    {
    }

    /// <summary>
    /// The cancel push button_ click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void CancelPushButtonClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.MyAccount);
    }

    /// <summary>
    /// The change password click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ChangePasswordClick(object sender, EventArgs e)
    {
        if (!this.Page.IsValid)
        {
            return;
        }

        var result = this.Get<IAspNetUsersHelper>().ChangePassword(
            this.PageBoardContext.MembershipUser.Id,
            this.CurrentPassword.Text,
            this.NewPassword.Text);

        if (result.Succeeded)
        {
            this.PageBoardContext.Notify(this.GetText("CHANGE_SUCCESS"), MessageTypes.success);
        }
        else
        {
            this.PageBoardContext.Notify(result.Errors.FirstOrDefault(), MessageTypes.danger);
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (Config.IsDotNetNuke)
        {
            // Not accessible...
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.IsPostBack)
        {
            this.ContentBody.CssClass = "card-body was-validated";
            return;
        }

        this.NewPasswordCompare.ToolTip = this.NewPasswordCompare.ErrorMessage = this.GetText("NO_PASSWORD_MATCH");
        this.NewOldPasswordCompare.ToolTip = this.NewOldPasswordCompare.ErrorMessage = this.GetText("PASSWORD_NOT_NEW");

        this.DataBind();
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"));
    }
}