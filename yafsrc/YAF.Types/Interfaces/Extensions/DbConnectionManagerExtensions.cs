// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbConnectionManagerExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The db connection manager extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Types.Interfaces.Extensions
{
	using System.Data;

	using YAF.Types;
	using YAF.Types.Interfaces;

	/// <summary>
	/// The db connection manager extensions.
	/// </summary>
	public static class DbConnectionManagerExtensions
	{
		#region Public Methods

		/// <summary>
		/// Get an open db connection.
		/// </summary>
		/// <param name="dbConnectionManager">
		/// The db connection manager.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public static IDbConnection GetOpenDbConnection([NotNull] this IDbConnectionManager dbConnectionManager)
		{
			CodeContracts.ArgumentNotNull(dbConnectionManager, "dbConnectionManager");

			var connection = dbConnectionManager.DBConnection;

			if (connection.State != ConnectionState.Open)
			{
				// open it up...
				connection.Open();
			}

			return connection;
		}

		#endregion
	}
}