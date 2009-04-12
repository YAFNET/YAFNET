/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Collections;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Caching helper class
	/// </summary>
	public class YafCache
	{
		#region Static Members

		/// <summary>
		/// Default instance
		/// </summary>
		private static YafCache _currentInstance;


		/// <summary>
		/// Default static constructor
		/// </summary>
		static YafCache()
		{
			// create new instance as a default cache instance
			_currentInstance = new YafCache();
		}


		/// <summary>
		/// Gets current instance of YafCache class
		/// </summary>
		public static YafCache Current
		{
			get
			{
				return _currentInstance;
			}
		}


		/// <summary>
		/// Creates string to be used as key for caching, board-specific
		/// </summary>
		/// <param name="key">Key we need to make board-specific</param>
		/// <returns>Board sepecific cache key based on key parameter</returns>
		public static string GetBoardCacheKey(string key)
		{
			// use current YafCache instance as source of board ID
			return GetBoardCacheKey(key, YafContext.Current);
		}
		/// <summary>
		/// Creates string to be used as key for caching, board-specific
		/// </summary>
		/// <param name="key">Key we need to make board-specific</param>
		/// <param name="context">Context from which to determine board ID</param>
		/// <returns>Board sepecific cache key based on key parameter</returns>
		public static string GetBoardCacheKey(string key, YafContext context)
		{
			return GetBoardCacheKey(key, context.PageBoardID);
		}
		/// <summary>
		/// Creates string to be used as key for caching, board-specific
		/// </summary>
		/// <param name="key">Key we need to make board-specific</param>
		/// <param name="boardID">Board ID to use when creating cache key</param>
		/// <returns>Board sepecific cache key based on key parameter</returns>
		public static string GetBoardCacheKey(string key, int boardID)
		{
			return String.Format("{0}.{1}", key, boardID);
		}

		#endregion


		#region Instantious Members

		// cache object to work with
		private Cache _cache;
		// default method for creating cache keys
		private CacheKeyCreationMethod _keyCreationMethod;


		/// <summary>
		/// Default constuctor uses HttpContext.Current as source for obtaining Cache object
		/// </summary>
		public YafCache() : this(HttpContext.Current.Cache) { }
		/// <summary>
		/// Initializes class with specified Cache object
		/// </summary>
		/// <param name="cache">Cache to work with</param>
		public YafCache(Cache cache)
		{
			_cache = cache;
			// use straigt method as default
			_keyCreationMethod = CacheKeyCreationMethod.Straight;
		}


		/// <summary>
		/// Indexer for obtaining and setting cache keys
		/// </summary>
		/// <param name="key">Cache key to get or set</param>
		/// <returns>Value cached under specified key</returns>
		public object this [string key]
		{
			get
			{
				return _cache[CreateKey(key)];
			}
			set
			{
				_cache[CreateKey(key)] = value;
			}
		}

		/// <summary>
		/// Gets default cache creation method
		/// </summary>
		public CacheKeyCreationMethod KeyCreationMethod
		{
			get
			{
				return _keyCreationMethod;
			}
			/*set
			{
				_keyCreationMethod = value;
			}*/
		}

		/// <summary>
		/// Gets count of cache keys currently saved.
		/// </summary>
		public int Count
		{
			get { return _cache.Count; }
		}


		/// <summary>
		/// Creates key using default cache key creation method
		/// </summary>
		/// <param name="key">Key to use for generating cache key</param>
		/// <returns>Returns cache key</returns>
		public string CreateKey(string key)
		{
			// output depends on cache key creation method
			switch (_keyCreationMethod)
			{
				case CacheKeyCreationMethod.BoardSpecific:
					// return board specific key
					return GetBoardCacheKey(key);
				case CacheKeyCreationMethod.Straight:
				default:
					// return same value as was supplied in parameter
					return key;
			}
		}


		/// <summary>
		/// Adds item to the cache.
		/// </summary>
		/// <param name="key">Key identifying item in cache.</param>
		/// <param name="value">Cached value.</param>
		/// <param name="dependencies">Cache dependencies, invalidating cache.</param>
		/// <param name="absoluteExpiration">Absolute expiration date. When used, sliding expiration has to be set to Cache.NoSlidingExpiration.</param>
		/// <param name="slidingExpiration">Sliding expiration of cache item. When used, absolute expiration has to be set to Cache.NoAbsoluteExpiration.</param>
		/// <param name="priority">Relative cost of object in cache. When system evicts objects from cache, objects with lower cost are removed first.</param>
		/// <param name="onRemoveCallback">Delegate that is called upon cache item remova. Can be null.</param>
		/// <returns>Cached item.</returns>
		public object Add(string key, object value, CacheDependency dependencies,
			DateTime absoluteExpiration, TimeSpan slidingExpiration,
			CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
		{
			return _cache.Add(
				CreateKey(key), 
				value, 
				dependencies, 
				absoluteExpiration,
				slidingExpiration,
				priority, 
				onRemoveCallback
				);
		}


		/// <summary>
		/// Inserts item to the cache.
		/// </summary>
		/// <param name="key">Key identifying item in cache.</param>
		/// <param name="value">Cached value.</param>
		public void Insert(string key, object value)
		{
			_cache.Insert(CreateKey(key), value);
		}
		/// <summary>
		/// Inserts item to the cache.
		/// </summary>
		/// <param name="key">Key identifying item in cache.</param>
		/// <param name="value">Cached value.</param>
		/// <param name="dependencies">Cache dependencies, invalidating cache.</param>
		public void Insert(string key, object value, CacheDependency dependencies)
		{
			_cache.Insert(CreateKey(key), value, dependencies);
		}
		/// <summary>
		/// Inserts item to the cache.
		/// </summary>
		/// <param name="key">Key identifying item in cache.</param>
		/// <param name="value">Cached value.</param>
		/// <param name="dependencies">Cache dependencies, invalidating cache.</param>
		/// <param name="absoluteExpiration">Absolute expiration date. When used, sliding expiration has to be set to Cache.NoSlidingExpiration.</param>
		/// <param name="slidingExpiration">Sliding expiration of cache item. When used, absolute expiration has to be set to Cache.NoAbsoluteExpiration.</param>
		public void Insert(string key, object value, CacheDependency dependencies, 
			DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			_cache.Insert(CreateKey(key), value, dependencies, absoluteExpiration, slidingExpiration);
		}
		/// <summary>
		/// Inserts item to the cache.
		/// </summary>
		/// <param name="key">Key identifying item in cache.</param>
		/// <param name="value">Cached value.</param>
		/// <param name="dependencies">Cache dependencies, invalidating cache.</param>
		/// <param name="absoluteExpiration">Absolute expiration date. When used, sliding expiration has to be set to Cache.NoSlidingExpiration.</param>
		/// <param name="slidingExpiration">Sliding expiration of cache item. When used, absolute expiration has to be set to Cache.NoAbsoluteExpiration.</param>
		/// <param name="priority">Relative cost of object in cache. When system evicts objects from cache, objects with lower cost are removed first.</param>
		/// <param name="onRemoveCallback">Delegate that is called upon cache item remova. Can be null.</param>
		/// <returns>Cached item.</returns>
		public void Insert(string key, object value, CacheDependency dependencies,
			DateTime absoluteExpiration, TimeSpan slidingExpiration,
			CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
		{
			_cache.Insert(
				CreateKey(key), 
				value, 
				dependencies, 
				absoluteExpiration, 
				slidingExpiration, 
				priority, 
				onRemoveCallback
				);
		}


		/// <summary>
		/// Removes specified key from cache
		/// </summary>
		/// <param name="key">Key to remove</param>
		/// <returns>Value removed from cache, null if no such key was cached</returns>
		public object Remove(string key)
		{
			return _cache.Remove(CreateKey(key));
		}
		/// <summary>
		/// Removes all keys for which given predicate returns true.
		/// </summary>
		/// <param name="predicate">Predicate for matching cache keys.</param>
		public void Remove(Predicate<string> predicate)
		{
			// get enumarator
			IDictionaryEnumerator key = _cache.GetEnumerator();

			// cycle through cache keys
			while (key.MoveNext())
			{
				// remove cache item if predicate returns true
				if (predicate(key.Key.ToString())) _cache.Remove(key.Key.ToString());
			}
		}


		/// <summary>
		/// Clear all cache entries from memory.
		/// </summary>
		public void Clear()
		{
			// get enumarator
			IDictionaryEnumerator key = _cache.GetEnumerator();

			// cycle through cache keys
			while (key.MoveNext())
			{
				// and remove them one by one
				_cache.Remove(key.Key.ToString());
			}
		}

		#endregion
	}


	/// <summary>
	/// Enumeration of methods for generating cache keys
	/// </summary>
	public enum CacheKeyCreationMethod
	{
		Straight,
		BoardSpecific
	}
}
