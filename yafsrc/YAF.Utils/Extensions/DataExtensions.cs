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
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using YAF.Types;

	/// <summary>
	/// The data extensions.
	/// </summary>
	public static class DataExtensions
	{
		#region Public Methods

		/// <summary>
		/// The to dictionary.
		/// </summary>
		/// <param name="dataRow">
		/// The data row.
		/// </param>
		/// <param name="comparer">
		/// The comparer.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public static IDictionary<string, object> ToDictionary(
			[NotNull] this DataRow dataRow, [CanBeNull] IEqualityComparer<string> comparer = null)
		{
			CodeContracts.ArgumentNotNull(dataRow, "dataRow");

			return dataRow.Table.Columns.OfType<DataColumn>().Select(c => c.ColumnName).ToDictionary(
				k => k, v => dataRow[v] == DBNull.Value ? null : dataRow[v], comparer ?? StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// The to dictionary.
		/// </summary>
		/// <param name="dataTable">
		/// The data table.
		/// </param>
		/// <param name="comparer">
		/// The comparer.
		/// </param>
		/// <returns>
		/// </returns>
		public static IEnumerable<IDictionary<string, object>> ToDictionary([NotNull] this DataTable dataTable, [CanBeNull] IEqualityComparer<string> comparer = null)
		{
			CodeContracts.ArgumentNotNull(dataTable, "dataTable");

			var columns = dataTable.Columns.OfType<DataColumn>().Select(c => c.ColumnName);

			foreach (var dataRow in dataTable.AsEnumerable())
			{
				yield return
					columns.ToDictionary(
						k => k, v => dataRow[v] == DBNull.Value ? null : dataRow[v], comparer ?? StringComparer.OrdinalIgnoreCase);
			}
		}

		#endregion
	}
}