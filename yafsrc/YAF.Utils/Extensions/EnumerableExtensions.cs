// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The enumerable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using YAF.Types;

	/// <summary>
	/// The enumerable extensions.
	/// </summary>
	public static class EnumerableExtensions
	{
		#region Public Methods

		/// <summary>
		/// Iterates through a generic list type
		/// </summary>
		/// <typeparam name="T">
		/// </typeparam>
		/// <param name="list">
		/// </param>
		/// <param name="action">
		/// </param>
		public static void ForEach<T>([NotNull] this IEnumerable<T> list, [NotNull] Action<T> action)
		{
			CodeContracts.ArgumentNotNull(list, "list");
			CodeContracts.ArgumentNotNull(action, "action");

			foreach (var item in list.ToList())
			{
				action(item);
			}
		}

		/// <summary>
		/// Iterates through a list with a isFirst flag.
		/// </summary>
		/// <typeparam name="T">
		/// </typeparam>
		/// <param name="list">
		/// </param>
		/// <param name="action">
		/// </param>
		public static void ForEachFirst<T>([NotNull] this IEnumerable<T> list, [NotNull] Action<T, bool> action)
		{
			CodeContracts.ArgumentNotNull(list, "list");
			CodeContracts.ArgumentNotNull(action, "action");

			bool isFirst = true;
			foreach (var item in list.ToList())
			{
				action(item, isFirst);
				isFirst = false;
			}
		}

		/// <summary>
		/// Iterates through a list with a index.
		/// </summary>
		/// <typeparam name="T">
		/// </typeparam>
		/// <param name="list">
		/// </param>
		/// <param name="action">
		/// </param>
		public static void ForEachIndex<T>([NotNull] this IEnumerable<T> list, [NotNull] Action<T, int> action)
		{
			CodeContracts.ArgumentNotNull(list, "list");
			CodeContracts.ArgumentNotNull(action, "action");

			int i = 0;
			foreach (var item in list.ToList())
			{
				action(item, i++);
			}
		}

		/// <summary>
		/// The infinite.
		/// </summary>
		/// <param name="currentEnumerable">
		/// The current enumerable.
		/// </param>
		/// <typeparam name="T">
		/// </typeparam>
		/// <returns>
		/// </returns>
		public static IEnumerable<T> Infinite<T>([NotNull] this IEnumerable<T> currentEnumerable)
		{
			foreach (var item in currentEnumerable)
			{
				yield return item;
			}

			while (true)
			{
				yield return default(T);
			}
		}

		/// <summary>
		/// Converts an <see cref="IEnumerable"/> to a HashSet -- similar to ToList()
		/// </summary>
		/// <param name="list">
		/// The list.
		/// </param>
		/// <typeparam name="T">
		/// </typeparam>
		/// <returns>
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		[NotNull]
		public static HashSet<T> ToHashSet<T>([NotNull] this IEnumerable<T> list)
		{
			CodeContracts.ArgumentNotNull(list, "list");

			return new HashSet<T>(list);
		}

		#endregion
	}
}