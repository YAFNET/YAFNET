/* Yet Another Forum.NET
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
  /// The im_skype.
  /// </summary>
  public partial class im_skype : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="im_skype"/> class.
    /// </summary>
    public im_skype()
      : base("IM_SKYPE")
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

        this.Msg.NavigateUrl = string.Format("skype:{0}?call", userData.Profile.Skype);
        this.Msg.Attributes.Add("onclick", "return skypeCheck();");
        this.Img.Src = string.Format("http://mystatus.skype.com/bigclassic/{0}", userData.Profile.Skype);
      }
    }
  }
}