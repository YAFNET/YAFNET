/* Yet Another Forum.NET
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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;

  using YAF.Classes.Data;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The default user display name.
  /// </summary>
  public class DefaultUserDisplayName : IUserDisplayName
  {
    #region Constants and Fields

    /// <summary>
    /// The _data cache.
    /// </summary>
    private readonly IDataCache<IDictionary<int, string>> _dataCache;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultUserDisplayName"/> class.
    /// </summary>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    public DefaultUserDisplayName([NotNull] IDataCache<IDictionary<int, string>> dataCache)
    {
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");

      this._dataCache = dataCache;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets UserDisplayNameCollection.
    /// </summary>
    private IDictionary<int, string> UserDisplayNameCollection
    {
      get
      {
        return this._dataCache.GetOrSet(Constants.Cache.UsersDisplayNameCollection, () => new Dictionary<int, string>());
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IUserDisplayName

    /// <summary>
    /// Remove the item from collection
    /// </summary>
    /// <param name="userId">
    /// </param>
    public void Clear(int userId)
    {
      // update collection...
      if (this.UserDisplayNameCollection.ContainsKey(userId))
      {
        this.UserDisplayNameCollection.Remove(userId);
      }
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
    /// The find.
    /// </summary>
    /// <param name="contains">
    /// The contains.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public IDictionary<int, string> Find([NotNull] string contains)
    {
      var usersFound = new Dictionary<int, string>();

      if (YafContext.Current.BoardSettings.EnableDisplayName)
      {
        var found = LegacyDb.UserFind(YafContext.Current.PageBoardID, true, null, null, contains, null, null);
        found.ForEach(u => usersFound.Add(u.UserID ?? 0, u.DisplayName));
      }
      else
      {
        var found = LegacyDb.UserFind(YafContext.Current.PageBoardID, true, contains, null, null, null, null);
        found.ForEach(u => usersFound.Add(u.UserID ?? 0, u.DisplayName));
      }

      return usersFound;
    }

    /// <summary>
    /// The get id.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// </returns>
    public int? GetId([NotNull] string name)
    {
      int? userId = null;

      if (name.IsNotSet())
      {
        return userId;
      }

      if (
        this.UserDisplayNameCollection.Any(
          x => x.Value.IsSet() && x.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
      {
        userId =
          this.UserDisplayNameCollection.Where(
            x => x.Value.IsSet() && x.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().
            Key;
      }
      else
      {
        // find the username...
        if (YafContext.Current.BoardSettings.EnableDisplayName)
        {
          var user =
            LegacyDb.UserFind(YafContext.Current.PageBoardID, false, null, null, name, null, null).FirstOrDefault();
          if (user != null)
          {
            userId = user.UserID ?? 0;

            // update collection...
            if (!this.UserDisplayNameCollection.ContainsKey(userId.Value))
            {
              this.UserDisplayNameCollection.Add(userId.Value, user.DisplayName);
            }
          }
        }
        else
        {
          var user =
            LegacyDb.UserFind(YafContext.Current.PageBoardID, false, name, null, null, null, null).FirstOrDefault();
          if (user != null)
          {
            userId = user.UserID ?? 0;

            // update collection...
            if (!this.UserDisplayNameCollection.ContainsKey(userId.Value))
            {
              this.UserDisplayNameCollection.Add(userId.Value, user.DisplayName);
            }
          }
        }
      }

      return userId;
    }

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
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
          if (YafContext.Current.BoardSettings.EnableDisplayName)
          {
            displayName = row.Field<string>("DisplayName");
          }

          if (displayName.IsNotSet())
          {
            // revert to their user name...
            displayName = row.Field<string>("Name");
          }

          // update collection...
          if (!this.UserDisplayNameCollection.ContainsKey(userId))
          {
            this.UserDisplayNameCollection.Add(userId, displayName);
          }
        }
      }

      return displayName;
    }

    #endregion

    #endregion
  }
}