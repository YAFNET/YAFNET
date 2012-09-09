/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
            this.Load += this.AdminPage_Load;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets PageName.
        /// </summary>
        public override string PageName
        {
            get
            {
                return "admin_{0}".FormatWith(base.PageName);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The admin page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AdminPage_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
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