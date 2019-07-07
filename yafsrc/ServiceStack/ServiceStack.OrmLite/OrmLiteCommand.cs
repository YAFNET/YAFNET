using System.Data;
using ServiceStack.Data;

namespace ServiceStack.OrmLite
{
    public class OrmLiteCommand : IDbCommand, IHasDbCommand
    {
        private OrmLiteConnection dbConn;

        public IOrmLiteDialectProvider DialectProvider;
        public bool IsDisposed { get; private set; }

        public OrmLiteCommand(OrmLiteConnection dbConn, IDbCommand dbCmd)
        {
            this.dbConn = dbConn;
            this.DbCommand = dbCmd;
            this.DialectProvider = dbConn.GetDialectProvider();
        }

        public void Dispose()
        {
            IsDisposed = true;
            this.DbCommand.Dispose();
        }

        public void Prepare()
        {
            this.DbCommand.Prepare();
        }

        public void Cancel()
        {
            this.DbCommand.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return this.DbCommand.CreateParameter();
        }

        public int ExecuteNonQuery()
        {
            return this.DbCommand.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            return this.DbCommand.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return this.DbCommand.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            return this.DbCommand.ExecuteScalar();
        }

        public IDbConnection Connection
        {
            get => this.DbCommand.Connection;
            set => this.DbCommand.Connection = value;
        }
        public IDbTransaction Transaction
        {
            get => this.DbCommand.Transaction;
            set => this.DbCommand.Transaction = value;
        }
        public string CommandText
        {
            get => this.DbCommand.CommandText;
            set => this.DbCommand.CommandText = value;
        }
        public int CommandTimeout
        {
            get => this.DbCommand.CommandTimeout;
            set => this.DbCommand.CommandTimeout = value;
        }
        public CommandType CommandType
        {
            get => this.DbCommand.CommandType;
            set => this.DbCommand.CommandType = value;
        }
        public IDataParameterCollection Parameters => this.DbCommand.Parameters;

        public UpdateRowSource UpdatedRowSource
        {
            get => this.DbCommand.UpdatedRowSource;
            set => this.DbCommand.UpdatedRowSource = value;
        }

        public IDbCommand DbCommand { get; }
    }
}