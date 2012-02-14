/*
 * (Copyright (c) 2011, Shannon Whitley <swhitley@whitleymedia.com> http://voiceoftech.com/
 * 
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted 
 * provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of conditions 
 * and the following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions
 * and the following disclaimer in the documentation and/or other materials provided with the distribution.

 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR 
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND 
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS 
 * BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace YAF
{
    using System;
    using System.Linq;
    using System.Web.Security;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Core.Services.Twitter;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// The Twitter Authentification Page
    /// </summary>
    public partial class Auth : Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.GetFirstOrDefault("denied") != null)
            {
                Response.Clear();
                Response.Write(
                    "<script type='text/javascript'>window.opener.location.href = '{0}';window.close();</script>".FormatWith(
                    YafBuildLink.GetLink(ForumPages.login).Replace("auth.aspx", "default.aspx")));

                return;
            }

            // Twitter oAuth Start
            if (Request["twitterauth"] != null && Request["twitterauth"] == "true")
            {
                var oAuth = new OAuthTwitter
                    {
                        CallBackUrl = this.Request.Url.AbsoluteUri.Replace("twitterauth=true", "twitterauth=false"),
                        ConsumerKey = Config.TwitterConsumerKey,
                        ConsumerSecret = Config.TwitterConsumerSecret
                    };

                // Redirect the user to Twitter for authorization.
                Response.Redirect(oAuth.AuthorizationLinkGet());
            }

            // Twitter Return
            if (Request["twitterauth"] != null && Request["twitterauth"] == "false")
            {
                var oAuth = new OAuthTwitter
                    {
                        ConsumerKey = Config.TwitterConsumerKey, 
                        ConsumerSecret = Config.TwitterConsumerSecret
                    };

                // Get the access token and secret.
                oAuth.AccessTokenGet(Request["oauth_token"], Request["oauth_verifier"]);

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
                                Response.Clear();
                                Response.Write(
                                    "<script type='text/javascript'>alert('{0}');window.opener.location.href = '{1}';window.close();</script>"
                                        .FormatWith(
                                            YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED_BYUSER"),
                                            YafBuildLink.GetLink(ForumPages.login).Replace("auth.aspx", "default.aspx")));
                            }

                            if (yafUser.Twitter.Equals(twitterUser.UserName) && yafUser.TwitterId.Equals(twitterUser.UserId))
                            {
                                this.LoginSuccess(false, oAuth, yafUserData.UserID, null);

                                Response.Clear();
                                Response.Write(
                                    "<script type='text/javascript'>window.opener.location.href = '{0}';window.close();</script>"
                                        .FormatWith(
                                            YafBuildLink.GetLink(ForumPages.forum).Replace("auth.aspx", "default.aspx")));

                                return;
                            }

                            Response.Clear();
                            Response.Write(
                                "<script type='text/javascript'>alert('{0}');window.opener.location.href = '{1}';window.close();</script>"
                                    .FormatWith(
                                        YafContext.Current.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTERID_NOTMATCH"),
                                            YafBuildLink.GetLink(ForumPages.login).Replace("auth.aspx", "default.aspx")));

                            return;
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
                                this.Response.Clear();
                                this.Response.Write(
                                    "<script type='text/javascript'>alert('{0}');window.opener.location.href = '{1}';window.close();</script>"
                                        .FormatWith(
                                            YafContext.Current.Get<ILocalization>().GetText(
                                                "LOGIN", "SSO_TWITTER_FAILED"),
                                            YafBuildLink.GetLink(ForumPages.login).Replace("auth.aspx", "default.aspx")));

                                return;
                            }

                            if (YafContext.Current.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
                            {
                                // send user register notification to the following admin users...
                                this.SendRegistrationNotificationEmail(user);
                            }

                            // save the time zone...
                            int userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

                            // send user register notification to the following admin users...
                            this.SendRegistrationMessageToUser(user, pass, securityAnswer, userId, oAuth);

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

                            this.LoginSuccess(true, oAuth, userId, user);

                            this.Response.Clear();
                            this.Response.Write(
                                "<script type='text/javascript'>alert('{0}');window.opener.location.href = '{1}';window.close();</script>"
                                    .FormatWith(
                                        YafContext.Current.Get<ILocalization>().GetText(
                                            "LOGIN", "UPDATE_EMAIL"),
                                        YafBuildLink.GetLink(ForumPages.cp_editprofile).Replace("auth.aspx", "default.aspx")));
                        }
                        else
                        {
                            this.Response.Clear();
                            this.Response.Write(
                                "<script type='text/javascript'>alert('{0}');window.opener.location.href = '{1}';window.close();</script>"
                                    .FormatWith(
                                        YafContext.Current.Get<ILocalization>().GetText(
                                            "LOGIN", "SSO_TWITTER_FAILED"),
                                        YafBuildLink.GetLink(ForumPages.login).Replace("auth.aspx", "default.aspx")));
                        }
                    }
                }
            }
            else
            {
                Response.Write(
                   "<script type='text/javascript'>window.opener.location.href = '{0}';window.close();</script>".FormatWith(
                   YafBuildLink.GetLink(ForumPages.login).Replace("auth.aspx", "default.aspx")));
            }
        }

        /// <summary>
        /// Call the Events when the Login was Successfully
        /// </summary>
        /// <param name="newUser">
        /// The new user.
        /// </param>
        /// <param name="oAuth">
        /// The o auth.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        private void LoginSuccess([NotNull]bool newUser, [NotNull]OAuthTwitter oAuth, [NotNull]int userId, [CanBeNull]MembershipUser user)
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
        private void SendRegistrationMessageToUser([NotNull] MembershipUser user, [NotNull] string pass, [NotNull] string securityAnswer, [NotNull] int userId, OAuthTwitter oAuth)
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

            string emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_FACEBOOK_REGISTER");

            var messageFlags = new MessageFlags { IsHtml = false, IsBBCode = true };

            // Send Message also as DM to Twitter.
            var tweetApi = new TweetAPI(oAuth);

            if (YafContext.Current.Get<YafBoardSettings>().AllowPrivateMessages)
            {
                LegacyDb.pmessage_save(2, userId, subject, emailBody, messageFlags.BitValue);

                string message = "{0}. {1}".FormatWith(
                subject, YafContext.Current.Get<ILocalization>().GetText("LOGIN", "TWITTER_DM"));

                tweetApi.SendDirectMessage(TweetAPI.ResponseFormat.json, user.UserName, message.Truncate(140));
            }
            else
            {
                string message = YafContext.Current.Get<ILocalization>().GetTextFormatted(
                    "LOGIN", "TWITTER_DM", YafContext.Current.Get<YafBoardSettings>().Name, user.UserName, pass);

                tweetApi.SendDirectMessage(TweetAPI.ResponseFormat.json, user.UserName, message.Truncate(140));
            }
        }

        /// <summary>
        /// The send registration notification email.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        private void SendRegistrationNotificationEmail([NotNull] MembershipUser user)
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