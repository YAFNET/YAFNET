/* Yet Another Forum.net
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
namespace YAF.Core.Services
{
  using System.Collections.Generic;
  using System.Web;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  /// <summary>
  /// User Ignored Service for the current user.
  /// </summary>
  public class YafUserIgnored : IUserIgnored
  {
    /// <summary>
    /// The _user ignore list.
    /// </summary>
    private List<int> _userIgnoreList = null;

    /// <summary>
    /// The is ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    /// <returns>
    /// The is ignored.
    /// </returns>
    public bool IsIgnored(int ignoredUserId)
    {
      if (this._userIgnoreList == null)
      {
        this._userIgnoreList = YafContext.Current.Get<IDBBroker>().UserIgnoredList(YafContext.Current.PageUserID);
      }

      if (this._userIgnoreList.Count > 0)
      {
        return this._userIgnoreList.Contains(ignoredUserId);
      }

      return false;
    }

    /// <summary>
    /// The clear ignore cache.
    /// </summary>
    public void ClearIgnoreCache()
    {
      // clear for the session
      string key = YafCache.GetBoardCacheKey(Constants.Cache.UserIgnoreList.FormatWith(YafContext.Current.PageUserID));
      YafContext.Current.Get<HttpSessionStateBase>().Remove(key);
    }

    /// <summary>
    /// The add ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    public void AddIgnored(int ignoredUserId)
    {
      LegacyDb.user_addignoreduser(YafContext.Current.PageUserID, ignoredUserId);
      ClearIgnoreCache();
    }

    /// <summary>
    /// The remove ignored.
    /// </summary>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    public void RemoveIgnored(int ignoredUserId)
    {
      LegacyDb.user_removeignoreduser(YafContext.Current.PageUserID, ignoredUserId);
      ClearIgnoreCache();
    }
  }
}
