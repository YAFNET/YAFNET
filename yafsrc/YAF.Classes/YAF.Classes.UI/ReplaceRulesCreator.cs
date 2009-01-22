using System;
using System.Collections.Generic;
using System.Text;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.UI
{
	/// <summary>
	/// Gets an instance of replace rules and uses
	/// caching if possible.
	/// </summary>
	public static class ReplaceRulesCreator
	{
		/// <summary>
		/// Gets relace rules instance for given flags.
		/// </summary>
		/// <param name="uniqueFlags">Flags identifying replace rules.</param>
		/// <returns>ReplaceRules for given unique flags.</returns>
		public static ReplaceRules GetInstance(bool[] uniqueFlags)
		{
			// convert flags to integer
			int rulesFlags = FlagsBase.GetIntFromBoolArray(uniqueFlags);

			// cache is board-specific since boards may have different custom BB Code...
			string key = YafCache.GetBoardCacheKey( String.Format(Constants.Cache.ReplaceRules, rulesFlags) );

			// try to get rules from the cache
			ReplaceRules rules = YafCache.Current[key] as ReplaceRules;

			if (rules == null)
			{
				// doesn't exist, create a new instance class...
				rules = new ReplaceRules();

				// cache this value
				YafCache.Current.Add(key, rules, null, DateTime.Now.AddMinutes(YafContext.Current.BoardSettings.ReplaceRulesCacheTimeout), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default, null);
			}

			return rules;
		}

		/// <summary>
		/// Clears all ReplaceRules from the cache.
		/// </summary>
		public static void ClearCache()
		{
			// match starting part of cache key
			string match = String.Format(Constants.Cache.ReplaceRules, "");

			// remove it entries from cache
			YafCache.Current.Remove
				(
					delegate(string key)
					{
						return key.StartsWith(match);
					}
				);
		}
	}
}
