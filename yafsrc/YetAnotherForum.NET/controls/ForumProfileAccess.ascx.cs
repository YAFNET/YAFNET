/* Yet Another Forum.NET
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Text;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The forum profile access.
    /// </summary>
    public partial class ForumProfileAccess : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.IsAdmin && !this.PageContext.IsForumModerator)
            {
                return;
            }

            var userID = (int)Security.StringToLongOrRedirect(this.Request.QueryString.GetFirstOrDefault("u"));

            using (DataTable dt2 = LegacyDb.user_accessmasks(this.PageContext.PageBoardID, userID))
            {
                var html = new StringBuilder();
                int nLastForumID = 0;
                foreach (DataRow row in dt2.Rows)
                {
                    if (nLastForumID != row["ForumID"].ToType<int>())
                    {
                        if (nLastForumID != 0)
                        {
                            html.AppendFormat("</td></tr>");
                        }

                        html.AppendFormat(
                            "<tr><td width='50%' class='postheader'>{0}</td><td width='50%' class='post'>",
                            this.HtmlEncode(row["ForumName"]));
                        nLastForumID = row["ForumID"].ToType<int>();
                    }
                    else
                    {
                        html.AppendFormat(", ");
                    }

                    html.AppendFormat("{0}", row["AccessMaskName"]);
                }

                if (nLastForumID != 0)
                {
                    html.AppendFormat("</td></tr>");
                }

                this.AccessMaskRow.Text = html.ToString();
            }
        }

        #endregion
    }
}