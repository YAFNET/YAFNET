/* Yet Another Forum.net
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
namespace YAF.Core.Services.Cache
{
  #region Using

    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Caching;

    using YAF.Types;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The http runtime cache -- uses HttpRuntime cache to store cache information.
  /// </summary>
  public class HttpRuntimeCache : IDataCache
  {
    private const string YafCacheKey = "YAFCACHE";

    #region Constants and Fields

    /// <summary>
    ///   The _event raiser.
    /// </summary>
    private readonly IRaiseEvent _eventRaiser;

    /// <summary>
    ///   The _have lock object.
    /// </summary>
    private readonly IHaveLockObject _haveLockObject;

    /// <summary>
    ///   The _treat cache key.
    /// </summary>
    private readonly ITreatCacheKey _treatCacheKey;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRuntimeCache"></see>
    ///   class.
    /// </summary>
    /// <param name="eventRaiser">
    /// The event raiser.
    /// </param>
    /// <param name="haveLockObject">
    /// The have lock object.
    /// </param>
    /// <param name="treatCacheKey">
    /// The treat Cache Key.
    /// </param>
    public HttpRuntimeCache(
      [NotNull] IRaiseEvent eventRaiser, 
      [NotNull] IHaveLockObject haveLockObject, 
      [NotNull] ITreatCacheKey treatCacheKey)
    {
      CodeContracts.VerifyNotNull(eventRaiser, "eventRaiser");
      CodeContracts.VerifyNotNull(haveLockObject, "haveLockObject");

      this._eventRaiser = eventRaiser;
      this._haveLockObject = haveLockObject;
      this._treatCacheKey = treatCacheKey;
    }

    #endregion

    #region Indexers

    /// <summary>
    ///   The this.
    /// </summary>
    /// <param name = "key">
    ///   The key.
    /// </param>
    public object this[[NotNull] string key]
    {
      get
      {
        return this.Get(key);
      }

      set
      {
        this.Set(key, value);
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IDataCache

    /// <summary>
    /// The get all.
    /// </summary>
    /// <returns>
    /// </returns>
    [NotNull]
    public IEnumerable<KeyValuePair<string, T>> GetAll<T>()
    {
      var isObject = typeof(T) == typeof(object);
      var dictionaryEnumerator = HttpRuntime.Cache.GetEnumerator();

      while (dictionaryEnumerator.MoveNext())
      {
        if (!dictionaryEnumerator.Key.ToString().StartsWith(YafCacheKey + "$"))
        {
          continue;
        }

        if (!isObject && !(dictionaryEnumerator.Value is T))
        {
          continue;
        }

        // key parts are YAFCACHE$[Provided Name]$[Possibily More]"
        var key = dictionaryEnumerator.Key.ToString().Split('$')[1];

        yield return new KeyValuePair<string, T>(key, (T)dictionaryEnumerator.Value);
      }

      yield break;
    }

    /// <summary>
    /// The get or set.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <param name="timeout">
    /// The timeout.
    /// </param>
    /// <returns>
    /// </returns>
    public T GetOrSet<T>([NotNull] string key, [NotNull] Func<T> getValue, TimeSpan timeout)
    {
      CodeContracts.VerifyNotNull(key, "key");
      CodeContracts.VerifyNotNull(getValue, "getValue");

      return this.GetOrSetInternal(
        key, 
        getValue, 
        (c) =>
        HttpRuntime.Cache.Add(
          this.CreateKey(key), 
          c, 
          null, 
          DateTime.Now + timeout, 
          Cache.NoSlidingExpiration, 
          CacheItemPriority.Default, 
          (k, v, r) => this._eventRaiser.Raise(new CacheItemRemovedEvent(k, v, r))));
    }

    /// <summary>
    /// The get or set.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <returns>
    /// </returns>
    public T GetOrSet<T>(string key, Func<T> getValue)
    {
      CodeContracts.VerifyNotNull(key, "key");
      CodeContracts.VerifyNotNull(getValue, "getValue");

      return this.GetOrSetInternal(
        key, 
        getValue, 
        (c) =>
        HttpRuntime.Cache.Add(
          this.CreateKey(key), 
          c, 
          null, 
          Cache.NoAbsoluteExpiration, 
          Cache.NoSlidingExpiration, 
          CacheItemPriority.Default, 
          (k, v, r) => this._eventRaiser.Raise(new CacheItemRemovedEvent(k, v, r))));
    }

    /// <summary>
    /// The set.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="timeout">
    /// The timeout.
    /// </param>
    public void Set([NotNull] string key, object value, TimeSpan timeout)
    {
      CodeContracts.VerifyNotNull(key, "key");

      var actualKey = this.CreateKey(key);

      lock (this._haveLockObject.Get(actualKey))
      {
        if (HttpRuntime.Cache[actualKey] != null)
        {
          HttpRuntime.Cache.Remove(actualKey);
        }

        HttpRuntime.Cache.Add(
          actualKey, 
          value, 
          null, 
          DateTime.Now + timeout, 
          Cache.NoSlidingExpiration, 
          CacheItemPriority.Default, 
          (k, v, r) => this._eventRaiser.Raise(new CacheItemRemovedEvent(k, v, r)));
      }
    }

    #endregion

    #region IReadValue<T>

    /// <summary>
    /// The get.
    /// </summary>
    /// <param name="originalKey">
    /// The key.
    /// </param>
    /// <returns>
    /// </returns>
    public object Get([NotNull] string originalKey)
    {
      CodeContracts.VerifyNotNull(originalKey, "key");

      return HttpRuntime.Cache[this.CreateKey(originalKey)] ?? null;
    }

    #endregion

    #region IRemoveValue

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    public void Remove([NotNull] string key)
    {
      HttpRuntime.Cache.Remove(this.CreateKey(key));
    }

    #endregion

    #region IWriteValue<T>

    /// <summary>
    /// The set.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void Set([NotNull] string key, [CanBeNull] object value)
    {
      CodeContracts.VerifyNotNull(key, "key");

      HttpRuntime.Cache[this.CreateKey(key)] = value;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The create key.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The create key.
    /// </returns>
    private string CreateKey([NotNull] string key)
    {
      return this._treatCacheKey.Treat("{0}${1}".FormatWith(YafCacheKey, key.Replace('$', '.')));
    }

    /// <summary>
    /// The get or set internal.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <param name="addToCacheFunction">
    /// The add to cache function.
    /// </param>
    /// <returns>
    /// </returns>
    private T GetOrSetInternal<T>([NotNull] string key, [NotNull] Func<T> getValue, [NotNull] Action<T> addToCacheFunction)
    {
      CodeContracts.VerifyNotNull(key, "key");
      CodeContracts.VerifyNotNull(getValue, "getValue");
      CodeContracts.VerifyNotNull(addToCacheFunction, "addToCacheFunction");

      var cachedItem = this.Get<T>(key);

      if (Equals(cachedItem, default(T)))
      {
        lock (this._haveLockObject.Get(this.CreateKey(key)))
        {
          // now that we're on lockdown, try one more time...
          cachedItem = this.Get<T>(key);

          if (Equals(cachedItem, default(T)))
          {
            // materialize the query
            cachedItem = getValue();

            if (!Equals(cachedItem, default(T)))
            {
              addToCacheFunction(cachedItem);
            }
          }
        }
      }

      return cachedItem;
    }

    #endregion
  }
}