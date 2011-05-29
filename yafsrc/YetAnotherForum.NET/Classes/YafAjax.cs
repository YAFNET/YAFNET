/* YetAnotherForum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.Classes
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Security;
    using System.Web.Services;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Class for JS jQuery  Ajax Methods
    /// </summary>
    [WebService(Namespace = "http://yetanotherforum.net/yafajax")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class YafAjax : WebService, IHaveServiceLocator
    {
        #region Properties

        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator
        {
            get
            {
                return YafContext.Current.ServiceLocator;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// SSO Login From Facebook
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="first_name">
        /// The first_name.
        /// </param>
        /// <param name="last_name">
        /// The last_name.
        /// </param>
        /// <param name="link">
        /// The link.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="birthday">
        /// The birthday.
        /// </param>
        /// <param name="hometown">
        /// The hometown.
        /// </param>
        /// <param name="gender">
        /// The gender.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="timezone">
        /// The timezone.
        /// </param>
        /// <param name="lokale">
        /// The lokale.
        /// </param>
        /// <returns>
        /// Returns the Login Status
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string LoginFacebookUser(
            string id,
            string name,
            string first_name,
            string last_name,
            string link,
            string username,
            string birthday,
            string hometown,
            string gender,
            string email,
            string timezone,
            string lokale)
        {
            if (!YafContext.Current.Get<YafBoardSettings>().AllowSingleSignOn)
            {
                return this.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED");
            }

            // Check if user exists
            var userName = YafContext.Current.Get<MembershipProvider>().GetUserNameByEmail(email);

            // Login user if exists
            if (!string.IsNullOrEmpty(userName))
            {
                var yafUser = YafUserProfile.GetProfile(userName);

                var yafUserData =
                    new CombinedUserDataHelper(YafContext.Current.Get<MembershipProvider>().GetUser(userName, true));

                if (!yafUserData.UseSingleSignOn)
                {
                    return this.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED_BYUSER");
                }

                if (yafUser.Facebook.Equals(id))
                {
                    FormsAuthentication.SetAuthCookie(userName, true);

                    YafContext.Current.Get<IRaiseEvent>().Raise(
                        new SuccessfulUserLoginEvent(YafContext.Current.PageUserID));

                    return "OK";
                }

                return this.Get<ILocalization>().GetText("LOGIN", "SSO_ID_NOTMATCH");
            }

            // Create User if not exists?!
            if (YafContext.Current.Get<YafBoardSettings>().RegisterNewFacebookUser)
            {
                MembershipCreateStatus status;

                var pass = Membership.GeneratePassword(32, 16);
                var securityAnswer = Membership.GeneratePassword(64, 30);

                MembershipUser user = this.Get<MembershipProvider>().CreateUser(
                    username,
                    pass,
                    email,
                    "Answer is a generated Pass",
                    securityAnswer,
                    true,
                    null,
                    out status);

                // setup inital roles (if any) for this user
                RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, username);

                // create the user in the YAF DB as well as sync roles...
                int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

                // create empty profile just so they have one
                YafUserProfile userProfile = YafUserProfile.GetProfile(username);

                userProfile.Facebook = id;
                userProfile.Homepage = link;
                userProfile.Birthday = DateTime.Parse(birthday);
                userProfile.RealName = name;

                userProfile.Save();

                // setup their inital profile information
                userProfile.Save();

                if (userID == null)
                {
                    // something is seriously wrong here -- redirect to failure...
                    return this.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                }

                if (this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
                {
                    // send user register notification to the following admin users...
                    this.SendRegistrationNotificationEmail(user);
                }

                // send user register notification to the following admin users...
                this.SendRegistrationNotificationToUser(user, pass, securityAnswer);

                // save the time zone...
                int userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

                LegacyDb.user_save(
                    userId,
                    YafContext.Current.PageBoardID,
                    username,
                    null,
                    email,
                    timezone,
                    null,
                    null,
                    null,
                    true,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);

                bool autoWatchTopicsEnabled = this.Get<YafBoardSettings>().DefaultNotificationSetting ==
                                              UserNotificationSetting.TopicsIPostToOrSubscribeTo;

                // save the settings...
                LegacyDb.user_savenotification(
                    userId,
                    true,
                    autoWatchTopicsEnabled,
                    this.Get<YafBoardSettings>().DefaultNotificationSetting,
                    this.Get<YafBoardSettings>().DefaultSendDigestEmail);

                // save avatar
                LegacyDb.user_saveavatar(
                    userId,
                    "https://graph.facebook.com/1457847725/picture",
                    null,
                    null);

                // Clearing cache with old Active User Lazy Data ...
                this.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userId));

                this.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));

                FormsAuthentication.SetAuthCookie(user.UserName, true);

                YafContext.Current.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(YafContext.Current.PageUserID));

                return "OK";
            }

            return this.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
        }

        private void SendRegistrationNotificationToUser([NotNull] MembershipUser user, [NotNull] string pass, [NotNull] string securityAnswer)
        {
            var notifyUser = new YafTemplateEmail();

            string subject =
              this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_NEW_FACEBOOK_USER_SUBJECT").FormatWith(
                this.Get<YafBoardSettings>().Name);

            notifyUser.TemplateParams["{user}"] = user.UserName;
            notifyUser.TemplateParams["{email}"] = user.Email;
            notifyUser.TemplateParams["{pass}"] = pass;
            notifyUser.TemplateParams["{answer}"] = securityAnswer;
            notifyUser.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;

            string emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_FACEBOOK_REGISTER");

            this.Get<ISendMail>().Queue(this.Get<YafBoardSettings>().ForumEmail, user.Email, subject, emailBody);
        }

        /// <summary>
        /// The send registration notification email.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        private void SendRegistrationNotificationEmail([NotNull] MembershipUser user)
        {
            string[] emails = this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.Split(';');

            var notifyAdmin = new YafTemplateEmail();

            string subject =
              this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_USER_REGISTER_EMAIL_SUBJECT").FormatWith(
                this.Get<YafBoardSettings>().Name);

            notifyAdmin.TemplateParams["{adminlink}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.admin_admin, true);
            notifyAdmin.TemplateParams["{user}"] = user.UserName;
            notifyAdmin.TemplateParams["{email}"] = user.Email;
            notifyAdmin.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;

            string emailBody = notifyAdmin.ProcessTemplate("NOTIFICATION_ON_USER_REGISTER");

            foreach (string email in emails.Where(email => email.Trim().IsSet()))
            {
                this.Get<ISendMail>().Queue(this.Get<YafBoardSettings>().ForumEmail, email.Trim(), subject, emailBody);
            }
        }

        /// <summary>
        /// The change album title.
        /// </summary>
        /// <param name="albumID">
        /// The album id.
        /// </param>
        /// <param name="newTitle">
        /// The New title.
        /// </param>
        /// <returns>
        /// the return object.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public YafAlbum.ReturnClass ChangeAlbumTitle(int albumID, [NotNull] string newTitle)
        {
            return YafAlbum.ChangeAlbumTitle(albumID, newTitle);
        }

        /// <summary>
        /// The change image caption.
        /// </summary>
        /// <param name="imageID">
        /// The Image id.
        /// </param>
        /// <param name="newCaption">
        /// The New caption.
        /// </param>
        /// <returns>
        /// the return object.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public YafAlbum.ReturnClass ChangeImageCaption(int imageID, [NotNull] string newCaption)
        {
            return YafAlbum.ChangeImageCaption(imageID, newCaption);
        }

        /// <summary>
        /// The refresh shout box.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The refresh shout box.
        /// </returns>
        [WebMethod]
        public int RefreshShoutBox(int boardId)
        {
            var messages = this.Get<IDataCache>().GetOrSet(
                "{0}_basic".FormatWith(Constants.Cache.Shoutbox),
                () => LegacyDb.shoutbox_getmessages(boardId, 1, false).AsEnumerable(),
                TimeSpan.FromMilliseconds(1000));

            var message = messages.FirstOrDefault();

            return message != null ? message.Field<int>("ShoutBoxMessageID") : 0;
        }

       #region Favorite Topic Function

        /// <summary>
        /// The add favorite topic.
        /// </summary>
        /// <param name="topicId">
        /// The topic ID.
        /// </param>
        /// <returns>
        /// The add favorite topic.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int AddFavoriteTopic(int topicId)
        {
            return this.Get<IFavoriteTopic>().AddFavoriteTopic(topicId);
        }

        /// <summary>
        /// The remove favorite topic.
        /// </summary>
        /// <param name="topicId">
        /// The favorite topic id.
        /// </param>
        /// <returns>
        /// The remove favorite topic.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int RemoveFavoriteTopic(int topicId)
        {
            return this.Get<IFavoriteTopic>().RemoveFavoriteTopic(topicId);
        }
       #endregion

       #region Thanks Functions

        /// <summary>
        /// Add Thanks to post
        /// </summary>
        /// <param name="msgID">
        /// The msg id.
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        [CanBeNull, WebMethod(EnableSession = true)]
        public ThankYouInfo AddThanks([NotNull] object msgID)
        {
            var messageID = msgID.ToType<int>();

            string username =
              LegacyDb.message_AddThanks(
                UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

            // if the user is empty, return a null object...
            return username.IsNotSet()
                     ? null
                     : YafThankYou.CreateThankYou(username, "BUTTON_THANKSDELETE", "BUTTON_THANKSDELETE_TT", messageID);
        }

        /// <summary>
        /// This method is called asynchronously when the user clicks on "Remove Thank" button.
        /// </summary>
        /// <param name="msgID">
        /// Message Id
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        [NotNull, WebMethod(EnableSession = true)]
        public ThankYouInfo RemoveThanks([NotNull] object msgID)
        {
            var messageID = msgID.ToType<int>();

            string username =
              LegacyDb.message_RemoveThanks(
                UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

            return YafThankYou.CreateThankYou(username, "BUTTON_THANKS", "BUTTON_THANKS_TT", messageID);
        }

        #endregion

        #endregion
    }
}