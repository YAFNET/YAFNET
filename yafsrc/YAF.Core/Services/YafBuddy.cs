/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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

    using YAF.Classes.Data;
    using YAF.Types;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// YAF buddies service
    /// </summary>
    public class YafBuddy : IBuddy, IHaveServiceLocator
    {
        #region Constants and Fields

        /// <summary>
        /// The DB broker.
        /// </summary>
        private readonly YafDbBroker _dbBroker;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafBuddy"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="dbBroker">
        /// The DB broker.
        /// </param>
        public YafBuddy([NotNull] IServiceLocator serviceLocator, [NotNull] YafDbBroker dbBroker)
        {
            this.ServiceLocator = serviceLocator;
            this._dbBroker = dbBroker;
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
        /// <param name="toUserID">
        /// the to user id.
        /// </param>
        /// <returns>
        /// The name of the second user + whether this request is approved or not. (This request
        ///   is approved without the target user's approval if the target user has sent a buddy request
        ///   to current user too or if the current user is already in the target user's buddy list.
        /// </returns>
        public string[] AddRequest(int toUserID)
        {
            this.ClearCache(toUserID);

            return LegacyDb.buddy_addrequest(
                YafContext.Current.PageUserID,
                toUserID,
                YafContext.Current.BoardSettings.EnableDisplayName);
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
            dv.RowFilter = "Approved = 0 AND UserID = {0}".FormatWith(YafContext.Current.PageUserID);
            foreach (DataRowView drv in dv)
            {
                this.ApproveRequest((int)drv["FromUserID"], mutual);
            }
        }

        /// <summary>
        /// Approves a buddy request.
        /// </summary>
        /// <param name="toUserID">
        /// the to user id.
        /// </param>
        /// <param name="mutual">
        /// should the second user be added to current user's buddy list too?
        /// </param>
        /// <returns>
        /// The name of the second user.
        /// </returns>
        public string ApproveRequest(int toUserID, bool mutual)
        {
            this.ClearCache(toUserID);
            return LegacyDb.buddy_approveRequest(
                toUserID,
                YafContext.Current.PageUserID,
                mutual,
                YafContext.Current.BoardSettings.EnableDisplayName);
        }

        /// <summary>
        /// Gets all the buddies of the current user.
        /// </summary>
        /// <returns>
        /// A <see cref="DataTable"/> of all buddies.
        /// </returns>
        public DataTable All()
        {
            return this._dbBroker.UserBuddyList(YafContext.Current.PageUserID);
        }

        /// <summary>
        /// Clears the buddies cache for the current user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public void ClearCache(int userId)
        {
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(YafContext.Current.PageUserID));
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));
        }

        /// <summary>
        /// Denies all buddy requests for the current user.
        /// </summary>
        public void DenyAllRequests()
        {
            var dt = this.All();
            var dv = dt.DefaultView;
            dv.RowFilter = "Approved = 0 AND UserID = {0}".FormatWith(YafContext.Current.PageUserID);

            foreach (
                var drv in
                    dv.Cast<DataRowView>()
                        .Where(drv => Convert.ToDateTime(drv["Requested"]).AddDays(14) < DateTime.UtcNow))
            {
                this.DenyRequest((int)drv["FromUserID"]);
            }
        }

        /// <summary>
        /// Denies a buddy request.
        /// </summary>
        /// <param name="toUserID">
        /// The to user id.
        /// </param>
        /// <returns>
        /// the name of the second user.
        /// </returns>
        public string DenyRequest(int toUserID)
        {
            this.ClearCache(toUserID);
            return LegacyDb.buddy_denyRequest(
                toUserID,
                YafContext.Current.PageUserID,
                YafContext.Current.BoardSettings.EnableDisplayName);
        }

        /// <summary>
        /// Gets all the buddies for the specified user.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// a <see cref="DataTable"/> of all buddies.
        /// </returns>
        public DataTable GetForUser(int userID)
        {
            return this._dbBroker.UserBuddyList(userID);
        }

        /// <summary>
        /// determines if the "<paramref name="buddyUserID"/>" and current user are buddies.
        /// </summary>
        /// <param name="buddyUserID">
        /// The Buddy User ID.
        /// </param>
        /// <param name="approved">
        /// Just look into approved buddies?
        /// </param>
        /// <returns>
        /// true if they are buddies, <see langword="false"/> if not.
        /// </returns>
        public bool IsBuddy(int buddyUserID, bool approved)
        {
            if (buddyUserID == YafContext.Current.PageUserID)
            {
                return true;
            }

            var userBuddyList = this._dbBroker.UserBuddyList(YafContext.Current.PageUserID);

            if (userBuddyList == null || !userBuddyList.HasRows())
            {
                return false;
            }

            // Filter the DataTable.
            if (approved)
            {
                if (userBuddyList.Select("UserID = {0} AND Approved = 1".FormatWith(buddyUserID)).Length > 0)
                {
                    return true;
                }
            }
            else
            {
                if (userBuddyList.Select("UserID = {0}".FormatWith(buddyUserID)).Length > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the "<paramref name="toUserID"/>" from current user's buddy list.
        /// </summary>
        /// <param name="toUserID">
        /// The to user id.
        /// </param>
        /// <returns>
        /// The name of the second user.
        /// </returns>
        public string Remove(int toUserID)
        {
            this.ClearCache(toUserID);
            return LegacyDb.buddy_remove(
                YafContext.Current.PageUserID,
                toUserID,
                YafContext.Current.BoardSettings.EnableDisplayName);
        }

        #endregion

        #endregion
    }
}