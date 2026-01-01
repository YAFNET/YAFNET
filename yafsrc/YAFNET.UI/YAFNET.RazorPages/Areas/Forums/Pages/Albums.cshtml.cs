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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System.Threading.Tasks;

namespace YAF.Pages;

using System.Collections.Generic;

using Core.Extensions;
using Core.Model;

using Types.Extensions;
using Types.Models;

/// <summary>
/// The Team Page
/// </summary>
public class AlbumsModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "AlbumsModel" /> class.
    /// </summary>
    public AlbumsModel()
        : base("ALBUMS", ForumPages.Albums)
    {
    }

    /// <summary>
    /// Gets or sets the albums.
    /// </summary>
    [BindProperty]
    public List<UserAlbum> Albums { get; set; }

    /// <summary>
    /// Gets or sets the album user.
    /// </summary>
    [BindProperty]
    public User AlbumUser { get; set; }

    /// <summary>
    /// Gets or sets the albums info.
    /// </summary>
    [BindProperty]
    public string AlbumsInfo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether show add album button.
    /// </summary>
    [BindProperty]
    public bool ShowAddAlbumButton { get; set; }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="u">
    /// The u.
    /// </param>
    public async Task<IActionResult> OnGetAsync(int u)
    {
        if (!this.PageBoardContext.BoardSettings.EnableAlbum)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.AlbumUser = await this.GetRepository<User>().GetByIdAsync(u);

        if (this.AlbumUser is null)
        {
            // No such user exists
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (!this.AlbumUser.UserFlags.IsApproved)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        var title = this.GetTextFormatted("ALBUMS_HEADER_TEXT", this.AlbumUser.DisplayOrUserName(), string.Empty);

        this.PageBoardContext.PageLinks.AddLink(title);
        this.PageTitle = title;

        return await this.BindDataAsync(u);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    private async Task<IActionResult> BindDataAsync(int userId)
    {
        // set the Data table
        var albums = this.GetRepository<UserAlbum>().ListByUserPaged(
            userId,
            this.PageBoardContext.PageIndex,
            this.PageBoardContext.BoardSettings.AlbumsPerPage);

        if (!albums.NullOrEmpty())
        {
            this.Albums = albums;
        }
        else
        {
            if (this.AlbumUser.ID != this.PageBoardContext.PageUserID)
            {
                return this.Get<ILinkBuilder>().AccessDenied();
            }

            this.Albums = [];
        }

        var userAlbum = (int)(await this.GetRepository<User>().MaxAlbumDataAsync(
            this.PageBoardContext.PageUserID,
            this.PageBoardContext.PageBoardID)).UserAlbum;

        // Show Albums Max Info
        if (this.AlbumUser.ID != this.PageBoardContext.PageUserID)
        {
            return this.Page();
        }

        this.AlbumsInfo = this.Get<ILocalization>().GetTextFormatted(
            "ALBUMS_INFO",
            this.PageBoardContext.NumAlbums,
            userAlbum);

        if (userAlbum > this.PageBoardContext.NumAlbums)
        {
            this.ShowAddAlbumButton = true;
        }

        this.AlbumsInfo = userAlbum > 0
                              ? this.Get<ILocalization>().GetTextFormatted(
                                  "ALBUMS_INFO",
                                  this.PageBoardContext.NumAlbums,
                                  userAlbum)
                              : this.Get<ILocalization>().GetText("ALBUMS_NOTALLOWED");

        return this.Page();
    }
}