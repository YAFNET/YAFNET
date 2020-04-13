/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0


 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Services.Auth
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Security;

    using ServiceStack;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Auth;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// Facebook Single Sign On Class
    /// </summary>
    public class Facebook : IAuthBase
    {
        /// <summary>
        /// Gets the authorize URL.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// Returns the Authorize URL
        /// </returns>
        public string GetAuthorizeUrl(HttpRequest request)
        {
            const string Scope = "email,user_birthday,user_location";

            var redirectUrl = GetRedirectURL(request);

            return
                $"https://graph.facebook.com/v3.3/oauth/authorize?client_id={Config.FacebookAPIKey}&redirect_uri={redirectUrl}{(redirectUrl.Contains("connectCurrent") ? "&state=connectCurrent" : string.Empty)}&scope={Scope}";
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="authorizationCode">
        /// The authorization code.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// Returns the Access Token
        /// </returns>
        public string GetAccessToken(string authorizationCode, HttpRequest request)
        {
            var urlGetAccessToken =
                $"https://graph.facebook.com/v3.3/oauth/access_token?client_id={Config.FacebookAPIKey}&client_secret={Config.FacebookSecretKey}&redirect_uri={GetRedirectURL(request)}&code={authorizationCode}";

            var responseData = AuthUtilities.WebRequest(AuthUtilities.Method.GET, urlGetAccessToken, null);

            if (responseData.IsNotSet())
            {
                return string.Empty;
            }

            return responseData.FromJson<FacebookAccessToken>().AccessToken ?? string.Empty;
        }

        #region Get Current Facebook User Profile

        /// <summary>
        /// Get The Facebook User Profile of the Current User
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="access_token">
        /// The access_token.
        /// </param>
        /// <returns>
        /// Returns the FacebookUser Profile
        /// </returns>
        public FacebookUser GetFacebookUser(HttpRequest request, string access_token)
        {
            var url =
                $"https://graph.facebook.com/v3.3/me?fields=email,first_name,last_name,location,birthday,gender,name,link&access_token={access_token}";

            return AuthUtilities.WebRequest(AuthUtilities.Method.GET, url, string.Empty).FromJson<FacebookUser>();
        }

        #endregion

        /// <summary>
        /// Generates the login URL.
        /// </summary>
        /// <param name="generatePopUpUrl">
        /// if set to <c>true</c> [generate pop up URL].
        /// </param>
        /// <param name="connectCurrentUser">
        /// if set to <c>true</c> [connect current user].
        /// </param>
        /// <returns>
        /// Returns the login URL
        /// </returns>
        public string GenerateLoginUrl(bool generatePopUpUrl, bool connectCurrentUser = false)
        {
            var authUrl =
                $"{BoardInfo.ForumBaseUrl}auth.aspx?auth={AuthService.facebook}{(connectCurrentUser ? "&connectCurrent=true" : string.Empty)}";

            return authUrl;
        }

        /// <summary>
        /// Logins the or create user.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="parameters">
        /// The access token.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// Returns if Login was successful or not
        /// </returns>
        public bool LoginOrCreateUser(HttpRequest request, string parameters, out string message)
        {
            if (!BoardContext.Current.Get<BoardSettings>().AllowSingleSignOn)
            {
                message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED");

                return false;
            }

            var facebookUser = this.GetFacebookUser(request, parameters);

            // Check if user name is null
            if (facebookUser.UserName.IsNotSet())
            {
                facebookUser.UserName = facebookUser.Name;
            }

            if (facebookUser.Email.IsNotSet())
            {
                message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FACEBOOK_FAILED3");

                return false;
            }

            // Check if user exists
            var userName = BoardContext.Current.Get<MembershipProvider>().GetUserNameByEmail(facebookUser.Email);

            if (userName.IsNotSet())
            {
                var userGender = 0;

                if (!facebookUser.Gender.IsSet())
                {
                    return CreateFacebookUser(facebookUser, userGender, out message);
                }

                userGender = facebookUser.Gender switch
                    {
                        "male" => 1,
                        "female" => 2,
                        _ => userGender
                    };

                // Create User if not exists?!
                return CreateFacebookUser(facebookUser, userGender, out message);
            }

            var yafUser = Utils.UserProfile.GetProfile(userName);

            var yafUserData =
                new CombinedUserDataHelper(BoardContext.Current.Get<MembershipProvider>().GetUser(userName, true));

            // Legacy Handling
            if (ValidationHelper.IsNumeric(yafUser.Facebook))
            {
                if (!yafUser.Facebook.Equals(facebookUser.UserID))
                {
                    message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FACEBOOK_FAILED2");

                    return false;
                }
            }

            if (!yafUser.FacebookId.Equals(facebookUser.UserID))
            {
                message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FACEBOOK_FAILED2");

                return false;
            }

            SingleSignOnUser.LoginSuccess(AuthService.facebook, userName, yafUserData.UserID, true);

            message = string.Empty;

            return true;
        }

        /// <summary>
        /// Connects the user.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="parameters">
        /// The access token.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// Returns if the connect was successful or not
        /// </returns>
        public bool ConnectUser(HttpRequest request, string parameters, out string message)
        {
            var facebookUser = this.GetFacebookUser(request, parameters);

            // Check if user name is null
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

            // Only validated logins can go here
            if (!BoardContext.Current.IsGuest)
            {
                // match the email address...
                if (facebookUser.Email != BoardContext.Current.CurrentUserData.Email)
                {
                    message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FACEBOOKNAME_NOTMATCH");

                    return false;
                }

                // Update profile with facebook informations
                var userProfile = BoardContext.Current.Profile;

                userProfile.Facebook = facebookUser.ProfileURL;
                userProfile.FacebookId = facebookUser.UserID;
                userProfile.Homepage = facebookUser.ProfileURL;

                if (facebookUser.Birthday.IsSet())
                {
                    var ci = CultureInfo.CreateSpecificCulture("en-US");
                    DateTime.TryParse(facebookUser.Birthday, ci, DateTimeStyles.None, out var userBirthdate);

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
                BoardContext.Current.GetRepository<User>().SaveAvatar(
                    BoardContext.Current.PageUserID,
                    $"https://graph.facebook.com/v3.3/{facebookUser.UserID}/picture",
                    null,
                    null);

                SingleSignOnUser.LoginSuccess(AuthService.facebook, null, BoardContext.Current.PageUserID, false);

                message = string.Empty;

                return true;
            }

            message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FACEBOOK_FAILED");
            return false;
        }

        /// <summary>
        /// Gets the redirect URL.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// Returns the Redirect URL
        /// </returns>
        private static string GetRedirectURL(HttpRequest request)
        {
            var urlCurrentPage = request.Url.AbsoluteUri.IndexOf('?') == -1
                                     ? request.Url.AbsoluteUri
                                     : request.Url.AbsoluteUri.Substring(0, request.Url.AbsoluteUri.IndexOf('?'));

            var nvc = new NameValueCollection();

            foreach (var key in request.QueryString.Cast<string>().Where(key => key != "code"))
            {
                nvc.Add(key, request.QueryString[key]);
            }

            var queryString = string.Empty;

            foreach (string key in nvc)
            {
                queryString += queryString == string.Empty ? "?" : "&";
                queryString += $"{key}={nvc[key]}";
            }

            return $"{urlCurrentPage}{queryString}";
        }

        /// <summary>
        /// Creates the facebook user
        /// </summary>
        /// <param name="facebookUser">
        /// The facebook user.
        /// </param>
        /// <param name="userGender">
        /// The user gender.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// Returns if the login was successfully or not
        /// </returns>
        private static bool CreateFacebookUser(FacebookUser facebookUser, int userGender, out string message)
        {
            if (BoardContext.Current.Get<BoardSettings>().DisableRegistrations)
            {
                message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                return false;
            }

            // Check user for bot
            var isPossibleSpamBot = false;

            var userIpAddress = BoardContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress();

            // Check content for spam
            if (BoardContext.Current.Get<ISpamCheck>().CheckUserForSpamBot(facebookUser.UserName, facebookUser.Email, userIpAddress, out var result))
            {
                BoardContext.Current.Get<ILogger>().Log(
                    null,
                    "Bot Detected",
                    $"Bot Check detected a possible SPAM BOT: (user name : '{facebookUser.UserName}', email : '{facebookUser.Email}', ip: '{userIpAddress}', reason : {result}), user was rejected.",
                    EventLogTypes.SpamBotDetected);

                if (BoardContext.Current.Get<BoardSettings>().BotHandlingOnRegister.Equals(1))
                {
                    // Flag user as spam bot
                    isPossibleSpamBot = true;
                }
                else if (BoardContext.Current.Get<BoardSettings>().BotHandlingOnRegister.Equals(2))
                {
                    message = BoardContext.Current.Get<ILocalization>().GetText("BOT_MESSAGE");

                    if (!BoardContext.Current.Get<BoardSettings>().BanBotIpOnDetection)
                    {
                        return false;
                    }

                    BoardContext.Current.GetRepository<BannedIP>()
                        .Save(
                            null,
                            userIpAddress,
                            $"A spam Bot who was trying to register was banned by IP {userIpAddress}",
                            BoardContext.Current.PageUserID);

                    // Clear cache
                    BoardContext.Current.Get<IDataCache>().Remove(Constants.Cache.BannedIP);

                    if (BoardContext.Current.Get<BoardSettings>().LogBannedIP)
                    {
                        BoardContext.Current.Get<ILogger>()
                            .Log(
                                null,
                                "IP BAN of Bot During Registration",
                                $"A spam Bot who was trying to register was banned by IP {userIpAddress}",
                                EventLogTypes.IpBanSet);
                    }

                    return false;
                }
            }

            var memberShipProvider = BoardContext.Current.Get<MembershipProvider>();

            var pass = Membership.GeneratePassword(32, 16);
            var securityAnswer = Membership.GeneratePassword(64, 30);

            var user = memberShipProvider.CreateUser(
                facebookUser.UserName,
                pass,
                facebookUser.Email,
                memberShipProvider.RequiresQuestionAndAnswer ? "Answer is a generated Pass" : null,
                memberShipProvider.RequiresQuestionAndAnswer ? securityAnswer : null,
                true,
                null,
                out var status);

            // setup initial roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(BoardContext.Current.PageBoardID, facebookUser.UserName);

            // create the user in the YAF DB as well as sync roles...
            var userID = RoleMembershipHelper.CreateForumUser(user, BoardContext.Current.PageBoardID);

            // create empty profile just so they have one
            var userProfile = Utils.UserProfile.GetProfile(facebookUser.UserName);

            // setup their initial profile information
            userProfile.Save();

            userProfile.Facebook = facebookUser.ProfileURL;
            userProfile.FacebookId = facebookUser.UserID;
            userProfile.Homepage = facebookUser.ProfileURL;

            if (facebookUser.Birthday.IsSet())
            {
                var ci = CultureInfo.CreateSpecificCulture("en-US");
                DateTime.TryParse(facebookUser.Birthday, ci, DateTimeStyles.None, out var userBirthdate);

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

            if (BoardContext.Current.Get<BoardSettings>().EnableIPInfoService)
            {
                var userIpLocator = BoardContext.Current.Get<IIpInfoService>().GetUserIpLocator();

                if (userIpLocator != null)
                {
                    userProfile.Country = userIpLocator["CountryCode"];
                }
            }

            userProfile.Save();

            // setup their initial profile information
            userProfile.Save();

            if (userID == null)
            {
                // something is seriously wrong here -- redirect to failure...
                message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                return false;
            }

            if (BoardContext.Current.Get<BoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
            {
                // send user register notification to the following admin users...
                BoardContext.Current.Get<ISendNotification>().SendRegistrationNotificationEmail(user, userID.Value);
            }

            if (isPossibleSpamBot)
            {
                BoardContext.Current.Get<ISendNotification>().SendSpamBotNotificationToAdmins(user, userID.Value);
            }

            // send user register notification to the user...
            BoardContext.Current.Get<ISendNotification>()
                .SendRegistrationNotificationToUser(user, pass, securityAnswer, "NOTIFICATION_ON_FACEBOOK_REGISTER");

            // save the time zone...
            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

            var autoWatchTopicsEnabled = BoardContext.Current.Get<BoardSettings>().DefaultNotificationSetting
                                         == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

            BoardContext.Current.GetRepository<User>().Save(
                userId,
                BoardContext.Current.PageBoardID,
                facebookUser.UserName,
                facebookUser.UserName,
                facebookUser.Email,
                TimeZoneInfo.Local.Id,
                null,
                null,
                null,
                null,
                BoardContext.Current.Get<BoardSettings>().DefaultNotificationSetting,
                autoWatchTopicsEnabled,
                TimeZoneInfo.Local.SupportsDaylightSavingTime,
                null,
                null);

            // save the settings...
            BoardContext.Current.GetRepository<User>().SaveNotification(
                userId,
                true,
                autoWatchTopicsEnabled,
                BoardContext.Current.Get<BoardSettings>().DefaultNotificationSetting.ToInt(),
                BoardContext.Current.Get<BoardSettings>().DefaultSendDigestEmail);

            // save avatar
            BoardContext.Current.GetRepository<User>().SaveAvatar(
                userId,
                $"https://graph.facebook.com/v3.3/{facebookUser.UserID}/picture",
                null,
                null);

            BoardContext.Current.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));

            SingleSignOnUser.LoginSuccess(AuthService.facebook, user.UserName, userId, true);

            message = string.Empty;

            return true;
        }
    }
}