/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Types.Constants
{
  /// <summary>
  /// The user communication type.
  /// </summary>
  public enum UserNotificationSetting
  {
    /// <summary>
    ///   No Notifications
    /// </summary>
    NoNotification = 10, 

    /// <summary>
    ///   The all topics.
    /// </summary>
    AllTopics = 20, 

    /// <summary>
    ///   The topics I post to or subscribe to.
    /// </summary>
    TopicsIPostToOrSubscribeTo = 30, 

    /// <summary>
    ///   The topics i subscribe to.
    /// </summary>
    TopicsISubscribeTo = 0
  }
}