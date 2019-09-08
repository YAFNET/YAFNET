using System;
using System.Data;
using System.Data.Common;
using ServiceStack.Data;

namespace ServiceStack.MiniProfiler.Data
{
    /// <summary>
    /// Wraps a database connection, allowing sql execution timings to be collected when a <see cref="IDbProfiler"/> session is started.
    /// </summary>
    public class ProfiledConnection : DbConnection, IHasDbConnection
    {
        /// <summary>
        /// Returns a new <see cref="ProfiledConnection"/> that wraps <paramref name="connection"/>, 
        /// providing query execution profiling.  If profiler is null, no profiling will occur.
        /// </summary>
        /// <param name="connection">Your provider-specific flavor of connection, e.g. SqlConnection, OracleConnection</param>
        /// <param name="profiler">The currently started <see cref="IDbProfiler"/> or null.</param>
        /// <param name="autoDisposeConnection">Determines whether the ProfiledDbConnection will dispose the underlying connection.</param>
        public ProfiledConnection(DbConnection connection, IDbProfiler profiler, bool autoDisposeConnection = true)
        {
        	Init(connection, profiler, autoDisposeConnection);
        }

        private void Init(DbConnection connection, IDbProfiler profiler, bool autoDisposeConnection)
    	{
    		if (connection == null) throw new ArgumentNullException("connection");

    	    AutoDisposeConnection = autoDisposeConnection;
    		this.InnerConnection = connection;
    		this.InnerConnection.StateChange += StateChangeHandler;

    		if (profiler != null)
    		{
    			this.Profiler = profiler;
    		}
    	}

        public ProfiledConnection(IDbConnection connection, IDbProfiler profiler, bool autoDisposeConnection = true)
        {
            if (connection is IHasDbConnection hasConn) connection = hasConn.DbConnection;

            if (!(connection is DbConnection dbConn))
				throw new ArgumentException($"{connection.GetType().FullName} does not inherit DbConnection");
			
			Init(dbConn, profiler, autoDisposeConnection);
        }


#pragma warning disable 1591 // xml doc comments warnings

        /// <summary>
        /// The underlying, real database connection to your db provider.
        /// </summary>
        public DbConnection InnerConnection { get; protected set; }

        public IDbConnection DbConnection => this.InnerConnection;

        /// <summary>
        /// The current profiler instance; could be null.
        /// </summary>
        public IDbProfiler Profiler { get; protected set; }

        /// <summary>
        /// The raw connection this is wrapping
        /// </summary>
        public DbConnection WrappedConnection => this.InnerConnection;

        protected override bool CanRaiseEvents => true;

        public override string ConnectionString
        {
            get => this.InnerConnection.ConnectionString;
            set => this.InnerConnection.ConnectionString = value;
        }

        public override int ConnectionTimeout => this.InnerConnection.ConnectionTimeout;

        public override string Database => this.InnerConnection.Database;

        public override string DataSource => this.InnerConnection.DataSource;

        public override string ServerVersion => this.InnerConnection.ServerVersion;

        public override ConnectionState State => this.InnerConnection.State;

        protected bool AutoDisposeConnection { get; set; }

        public override void ChangeDatabase(string databaseName)
        {
            this.InnerConnection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            if (AutoDisposeConnection)
                this.InnerConnection.Close();
        }

		//public override void EnlistTransaction(System.Transactions.Transaction transaction)
		//{
		//    _conn.EnlistTransaction(transaction);
		//}
#if !NETSTANDARD2_0
        public override DataTable GetSchema()
        {
            return this.InnerConnection.GetSchema();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return this.InnerConnection.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return this.InnerConnection.GetSchema(collectionName, restrictionValues);
        }
#endif

        public override void Open()
        {
            if (this.InnerConnection.State != ConnectionState.Open)
                this.InnerConnection.Open();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new ProfiledDbTransaction(this.InnerConnection.BeginTransaction(isolationLevel), this);
        }

        protected override DbCommand CreateDbCommand()
        {
            return new ProfiledCommand(this.InnerConnection.CreateCommand(), this, this.Profiler);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.InnerConnection != null)
            {
                this.InnerConnection.StateChange -= StateChangeHandler;
                if (AutoDisposeConnection)
                    this.InnerConnection.Dispose();
            }
            this.InnerConnection = null;
            this.Profiler = null;
            base.Dispose(disposing);
        }

        void StateChangeHandler(object sender, StateChangeEventArgs e)
        {
            OnStateChange(e);
        }
    }
}

#pragma warning restore 1591 // xml doc comments warnings