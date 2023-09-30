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

namespace YAF.Pages;

/// <summary>
/// The Main Board Page.
/// </summary>
public partial class Board : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "Board" /> class.
    /// </summary>
    public Board()
        : base("DEFAULT", ForumPages.Board)
    {
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Since these controls have EnabledViewState=false, set their visibility on every page load so that this value is not lost on post-back.
        // This is important for another reason: these are board settings; values in the view state should have no impact on whether these controls are shown or not.
        this.ForumStats.Visible = this.PageBoardContext.BoardSettings.ShowForumStatistics;
        this.ForumStatistics.Visible = this.PageBoardContext.BoardSettings.ShowForumStatistics;
        this.ActiveDiscussions.Visible = this.PageBoardContext.BoardSettings.ShowActiveDiscussions;

        this.RenderGuestControls();

        if (this.IsPostBack)
        {
            return;
        }

        if (this.PageBoardContext.PageCategoryID == 0)
        {
            return;
        }

        this.Welcome.Visible = false;
    }

    /// <summary>
    /// The create page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();

        if (this.PageBoardContext.PageCategoryID == 0)
        {
            return;
        }

        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);
    }

    /// <summary>
    /// Render The GuestBar
    /// </summary>
    private void RenderGuestControls()
    {
        if (!this.PageBoardContext.IsGuest)
        {
            return;
        }

        if (!this.PageBoardContext.BoardSettings.ShowConnectMessageInTopic)
        {
            return;
        }

        this.GuestUserMessage.Visible = true;

        this.GuestMessage.Text = this.GetText("TOOLBAR", "WELCOME_GUEST_FULL");

        var endPoint = new Label { Text = "." };

        var isLoginAllowed = false;
        var isRegisterAllowed = false;

        if (Config.IsAnyPortal)
        {
            this.GuestMessage.Text = this.GetText("TOOLBAR", "WELCOME_GUEST");
        }
        else
        {
            if (Config.AllowLoginAndLogoff)
            {
                var navigateUrl = "javascript:void(0);";

                if (this.PageBoardContext.CurrentForumPage.IsAccountPage)
                {
                    navigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Account_Login);
                }

                // show login
                var loginLink = new HyperLink
                                    {
                                        Text = this.GetText("TOOLBAR", "LOGIN"),
                                        ToolTip = this.GetText("TOOLBAR", "LOGIN"),
                                        NavigateUrl = navigateUrl,
                                        CssClass = "alert-link LoginLink"
                                    };

                this.GuestUserMessage.Controls.Add(loginLink);

                isLoginAllowed = true;
            }

            if (!this.PageBoardContext.BoardSettings.DisableRegistrations)
            {
                if (isLoginAllowed)
                {
                    this.GuestUserMessage.Controls.Add(
                        new Label { Text = $"&nbsp;{this.GetText("COMMON", "OR")}&nbsp;" });
                }

                // show register link
                var registerLink = new HyperLink
                                       {
                                           Text = this.GetText("TOOLBAR", "REGISTER"),
                                           NavigateUrl = this.PageBoardContext.BoardSettings.ShowRulesForRegistration
                                                             ? this.Get<LinkBuilder>().GetLink(ForumPages.RulesAndPrivacy)
                                                             : this.Get<LinkBuilder>().GetLink(ForumPages.Account_Register),
                                           CssClass = "alert-link"
                                       };

                this.GuestUserMessage.Controls.Add(registerLink);

                this.GuestUserMessage.Controls.Add(endPoint);

                isRegisterAllowed = true;
            }
            else
            {
                this.GuestUserMessage.Controls.Add(endPoint);

                this.GuestUserMessage.Controls.Add(
                    new Label { Text = this.GetText("TOOLBAR", "DISABLED_REGISTER") });
            }

            // If both disallowed
            if (isLoginAllowed || isRegisterAllowed)
            {
                return;
            }

            this.GuestUserMessage.Controls.Clear();
            this.GuestUserMessage.Controls.Add(new Label { Text = this.GetText("TOOLBAR", "WELCOME_GUEST_NO") });
        }
    }
}