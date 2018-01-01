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
namespace YAF.Core
{
    #region Using

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Classes;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The default user display name.
    /// </summary>
    public class DefaultUserDisplayName : IUserDisplayName, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultUserDisplayName"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public DefaultUserDisplayName(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        ///   Gets UserDisplayNameCollection.
        /// </summary>
        private ConcurrentDictionary<int, string> UserDisplayNameCollection
        {
            get
            {
                return
                  this.Get<IObjectStore>().GetOrSet(
                    Constants.Cache.UsersDisplayNameCollection, () => new ConcurrentDictionary<int, string>());
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IUserDisplayName

        /// <summary>
        /// Remove the item from collection
        /// </summary>
        /// <param name="userId">The user id.</param>
        public void Clear(int userId)
        {
            // update collection...
            string outValue;
            this.UserDisplayNameCollection.TryRemove(userId, out outValue);
        }

        /// <summary>
        /// Remove all the items from the collection
        /// </summary>
        public void Clear()
        {
            // update collection...
            this.UserDisplayNameCollection.Clear();
        }

        /// <summary>
        /// Find user
        /// </summary>
        /// <param name="contains">The contains.</param>
        /// <returns>
        /// Returns the Found User
        /// </returns>
        [NotNull]
        public IDictionary<int, string> Find([NotNull] string contains)
        {
            IList<User> found;

            if (YafContext.Current.Get<YafBoardSettings>().EnableDisplayName)
            {
                found = this.GetRepository<User>().FindUserTyped(filter: true, displayName: contains);
                return found.ToDictionary(k => k.ID, v => v.DisplayName);
            }

            found = this.GetRepository<User>().FindUserTyped(filter: true, userName: contains);
            return found.ToDictionary(k => k.ID, v => v.Name);
        }

        /// <summary>
        /// Get the UserID from the user name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The get id.
        /// </returns>
        public int? GetId([NotNull] string name)
        {
            int? userId;

            if (name.IsNotSet())
            {
                return null;
            }

            var keyValue =
                this.UserDisplayNameCollection
                .ToList()
                .FirstOrDefault(x => x.Value.IsSet() && x.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if (keyValue.IsNotDefault())
            {
                userId = keyValue.Key;
            }
            else
            {
                // find the username...
                if (YafContext.Current.Get<YafBoardSettings>().EnableDisplayName)
                {
                    var user =
                      this.GetRepository<User>().FindUserTyped(filter: true, displayName: name).FirstOrDefault();

                    if (user == null)
                    {
                        return null;
                    }

                    userId = user.ID;
                    this.UserDisplayNameCollection.AddOrUpdate(userId.Value, k => user.DisplayName, (k, v) => user.DisplayName);
                }
                else
                {
                    var user =
                      this.GetRepository<User>().FindUserTyped(filter: true, userName: name).FirstOrDefault();

                    if (user == null)
                    {
                        return null;
                    }

                    userId = user.ID;
                    this.UserDisplayNameCollection.AddOrUpdate(userId.Value, k => user.DisplayName, (k, v) => user.DisplayName);
                }
            }

            return userId;
        }

        /// <summary>
        /// Get the Display Name from a <paramref name="userId"/>
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>
        /// The get.
        /// </returns>
        public string GetName(int userId)
        {
            string displayName;

            if (this.UserDisplayNameCollection.TryGetValue(userId, out displayName))
            {
                return displayName;
            }

            var row = UserMembershipHelper.GetUserRowForID(userId, true);

            if (row == null)
            {
                return null;
            }

            if (YafContext.Current.Get<YafBoardSettings>().EnableDisplayName)
            {
                displayName = row.Field<string>("DisplayName");
            }

            if (displayName.IsNotSet())
            {
                // revert to their user name...
                displayName = row.Field<string>("Name");
            }

            this.UserDisplayNameCollection.AddOrUpdate(userId, k => displayName, (k, v) => displayName);

            return displayName;
        }

        #endregion

        #endregion
    }
}