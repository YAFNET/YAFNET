/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
namespace YAF.Core.Services.Cache
{
  #region Using

    using System;
    using System.Collections.Generic;
    using System.Runtime.Caching;
    using System.Web.Caching;

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;

    #endregion

  /// <summary>
  /// The http runtime cache -- uses HttpRuntime cache to store cache information.
  /// </summary>
  public class HttpRuntimeCache : IDataCache
  {
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
      get => this.Get(key);

      set => this.Set(key, value);
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

      foreach (var item in MemoryCache.Default)
      {
          if (!item.Key.StartsWith($"{Constants.Cache.YafCacheKey}$"))
          {
              continue;
          }

          if (!isObject && !(item.Value is T))
          {
              continue;
          }

          // key parts are YAFCACHE$[Provided Name]$[Possibily More]"
          var key = item.Key.Split('$')[1];

          yield return new KeyValuePair<string, T>(key, (T)item.Value);
      }
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
        c =>
        {
            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + timeout,
                SlidingExpiration = Cache.NoSlidingExpiration,
                Priority = System.Runtime.Caching.CacheItemPriority.Default,
                RemovedCallback = (k) => this._eventRaiser.Raise(new CacheItemRemovedEvent(k))
            };
            MemoryCache.Default.Add(
                new CacheItem(this.CreateKey(key)) { Value = c },
                cacheItemPolicy);
        });
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
          c =>
          {
              var cacheItemPolicy = new CacheItemPolicy
              {
                  AbsoluteExpiration = Cache.NoAbsoluteExpiration,
                  SlidingExpiration = Cache.NoSlidingExpiration,
                  Priority = System.Runtime.Caching.CacheItemPriority.Default,
                  RemovedCallback = (k) => this._eventRaiser.Raise(new CacheItemRemovedEvent(k))
              };
              MemoryCache.Default.Add(
                  new CacheItem(this.CreateKey(key)) { Value = c },
                  cacheItemPolicy);
          });
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
          if (MemoryCache.Default[actualKey] != null)
          {
              MemoryCache.Default.Remove(actualKey);
          }

          var cacheItemPolicy = new CacheItemPolicy
          {
              AbsoluteExpiration = DateTime.Now + timeout,
              SlidingExpiration = Cache.NoSlidingExpiration,
              Priority = System.Runtime.Caching.CacheItemPriority.Default,
              RemovedCallback = k => this._eventRaiser.Raise(new CacheItemRemovedEvent(k))
          };

          MemoryCache.Default.Add(new CacheItem(actualKey) { Value = value }, cacheItemPolicy);
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
    /// The <see cref="object"/>.
    /// </returns>
    public object Get([NotNull] string originalKey)
    {
      CodeContracts.VerifyNotNull(originalKey, "key");

      return MemoryCache.Default[this.CreateKey(originalKey)];
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
        MemoryCache.Default.Remove(this.CreateKey(key));
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

      MemoryCache.Default[this.CreateKey(key)] = value;
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
    /// The <see cref="string"/>.
    /// </returns>
    private string CreateKey([NotNull] string key)
    {
        key = key.Replace('$', '.');
        return this._treatCacheKey.Treat($"{Constants.Cache.YafCacheKey}${key}");
    }

    /// <summary>
    /// The get or set internal.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
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
    /// The <see cref="T"/>.
    /// </returns>
    private T GetOrSetInternal<T>([NotNull] string key, [NotNull] Func<T> getValue, [NotNull] Action<T> addToCacheFunction)
    {
      CodeContracts.VerifyNotNull(key, "key");
      CodeContracts.VerifyNotNull(getValue, "getValue");
      CodeContracts.VerifyNotNull(addToCacheFunction, "addToCacheFunction");

      var cachedItem = this.Get<T>(key);

      if (!Equals(cachedItem, default(T)))
      {
          return cachedItem;
      }

      lock (this._haveLockObject.Get(this.CreateKey(key)))
      {
          // now that we're on lockdown, try one more time...
          cachedItem = this.Get<T>(key);

          if (!Equals(cachedItem, default(T)))
          {
              return cachedItem;
          }

          // materialize the query
          cachedItem = getValue();

          if (!Equals(cachedItem, default(T)))
          {
              addToCacheFunction(cachedItem);
          }
      }

      return cachedItem;
    }

    #endregion
  }
}