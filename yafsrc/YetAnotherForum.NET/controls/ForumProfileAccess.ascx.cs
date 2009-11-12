/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
  using System;
  using System.Data;
  using System.Text;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The forum profile access.
  /// </summary>
  public partial class ForumProfileAccess : BaseUserControl
  {
    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (PageContext.IsAdmin || PageContext.IsForumModerator)
      {
        var userID = (int) Security.StringToLongOrRedirect(Request.QueryString["u"]);

        using (DataTable dt2 = DB.user_accessmasks(PageContext.PageBoardID, userID))
        {
          var html = new StringBuilder();
          int nLastForumID = 0;
          foreach (DataRow row in dt2.Rows)
          {
            if (nLastForumID != Convert.ToInt32(row["ForumID"]))
            {
              if (nLastForumID != 0)
              {
                html.AppendFormat("</td></tr>");
              }

              html.AppendFormat("<tr><td width='50%' class='postheader'>{0}</td><td width='50%' class='post'>", row["ForumName"]);
              nLastForumID = Convert.ToInt32(row["ForumID"]);
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
    }
  }
}