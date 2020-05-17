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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Web;

    using YAF.Configuration;
    
    using YAF.Core.BasePages;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// the Albums Page.
    /// </summary>
    public partial class Albums : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the Albums class.
        /// </summary>
        public Albums()
            : base("ALBUM")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<BoardSettings>().EnableAlbum)
            {
                BuildLink.AccessDenied();
            }

            if (!this.Get<HttpRequestBase>().QueryString.Exists("u"))
            {
                BuildLink.AccessDenied();
            }

            var userId =
                Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

            var user = this.Get<IAspNetUsersHelper>().GetMembershipUserById(userId);

            if (user == null)
            {
                // No such user exists
                BuildLink.AccessDenied();
            }

            if (user.IsApproved == false)
            {
                BuildLink.AccessDenied();
            }

            var displayName = this.Get<IAspNetUsersHelper>().GetDisplayNameFromID(userId);

            this.PageLinks.Clear();
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.Get<BoardSettings>().EnableDisplayName
                    ? displayName
                    : this.Get<IAspNetUsersHelper>().GetUserNameFromID(userId),
                BuildLink.GetLink(ForumPages.UserProfile, "u={0}", userId));
            this.PageLinks.AddLink(this.GetText("ALBUMS"), string.Empty);

            // Initialize the Album List control.
            this.AlbumList1.UserID = userId.ToType<int>();
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // 
        }

        #endregion
    }
}