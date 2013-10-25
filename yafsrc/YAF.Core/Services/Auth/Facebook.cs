/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Björnar Henden
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

namespace YAF.Core.Services.Auth
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Extensions;
    using YAF.Utils.Helpers;

    /// <summary>
    /// Facebook Single Sign On Class
    /// </summary>
    public class Facebook : IAuthBase
    {
        /// <summary>
        /// Gets the authorize URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns the Authorize URL</returns>
        public string GetAuthorizeUrl(HttpRequest request)
        {
            const string SCOPE = "email,user_birthday,status_update,publish_stream,user_location";

            var redirectUrl = this.GetRedirectURL(request);

            return
                "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}{2}&scope={3}".FormatWith(
                    Config.FacebookAPIKey,
                    redirectUrl,
                    redirectUrl.Contains("connectCurrent") ? "&state=connectCurrent" : string.Empty,
                    SCOPE);
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="authorizationCode">The authorization code.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Returns the Access Token
        /// </returns>
        public string GetAccessToken(string authorizationCode, HttpRequest request)
        {
            var urlGetAccessToken =
                "https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&redirect_uri={2}&code={3}"
                    .FormatWith(
                        Config.FacebookAPIKey,
                        Config.FacebookSecretKey,
                        this.GetRedirectURL(request),
                        authorizationCode);

            var responseData = AuthUtilities.WebRequest(AuthUtilities.Method.GET, urlGetAccessToken, null);

            if (responseData.IsNotSet())
            {
                return string.Empty;
            }

            var queryStringCollection = HttpUtility.ParseQueryString(responseData);

            return queryStringCollection["access_token"] ?? string.Empty;
        }

        #region Get Current Facebook User Profile

        /// <summary>
        /// Get The Facebook User Profile of the Current User
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="access_token">The access_token.</param>
        /// <returns>
        /// Returns the FacebookUser Profile
        /// </returns>
        public FacebookUser GetFacebookUser(HttpRequest request, string access_token)
        {
            var url = "https://graph.facebook.com/me?access_token={0}".FormatWith(access_token);

            return AuthUtilities.WebRequest(AuthUtilities.Method.GET, url, string.Empty).FromJson<FacebookUser>();
        }

        #endregion

        /// <summary>
        /// Generates the login URL.
        /// </summary>
        /// <param name="generatePopUpUrl">if set to <c>true</c> [generate pop up URL].</param>
        /// <param name="connectCurrentUser">if set to <c>true</c> [connect current user].</param>
        /// <returns>Returns the login URL</returns>
        public string GenerateLoginUrl(bool generatePopUpUrl, bool connectCurrentUser = false)
        {
            var authUrl = "{0}auth.aspx?auth={1}{2}".FormatWith(
                YafForumInfo.ForumBaseUrl, 
                AuthService.facebook,
                connectCurrentUser ? "&connectCurrent=true" : string.Empty);

            return authUrl;
        }

        /// <summary>
        /// Logins the or create user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="parameters">The access token.</param>
        /// <param name="message">The message.</param>
        /// <returns>Returns if Login was successful or not</returns>
        public bool LoginOrCreateUser(HttpRequest request, string parameters, out string message)
        {
            if (!YafContext.Current.Get<YafBoardSettings>().AllowSingleSignOn)
            {
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED");

                return false;
            }

            var facebookUser = this.GetFacebookUser(request, parameters);

            // Check if username is null
            if (facebookUser.UserName.IsNotSet())
            {
                facebookUser.UserName = facebookUser.Name;
            }

           // Check if user exists
            var userName = YafContext.Current.Get<MembershipProvider>().GetUserNameByEmail(facebookUser.Email);

            if (userName.IsNotSet())
            {
                var userGender = 0;

                if (facebookUser.Gender.IsSet())
                {
                    switch (facebookUser.Gender)
                    {
                        case "male":
                            userGender = 1;
                            break;
                        case "female":
                            userGender = 2;
                            break;
                    }
                }

                // Create User if not exists?!
                return this.CreateFacebookUser(facebookUser, userGender, out message);
            }

            var yafUser = YafUserProfile.GetProfile(userName);

            var yafUserData =
                new CombinedUserDataHelper(YafContext.Current.Get<MembershipProvider>().GetUser(userName, true));

            // Legacy Handling
            if (ValidationHelper.IsNumeric(yafUser.Facebook))
            {
                if (!yafUser.Facebook.Equals(facebookUser.UserID))
                {
                    message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FACEBOOK_FAILED");

                    return false;
                }
            }

            if (!yafUser.FacebookId.Equals(facebookUser.UserID))
            {
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FACEBOOK_FAILED");

                return false;
            }

            YafSingleSignOnUser.LoginSuccess(AuthService.facebook, userName, yafUserData.UserID, true);

            message = string.Empty;

            return true;
        }

        /// <summary>
        /// Connects the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="parameters">The access token.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// Returns if the connect was successful or not
        /// </returns>
        public bool ConnectUser(HttpRequest request, string parameters, out string message)
        {
            var facebookUser = this.GetFacebookUser(request, parameters);

            // Check if username is null
            if (facebookUser.UserName.IsNotSet())
            {
                facebookUser.UserName = facebookUser.Name;
            }

            var userGender = 0;

            if (facebookUser.Gender.IsSet())
            {
                switch (facebookUser.Gender)
                {
                    case "male":
                        userGender = 1;
                        break;
                    case "female":
                        userGender = 2;
                        break;
                }
            }

            // Create User if not exists?!
            if (!YafContext.Current.IsGuest && !YafContext.Current.Get<YafBoardSettings>().DisableRegistrations)
            {
                // match the email address...
                if (facebookUser.Email != YafContext.Current.CurrentUserData.Email)
                {
                    message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FACEBOOKNAME_NOTMATCH");

                    return false;
                }

                // Update profile with facebook informations
                YafUserProfile userProfile = YafContext.Current.Profile;

                userProfile.Facebook = facebookUser.ProfileURL;
                userProfile.FacebookId = facebookUser.UserID;
                userProfile.Homepage = facebookUser.ProfileURL;

                if (facebookUser.Birthday.IsSet())
                {
                    DateTime userBirthdate;
                    var ci = CultureInfo.CreateSpecificCulture("en-US");
                    DateTime.TryParse(facebookUser.Birthday, ci, DateTimeStyles.None, out userBirthdate);

                    if (userBirthdate > DateTimeHelper.SqlDbMinTime().Date)
                    {
                        userProfile.Birthday = userBirthdate;
                    }
                }

                userProfile.RealName = facebookUser.Name;
                userProfile.Gender = userGender;

                if (facebookUser.Location != null && facebookUser.Location.Name.IsSet())
                {
                    userProfile.Location = facebookUser.Location.Name;
                }

                userProfile.Save();

                // save avatar
                LegacyDb.user_saveavatar(
                    YafContext.Current.PageUserID,
                    "https://graph.facebook.com/{0}/picture".FormatWith(facebookUser.UserID),
                    null,
                    null);

                YafSingleSignOnUser.LoginSuccess(AuthService.facebook, null, YafContext.Current.PageUserID, false);

                message = string.Empty;

                return true;
            }

            message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FACEBOOK_FAILED");
            return false;
        }

        /// <summary>
        /// Gets the redirect URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns the Redirect URL</returns>
        private string GetRedirectURL(HttpRequest request)
        {
            var urlCurrentPage = request.Url.AbsoluteUri.IndexOf('?') == -1
                                     ? request.Url.AbsoluteUri
                                     : request.Url.AbsoluteUri.Substring(0, request.Url.AbsoluteUri.IndexOf('?'));

            var nvc = new NameValueCollection();

            foreach (string key in request.QueryString.Cast<string>().Where(key => key != "code"))
            {
                nvc.Add(key, request.QueryString[key]);
            }

            var queryString = string.Empty;

            foreach (string key in nvc)
            {
                queryString += queryString == string.Empty ? "?" : "&";
                queryString += "{0}={1}".FormatWith(key, nvc[key]);
            }

            return "{0}{1}".FormatWith(urlCurrentPage, queryString);
        }

        /// <summary>
        /// Creates the facebook user
        /// </summary>
        /// <param name="facebookUser">The facebook user.</param>
        /// <param name="userGender">The user gender.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// Returns if the login was successfully or not
        /// </returns>
        private bool CreateFacebookUser(FacebookUser facebookUser, int userGender, out string message)
        {
            if (YafContext.Current.Get<YafBoardSettings>().DisableRegistrations)
            {
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                return false;
            }

            MembershipCreateStatus status;

            var pass = Membership.GeneratePassword(32, 16);
            var securityAnswer = Membership.GeneratePassword(64, 30);

            MembershipUser user = YafContext.Current.Get<MembershipProvider>()
                .CreateUser(
                    facebookUser.UserName,
                    pass,
                    facebookUser.Email,
                    "Answer is a generated Pass",
                    securityAnswer,
                    true,
                    null,
                    out status);

            // setup inital roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, facebookUser.UserName);

            // create the user in the YAF DB as well as sync roles...
            int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

            // create empty profile just so they have one
            YafUserProfile userProfile = YafUserProfile.GetProfile(facebookUser.UserName);

            userProfile.Facebook = facebookUser.ProfileURL;
            userProfile.FacebookId = facebookUser.UserID;
            userProfile.Homepage = facebookUser.ProfileURL;

            if (facebookUser.Birthday.IsSet())
            {
                DateTime userBirthdate;
                var ci = CultureInfo.CreateSpecificCulture("en-US");
                DateTime.TryParse(facebookUser.Birthday, ci, DateTimeStyles.None, out userBirthdate);

                if (userBirthdate > DateTimeHelper.SqlDbMinTime().Date)
                {
                    userProfile.Birthday = userBirthdate;
                }
            }

            userProfile.RealName = facebookUser.Name;
            userProfile.Gender = userGender;

            if (facebookUser.Location != null && facebookUser.Location.Name.IsSet())
            {
                userProfile.Location = facebookUser.Location.Name;
            }

            userProfile.Save();

            // setup their inital profile information
            userProfile.Save();

            if (userID == null)
            {
                // something is seriously wrong here -- redirect to failure...
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                return false;
            }

            if (YafContext.Current.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
            {
                // send user register notification to the following admin users...
                YafSingleSignOnUser.SendRegistrationNotificationEmail(user);
            }

            // send user register notification to the user...
            YafContext.Current.Get<ISendNotification>()
                .SendRegistrationNotificationToUser(user, pass, securityAnswer, "NOTIFICATION_ON_FACEBOOK_REGISTER");

            // save the time zone...
            int userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

            LegacyDb.user_save(
                userId,
                YafContext.Current.PageBoardID,
                facebookUser.UserName,
                facebookUser.UserName,
                facebookUser.Email,
                0,
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

            bool autoWatchTopicsEnabled = YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting
                                          == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

            // save the settings...
            LegacyDb.user_savenotification(
                userId,
                true,
                autoWatchTopicsEnabled,
                YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting,
                YafContext.Current.Get<YafBoardSettings>().DefaultSendDigestEmail);

            // save avatar
            LegacyDb.user_saveavatar(
                userId,
                "https://graph.facebook.com/{0}/picture".FormatWith(facebookUser.UserID),
                null,
                null);

            YafContext.Current.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));

            YafSingleSignOnUser.LoginSuccess(AuthService.facebook, user.UserName, userId, true);

            message = string.Empty;

            return true;
        }
    }
}