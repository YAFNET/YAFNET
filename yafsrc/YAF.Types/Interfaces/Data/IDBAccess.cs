// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDBAccess.cs" company="">
//   
// </copyright>
// <summary>
//   DBAccess Interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Types.Interfaces
{
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;

	/// <summary>
	/// DBAccess Interface
	/// </summary>
	public interface IDbAccess
	{
		#region Properties

		/// <summary>
		///   Filter list of result filters.
		/// </summary>
		IList<IDataTableResultFilter> ResultFilterList { get; }

		#endregion

		#region Public Methods

		/// <summary>
		/// The execute non query.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		void ExecuteNonQuery([NotNull] IDbCommand cmd, bool transaction);

		/// <summary>
		/// The execute scalar.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The execute scalar.
		/// </returns>
		object ExecuteScalar([NotNull] IDbCommand cmd, bool transaction);

		/// <summary>
		/// The get command.
		/// </summary>
		/// <param name="sql">
		/// The sql.
		/// </param>
		/// <param name="isStoredProcedure">
		/// The is stored procedure.
		/// </param>
		/// <param name="parameters">Command Parameters</param>
		/// <returns>
		/// </returns>
		DbCommand GetCommand(
			[NotNull] string sql,
			bool isStoredProcedure = true,
			[CanBeNull] IEnumerable<KeyValuePair<string, object>> parameters = null);

		/// <summary>
		/// Gets the current connection manager.
		/// </summary>
		/// <returns>
		/// </returns>
		IDbConnectionManager GetConnectionManager();

		/// <summary>
		/// The get data.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// </returns>
		DataTable GetData([NotNull] IDbCommand cmd, bool transaction);

		/// <summary>
		/// The get data.
		/// </summary>
		/// <param name="commandText">
		/// The command text.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// </returns>
		DataTable GetData([NotNull] string commandText, bool transaction);

		/// <summary>
		/// The get dataset.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// </returns>
		DataSet GetDataset([NotNull] IDbCommand cmd, bool transaction);

		/// <summary>
		/// The get reader.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <returns>
		/// </returns>
		IDataReader GetReader([NotNull] IDbCommand cmd);

		/// <summary>
		/// Sets the connection manager adapter.
		/// </summary>
		/// <typeparam name="TManager">
		/// </typeparam>
		void SetConnectionManagerAdapter<TManager>() where TManager : IDbConnectionManager;

		#endregion
	}
}