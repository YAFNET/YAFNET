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
namespace YAF.Classes.Data
{
	using System;
	using System.Data;
	using System.Data.Common;

	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Data;

    /// <summary>
	/// The db access extensions.
	/// </summary>
	public static class DbAccessV2Extensions
	{
		#region Public Methods

		/// <summary>
		/// The replace command text.
		/// </summary>
		/// <param name="dbCommand">
		/// The db command.
		/// </param>
		public static DbCommand ReplaceCommandText([NotNull] this DbCommand dbCommand)
		{
            var commandText = dbCommand.CommandText;

            commandText = commandText.Replace("{databaseOwner}", Config.DatabaseOwner);
            commandText = commandText.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);

		    dbCommand.CommandText = commandText;

			return dbCommand;
		}

		/// <summary>
		/// Test the DB Connection.
		/// </summary>
		/// <param name="dbAccess">
		/// The db Access.
		/// </param>
		/// <param name="exceptionMessage">
		/// outbound ExceptionMessage
		/// </param>
		/// <returns>
		/// true if successfully connected
		/// </returns>
		public static bool TestConnection([NotNull] this IDbAccess dbAccess, [NotNull] out string exceptionMessage)
		{
			exceptionMessage = string.Empty;
			bool success = false;

			try
			{
				using (var connection = dbAccess.CreateConnectionOpen())
				{
					// we're connected!
					var conn = connection;
				}

				// success
				success = true;
			}
			catch (DbException x)
			{
				// unable to connect...
				exceptionMessage = x.Message;
			}

			return success;
		}

		#endregion
	}
}