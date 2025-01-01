/* Yet Another Forum.NET
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

namespace YAF.Core.Model;

using System;
using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The Activity repository extensions.
/// </summary>
public static class ActivityRepositoryExtensions
{
    /// <summary>
    /// Gets the Users notifications.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// Returns the List of Activity Notifications
    /// </returns>
    public static List<Tuple<Activity, User, Topic>> Notifications(
        this IRepository<Activity> repository,
        int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Activity>();

        expression.Join<User>((a, u) => u.ID == a.FromUserID).LeftJoin<Topic>((a, t) => t.ID == a.TopicID.Value)
            .Where<Activity>(a => a.UserID == userId && a.FromUserID.HasValue).OrderByDescending(a => a.Created);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Activity, User, Topic>(expression));
    }

    /// <summary>
    /// Gets the User Timeline.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// Returns the User Timeline
    /// </returns>
    public static List<Tuple<Activity, Topic>> Timeline(this IRepository<Activity> repository, int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Activity>();

        expression.LeftJoin<Topic>((a, t) => t.ID == a.TopicID).Where<Activity>(
            a => a.UserID == userId && (a.Flags & 1024) != 1024 && (a.Flags & 4096) != 4096
                 && (a.Flags & 8192) != 8192 && (a.Flags & 16384) != 16384).OrderByDescending(a => a.Created);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Activity, Topic>(expression));
    }

    /// <summary>
    /// The update notification.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    public static void UpdateNotification(
        this IRepository<Activity> repository,
        int userId,
        int messageId)
    {
        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));

        repository.UpdateOnly(
            () => new Activity { Notification = false },
            a => a.UserID == userId && a.MessageID == messageId && a.Notification);
    }

    /// <summary>
    /// Sets all Notifications as read for the topic
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="topicId">
    /// The message id.
    /// </param>
    public static void UpdateTopicNotification(
        this IRepository<Activity> repository,
        int userId,
        int topicId)
    {
        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));

        repository.UpdateOnly(
            () => new Activity { Notification = false },
            a => a.UserID == userId && a.TopicID == topicId && a.Notification);
    }

    /// <summary>
    /// Mark all Notifications as read for the user
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    public static void MarkAllAsRead(this IRepository<Activity> repository, int userId)
    {
        repository.UpdateOnly(
            () => new Activity { Notification = false },
            a => a.UserID == userId && a.Notification);
    }
}