/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;

    /// <summary>
    /// The Activity repository extensions.
    /// </summary>
    public static class ActivityRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets The Activity Stream
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Activity> List(
            this IRepository<Activity> repository,
            int userId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Get(a => a.UserID == userId).OrderByDescending(a => a.Created).ToList();
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
            CodeContracts.VerifyNotNull(repository, "repository");

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
            CodeContracts.VerifyNotNull(repository, "repository");

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
        public static void MarkAllAsRead(
            this IRepository<Activity> repository,
            int userId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(
                () => new Activity { Notification = false },
                a => a.UserID == userId && a.Notification);
        }

        #endregion
    }
}