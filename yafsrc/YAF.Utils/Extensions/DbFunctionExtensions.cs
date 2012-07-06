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
namespace YAF.Utils.Extensions
{
	#region Using

	using System;
	using System.Collections.Generic;

	using YAF.Types;
	using YAF.Types.Interfaces;

	#endregion

	/// <summary>
	/// The db function extensions.
	/// </summary>
	public static class DbFunctionExtensions
	{
		#region Public Methods

		/// <summary>
		/// The get data typed.
		/// </summary>
		/// <param name="dbFunction">
		/// The db function.
		/// </param>
		/// <param name="function">
		/// The function.
		/// </param>
		/// <param name="comparer">
		/// The comparer.
		/// </param>
		/// <typeparam name="T">
		/// </typeparam>
		/// <returns>
		/// </returns>
		[CanBeNull]
		public static IEnumerable<T> GetDataTyped<T>(
			[NotNull] this IDbFunction dbFunction, 
			[NotNull] Func<object, object> function, 
			[CanBeNull] IEqualityComparer<string> comparer = null) where T : IDataLoadable, new()
		{
			CodeContracts.ArgumentNotNull(dbFunction, "dbFunction");
			CodeContracts.ArgumentNotNull(function, "function");

			return dbFunction.GetData(function).Typed<T>(comparer);
		}

		/// <summary>
		/// The get scalar as.
		/// </summary>
		/// <param name="dbFunction">
		/// The db function.
		/// </param>
		/// <param name="function">
		/// The function.
		/// </param>
		/// <typeparam name="T">
		/// </typeparam>
		/// <returns>
		/// </returns>
		[CanBeNull]
		public static T GetScalar<T>([NotNull] this IDbFunction dbFunction, [NotNull] Func<object, object> function)
		{
			CodeContracts.ArgumentNotNull(dbFunction, "dbFunction");
			CodeContracts.ArgumentNotNull(function, "function");

			return ((object)function(dbFunction.Scalar)).ToType<T>();
		}

		#endregion
	}
}