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
    using System.Text;
    using System.Web;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// Twitter Single Sign On Class
    /// </summary>
    public class Twitter : IAuthBase
    {
        /// <summary>
        ///   Gets or sets the User IP Info.
        /// </summary>
        private IDictionary<string, string> UserIpLocator { get; set; }

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
        /// Returns the Login URL
        /// </returns>
        public string GenerateLoginUrl(bool generatePopUpUrl, bool connectCurrentUser = false)
        {
            var oAuth = new OAuthTwitter
                            {
                                CallBackUrl =
                                    "{0}auth.aspx?auth={1}{2}".FormatWith(
                                        YafForumInfo.ForumBaseUrl, 
                                        AuthService.twitter, 
                                        connectCurrentUser ? "&connectCurrent=true" : string.Empty), 
                                ConsumerKey = Config.TwitterConsumerKey, 
                                ConsumerSecret = Config.TwitterConsumerSecret
                            };

            return generatePopUpUrl
                       ? "javascript:window.open('{0}', 'Twitter Login Window', 'width=800,height=700,left=150,top=100,scrollbar=no,resize=no'); return false;"
                             .FormatWith(oAuth.AuthorizationLinkGet())
                       : oAuth.AuthorizationLinkGet();
        }

        /// <summary>
        /// Logins the or create user.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// Returns if Login was successful or not
        /// </returns>
        public bool LoginOrCreateUser(HttpRequest request, string parameters, out string message)
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
                    var checkUser = YafContext.Current.Get<MembershipProvider>().GetUser(twitterUser.UserName, false);

                    // Login user if exists
                    if (checkUser == null)
                    {
                        return CreateTwitterUser(twitterUser, oAuth, out message);
                    }

                    // LOGIN Existing User
                    var yafUser = YafUserProfile.GetProfile(checkUser.UserName);

                    var yafUserData = new CombinedUserDataHelper(checkUser);

                    if (yafUser.Twitter.IsNotSet() && yafUser.TwitterId.IsNotSet())
                    {
                        // user with the same name exists but account is not connected, exit!
                        message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

                        return false;
                    }

                    if (yafUser.Twitter.Equals(twitterUser.UserName)
                        && yafUser.TwitterId.Equals(twitterUser.UserId.ToString()))
                    {
                        LoginTwitterSuccess(false, oAuth, yafUserData.UserID, checkUser);

                        message = string.Empty;

                        return true;
                    }

                    message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTERID_NOTMATCH");

                    return false;

                    // User does not exist create new user
                }
            }

            message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

            return false;
        }

        /// <summary>
        /// Connects the user.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// Returns if the connect was successful or not
        /// </returns>
        public bool ConnectUser(HttpRequest request, string parameters, out string message)
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
                    // Create User if not exists?!
                    if (!YafContext.Current.IsGuest && !YafContext.Current.Get<YafBoardSettings>().DisableRegistrations)
                    {
                        // Because twitter doesn't provide the email we need to match the user name...
                        if (twitterUser.UserName != YafContext.Current.Profile.UserName)
                        {
                            message = YafContext.Current.Get<ILocalization>()
                                .GetText("LOGIN", "SSO_TWITTERNAME_NOTMATCH");

                            return false;
                        }

                        // Update profile with twitter informations
                        var userProfile = YafContext.Current.Profile;

                        userProfile.TwitterId = twitterUser.UserId.ToString();
                        userProfile.Twitter = twitterUser.UserName;
                        userProfile.Homepage = twitterUser.Url.IsSet()
                                                   ? twitterUser.Url
                                                   : "http://twitter.com/{0}".FormatWith(twitterUser.UserName);
                        userProfile.RealName = twitterUser.Name;
                        userProfile.Interests = twitterUser.Description;
                        userProfile.Location = twitterUser.Location;

                        userProfile.Save();

                        // save avatar
                        if (twitterUser.ProfileImageUrl.IsSet())
                        {
                            LegacyDb.user_saveavatar(
                                YafContext.Current.PageUserID, 
                                twitterUser.ProfileImageUrl, 
                                null, 
                                null);
                        }

                        YafSingleSignOnUser.LoginSuccess(AuthService.twitter, null, YafContext.Current.PageUserID, false);

                        message = string.Empty;

                        return true;
                    }
                }
            }

            message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

            return false;
        }

        /// <summary>
        /// Creates the or assign twitter user.
        /// </summary>
        /// <param name="twitterUser">
        /// The twitter user.
        /// </param>
        /// <param name="oAuth">
        /// The oAUTH.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// Returns if the login was successfully or not
        /// </returns>
        private bool CreateTwitterUser(TwitterUser twitterUser, OAuthTwitter oAuth, out string message)
        {
            if (YafContext.Current.Get<YafBoardSettings>().DisableRegistrations)
            {
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                return false;
            }

            // Create User if not exists?! Doesn't work because there is no Email
            var email = "{0}@twitter.com".FormatWith(twitterUser.UserName);

            // Check user for bot
            /*var spamChecker = new YafSpamCheck();
            string result;
            var isPossibleSpamBot = false;

            var userIpAddress = YafContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress();

            // Check content for spam
            if (spamChecker.CheckUserForSpamBot(twitterUser.UserName, twitterUser.Email, userIpAddress, out result))
            {
                YafContext.Current.Get<ILogger>().Log(
                    null,
                    "Bot Detected",
                    "Bot Check detected a possible SPAM BOT: (user name : '{0}', email : '{1}', ip: '{2}', reason : {3}), user was rejected."
                        .FormatWith(twitterUser.UserName, twitterUser.Email, userIpAddress, result),
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
            }*/

            // Create User if not exists?!
            MembershipCreateStatus status;

            var memberShipProvider = YafContext.Current.Get<MembershipProvider>();

            var pass = Membership.GeneratePassword(32, 16);
            var securityAnswer = Membership.GeneratePassword(64, 30);

            var user = memberShipProvider.CreateUser(
                twitterUser.UserName,
                pass,
                email,
                memberShipProvider.RequiresQuestionAndAnswer ? "Answer is a generated Pass" : null,
                memberShipProvider.RequiresQuestionAndAnswer ? securityAnswer : null,
                true,
                null,
                out status);

            // setup initial roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, twitterUser.UserName);

            // create the user in the YAF DB as well as sync roles...
            var userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

            // create empty profile just so they have one
            var userProfile = YafUserProfile.GetProfile(twitterUser.UserName);

            // setup their initial profile information
            userProfile.Save();

            userProfile.TwitterId = twitterUser.UserId.ToString();
            userProfile.Twitter = twitterUser.UserName;
            userProfile.Homepage = twitterUser.Url.IsSet()
                                       ? twitterUser.Url
                                       : "http://twitter.com/{0}".FormatWith(twitterUser.UserName);
            userProfile.RealName = twitterUser.Name;
            userProfile.Interests = twitterUser.Description;
            userProfile.Location = twitterUser.Location;

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
                message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

                return false;
            }

            if (YafContext.Current.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
            {
                // send user register notification to the following admin users...
                YafContext.Current.Get<ISendNotification>().SendRegistrationNotificationEmail(user, userID.Value);
            }

            // save the time zone...
            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

            // send user register notification to the following admin users...
            SendRegistrationMessageToTwitterUser(user, pass, securityAnswer, userId, oAuth);

            var autoWatchTopicsEnabled = YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting
                                         == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

            LegacyDb.user_save(
                userID: userId,
                boardID: YafContext.Current.PageBoardID,
                userName: twitterUser.UserName,
                displayName: twitterUser.UserName,
                email: email,
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
            if (twitterUser.ProfileImageUrl.IsSet())
            {
                LegacyDb.user_saveavatar(userId, twitterUser.ProfileImageUrl, null, null);
            }

            LoginTwitterSuccess(true, oAuth, userId, user);

            message = YafContext.Current.Get<ILocalization>().GetText("LOGIN", "UPDATE_EMAIL");

            return true;
        }

        /// <summary>
        /// Call the Events when the Twitter Login was Successfully
        /// </summary>
        /// <param name="newUser">
        /// The new user.
        /// </param>
        /// <param name="oAuth">
        /// The twitter oAUTH.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        private static void LoginTwitterSuccess(
            [NotNull] bool newUser, 
            [NotNull] OAuthTwitter oAuth, 
            [NotNull] int userId, 
            [CanBeNull] MembershipUser user)
        {
            if (newUser)
            {
                YafContext.Current.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));
            }
            else
            {
                // Clearing cache with old Active User Lazy Data ...
                YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userId));
            }

            // Store Tokens in Session (Could Bes Stored in DB but it would be a Security Problem)
            YafContext.Current.Get<IYafSession>().TwitterToken = oAuth.Token;
            YafContext.Current.Get<IYafSession>().TwitterTokenSecret = oAuth.TokenSecret;

            YafSingleSignOnUser.LoginSuccess(AuthService.twitter, user.UserName, userId, true);
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
        /// The oAUTH.
        /// </param>
        private static void SendRegistrationMessageToTwitterUser(
            [NotNull] MembershipUser user, 
            [NotNull] string pass, 
            [NotNull] string securityAnswer, 
            [NotNull] int userId, 
            OAuthTwitter oAuth)
        {
            var notifyUser = new YafTemplateEmail();

            var subject =
                YafContext.Current.Get<ILocalization>()
                    .GetText("COMMON", "NOTIFICATION_ON_NEW_FACEBOOK_USER_SUBJECT")
                    .FormatWith(YafContext.Current.Get<YafBoardSettings>().Name);

            notifyUser.TemplateParams["{user}"] = user.UserName;
            notifyUser.TemplateParams["{email}"] = user.Email;
            notifyUser.TemplateParams["{pass}"] = pass;
            notifyUser.TemplateParams["{answer}"] = securityAnswer;
            notifyUser.TemplateParams["{forumname}"] = YafContext.Current.Get<YafBoardSettings>().Name;

            var emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_TWITTER_REGISTER");

            var messageFlags = new MessageFlags { IsHtml = false, IsBBCode = true };

            // Send Message also as DM to Twitter.
            var tweetApi = new TweetAPI(oAuth);

            var message = "{0}. {1}".FormatWith(
                subject, 
                YafContext.Current.Get<ILocalization>().GetText("LOGIN", "TWITTER_DM"));

            if (YafContext.Current.Get<YafBoardSettings>().AllowPrivateMessages)
            {
                LegacyDb.pmessage_save(2, userId, subject, emailBody, messageFlags.BitValue, -1);
            }
            else
            {
                message = YafContext.Current.Get<ILocalization>()
                    .GetTextFormatted(
                        "LOGIN", 
                        "TWITTER_DM_ACCOUNT", 
                        YafContext.Current.Get<YafBoardSettings>().Name, 
                        user.UserName, 
                        pass);
            }

            try
            {
                tweetApi.SendDirectMessage(TweetAPI.ResponseFormat.json, user.UserName, message.Truncate(140));
            }
            catch (Exception ex)
            {
                YafContext.Current.Get<ILogger>().Error(ex, "Error while sending Twitter DM Message");
            }
        }
    }
}