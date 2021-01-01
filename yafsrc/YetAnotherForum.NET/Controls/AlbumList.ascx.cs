/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The AlbumList control.
    /// </summary>
    public partial class AlbumList : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the User ID.
        /// </summary>
        public User User { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// redirects to the add new album page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AddAlbum_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.EditAlbumImages, "a=new");
        }

        /// <summary>
        /// The item command method for albums repeater. Redirects to the album page.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void Albums_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.EditAlbumImages, "a={0}", e.CommandArgument);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            if (this.User.ID == this.PageContext.PageUserID)
            {
                // Register Js Blocks.
                this.PageContext.PageElements.RegisterJsBlockStartup(
                    "AlbumEventsJs",
                    JavaScriptBlocks.AlbumEventsJs(
                        this.Get<ILocalization>().GetText("ALBUM_CHANGE_TITLE").ToJsString(),
                        this.Get<ILocalization>().GetText("ALBUM_IMAGE_CHANGE_CAPTION").ToJsString()));
                this.PageContext.PageElements.RegisterJsBlockStartup(
                    "ChangeAlbumTitleJs", JavaScriptBlocks.ChangeAlbumTitleJs);
                this.PageContext.PageElements.RegisterJsBlockStartup(
                    "AlbumCallbackSuccessJS", JavaScriptBlocks.AlbumCallbackSuccessJs);
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
            if (this.IsPostBack)
            {
                return;
            }

            this.Header.Param0 = this.HtmlEncode(this.User.DisplayOrUserName());

            this.BindData();

            var userAlbum = (int)this.GetRepository<User>().MaxAlbumData(
                this.PageContext.PageUserID,
                this.PageContext.PageBoardID).UserAlbum;

            HttpContext.Current.Session["localizationFile"] = this.Get<ILocalization>().LanguageFileName;

            // Show Albums Max Info
            if (this.User.ID == this.PageContext.PageUserID)
            {
                this.albumsInfo.Text = this.Get<ILocalization>().GetTextFormatted(
                    "ALBUMS_INFO", this.PageContext.NumAlbums, userAlbum);
                if (userAlbum > this.PageContext.NumAlbums)
                {
                    this.AddAlbum.Visible = true;
                }

                this.albumsInfo.Text = userAlbum > 0
                                           ? this.Get<ILocalization>().GetTextFormatted(
                                               "ALBUMS_INFO", this.PageContext.NumAlbums, userAlbum)
                                           : this.Get<ILocalization>().GetText("ALBUMS_NOTALLOWED");

                this.albumsInfo.Visible = true;
            }

            if (!this.AddAlbum.Visible)
            {
                return;
            }

            this.AddAlbum.TextLocalizedPage = "BUTTON";
            this.AddAlbum.TextLocalizedTag = "BUTTON_ADDALBUM";
        }

        /// <summary>
        /// The pager_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// Binds the album list data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.PageContext.BoardSettings.AlbumsPerPage;

            // set the Data table
            var albumListDT = this.GetRepository<UserAlbum>().ListByUser(this.User.ID);

            if (albumListDT == null || !albumListDT.Any())
            {
                return;
            }

            this.PagerTop.Count = albumListDT.Count;

            // create paged data source for the album list
            var pds = new PagedDataSource
                {
                    DataSource = albumListDT,
                    AllowPaging = true,
                    CurrentPageIndex = this.PagerTop.CurrentPageIndex,
                    PageSize = this.PagerTop.PageSize
                };

            this.Albums.DataSource = pds;
            this.DataBind();
        }

        #endregion
    }
}