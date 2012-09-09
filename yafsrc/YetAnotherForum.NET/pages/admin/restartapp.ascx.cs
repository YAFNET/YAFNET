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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.IO;
    using System.Web;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Admin Restart App Page.
    /// </summary>
    public partial class restartapp : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(
                    this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
                this.PageLinks.AddLink(this.GetText("ADMIN_RESTARTAPP", "TITLE"), string.Empty);

                this.Page.Header.Title = "{0} - {1}".FormatWith(
                    this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_RESTARTAPP", "TITLE"));

                this.RestartApp.Text = this.GetText("ADMIN_RESTARTAPP", "TITLE");
            }

            this.DataBind();
        }

        /// <summary>
        /// Try to Restart the Application
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void RestartApp_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (General.GetCurrentTrustLevel() >= AspNetHostingPermissionLevel.High)
            {
                HttpRuntime.UnloadAppDomain();
            }
            else
            {
                try
                {
                    File.SetLastWriteTime(this.Server.MapPath("~/web.config"), DateTime.Now);
                }
                catch (Exception)
                {
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_RESTARTAPP", "MSG_TRUST"), MessageTypes.Error);
                }
            }
        }

        #endregion
    }
}