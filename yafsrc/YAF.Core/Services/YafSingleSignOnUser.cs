/* Yet Another Forum.NET
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

namespace YAF.Core.Services
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Services.Twitter;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// Single Sign On User Class to handle Twitter and Facebook Logins
    /// </summary>
    public class YafSingleSignOnUser
    {
        /// <summary>
        /// Generates the twitter login URL.
        /// </summary>
        /// <param name="generatePopUpUrl">if set to <c>true</c> [generate pop up URL].</param>
        /// <returns>
        /// Returns the login Url
        /// </returns>
        public static string GenerateTwitterLoginUrl(bool generatePopUpUrl)
        {
            var oAuth = new OAuthTwitter
                    {
                        CallBackUrl = "{0}auth.aspx?twitterauth=false".FormatWith(YafForumInfo.ForumBaseUrl),
                        ConsumerKey = Config.TwitterConsumerKey,
                        ConsumerSecret = Config.TwitterConsumerSecret
                    };

            return generatePopUpUrl
                       ? "javascript:window.open('{0}', 'TwitterLoginWindow', 'width=800,height=700,left=150,top=100,scrollbar=no,resize=no'); return false;"
                             .FormatWith(oAuth.AuthorizationLinkGet())
                       : oAuth.AuthorizationLinkGet();
        }

        /// <summary>
        /// Logins/Registers the twitter user.
        /// </summary>
        /// <param name="request">
        /// The page request.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// Returns if the login was successfully or not.
        /// </returns>
        public static bool LoginTwitterUser(HttpRequest request, ref string message)
        {
            var oAuth = new OAuthTwitter
            {
                ConsumerKey = Config.TwitterConsumerKey,
                ConsumerSecret = Config.TwitterConsumerSecret
            };

            // Get the access token and secret.
            oAuth.AccessTokenGet(request["oauth_token"], request["oauth_verifier"]);

            if (oAuth.TokenSecret.Length > 0)
            {
                var tweetAPI = new TweetAPI(oAuth);

                var twitterUser = tweetAPI.GetUser();

                if (twitterUser.UserId > 0)
                {
                    // Check if user exists
                    var checkUser = YafContext.Current.Get<MembershipProvider>().GetUser(
                        twitterUser.UserName, false);

                    // Login user if exists
                    if (checkUser != null)
                    {
                        // LOGIN Existing User
                        var yafUser = YafUserProfile.GetProfile(checkUser.UserName);

                        var yafUserData = new CombinedUserDataHelper(checkUser);

                        if (!yafUserData.UseSingleSignOn)
                        {
                            message = YafContext.Current.Get<ILocalization>().GetText(
                                "LOGIN", "SSO_DEACTIVATED_BYUSER");

                            return false;
                        }

                        if (yafUser.Twitter.Equals(twitterUser.UserName) && yafUser.TwitterId.Equals(twitterUser.UserId.ToString()))
                        {
                            LoginTwitterSuccess(false, oAuth, yafUserData.UserID, checkUser);

                            return true;
                        }

                        message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTERID_NOTMATCH");

                        return false;
                    }

                    // Create User if not exists?! Doesnt work because there is no Email
                    var email = "{0}@twitter.com".FormatWith(twitterUser.UserName);

                    // Create User if not exists?!
                    if (YafContext.Current.Get<YafBoardSettings>().RegisterNewFacebookUser &&
                        !YafContext.Current.Get<YafBoardSettings>().DisableRegistrations)
                    {
                        MembershipCreateStatus status;

                        var pass = Membership.GeneratePassword(32, 16);
                        var securityAnswer = Membership.GeneratePassword(64, 30);

                        MembershipUser user = YafContext.Current.Get<MembershipProvider>().CreateUser(
                            twitterUser.UserName, pass, email, "Answer is a generated Pass", securityAnswer, true, null, out status);

                        // setup inital roles (if any) for this user
                        RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, twitterUser.UserName);

                        // create the user in the YAF DB as well as sync roles...
                        int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

                        // create empty profile just so they have one
                        YafUserProfile userProfile = YafUserProfile.GetProfile(twitterUser.UserName);

                        userProfile.TwitterId = twitterUser.UserId.ToString();
                        userProfile.Twitter = twitterUser.UserName;
                        userProfile.Homepage = !string.IsNullOrEmpty(twitterUser.Url)
                                                   ? twitterUser.Url
                                                   : "http://twitter.com/{0}".FormatWith(twitterUser.UserName);
                        userProfile.RealName = twitterUser.Name;
                        userProfile.Interests = twitterUser.Description;
                        userProfile.Location = twitterUser.Location;

                        userProfile.Save();

                        // setup their inital profile information
                        userProfile.Save();

                        if (userID == null)
                        {
                            // something is seriously wrong here -- redirect to failure...
                            message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

                            return false;
                        }

                        if (YafContext.Current.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
                        {
                            // send user register notification to the following admin users...
                            SendRegistrationNotificationEmail(user);
                        }

                        // save the time zone...
                        int userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

                        // send user register notification to the following admin users...
                        SendRegistrationMessageToTwitterUser(user, pass, securityAnswer, userId, oAuth);

                        LegacyDb.user_save(
                            userId,
                            YafContext.Current.PageBoardID,
                            twitterUser.UserName,
                            null,
                            email,
                            0,
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

                        bool autoWatchTopicsEnabled = YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting ==
                                                      UserNotificationSetting.TopicsIPostToOrSubscribeTo;

                        // save the settings...
                        LegacyDb.user_savenotification(
                            userId,
                            true,
                            autoWatchTopicsEnabled,
                            YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting,
                            YafContext.Current.Get<YafBoardSettings>().DefaultSendDigestEmail);

                        // save avatar
                        if (!string.IsNullOrEmpty(twitterUser.ProfileImageUrl))
                        {
                            LegacyDb.user_saveavatar(
                                userId,
                                twitterUser.ProfileImageUrl,
                                null,
                                null);
                        }

                        LoginTwitterSuccess(true, oAuth, userId, user);

                        message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "UPDATE_EMAIL");

                        return true;
                    }

                    message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

                    return false;
                }
            }

            message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

            return false;
        }

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
        /// The first name.
        /// </param>
        /// <param name="last_name">
        /// The last name.
        /// </param>
        /// <param name="link">
        /// The link.
        /// </param>
        /// <param name="username">
        /// The user name.
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
        /// <param name="locale">
        /// The locale.
        /// </param>
        /// <param name="remember">
        /// The remember.
        /// </param>
        /// <returns>
        /// Returns the Login Status
        /// </returns>
        public static string LoginFacebookUser(
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
            string locale,
            bool remember)
        {
            if (!YafContext.Current.Get<YafBoardSettings>().AllowSingleSignOn)
            {
                return YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED");
            }

            // Check if username is null
            if (string.IsNullOrEmpty(username))
            {
                username = name;
            }

            var userGender = 0;

            if (!string.IsNullOrEmpty(gender))
            {
                switch (gender)
                {
                    case "male":
                        userGender = 1;
                        break;
                    case "female":
                        userGender = 2;
                        break;
                }
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
                    return YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED_BYUSER");
                }

                if (yafUser.Facebook.Equals(id))
                {
                    // Add Flag to User that indicates that the user is logged in via facebook
                    LegacyDb.user_update_single_sign_on_status(yafUserData.UserID, true, false);

                    FormsAuthentication.SetAuthCookie(userName, remember);

                    YafContext.Current.Get<IRaiseEvent>().Raise(
                        new SuccessfulUserLoginEvent(YafContext.Current.PageUserID));

                    return "OK";
                }

                return YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_ID_NOTMATCH");
            }

            // Create User if not exists?!
            if (YafContext.Current.Get<YafBoardSettings>().RegisterNewFacebookUser &&
                !YafContext.Current.Get<YafBoardSettings>().DisableRegistrations)
            {
                MembershipCreateStatus status;

                var pass = Membership.GeneratePassword(32, 16);
                var securityAnswer = Membership.GeneratePassword(64, 30);

                MembershipUser user = YafContext.Current.Get<MembershipProvider>().CreateUser(
                    username, pass, email, "Answer is a generated Pass", securityAnswer, true, null, out status);

                // setup inital roles (if any) for this user
                RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, username);

                // create the user in the YAF DB as well as sync roles...
                int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

                // create empty profile just so they have one
                YafUserProfile userProfile = YafUserProfile.GetProfile(username);

                userProfile.Facebook = id;
                userProfile.Homepage = link;

                if (!string.IsNullOrEmpty(birthday))
                {
                    DateTime userBirthdate;
                    var ci = CultureInfo.CreateSpecificCulture("en-US");
                    DateTime.TryParse(birthday, ci, DateTimeStyles.None, out userBirthdate);

                    if (userBirthdate > DateTime.MinValue.Date)
                    {
                        userProfile.Birthday = userBirthdate;
                    }
                }

                userProfile.RealName = username;
                userProfile.Gender = userGender;

                if (!string.IsNullOrEmpty(hometown))
                {
                    userProfile.Location = hometown;
                }

                userProfile.Save();

                // setup their inital profile information
                userProfile.Save();

                if (userID == null)
                {
                    // something is seriously wrong here -- redirect to failure...
                    return YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                }

                if (YafContext.Current.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
                {
                    // send user register notification to the following admin users...
                    SendRegistrationNotificationEmail(user);
                }

                // send user register notification to the user...
                YafContext.Current.Get<ISendNotification>().SendRegistrationNotificationToUser(
                    user, pass, securityAnswer, "NOTIFICATION_ON_FACEBOOK_REGISTER");

                // save the time zone...
                int userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

                LegacyDb.user_save(
                    userId,
                    YafContext.Current.PageBoardID,
                    username,
                    username,
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

                bool autoWatchTopicsEnabled = YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting ==
                                              UserNotificationSetting.TopicsIPostToOrSubscribeTo;

                // save the settings...
                LegacyDb.user_savenotification(
                    userId,
                    true,
                    autoWatchTopicsEnabled,
                    YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting,
                    YafContext.Current.Get<YafBoardSettings>().DefaultSendDigestEmail);

                // save avatar
                LegacyDb.user_saveavatar(userId, "https://graph.facebook.com/{0}/picture".FormatWith(id), null, null);

                // Clearing cache with old Active User Lazy Data ...
                YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userId));

                YafContext.Current.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));

                // Add Flag to User that indicates that the user is logged in via facebook
                LegacyDb.user_update_single_sign_on_status(userId, true, false);

                FormsAuthentication.SetAuthCookie(user.UserName, remember);

                YafContext.Current.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(YafContext.Current.PageUserID));

                return "OK";
            }

            return YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
        }

        /// <summary>
        /// Call the Events when the Twitter Login was Successfully
        /// </summary>
        /// <param name="newUser">
        /// The new user.
        /// </param>
        /// <param name="oAuth">
        /// The twitter oauth.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        private static void LoginTwitterSuccess([NotNull]bool newUser, [NotNull]OAuthTwitter oAuth, [NotNull]int userId, [CanBeNull]MembershipUser user)
        {
            // Clearing cache with old Active User Lazy Data ...
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userId));

            if (newUser)
            {
                YafContext.Current.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));
            }

            // Store Tokens in Session (Could Bes Stored in DB but it would be a Security Problem)
            YafContext.Current.Get<IYafSession>().TwitterToken = oAuth.Token;
            YafContext.Current.Get<IYafSession>().TwitterTokenSecret = oAuth.TokenSecret;

            // Add Flag to User that indicates that the user is logged in via twitter
            LegacyDb.user_update_single_sign_on_status(userId, false, true);

            FormsAuthentication.SetAuthCookie(user.UserName, true);

            YafContext.Current.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(userId));
        }

        /// <summary>
        /// Send an Private Message to the Newly Created User with
        /// his Account Info (Pass, Security Question and Answer)
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="pass">
        /// The pass.
        /// </param>
        /// <param name="securityAnswer">
        /// The security answer.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="oAuth">
        /// The o Auth.
        /// </param>
        private static void SendRegistrationMessageToTwitterUser([NotNull] MembershipUser user, [NotNull] string pass, [NotNull] string securityAnswer, [NotNull] int userId, OAuthTwitter oAuth)
        {
            var notifyUser = new YafTemplateEmail();

            string subject =
                YafContext.Current.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_NEW_FACEBOOK_USER_SUBJECT").FormatWith(
                    YafContext.Current.Get<YafBoardSettings>().Name);

            notifyUser.TemplateParams["{user}"] = user.UserName;
            notifyUser.TemplateParams["{email}"] = user.Email;
            notifyUser.TemplateParams["{pass}"] = pass;
            notifyUser.TemplateParams["{answer}"] = securityAnswer;
            notifyUser.TemplateParams["{forumname}"] = YafContext.Current.Get<YafBoardSettings>().Name;

            string emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_TWITTER_REGISTER");

            var messageFlags = new MessageFlags { IsHtml = false, IsBBCode = true };

            // Send Message also as DM to Twitter.
            var tweetApi = new TweetAPI(oAuth);

            if (YafContext.Current.Get<YafBoardSettings>().AllowPrivateMessages)
            {
                LegacyDb.pmessage_save(2, userId, subject, emailBody, messageFlags.BitValue, -1);

                string message = "{0}. {1}".FormatWith(
                subject, YafContext.Current.Get<ILocalization>().GetText("LOGIN", "TWITTER_DM"));

                tweetApi.SendDirectMessage(TweetAPI.ResponseFormat.json, user.UserName, message.Truncate(140));
            }
            else
            {
                string message = YafContext.Current.Get<ILocalization>().GetTextFormatted(
                    "LOGIN", "TWITTER_DM_ACCOUNT", YafContext.Current.Get<YafBoardSettings>().Name, user.UserName, pass);

                tweetApi.SendDirectMessage(TweetAPI.ResponseFormat.json, user.UserName, message.Truncate(140));
            }
        }

        /// <summary>
        /// The send registration notification email.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        private static void SendRegistrationNotificationEmail([NotNull] MembershipUser user)
        {
            string[] emails = YafContext.Current.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.Split(';');

            var notifyAdmin = new YafTemplateEmail();

            string subject =
                YafContext.Current.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_USER_REGISTER_EMAIL_SUBJECT").FormatWith(
                    YafContext.Current.Get<YafBoardSettings>().Name);

            notifyAdmin.TemplateParams["{adminlink}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.admin_admin, true);
            notifyAdmin.TemplateParams["{user}"] = user.UserName;
            notifyAdmin.TemplateParams["{email}"] = user.Email;
            notifyAdmin.TemplateParams["{forumname}"] = YafContext.Current.Get<YafBoardSettings>().Name;

            string emailBody = notifyAdmin.ProcessTemplate("NOTIFICATION_ON_USER_REGISTER");

            foreach (string email in emails.Where(email => email.Trim().IsSet()))
            {
                YafContext.Current.Get<ISendMail>().Queue(YafContext.Current.Get<YafBoardSettings>().ForumEmail, email.Trim(), subject, emailBody);
            }
        }
    }
}
