/* YetAnotherForum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

namespace YAF
{
    using System;
    using System.Web.UI;

    using YAF.Core.Services;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Utils;

    /// <summary>
    /// The Twitter Authentification Page
    /// </summary>
    public partial class Auth : Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.GetFirstOrDefault("denied") != null)
            {
                Response.Clear();
                Response.Write(
                    "<script type='text/javascript'>window.opener.location.href = '{0}';window.close();</script>".FormatWith(
                    YafBuildLink.GetLink(ForumPages.login).Replace("auth.aspx", "default.aspx")));

                return;
            }

            // Twitter Return
            if (Request["twitterauth"] != null && Request["twitterauth"] == "false")
            {
                string message = string.Empty;

                YafSingleSignOnUser.LoginTwitterUser(this.Request, ref message);

                Response.Clear();

                if (!string.IsNullOrEmpty(message))
                {
                    this.Response.Write(
                        "<script type='text/javascript'>alert('{0}');window.opener.location.href = '{1}';window.close();</script>"
                            .FormatWith(
                                message, YafBuildLink.GetLink(ForumPages.login).Replace("auth.aspx", "default.aspx")));
                }
                else
                {
                    Response.Write(
                                "<script type='text/javascript'>window.opener.location.href = '{0}';window.close();</script>"
                                    .FormatWith(
                                        YafBuildLink.GetLink(ForumPages.forum).Replace("auth.aspx", "default.aspx")));
                }
            }
            else
            {
                Response.Write(
                   "<script type='text/javascript'>window.opener.location.href = '{0}';window.close();</script>".FormatWith(
                   YafBuildLink.GetLink(ForumPages.login).Replace("auth.aspx", "default.aspx")));
            }
        }
    }
}