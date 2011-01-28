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
  using System.Linq;

  using YAF.Types;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The i data cache extensions.
  /// </summary>
  public static class IDataCacheExtensions
  {
    #region Public Methods

    /// <summary>
    /// Creates string to be used as key for caching, board-specific
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="dataCache">
    /// The data Cache.
    /// </param>
    /// <param name="key">
    /// Key we need to make board-specific
    /// </param>
    /// <returns>
    /// Board sepecific cache key based on key parameter
    /// </returns>
    public static string GetBoardCacheKey<T>([NotNull] this IDataCache<T> dataCache, [NotNull] string key)
    {
      // use current YafCache instance as source of board ID
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");
      CodeContracts.ArgumentNotNull(key, "key");

      return dataCache.GetBoardCacheKey(key, YafContext.Current.PageBoardID);
    }

    /// <summary>
    /// Creates string to be used as key for caching, board-specific
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="dataCache">
    /// The data Cache.
    /// </param>
    /// <param name="key">
    /// Key we need to make board-specific
    /// </param>
    /// <param name="boardID">
    /// Board ID to use when creating cache key
    /// </param>
    /// <returns>
    /// Board sepecific cache key based on key parameter
    /// </returns>
    public static string GetBoardCacheKey<T>([NotNull] this IDataCache<T> dataCache, [NotNull] string key, int boardID)
    {
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");
      CodeContracts.ArgumentNotNull(key, "key");

      return "{0}.{1}".FormatWith(key, boardID);
    }

    /// <summary>
    /// The remote all.
    /// </summary>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public static void RemoveAll<T>([NotNull] this IDataCache<T> dataCache)
    {
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");

      dataCache.GetAll().ForEach(i => dataCache.Remove(i.Key));
    }

    /// <summary>
    /// The remote all where.
    /// </summary>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    /// <param name="whereFunc">
    /// The where function.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public static void RemoveAllWhere<T>(
      [NotNull] this IDataCache<T> dataCache, [NotNull] Func<KeyValuePair<string, T>, bool> whereFunc)
    {
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");
      CodeContracts.ArgumentNotNull(whereFunc, "whereFunc");

      dataCache.GetAll().Where(whereFunc).ForEach(i => dataCache.Remove(i.Key));
    }

    #endregion
  }
}