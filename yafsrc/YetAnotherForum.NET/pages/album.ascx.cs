/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// the album page.
    /// </summary>
    public partial class Album : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the Album class.
        /// </summary>
        public Album()
            : base("ALBUM")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and YAF JS...
            YafContext.Current.PageElements.RegisterJsBlock("toggleMessageJs", JavaScriptBlocks.ToggleMessageJs);

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<YafBoardSettings>().EnableAlbum)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u") == null
                || this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a") == null)
            {
                YafBuildLink.AccessDenied();
            }

            var userId = Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));
            var albumId = Security.StringToLongOrRedirect(
                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"));

            var displayName = this.Get<IUserDisplayName>().GetName((int)userId);

            // Generate the page links.
            this.PageLinks.Clear();
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(displayName, YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", userId, displayName));
            this.PageLinks.AddLink(this.GetText("ALBUMS"), YafBuildLink.GetLink(ForumPages.albums, "u={0}", userId));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            // Set the title text.
            this.LocalizedLabel1.Param0 = this.Server.HtmlEncode(displayName);
            this.LocalizedLabel1.Param1 = this.Server.HtmlEncode(LegacyDb.album_gettitle(albumId));

            this.Back.Text = this.GetText("BACK_ALBUMS");

            // Initialize the Album Image List control.
            this.AlbumImageList1.UserID = (int)userId;
            this.AlbumImageList1.AlbumID = (int)albumId;
        }

        /// <summary>
        /// Go Back to Albums Page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected void Back_Click(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(
                ForumPages.albums, "u={0}", this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));
        }

        #endregion
    }
}