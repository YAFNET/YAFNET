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

using YAF.Types.Models;

/// <summary>
/// The moderation rights of the current user.
/// </summary>
public static class ModerationHelper
{
    /// <summary>
    /// Checks if the current user is an admin of the board of the forum or a moderator of the forum
    /// (e.g. for the target forum of a move, the id is sent by the client).
    /// </summary>
    /// <param name="forumId">The forum id.</param>
    /// <returns>Returns if the user can moderate the forum.</returns>
    public static bool CanModerateForum(int forumId)
    {
        var context = BoardContext.Current;

        var forum = context.GetRepository<Forum>().GetById(forumId);

        if (forum == null || !context.GetRepository<Category>()
                .Exists(c => c.ID == forum.CategoryID && c.BoardID == context.PageBoardID))
        {
            return false;
        }

        if (context.IsAdmin)
        {
            return true;
        }

        var userId = context.PageUserID;

        return context.GetRepository<VAccess>().Exists(
            v => v.UserID == userId && v.ForumID == forumId && v.ModeratorAccess > 0);
    }

    /// <summary>
    /// Checks if the current user can moderate the forum of the topic.
    /// </summary>
    /// <param name="topicId">The topic id.</param>
    /// <returns>Returns if the user can moderate the topic.</returns>
    public static bool CanModerateTopic(int topicId)
    {
        var topic = BoardContext.Current.GetRepository<Topic>().GetById(topicId);

        return topic != null && CanModerateForum(topic.ForumID);
    }
}
