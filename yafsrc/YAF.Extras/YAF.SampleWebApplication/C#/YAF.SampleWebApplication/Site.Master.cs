/* Yet Another Forum.NET
 * Copyright (C) Jaben Cargman
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

namespace YAF.SampleWebApplication
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// Class SiteMaster
    /// </summary>
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        /// <summary>
        /// The get return url.
        /// </summary>
        /// <returns>
        /// The url.
        /// </returns>
        protected string GetReturnUrl()
        {
            return HttpContext.Current.Server.UrlEncode(
                        HttpContext.Current.Request.QueryString.GetFirstOrDefault("ReturnUrl").IsSet()
                            ? General.GetSafeRawUrl(
                                HttpContext.Current.Request.QueryString.GetFirstOrDefault("ReturnUrl"))
                            : General.GetSafeRawUrl());
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var loginLink = this.HeadLoginView.FindControlAs<HyperLink>("LoginLink");

            if (loginLink != null)
            {
                loginLink.NavigateUrl = "~/forum/forum.aspx?g=login&ReturnUrl={0}".FormatWith(this.GetReturnUrl());
            }
        }
    }
}
