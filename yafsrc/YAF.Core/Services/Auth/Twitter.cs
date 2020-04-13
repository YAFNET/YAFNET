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
    using System.Text;
    using System.Web;
    using System.Web.Security;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Auth;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;

    /// <summary>
    /// Twitter Single Sign On Class
    /// </summary>
    public class Twitter : IAuthBase
    {
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
                                    $"{BoardInfo.ForumBaseUrl}auth.aspx?auth={AuthService.twitter}{(connectCurrentUser ? "&connectCurrent=true" : string.Empty)}", 
                                ConsumerKey = Config.TwitterConsumerKey, 
                                ConsumerSecret = Config.TwitterConsumerSecret
                            };

            return generatePopUpUrl
                       ? $"javascript:window.open('{oAuth.AuthorizationLinkGet()}', 'Twitter Login Window', 'width=800,height=700,left=150,top=100,scrollbar=no,resize=no'); return false;"
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
                    var checkUser = BoardContext.Current.Get<MembershipProvider>().GetUser(twitterUser.UserName, false);

                    // Login user if exists
                    if (checkUser == null)
                    {
                        return CreateTwitterUser(twitterUser, oAuth, out message);
                    }

                    // LOGIN Existing User
                    var yafUser = Utils.UserProfile.GetProfile(checkUser.UserName);

                    var yafUserData = new CombinedUserDataHelper(checkUser);

                    if (yafUser.Twitter.IsNotSet() && yafUser.TwitterId.IsNotSet())
                    {
                        // user with the same name exists but account is not connected, exit!
                        message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

                        return false;
                    }

                    if (yafUser.Twitter.Equals(twitterUser.UserName)
                        && yafUser.TwitterId.Equals(twitterUser.UserId.ToString()))
                    {
                        LoginTwitterSuccess(false, oAuth, yafUserData.UserID, checkUser);

                        message = string.Empty;

                        return true;
                    }

                    message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTERID_NOTMATCH");

                    return false;

                    // User does not exist create new user
                }
            }

            message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

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
                    if (!BoardContext.Current.IsGuest && !BoardContext.Current.Get<BoardSettings>().DisableRegistrations)
                    {
                        // Because twitter doesn't provide the email we need to match the user name...
                        if (twitterUser.UserName != BoardContext.Current.Profile.UserName)
                        {
                            message = BoardContext.Current.Get<ILocalization>()
                                .GetText("LOGIN", "SSO_TWITTERNAME_NOTMATCH");

                            return false;
                        }

                        // Update profile with twitter informations
                        var userProfile = BoardContext.Current.Profile;

                        userProfile.TwitterId = twitterUser.UserId.ToString();
                        userProfile.Twitter = twitterUser.UserName;
                        userProfile.Homepage = twitterUser.Url.IsSet()
                                                   ? twitterUser.Url
                                                   : $"http://twitter.com/{twitterUser.UserName}";
                        userProfile.RealName = twitterUser.Name;
                        userProfile.Interests = twitterUser.Description;
                        userProfile.Location = twitterUser.Location;

                        userProfile.Save();

                        // save avatar
                        if (twitterUser.ProfileImageUrl.IsSet())
                        {
                            BoardContext.Current.GetRepository<User>().SaveAvatar(
                                BoardContext.Current.PageUserID, 
                                twitterUser.ProfileImageUrl, 
                                null, 
                                null);
                        }

                        SingleSignOnUser.LoginSuccess(AuthService.twitter, null, BoardContext.Current.PageUserID, false);

                        message = string.Empty;

                        return true;
                    }
                }
            }

            message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

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
        private static bool CreateTwitterUser(TwitterUser twitterUser, OAuthTwitter oAuth, out string message)
        {
            if (BoardContext.Current.Get<BoardSettings>().DisableRegistrations)
            {
                message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                return false;
            }

            // Create User if not exists?! Doesn't work because there is no Email
            var email = $"{twitterUser.UserName}@twitter.com";

            // Check user for bot
            /*var spamChecker = new YafSpamCheck();
            string result;
            var isPossibleSpamBot = false;

            var userIpAddress = BoardContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress();

            // Check content for spam
            if (spamChecker.CheckUserForSpamBot(twitterUser.UserName, twitterUser.Email, userIpAddress, out result))
            {
                BoardContext.Current.Get<ILogger>().Log(
                    null,
                    "Bot Detected",
                    "Bot Check detected a possible SPAM BOT: (user name : '{0}', email : '{1}', ip: '{2}', reason : {3}), user was rejected."
                        .FormatWith(twitterUser.UserName, twitterUser.Email, userIpAddress, result),
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
                            "A spam Bot who was trying to register was banned by IP {0}".FormatWith(userIpAddress),
                            BoardContext.Current.PageUserID);

                    // Clear cache
                    BoardContext.Current.Get<IDataCache>().Remove(Constants.Cache.BannedIP);

                    if (BoardContext.Current.Get<BoardSettings>().LogBannedIP)
                    {
                        BoardContext.Current.Get<ILogger>()
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
            var memberShipProvider = BoardContext.Current.Get<MembershipProvider>();

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
                out var status);

            // setup initial roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(BoardContext.Current.PageBoardID, twitterUser.UserName);

            // create the user in the YAF DB as well as sync roles...
            var userID = RoleMembershipHelper.CreateForumUser(user, BoardContext.Current.PageBoardID);

            // create empty profile just so they have one
            var userProfile = Utils.UserProfile.GetProfile(twitterUser.UserName);

            // setup their initial profile information
            userProfile.Save();

            userProfile.TwitterId = twitterUser.UserId.ToString();
            userProfile.Twitter = twitterUser.UserName;
            userProfile.Homepage = twitterUser.Url.IsSet()
                                       ? twitterUser.Url
                                       : $"http://twitter.com/{twitterUser.UserName}";
            userProfile.RealName = twitterUser.Name;
            userProfile.Interests = twitterUser.Description;
            userProfile.Location = twitterUser.Location;

            if (BoardContext.Current.Get<BoardSettings>().EnableIPInfoService)
            {
                var userIpLocator = BoardContext.Current.Get<IIpInfoService>().GetUserIpLocator();

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
                message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED");

                return false;
            }

            if (BoardContext.Current.Get<BoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
            {
                // send user register notification to the following admin users...
                BoardContext.Current.Get<ISendNotification>().SendRegistrationNotificationEmail(user, userID.Value);
            }

            // save the time zone...
            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

            // send user register notification to the following admin users...
            SendRegistrationMessageToTwitterUser(user, pass, securityAnswer, userId, oAuth);

            var autoWatchTopicsEnabled = BoardContext.Current.Get<BoardSettings>().DefaultNotificationSetting
                                         == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

            BoardContext.Current.GetRepository<User>().Save(
                userId,
                BoardContext.Current.PageBoardID,
                twitterUser.UserName,
                twitterUser.UserName,
                email,
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
            if (twitterUser.ProfileImageUrl.IsSet())
            {
                BoardContext.Current.GetRepository<User>().SaveAvatar(userId, twitterUser.ProfileImageUrl, null, null);
            }

            LoginTwitterSuccess(true, oAuth, userId, user);

            message = BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "UPDATE_EMAIL");

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
                BoardContext.Current.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));
            }
            else
            {
                // Clearing cache with old Active User Lazy Data ...
                BoardContext.Current.Get<IDataCache>().Remove(string.Format(Constants.Cache.ActiveUserLazyData, userId));
            }

            // Store Tokens in Session (Could Bes Stored in DB but it would be a Security Problem)
            BoardContext.Current.Get<ISession>().TwitterToken = oAuth.Token;
            BoardContext.Current.Get<ISession>().TwitterTokenSecret = oAuth.TokenSecret;

            SingleSignOnUser.LoginSuccess(AuthService.twitter, user.UserName, userId, true);
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
            var subject = string.Format(
                BoardContext.Current.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_NEW_FACEBOOK_USER_SUBJECT"),
                BoardContext.Current.Get<BoardSettings>().Name);

            var notifyUser = new TemplateEmail
                                 {
                                     TemplateParams =
                                         {
                                             ["{user}"] = user.UserName,
                                             ["{email}"] = user.Email,
                                             ["{pass}"] = pass,
                                             ["{answer}"] = securityAnswer,
                                             ["{forumname}"] = BoardContext.Current.Get<BoardSettings>().Name
                                         }
                                 };


            var emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_TWITTER_REGISTER");

            var messageFlags = new MessageFlags { IsHtml = false, IsBBCode = true };

            // Send Message also as DM to Twitter.
            var tweetApi = new TweetAPI(oAuth);

            var message = $"{subject}. {BoardContext.Current.Get<ILocalization>().GetText("LOGIN", "TWITTER_DM")}";

            if (BoardContext.Current.Get<BoardSettings>().AllowPrivateMessages)
            {
                BoardContext.Current.GetRepository<PMessage>().SendMessage(2, userId, subject, emailBody, messageFlags.BitValue, -1);
            }
            else
            {
                message = BoardContext.Current.Get<ILocalization>()
                    .GetTextFormatted(
                        "LOGIN", 
                        "TWITTER_DM_ACCOUNT", 
                        BoardContext.Current.Get<BoardSettings>().Name, 
                        user.UserName, 
                        pass);
            }

            try
            {
                tweetApi.SendDirectMessage(TweetAPI.ResponseFormat.json, user.UserName, message.Truncate(140));
            }
            catch (Exception ex)
            {
                BoardContext.Current.Get<ILogger>().Error(ex, "Error while sending Twitter DM Message");
            }
        }
    }
}