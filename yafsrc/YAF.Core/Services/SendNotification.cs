﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

using YAF.Core.Model;
using YAF.Types.Constants;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The YAF Send Notification.
/// </summary>
public class SendNotification : ISendNotification, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SendNotification"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public SendNotification(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

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

    /// <summary>
    /// Sends Notifications to Moderators that Message Needs Approval
    /// </summary>
    /// <param name="forumId">The forum id.</param>
    /// <param name="newMessageId">The new message id.</param>
    /// <param name="isSpamMessage">if set to <c>true</c> [is spam message].</param>
    public void ToModeratorsThatMessageNeedsApproval(
        int forumId,
        int newMessageId,
        bool isSpamMessage)
    {
        var moderatorsFiltered = this.Get<DataBroker>().GetModerators().Where(f => f.ForumID.Equals(forumId));
        var moderatorUserNames = new List<string>();

        moderatorsFiltered.ForEach(
            moderator =>
                {
                    if (moderator.IsGroup)
                    {
                        moderatorUserNames.AddRange(
                            this.Get<IAspNetRolesHelper>().GetUsersInRole(moderator.Name).Select(u => u.UserName));
                    }
                    else
                    {
                        moderatorUserNames.Add(moderator.Name);
                    }
                });

        var inlineCss = File.ReadAllText(
            this.Get<HttpContextBase>().Server
                .MapPath(this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")));

        var forumLink = this.Get<LinkBuilder>().ForumUrl;

        var adminLink = this.Get<LinkBuilder>().GetAbsoluteLink(
            ForumPages.Moderate_UnapprovedPosts,
            new { f = forumId });

        var currentContext = HttpContext.Current;

        // send each message...
        moderatorUserNames.Distinct().AsParallel().ForAll(
            userName =>
                {
                    HttpContext.Current = currentContext;

                    try
                    {
                        // add each member of the group
                        var user = this.GetRepository<User>().GetSingle(x => x.Name == userName);

                        if (user == null)
                        {
                            return;
                        }

                        var languageFile = UserHelper.GetUserLanguageFile(user);

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
                                            ["{css}"] = inlineCss,
                                            ["{forumlink}"] = forumLink,
                                            ["{title}"] = subject
                                        }
                                };

                        notifyModerators.SendEmail(new MailAddress(user.Email, user.DisplayOrUserName()), subject);
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
                this.Get<DataBroker>().GetModerators().Where(f => f.ForumID.Equals(pageForumID));
            var moderatorUserNames = new List<string>();

            moderatorsFiltered.ForEach(
                moderator =>
                    {
                        if (moderator.IsGroup)
                        {
                            moderatorUserNames.AddRange(
                                this.Get<IAspNetRolesHelper>().GetUsersInRole(moderator.Name)
                                    .Select(u => u.UserName));
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
                            var membershipUser = this.Get<IAspNetUsersHelper>().GetUserByName(userName);
                            var user = this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(membershipUser.Id);

                            var languageFile = UserHelper.GetUserLanguageFile(user);

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
                                                                           .GetNameById(reporter),
                                                                   ["{adminlink}"] = this.Get<LinkBuilder>()
                                                                       .GetAbsoluteLink(
                                                                           ForumPages.Moderate_ReportedPosts,
                                                                           new {f = pageForumID})
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
            this.Get<ILoggerService>().Error(
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
    public void ToPrivateMessageRecipient(int toUserId, string subject)
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
            var userPMessageId = this.GetRepository<UserPMessage>().Get(p => p.UserID == toUserId)
                .OrderByDescending(p => p.ID).FirstOrDefault().ID;

            var languageFile = UserHelper.GetUserLanguageFile(toUser);

            var displayName = BoardContext.Current.PageUser.DisplayOrUserName();

            // send this user a PM notification e-mail
            var notificationTemplate = new TemplateEmail("PMNOTIFICATION")
                                           {
                                               TemplateLanguageFile = languageFile,
                                               TemplateParams =
                                                   {
                                                       ["{fromuser}"] = displayName,
                                                       ["{link}"] =
                                                           $"{this.Get<LinkBuilder>().GetAbsoluteLink(ForumPages.PrivateMessage, new {pm = userPMessageId})}\r\n\r\n",
                                                       ["{subject}"] = subject,
                                                       ["{username}"] = toUser.DisplayOrUserName()
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
            this.Get<ILoggerService>().Error(
                x,
                $"Send PM Notification Error for UserID {BoardContext.Current.PageUserID}");

            // tell user about failure
            BoardContext.Current.Notify(
                this.Get<ILocalization>().GetTextFormatted("Failed", x.Message),
                MessageTypes.danger);
        }
    }

    /// <summary>
    /// The to watching users.
    /// </summary>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    public void ToWatchingUsers(int messageId)
    {
        var message = this.GetRepository<Message>().GetMessage(messageId);

        this.Get<ISendNotification>().ToWatchingUsers(message);
    }

    /// <summary>
    /// The to watching users.
    /// </summary>
    /// <param name="message">
    /// The new message.
    /// </param>
    /// <param name="newTopic">
    /// Indicates if Post is New Topic or reply
    /// </param>
    public void ToWatchingUsers(Message message, bool newTopic = false)
    {
        var mailMessages = new List<MailMessage>();
        var boardName = this.BoardSettings.Name;
        var forumEmail = this.BoardSettings.ForumEmail;

        // cleaned body as text...
        var bodyText = this.Get<IBadWordReplace>()
            .Replace(
                BBCodeHelper.StripBBCode(HtmlTagHelper.StripHtml(HtmlTagHelper.CleanHtmlString(message.MessageText))))
            .RemoveMultipleWhitespace();

        var watchUsers = this.GetRepository<User>().WatchMailList(message.TopicID, message.UserID);

        var watchEmail = new TemplateEmail("TOPICPOST")
                             {
                                 TemplateParams =
                                     {
                                         ["{topic}"] =
                                             HttpUtility.HtmlDecode(
                                                 this.Get<IBadWordReplace>().Replace(message.Topic.TopicName)),
                                         ["{postedby}"] =
                                             this.Get<IUserDisplayName>().GetNameById(message.UserID),
                                         ["{body}"] = bodyText,
                                         ["{bodytruncated}"] = bodyText.Truncate(160),
                                         ["{link}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
                                             ForumPages.Posts,
                                             new {m = message.ID, name = message.Topic.TopicName}),
                                         ["{subscriptionlink}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
                                             ForumPages.Profile_Subscriptions)
                                     }
                             };

        var currentContext = HttpContext.Current;

        watchUsers.AsParallel().ForAll(
            user =>
                {
                    HttpContext.Current = currentContext;

                    try
                    {
                        // Add to stream
                        if (user.Activity)
                        {
                            if (newTopic)
                            {
                                this.Get<IActivityStream>().AddWatchTopicToStream(
                                    user.ID,
                                    message.TopicID,
                                    message.ID,
                                    message.Topic.TopicName,
                                    message.MessageText,
                                    message.UserID);
                            }
                            else
                            {
                                this.Get<IActivityStream>().AddWatchReplyToStream(
                                    user.ID,
                                    message.TopicID,
                                    message.ID,
                                    message.Topic.TopicName,
                                    message.MessageText,
                                    message.UserID);
                            }
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

        if (mailMessages.Any())
        {
            // Now send all mails..
            this.Get<IMailService>().SendAll(mailMessages);
        }
    }

    /// <summary>
    /// Send an Email to the Newly Created User with
    /// his Account Info (Pass)
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="pass">
    /// The pass.
    /// </param>
    /// <param name="templateName">
    /// The template Name.
    /// </param>
    public void SendRegistrationNotificationToUser(
        AspNetUsers user,
        string pass,
        string templateName)
    {
        var subject = this.Get<ILocalization>().GetTextFormatted(
            "NOTIFICATION_ON_NEW_FACEBOOK_USER_SUBJECT",
            this.BoardSettings.Name);

        var notifyUser = new TemplateEmail(templateName)
                             {
                                 TemplateParams =
                                     {
                                         ["{user}"] = user.UserName, ["{email}"] = user.Email, ["{pass}"] = pass
                                     }
                             };

        notifyUser.SendEmail(new MailAddress(user.Email, user.UserName), subject);
    }

    /// <summary>
    /// Sends notification that the User was awarded with a Medal
    /// </summary>
    /// <param name="toUserId">To user id.</param>
    /// <param name="medalName">Name of the medal.</param>
    public void ToUserWithNewMedal(int toUserId, string medalName)
    {
        var toUser = this.GetRepository<User>().GetById(toUserId);

        if (toUser == null)
        {
            return;
        }

        var languageFile = UserHelper.GetUserLanguageFile(toUser);

        var subject = string.Format(
            this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_MEDAL_AWARDED_SUBJECT", languageFile),
            this.BoardSettings.Name);

        var notifyUser = new TemplateEmail("NOTIFICATION_ON_MEDAL_AWARDED")
                             {
                                 TemplateLanguageFile = languageFile,
                                 TemplateParams =
                                     {
                                         ["{user}"] = toUser.DisplayOrUserName(), ["{medalname}"] = medalName
                                     }
                             };

        notifyUser.SendEmail(new MailAddress(toUser.Email, toUser.DisplayOrUserName()), subject);
    }

    /// <summary>
    /// Sends the role un assignment notification.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="removedRoles">The removed roles.</param>
    public void SendRoleUnAssignmentNotification(AspNetUsers user, List<string> removedRoles)
    {
        var subject = this.Get<ILocalization>().GetTextFormatted(
            "NOTIFICATION_ROLE_ASSIGNMENT_SUBJECT",
            this.BoardSettings.Name);

        var templateEmail = new TemplateEmail("NOTIFICATION_ROLE_UNASSIGNMENT")
                                {
                                    TemplateParams =
                                        {
                                            ["{user}"] = user.UserName,
                                            ["{roles}"] = string.Join(", ", removedRoles),
                                            ["{forumname}"] = this.BoardSettings.Name
                                        }
                                };

        templateEmail.SendEmail(new MailAddress(user.Email, user.UserName), subject);
    }

    /// <summary>
    /// Sends the role assignment notification.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="addedRoles">The added roles.</param>
    public void SendRoleAssignmentNotification(AspNetUsers user, List<string> addedRoles)
    {
        var subject = this.Get<ILocalization>().GetTextFormatted(
            "NOTIFICATION_ROLE_ASSIGNMENT_SUBJECT",
            this.BoardSettings.Name);

        var templateEmail = new TemplateEmail("NOTIFICATION_ROLE_ASSIGNMENT")
                                {
                                    TemplateParams =
                                        {
                                            ["{user}"] = user.UserName,
                                            ["{roles}"] = string.Join(", ", addedRoles),
                                            ["{forumname}"] = this.BoardSettings.Name
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
    public void SendRegistrationNotificationEmail(AspNetUsers user, int userId)
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
                                          ["{adminlink}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
                                              ForumPages.Admin_EditUser,
                                              new {u = userId}),
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
    public void SendSpamBotNotificationToAdmins(AspNetUsers user, int userId)
    {
        // Get Admin Group ID
        var adminGroupId = this.GetRepository<Group>().List(boardId: BoardContext.Current.PageBoardID)
            .Where(group => group.Name.Contains("Admin")).Select(group => group.ID).FirstOrDefault();

        if (adminGroupId <= 0)
        {
            return;
        }

        var emails = this.GetRepository<User>().GroupEmails(adminGroupId);

        emails.ForEach(
            email =>
                {
                    var emailAddress = email;

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
                                                      ["{adminlink}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
                                                          ForumPages.Admin_EditUser,
                                                          new {u = userId}),
                                                      ["{user}"] = user.UserName,
                                                      ["{email}"] = user.Email
                                                  }
                                          };

                    notifyAdmin.SendEmail(new MailAddress(emailAddress), subject);
                });
    }

    /// <summary>
    /// Sends the user welcome notification.
    /// </summary>
    /// <param name="user">The user.</param>
    public void SendUserWelcomeNotification(User user)
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
                                 TemplateParams =
                                     {
                                         ["{user}"] = user.DisplayOrUserName()
                                     }
                             };

        var emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_WELCOME_USER_TEXT");

        var messageFlags = new MessageFlags
                               {
                                   IsHtml = false, IsBBCode = true
                               };

        if (this.BoardSettings.AllowPrivateMessages
            && this.BoardSettings.SendWelcomeNotificationAfterRegister.Equals(2))
        {
            var hostUser = this.GetRepository<User>()
                .Get(u => u.BoardID == BoardContext.Current.PageBoardID && (u.Flags & 1) == 1)
                .FirstOrDefault();

            if (hostUser != null)
            {
                BoardContext.Current.GetRepository<PMessage>().SendMessage(
                    hostUser.ID,
                    user.ID,
                    subject,
                    emailBody,
                    messageFlags.BitValue,
                    -1);
            }
        }
        else
        {
            notifyUser.SendEmail(new MailAddress(user.Email, user.DisplayOrUserName()), subject);
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
        AspNetUsers user,
        string email,
        int? userId,
        string newUsername = null)
    {
        var token = HttpUtility.UrlEncode(
            this.Get<IAspNetUsersHelper>().GenerateEmailConfirmationResetToken(user.Id));

        // save verification record...
        this.GetRepository<CheckEmail>().Save(userId, token, user.Email);

        var subject = this.Get<ILocalization>().GetTextFormatted(
            "VERIFICATION_EMAIL_SUBJECT",
            this.BoardSettings.Name);

        var verifyEmail = new TemplateEmail("VERIFYEMAIL")
                              {
                                  TemplateParams =
                                      {
                                          ["{link}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
                                              ForumPages.Account_Approve,
                                              new {code = token}),
                                          ["{key}"] = token,
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
        DateTime suspendedUntil,
        string suspendReason,
        string email,
        string userName)
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
                                         ["{forumname}"] = this.BoardSettings.Name
                                     }
                             };

        notifyUser.SendEmail(new MailAddress(email, userName), subject);
    }

    /// <summary>
    /// Sends the user a suspension notification.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="userName">Name of the user.</param>
    public void SendUserSuspensionEndedNotification(string email, string userName)
    {
        var subject = this.Get<ILocalization>().GetTextFormatted(
            "NOTIFICATION_ON_SUSPENDING_USER_SUBJECT",
            this.BoardSettings.Name);

        var notifyUser = new TemplateEmail("NOTIFICATION_ON_SUSPENDING_ENDED_USER")
                             {
                                 TemplateParams =
                                     {
                                         ["{user}"] = userName, ["{forumname}"] = this.BoardSettings.Name
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
    /// <param name="token">
    /// The token.
    /// </param>
    public void SendPasswordReset(AspNetUsers user, string token)
    {
        // re-send verification email instead of lost password...
        var verifyEmail = new TemplateEmail("RESET_PASS");

        var subject = this.Get<ILocalization>().GetTextFormatted(
            "RESET_PASS_EMAIL_SUBJECT",
            this.Get<BoardSettings>().Name);

        verifyEmail.TemplateParams["{link}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
            ForumPages.Account_ResetPassword,
            new { code = token });
        verifyEmail.TemplateParams["{forumname}"] = this.Get<BoardSettings>().Name;
        verifyEmail.TemplateParams["{forumlink}"] = $"{this.Get<LinkBuilder>().ForumUrl}";

        verifyEmail.SendEmail(new MailAddress(user.Email, user.UserName), subject);
    }
}