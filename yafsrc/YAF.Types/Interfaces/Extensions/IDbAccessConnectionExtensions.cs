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
namespace YAF.Types.Interfaces.Extensions
{
	#region Using

	using System.Data;
	using System.Data.Common;

	using YAF.Types.Interfaces.Data;

    #endregion

	/// <summary>
	/// The db connection extensions.
	/// </summary>
	public static class IDbAccessConnectionExtensions
	{
		#region Public Methods

		/// <summary>
		/// The create connection.
		/// </summary>
		/// <param name="dbAccess">
		/// The db access.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public static DbConnection CreateConnection([NotNull] this IDbAccessV2 dbAccess)
		{
			CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");

			var connection = dbAccess.DbProviderFactory.CreateConnection();
			connection.ConnectionString = dbAccess.ConnectionString;

			return connection;
		}

		/// <summary>
		/// Get an open db connection.
		/// </summary>
		/// <param name="dbAccess">
		/// The db Access.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public static DbConnection CreateConnectionOpen([NotNull] this IDbAccessV2 dbAccess)
		{
			CodeContracts.ArgumentNotNull(dbAccess, "dbAccess");

			var connection = dbAccess.CreateConnection();

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