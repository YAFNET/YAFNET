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

namespace YAF.Controls;

using YAF.Types.Models;
using YAF.Web.Controls;

/// <summary>
/// The AlbumImageList control.
/// </summary>
public partial class AlbumImageList : BaseUserControl
{
    /// <summary>
    /// Gets or sets the user album.
    /// </summary>
    public UserAlbum UserAlbum
    {
        get => this.ViewState["UserAlbum"].ToType<UserAlbum>();

        set => this.ViewState["UserAlbum"] = value;
    }

    /// <summary>
    ///   Gets or sets the User ID.
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    ///   Gets or sets the _cover image id.
    /// </summary>
    private string _coverImageID { get; set; }

    /// <summary>
    /// The ItemCommand method for the cover buttons. Sets/Removes cover image.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
    protected void AlbumImages_ItemCommand(object sender, CommandEventArgs e)
    {
        if (this.UserAlbum.CoverImageID.ToString() == e.CommandArgument.ToString())
        {
            this.GetRepository<UserAlbum>().UpdateCover(this.UserAlbum.ID, null);
        }
        else
        {
            this.GetRepository<UserAlbum>().UpdateCover(this.UserAlbum.ID, e.CommandArgument.ToType<int>());
        }

        this.BindData();
    }

    /// <summary>
    /// Initialize the scripts for changing images' caption.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
    protected void AlbumImages_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (this.UserID != this.PageBoardContext.PageUserID)
        {
            return;
        }

        var setCover = e.Item.FindControlAs<ThemeButton>("SetCover");

        if (setCover == null)
        {
            return;
        }

        // Is this the cover image?
        if (setCover.CommandArgument == this._coverImageID)
        {
            setCover.Text = this.GetText("BUTTON_RESETCOVER");
            setCover.Icon = "trash";
        }
        else
        {
            setCover.Text = this.GetText("BUTTON_SETCOVER");
            setCover.Icon = "tag";
        }
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    override protected void OnPreRender(EventArgs e)
    {
        if (this.UserID == this.PageBoardContext.PageUserID)
        {
            // Register Js Blocks.
            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                nameof(JavaScriptBlocks.                AlbumImageEditCaptionJs),
                JavaScriptBlocks.                AlbumImageEditCaptionJs);
        }

        base.OnPreRender(e);
    }

    /// <summary>
    /// Called when the page loads
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Re-Binds the Data after Page Change
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Pager_PageChange(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PagerTop.PageSize = this.PageBoardContext.BoardSettings.AlbumImagesPerPage;

        // set the Data table
        var albumImageList = this.GetRepository<UserAlbumImage>().ListPaged(
            this.UserAlbum.ID,
            this.PagerTop.CurrentPageIndex,
            this.PagerTop.PageSize);

        // Does this album has a cover?
        this._coverImageID = this.UserAlbum.CoverImageID == null
                                 ? string.Empty
                                 : this.UserAlbum.CoverImageID.ToString();

        if (albumImageList.NullOrEmpty())
        {
            return;
        }

        this.PagerTop.Count = this.GetRepository<UserAlbumImage>().List(this.UserAlbum.ID).Count;

        this.AlbumImages.DataSource = albumImageList;
        this.DataBind();
    }
}