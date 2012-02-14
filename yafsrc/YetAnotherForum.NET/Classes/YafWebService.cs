﻿/* YetAnotherForum.NET
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
#region Using

using System;
using System.Web.Services;

using YAF.Classes;
using YAF.Classes.Data;
using YAF.Core;
using YAF.Types;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Utils;

#endregion

/// <summary>
/// Summary description for YafForumWebService
/// </summary>
[WebService(Namespace = "http://yetanotherforum.net/services")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class YafWebService : WebService, IHaveServiceLocator
{
    /// <summary>
    /// Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator
    {
        get
        {
            return YafContext.Current.ServiceLocator;
        }
    }

    #region Public Methods

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
    /// <param name="status">
    /// The status.
    /// </param>
    /// <param name="styles">
    /// The styles.
    /// </param>
    /// <param name="description">
    /// The description.
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
    /// <exception cref="SecurityFailureInvalidWebServiceTokenException">
    /// Invalid Secure Web Service Token: Operation Failed
    /// </exception>
    /// <exception cref="SecurityFailureInvalidWebServiceTokenException">Invalid Secure Web Service Token: Operation Failed</exception>
    [WebMethod]
    public long CreateNewTopic(
        [NotNull] string token,
        int forumid,
        int userid,
        [NotNull] string username,
        [CanBeNull] string status,
        [CanBeNull] string styles,
        [CanBeNull] string description,
        [NotNull] string subject,
        [NotNull] string post,
        [NotNull] string ip,
        int priority,
        int flags)
    {
        // validate token...
        if (token != YafContext.Current.Get<YafBoardSettings>().WebServiceToken)
        {
            throw new SecurityFailureInvalidWebServiceTokenException(
                "Invalid Secure Web Service Token: Operation Failed");
        }

        long messageId = 0;
        string subjectEncoded = this.Server.HtmlEncode(subject);

        return LegacyDb.topic_save(
            forumid,
            subjectEncoded,
            status,
            styles,
            description,
            post,
            userid,
            priority,
            username,
            ip,
            DateTime.UtcNow,
            null,
            flags,
            ref messageId);
    }

    /// <summary>
    /// The set display name from username.
    /// </summary>
    /// <param name="token">
    /// The token.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="displayName">
    /// The display Name.
    /// </param>
    /// <exception cref="Exception">
    /// <c>Exception</c>.
    /// </exception>
    /// <returns>
    /// The set display name from username.
    /// </returns>
    /// <exception cref="NonUniqueDisplayNameException">
    /// <c>NonUniqueDisplayNameException</c>.
    /// </exception>
    /// <exception cref="SecurityFailureInvalidWebServiceTokenException">Invalid Secure Web Service Token: Operation Failed</exception>
    [WebMethod]
    public bool SetDisplayNameFromUsername(
        [NotNull] string token, [NotNull] string username, [NotNull] string displayName)
    {
        // validate token...
        if (token != YafContext.Current.Get<YafBoardSettings>().WebServiceToken)
        {
            throw new SecurityFailureInvalidWebServiceTokenException(
                "Invalid Secure Web Service Token: Operation Failed");
        }

        // get the user id...
        var membershipUser = UserMembershipHelper.GetMembershipUserByName(username);

        if (membershipUser != null)
        {
            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey);

            var displayNameId = this.Get<IUserDisplayName>().GetId(displayName);

            if (displayNameId.HasValue && displayNameId.Value != userId)
            {
                // problem...
                throw new NonUniqueDisplayNameException(
                    "Display Name must be unique. {0} display name already exists in the database.".FormatWith(
                        displayName));
            }

            var userFields = LegacyDb.user_list(Config.BoardID, userId, null).Rows[0];

            LegacyDb.user_save(
                userId,
                Config.BoardID,
                null,
                displayName,
                null,
                userFields["TimeZone"],
                userFields["LanguageFile"],
                userFields["Culture"],
                userFields["ThemeFile"],
                userFields["UseSingleSignOn"],
                userFields["TextEditor"],
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));

            return true;
        }

        return false;
    }

    #endregion
}

/// <summary>
/// The security failure invalid web service token exception.
/// </summary>
public class SecurityFailureInvalidWebServiceTokenException : Exception
{
  #region Constructors and Destructors

  /// <summary>
  /// Initializes a new instance of the <see cref="SecurityFailureInvalidWebServiceTokenException"/> class.
  /// </summary>
  /// <param name="message">
  /// The message.
  /// </param>
  public SecurityFailureInvalidWebServiceTokenException([NotNull] string message)
    : base(message)
  {
  }

  #endregion
}

/// <summary>
/// The non unique display name exception.
/// </summary>
public class NonUniqueDisplayNameException : Exception
{
  #region Constructors and Destructors

  /// <summary>
  /// Initializes a new instance of the <see cref="NonUniqueDisplayNameException"/> class.
  /// </summary>
  /// <param name="message">
  /// The message.
  /// </param>
  public NonUniqueDisplayNameException([NotNull] string message)
    : base(message)
  {
  }

  #endregion
}