/* Yet Another Forum.NET
 * Copyright (C) 2006-2007 Jaben Cargman
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
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;

namespace YAF.Controls.Statistics
{
    [ToolboxData("<{0}:MostActiveUsers runat=\"server\"></{0}:MostActiveUsers>")]
    public class MostActiveUsers : BaseControl
    {
        private int _displayNumber = 10;
        /// <summary>
        /// The default constructor for MostActiveUsers.
        /// </summary>
        public MostActiveUsers()
        {

        }

        public int DisplayNumber
        {
            get
            {
                return _displayNumber;
            }
            set
            {
                _displayNumber = value;
            }
        }

        /// <summary>
        /// Renders the MostActiveUsers class.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            string act_rank = "";

            act_rank += "<table width=\"90%\" class=\"content\" cellspacing=\"1\" border=\"0\" cellpadding=\"0\">";
            act_rank += "<tr><td class=\"header1\">Most Active Users</td></tr>";
            System.Data.DataTable rank = YAF.Classes.Data.DB.user_activity_rank(DisplayNumber);
            int i = 1;

            act_rank += "<tr><td class=post><table cellspacing=0 cellpadding=0 align=center>";

            foreach (System.Data.DataRow r in rank.Rows)
            {
                // string img = "<img src='/yetanotherforum.net/themes/standard/user_rank1.gif'/>";
                i++;
                act_rank += "<tr class=\"post\">";

                // Immagine
                // act_rank += string.Format("<td align=\"center\">{0}</td>", img);

                // Nome autore
                act_rank += string.Format("<td width=\"75%\">&nbsp;<a href='{1}'>{0}</a></td>", r["Name"], YafBuildLink.GetLink(ForumPages.profile, "u={0}", r["ID"]));

                // Numero post
                act_rank += string.Format("<td align=\"center\">{0}</td></tr>", r["NumOfPosts"]);

                act_rank += "</tr>";
            }

            act_rank += "</table></td></tr>";

            act_rank += "</table>";
            writer.Write(act_rank);
        }
    }
}
