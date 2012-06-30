/* Yet Another Forum.NET
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
namespace YAF.Core
{
    #region Using

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;

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
            IEnumerable<TypedUserFind> found;

            if (YafContext.Current.Get<YafBoardSettings>().EnableDisplayName)
            {
                found = LegacyDb.UserFind(YafContext.Current.PageBoardID, true, null, null, contains, null, null);
                return found.ToDictionary(k => k.UserID ?? 0, v => v.DisplayName);
            }

            found = LegacyDb.UserFind(YafContext.Current.PageBoardID, true, contains, null, null, null, null);
            return found.ToDictionary(k => k.UserID ?? 0, v => v.Name);
        }

        /// <summary>
        /// Get the userid from the user name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The get id.
        /// </returns>
        public int? GetId([NotNull] string name)
        {
            int? userId = null;

            if (name.IsNotSet())
            {
                return userId;
            }

            var keyValue =
                this.UserDisplayNameCollection
                .ToList()
                .FirstOrDefault(x => x.Value.IsSet() && x.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if (keyValue.IsNotNull())
            {
                userId = keyValue.Key;
            }
            else
            {
                // find the username...
                if (YafContext.Current.Get<YafBoardSettings>().EnableDisplayName)
                {
                    var user =
                      LegacyDb.UserFind(YafContext.Current.PageBoardID, false, null, null, name, null, null).FirstOrDefault();

                    if (user != null)
                    {
                        userId = user.UserID ?? 0;
                        this.UserDisplayNameCollection.AddOrUpdate(userId.Value, k => user.DisplayName, (k, v) => user.DisplayName);
                    }
                }
                else
                {
                    var user =
                      LegacyDb.UserFind(YafContext.Current.PageBoardID, false, name, null, null, null, null).FirstOrDefault();

                    if (user != null)
                    {
                        userId = user.UserID ?? 0;
                        this.UserDisplayNameCollection.AddOrUpdate(userId.Value, k => user.DisplayName, (k, v) => user.DisplayName);
                    }
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

            if (!this.UserDisplayNameCollection.TryGetValue(userId, out displayName))
            {
                var row = UserMembershipHelper.GetUserRowForID(userId, true);

                if (row != null)
                {
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
                }
            }

            return displayName;
        }

        #endregion

        #endregion
    }
}