// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlGetStatsFunction.cs" company="">
//   
// </copyright>
// <summary>
//   The ms sql get stats function.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Data.MsSql
{
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Text;

	using YAF.Types;
	using YAF.Types.Interfaces;

	/// <summary>
	/// The ms sql get stats function.
	/// </summary>
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
		public bool IsSupportedOperation([NotNull] string operationName)
		{
			return operationName.Equals("getstats");
		}

		#endregion

		#region Methods

		/// <summary>
		/// The run operation.
		/// </summary>
		/// <param name="sqlConnection">
		/// The sql connection.
		/// </param>
		/// <param name="dbUnitOfWork">
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
			IDbUnitOfWork dbUnitOfWork, 
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

			using (var cmd = this.DbAccess.GetCommand(sb.ToString(), false))
			{
				this.DbAccess.ExecuteNonQuery(cmd, dbUnitOfWork);
			}

			// no result...
			result = null;

			return true;
		}

		#endregion
	}
}