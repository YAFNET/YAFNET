/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Utils
{
	#region Using

	using System;
	using System.Collections.Generic;
	using System.Linq;

	using YAF.Types;

	#endregion

	/// <summary>
	/// The enumerable extensions.
	/// </summary>
	public static class EnumerableExtensions
	{
		#region Public Methods and Operators

		/// <summary>
		/// Iterates through all the elements in the IEnumerable of 
		/// <typeparamref name="T"/> performing the <paramref name="action"/> for each
		/// element.
		/// </summary>
		/// <typeparam name="T">
		/// </typeparam>
		/// <param name="currentEnumerable">
		/// </param>
		/// <param name="action">
		/// </param>
		public static void ForEach<T>([NotNull] this IEnumerable<T> currentEnumerable, [NotNull] Action<T> action)
		{
			CodeContracts.ArgumentNotNull(currentEnumerable, "currentEnumerable");
			CodeContracts.ArgumentNotNull(action, "action");

			foreach (var item in ToList(currentEnumerable))
			{
				action(item);
			}
		}

		/// <summary>
		/// Internal function used to convert an IEnumerable ToList without doing duplicate work if the IEnumerable is already IList.
		/// </summary>
		/// <param name="currentEnumerable"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static IList<T> ToList<T>(IEnumerable<T> currentEnumerable)
		{
			IList<T> currentList = null;

			if (currentEnumerable is IList<T>)
			{
				currentList = currentEnumerable as IList<T>;
			}
			else
			{
				currentList = new List<T>(currentEnumerable);
			}

			return currentList;
		}

		/// <summary>
		/// Iterates through all the elements in the IEnumerable of 
		/// <typeparamref name="T"/> performing the <paramref name="action"/> for each
		/// element with a IsFirst flag.
		/// </summary>
		/// <typeparam name="T">
		/// </typeparam>
		/// <param name="currentEnumerable">
		/// </param>
		/// <param name="action">
		/// </param>
		public static void ForEachFirst<T>([NotNull] this IEnumerable<T> currentEnumerable, [NotNull] Action<T, bool> action)
		{
			CodeContracts.ArgumentNotNull(currentEnumerable, "currentEnumerable");
			CodeContracts.ArgumentNotNull(action, "action");

			bool isFirst = true;
			foreach (var item in ToList(currentEnumerable))
			{
				action(item, isFirst);
				isFirst = false;
			}
		}

		/// <summary>
		/// Iterates through all the elements in the IEnumerable of 
		/// <typeparamref name="T"/> performing the <paramref name="action"/> for each
		/// element with an index.
		/// </summary>
		/// <typeparam name="T">
		/// </typeparam>
		/// <param name="currentEnumerable">
		/// </param>
		/// <param name="action">
		/// </param>
		public static void ForEachIndex<T>([NotNull] this IEnumerable<T> currentEnumerable, [NotNull] Action<T, int> action)
		{
			CodeContracts.ArgumentNotNull(currentEnumerable, "currentEnumerable");
			CodeContracts.ArgumentNotNull(action, "action");

			int i = 0;
			foreach (var item in ToList(currentEnumerable))
			{
				action(item, i++);
			}
		}

		/// <summary>
		/// Creates an infinite IEnumerable from the <paramref name="currentEnumerable"/> padding it with default(<typeparamref name="T"/>).
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
			CodeContracts.ArgumentNotNull(currentEnumerable, "currentEnumerable");

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
		/// If the <paramref name="currentEnumerable"/> is <see langword="null"/>, an
		/// Empty IEnumerable of <typeparamref name="T"/> is returned, else 
		/// <paramref name="currentEnumerable"/> is returned.
		/// </summary>
		/// <param name="currentEnumerable">
		/// The current enumerable.
		/// </param>
		/// <typeparam name="T">
		/// </typeparam>
		/// <returns>
		/// </returns>
		public static IEnumerable<T> IfNullEmpty<T>([CanBeNull] this IEnumerable<T> currentEnumerable)
		{
			if (currentEnumerable == null)
			{
				return Enumerable.Empty<T>();
			}

			return currentEnumerable;
		}

		/// <summary>
		/// Converts an <see cref="IEnumerable"/> to a HashSet -- similar to ToList()
		/// </summary>
		/// <param name="list">
		/// The currentEnumerable. 
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
			CodeContracts.ArgumentNotNull(list, "currentEnumerable");

			return new HashSet<T>(list);
		}

		#endregion
	}
}