/* Yet Another Forum.net
 * Copyright (C) 2006-2011 Jaben Cargman
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
namespace YAF.Modules
{
  #region Using

  using System;
  using System.Data;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Structures;

  #endregion

  /// <summary>
  /// The clear cache on events.
  /// </summary>
  public class ClearCacheOnEvents : IHaveServiceLocator, IHandleEvent<SuccessfulUserLoginEvent>, IHandleEvent<UpdateUserEvent>, IHandleEvent<UserLogoutEvent>
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ClearCacheOnEvents"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public ClearCacheOnEvents([NotNull] IServiceLocator serviceLocator, IDataCache dataCache)
    {
      this.ServiceLocator = serviceLocator;
      DataCache = dataCache;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order
    {
      get
      {
        return 10000;
      }
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    void IHandleEvent<UserLogoutEvent>.Handle(UserLogoutEvent @event)
    {
      // Clearing user cache with permissions data and active users cache...));
      this.DataCache.Remove(Constants.Cache.ActiveUserLazyData.FormatWith(@event.UserId));
      this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    void IHandleEvent<UpdateUserEvent>.Handle(UpdateUserEvent @event)
    {
      // clear the cache for this user...
      var userId = @event.UserId;

      this.Get<IUserDisplayName>().Clear(userId);
      this.DataCache.Remove(Constants.Cache.UserListForID.FormatWith(userId));
      this.DataCache.Remove(Constants.Cache.UserBuddies.FormatWith(userId));

      var cache = this.DataCache.GetOrSet(
        Constants.Cache.UserSignatureCache, () => new MostRecentlyUsed(250), TimeSpan.FromMinutes(10));

      // remove from the the signature cache...
      cache.Remove(userId);

      // Clearing cache with old Active User Lazy Data ...
      this.DataCache.Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userId));
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    public IDataCache DataCache { get; set; }

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
      this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
    }

    #endregion

    #endregion
  }
}