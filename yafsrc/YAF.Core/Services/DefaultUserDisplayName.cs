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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
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
            this.UserDisplayNameCollection.TryRemove(userId, out _);
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
        public IList<User> Find([NotNull] string contains)
        {
            return BoardContext.Current.Get<BoardSettings>().EnableDisplayName
                       ? this.GetRepository<User>().Get(u => u.DisplayName.Contains(contains))
                       : this.GetRepository<User>().Get(u => u.Name.Contains(contains));
        }

        /// <summary>
        /// Get the UserID from the user name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="int?"/>.
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
                if (BoardContext.Current.Get<BoardSettings>().EnableDisplayName)
                {
                    var user =
                      this.GetRepository<User>().FindUserTyped(true, displayName: name).FirstOrDefault();

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
                      this.GetRepository<User>().FindUserTyped(true, userName: name).FirstOrDefault();

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
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetName(int userId)
        {
            if (this.UserDisplayNameCollection.TryGetValue(userId, out var displayName))
            {
                return displayName;
            }

            var row = BoardContext.Current.GetRepository<User>().GetById(userId);

            if (row == null)
            {
                return null;
            }

            if (BoardContext.Current.Get<BoardSettings>().EnableDisplayName)
            {
                displayName = row.DisplayName;
            }

            if (displayName.IsNotSet())
            {
                // revert to their user name...
                displayName = row.Name;
            }

            this.UserDisplayNameCollection.AddOrUpdate(userId, k => displayName, (k, v) => displayName);

            return displayName;
        }

        #endregion

        #endregion
    }
}