/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
    using YAF.Types.Objects.Model;

    #endregion

    /// <summary>
    /// YAF Friends service
    /// </summary>
    public class Friends : IFriends, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Friends"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public Friends([NotNull] IServiceLocator serviceLocator)
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

        #region IBuddy

        /// <summary>
        /// Adds a buddy request.
        /// </summary>
        /// <param name="toUserId">
        /// the to user id.
        /// </param>
        /// <returns>
        /// The name of the second user + whether this request is approved or not. (This request
        ///   is approved without the target user's approval if the target user has sent a buddy request
        ///   to current user too or if the current user is already in the target user's buddy list.
        /// </returns>
        public bool AddRequest(int toUserId)
        {
            this.ClearCache(toUserId);

            return this.GetRepository<Buddy>().AddRequest(
                BoardContext.Current.PageUserID,
                toUserId);
        }

        /// <summary>
        /// Approves all buddy requests for the current user.
        /// </summary>
        /// <param name="mutual">
        /// should the users be added to current user's buddy list too?
        /// </param>
        public void ApproveAllRequests(bool mutual)
        {
            var dt = this.ListAll().Where(x => x.Approved && x.UserID == BoardContext.Current.PageUserID);

            dt.ForEach(drv => this.ApproveRequest(drv.FromUserID, mutual));
        }

        /// <summary>
        /// Approves a buddy request.
        /// </summary>
        /// <param name="toUserId">
        /// the to user id.
        /// </param>
        /// <param name="mutual">
        /// should the second user be added to current user's buddy list too?
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ApproveRequest(int toUserId, bool mutual)
        {
            this.ClearCache(toUserId);

            return this.GetRepository<Buddy>().ApproveRequest(
                toUserId,
                BoardContext.Current.PageUserID,
                mutual);
        }

        /// <summary>
        /// Gets all the buddies of the current user.
        /// </summary>
        /// <returns>
        /// A <see cref="List"/> of all buddies.
        /// </returns>
        public List<BuddyUser> ListAll()
        {
            return this.Get<IDataCache>().GetOrSet(
                string.Format(Constants.Cache.UserBuddies, BoardContext.Current.PageUserID),
                () => this.GetRepository<Buddy>().ListAll(BoardContext.Current.PageUserID),
                TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// Clears the buddies cache for the current user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public void ClearCache(int userId)
        {
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(BoardContext.Current.PageUserID));
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));
        }

        /// <summary>
        /// Denies all buddy requests for the current user.
        /// </summary>
        public void DenyAllRequests()
        {
            var dt = this.ListAll()
                .Where(x => x.Approved == false && x.UserID == BoardContext.Current.PageUserID);

            dt.Where(x => Convert.ToDateTime(x.Requested).AddDays(14) < DateTime.UtcNow)
                .ForEach(x => this.DenyRequest(x.FromUserID));
        }

        /// <summary>
        /// Denies a buddy request.
        /// </summary>
        /// <param name="fromUserId">
        /// The from User Id.
        /// </param>
        public void DenyRequest(int fromUserId)
        {
            this.ClearCache(fromUserId);

            this.GetRepository<Buddy>().DenyRequest(
                fromUserId,
                BoardContext.Current.PageUserID);
        }

        /// <summary>
        /// Gets all the buddies for the specified user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<BuddyUser> GetForUser(int userId)
        {
            return this.GetRepository<Buddy>().ListAll(userId);
        }

        /// <summary>
        /// determines if the "<paramref name="buddyUserId"/>" and current user are buddies.
        /// </summary>
        /// <param name="buddyUserId">
        /// The Buddy User ID.
        /// </param>
        /// <param name="approved">
        /// Just look into approved buddies?
        /// </param>
        /// <returns>
        /// true if they are buddies, <see langword="false"/> if not.
        /// </returns>
        public bool IsBuddy(int buddyUserId, bool approved)
        {
            if (buddyUserId == BoardContext.Current.PageUserID)
            {
                return true;
            }

            var userBuddyList = this.Get<IFriends>().ListAll();

            if (userBuddyList.NullOrEmpty())
            {
                return false;
            }

            // Filter
            if (approved)
            {
                if (userBuddyList.Any(x => x.UserID == buddyUserId && x.Approved))
                {
                    return true;
                }
            }
            else
            {
                if (userBuddyList.Any(x => x.UserID == buddyUserId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the "<paramref name="toUserId"/>" from current user's buddy list.
        /// </summary>
        /// <param name="toUserId">
        /// The to user id.
        /// </param>
        /// <returns>
        /// The name of the second user.
        /// </returns>
        public string Remove(int toUserId)
        {
            this.ClearCache(toUserId);

            this.GetRepository<Buddy>().Remove(BoardContext.Current.PageUserID, toUserId);

            return this.Get<IUserDisplayName>().GetNameById(toUserId);
        }

        #endregion

        #endregion
    }
}