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

namespace YAF.Core
{
    #region Using

    using System;

    using YAF.Classes.Data;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Admin page with extra security. All admin pages need to be derived from this base class.
    /// </summary>
    public class AdminPage : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AdminPage" /> class. 
        ///   Creates the Administration page.
        /// </summary>
        public AdminPage()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminPage"/> class.
        /// </summary>
        /// <param name="transPage">
        /// The trans page.
        /// </param>
        public AdminPage([CanBeNull] string transPage)
            : base(transPage)
        {
            this.IsAdminPage = true;
            this.Load += this.AdminPageLoad;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Page Name.
        /// </summary>
        public override string PageName
        {
            get
            {
                return "admin_{0}".FormatWith(base.PageName);
            }
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the AdminPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AdminPageLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.CreatePageLinks();
            }
            
            // not admins are forbidden
            if (!this.PageContext.IsAdmin)
            {
                YafBuildLink.AccessDenied();
            }

            // host admins are not checked
            if (this.PageContext.IsHostAdmin)
            {
                return;
            }

            // Load the page access list.
            var dt = LegacyDb.adminpageaccess_list(
                this.PageContext.PageUserID, this.PageContext.ForumPageType.ToString().ToLowerInvariant());

            // Check access rights to the page.
            if (!this.PageContext.ForumPageType.ToString().IsSet() || dt == null || dt.Rows.Count <= 0)
            {
                YafBuildLink.RedirectInfoPage(InfoMessage.HostAdminPermissionsAreRequired);
            }
        }

        #endregion
    }
}