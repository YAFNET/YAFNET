/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Web.Security;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for active.
  /// </summary>
  public partial class im_aim : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="im_aim"/> class.
    /// </summary>
    public im_aim()
      : base("IM_AIM")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets UserID.
    /// </summary>
    public int UserID
    {
      get
      {
        return (int)Security.StringToLongOrRedirect(this.Request.QueryString["u"]);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.User == null)
      {
        YafBuildLink.AccessDenied();
      }

      if (!this.IsPostBack)
      {
        // get user data...
        MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.UserID);

        if (user == null)
        {
          YafBuildLink.AccessDenied( /*No such user exists*/);
        }

        string displayName = UserMembershipHelper.GetDisplayNameFromID(this.UserID);

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(
          this.PageContext.BoardSettings.EnableDisplayName
             ? displayName : user.UserName, 
          YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.UserID));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        // get full user data...
        var userData = new CombinedUserDataHelper(user, this.UserID);

        this.Msg.NavigateUrl = "aim:goim?screenname={0}&message=Hi.+Are+you+there?".FormatWith(userData.Profile.AIM);
        this.Buddy.NavigateUrl = "aim:addbuddy?screenname={0}".FormatWith(userData.Profile.AIM);
      }
    }

    #endregion
  }
}