/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
namespace YAF.Modules
{
  #region Using

  using System;

  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.EventProxies;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils.Structures;

  #endregion

  /// <summary>
  /// The clear cache on events.
  /// </summary>
  [ExportService(serviceLifetimeScope: ServiceLifetimeScope.InstancePerScope)]
  public class ClearCacheOnEvents : IHaveServiceLocator, 
                                    IHandleEvent<SuccessfulUserLoginEvent>, 
                                    IHandleEvent<UpdateUserEvent>, 
                                    IHandleEvent<UserLogoutEvent>
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ClearCacheOnEvents"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="dataCache">
    /// The data Cache.
    /// </param>
    public ClearCacheOnEvents([NotNull] IServiceLocator serviceLocator, [NotNull] IDataCache dataCache)
    {
      CodeContracts.VerifyNotNull(obj: serviceLocator, argumentName: "serviceLocator");
      CodeContracts.VerifyNotNull(obj: dataCache, argumentName: "dataCache");

      this.ServiceLocator = serviceLocator;
      this.DataCache = dataCache;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets DataCache.
    /// </summary>
    public IDataCache DataCache { get; set; }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 10000;

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    #endregion

    #region Implemented Interfaces

    #region IHandleEvent<SuccessfulUserLoginEvent>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    void IHandleEvent<SuccessfulUserLoginEvent>.Handle([NotNull] SuccessfulUserLoginEvent @event)
    {
      // vzrus: to clear the cache to show user in the list at once));
      this.DataCache.Remove(key: Constants.Cache.UsersOnlineStatus);
      this.DataCache.Remove(key: Constants.Cache.ForumActiveDiscussions);
    }

    #endregion

    #region IHandleEvent<UpdateUserEvent>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    void IHandleEvent<UpdateUserEvent>.Handle([NotNull] UpdateUserEvent @event)
    {
      // clear the cache for this user...
      var userId = @event.UserId;

      this.Get<IUserDisplayName>().Clear(userId: userId);
      this.DataCache.Remove(key: string.Format(format: Constants.Cache.UserListForID, arg0: userId));
      this.DataCache.Remove(key: string.Format(format: Constants.Cache.UserBuddies, arg0: userId));

      var cache = this.DataCache.GetOrSet(
        key: Constants.Cache.UserSignatureCache, getValue: () => new MostRecentlyUsed(maxItems: 250), timeout: TimeSpan.FromMinutes(value: 10));

      // remove from the the signature cache...
      cache.Remove(key: userId);

      // Clearing cache with old Active User Lazy Data ...
      this.DataCache.Remove(key: string.Format(format: Constants.Cache.ActiveUserLazyData, arg0: userId));
    }

    #endregion

    #region IHandleEvent<UserLogoutEvent>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    void IHandleEvent<UserLogoutEvent>.Handle([NotNull] UserLogoutEvent @event)
    {
      // Clearing user cache with permissions data and active users cache...));
      this.DataCache.Remove(key: string.Format(format: Constants.Cache.ActiveUserLazyData, arg0: @event.UserId));
      this.DataCache.Remove(key: Constants.Cache.UsersOnlineStatus);
    }

    #endregion

    #endregion
  }
}