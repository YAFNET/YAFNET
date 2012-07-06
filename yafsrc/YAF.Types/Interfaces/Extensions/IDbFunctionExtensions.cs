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
namespace YAF.Types.Interfaces
{
	#region Using

	using System;
	using System.Data;

	#endregion

	/// <summary>
	/// The i db function extensions.
	/// </summary>
	public static class IDbFunctionExtensions
	{
		#region Public Methods

		/// <summary>
		/// The get data.
		/// </summary>
		/// <param name="dbFunction">
		/// The db function.
		/// </param>
		/// <param name="function">
		/// The function.
		/// </param>
		/// <returns>
		/// </returns>
		[CanBeNull]
		public static DataTable GetData([NotNull] this IDbFunction dbFunction, [NotNull] Func<object, object> function)
		{
			CodeContracts.ArgumentNotNull(dbFunction, "dbFunction");
			CodeContracts.ArgumentNotNull(function, "function");

			return (DataTable)function(dbFunction.GetData);
		}

		/// <summary>
		/// The get data set.
		/// </summary>
		/// <param name="dbFunction">
		/// The db function.
		/// </param>
		/// <param name="function">
		/// The function.
		/// </param>
		/// <returns>
		/// </returns>
		[CanBeNull]
		public static DataSet GetDataSet([NotNull] this IDbFunction dbFunction, [NotNull] Func<object, object> function)
		{
			CodeContracts.ArgumentNotNull(dbFunction, "dbFunction");
			CodeContracts.ArgumentNotNull(function, "function");

			return (DataSet)function(dbFunction.GetDataSet);
		}

		#endregion
	}
}