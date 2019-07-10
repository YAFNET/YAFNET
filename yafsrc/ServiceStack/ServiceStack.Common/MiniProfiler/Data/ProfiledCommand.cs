using System;
using System.Data.Common;
using System.Data;
using ServiceStack.Data;

#pragma warning disable 1591 // xml doc comments warnings

namespace ServiceStack.MiniProfiler.Data
{
    public class ProfiledCommand : DbCommand, IHasDbCommand
    {
        private DbConnection _conn;
        private DbTransaction _tran;

        public ProfiledCommand(DbCommand cmd, DbConnection conn, IDbProfiler profiler)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");

            this.DbCommand = cmd;
            _conn = conn;

            if (profiler != null)
            {
                this.DbProfiler = profiler;
            }
        }

        public override string CommandText
        {
            get => this.DbCommand.CommandText;
            set => this.DbCommand.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => this.DbCommand.CommandTimeout;
            set => this.DbCommand.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => this.DbCommand.CommandType;
            set => this.DbCommand.CommandType = value;
        }

        public DbCommand DbCommand { get; protected set; }

        IDbCommand IHasDbCommand.DbCommand => DbCommand;

        protected override DbConnection DbConnection
        {
            get => _conn;
            set
            {
                _conn = value;
                this.DbCommand.Connection = !(value is ProfiledConnection awesomeConn) ? value : awesomeConn.WrappedConnection;
            }
        }

        protected override DbParameterCollection DbParameterCollection => this.DbCommand.Parameters;

        protected override DbTransaction DbTransaction
        {
            get => _tran;
            set
            {
                this._tran = value;
                this.DbCommand.Transaction = !(value is ProfiledDbTransaction awesomeTran) || !(awesomeTran.DbTransaction is DbTransaction) ?
                    value : (DbTransaction)awesomeTran.DbTransaction;
            }
        }

        protected IDbProfiler DbProfiler { get; set; }

        public override bool DesignTimeVisible
        {
            get => this.DbCommand.DesignTimeVisible;
            set => this.DbCommand.DesignTimeVisible = value;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => this.DbCommand.UpdatedRowSource;
            set => this.DbCommand.UpdatedRowSource = value;
        }


        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (this.DbProfiler == null || !this.DbProfiler.IsActive)
            {
                return this.DbCommand.ExecuteReader(behavior);
            }

            DbDataReader result = null;
            this.DbProfiler.ExecuteStart(this, ExecuteType.Reader);
            try
            {
                result = this.DbCommand.ExecuteReader(behavior);
                result = new ProfiledDbDataReader(result, _conn, this.DbProfiler);
            }
            catch (Exception e)
            {
                this.DbProfiler.OnError(this, ExecuteType.Reader, e);
                throw;
            }
            finally
            {
                this.DbProfiler.ExecuteFinish(this, ExecuteType.Reader, result);
            }
            return result;
        }

        public override int ExecuteNonQuery()
        {
            if (this.DbProfiler == null || !this.DbProfiler.IsActive)
            {
                return this.DbCommand.ExecuteNonQuery();
            }

            int result;

            this.DbProfiler.ExecuteStart(this, ExecuteType.NonQuery);
            try
            {
                result = this.DbCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                this.DbProfiler.OnError(this, ExecuteType.NonQuery, e);
                throw;
            }
            finally
            {
                this.DbProfiler.ExecuteFinish(this, ExecuteType.NonQuery, null);
            }
            return result;
        }

        public override object ExecuteScalar()
        {
            if (this.DbProfiler == null || !this.DbProfiler.IsActive)
            {
                return this.DbCommand.ExecuteScalar();
            }

            object result;
            this.DbProfiler.ExecuteStart(this, ExecuteType.Scalar);
            try
            {
                result = this.DbCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                this.DbProfiler.OnError(this, ExecuteType.Scalar, e);
                throw;
            }
            finally
            {
                this.DbProfiler.ExecuteFinish(this, ExecuteType.Scalar, null);
            }
            return result;
        }

        public override void Cancel()
        {
            this.DbCommand.Cancel();
        }

        public override void Prepare()
        {
            this.DbCommand.Prepare();
        }

        protected override DbParameter CreateDbParameter()
        {
            return this.DbCommand.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DbCommand?.Dispose();
            }
            this.DbCommand = null;
            base.Dispose(disposing);
        }
    }
}

#pragma warning restore 1591 // xml doc comments warnings