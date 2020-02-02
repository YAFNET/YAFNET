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
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Web;
    using System.Web.Security;

    using ServiceStack;

    using YAF.Configuration;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Extensions;
    using YAF.Utils.Helpers;

    /// <summary>
    /// Google Single Sign On Class
    /// </summary>
    public class Google : IAuthBase
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
            var redirectUrl = GetRedirectURL(request);

            return
                $"https://accounts.google.com/o/oauth2/v2/auth?client_id={Config.GoogleClientID}&redirect_uri={HttpUtility.UrlEncode(redirectUrl)}&scope={HttpUtility.UrlEncode("https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email")}&response_type=code";
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
        public GoogleTokens GetAccessToken(string authorizationCode, HttpRequest request)
        {
            var code = $"code={HttpUtility.UrlEncode(authorizationCode)}";

            return
               AuthUtilities.WebRequest(
                AuthUtilities.Method.POST,
                "https://www.googleapis.com/oauth2/v4/token",
                $"{code}&client_id={Config.GoogleClientID}&client_secret={Config.GoogleClientSecret}&redirect_uri={HttpUtility.UrlEncode(GetRedirectURL(request))}&grant_type=authorization_code").FromJson<GoogleTokens>();
        }

        #region Get Current Google User Profile

        /// <summary>
        /// Get The Google User Profile of the Current User
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="access_token">
        /// The access_token.
        /// </param>
        /// <returns>
        /// Returns the GoogleUser Profile
        /// </returns>
        public GoogleUser GetGoogleUser(HttpRequest request, string access_token)
        {
            var headers = new List<KeyValuePair<string, string>>
                              {
                                  new KeyValuePair<string, string>(
                                      "Authorization",
                                      $"OAuth {access_token}")
                              };

            return
                AuthUtilities.WebRequest(
                    AuthUtilities.Method.GET,
                    "https://www.googleapis.com/oauth2/v2/userinfo?alt=json",
                    string.Empty,
                    headers).FromJson<GoogleUser>();
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
                $"{BoardInfo.ForumBaseUrl}auth.aspx?auth={AuthService.google}{(connectCurrentUser ? "&connectCurrent=true" : string.Empty)}";

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
            if (!YafContext.Current.Get<BoardSettings>().AllowSingleSignOn)
            {
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED");

                return false;
            }

            var googleUser = this.GetGoogleUser(request, parameters);

            var userGender = 0;

            if (googleUser.Gender.IsSet())
            {
                switch (googleUser.Gender)
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
            var userName = YafContext.Current.Get<MembershipProvider>().GetUserNameByEmail(googleUser.Email);

            if (userName.IsNotSet())
            {
                // Create User if not exists?!
                return CreateGoogleUser(googleUser, userGender, out message);
            }

            var yafUser = YafUserProfile.GetProfile(userName);

            var yafUserData =
                new CombinedUserDataHelper(YafContext.Current.Get<MembershipProvider>().GetUser(userName, true));

            if (!yafUser.GoogleId.Equals(googleUser.UserID))
            {
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLE_FAILED2");

                return false;
            }

            SingleSignOnUser.LoginSuccess(AuthService.google, userName, yafUserData.UserID, true);

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
            var googleUser = this.GetGoogleUser(request, parameters);

            var userGender = 0;

            if (googleUser.Gender.IsSet())
            {
                switch (googleUser.Gender)
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
            if (!YafContext.Current.IsGuest && !YafContext.Current.Get<BoardSettings>().DisableRegistrations)
            {
                // Match the Email address?
                if (googleUser.Email != YafContext.Current.CurrentUserData.Email)
                {
                    message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLENAME_NOTMATCH");

                    return false;
                }

                // Update profile with Google informations
                var userProfile = YafContext.Current.Profile;

                userProfile.GoogleId = googleUser.UserID;
                userProfile.Homepage = googleUser.ProfileURL;

                userProfile.Gender = userGender;

                userProfile.Save();

                // save avatar
                YafContext.Current.GetRepository<User>().SaveAvatar(YafContext.Current.PageUserID, googleUser.ProfileImage, null, null);

                SingleSignOnUser.LoginSuccess(AuthService.google, null, YafContext.Current.PageUserID, false);

                message = string.Empty;

                return true;
            }

            message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLE_FAILED");
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
        /// Creates the Google user
        /// </summary>
        /// <param name="googleUser">
        /// The Google user.
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
        private static bool CreateGoogleUser(GoogleUser googleUser, int userGender, out string message)
        {
            if (YafContext.Current.Get<BoardSettings>().DisableRegistrations)
            {
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                return false;
            }

            // Check user for bot
            var isPossibleSpamBot = false;

            var userIpAddress = YafContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress();

            // Check content for spam
            if (YafContext.Current.Get<ISpamCheck>().CheckUserForSpamBot(googleUser.UserName, googleUser.Email, userIpAddress, out var result))
            {
                YafContext.Current.Get<ILogger>().Log(
                    null,
                    "Bot Detected",
                    $"Bot Check detected a possible SPAM BOT: (user name : '{googleUser.UserName}', email : '{googleUser.Email}', ip: '{userIpAddress}', reason : {result}), user was rejected.",
                    EventLogTypes.SpamBotDetected);

                if (YafContext.Current.Get<BoardSettings>().BotHandlingOnRegister.Equals(1))
                {
                    // Flag user as spam bot
                    isPossibleSpamBot = true;
                }
                else if (YafContext.Current.Get<BoardSettings>().BotHandlingOnRegister.Equals(2))
                {
                    message = YafContext.Current.Get<ILocalization>().GetText("BOT_MESSAGE");

                    if (!YafContext.Current.Get<BoardSettings>().BanBotIpOnDetection)
                    {
                        return false;
                    }

                    YafContext.Current.GetRepository<BannedIP>()
                        .Save(
                            null,
                            userIpAddress,
                            $"A spam Bot who was trying to register was banned by IP {userIpAddress}",
                            YafContext.Current.PageUserID);

                    // Clear cache
                    YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.BannedIP);

                    if (YafContext.Current.Get<BoardSettings>().LogBannedIP)
                    {
                        YafContext.Current.Get<ILogger>()
                            .Log(
                                null,
                                "IP BAN of Bot During Registration",
                                $"A spam Bot who was trying to register was banned by IP {userIpAddress}",
                                EventLogTypes.IpBanSet);
                    }

                    return false;
                }
            }

            var memberShipProvider = YafContext.Current.Get<MembershipProvider>();

            var pass = Membership.GeneratePassword(32, 16);
            var securityAnswer = Membership.GeneratePassword(64, 30);

            var user = memberShipProvider.CreateUser(
                googleUser.UserName,
                pass,
                googleUser.Email,
                memberShipProvider.RequiresQuestionAndAnswer ? "Answer is a generated Pass" : null,
                memberShipProvider.RequiresQuestionAndAnswer ? securityAnswer : null,
                true,
                null,
                out var status);

            // setup initial roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, googleUser.UserName);

            // create the user in the YAF DB as well as sync roles...
            var userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

            // create empty profile just so they have one
            var userProfile = YafUserProfile.GetProfile(googleUser.UserName);

            // setup their initial profile information
            userProfile.Save();

            userProfile.GoogleId = googleUser.UserID;
            userProfile.Homepage = googleUser.ProfileURL;

            userProfile.Gender = userGender;

            if (YafContext.Current.Get<BoardSettings>().EnableIPInfoService)
            {
                var userIpLocator = YafContext.Current.Get<IIpInfoService>().GetUserIpLocator();

                if (userIpLocator != null)
                {
                    userProfile.Country = userIpLocator["CountryCode"];

                    var location = new StringBuilder();

                    if (userIpLocator["RegionName"] != null && userIpLocator["RegionName"].IsSet()
                                                                 && !userIpLocator["RegionName"].Equals("-"))
                    {
                        location.Append(userIpLocator["RegionName"]);
                    }

                    if (userIpLocator["CityName"] != null && userIpLocator["CityName"].IsSet()
                                                               && !userIpLocator["CityName"].Equals("-"))
                    {
                        location.AppendFormat(", {0}", userIpLocator["CityName"]);
                    }

                    userProfile.Location = location.ToString();
                }
            }

            userProfile.Save();

            if (userID == null)
            {
                // something is seriously wrong here -- redirect to failure...
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                return false;
            }

            if (YafContext.Current.Get<BoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
            {
                // send user register notification to the following admin users...
                YafContext.Current.Get<ISendNotification>().SendRegistrationNotificationEmail(user, userID.Value);
            }

            if (isPossibleSpamBot)
            {
                YafContext.Current.Get<ISendNotification>().SendSpamBotNotificationToAdmins(user, userID.Value);
            }

            // send user register notification to the user...
            YafContext.Current.Get<ISendNotification>()
                .SendRegistrationNotificationToUser(user, pass, securityAnswer, "NOTIFICATION_ON_GOOGLE_REGISTER");

            // save the time zone...
            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

            var autoWatchTopicsEnabled = YafContext.Current.Get<BoardSettings>().DefaultNotificationSetting
                                         == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

            YafContext.Current.GetRepository<User>().Save(
                userId,
                YafContext.Current.PageBoardID,
                googleUser.UserName,
                googleUser.UserName,
                googleUser.Email,
                TimeZoneInfo.Local.Id,
                null,
                null,
                null,
                null,
                null,
                YafContext.Current.Get<BoardSettings>().DefaultNotificationSetting,
                autoWatchTopicsEnabled,
                TimeZoneInfo.Local.SupportsDaylightSavingTime,
                null,
                null);

            // save the settings...
            YafContext.Current.GetRepository<User>().SaveNotification(
                userId,
                true,
                autoWatchTopicsEnabled,
                YafContext.Current.Get<BoardSettings>().DefaultNotificationSetting.ToInt(),
                YafContext.Current.Get<BoardSettings>().DefaultSendDigestEmail);

            // save avatar
            YafContext.Current.GetRepository<User>().SaveAvatar(userId, googleUser.ProfileImage, null, null);

            YafContext.Current.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));

            SingleSignOnUser.LoginSuccess(AuthService.google, user.UserName, userId, true);

            message = string.Empty;

            return true;
        }
    }

    /// <summary>
    /// Google Token Class
    /// </summary>
    [DataContract]
    public class GoogleTokens
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the expires in.
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        [DataMember(Name = "id_token")]
        public string IDToken { get; set; }
    }
}