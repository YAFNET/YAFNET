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