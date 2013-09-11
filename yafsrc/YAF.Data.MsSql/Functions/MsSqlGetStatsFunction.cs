/* Yet Another Forum.NET
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

using System.Linq;
using YAF.Types.Extensions;

namespace YAF.Data.MsSql
{
    using System;
    using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Text;

	using YAF.Types;
	using YAF.Types.Attributes;
	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Data;

    /// <summary>
	/// The ms sql get stats function.
	/// </summary>
    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
	public class MsSqlGetStatsFunction : BaseMsSqlFunction
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MsSqlGetStatsFunction"/> class.
		/// </summary>
		/// <param name="dbAccess">
		/// The db access.
		/// </param>
		public MsSqlGetStatsFunction([NotNull] IDbAccess dbAccess)
			: base(dbAccess)
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		///   Gets SortOrder.
		/// </summary>
		public override int SortOrder
		{
			get
			{
				return 1000;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// The supported operation.
		/// </summary>
		/// <param name="operationName">
		/// The operation name.
		/// </param>
		/// <returns>
		/// True if the operation is supported.
		/// </returns>
		public override bool IsSupportedOperation([NotNull] string operationName)
		{
			return operationName.Equals("getstats", StringComparison.InvariantCultureIgnoreCase);
		}

		#endregion

		#region Methods

		/// <summary>
		/// The run operation.
		/// </summary>
		/// <param name="sqlConnection">
		/// The sql connection.
		/// </param>
		/// <param name="dbTransaction">
		/// The db unit of work.
		/// </param>
		/// <param name="dbfunctionType">
		/// The dbfunction type.
		/// </param>
		/// <param name="operationName">
		/// The operation name.
		/// </param>
		/// <param name="parameters">
		/// The parameters.
		/// </param>
		/// <param name="result">
		/// The result.
		/// </param>
		/// <returns>
		/// The run operation.
		/// </returns>
		protected override bool RunOperation(
			SqlConnection sqlConnection, 
			IDbTransaction dbTransaction, 
			DbFunctionType dbfunctionType, 
			string operationName, 
			IEnumerable<KeyValuePair<string, object>> parameters, 
			out object result)
		{
			// create statistic SQL...
			var sb = new StringBuilder();

			sb.AppendLine("DECLARE @TableName sysname");
			sb.AppendLine("DECLARE cur_showfragmentation CURSOR FOR");
			sb.AppendLine("SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{objectQualifier}%'");
			sb.AppendLine("OPEN cur_showfragmentation");
			sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
			sb.AppendLine("WHILE @@FETCH_STATUS = 0");
			sb.AppendLine("BEGIN");
			sb.AppendLine("DBCC SHOWCONTIG (@TableName)");
			sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
			sb.AppendLine("END");
			sb.AppendLine("CLOSE cur_showfragmentation");
			sb.AppendLine("DEALLOCATE cur_showfragmentation");

            using (var cmd = this.DbAccess.GetCommand(sb.ToString(), CommandType.Text))
			{
				this.DbAccess.ExecuteNonQuery(cmd, dbTransaction);
			}

		    result = this._sqlMessages.Select(s => s.Message).ToDelimitedString("\r\n");

			return true;
		}

		#endregion
	}
}