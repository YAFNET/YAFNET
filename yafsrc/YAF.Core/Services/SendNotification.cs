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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Identity.Interfaces;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.IdentityModels;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The YAF Send Notification.
    /// </summary>
    public class SendNotification : ISendNotification, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SendNotification"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public SendNotification([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        /// Gets the board settings.
        /// </summary>
        /// <value>
        /// The board settings.
        /// </value>
        public BoardSettings BoardSettings => this.Get<BoardSettings>();

        #endregion

        #region Implemented Interfaces

        #region ISendNotification

        /// <summary>
        /// Sends Notifications to Moderators that Message Needs Approval
        /// </summary>
        /// <param name="forumId">The forum id.</param>
        /// <param name="newMessageId">The new message id.</param>
        /// <param name="isSpamMessage">if set to <c>true</c> [is spam message].</param>
        public void ToModeratorsThatMessageNeedsApproval(int forumId, int newMessageId, bool isSpamMessage)
        {
            var moderatorsFiltered = this.Get<DataBroker>().GetAllModerators().Where(f => f.ForumID.Equals(forumId));
            var moderatorUserNames = new List<string>();

            moderatorsFiltered.ForEach(
                moderator =>
                    {
                        if (moderator.IsGroup)
                        {
                            moderatorUserNames.AddRange(RoleMembershipHelper.GetUsersInRole(moderator.Name).Select(u => u.UserName));
                        }
                        else
                        {
                            moderatorUserNames.Add(moderator.Name);
                        }
                    });

            var themeCss =
                $"{this.Get<BoardSettings>().BaseUrlMask}{this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")}";

            var forumLink = BoardInfo.ForumURL;

            var adminLink = BuildLink.GetLinkNotEscaped(ForumPages.Moderate_UnapprovedPosts, true, "f={0}", forumId);

            var currentContext = HttpContext.Current;

            // send each message...
            moderatorUserNames.Distinct().AsParallel().ForAll(
                userName =>
                    {
                        HttpContext.Current = currentContext;

                        try
                        {
                            // add each member of the group
                            var membershipUser = UserMembershipHelper.GetUserByName(userName);
                            var userId =
                                UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.Id);

                            var languageFile = UserHelper.GetUserLanguageFile(userId);

                            var subject = string.Format(
                                this.Get<ILocalization>().GetText(
                                    "COMMON",
                                    isSpamMessage
                                        ? "NOTIFICATION_ON_MODERATOR_SPAMMESSAGE_APPROVAL"
                                        : "NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL",
                                    languageFile),
                                this.BoardSettings.Name);

                            var notifyModerators =
                                new TemplateEmail(
                                    isSpamMessage
                                        ? "NOTIFICATION_ON_MODERATOR_SPAMMESSAGE_APPROVAL"
                                        : "NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL")
                                    {
                                        TemplateLanguageFile = languageFile,
                                        TemplateParams =
                                            {
                                                ["{user}"] = userName,
                                                ["{adminlink}"] = adminLink,
                                                ["{themecss}"] = themeCss,
                                                ["{forumlink}"] = forumLink
                                            }
                                    };

                            notifyModerators.SendEmail(
                                new MailAddress(membershipUser.Email, membershipUser.UserName),
                                subject);
                        }
                        finally
                        {
                            HttpContext.Current = null;
                        }
                    });
        }

        /// <summary>
        /// Sends Notifications to Moderators that a Message was Reported
        /// </summary>
        /// <param name="pageForumID">
        /// The page Forum ID.
        /// </param>
        /// <param name="reportedMessageId">
        /// The reported message id.
        /// </param>
        /// <param name="reporter">
        /// The reporter.
        /// </param>
        /// <param name="reportText">
        /// The report Text.
        /// </param>
        public void ToModeratorsThatMessageWasReported(
            int pageForumID,
            int reportedMessageId,
            int reporter,
            string reportText)
        {
            try
            {
                var moderatorsFiltered =
                    this.Get<DataBroker>().GetAllModerators().Where(f => f.ForumID.Equals(pageForumID));
                var moderatorUserNames = new List<string>();

                moderatorsFiltered.ForEach(
                    moderator =>
                        {
                            if (moderator.IsGroup)
                            {
                                moderatorUserNames.AddRange(
                                    RoleMembershipHelper.GetUsersInRole(moderator.Name).Select(u => u.UserName));
                            }
                            else
                            {
                                moderatorUserNames.Add(moderator.Name);
                            }
                        });

                var currentContext = HttpContext.Current;

                // send each message...
                moderatorUserNames.Distinct().AsParallel().ForAll(
                    userName =>
                        {
                            HttpContext.Current = currentContext;

                            try
                            {
                                // add each member of the group
                                var membershipUser = UserMembershipHelper.GetUserByName(userName);
                                var userId =
                                    UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.Id);

                                var languageFile = UserHelper.GetUserLanguageFile(userId);

                                var subject = string.Format(
                                    this.Get<ILocalization>().GetText(
                                        "COMMON",
                                        "NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE",
                                        languageFile),
                                    this.BoardSettings.Name);

                                var notifyModerators = new TemplateEmail("NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE")
                                                           {
                                                               // get the user localization...
                                                               TemplateLanguageFile = languageFile,
                                                               TemplateParams =
                                                                   {
                                                                       ["{user}"] = userName,
                                                                       ["{reason}"] = reportText,
                                                                       ["{reporter}"] =
                                                                           this.Get<IUserDisplayName>()
                                                                               .GetName(reporter),
                                                                       ["{adminlink}"] = BuildLink.GetLinkNotEscaped(
                                                                           ForumPages.Moderate_ReportedPosts,
                                                                           true,
                                                                           "f={0}",
                                                                           pageForumID)
                                                                   }
                                                           };

                                notifyModerators.SendEmail(
                                    new MailAddress(membershipUser.Email, membershipUser.UserName),
                                    subject);
                            }
                            finally
                            {
                                HttpContext.Current = null;
                            }
                        });
            }
            catch (Exception x)
            {
                // report exception to the forum's event log
                this.Get<ILogger>().Error(
                    x,
                    $"Send Message Report Notification Error for UserID {BoardContext.Current.PageUserID}");
            }
        }

        /// <summary>
        /// Sends notification about new PM in user's inbox.
        /// </summary>
        /// <param name="toUserId">
        /// User supposed to receive notification about new PM.
        /// </param>
        /// <param name="subject">
        /// Subject of PM user is notified about.
        /// </param>
        public void ToPrivateMessageRecipient(int toUserId, [NotNull] string subject)
        {
            try
            {
                // user's PM notification setting
                var privateMessageNotificationEnabled = false;

                // user's email
                var toEMail = string.Empty;

                var toUser = this.GetRepository<User>().GetById(toUserId);

                if (toUser != null)
                {
                    privateMessageNotificationEnabled = toUser.PMNotification;
                    toEMail = toUser.Email;
                }

                if (!privateMessageNotificationEnabled)
                {
                    return;
                }

                // get the PM ID
                var userPMessageId = this.GetRepository<PMessage>().ListAsDataTable(toUserId, null, null).GetFirstRow()
                    .Field<int>("UserPMessageID");

                var languageFile = UserHelper.GetUserLanguageFile(toUserId);

                var displayName = this.Get<IUserDisplayName>().GetName(BoardContext.Current.PageUserID);

                // send this user a PM notification e-mail
                var notificationTemplate = new TemplateEmail("PMNOTIFICATION")
                                               {
                                                   TemplateLanguageFile = languageFile,
                                                   TemplateParams =
                                                       {
                                                           ["{fromuser}"] = displayName,
                                                           ["{link}"] =
                                                               $"{BuildLink.GetLinkNotEscaped(ForumPages.PrivateMessage, true, "pm={0}", userPMessageId)}\r\n\r\n",
                                                           ["{subject}"] = subject,
                                                           ["{username}"] =
                                                               this.BoardSettings.EnableDisplayName
                                                                   ? toUser.DisplayName
                                                                   : toUser.Name
                                                       }
                                               };

                // create notification email subject
                var emailSubject = string.Format(
                    this.Get<ILocalization>().GetText("COMMON", "PM_NOTIFICATION_SUBJECT", languageFile),
                    displayName,
                    this.BoardSettings.Name,
                    subject);

                // send email
                notificationTemplate.SendEmail(new MailAddress(toEMail), emailSubject);
            }
            catch (Exception x)
            {
                // report exception to the forum's event log
                this.Get<ILogger>().Error(x, $"Send PM Notification Error for UserID {BoardContext.Current.PageUserID}");

                // tell user about failure
                BoardContext.Current.AddLoadMessage(
                    this.Get<ILocalization>().GetTextFormatted("Failed", x.Message),
                    MessageTypes.danger);
            }
        }

        /// <summary>
        /// The to watching users.
        /// </summary>
        /// <param name="newMessageId">
        /// The new message id.
        /// </param>
        public void ToWatchingUsers(int newMessageId)
        {
            var mailMessages = new List<MailMessage>();
            var boardName = this.BoardSettings.Name;
            var forumEmail = this.BoardSettings.ForumEmail;

            var message = this.GetRepository<Message>().MessageList(newMessageId).FirstOrDefault();

            var messageAuthorUserID = message.UserID ?? 0;

            // cleaned body as text...
            var bodyText = this.Get<IBadWordReplace>()
                .Replace(BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(message.Message))))
                .RemoveMultipleWhitespace();

            var watchUsers = this.GetRepository<User>()
                .WatchMailListAsDataTable(message.TopicID ?? 0, messageAuthorUserID);

            var watchEmail = new TemplateEmail("TOPICPOST")
                                 {
                                     TemplateParams =
                                         {
                                             ["{topic}"] =
                                                 HttpUtility.HtmlDecode(
                                                     this.Get<IBadWordReplace>().Replace(message.Topic)),
                                             ["{postedby}"] =
                                                 UserMembershipHelper.GetDisplayNameFromID(messageAuthorUserID),
                                             ["{body}"] = bodyText,
                                             ["{bodytruncated}"] = bodyText.Truncate(160),
                                             ["{link}"] = BuildLink.GetLinkNotEscaped(
                                                 ForumPages.Posts,
                                                 true,
                                                 "m={0}#post{0}",
                                                 newMessageId),
                                             ["{subscriptionlink}"] = BuildLink.GetLinkNotEscaped(
                                                 ForumPages.Profile_Subscriptions,
                                                 true)
                                         }
                                 };

            var currentContext = HttpContext.Current;

            watchUsers.Rows.Cast<DataRow>().AsParallel().ForAll(
                row =>
                    {
                        HttpContext.Current = currentContext;

                        try
                        {
                            var languageFile =
                                row.Field<string>("LanguageFile").IsSet() && this.Get<BoardSettings>().AllowUserLanguage
                                    ? row.Field<string>("LanguageFile")
                                    : this.Get<BoardSettings>().Language;

                            var subject = string.Format(
                                this.Get<ILocalization>().GetText("COMMON", "TOPIC_NOTIFICATION_SUBJECT", languageFile),
                                boardName);

                            watchEmail.TemplateLanguageFile = languageFile;
                            mailMessages.Add(watchEmail.CreateEmail(
                                new MailAddress(forumEmail, boardName),
                                new MailAddress(
                                    row.Field<string>("Email"),
                                    this.BoardSettings.EnableDisplayName
                                        ? row.Field<string>("DisplayName")
                                        : row.Field<string>("Name")),
                                subject));
                        }
                        finally
                        {
                            HttpContext.Current = null;
                        }
                    });

            if (this.BoardSettings.AllowNotificationAllPostsAllTopics)
            {
                var usersWithAll = this.GetRepository<User>().FindUserTyped(
                    false,
                    notificationType: UserNotificationSetting.AllTopics.ToInt());

                // create individual watch emails for all users who have All Posts on...
                usersWithAll.Where(x => x.ID != messageAuthorUserID && x.ProviderUserKey != null).AsParallel().ForAll(
                    user =>
                    {
                        HttpContext.Current = currentContext;

                        try
                        {
                            if (user.Email.IsNotSet())
                            {
                                return;
                            }

                            var languageFile = user.LanguageFile.IsSet() && this.Get<BoardSettings>().AllowUserLanguage
                                ? user.LanguageFile
                                : this.Get<BoardSettings>().Language;

                            var subject = string.Format(
                                this.Get<ILocalization>().GetText("COMMON", "TOPIC_NOTIFICATION_SUBJECT", languageFile),
                                boardName);

                            watchEmail.TemplateLanguageFile = languageFile;

                            mailMessages.Add(
                                watchEmail.CreateEmail(
                                    new MailAddress(forumEmail, boardName),
                                    new MailAddress(
                                        user.Email,
                                        this.BoardSettings.EnableDisplayName ? user.DisplayName : user.Name),
                                    subject));
                        }
                        finally
                        {
                            HttpContext.Current = null;
                        }
                    });
            }

            if (mailMessages.Any())
            {
                // Now send all mails..
                this.Get<ISendMail>().SendAll(
                    mailMessages,
                    (mailMessage, exception) => this.Get<ILogger>().Log(
                        "Mail Error",
                        EventLogTypes.Error,
                        "SYSTEM",
                        null,
                        exception));
            }
        }

        /// <summary>
        /// Send an Email to the Newly Created User with
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
        /// <param name="templateName">
        /// The template Name.
        /// </param>
        public void SendRegistrationNotificationToUser(
            [NotNull] ApplicationUser user,
            [NotNull] string pass,
            [NotNull] string securityAnswer,
            string templateName)
        {
            var subject = this.Get<ILocalization>().GetTextFormatted(
                "NOTIFICATION_ON_NEW_FACEBOOK_USER_SUBJECT",
                this.BoardSettings.Name);

            var notifyUser = new TemplateEmail(templateName)
                                 {
                                     TemplateParams =
                                         {
                                             ["{user}"] = user.UserName,
                                             ["{email}"] = user.Email,
                                             ["{pass}"] = pass,
                                             ["{answer}"] = securityAnswer
                                         }
                                 };

            notifyUser.SendEmail(new MailAddress(user.Email, user.UserName), subject);
        }

        /// <summary>
        /// Sends notification that the User was awarded with a Medal
        /// </summary>
        /// <param name="toUserId">To user id.</param>
        /// <param name="medalName">Name of the medal.</param>
        public void ToUserWithNewMedal([NotNull] int toUserId, [NotNull] string medalName)
        {
            var userList = this.GetRepository<User>().UserList(
                BoardContext.Current.PageBoardID,
                toUserId,
                true,
                null,
                null,
                null).ToList();

            TypedUserList toUser;

            if (userList.Any())
            {
                toUser = userList.First();
            }
            else
            {
                return;
            }

            var languageFile = UserHelper.GetUserLanguageFile(toUser.UserID.ToType<int>());

            var subject = string.Format(
                this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_MEDAL_AWARDED_SUBJECT", languageFile),
                this.BoardSettings.Name);

            var notifyUser = new TemplateEmail("NOTIFICATION_ON_MEDAL_AWARDED")
                                 {
                                     TemplateLanguageFile = languageFile,
                                     TemplateParams =
                                         {
                                             ["{user}"] =
                                                 this.BoardSettings.EnableDisplayName
                                                     ? toUser.DisplayName
                                                     : toUser.Name,
                                             ["{medalname}"] = medalName
                                         }
                                 };

            notifyUser.SendEmail(
                new MailAddress(toUser.Email, this.BoardSettings.EnableDisplayName ? toUser.DisplayName : toUser.Name),
                subject);
        }

        /// <summary>
        /// Sends the role un assignment notification.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="removedRoles">The removed roles.</param>
        public void SendRoleUnAssignmentNotification([NotNull] ApplicationUser user, List<string> removedRoles)
        {
            var subject = this.Get<ILocalization>().GetTextFormatted(
                "NOTIFICATION_ROLE_ASSIGNMENT_SUBJECT",
                this.BoardSettings.Name);

            var templateEmail = new TemplateEmail("NOTIFICATION_ROLE_UNASSIGNMENT")
                                    {
                                        TemplateParams =
                                            {
                                                ["{user}"] = user.UserName,
                                                ["{roles}"] = string.Join(", ", removedRoles.ToArray()),
                                                ["{forumname}"] = this.BoardSettings.Name,
                                                ["{forumurl}"] = BoardInfo.ForumURL
                                            }
                                    };

            templateEmail.SendEmail(new MailAddress(user.Email, user.UserName), subject);
        }

        /// <summary>
        /// Sends the role assignment notification.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="addedRoles">The added roles.</param>
        public void SendRoleAssignmentNotification([NotNull] ApplicationUser user, List<string> addedRoles)
        {
            var subject = this.Get<ILocalization>().GetTextFormatted(
                "NOTIFICATION_ROLE_ASSIGNMENT_SUBJECT",
                this.BoardSettings.Name);

            var templateEmail = new TemplateEmail("NOTIFICATION_ROLE_ASSIGNMENT")
                                    {
                                        TemplateParams =
                                            {
                                                ["{user}"] = user.UserName,
                                                ["{roles}"] = string.Join(", ", addedRoles.ToArray()),
                                                ["{forumname}"] = this.BoardSettings.Name,
                                                ["{forumurl}"] = BoardInfo.ForumURL
                                            }
                                    };

            templateEmail.SendEmail(new MailAddress(user.Email, user.UserName), subject);
        }

        /// <summary>
        /// Sends a new user notification email to all emails in the NotificationOnUserRegisterEmailList
        /// Setting
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userId">The user id.</param>
        public void SendRegistrationNotificationEmail([NotNull] ApplicationUser user, int userId)
        {
            if (this.BoardSettings.NotificationOnUserRegisterEmailList.IsNotSet())
            {
                return;
            }

            var emails = this.BoardSettings.NotificationOnUserRegisterEmailList.Split(';');

            var subject = string.Format(
                this.Get<ILocalization>().GetText(
                    "COMMON",
                    "NOTIFICATION_ON_USER_REGISTER_EMAIL_SUBJECT",
                    this.BoardSettings.Language),
                this.BoardSettings.Name);

            var notifyAdmin = new TemplateEmail("NOTIFICATION_ON_USER_REGISTER")
                                  {
                                      TemplateLanguageFile = this.BoardSettings.Language,
                                      TemplateParams =
                                          {
                                              ["{adminlink}"] = BuildLink.GetLinkNotEscaped(
                                                  ForumPages.Admin_EditUser,
                                                  true,
                                                  "u={0}",
                                                  userId),
                                              ["{user}"] = user.UserName,
                                              ["{email}"] = user.Email
                                          }
                                  };

            emails.Where(email => email.Trim().IsSet())
                .ForEach(email => notifyAdmin.SendEmail(new MailAddress(email.Trim()), subject));
        }

        /// <summary>
        /// Sends a spam bot notification to admins.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userId">The user id.</param>
        public void SendSpamBotNotificationToAdmins([NotNull] ApplicationUser user, int userId)
        {
            // Get Admin Group ID
            var adminGroupId = this.GetRepository<Group>().List(boardId: BoardContext.Current.PageBoardID)
                .Where(group => group.Name.Contains("Admin")).Select(group => group.ID).FirstOrDefault();

            if (adminGroupId <= 0)
            {
                return;
            }

            using (var dt = this.GetRepository<User>().EmailsAsDataTable(BoardContext.Current.PageBoardID, adminGroupId))
            {
                dt.Rows.Cast<DataRow>().ForEach(
                    row =>
                        {
                            var emailAddress = row.Field<string>("Email");

                            if (emailAddress.IsNotSet())
                            {
                                return;
                            }

                            var subject = this.Get<ILocalization>().GetTextFormatted(
                                "COMMON",
                                "NOTIFICATION_ON_BOT_USER_REGISTER_EMAIL_SUBJECT",
                                this.BoardSettings.Name);

                            var notifyAdmin = new TemplateEmail("NOTIFICATION_ON_BOT_USER_REGISTER")
                                                  {
                                                      TemplateParams =
                                                          {
                                                              ["{adminlink}"] = BuildLink.GetLinkNotEscaped(
                                                                  ForumPages.Admin_EditUser,
                                                                  true,
                                                                  "u={0}",
                                                                  userId),
                                                              ["{user}"] = user.UserName,
                                                              ["{email}"] = user.Email
                                                          }
                                                  };

                            notifyAdmin.SendEmail(new MailAddress(emailAddress), subject);
                        });
            }
        }

        /// <summary>
        /// Sends the user welcome notification.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userId">The user identifier.</param>
        public void SendUserWelcomeNotification([NotNull] ApplicationUser user, int userId)
        {
            if (this.BoardSettings.SendWelcomeNotificationAfterRegister.Equals(0))
            {
                return;
            }

            var subject = this.Get<ILocalization>().GetTextFormatted(
                "NOTIFICATION_ON_WELCOME_USER_SUBJECT",
                this.BoardSettings.Name);

            var notifyUser = new TemplateEmail("NOTIFICATION_ON_WELCOME_USER")
                                 {
                                     TemplateParams = { ["{user}"] = user.UserName }
                                 };

            var emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_WELCOME_USER_TEXT");

            var messageFlags = new MessageFlags { IsHtml = false, IsBBCode = true };

            if (this.BoardSettings.AllowPrivateMessages
                && this.BoardSettings.SendWelcomeNotificationAfterRegister.Equals(2))
            {
                var users = this.GetRepository<User>().UserList(
                    BoardContext.Current.PageBoardID,
                    null,
                    true,
                    null,
                    null,
                    null).ToList();

                var hostUser = users.FirstOrDefault(u => u.IsHostAdmin > 0);

                BoardContext.Current.GetRepository<PMessage>().SendMessage(
                    hostUser.UserID.Value,
                    userId,
                    subject,
                    emailBody,
                    messageFlags.BitValue,
                    -1);
            }
            else
            {
                notifyUser.SendEmail(new MailAddress(user.Email, user.UserName), subject);
            }
        }

        /// <summary>
        /// Sends the verification email.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="email">The email.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="newUsername">The new username.</param>
        public void SendVerificationEmail(
            [NotNull] ApplicationUser user,
            [NotNull] string email,
            int? userId,
            string newUsername = null)
        {
            CodeContracts.VerifyNotNull(email, "email");
            CodeContracts.VerifyNotNull(user, "user");

            var code = HttpUtility.UrlEncode(
                BoardContext.Current.Get<IApplicationUserManager>().GenerateEmailConfirmationResetToken(user.Id));

            // save verification record...
            this.GetRepository<CheckEmail>().Save(userId, code, user.Email);

            var subject = this.Get<ILocalization>().GetTextFormatted(
                "VERIFICATION_EMAIL_SUBJECT",
                this.BoardSettings.Name);

            var verifyEmail = new TemplateEmail("VERIFYEMAIL")
                                  {
                                      TemplateParams =
                                          {
                                              ["{link}"] =
                                                  BuildLink.GetLinkNotEscaped(ForumPages.Account_Approve, true, "code={0}", code),
                                              ["{key}"] = code,
                                              ["{username}"] = user.UserName
                                          }
                                  };

            verifyEmail.SendEmail(new MailAddress(email, newUsername ?? user.UserName), subject);
        }

        /// <summary>
        /// Sends the user a suspension notification.
        /// </summary>
        /// <param name="suspendedUntil">The suspended until.</param>
        /// <param name="suspendReason">The suspend reason.</param>
        /// <param name="email">The email.</param>
        /// <param name="userName">Name of the user.</param>
        public void SendUserSuspensionNotification(
            [NotNull] System.DateTime suspendedUntil,
            [NotNull] string suspendReason,
            [NotNull] string email,
            [NotNull] string userName)
        {
            var subject = this.Get<ILocalization>().GetTextFormatted(
                "NOTIFICATION_ON_SUSPENDING_USER_SUBJECT",
                this.BoardSettings.Name);

            var notifyUser = new TemplateEmail("NOTIFICATION_ON_SUSPENDING_USER")
                                 {
                                     TemplateParams =
                                         {
                                             ["{user}"] = userName,
                                             ["{suspendReason}"] = suspendReason,
                                             ["{suspendedUntil}"] =
                                                 suspendedUntil.ToString(CultureInfo.InvariantCulture),
                                             ["{forumname}"] = this.BoardSettings.Name,
                                             ["{forumurl}"] = BoardInfo.ForumURL
                                         }
                                 };

            notifyUser.SendEmail(new MailAddress(email, userName), subject);
        }

        /// <summary>
        /// Sends the user a suspension notification.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="userName">Name of the user.</param>
        public void SendUserSuspensionEndedNotification([NotNull] string email, [NotNull] string userName)
        {
            var subject = this.Get<ILocalization>().GetTextFormatted(
                "NOTIFICATION_ON_SUSPENDING_USER_SUBJECT",
                this.BoardSettings.Name);

            var notifyUser = new TemplateEmail("NOTIFICATION_ON_SUSPENDING_ENDED_USER")
                                 {
                                     TemplateParams =
                                         {
                                             ["{user}"] = userName,
                                             ["{forumname}"] = this.BoardSettings.Name,
                                             ["{forumurl}"] = BoardInfo.ForumURL
                                         }
                                 };

            notifyUser.SendEmail(new MailAddress(email, userName), subject);
        }

        /// <summary>
        /// The send password reset.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        public void SendPasswordReset([NotNull] ApplicationUser user, [NotNull] string code)
        {
            // re-send verification email instead of lost password...
            var verifyEmail = new TemplateEmail("RESET_PASS");

            var subject = this.Get<ILocalization>().GetTextFormatted(
                "RESET_PASS_EMAIL_SUBJECT",
                this.Get<BoardSettings>().Name);

            verifyEmail.TemplateParams["{link}"] = BuildLink.GetLinkNotEscaped(
                ForumPages.Account_ResetPassword,
                true,
                "code={0}",
                code);
            verifyEmail.TemplateParams["{forumname}"] = this.Get<BoardSettings>().Name;
            verifyEmail.TemplateParams["{forumlink}"] = $"{BoardInfo.ForumURL}";

            verifyEmail.SendEmail(new MailAddress(user.Email, user.UserName), subject);
        }

        #endregion

        #endregion
    }
}