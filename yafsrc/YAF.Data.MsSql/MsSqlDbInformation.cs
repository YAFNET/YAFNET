/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Collections.Generic;
using YAF.Classes;
using YAF.Core.Data;
using YAF.Types.Interfaces.Data;

namespace YAF.Data.MsSql
{
    using System.Data.SqlClient;
    using System.Linq;

    using YAF.Types;

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
            "mssql/forum_ns.sql",
            "mssql/providers/tables.sql",
            "mssql/providers/indexes.sql",
            "mssql/providers/procedures.sql"
        };

        protected DbConnectionParam[] _dbParameters = new[]
        {
            new DbConnectionParam(0, "Password", string.Empty), 
            new DbConnectionParam(1, "Data Source", "(local)"), 
            new DbConnectionParam(2, "Initial Catalog", string.Empty), 
            new DbConnectionParam(11, "Use Integrated Security", "true")
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

        public IDbConnectionParam[] DbConnectionParameters
        {
            get
            {
                return this._dbParameters.OfType<IDbConnectionParam>().ToArray();
            }
        }

        public string BuildConnectionString([NotNull] IEnumerable<IDbConnectionParam> parameters)
        {
            CodeContracts.VerifyNotNull(parameters, "parameters");

            var connBuilder = new SqlConnectionStringBuilder();

            foreach (var param in parameters)
            {
                connBuilder[param.Name] = param.Value;
            }

            return connBuilder.ConnectionString;
        }

        public MsSqlDbInformation()
        {
            ConnectionString = () => Config.ConnectionString;
            ProviderName = MsSqlDbAccess.ProviderTypeName;
        }
    }
}