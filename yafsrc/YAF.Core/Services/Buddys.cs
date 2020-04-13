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
namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;

    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// YAF buddies service
    /// </summary>
    public class Buddys : IBuddy, IHaveServiceLocator
    {
        #region Constants and Fields

        /// <summary>
        /// The DB broker.
        /// </summary>
        private readonly DataBroker dbBroker;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Buddys"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="dbBroker">
        /// The DB broker.
        /// </param>
        public Buddys([NotNull] IServiceLocator serviceLocator, [NotNull] DataBroker dbBroker)
        {
            this.ServiceLocator = serviceLocator;
            this.dbBroker = dbBroker;
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
        public string[] AddRequest(int toUserId)
        {
            this.ClearCache(toUserId);

            return this.GetRepository<Buddy>().AddRequest(
                BoardContext.Current.PageUserID,
                toUserId,
                BoardContext.Current.BoardSettings.EnableDisplayName);
        }

        /// <summary>
        /// Approves all buddy requests for the current user.
        /// </summary>
        /// <param name="mutual">
        /// should the users be added to current user's buddy list too?
        /// </param>
        public void ApproveAllRequests(bool mutual)
        {
            var dt = this.All();
            var dv = dt.DefaultView;
            dv.RowFilter = $"Approved = 0 AND UserID = {BoardContext.Current.PageUserID}";

            dv.Cast<DataRowView>().ForEach(drv => this.ApproveRequest((int)drv["FromUserID"], mutual));
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
        /// The name of the second user.
        /// </returns>
        public string ApproveRequest(int toUserId, bool mutual)
        {
            this.ClearCache(toUserId);
            return this.GetRepository<Buddy>().ApproveRequest(
                toUserId,
                BoardContext.Current.PageUserID,
                mutual,
                BoardContext.Current.BoardSettings.EnableDisplayName);
        }

        /// <summary>
        /// Gets all the buddies of the current user.
        /// </summary>
        /// <returns>
        /// A <see cref="DataTable"/> of all buddies.
        /// </returns>
        public DataTable All()
        {
            return this.dbBroker.UserBuddyList(BoardContext.Current.PageUserID);
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
            var dt = this.All();
            var dv = dt.DefaultView;
            dv.RowFilter = $"Approved = 0 AND UserID = {BoardContext.Current.PageUserID}";

            dv.Cast<DataRowView>().Where(drv => Convert.ToDateTime(drv["Requested"]).AddDays(14) < System.DateTime.UtcNow)
                .ForEach(drv => this.DenyRequest((int)drv["FromUserID"]));
        }

        /// <summary>
        /// Denies a buddy request.
        /// </summary>
        /// <param name="toUserId">
        /// The to user id.
        /// </param>
        /// <returns>
        /// the name of the second user.
        /// </returns>
        public string DenyRequest(int toUserId)
        {
            this.ClearCache(toUserId);
            return this.GetRepository<Buddy>().DenyRequest(
                toUserId,
                BoardContext.Current.PageUserID,
                BoardContext.Current.BoardSettings.EnableDisplayName);
        }

        /// <summary>
        /// Gets all the buddies for the specified user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// a <see cref="DataTable"/> of all buddies.
        /// </returns>
        public DataTable GetForUser(int userId)
        {
            return this.dbBroker.UserBuddyList(userId);
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

            var userBuddyList = this.dbBroker.UserBuddyList(BoardContext.Current.PageUserID);

            if (userBuddyList == null || !userBuddyList.HasRows())
            {
                return false;
            }

            // Filter the DataTable.
            if (approved)
            {
                if (userBuddyList.Select($"UserID = {buddyUserId} AND Approved = 1").Length > 0)
                {
                    return true;
                }
            }
            else
            {
                if (userBuddyList.Select($"UserID = {buddyUserId}").Length > 0)
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
            return this.GetRepository<Buddy>().Remove(
                BoardContext.Current.PageUserID,
                toUserId,
                BoardContext.Current.BoardSettings.EnableDisplayName);
        }

        #endregion

        #endregion
    }
}