// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbAccessExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The i db access extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Types.Interfaces
{
	#region Using

	using System.Data;

	#endregion

	/// <summary>
	/// The i db access extensions.
	/// </summary>
	public static class IDbAccessExtensions
	{
		#region Public Methods

		/// <summary>
		/// The execute non query.
		/// </summary>
		/// <param name="dbAccess">
		/// The db access.
		/// </param>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		public static void ExecuteNonQuery([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd)
		{
			CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
			CodeContracts.ArgumentNotNull(cmd, "cmd");

			dbAccess.ExecuteNonQuery(cmd, false);
		}

		/// <summary>
		/// The execute scalar.
		/// </summary>
		/// <param name="dbAccess">
		/// The db access.
		/// </param>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <returns>
		/// The execute scalar.
		/// </returns>
		public static object ExecuteScalar([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd)
		{
			CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
			CodeContracts.ArgumentNotNull(cmd, "cmd");

			return dbAccess.ExecuteScalar(cmd, false);
		}

		/// <summary>
		/// The get data.
		/// </summary>
		/// <param name="dbAccess">
		/// The db access.
		/// </param>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <returns>
		/// </returns>
		public static DataTable GetData([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd)
		{
			CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
			CodeContracts.ArgumentNotNull(cmd, "cmd");

			return dbAccess.GetData(cmd, false);
		}

		/// <summary>
		/// The get data.
		/// </summary>
		/// <param name="dbAccess">
		/// The db access.
		/// </param>
		/// <param name="sql">
		/// The sql.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// </returns>
		public static DataTable GetData([NotNull] this IDbAccess dbAccess, [NotNull] string sql, bool transaction)
		{
			CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
			CodeContracts.ArgumentNotNull(sql, "sql");

			var cmd = dbAccess.GetCommand(sql);

			return dbAccess.GetData(cmd, transaction);
		}

		/// <summary>
		/// The get dataset.
		/// </summary>
		/// <param name="dbAccess">
		/// The db access.
		/// </param>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <returns>
		/// </returns>
		public static DataSet GetDataset([NotNull] this IDbAccess dbAccess, [NotNull] IDbCommand cmd)
		{
			CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");
			CodeContracts.ArgumentNotNull(cmd, "cmd");

			return dbAccess.GetDataset(cmd, false);
		}

		#endregion
	}
}