using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;

namespace YAF.Classes.Utils
{
	public class YafCache
	{
		#region Static Members

		private static YafCache _currentInstance;

		static YafCache()
		{
			_currentInstance = new YafCache();
		}

		public static YafCache Current
		{
			get
			{
				return _currentInstance;
			}
		}

		public static string GetBoardCacheKey(string key)
		{
			return GetBoardCacheKey(key, YafContext.Current);
		}
		public static string GetBoardCacheKey(string key, YafContext context)
		{
			return GetBoardCacheKey(key, context.PageBoardID);
		}
		public static string GetBoardCacheKey(string key, int boardID)
		{
			return String.Format("{0}.{1}", key, boardID);
		}

		#endregion


		#region Instantious Members

		private Cache _cache;
		private CacheKeyCreationMethod _keyCreationMethod;

		public YafCache() : this(HttpContext.Current.Cache) { }
		public YafCache(Cache cache)
		{
			_cache = cache;
			_keyCreationMethod = CacheKeyCreationMethod.Straight;
		}

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

		public string CreateKey(string key)
		{
			switch (_keyCreationMethod)
			{
				case CacheKeyCreationMethod.BoardSpecific:
					return GetBoardCacheKey(key);
				case CacheKeyCreationMethod.Straight:
				default:
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


		public object Remove(string key)
		{
			return _cache.Remove(CreateKey(key));
		}

		#endregion
	}

	public enum CacheKeyCreationMethod
	{
		Straight,
		BoardSpecific
	}
}
