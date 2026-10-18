/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
using System.Threading.Tasks;
using System.Web;

using Microsoft.Extensions.Logging;

using MimeKit;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

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
    public async Task ToModeratorsThatMessageNeedsApprovalAsync(int forumId, int newMessageId, bool isSpamMessage)
    {
        // the moderators by id (the names are only unique per board)
        var moderatorIds = (await this.Get<DataBroker>().GetModeratorsAsync()).Where(f => f.ForumID.Equals(forumId))
            .Select(moderator => moderator.ModeratorID).Distinct().ToList();

        var forumLink = this.Get<ILinkBuilder>().ForumUrl;

        var adminLink = this.Get<ILinkBuilder>().GetAbsoluteLink(ForumPages.Moderate_UnapprovedPosts, new {f = forumId});

        // send each message...
        foreach (var moderatorId in moderatorIds)
        {
            var user = await this.GetRepository<User>().GetByIdAsync(moderatorId);

            if (user == null || user.Email.IsNotSet())
            {
                continue;
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
                                ["{user}"] = user.Name,
                                ["{adminlink}"] = adminLink,
                                ["{forumlink}"] = forumLink
                            }
                    };

            await this.SendNotificationMailAsync(
                () => notifyModerators.SendEmailAsync(new MailboxAddress(user.DisplayOrUserName(), user.Email), subject),
                "Approval");
        }
    }

    /// <summary>
    /// Sends Notifications to Moderators that a Message was Reported
    /// </summary>
    /// <param name="pageForumId">
    ///     The page Forum ID.
    /// </param>
    /// <param name="reportedMessageId">
    ///     The reported message id.
    /// </param>
    /// <param name="reporter">
    ///     The reporter.
    /// </param>
    /// <param name="reportText">
    ///     The report Text.
    /// </param>
    public async Task ToModeratorsThatMessageWasReportedAsync(
        int pageForumId,
        int reportedMessageId,
        int reporter,
        string reportText)
    {
        try
        {
            // the moderators by id (the names are only unique per board)
            var moderatorIds = (await this.Get<DataBroker>().GetModeratorsAsync())
                .Where(f => f.ForumID.Equals(pageForumId)).Select(moderator => moderator.ModeratorID).Distinct().ToList();

            // send each message...
            foreach (var moderatorId in moderatorIds)
            {
                var user = await this.GetRepository<User>().GetByIdAsync(moderatorId);

                if (user == null || user.Email.IsNotSet())
                {
                    continue;
                }

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
                                                               ["{user}"] = user.Name,
                                                               ["{reason}"] = reportText,
                                                               ["{reporter}"] =
                                                                   this.Get<IUserDisplayName>().GetNameById(reporter),
                                                               ["{adminlink}"] = this.Get<ILinkBuilder>()
                                                                   .GetAbsoluteLink(
                                                                       ForumPages.Moderate_ReportedPosts,
                                                                       new {f = pageForumId})
                                                           }
                };

                await notifyModerators.SendEmailAsync(
                    new MailboxAddress(user.DisplayOrUserName(), user.Email),
                    subject);
            }
        }
        catch (Exception x)
        {
            // report exception to the forum's event log
            this.Get<ILogger<SendNotification>>().Error(
                x,
                $"Send Message Report Notification Error for UserID {BoardContext.Current.PageUserID}");
        }
    }

    /// <summary>
    /// The to watching users.
    /// </summary>
    /// <param name="messageId">
    ///     The message Id.
    /// </param>
    public Task ToWatchingUsersAsync(int messageId)
    {
        var message = this.GetRepository<Message>().GetMessage(messageId);

        return this.Get<ISendNotification>().ToWatchingUsersAsync(message);
    }

    /// <summary>
    /// The to watching users.
    /// </summary>
    /// <param name="message">
    ///     The new message.
    /// </param>
    /// <param name="newTopic">
    ///     Indicates if Post is New Topic or reply
    /// </param>
    /// The to watching users.
    public async Task ToWatchingUsersAsync(Message message, bool newTopic = false)
    {
        var mailMessages = new List<MimeMessage>();
        var boardName = this.BoardSettings.Name;
        var forumEmail = this.BoardSettings.ForumEmail;

        // cleaned body as text, without hidden content (hide, hide-reply, hide-thanks, ...): it is only shown in the forum
        var bodyText = this.Get<IBadWordReplace>()
            .Replace(
                BBCodeHelper.StripBBCode(
                    HtmlTagHelper.StripHtml(
                        HtmlTagHelper.CleanHtmlString(
                            this.Get<IFormatMessage>().RemoveHiddenBBCodeContent(message.MessageText)))))
            .RemoveMultipleWhitespace();

        var watchUsers = await this.GetRepository<User>().WatchMailListAsync(message.TopicID, message.UserID);

        // the mail contains the post, only watchers who (still) can read the forum get it
        var forumId = message.Topic.ForumID;

        var readers = (await this.GetRepository<VAccess>().GetAsync(v => v.ForumID == forumId && v.ReadAccess > 0))
            .Select(v => v.UserID).ToHashSet();

        watchUsers.RemoveAll(u => !readers.Contains(u.ID));

        var topicLink = this.Get<ILinkBuilder>().GetAbsoluteLink(
            ForumPages.Post,
            new { m = message.ID, name = message.Topic.TopicName });

        var watchEmail = new TemplateEmail("TOPICPOST")
                             {
                                 TemplateParams =
                                     {
                                         ["{topic}"] =
                                             HttpUtility.HtmlDecode(
                                                 this.Get<IBadWordReplace>().Replace(message.Topic.TopicName)),
                                         ["{postedby}"] = this.Get<IUserDisplayName>().GetNameById(message.UserID),
                                         ["{body}"] = bodyText,
                                         ["{bodytruncated}"] = bodyText.Truncate(160),
                                         ["{link}"] = topicLink,
                                         ["{subscriptionlink}"] = this.Get<ILinkBuilder>().GetAbsoluteLink(
                                             ForumPages.Profile_Subscriptions)
                                     }
                             };

        // sequential: the template email is shared and its language is set per user
        foreach (var user in watchUsers)
        {
            // Add to stream
            if (user.Activity)
            {
                if (newTopic)
                {
                    this.Get<IActivityStream>().AddWatchTopicToStream(
                        user,
                        message.TopicID,
                        message.ID,
                        message.Topic.TopicName,
                        message.MessageText,
                        message.UserID);
                }
                else
                {
                    this.Get<IActivityStream>().AddWatchReplyToStream(
                        user,
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

            // Send push Notifications
            if (this.Get<VapidConfiguration>().IsPwaEnabled())
            {
                await this.Get<ISendPushNotification>()
                    .SendTopicPushNotificationAsync(user, message, newTopic, subject, topicLink, languageFile);
            }

            watchEmail.TemplateLanguageFile = languageFile;

            var fromAddress = MailboxAddress.Parse(forumEmail);
            fromAddress.Name = boardName;

            var toAddress = MailboxAddress.Parse(user.Email);
            toAddress.Name = this.BoardSettings.EnableDisplayName ? user.DisplayName : user.Name;

            mailMessages.Add(await watchEmail.CreateEmailAsync(fromAddress, toAddress, subject));
        }

        if (mailMessages.HasItems())
        {
            // Now send all mails
            await this.SendNotificationMailAsync(() => this.Get<IMailService>().SendAllAsync(mailMessages), "Watch");
        }
    }

    /// <summary>
    /// Send email to the Newly Created PageUser with
    /// his Account Info (Pass)
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="pass">
    ///     The pass.
    /// </param>
    /// <param name="templateName">
    ///     The template Name.
    /// </param>
    public Task SendRegistrationNotificationToUserAsync(
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

        return notifyUser.SendEmailAsync(new MailboxAddress(user.UserName, user.Email), subject);
    }

    /// <summary>
    /// Sends notification that the PageUser was awarded with a Medal
    /// </summary>
    /// <param name="toUserId">To user id.</param>
    /// <param name="medalName">Name of the medal.</param>
    public async Task ToUserWithNewMedalAsync(int toUserId, string medalName)
    {
        var toUser = await this.GetRepository<User>().GetByIdAsync(toUserId);

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
                                 TemplateParams = {["{user}"] = toUser.DisplayOrUserName(), ["{medalname}"] = medalName}
                             };

        await this.SendNotificationMailAsync(
            () => notifyUser.SendEmailAsync(new MailboxAddress(toUser.DisplayOrUserName(), toUser.Email), subject),
            "Medal");
    }

    /// <summary>
    /// Sends the role un assignment notification.
    /// </summary>
    /// <param name="user">The user (the mail is in the language of the user).</param>
    /// <param name="removedRoles">The removed roles.</param>
    public Task SendRoleUnAssignmentNotificationAsync(User user, List<string> removedRoles)
    {
        var languageFile = UserHelper.GetUserLanguageFile(user);

        var subject = string.Format(
            this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ROLE_ASSIGNMENT_SUBJECT", languageFile),
            this.BoardSettings.Name);

        var templateEmail = new TemplateEmail("NOTIFICATION_ROLE_UNASSIGNMENT")
                                {
                                    TemplateLanguageFile = languageFile,
                                    TemplateParams =
                                        {
                                            ["{user}"] = user.Name,
                                            ["{roles}"] = removedRoles.ToDelimitedString(","),
                                            ["{forumname}"] = this.BoardSettings.Name
                                        }
                                };

        return this.SendNotificationMailAsync(
            () => templateEmail.SendEmailAsync(new MailboxAddress(user.Name, user.Email), subject),
            "Role");
    }

    /// <summary>
    /// Sends the role assignment notification.
    /// </summary>
    /// <param name="user">The user (the mail is in the language of the user).</param>
    /// <param name="addedRoles">The added roles.</param>
    public Task SendRoleAssignmentNotificationAsync(User user, List<string> addedRoles)
    {
        var languageFile = UserHelper.GetUserLanguageFile(user);

        var subject = string.Format(
            this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ROLE_ASSIGNMENT_SUBJECT", languageFile),
            this.BoardSettings.Name);

        var templateEmail = new TemplateEmail("NOTIFICATION_ROLE_ASSIGNMENT")
                                {
                                    TemplateLanguageFile = languageFile,
                                    TemplateParams =
                                        {
                                            ["{user}"] = user.Name,
                                            ["{roles}"] = addedRoles.ToDelimitedString(","),
                                            ["{forumname}"] = this.BoardSettings.Name
                                        }
                                };

        return this.SendNotificationMailAsync(
            () => templateEmail.SendEmailAsync(new MailboxAddress(user.Name, user.Email), subject),
            "Role");
    }

    /// <summary>
    /// Sends a new user notification email to all emails in the NotificationOnUserRegisterEmailList
    /// Setting
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="userId">The user id.</param>
    public async Task SendRegistrationNotificationEmailAsync(AspNetUsers user, int userId)
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
                                          ["{adminlink}"] = this.Get<ILinkBuilder>().GetAbsoluteLink(
                                              ForumPages.Admin_EditUser,
                                              new {u = userId}),
                                          ["{user}"] = user.UserName,
                                          ["{email}"] = user.Email
                                      }
                              };

        foreach (var email in emails.Where(email => email.Trim().IsSet()))
        {
            await this.SendNotificationMailAsync(
                () => notifyAdmin.SendEmailAsync(MailboxAddress.Parse(email.Trim()), subject),
                "Registration");
        }
    }

    /// <summary>
    /// Sends a spam bot notification to admins.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="userId">The user id.</param>
    public async Task SendSpamBotNotificationToAdminsAsync(AspNetUsers user, int userId)
    {
        // the admin groups of the board (by the admin flag, the name can be changed)
        var adminGroupIds = this.GetRepository<Group>().List(boardId: BoardContext.Current.PageBoardID)
            .Where(group => group.GroupFlags.IsAdmin).Select(group => group.ID).ToList();

        var emails = new List<string>();

        foreach (var adminGroupId in adminGroupIds)
        {
            emails.AddRange(await this.GetRepository<User>().GroupEmailsAsync(adminGroupId));
        }

        emails = [.. emails.Distinct(StringComparer.OrdinalIgnoreCase)];

        foreach (var email in emails)
        {
            if (email.IsNotSet())
            {
                continue;
            }

            // the text of the page COMMON (the subject was the missing text of the tag COMMON)
            var subject = string.Format(
                this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_BOT_USER_REGISTER_EMAIL_SUBJECT"),
                this.BoardSettings.Name);

            var notifyAdmin = new TemplateEmail("NOTIFICATION_ON_BOT_USER_REGISTER")
                                  {
                                      TemplateParams =
                                          {
                                              ["{adminlink}"] = this.Get<ILinkBuilder>().GetAbsoluteLink(
                                                  ForumPages.Admin_EditUser,
                                                  new {u = userId}),
                                              ["{user}"] = user.UserName,
                                              ["{email}"] = user.Email
                                          }
                                  };

            await this.SendNotificationMailAsync(
                () => notifyAdmin.SendEmailAsync(MailboxAddress.Parse(email), subject),
                "Spam Bot");
        }
    }

    /// <summary>
    /// Sends the user welcome notification.
    /// </summary>
    /// <param name="user">The user.</param>
    public async Task SendUserWelcomeNotificationAsync(User user)
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
                                 TemplateParams = {["{user}"] = user.DisplayOrUserName()}
                             };

        var emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_WELCOME_USER_TEXT");

        if (this.BoardSettings.AllowPrivateMessages
            && this.BoardSettings.SendWelcomeNotificationAfterRegister.Equals(2))
        {
            var hostUsers = await this.GetRepository<User>()
                .GetAsync(u => u.BoardID == BoardContext.Current.PageBoardID && (u.Flags & 1) == 1);

                var hostUser = hostUsers.FirstOrDefault();

                if (hostUser != null)
                {
                    await this.GetRepository<PrivateMessage>().InsertAsync(
                        new PrivateMessage
                            {
                                Created = DateTime.UtcNow,
                                Flags = 0,
                                FromUserId = hostUser.ID,
                                ToUserId = user.ID,
                                Body = emailBody
                            });
                }
        }
        else
        {
            await this.SendNotificationMailAsync(
                () => notifyUser.SendEmailAsync(new MailboxAddress(user.DisplayOrUserName(), user.Email), subject),
                "Welcome");
        }
    }

    /// <summary>
    /// Sends the verification email.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="email">The email.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="newUsername">The new username.</param>
    public async Task SendVerificationEmailAsync(
        AspNetUsers user,
        string email,
        int userId,
        string newUsername = null)
    {
        var token = HttpUtility.UrlEncode(
            await this.Get<IAspNetUsersHelper>().GenerateEmailConfirmationResetTokenAsync(user),
            Encoding.UTF8);

        // save verification record...
        await this.GetRepository<CheckEmail>().SaveAsync(userId, token, user.Email);

        var subject = this.Get<ILocalization>().GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", this.BoardSettings.Name);

        var verifyEmail = new TemplateEmail("VERIFYEMAIL")
                              {
                                  TemplateParams =
                                      {
                                          ["{link}"] =
                                              this.Get<ILinkBuilder>().GetAbsoluteLink(
                                                  ForumPages.Account_Approve,
                                                  new {code = token}),
                                          ["{key}"] = token,
                                          ["{username}"] = user.UserName
                                      }
                              };

        await verifyEmail.SendEmailAsync(new MailboxAddress(newUsername ?? user.UserName, email), subject);
    }

    /// <summary>
    /// Sends the user a suspension notification.
    /// </summary>
    /// <param name="suspendedUntil">The suspended until.</param>
    /// <param name="suspendReason">The suspend reason.</param>
    /// <param name="user">The user (the mail is in the language of the user).</param>
    public Task SendUserSuspensionNotificationAsync(
        DateTime suspendedUntil,
        string suspendReason,
        User user)
    {
        var languageFile = UserHelper.GetUserLanguageFile(user);
        var userName = user.DisplayOrUserName();

        var subject = string.Format(
            this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_SUSPENDING_USER_SUBJECT", languageFile),
            this.BoardSettings.Name);

        var notifyUser = new TemplateEmail("NOTIFICATION_ON_SUSPENDING_USER")
                             {
                                 TemplateLanguageFile = languageFile,
                                 TemplateParams =
                                     {
                                         ["{user}"] = userName,
                                         ["{suspendReason}"] = suspendReason,
                                         ["{suspendedUntil}"] = suspendedUntil.ToString(CultureInfo.InvariantCulture),
                                         ["{forumname}"] = this.BoardSettings.Name
                                     }
                             };

        return this.SendNotificationMailAsync(
            () => notifyUser.SendEmailAsync(new MailboxAddress(userName, user.Email), subject),
            "Suspension");
    }

    /// <summary>
    /// Sends the user a suspension notification.
    /// </summary>
    /// <param name="user">The user (the mail is in the language of the user).</param>
    public Task SendUserSuspensionEndedNotificationAsync(User user)
    {
        var languageFile = UserHelper.GetUserLanguageFile(user);
        var userName = user.DisplayOrUserName();

        var subject = string.Format(
            this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_SUSPENDING_USER_SUBJECT", languageFile),
            this.BoardSettings.Name);

        var notifyUser = new TemplateEmail("NOTIFICATION_ON_SUSPENDING_ENDED_USER")
                             {
                                 TemplateLanguageFile = languageFile,
                                 TemplateParams = {["{user}"] = userName, ["{forumname}"] = this.BoardSettings.Name}
                             };

        return this.SendNotificationMailAsync(
            () => notifyUser.SendEmailAsync(new MailboxAddress(userName, user.Email), subject),
            "Suspension");
    }

    /// <summary>
    /// The send password reset.
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="token">
    ///     The token.
    /// </param>
    public Task SendPasswordResetAsync(AspNetUsers user, string token)
    {
        // re-send verification email instead of lost password...
        var verifyEmail = new TemplateEmail("RESET_PASS");

        var subject = this.Get<ILocalization>().GetTextFormatted(
            "RESET_PASS_EMAIL_SUBJECT",
            this.Get<BoardSettings>().Name);

        verifyEmail.TemplateParams["{link}"] = this.Get<ILinkBuilder>().GetAbsoluteLink(
            ForumPages.Account_ResetPassword,
            new {code = token});

        verifyEmail.TemplateParams["{forumname}"] = this.Get<BoardSettings>().Name;
        verifyEmail.TemplateParams["{forumlink}"] = $"{this.Get<ILinkBuilder>().ForumUrl}";

        return verifyEmail.SendEmailAsync(new MailboxAddress(user.UserName, user.Email), subject);
    }

    /// <summary>
    /// Sends a notification mail: a mail error (e.g. the mail server is down) is logged, it does not fail
    /// the action the notification is sent for (the post, the role change, ...).
    /// </summary>
    /// <param name="send">Sends the mail.</param>
    /// <param name="notification">The name of the notification (for the log).</param>
    private async Task SendNotificationMailAsync(Func<Task> send, string notification)
    {
        try
        {
            await send();
        }
        catch (Exception x)
        {
            this.Get<ILogger<SendNotification>>().Error(x, $"{notification} Notification Error");
        }
    }
}
