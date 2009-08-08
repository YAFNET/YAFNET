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
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;

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
			ReplaceRules rules = YafContext.Current.Cache[key] as ReplaceRules;

			if (rules == null)
			{
				// doesn't exist, create a new instance class...
				rules = new ReplaceRules();

				// cache this value
				YafContext.Current.Cache.Add(key, rules, null, DateTime.Now.AddMinutes(YafContext.Current.BoardSettings.ReplaceRulesCacheTimeout), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default, null);
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
			YafContext.Current.Cache.Remove
				(
					delegate(string key)
					{
						return key.StartsWith(match);
					}
				);
		}
	}
}
