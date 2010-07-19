namespace YAF.Classes.Core
{
  using System.Collections.Generic;
  using System.Data;
  using System.Data.DataSetExtensions;
  using System.Linq;
  using System.Net.Mail;

  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  public class YafSendNotification
  {
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

      string subject = YafContext.Current.Localization.GetText("COMMON", "NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL").FormatWith(YafContext.Current.BoardSettings.Name);

      notifyModerators.TemplateParams["{adminlink}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.moderate_unapprovedposts, true, "f={0}", forumId);
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
  }
}