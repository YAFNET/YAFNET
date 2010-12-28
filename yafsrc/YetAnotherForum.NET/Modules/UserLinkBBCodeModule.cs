/* YetAnotherForum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils.Helpers;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Controls;

  public class UserLinkBBCodeModule : YafBBCodeControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UserLinkBBCodeModule"/> class.
    /// </summary>
    public UserLinkBBCodeModule()
      : base()
    {
    }

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

      var foundUsers = YafContext.Current.Get<IUserDisplayName>().Find(userName.Trim());

      if (foundUsers.Any())
      {
        var stringBuilder = new StringBuilder();
        int userId = foundUsers.First().Key;

        var userLink = new UserLink()
          {
            UserID = userId,
            CssClass = "UserLinkBBCode",
            BlankTarget = true,
            ID = "UserLinkBBCodeFor{0}".FormatWith(userId)
          };

        var showOnlineStatusImage = YafContext.Current.BoardSettings.ShowUserOnlineStatus &&
                                    !UserMembershipHelper.IsGuestUser(userId);

        var onlineStatusImage = new OnlineStatusImage()
          {
            ID = "OnlineStatusImage",
            Style = "vertical-align: bottom",
            UserID = userId
          };

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