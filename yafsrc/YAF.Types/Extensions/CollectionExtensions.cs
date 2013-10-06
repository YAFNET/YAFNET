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
namespace YAF.Types.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Types;

    /// <summary>
	/// The collection extensions.
	/// </summary>
	public static class CollectionExtensions
	{
		#region Public Methods

		/// <summary>
		/// The add or update.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <param name="value">
		/// The value.
		/// </param>
		/// <typeparam name="TKey">
		/// </typeparam>
		/// <typeparam name="TValue">
		/// </typeparam>
		public static void AddOrUpdate<TKey, TValue>(
			[NotNull] this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			CodeContracts.VerifyNotNull(dictionary, "dictionary");

			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}

		/// <summary>
		/// The add range.
		/// </summary>
		/// <param name="dictionaryFirst">
		/// The dictionary first.
		/// </param>
		/// <param name="dictionarySecondary">
		/// The dictionary secondary.
		/// </param>
		/// <typeparam name="TKey">
		/// </typeparam>
		/// <typeparam name="TValue">
		/// </typeparam>
		public static void AddRange<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionaryFirst, [NotNull] IDictionary<TKey, TValue> dictionarySecondary)
		{
			CodeContracts.VerifyNotNull(dictionaryFirst, "dictionaryFirst");
			CodeContracts.VerifyNotNull(dictionarySecondary, "dictionarySecondary");

			dictionarySecondary.ToList().ForEach(i => dictionaryFirst.AddOrUpdate(i.Key, i.Value));
		}

		#endregion
	}
}