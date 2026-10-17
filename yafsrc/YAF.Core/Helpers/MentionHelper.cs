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

namespace YAF.Core.Helpers;

using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The notifications of the users mentioned and quoted in a post.
/// </summary>
public static class MentionHelper
{
    /// <summary>
    /// Adds the notifications of the users mentioned (@[userlink]name[/userlink]) and quoted
    /// ([quote=name;id]) in the message text. The names are typed by the user (a guest, a deleted user,
    /// another display name), only existing users who can read the forum are notified, not the author.
    /// </summary>
    /// <param name="messageText">The message text.</param>
    /// <param name="forumId">The forum identifier.</param>
    /// <param name="topicId">The topic identifier.</param>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="authorId">The user identifier of the author.</param>
    public static void NotifyMentionedAndQuotedUsers(
        string messageText,
        int forumId,
        int topicId,
        int messageId,
        int authorId)
    {
        if (messageText.IsNotSet())
        {
            return;
        }

        var activityStream = BoardContext.Current.Get<IActivityStream>();

        foreach (var name in BBCodeHelper.FindMentions(messageText))
        {
            var userId = FindUserToNotify(name, forumId, authorId);

            if (userId.HasValue)
            {
                activityStream.AddMentionToStream(userId.Value, topicId, messageId, authorId);
            }
        }

        foreach (var name in BBCodeHelper.FindUserQuoting(messageText))
        {
            var userId = FindUserToNotify(name, forumId, authorId);

            if (userId.HasValue)
            {
                activityStream.AddQuotingToStream(userId.Value, topicId, messageId, authorId);
            }
        }
    }

    /// <summary>
    /// Finds the user to notify.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="forumId">The forum identifier.</param>
    /// <param name="authorId">The user identifier of the author.</param>
    /// <returns>The user identifier or null.</returns>
    private static int? FindUserToNotify(string name, int forumId, int authorId)
    {
        var user = BoardContext.Current.Get<IUserDisplayName>().FindUserByName(name);

        if (user is null || user.UserFlags.IsGuest || user.ID == authorId)
        {
            return null;
        }

        return BoardContext.Current.GetRepository<VAccess>().Exists(
                   v => v.UserID == user.ID && v.ForumID == forumId && v.ReadAccess > 0)
                   ? user.ID
                   : null;
    }
}
