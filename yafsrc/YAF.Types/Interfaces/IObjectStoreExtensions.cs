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
namespace YAF.Types.Interfaces
{
  #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

  /// <summary>
  /// The object store extensions.
  /// </summary>
  public static class IObjectStoreExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="objectStore">
    /// The object store.
    /// </param>
    /// <param name="originalKey">
    /// The original key.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this IObjectStore objectStore, [NotNull] string originalKey)
    {
      CodeContracts.VerifyNotNull(objectStore, "objectStore");
      CodeContracts.VerifyNotNull(originalKey, "originalKey");

      var item = objectStore.Get(originalKey);

      if (item is T)
      {
        return (T)item;
      }

      return default(T);
    }

    /// <summary>
    /// The remote all.
    /// </summary>
    /// <param name="objectStore">
    /// The object store.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public static void RemoveOf<T>([NotNull] this IObjectStore objectStore)
    {
      CodeContracts.VerifyNotNull(objectStore, "objectStore");

      foreach (var i in objectStore.GetAll<T>().ToList())
      {
        objectStore.Remove(i.Key);
      }
    }

    /// <summary>
    /// Clear the entire cache.
    /// </summary>
    /// <param name="objectStore">
    /// The object store.
    /// </param>
    public static void Clear([NotNull] this IObjectStore objectStore)
    {
      // remove all objects in the cache...
      CodeContracts.VerifyNotNull(objectStore, "objectStore");

      objectStore.RemoveOf<object>();
    }

    /// <summary>
    /// Count of objects in the cache.
    /// </summary>
    /// <param name="objectStore">
    /// The object store.
    /// </param>
    public static int Count([NotNull] this IObjectStore objectStore)
    {
      // remove all objects in the cache...
      CodeContracts.VerifyNotNull(objectStore, "objectStore");

      return objectStore.GetAll<object>().Count();
    }

    /// <summary>
    /// Count of T in the cache.
    /// </summary>
    /// <param name="objectStore">
    /// The object store.
    /// </param>
    public static int CountOf<T>([NotNull] this IObjectStore objectStore)
    {
      // remove all objects in the cache...
      CodeContracts.VerifyNotNull(objectStore, "objectStore");

      return objectStore.GetAll<T>().Count();
    }

    /// <summary>
    /// The remote all where.
    /// </summary>
    /// <param name="objectStore">
    /// The object store.
    /// </param>
    /// <param name="whereFunc">
    /// The where function.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public static void RemoveOf<T>(
      [NotNull] this IObjectStore objectStore, [NotNull] Func<KeyValuePair<string, T>, bool> whereFunc)
    {
      CodeContracts.VerifyNotNull(objectStore, "objectStore");
      CodeContracts.VerifyNotNull(whereFunc, "whereFunc");

      foreach (var i in objectStore.GetAll<T>().Where(whereFunc).ToList())
      {
        objectStore.Remove(i.Key);
      }
    }

    /// <summary>
    /// The remote all where.
    /// </summary>
    /// <param name="objectStore">
    /// The object store.
    /// </param>
    /// <param name="whereFunc">
    /// The where function.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public static void Remove(
      [NotNull] this IObjectStore objectStore, [NotNull] Func<string, bool> whereFunc)
    {
      CodeContracts.VerifyNotNull(objectStore, "objectStore");
      CodeContracts.VerifyNotNull(whereFunc, "whereFunc");

      foreach (var i in objectStore.GetAll<object>().Where(k => whereFunc(k.Key)).ToList())
      {
        objectStore.Remove(i.Key);
      }
    }

    #endregion
  }
}