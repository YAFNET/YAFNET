/* Yet Another Forum.NET
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
  #region Using

  using YAF.Classes.Interfaces;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The default user display name.
  /// </summary>
  public class DefaultUserDisplayName : IUserDisplayName
  {
    #region Constants and Fields

    /// <summary>
    /// The _user display name collection.
    /// </summary>
    private ThreadSafeDictionary<int, string> _userDisplayNameCollection = null;

    #endregion

    #region Properties

    /// <summary>
    /// Gets UserDisplayNameCollection.
    /// </summary>
    private ThreadSafeDictionary<int, string> UserDisplayNameCollection
    {
      get
      {
        if (this._userDisplayNameCollection == null)
        {
          string key = YafCache.GetBoardCacheKey(Constants.Cache.UsersDisplayNameCollection);
          this._userDisplayNameCollection = YafContext.Current.Cache.GetItem(
            key, 999, () => new ThreadSafeDictionary<int, string>());
        }

        return this._userDisplayNameCollection;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IUserDisplayName

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// The get.
    /// </returns>
    public string Get(int userId)
    {
      string displayName;

      if (!this.UserDisplayNameCollection.TryGetValue(userId, out displayName))
      {
        string userName = UserMembershipHelper.GetUserNameFromID(userId);

        if (YafContext.Current.BoardSettings.EnableDisplayName)
        {
          displayName = YafUserProfile.GetProfile(userName).DisplayName;

          if (displayName.IsNullOrEmptyTrimmed())
          {
            // revert to their user name...
            displayName = userName;
          }
        }
        else
        {
          displayName = userName;
        }

        // update collection...
        this.UserDisplayNameCollection.MergeSafe(userId, displayName);
      }

      return displayName;
    }

    #endregion

    #endregion
  }
}