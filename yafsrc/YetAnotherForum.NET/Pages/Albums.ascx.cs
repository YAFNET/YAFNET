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

namespace YAF.Pages;

using YAF.Types.Models;

/// <summary>
/// the Albums Page.
/// </summary>
public partial class Albums : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the Albums class.
    /// </summary>
    public Albums()
        : base("ALBUMS", ForumPages.Albums)
    {
    }

    /// <summary>
    ///   Gets user ID of edited user.
    /// </summary>
    protected int CurrentUserID =>
        this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.PageBoardContext.BoardSettings.EnableAlbum)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (!this.Get<HttpRequestBase>().QueryString.Exists("u"))
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        var user = this.GetRepository<User>().GetById(this.CurrentUserID);

        if (user == null)
        {
            // No such user exists
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (!user.UserFlags.IsApproved)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddUser(this.CurrentUserID, user.DisplayOrUserName());
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ALBUMS"), string.Empty);

        // Initialize the Album List control.
        this.AlbumList1.User = user;
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
    }
}