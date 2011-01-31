namespace YAF.Types.Interfaces
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;

  #endregion

  /// <summary>
  /// The i data cache extensions.
  /// </summary>
  public static class IDataCacheExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    /// <param name="originalKey">
    /// The original key.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this IDataCache dataCache, [NotNull] string originalKey)
    {
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");
      CodeContracts.ArgumentNotNull(originalKey, "originalKey");

      var item = dataCache.Get(originalKey);

      if (item is T)
      {
        return (T)item;
      }

      return default(T);
    }

    /// <summary>
    /// The remote all.
    /// </summary>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public static void RemoveOf<T>([NotNull] this IDataCache dataCache)
    {
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");

      foreach (var i in dataCache.GetAll<T>().ToList())
      {
        dataCache.Remove(i.Key);
      }
    }

    /// <summary>
    /// Clear the entire cache.
    /// </summary>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    public static void Clear([NotNull] this IDataCache dataCache)
    {
      // remove all objects in the cache...
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");

      dataCache.RemoveOf<object>();
    }

    /// <summary>
    /// Count of objects in the cache.
    /// </summary>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    public static int Count([NotNull] this IDataCache dataCache)
    {
      // remove all objects in the cache...
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");

      return dataCache.GetAll<object>().Count();
    }

    /// <summary>
    /// Count of T in the cache.
    /// </summary>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    public static int CountOf<T>([NotNull] this IDataCache dataCache)
    {
      // remove all objects in the cache...
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");

      return dataCache.GetAll<T>().Count();
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
    public static void RemoveOf<T>(
      [NotNull] this IDataCache dataCache, [NotNull] Func<KeyValuePair<string, T>, bool> whereFunc)
    {
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");
      CodeContracts.ArgumentNotNull(whereFunc, "whereFunc");

      foreach (var i in dataCache.GetAll<T>().Where(whereFunc).ToList())
      {
        dataCache.Remove(i.Key);
      }
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
    public static void Remove(
      [NotNull] this IDataCache dataCache, [NotNull] Func<string, bool> whereFunc)
    {
      CodeContracts.ArgumentNotNull(dataCache, "dataCache");
      CodeContracts.ArgumentNotNull(whereFunc, "whereFunc");

      foreach (var i in dataCache.GetAll<object>().Where(k => whereFunc(k.Key)).ToList())
      {
        dataCache.Remove(i.Key);
      }
    }

    #endregion
  }
}