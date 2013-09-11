// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseMsSqlFunction.cs" company="">
//   
// </copyright>
// <summary>
//   The base ms sql function.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Data.MsSql
{
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	using ServiceStack.OrmLite;

	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Data;

	/// <summary>
	/// The base ms sql function.
	/// </summary>
	public abstract class BaseMsSqlFunction : IDbSpecificFunction
	{
		#region Constants and Fields

		/// <summary>
		///   The _sql messages.
		/// </summary>
		protected List<SqlInfoMessageEventArgs> _sqlMessages = new List<SqlInfoMessageEventArgs>();

		/// <summary>
		///   The _message.
		/// </summary>
		private IList<string> _message;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseMsSqlFunction"/> class.
		/// </summary>
		/// <param name="dbAccess">
		/// The db access.
		/// </param>
		public BaseMsSqlFunction([NotNull] IDbAccess dbAccess)
		{
			this.DbAccess = dbAccess;
		}

		#endregion

		#region Public Properties

		/// <summary>
		///   Gets or sets DbAccess.
		/// </summary>
		public IDbAccess DbAccess { get; set; }

		/// <summary>
		///   Gets ProviderName.
		/// </summary>
		[NotNull]
		public virtual string ProviderName
		{
			get
			{
			    return MsSqlDbAccess.ProviderTypeName;
			}
		}

		/// <summary>
		///   Gets SortOrder.
		/// </summary>
		public abstract int SortOrder { get; }

		/// <summary>
		///   Gets SqlMessages.
		/// </summary>
		[NotNull]
		public IList<SqlInfoMessageEventArgs> SqlMessages
		{
			get
			{
				return this._sqlMessages;
			}
		}

		#endregion

		#region Public Methods

	    /// <summary>
	    /// The execute.
	    /// </summary>
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
	    /// <param name="transaction"></param>
	    /// <returns>
	    /// The execute.
	    /// </returns>
	    public virtual bool Execute(
			DbFunctionType dbfunctionType, 
			[NotNull] string operationName, 
			[NotNull] IEnumerable<KeyValuePair<string, object>> parameters, 
			[CanBeNull] out object result,
            IDbTransaction transaction = null)
		{
			if (this.IsSupportedOperation(operationName))
			{
				this._sqlMessages.Clear();

			    bool createdTransaction = transaction == null;

			    try
			    {
                    if (transaction == null)
                    {
                        transaction = this.DbAccess.BeginTransaction();
                    }

                    if (transaction.Connection is SqlConnection)
                    {
                        var sqlConnection = transaction.Connection as SqlConnection;
                        sqlConnection.FireInfoMessageEventOnUserErrors = true;
                        sqlConnection.InfoMessage += new SqlInfoMessageEventHandler(this.sqlConnection_InfoMessage);

                        var operationSuccessful = this.RunOperation(sqlConnection, transaction, dbfunctionType, operationName, parameters, out result);

                        if (createdTransaction && operationSuccessful)
                        {
                            transaction.Commit();
                        }

                        return operationSuccessful;
                    }
			    }
			    finally
			    {
                    if (createdTransaction && transaction != null)
			        {
                        transaction.Dispose();
			        }
			    }
			}

			result = null;

			return false;
		}

		/// <summary>
		/// The supported operation.
		/// </summary>
		/// <param name="operationName">
		/// The operation name.
		/// </param>
		/// <returns>
		/// True if the operation is supported.
		/// </returns>
		public abstract bool IsSupportedOperation([NotNull] string operationName);

		#endregion

		#region Methods

		/// <summary>
		/// The run operation.
		/// </summary>
		/// <param name="sqlConnection">
		/// The sql connection.
		/// </param>
		/// <param name="dbTransaction">
		/// The unit Of Work.
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
		protected abstract bool RunOperation(
			[NotNull] SqlConnection sqlConnection,
            [NotNull] IDbTransaction dbTransaction, 
			DbFunctionType dbfunctionType, 
			[NotNull] string operationName, 
			[NotNull] IEnumerable<KeyValuePair<string, object>> parameters, 
			[CanBeNull] out object result);

		/// <summary>
		/// The sql connection_ info message.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void sqlConnection_InfoMessage([NotNull] object sender, [NotNull] SqlInfoMessageEventArgs e)
		{
			this._sqlMessages.Add(e);
		}

		#endregion
	}
}