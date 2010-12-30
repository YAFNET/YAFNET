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

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Net.Mail;
  using System.Web.Security;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
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
      if (this.Page.IsValid)
      {
        string newEmail = this.Email.Text.Trim();
        string newUsername = this.UserName.Text.Trim();

        if (!ValidationHelper.IsValidEmail(newEmail))
        {
          this.PageContext.AddLoadMessage("You have entered an illegal e-mail address.");
          return;
        }

        if (UserMembershipHelper.UserExists(this.UserName.Text.Trim(), newEmail))
        {
          this.PageContext.AddLoadMessage("Username or email are already registered.");
          return;
        }

        string hashinput = DateTime.UtcNow + newEmail + Security.CreatePassword(20);
        string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

        MembershipCreateStatus status;
        MembershipUser user = this.PageContext.CurrentMembership.CreateUser(
          newUsername, 
          this.Password.Text.Trim(), 
          newEmail, 
          this.Question.Text.Trim(), 
          this.Answer.Text.Trim(), 
          !this.PageContext.BoardSettings.EmailVerification, 
          null, 
          out status);

        if (status != MembershipCreateStatus.Success)
        {
          // error of some kind
          this.PageContext.AddLoadMessage("Membership Error Creating User: " + status);
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
        DB.user_save(
          UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey), 
          this.PageContext.PageBoardID, 
          null, 
          null, 
          null, 
          Convert.ToInt32(this.TimeZones.SelectedValue), 
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

        if (this.PageContext.BoardSettings.EmailVerification)
        {
          // send template email
          var verifyEmail = new YafTemplateEmail("VERIFYEMAIL");

          verifyEmail.TemplateParams["{link}"] = YafBuildLink.GetLink(ForumPages.approve, true, "k={0}", hash);
          verifyEmail.TemplateParams["{key}"] = hash;
          verifyEmail.TemplateParams["{forumname}"] = this.PageContext.BoardSettings.Name;
          verifyEmail.TemplateParams["{forumlink}"] = "{0}".FormatWith(this.ForumURL);

          string subject =
            this.PageContext.Localization.GetText("COMMON", "EMAILVERIFICATION_SUBJECT").FormatWith(
              this.PageContext.BoardSettings.Name);

          verifyEmail.SendEmail(new MailAddress(newEmail, newUsername), subject, true);
        }

        // success
        this.PageContext.AddLoadMessage("User {0} Created Successfully.".FormatWith(this.UserName.Text.Trim()));
        YafBuildLink.Redirect(ForumPages.admin_reguser);
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);
        this.PageLinks.AddLink("Users", string.Empty);

        this.TimeZones.DataSource = StaticDataHelper.TimeZones();
        this.DataBind();
        this.TimeZones.Items.FindByValue("0").Selected = true;
      }
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