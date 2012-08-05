/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net.Mail;
    using System.Web;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The yaf send notification.
    /// </summary>
    public class YafSendNotification : ISendNotification, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafSendNotification"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public YafSendNotification([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Implemented Interfaces

        #region ISendNotification

        /// <summary>
        /// Sends Notifications to Moderators that Message Needs Approval
        /// </summary>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="newMessageId">
        /// The new message id.
        /// </param>
        public void ToModeratorsThatMessageNeedsApproval(int forumId, int newMessageId)
        {
            var moderatorsFiltered = this.Get<IDBBroker>().GetAllModerators().Where(f => f.ForumID.Equals(forumId));
            var moderatorUserNames = new List<string>();

            foreach (var moderator in moderatorsFiltered)
            {
                if (moderator.IsGroup)
                {
                    moderatorUserNames.AddRange(this.Get<RoleProvider>().GetUsersInRole(moderator.Name));
                }
                else
                {
                    moderatorUserNames.Add(moderator.Name);
                }
            }

            // send each message...
            foreach (var userName in moderatorUserNames.Distinct())
            {
                // add each member of the group
                var membershipUser = UserMembershipHelper.GetUser(userName);
                var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey);

                var languageFile = UserHelper.GetUserLanguageFile(userId);

                var subject =
                    this.Get<ILocalization>().GetText(
                        "COMMON", "NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL", languageFile).FormatWith(this.Get<YafBoardSettings>().Name);

                var notifyModerators = new YafTemplateEmail("NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL")
                                           {
                                               // get the user localization...
                                               TemplateLanguageFile = languageFile
                                           };

                notifyModerators.TemplateParams["{adminlink}"] =
                    YafBuildLink.GetLinkNotEscaped(ForumPages.moderate_unapprovedposts, true, "f={0}", forumId);
                notifyModerators.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;

                notifyModerators.SendEmail(
                    new MailAddress(membershipUser.Email, membershipUser.UserName), subject, true);
            }
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
            int pageForumID, int reportedMessageId, int reporter, string reportText)
        {
            try
            {
                var moderatorsFiltered =
                    this.Get<IDBBroker>().GetAllModerators().Where(f => f.ForumID.Equals(pageForumID));
                var moderatorUserNames = new List<string>();

                foreach (var moderator in moderatorsFiltered)
                {
                    if (moderator.IsGroup)
                    {
                        moderatorUserNames.AddRange(this.Get<RoleProvider>().GetUsersInRole(moderator.Name));
                    }
                    else
                    {
                        moderatorUserNames.Add(moderator.Name);
                    }
                }

                // send each message...
                foreach (var userName in moderatorUserNames.Distinct())
                {
                    // add each member of the group
                    var membershipUser = UserMembershipHelper.GetUser(userName);
                    var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey);

                    var languageFile = UserHelper.GetUserLanguageFile(userId);

                    var subject =
                        this.Get<ILocalization>().GetText(
                            "COMMON",
                            "NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE",
                            languageFile).FormatWith(this.Get<YafBoardSettings>().Name);

                    var notifyModerators = new YafTemplateEmail("NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE")
                                               {
                                                   // get the user localization...
                                                   TemplateLanguageFile = languageFile
                                               };

                    notifyModerators.TemplateParams["{reason}"] = reportText;
                    notifyModerators.TemplateParams["{reporter}"] = this.Get<IUserDisplayName>().GetName(reporter);
                    notifyModerators.TemplateParams["{adminlink}"] =
                        YafBuildLink.GetLinkNotEscaped(ForumPages.moderate_reportedposts, true, "f={0}", pageForumID);
                    notifyModerators.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;

                    notifyModerators.SendEmail(
                        new MailAddress(membershipUser.Email, membershipUser.UserName), subject, true);
                }
            }
            catch (Exception x)
            {
                // report exception to the forum's event log
                LegacyDb.eventlog_create(YafContext.Current.PageUserID, "SendMessageReportNotification", x);
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
                bool privateMessageNotificationEnabled = false;

                // user's email
                var toEMail = string.Empty;

                var userList = LegacyDb.UserList(YafContext.Current.PageBoardID, toUserId, true, null, null, null);

                if (userList.Any())
                {
                    privateMessageNotificationEnabled = userList.First().PMNotification ?? false;
                    toEMail = userList.First().Email;
                }

                if (!privateMessageNotificationEnabled)
                {
                    return;
                }

                // get the PM ID
                // Ederon : 11/21/2007 - PageBoardID as parameter of DB.pmessage_list?
                // using (DataTable dt = DB.pmessage_list(toUserID, PageContext.PageBoardID, null))
                int userPMessageId =
                    LegacyDb.pmessage_list(toUserId, null, null).GetFirstRow().Field<int>("UserPMessageID");

                /*// get the sender e-mail -- DISABLED: too much information...
                    // using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
                    // senderEmail = ( string ) dt.Rows [0] ["Email"];*/

                var languageFile = UserHelper.GetUserLanguageFile(toUserId);

                // send this user a PM notification e-mail
                var notificationTemplate = new YafTemplateEmail("PMNOTIFICATION") { TemplateLanguageFile = languageFile };

                var displayName = this.Get<IUserDisplayName>().GetName(YafContext.Current.PageUserID);

                // fill the template with relevant info
                notificationTemplate.TemplateParams["{fromuser}"] = displayName;

                notificationTemplate.TemplateParams["{link}"] =
                    "{0}\r\n\r\n".FormatWith(
                        YafBuildLink.GetLinkNotEscaped(ForumPages.cp_message, true, "pm={0}", userPMessageId));
                notificationTemplate.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;
                notificationTemplate.TemplateParams["{subject}"] = subject;

                // create notification email subject
                var emailSubject =
                    this.Get<ILocalization>().GetText(
                        "COMMON", "PM_NOTIFICATION_SUBJECT", languageFile).FormatWith(
                            displayName, this.Get<YafBoardSettings>().Name, subject);

                // send email
                notificationTemplate.SendEmail(new MailAddress(toEMail), emailSubject, true);
            }
            catch (Exception x)
            {
                // report exception to the forum's event log
                LegacyDb.eventlog_create(YafContext.Current.PageUserID, "SendPmNotification", x);

                // tell user about failure
                YafContext.Current.AddLoadMessage(
                    this.Get<ILocalization>().GetTextFormatted("Failed", x.Message), MessageTypes.Error);
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
            IEnumerable<TypedUserFind> usersWithAll = new List<TypedUserFind>();

            if (this.Get<YafBoardSettings>().AllowNotificationAllPostsAllTopics)
            {
                // TODO: validate permissions!
                usersWithAll = LegacyDb.UserFind(
                    YafContext.Current.PageBoardID,
                    false,
                    null,
                    null,
                    null,
                    UserNotificationSetting.AllTopics.ToInt(),
                    null);
            }

            // TODO : Rewrite Watch Topic code to allow watch mails in the users language, as workaround send all messages in the default board language
            var languageFile = this.Get<YafBoardSettings>().Language;

            foreach (var message in LegacyDb.MessageList(newMessageId))
            {
                int userId = message.UserID ?? 0;

                var watchEmail = new YafTemplateEmail("TOPICPOST") { TemplateLanguageFile = languageFile };

                // cleaned body as text...
                var bodyText =
                    StringExtensions.RemoveMultipleWhitespace(
                        BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(message.Message))));

                // Send track mails
                var subject =
                    this.Get<ILocalization>().GetText("COMMON", "TOPIC_NOTIFICATION_SUBJECT", languageFile).FormatWith(
                        this.Get<YafBoardSettings>().Name);

                watchEmail.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;
                watchEmail.TemplateParams["{topic}"] = HttpUtility.HtmlDecode(message.Topic);
                watchEmail.TemplateParams["{postedby}"] = UserMembershipHelper.GetDisplayNameFromID(userId);
                watchEmail.TemplateParams["{body}"] = bodyText;
                watchEmail.TemplateParams["{bodytruncated}"] = bodyText.Truncate(160);
                watchEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, true, "m={0}#post{0}", newMessageId);

                watchEmail.CreateWatch(
                    message.TopicID ?? 0,
                    userId,
                    new MailAddress(this.Get<YafBoardSettings>().ForumEmail, this.Get<YafBoardSettings>().Name),
                    subject);

                // create individual watch emails for all users who have All Posts on...
                foreach (var user in usersWithAll.Where(x => x.UserID.HasValue && x.UserID.Value != userId))
                {
                    // Make sure its not a guest
                    if (user.ProviderUserKey == null)
                    {
                        continue;
                    }

                    var membershipUser = UserMembershipHelper.GetUser(user.ProviderUserKey);

                    if (!membershipUser.Email.IsSet())
                    {
                        continue;
                    }

                    watchEmail.TemplateLanguageFile = !string.IsNullOrEmpty(user.LanguageFile)
                                                          ? user.LanguageFile
                                                          : this.Get<ILocalization>().LanguageFileName;
                    watchEmail.SendEmail(
                        new MailAddress(this.Get<YafBoardSettings>().ForumEmail, this.Get<YafBoardSettings>().Name),
                        new MailAddress(membershipUser.Email, membershipUser.UserName),
                        subject,
                        true);
                }
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
            [NotNull] MembershipUser user, [NotNull] string pass, [NotNull] string securityAnswer, string templateName)
        {
            var notifyUser = new YafTemplateEmail();

            var subject =
                YafContext.Current.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_NEW_FACEBOOK_USER_SUBJECT").FormatWith(YafContext.Current.Get<YafBoardSettings>().Name);

            notifyUser.TemplateParams["{user}"] = user.UserName;
            notifyUser.TemplateParams["{email}"] = user.Email;
            notifyUser.TemplateParams["{pass}"] = pass;
            notifyUser.TemplateParams["{answer}"] = securityAnswer;
            notifyUser.TemplateParams["{forumname}"] = YafContext.Current.Get<YafBoardSettings>().Name;

            var emailBody = notifyUser.ProcessTemplate(templateName);

            YafContext.Current.Get<ISendMail>().Queue(
                YafContext.Current.Get<YafBoardSettings>().ForumEmail, user.Email, subject, emailBody);
        }

        /// <summary>
        /// Sends notification that the User was awarded with a Medal
        /// </summary>
        /// <param name="toUserId">To user id.</param>
        /// <param name="medalName">Name of the medal.</param>
        public void ToUserWithNewMedal([NotNull] int toUserId, [NotNull] string medalName)
        {
            var userList = LegacyDb.UserList(YafContext.Current.PageBoardID, toUserId, true, null, null, null);

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

            var notifyUser = new YafTemplateEmail("NOTIFICATION_ON_MEDAL_AWARDED")
                                 { TemplateLanguageFile = languageFile };

            var subject =
                YafContext.Current.Get<ILocalization>().GetText(
                    "COMMON", "NOTIFICATION_ON_MEDAL_AWARDED_SUBJECT", languageFile).FormatWith(
                        YafContext.Current.Get<YafBoardSettings>().Name);

            notifyUser.TemplateParams["{user}"] = YafContext.Current.Get<YafBoardSettings>().EnableDisplayName
                                                      ? toUser.DisplayName
                                                      : toUser.Name;
            notifyUser.TemplateParams["{medalname}"] = medalName;
            notifyUser.TemplateParams["{forumname}"] = YafContext.Current.Get<YafBoardSettings>().Name;

            notifyUser.SendEmail(
                new MailAddress(
                    toUser.Email,
                    YafContext.Current.Get<YafBoardSettings>().EnableDisplayName ? toUser.DisplayName : toUser.Name),
                subject,
                true);
        }

        #endregion

        #endregion
    }
}