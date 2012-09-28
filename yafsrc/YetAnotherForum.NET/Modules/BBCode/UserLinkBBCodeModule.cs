/* YetAnotherForum.NET
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
namespace YAF.Modules
{
    using System.Linq;
    using System.Text;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The BB Code UserLink Module
    /// </summary>
    public class UserLinkBBCodeModule : YafBBCodeControl
  {
        /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      var userName = Parameters["inner"];

      if (userName.IsNotSet() || userName.Length > 50)
      {
        return;
      }

      var userId = this.Get<IUserDisplayName>().GetId(userName.Trim());

      if (userId.HasValue)
      {
        var stringBuilder = new StringBuilder();

          var userLink = new UserLink
              {
                  UserID = (int)userId,
                  CssClass = "UserLinkBBCode",
                  BlankTarget = true,
                  ID = "UserLinkBBCodeFor{0}".FormatWith(userId)
              };

          var showOnlineStatusImage = this.Get<YafBoardSettings>().ShowUserOnlineStatus &&
                                    !UserMembershipHelper.IsGuestUser(userId);

          var onlineStatusImage = new OnlineStatusImage { ID = "OnlineStatusImage", Style = "vertical-align: bottom", UserID = (int)userId };

        stringBuilder.AppendLine("<!-- BEGIN userlink -->");
        stringBuilder.AppendLine(@"<span class=""userLinkContainer"">");
        stringBuilder.AppendLine(userLink.RenderToString());

        if (showOnlineStatusImage)
        {
          stringBuilder.AppendLine(onlineStatusImage.RenderToString()); 
        }

        stringBuilder.AppendLine("</span>");
        stringBuilder.AppendLine("<!-- END userlink -->");

        writer.Write(stringBuilder.ToString());
      }
      else
      {
        writer.Write(this.HtmlEncode(userName));
      }
    }
  }
}