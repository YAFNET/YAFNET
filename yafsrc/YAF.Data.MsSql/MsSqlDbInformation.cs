/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Data.MsSql
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using YAF.Configuration;
    using YAF.Core.Data;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// MS SQL DB Information
    /// </summary>
    public class MsSqlDbInformation : IDbInformation
    {
        /// <summary>
        /// The install script list
        /// </summary>
        private static readonly string[] InstallScriptList =
            {
                "mssql/install/tables.sql", 
                "mssql/install/indexes.sql", 
                "mssql/install/views.sql",
                "mssql/install/constraints.sql", 
                "mssql/install/functions.sql", 
                "mssql/install/procedures.sql",
            };

        /// <summary>
        /// The upgrade script list
        /// </summary>
        private static readonly string[] UpgradeScriptList =
            {
                "mssql/upgrade/tables.sql", 
                "mssql/upgrade/indexes.sql", 
                "mssql/upgrade/views.sql",
                "mssql/upgrade/constraints.sql", 
                "mssql/upgrade/triggers.sql",
                "mssql/upgrade/functions.sql", 
                "mssql/upgrade/procedures.sql"
            };

        /// <summary>
        /// The YAF Provider Upgrade script list
        /// </summary>
        private static readonly string[] IdentityUpgradeScriptList =
            {
                "mssql/upgrade/identity/upgrade.sql"
            };

        /// <summary>
        /// The DB parameters
        /// </summary>
        private readonly DbConnectionParam[] dbParameters =
        {
            new DbConnectionParam(0, "Password", string.Empty),
            new DbConnectionParam(1, "Data Source", "(local)"),
            new DbConnectionParam(2, "Initial Catalog", string.Empty),
            new DbConnectionParam(11, "Use Integrated Security", "true")
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDbInformation"/> class.
        /// </summary>
        public MsSqlDbInformation()
        {
            this.ConnectionString = () => Config.ConnectionString;
            this.ProviderName = MsSqlDbAccess.ProviderTypeName;
        }

        /// <summary>
        /// Gets or sets the DB Connection String
        /// </summary>
        public Func<string> ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the DB Provider Name
        /// </summary>
        public string ProviderName { get; protected set; }

        /// <summary>
        /// Gets Full Text Upgrade Script.
        /// </summary>
        public string FullTextUpgradeScript => "mssql/upgrade/fulltext.sql";

        /// <summary>
        /// Gets the Install Script List.
        /// </summary>
        public IEnumerable<string> InstallScripts => InstallScriptList;

        /// <summary>
        /// Gets the Upgrade Script List.
        /// </summary>
        public IEnumerable<string> UpgradeScripts => UpgradeScriptList;

        /// <summary>
        /// Gets the YAF Provider Upgrade Script List.
        /// </summary>
        public IEnumerable<string> IdentityUpgradeScripts => IdentityUpgradeScriptList;

        /// <summary>
        /// Gets the DB Connection Parameters.
        /// </summary>
        public IDbConnectionParam[] DbConnectionParameters => this.dbParameters.OfType<IDbConnectionParam>().ToArray();

        /// <summary>
        /// Builds a connection string.
        /// </summary>
        /// <param name="parameters">The Connection Parameters</param>
        /// <returns>Returns the Connection String</returns>
        public string BuildConnectionString([NotNull] IEnumerable<IDbConnectionParam> parameters)
        {
            CodeContracts.VerifyNotNull(parameters, "parameters");

            var connBuilder = new SqlConnectionStringBuilder();

            parameters.ForEach(param => connBuilder[param.Name] = param.Value);

            return connBuilder.ConnectionString;
        }
    }
}