/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
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
        ///   Gets or sets the User IP Info.
        /// </summary>
        private IDictionary<string, string> UserIpLocator { get; set; }

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
                "https://accounts.google.com/o/oauth2/auth?client_id={0}&redirect_uri={1}&scope={2}&response_type=code"
                    .FormatWith(
                        Config.GoogleClientID,
                        HttpUtility.UrlEncode(redirectUrl),
                        HttpUtility.UrlEncode(
                            "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email"));
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
            var code = "code={0}".FormatWith(HttpUtility.UrlEncode(authorizationCode));

            return
               AuthUtilities.WebRequest(
                AuthUtilities.Method.POST,
                "https://accounts.google.com/o/oauth2/token",
                "{0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type={4}".FormatWith(
                    code,
                    Config.GoogleClientID,
                    Config.GoogleClientSecret,
                    HttpUtility.UrlEncode(GetRedirectURL(request)),
                    "authorization_code")).FromJson<GoogleTokens>();
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
                                      "OAuth {0}".FormatWith(access_token))
                              };

            return
                AuthUtilities.WebRequest(
                    AuthUtilities.Method.GET,
                    "https://www.googleapis.com/oauth2/v1/userinfo?alt=json",
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
            var authUrl = "{0}auth.aspx?auth={1}{2}".FormatWith(
                YafForumInfo.ForumBaseUrl,
                AuthService.google,
                connectCurrentUser ? "&connectCurrent=true" : string.Empty);

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
            if (!YafContext.Current.Get<YafBoardSettings>().AllowSingleSignOn)
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
                return this.CreateGoogleUser(googleUser, userGender, out message);
            }

            var yafUser = YafUserProfile.GetProfile(userName);

            var yafUserData =
                new CombinedUserDataHelper(YafContext.Current.Get<MembershipProvider>().GetUser(userName, true));

            if (!yafUser.GoogleId.Equals(googleUser.UserID))
            {
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLE_FAILED2");

                return false;
            }

            YafSingleSignOnUser.LoginSuccess(AuthService.google, userName, yafUserData.UserID, true);

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
            if (!YafContext.Current.IsGuest && !YafContext.Current.Get<YafBoardSettings>().DisableRegistrations)
            {
                // Match the Email address?
                if (googleUser.Email != YafContext.Current.CurrentUserData.Email)
                {
                    message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLENAME_NOTMATCH");

                    return false;
                }

                // Update profile with Google informations
                var userProfile = YafContext.Current.Profile;

                userProfile.Google = googleUser.ProfileURL;
                userProfile.GoogleId = googleUser.UserID;
                userProfile.Homepage = googleUser.ProfileURL;

                userProfile.Gender = userGender;

                userProfile.Save();

                // save avatar
                LegacyDb.user_saveavatar(YafContext.Current.PageUserID, googleUser.ProfileImage, null, null);

                YafSingleSignOnUser.LoginSuccess(AuthService.google, null, YafContext.Current.PageUserID, false);

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
                queryString += "{0}={1}".FormatWith(key, nvc[key]);
            }

            return "{0}{1}".FormatWith(urlCurrentPage, queryString);
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
        private bool CreateGoogleUser(GoogleUser googleUser, int userGender, out string message)
        {
            if (YafContext.Current.Get<YafBoardSettings>().DisableRegistrations)
            {
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                return false;
            }

            // Check user for bot
            var spamChecker = new YafSpamCheck();
            string result;
            var isPossibleSpamBot = false;

            var userIpAddress = YafContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress();

            // Check content for spam
            if (spamChecker.CheckUserForSpamBot(googleUser.UserName, googleUser.Email, userIpAddress, out result))
            {
                YafContext.Current.Get<ILogger>().Log(
                    null,
                    "Bot Detected",
                    "Bot Check detected a possible SPAM BOT: (user name : '{0}', email : '{1}', ip: '{2}', reason : {3}), user was rejected."
                        .FormatWith(googleUser.UserName, googleUser.Email, userIpAddress, result),
                    EventLogTypes.SpamBotDetected);

                if (YafContext.Current.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(1))
                {
                    // Flag user as spam bot
                    isPossibleSpamBot = true;
                }
                else if (YafContext.Current.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(2))
                {
                    message = YafContext.Current.Get<ILocalization>().GetText("BOT_MESSAGE");

                    if (!YafContext.Current.Get<YafBoardSettings>().BanBotIpOnDetection)
                    {
                        return false;
                    }

                    YafContext.Current.GetRepository<BannedIP>()
                        .Save(
                            null,
                            userIpAddress,
                            "A spam Bot who was trying to register was banned by IP {0}".FormatWith(userIpAddress),
                            YafContext.Current.PageUserID);

                    // Clear cache
                    YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.BannedIP);

                    if (YafContext.Current.Get<YafBoardSettings>().LogBannedIP)
                    {
                        YafContext.Current.Get<ILogger>()
                            .Log(
                                null,
                                "IP BAN of Bot During Registration",
                                "A spam Bot who was trying to register was banned by IP {0}".FormatWith(
                                    userIpAddress),
                                EventLogTypes.IpBanSet);
                    }

                    return false;
                }
            }

            MembershipCreateStatus status;

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
                out status);

            // setup initial roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, googleUser.UserName);

            // create the user in the YAF DB as well as sync roles...
            var userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

            // create empty profile just so they have one
            var userProfile = YafUserProfile.GetProfile(googleUser.UserName);

            // setup their initial profile information
            userProfile.Save();

            userProfile.Google = googleUser.ProfileURL;
            userProfile.GoogleId = googleUser.UserID;
            userProfile.Homepage = googleUser.ProfileURL;

            userProfile.Gender = userGender;

            if (YafContext.Current.Get<YafBoardSettings>().EnableIPInfoService && this.UserIpLocator == null)
            {
                this.UserIpLocator = new IPDetails().GetData(
                    YafContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    "text",
                    false,
                    YafContext.Current.CurrentForumPage.Localization.Culture.Name,
                    string.Empty,
                    string.Empty);

                if (this.UserIpLocator != null && this.UserIpLocator["StatusCode"] == "OK"
                                               && this.UserIpLocator.Count > 0)
                {
                    userProfile.Country = this.UserIpLocator["CountryCode"];

                    var location = new StringBuilder();

                    if (this.UserIpLocator["RegionName"] != null && this.UserIpLocator["RegionName"].IsSet()
                                                                 && !this.UserIpLocator["RegionName"].Equals("-"))
                    {
                        location.Append(this.UserIpLocator["RegionName"]);
                    }

                    if (this.UserIpLocator["CityName"] != null && this.UserIpLocator["CityName"].IsSet()
                                                               && !this.UserIpLocator["CityName"].Equals("-"))
                    {
                        location.AppendFormat(", {0}", this.UserIpLocator["CityName"]);
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

            if (YafContext.Current.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
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

            var autoWatchTopicsEnabled = YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting
                                         == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

            LegacyDb.user_save(
                userID: userId,
                boardID: YafContext.Current.PageBoardID,
                userName: googleUser.UserName,
                displayName: googleUser.UserName,
                email: googleUser.Email,
                timeZone: TimeZoneInfo.Local.Id,
                languageFile: null,
                culture: null,
                themeFile: null,
                textEditor: null,
                useMobileTheme: null,
                approved: null,
                pmNotification: YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting,
                autoWatchTopics: autoWatchTopicsEnabled,
                dSTUser: TimeZoneInfo.Local.SupportsDaylightSavingTime,
                hideUser: null,
                notificationType: null);

            // save the settings...
            LegacyDb.user_savenotification(
                userId,
                true,
                autoWatchTopicsEnabled,
                YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting,
                YafContext.Current.Get<YafBoardSettings>().DefaultSendDigestEmail);

            // save avatar
            LegacyDb.user_saveavatar(userId, googleUser.ProfileImage, null, null);

            YafContext.Current.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));

            YafSingleSignOnUser.LoginSuccess(AuthService.google, user.UserName, userId, true);

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