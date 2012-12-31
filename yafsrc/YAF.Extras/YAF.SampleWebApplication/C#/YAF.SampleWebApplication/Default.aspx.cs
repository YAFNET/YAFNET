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
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    /// <summary>
    /// The Default Page
    /// </summary>
    public partial class _Default : Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Type csType = typeof(Page);

                if (!YafContext.Current.Get<YafBoardSettings>().ShowRelativeTime)
                {
                    return;
                }

                var uRLToResource = Config.JQueryFile;

                if (!uRLToResource.StartsWith("http"))
                {
                    uRLToResource = YafForumInfo.GetURLToResource(Config.JQueryFile);
                }

                ScriptManager.RegisterClientScriptInclude(this, csType, "JQuery", uRLToResource);

                ScriptManager.RegisterClientScriptInclude(
                    this, csType, "jqueryTimeagoscript", YafForumInfo.GetURLToResource("js/jquery.timeago.js"));

                ScriptManager.RegisterStartupScript(this, csType, "timeagoloadjs", JavaScriptBlocks.TimeagoLoadJs, true);
            }
            catch (Exception)
            {
                this.Response.Redirect("~/forum/install/default.aspx");
            }
        }
    }
}
