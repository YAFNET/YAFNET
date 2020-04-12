/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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

      return default;
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

        objectStore.GetAll<T>().ToList().ForEach(i => objectStore.Remove(i.Key));
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

        objectStore.GetAll<T>().Where(whereFunc).ToList().ForEach(i => objectStore.Remove(i.Key));
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

        objectStore.GetAll<object>().Where(k => whereFunc(k.Key)).ToList().ForEach(i => objectStore.Remove(i.Key));
    }

    #endregion
  }
}