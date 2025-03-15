/* Yet Another Forum.NET
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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages;

using System.Collections.Generic;

using Core.Extensions;
using Core.Model;

using Types.Extensions;
using Types.Models;

/// <summary>
/// The Team Page
/// </summary>
public class AlbumModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "AlbumModel" /> class.
    /// </summary>
    public AlbumModel()
        : base("ALBUM", ForumPages.Album)
    {
    }

    /// <summary>
    /// Gets or sets the images.
    /// </summary>
    [BindProperty]
    public List<UserAlbumImage> Images { get; set; }

    /// <summary>
    /// Gets or sets the album.
    /// </summary>
    [BindProperty]
    public UserAlbum Album { get; set; }

    /// <summary>
    /// Gets or sets the album user.
    /// </summary>
    [BindProperty]
    public User AlbumUser { get; set; }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="u">
    /// The u.
    /// </param>
    /// <param name="a">
    /// The a.
    /// </param>
    public IActionResult OnGet(int u, int a)
    {
        return !this.PageBoardContext.BoardSettings.EnableAlbum ? this.Get<ILinkBuilder>().AccessDenied() : this.BindData(u, a);
    }

    /// <summary>
    /// Removes the cover image.
    /// </summary>
    /// <param name="u">
    /// The u.
    /// </param>
    /// <param name="a">
    /// The a.
    /// </param>
    public void OnPostRemoveCover(int u, int a)
    {
        this.GetRepository<UserAlbum>().UpdateCover(a, null);

        this.BindData(u, a);
    }

    /// <summary>
    /// Sets the cover image.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="u">
    /// The u.
    /// </param>
    /// <param name="a">
    /// The a.
    /// </param>
    public void OnPostSetCover(int id, int u, int a)
    {
        this.GetRepository<UserAlbum>().UpdateCover(a, id);

        this.BindData(u, a);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="albumId">
    /// The album Id.
    /// </param>
    private IActionResult BindData(int userId, int albumId)
    {
        this.AlbumUser = this.GetRepository<User>().GetById(userId);

        if (this.AlbumUser is null)
        {
            // No such user exists
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (!this.AlbumUser.UserFlags.IsApproved)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // set the Data table
        var album = this.GetRepository<UserAlbum>().GetById(albumId);

        // Generate the page links.
        this.PageBoardContext.PageLinks.AddUser(this.AlbumUser.ID, this.AlbumUser.DisplayOrUserName());
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ALBUMS"), this.Get<ILinkBuilder>().GetLink(ForumPages.Albums, new { u = this.AlbumUser.ID }));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        if (album != null)
        {
            this.Album = album;
        }
        else
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // set the Data table
        var albumImageList = this.GetRepository<UserAlbumImage>().ListPaged(
            albumId,
            this.PageBoardContext.PageIndex,
            this.PageBoardContext.BoardSettings.AlbumImagesPerPage);

        if (albumImageList.NullOrEmpty())
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.Images = albumImageList;

        return this.Page();
    }
}