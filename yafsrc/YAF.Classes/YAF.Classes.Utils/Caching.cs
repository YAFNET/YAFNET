using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;

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


		public void Insert(string key, object value)
		{
			_cache.Insert(CreateKey(key), value);
		}
		public void Insert(string key, object value, CacheDependency dependencies)
		{
			_cache.Insert(CreateKey(key), value, dependencies);
		}
		public void Insert(string key, object value, CacheDependency dependencies, 
			DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
			_cache.Insert(CreateKey(key), value, dependencies, absoluteExpiration, slidingExpiration);
		}
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
