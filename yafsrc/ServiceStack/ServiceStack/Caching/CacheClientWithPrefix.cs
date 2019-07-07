using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceStack.Caching
{
    /// <summary>
    /// Decorates the ICacheClient (and its sibblings) prefixing every key with the given prefix
    /// 
    /// Usefull for multi-tenant environments
    /// </summary>
    public class CacheClientWithPrefix : ICacheClient, ICacheClientExtended, IRemoveByPattern
    {
        private readonly ICacheClient cache;

        public CacheClientWithPrefix(ICacheClient cache, string prefix)
        {
            this.Prefix = prefix;
            this.cache = cache;
        }

        public bool Remove(string key)
        {
            return cache.Remove(this.Prefix + key);
        }

        public T Get<T>(string key)
        {
            return cache.Get<T>(this.Prefix + key);
        }

        public long Increment(string key, uint amount)
        {
            return cache.Increment(this.Prefix + key, amount);
        }

        public long Decrement(string key, uint amount)
        {
            return cache.Decrement(this.Prefix + key, amount);
        }

        public bool Add<T>(string key, T value)
        {
            return cache.Add(this.Prefix + key, value);
        }

        public bool Set<T>(string key, T value)
        {
            return cache.Set(this.Prefix + key, value);
        }

        public bool Replace<T>(string key, T value)
        {
            return cache.Replace(this.Prefix + key, value);
        }

        public void SetAll<T>(IDictionary<string, T> values)
        {
            cache.SetAll(values.ToDictionary(x => this.Prefix + x.Key, x => x.Value));
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            return cache.GetAll<T>(keys.Select(x => this.Prefix + x));
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return cache.Replace(this.Prefix + key, value, expiresIn);
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return cache.Set(this.Prefix + key, value, expiresIn);
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            return cache.Add(this.Prefix + key, value, expiresIn);
        }

        public bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            return cache.Replace(this.Prefix + key, value, expiresAt);
        }

        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            return cache.Set(this.Prefix + key, value, expiresAt);
        }

        public bool Add<T>(string key, T value, DateTime expiresAt)
        {
            return cache.Add(this.Prefix + key, value, expiresAt);
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            cache.RemoveAll(keys.Select(x => this.Prefix + x));
        }

        public void FlushAll()
        {
            // Cannot be prefixed
            cache.FlushAll();
        }

        public void Dispose()
        {
            cache.Dispose();
        }

        public void RemoveByPattern(string pattern)
        {
            (cache as IRemoveByPattern)?.RemoveByPattern(this.Prefix + pattern);
        }

        public void RemoveByRegex(string regex)
        {
            (cache as IRemoveByPattern)?.RemoveByRegex(this.Prefix + regex);
        }

        public IEnumerable<string> GetKeysByPattern(string pattern)
        {
            return (cache as ICacheClientExtended)?.GetKeysByPattern(this.Prefix + pattern);
        }

        public TimeSpan? GetTimeToLive(string key)
        {
            return (cache as ICacheClientExtended)?.GetTimeToLive(key);
        }
        
        public string Prefix { get; }
    }

    public static class CacheClientWithPrefixExtensions
    {
        /// <summary>
        /// Decorates the ICacheClient (and its sibblings) prefixing every key with the given prefix
        /// 
        /// Usefull for multi-tenant environments
        /// </summary>
        public static ICacheClient WithPrefix(this ICacheClient cache, string prefix)
        {
            return new CacheClientWithPrefix(cache, prefix);            
        }
    }

}
