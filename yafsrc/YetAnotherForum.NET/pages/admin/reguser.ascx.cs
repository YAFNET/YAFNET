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

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Net.Mail;
  using System.Web.Security;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Model;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Types.Models;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for reguser.
  /// </summary>
  public partial class reguser : AdminPage
  {
    #region Methods

    /// <summary>
    /// The forum register_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForumRegister_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.Page.IsValid)
        {
            return;
        }

        string newEmail = this.Email.Text.Trim();
        string newUsername = this.UserName.Text.Trim();

        if (!ValidationHelper.IsValidEmail(newEmail))
        {
            this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_INVALID_MAIL"));
            return;
        }

        if (UserMembershipHelper.UserExists(this.UserName.Text.Trim(), newEmail))
        {
            this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_NAME_EXISTS"));
            return;
        }

        MembershipCreateStatus status;
        MembershipUser user = this.Get<MembershipProvider>().CreateUser(
            newUsername, 
            this.Password.Text.Trim(), 
            newEmail, 
            this.Question.Text.Trim(), 
            this.Answer.Text.Trim(),
            !this.Get<YafBoardSettings>().EmailVerification, 
            null, 
            out status);

        if (status != MembershipCreateStatus.Success)
        {
            // error of some kind
            this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_ERROR_CREATE").FormatWith(status));
            return;
        }

        // setup inital roles (if any) for this user
        RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, newUsername);

        // create the user in the YAF DB as well as sync roles...
        int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

        // create profile
        YafUserProfile userProfile = YafUserProfile.GetProfile(newUsername);

        // setup their inital profile information
        userProfile.Location = this.Location.Text.Trim();
        userProfile.Homepage = this.HomePage.Text.Trim();
        userProfile.Save();

        // save the time zone...
        LegacyDb.user_save(
            UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey), 
            this.PageContext.PageBoardID, 
            null, 
            null, 
            null, 
            this.TimeZones.SelectedValue.ToType<int>(), 
            null, 
            null, 
            null,
            null, 
            null, 
            null,
            null, 
            null, 
            null, 
            null, 
            null, 
            null);

        if (this.Get<YafBoardSettings>().EmailVerification)
        {
            this.SendVerificationEmail(user, newEmail, userID, newUsername);
        }

        bool autoWatchTopicsEnabled =
            this.Get<YafBoardSettings>().DefaultNotificationSetting.Equals(
                UserNotificationSetting.TopicsIPostToOrSubscribeTo);

        LegacyDb.user_savenotification(
            UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey),
            true,
            autoWatchTopicsEnabled,
            this.Get<YafBoardSettings>().DefaultNotificationSetting,
            this.Get<YafBoardSettings>().DefaultSendDigestEmail);


        // success
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_CREATED").FormatWith(this.UserName.Text.Trim()));
        YafBuildLink.Redirect(ForumPages.admin_reguser);
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

        this.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_users));

        // current page label (no link)
        this.PageLinks.AddLink(this.GetText("ADMIN_REGUSER", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
           this.GetText("ADMIN_ADMIN", "Administration"),
           this.GetText("ADMIN_USERS", "TITLE"),
           this.GetText("ADMIN_REGUSER", "TITLE"));

        this.ForumRegister.Text = this.GetText("ADMIN_REGUSER", "REGISTER");
        this.cancel.Text = this.GetText("COMMON", "CANCEL");

        this.TimeZones.DataSource = StaticDataHelper.TimeZones();
        this.DataBind();
        this.TimeZones.Items.FindByValue("0").Selected = true;
    }

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_users);
    }

    #endregion
  }
}