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
  using System;
  using System.Data;

  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Yaf buddies service
  /// </summary>
  public static class YafBuddies
  {
    #region Public Methods

    /// <summary>
    /// Adds a buddy request.
    /// </summary>
    /// <param name="ToUserID">
    /// the to user id.
    /// </param>
    /// <returns>
    /// The name of the second user + whether this request is approved or not. (This request
    /// is approved without the target user's approval if the target user has sent a buddy request
    /// to current user too or if the current user is already in the target user's buddy list.
    /// </returns>
    public static string[] AddBuddyRequest(int ToUserID)
    {
      ClearBuddyCache(ToUserID);

      return DB.buddy_addrequest(YafContext.Current.PageUserID, ToUserID);
    }

    /// <summary>
    /// Approves all buddy requests for the current user.
    /// </summary>
    /// <param name="Mutual">
    /// should the users be added to current user's buddy list too?
    /// </param>
    public static void ApproveAllBuddyRequests(bool Mutual)
    {
      DataTable dt = BuddyList();
      DataView dv = dt.DefaultView;
      dv.RowFilter = "Approved = 0 AND UserID = {0}".FormatWith(YafContext.Current.PageUserID);
      foreach (DataRowView drv in dv)
      {
        ApproveBuddyRequest((int)drv["FromUserID"], Mutual);
      }
    }

    /// <summary>
    /// Approves a buddy request.
    /// </summary>
    /// <param name="ToUserID">
    /// the to user id.
    /// </param>
    /// <param name="Mutual">
    /// should the second user be added to current user's buddy list too?
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    public static string ApproveBuddyRequest(int ToUserID, bool Mutual)
    {
      ClearBuddyCache(ToUserID);
      return DB.buddy_approveRequest(ToUserID, YafContext.Current.PageUserID, Mutual);
    }

    /// <summary>
    /// Gets all the buddies of the current user.
    /// </summary>
    /// <returns>
    /// A <see cref="DataTable"/> of all buddies.
    /// </returns>
    public static DataTable BuddyList()
    {
      return YafContext.Current.Get<YafDBBroker>().UserBuddyList(YafContext.Current.PageUserID);
    }

    /// <summary>
    /// Clears the buddies cache for the current user.
    /// </summary>
    /// <param name="UserID">
    /// The User ID.
    /// </param>
    public static void ClearBuddyCache(int UserID)
    {
      // Clear for the current user.
      YafContext.Current.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.UserBuddies.FormatWith(YafContext.Current.PageUserID)));
      YafContext.Current.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ActiveUserLazyData.FormatWith(YafContext.Current.PageUserID)));
      
       // Clear for the second user.
      YafContext.Current.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.UserBuddies.FormatWith(UserID)));
      YafContext.Current.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ActiveUserLazyData.FormatWith(UserID)));
      
    }

    /// <summary>
    /// Denies all buddy requests for the current user.
    /// </summary>
    public static void DenyAllBuddyRequests()
    {
      DataTable dt = BuddyList();
      DataView dv = dt.DefaultView;
      dv.RowFilter = "Approved = 0 AND UserID = {0}".FormatWith(YafContext.Current.PageUserID);
      foreach (DataRowView drv in dv)
      {
        if (Convert.ToDateTime(drv["Requested"]).AddDays(14) < DateTime.UtcNow)
        {
          DenyBuddyRequest((int)drv["FromUserID"]);
        }
      }
    }

    /// <summary>
    /// Denies a buddy request.
    /// </summary>
    /// <param name="ToUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// the name of the second user.
    /// </returns>
    public static string DenyBuddyRequest(int ToUserID)
    {
      ClearBuddyCache(ToUserID);
      return DB.buddy_denyRequest(ToUserID, YafContext.Current.PageUserID);
    }

    /// <summary>
    /// Gets all the buddies for the specified user.
    /// </summary>
    /// <param name="UserID">
    /// The user id.
    /// </param>
    /// <returns>
    /// a <see cref="DataTable"/> of all buddies.
    /// </returns>
    public static DataTable GetBuddiesForUser(int UserID)
    {
      return YafContext.Current.Get<YafDBBroker>().UserBuddyList(UserID);
    }

    /// <summary>
    /// determines if the "<paramref name="BuddyUserID"/>" and current user are buddies.
    /// </summary>
    /// <param name="BuddyUserID">
    /// The Buddy User ID.
    /// </param>
    /// <param name="Approved">
    /// Just look into approved buddies?
    /// </param>
    /// <returns>
    /// true if they are buddies, <see langword="false"/> if not.
    /// </returns>
    public static bool IsBuddy(int BuddyUserID, bool Approved)
    {
      if (BuddyUserID == YafContext.Current.PageUserID)
      {
        return true;
      }

      DataTable userBuddyList = YafContext.Current.Get<YafDBBroker>().UserBuddyList(YafContext.Current.PageUserID);

      if ((userBuddyList != null) && (userBuddyList.Rows.Count > 0))
      {
        // Filter the DataTable.
        if (Approved)
        {
          if (userBuddyList.Select("UserID = {0} AND Approved = 1".FormatWith(BuddyUserID)).Length > 0)
          {
            return true;
          }
        }
        else
        {
          if (userBuddyList.Select("UserID = {0}".FormatWith(BuddyUserID)).Length > 0)
          {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Removes the "<paramref name="ToUserID"/>" from current user's buddy list.
    /// </summary>
    /// <param name="ToUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    public static string RemoveBuddy(int ToUserID)
    {
      ClearBuddyCache(ToUserID);
      return DB.buddy_remove(YafContext.Current.PageUserID, ToUserID);
    }

    #endregion
  }
}