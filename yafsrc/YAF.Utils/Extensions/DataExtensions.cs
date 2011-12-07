// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The data extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Utils.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using YAF.Classes;
	using YAF.Types;
	using YAF.Types.Interfaces;

	/// <summary>
	/// The data extensions.
	/// </summary>
	public static class DataExtensions
	{
		#region Public Methods

		/// <summary>
		/// Gets qualified object name
		/// </summary>
		/// <param name="name">
		/// Base name of an object
		/// </param>
		/// <returns>
		/// Returns qualified object name of format {databaseOwner}.{objectQualifier}name
		/// </returns>
		public static string GetObjectName([NotNull] string name)
		{
			return "[{0}].[{1}{2}]".FormatWith(Config.DatabaseOwner, Config.DatabaseObjectQualifier, name);
		}

		/// <summary>
		/// The to dictionary.
		/// </summary>
		/// <param name="reader">
		/// The reader.
		/// </param>
		/// <param name="comparer">
		/// The comparer.
		/// </param>
		/// <returns>
		/// </returns>
		public static IEnumerable<IDictionary<string, object>> ToDictionary(
			[NotNull] this IDataReader reader, [CanBeNull] IEqualityComparer<string> comparer = null)
		{
			CodeContracts.ArgumentNotNull(reader, "reader");

			while (reader.Read())
			{
				var rowDictionary = new Dictionary<string, object>(comparer ?? StringComparer.OrdinalIgnoreCase);

				for (int fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)
				{
					rowDictionary.Add(reader.GetName(fieldIndex), reader.GetValue(fieldIndex));
				}

				yield return rowDictionary;
			}

			yield break;
		}

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
		[NotNull]
		public static IEnumerable<IDictionary<string, object>> ToDictionary(
			[NotNull] this DataTable dataTable, [CanBeNull] IEqualityComparer<string> comparer = null)
		{
			CodeContracts.ArgumentNotNull(dataTable, "dataTable");

			var columns = dataTable.Columns.OfType<DataColumn>().Select(c => c.ColumnName);

			return
				dataTable.AsEnumerable().Select(
					dataRow =>
					columns.ToDictionary(
						k => k, v => dataRow[v] == DBNull.Value ? null : dataRow[v], comparer ?? StringComparer.OrdinalIgnoreCase)).Cast
					<IDictionary<string, object>>();
		}

		/// <summary>
		/// The typed.
		/// </summary>
		/// <param name="dataTable">
		/// The data table.
		/// </param>
		/// <param name="comparer">
		/// The comparer.
		/// </param>
		/// <typeparam name="T">
		/// </typeparam>
		/// <returns>
		/// </returns>
		[NotNull]
		public static IEnumerable<T> Typed<T>(
			[NotNull] this DataTable dataTable, [CanBeNull] IEqualityComparer<string> comparer = null)
			where T : IDataLoadable, new()
		{
			return dataTable.ToDictionary(comparer).Select(
				d =>
					{
						var newObj = new T();
						newObj.LoadFromDictionary(d);
						return newObj;
					});
		}

		#endregion
	}
}