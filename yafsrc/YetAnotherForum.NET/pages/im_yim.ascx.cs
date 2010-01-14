/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages
{
  // YAF.Pages
  using System;
  using System.Web.Security;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for active.
  /// </summary>
  public partial class im_yim : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="im_yim"/> class.
    /// </summary>
    public im_yim()
      : base("IM_YIM")
    {
    }

    /// <summary>
    /// Gets UserID.
    /// </summary>
    public int UserID
    {
      get
      {
        return (int) Security.StringToLongOrRedirect(Request.QueryString["u"]);
      }
    }

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
      if (User == null)
      {
        YafBuildLink.AccessDenied();
      }

      if (!IsPostBack)
      {
        // get user data...
        MembershipUser user = UserMembershipHelper.GetMembershipUserById(UserID);

        if (user == null)
        {
          YafBuildLink.AccessDenied( /*No such user exists*/);
        }

        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(user.UserName, YafBuildLink.GetLink(ForumPages.profile, "u={0}", UserID));
        this.PageLinks.AddLink(GetText("TITLE"), string.Empty);

        // get full user data...
        var userData = new CombinedUserDataHelper(user, UserID);

        this.Img.Src = string.Format("http://opi.yahoo.com/online?u={0}&m=g&t=2", userData.Profile.YIM);
        this.Msg.NavigateUrl = string.Format("http://edit.yahoo.com/config/send_webmesg?.target={0}&.src=pg", userData.Profile.YIM);
      }
    }
  }
}