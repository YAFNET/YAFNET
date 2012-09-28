/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Net.Mail;
  using System.Web;
  using System.Web.Security;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Types.Interfaces.Extensions;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for active.
  /// </summary>
  public partial class im_email : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "im_email" /> class.
    /// </summary>
    public im_email()
      : base("IM_EMAIL")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets UserID.
    /// </summary>
    public int UserID
    {
      get
      {
        return (int)Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString["u"]);
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
      if (this.User == null || !this.Get<YafBoardSettings>().AllowEmailSending)
      {
        YafBuildLink.AccessDenied();
      }

        if (this.IsPostBack)
        {
            return;
        }

        // get user data...
        MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.UserID);

        if (user == null)
        {
            YafBuildLink.AccessDenied(/*No such user exists*/);
        }

        string displayName = UserMembershipHelper.GetDisplayNameFromID(this.UserID);

        this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(
            this.PageContext.BoardSettings.EnableDisplayName ? displayName : user.UserName, 
            YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.UserID));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        this.Send.Text = this.GetText("SEND");
    }

    /// <summary>
    /// The send_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Send_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      try
      {
        // get "to" user...
        MembershipUser toUser = UserMembershipHelper.GetMembershipUserById(this.UserID);

        // send it...
        this.Get<ISendMail>().Send(
          new MailAddress(this.PageContext.User.Email, this.PageContext.User.UserName), 
          new MailAddress(toUser.Email.Trim(), toUser.UserName.Trim()), 
          this.Subject.Text.Trim(), 
          this.Body.Text.Trim());

        // redirect to profile page...
        YafBuildLink.Redirect(ForumPages.profile, false, "u={0}", this.UserID);
      }
      catch (Exception x)
      {
          LegacyDb.eventlog_create(this.PageContext.PageUserID, this, x);

          this.PageContext.AddLoadMessage(this.PageContext.IsAdmin ? x.Message : this.GetText("ERROR"));
      }
    }

    #endregion
  }
}