using System;
using System.Collections.Generic;
using System.Data;

namespace ServiceStack.OrmLite.Dapper
{
    using System.Data.SqlClient;

    using Microsoft.SqlServer.Server;

    /// <summary>
    /// Used to pass a IEnumerable&lt;SqlDataRecord&gt; as a SqlDataRecordListTVPParameter
    /// </summary>
    internal sealed class SqlDataRecordListTVPParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly IEnumerable<SqlDataRecord> data;
        private readonly string typeName;

        /// <summary>
        /// Create a new instance of <see cref="SqlDataRecordListTVPParameter"/>.
        /// </summary>
        /// <param name="data">The data records to convert into TVPs.</param>
        /// <param name="typeName">The parameter type name.</param>
        public SqlDataRecordListTVPParameter(IEnumerable<SqlDataRecord> data, string typeName)
        {
            this.data = data;
            this.typeName = typeName;
        }

        void SqlMapper.ICustomQueryParameter.AddParameter(IDbCommand command, string name)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            Set(param, data, typeName);
            command.Parameters.Add(param);
        }

        internal static void Set(IDbDataParameter parameter, IEnumerable<SqlDataRecord> data, string typeName)
        {
            parameter.Value = (object)data ?? DBNull.Value;
            if (parameter is SqlParameter sqlParam)
            {
                sqlParam.SqlDbType = SqlDbType.Structured;
                sqlParam.TypeName = typeName;
            }
        }
    }
}
