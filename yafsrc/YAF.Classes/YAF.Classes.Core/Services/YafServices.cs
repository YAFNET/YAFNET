/* YetAnotherForum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Core
{
  /// <summary>
  /// The yaf services.
  /// </summary>
  public static class YafServices
  {
    /// <summary>
    /// Gets Notification.
    /// </summary>
    public static YafSendNotification SendNotification
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafSendNotification>();
      }
    }

    /// <summary>
    /// Gets BadWordReplace.
    /// </summary>
    public static YafBadWordReplace BadWordReplace
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafBadWordReplace>();
      }
    }

    /// <summary>
    /// Gets Permissions.
    /// </summary>
    public static YafPermissions Permissions
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafPermissions>();
      }
    }

    /// <summary>
    /// Gets SendMail.
    /// </summary>
    public static YafSendMail SendMail
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafSendMail>();
      }
    }

    /// <summary>
    /// Gets DBBroker.
    /// </summary>
    public static YafDBBroker DBBroker
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafDBBroker>();
      }
    }

    /// <summary>
    /// Gets DateTime.
    /// </summary>
    public static YafDateTime DateTime
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafDateTime>();
      }
    }

    /// <summary>
    /// Gets StopWatch.
    /// </summary>
    public static YafStopWatch StopWatch
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafStopWatch>();
      }
    }

    /// <summary>
    /// Gets InitializeDb.
    /// </summary>
    public static YafInitializeDb InitializeDb
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafInitializeDb>();
      }
    }

    /// <summary>
    /// Gets BannedIps.
    /// </summary>
    public static YafCheckBannedIps BannedIps
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafCheckBannedIps>();
      }
    }

    /// <summary>
    /// Gets Avatar.
    /// </summary>
    public static YafAvatars Avatar
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafAvatars>();
      }
    }

    /// <summary>
    /// Gets User Ignored.
    /// </summary>
    public static YafUserIgnored UserIgnored
    {
      get
      {
        return YafContext.Current.InstanceFactory.GetInstance<YafUserIgnored>();
      }
    }

    /// <summary>
    /// Gets Favorite Topic.
    /// </summary>
    public static YafFavoriteTopic FavoriteTopic
    {
        get
        {
            return YafContext.Current.InstanceFactory.GetInstance<YafFavoriteTopic>();
        }
    }
  }
}