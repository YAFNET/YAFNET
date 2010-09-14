/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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

namespace YAF.Classes.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Net.Mail;
  using System.Web;

  using YAF.Classes.Data;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The yaf send notification.
  /// </summary>
  public class YafSendNotification
  {
    #region Public Methods

    /// <summary>
    /// The to moderators that message needs approval.
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="newMessageId">
    /// The new message id.
    /// </param>
    public void ToModeratorsThatMessageNeedsApproval(int forumId, int newMessageId)
    {
      var moderatorsFiltered = YafServices.DBBroker.GetAllModerators().Where(f => f.ForumID.Equals(forumId));
      var moderatorUserNames = new List<string>();

      foreach (var moderator in moderatorsFiltered)
      {
        if (moderator.IsGroup)
        {
          moderatorUserNames.AddRange(YafContext.Current.CurrentRoles.GetUsersInRole(moderator.Name));
        }
        else
        {
          moderatorUserNames.Add(moderator.Name);
        }
      }

      var notifyModerators = new YafTemplateEmail("NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL");

      string subject =
        YafContext.Current.Localization.GetText("COMMON", "NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL").FormatWith(
          YafContext.Current.BoardSettings.Name);

      notifyModerators.TemplateParams["{adminlink}"] =
        YafBuildLink.GetLinkNotEscaped(ForumPages.moderate_unapprovedposts, true, "f={0}", forumId);
      notifyModerators.TemplateParams["{forumname}"] = YafContext.Current.BoardSettings.Name;

      // send each message...
      foreach (var userName in moderatorUserNames.Distinct())
      {
        // add each member of the group
        var membershipUser = UserMembershipHelper.GetUser(userName);
        var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey);

        // get the user localization...
        notifyModerators.TemplateLanguageFile = UserHelper.GetUserLanguageFile(userId);
        notifyModerators.SendEmail(new MailAddress(membershipUser.Email, membershipUser.UserName), subject, true);
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
        bool privateMessageNotificationEnabled;

        // user's email
        string toEMail;

        // read user's info from DB
        using (DataTable dt = DB.user_list(YafContext.Current.PageBoardID, toUserId, true))
        {
          privateMessageNotificationEnabled = (bool)dt.Rows[0]["PMNotification"];
          toEMail = (string)dt.Rows[0]["EMail"];
        }

        if (privateMessageNotificationEnabled)
        {
          // user has PM notification set on
          int userPMessageId;

          // string senderEmail;

          // get the PM ID
          // Ederon : 11/21/2007 - PageBoardID as parameter of DB.pmessage_list?
          // using (DataTable dt = DB.pmessage_list(toUserID, PageContext.PageBoardID, null))
          using (DataTable dt = DB.pmessage_list(toUserId, null, null))
          {
            userPMessageId = (int)dt.Rows[0]["UserPMessageID"];
          }

          // get the sender e-mail -- DISABLED: too much information...
          // using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
          // senderEmail = ( string ) dt.Rows [0] ["Email"];

          // send this user a PM notification e-mail
          var notificationTemplate = new YafTemplateEmail("PMNOTIFICATION")
            {
               TemplateLanguageFile = UserHelper.GetUserLanguageFile(toUserId) 
            };

          // fill the template with relevant info
          notificationTemplate.TemplateParams["{fromuser}"] = YafContext.Current.PageUserName;
          notificationTemplate.TemplateParams["{link}"] =
            "{0}\r\n\r\n".FormatWith(
              YafBuildLink.GetLinkNotEscaped(ForumPages.cp_message, true, "pm={0}", userPMessageId));
          notificationTemplate.TemplateParams["{forumname}"] = YafContext.Current.BoardSettings.Name;
          notificationTemplate.TemplateParams["{subject}"] = subject;

          // create notification email subject
          string emailSubject =
            YafContext.Current.Localization.GetText("COMMON", "PM_NOTIFICATION_SUBJECT").FormatWith(
              YafContext.Current.PageUserName, YafContext.Current.BoardSettings.Name, subject);

          // send email
          notificationTemplate.SendEmail(new MailAddress(toEMail), emailSubject, true);
        }
      }
      catch (Exception x)
      {
        // report exception to the forum's event log
        DB.eventlog_create(YafContext.Current.PageUserID, "SendPmNotification", x);

        // tell user about failure
        YafContext.Current.AddLoadMessage(YafContext.Current.Localization.GetTextFormatted("Failed", x.Message));
      }
    }

    /// <summary>
    /// The to watching users.
    /// </summary>
    /// <param name="newMessageId">
    /// The new message id.
    /// </param>
    public void ToWatchingUsers(long newMessageId)
    {
      IEnumerable<TypedUserFind> usersWithAll = new List<TypedUserFind>();

      if (YafContext.Current.BoardSettings.AllowNotificationAllPostsAllTopics)
      {
        // TODO: validate permissions!
        usersWithAll = DB.UserFind(
          YafContext.Current.PageBoardID, false, null, null, null, UserNotificationSetting.AllTopics.ToInt(), null);
      }

      using (DataTable dt = DB.message_list(newMessageId))
      {
        foreach (DataRow row in dt.Rows)
        {
          var userId = row["UserID"].ToType<int>();

          var watchEmail = new YafTemplateEmail("TOPICPOST");
          watchEmail.TemplateLanguageFile = UserHelper.GetUserLanguageFile(userId);

          // cleaned body as text...
          string bodyText =
            StringHelper.RemoveMultipleWhitespace(
              BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(row["Message"].ToString()))));

          // Send track mails
          string subject =
            YafContext.Current.Localization.GetText("COMMON", "TOPIC_NOTIFICATION_SUBJECT").FormatWith(
              YafContext.Current.BoardSettings.Name);

          watchEmail.TemplateParams["{forumname}"] = YafContext.Current.BoardSettings.Name;
          watchEmail.TemplateParams["{topic}"] = HttpUtility.HtmlDecode(row["Topic"].ToString());
          watchEmail.TemplateParams["{postedby}"] =
            UserMembershipHelper.GetDisplayNameFromID(row["UserID"].ToType<long>());
          watchEmail.TemplateParams["{body}"] = bodyText;
          watchEmail.TemplateParams["{bodytruncated}"] = StringHelper.Truncate(bodyText, 160);
          watchEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(
            ForumPages.posts, true, "m={0}#post{0}", newMessageId);

          watchEmail.CreateWatch(
            row["TopicID"].ToType<int>(), 
            userId, 
            new MailAddress(YafContext.Current.BoardSettings.ForumEmail, YafContext.Current.BoardSettings.Name), 
            subject);

          // create individual watch emails for all users who have All Posts on...
          foreach (var user in usersWithAll.Where(x => x.UserID.HasValue && x.UserID.Value != userId))
          {
            var membershipUser = UserMembershipHelper.GetUser(user.ProviderUserKey);

            watchEmail.TemplateLanguageFile = !string.IsNullOrEmpty(user.LanguageFile)
                                                ? user.LanguageFile
                                                : YafContext.Current.Localization.LanguageFileName;
            watchEmail.SendEmail(
              new MailAddress(YafContext.Current.BoardSettings.ForumEmail, YafContext.Current.BoardSettings.Name), 
              new MailAddress(membershipUser.Email, membershipUser.UserName), 
              subject, 
              true);
          }
        }
      }
    }

    #endregion
  }
}