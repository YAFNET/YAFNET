/* YetAnotherForum.NET
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
using System;
using System.Web.Services;

using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

/// <summary>
/// Summary description for YafForumWebService
/// </summary>
[WebService(Namespace = "http://yetanotherforum.net/services")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class YafWebService : WebService
{
  /// <summary>
  /// The create new topic.
  /// </summary>
  /// <param name="token">
  /// The token.
  /// </param>
  /// <param name="forumid">
  /// The forumid.
  /// </param>
  /// <param name="userid">
  /// The userid.
  /// </param>
  /// <param name="username">
  /// The username.
  /// </param>
  /// <param name="subject">
  /// The subject.
  /// </param>
  /// <param name="post">
  /// The post.
  /// </param>
  /// <param name="ip">
  /// The ip.
  /// </param>
  /// <param name="priority">
  /// The priority.
  /// </param>
  /// <param name="flags">
  /// The flags.
  /// </param>
  /// <returns>
  /// The create new topic.
  /// </returns>
  /// <exception cref="Exception">
  /// </exception>
  [WebMethod]
  public long CreateNewTopic(string token, int forumid, int userid, string username, string subject, string post, string ip, int priority, int flags)
  {
    // validate token...
    if (token != YafContext.Current.BoardSettings.WebServiceToken)
    {
      throw new Exception("Invalid Secure Web Service Token: Operation Failed");
    }

    long messageId = 0;
    string subjectEncoded = Server.HtmlEncode(subject);

    return DB.topic_save(forumid, subjectEncoded, post, userid, priority, username, ip, null, null, flags, ref messageId);
  }

  /// <exception cref="Exception"><c>Exception</c>.</exception>
  [WebMethod]
  public bool SetDisplayNameFromUsername(string token, string username, string displayName)
  {
    // validate token...
    if (token != YafContext.Current.BoardSettings.WebServiceToken)
    {
      throw new Exception("Invalid Secure Web Service Token: Operation Failed");
    }

    // get the user id...
    var membershipUser = UserMembershipHelper.GetMembershipUserByName(username);

    if (membershipUser != null)
    {
      var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey);

      var displayNameId = YafContext.Current.UserDisplayName.GetId(displayName);

      if (displayNameId.HasValue && displayNameId.Value != userId)
      {
        // problem...
        throw new Exception(
          "Display Name must be unique. {0} display name already exists in the database.".FormatWith(displayName));
      }

      var userFields = DB.user_list(Config.BoardID, userId, null).Rows[0];

      DB.user_save(
        userId,
        Config.BoardID,
        null,
        displayName,
        null,
        userFields["TimeZone"],
         userFields["LanguageFile"],
         userFields["Culture"],
         userFields["ThemeFile"],
        null,
        null,
        null,
        null,
        null,
        null,
        null);

      UserMembershipHelper.ClearCacheForUserId(userId);

      return true;
    }

    return false;
  }
}
