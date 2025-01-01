﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Pages.Admin;

using YAF.Types.Models.Identity;
using YAF.Types.Models;

/// <summary>
/// The Admin edit user page.
/// </summary>
public partial class EditUser : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditUser"/> class. 
    /// </summary>
    public EditUser()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    /// <summary>
    /// Gets or sets the current edit user.
    /// </summary>
    /// <value>The user.</value>
    public Tuple<User, AspNetUsers, Rank, VAccess> EditBoardUser
    {
        get => this.ViewState["EditBoardUser"].ToType<Tuple<User, AspNetUsers, Rank, VAccess>>();

        set => this.ViewState["EditBoardUser"] = value;
    }

    /// <summary>
    /// Registers the java scripts
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    override protected void OnPreRender(EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlock(
            "EditUserTabsJs",
            JavaScriptBlocks.BootstrapTabsLoadJs(this.EditUserTabs.ClientID, this.hidLastTab.ClientID));

        base.OnPreRender(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        var currentUserId = this.Get<LinkBuilder>()
            .StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

        this.EditBoardUser = this.Get<IAspNetUsersHelper>().GetBoardUser(currentUserId, includeNonApproved: true);

        if (this.EditBoardUser == null)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);

            return;
        }

        this.ProfileSettings.User = this.EditBoardUser.Item1;
        this.QuickEditControl.User = this.EditBoardUser;
        this.ProfileEditControl.User = this.EditBoardUser;
        this.GroupEditControl.User = this.EditBoardUser;
        this.UserPointsControl.User = this.EditBoardUser.Item1;
        this.KillEdit1.User = this.EditBoardUser;
        this.ResestPassword.User = this.EditBoardUser;

        // do admin permission check...
        if (!this.PageBoardContext.PageUser.UserFlags.IsHostAdmin && this.EditBoardUser.Item1.UserFlags.IsHostAdmin)
        {
            // user is not host admin and is attempted to edit host admin account...
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.IsPostBack)
        {
            return;
        }

        var userName = this.HtmlEncode(this.EditBoardUser.Item1.DisplayOrUserName());

        var header = string.Format(this.GetText("ADMIN_EDITUSER", "TITLE"), userName);

        this.IconHeader.Text = header;

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(header, string.Empty);

        this.EditUserTabs.DataBind();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_USERS", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Users));
    }
}