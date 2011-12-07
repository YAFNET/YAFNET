// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The collection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Types.Interfaces.Extensions
{
	using System.Collections.Generic;
	using System.Linq;

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
			CodeContracts.ArgumentNotNull(dictionary, "dictionary");

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
		public static void AddRange<TKey, TValue>(
			[NotNull] this IDictionary<TKey, TValue> dictionaryFirst, [NotNull] IDictionary<TKey, TValue> dictionarySecondary)
		{
			CodeContracts.ArgumentNotNull(dictionaryFirst, "dictionaryFirst");
			CodeContracts.ArgumentNotNull(dictionarySecondary, "dictionarySecondary");

			dictionarySecondary.ToList().ForEach(i => dictionaryFirst.AddOrUpdate(i.Key, i.Value));
		}

		/// <summary>
		/// The get value.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <typeparam name="TKey">
		/// </typeparam>
		/// <typeparam name="TValue">
		/// </typeparam>
		/// <returns>
		/// </returns>
		public static TValue GetValue<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			CodeContracts.ArgumentNotNull(dictionary, "dictionary");

			return GetValueOrDefault(dictionary, key, default(TValue));
		}

		/// <summary>
		/// The get value or default.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <param name="defaultValue">
		/// The default value.
		/// </param>
		/// <typeparam name="TKey">
		/// </typeparam>
		/// <typeparam name="TValue">
		/// </typeparam>
		/// <returns>
		/// </returns>
		public static TValue GetValueOrDefault<TKey, TValue>(
			[NotNull] this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			CodeContracts.ArgumentNotNull(dictionary, "dictionary");

			TValue returnValue;

			if (dictionary.TryGetValue(key, out returnValue))
			{
				return returnValue;
			}

			return defaultValue;
		}

		#endregion
	}
}