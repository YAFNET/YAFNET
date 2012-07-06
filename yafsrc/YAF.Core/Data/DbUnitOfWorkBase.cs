/* Yet Another Forum.NET
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
namespace YAF.Core.Data
{
	#region Using

	using System.Data;
	using System.Data.Common;

	using YAF.Types;
	using YAF.Types.Interfaces;

	#endregion

	/// <summary>
	/// The db unit of work base.
	/// </summary>
	public class DbUnitOfWorkBase : IDbUnitOfWork
	{
		#region Constants and Fields

		/// <summary>
		/// The _connection.
		/// </summary>
		private readonly DbConnection _connection;

		/// <summary>
		/// The _transaction.
		/// </summary>
		private readonly DbTransaction _transaction;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DbUnitOfWorkBase"/> class.
		/// </summary>
		/// <param name="connection">
		/// The connection.
		/// </param>
		/// <param name="isolationLevel">
		/// The isolation level.
		/// </param>
		public DbUnitOfWorkBase(
			[NotNull] DbConnection connection, IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
		{
			CodeContracts.ArgumentNotNull(connection, "connection");

			this._connection = connection;
			this._transaction = this._connection.BeginTransaction(isolationLevel);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets Transaction.
		/// </summary>
		public DbTransaction Transaction
		{
			get
			{
				return this._transaction;
			}
		}

		#endregion

		#region Implemented Interfaces

		#region IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			if (this.Transaction != null)
			{
				this.Transaction.Dispose();
			}

			if (this._connection != null)
			{
				if (this._connection.State == ConnectionState.Open)
				{
					this._connection.Close();
				}

				this._connection.Dispose();
			}
		}

		#endregion

		#endregion
	}
}