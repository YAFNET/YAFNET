using System;
using System.Collections.Generic;
using YAF.Classes;
using YAF.Core.Data;
using YAF.Types.Interfaces.Data;

namespace YAF.Data.MsSql
{
    public class MsSqlDbInformation : IDbInformation
    {
        public Func<string> ConnectionString { get; set; }

        public string ProviderName
        {
            get; protected set;
        }

        private static readonly string[] _scriptList =
        {
            "mssql/tables.sql",
            "mssql/indexes.sql",
            "mssql/views.sql", 
            "mssql/constraints.sql",
            "mssql/triggers.sql", 
            "mssql/functions.sql", 
            "mssql/procedures.sql",
            "mssql/providers/tables.sql", 
            "mssql/providers/indexes.sql", 
            "mssql/providers/procedures.sql"
        };

        protected DbConnectionParam[] DbParameters = new[]
        {
            new DbConnectionParam(0, "Password", string.Empty, true), new DbConnectionParam(1, "Data Source", "(local)", true), 
            new DbConnectionParam(2, "Initial Catalog", string.Empty, true), new DbConnectionParam(11, "Use Integrated Security", "true", true)
        };

        public string FullTextScript
        {
            get { return "mssql/fulltext.sql"; }
        }

        public IEnumerable<string> Scripts
        {
            get
            {
                return _scriptList;
            }
        }

        public IEnumerable<IDbConnectionParam> DbConnectionParameters
        {
            get
            {
                return this.DbParameters;
            }
        }

        public MsSqlDbInformation()
        {
            ConnectionString = () => Config.ConnectionString;
            ProviderName = MsSqlDbAccess.ProviderTypeName;
        }
    }
}