/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// The AlbumImageList control.
    /// </summary>
    public partial class AlbumImageList : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the Album ID.
        /// </summary>
        public int AlbumID { get; set; }

        /// <summary>
        ///   Gets or sets the User ID.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        ///   Gets or sets the _cover image id.
        /// </summary>
        private string _coverImageID { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The ItemCommand method for the cover buttons. Sets/Removes cover image.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void AlbumImages_ItemCommand([NotNull] object sender, [NotNull] CommandEventArgs e)
        {
            var dt = this.GetRepository<UserAlbum>().List(this.AlbumID).FirstOrDefault();
            
                if (dt.CoverImageID.ToString() == e.CommandArgument.ToString())
                {
                    this.GetRepository<UserAlbum>().UpdateCover(this.AlbumID, null);
                }
                else
                {
                    this.GetRepository<UserAlbum>().UpdateCover(this.AlbumID, e.CommandArgument.ToType<int>());
                }
  
            this.BindData();
        }

        /// <summary>
        /// Initialize the scripts for changing images' caption.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void AlbumImages_ItemDataBound([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            if (this.UserID != this.PageContext.PageUserID)
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
                setCover.TextLocalizedTag = "BUTTON_RESETCOVER";
                setCover.Type = ButtonStyle.Danger;
                setCover.Icon = "trash";
            }
            else
            {
                setCover.TextLocalizedTag = "BUTTON_SETCOVER";
                setCover.Type = ButtonStyle.Success;
                setCover.Icon = "tag";
            }
        }

        /// <summary>
        /// Redirect to the edit album page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void EditAlbums_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.EditAlbumImages, "a={0}", this.AlbumID);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            if (this.UserID == this.PageContext.PageUserID)
            {
                // Register Js Blocks.
                BoardContext.Current.PageElements.RegisterJsBlockStartup(
                    "AlbumEventsJs",
                    JavaScriptBlocks.AlbumEventsJs(
                        this.GetText("ALBUM_CHANGE_TITLE").ToJsString(), this.GetText("ALBUM_IMAGE_CHANGE_CAPTION").ToJsString()));
                BoardContext.Current.PageElements.RegisterJsBlockStartup(
                    "ChangeAlbumTitleJs", JavaScriptBlocks.ChangeAlbumTitleJs);
                BoardContext.Current.PageElements.RegisterJsBlockStartup(
                    "ChangeImageCaptionJs", JavaScriptBlocks.ChangeImageCaptionJs);
                BoardContext.Current.PageElements.RegisterJsBlockStartup(
                    "AlbumCallbackSuccessJS", JavaScriptBlocks.AlbumCallbackSuccessJs);
                this.ltrTitleOnly.Visible = false;
            }

            base.OnPreRender(e);
        }

        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.UserID == this.PageContext.PageUserID)
            {
                this.ltrTitleOnly.Visible = false;

                // Initialize the edit control.
                this.EditAlbums.Visible = true;
            }

            this.BindData();
        }

        /// <summary>
        /// Re-Binds the Data after Page Change
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.Get<BoardSettings>().AlbumImagesPerPage;
            var albumTitle = this.GetRepository<UserAlbum>().GetTitle(this.AlbumID);

            // if (UserID == PageContext.PageUserID)
            // ltrTitle.Visible = false;
            this.ltrTitleOnly.Text = this.HtmlEncode(albumTitle);
            this.ltrTitle.Text = albumTitle == string.Empty
                                     ? this.GetText("ALBUM_CHANGE_TITLE")
                                     : this.HtmlEncode(albumTitle);

            // set the Data table
            var albumImageList = this.GetRepository<UserAlbumImage>().List(this.AlbumID);
            var album = this.GetRepository<UserAlbum>().List(this.AlbumID).FirstOrDefault();

            // Does this album has a cover?
            this._coverImageID = album.CoverImageID == null
                                     ? string.Empty
                                     : album.CoverImageID.ToString();

            if (albumImageList == null || !albumImageList.Any())
            {
                return;
            }

            this.PagerTop.Count = albumImageList.Count;
            
            // Create paged data source for the album image list
            var pds = new PagedDataSource
                          {
                              DataSource = albumImageList,
                              AllowPaging = true,
                              CurrentPageIndex = this.PagerTop.CurrentPageIndex,
                              PageSize = this.PagerTop.PageSize
                          };

            this.AlbumImages.DataSource = pds;
            this.DataBind();
        }

        #endregion
    }
}