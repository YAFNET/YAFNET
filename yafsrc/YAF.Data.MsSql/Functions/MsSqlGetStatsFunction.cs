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

namespace YAF.Data.MsSql.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// The ms sql get stats function.
    /// </summary>
    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class MsSqlGetStatsFunction : BaseMsSqlFunction
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlGetStatsFunction"/> class.
        /// </summary>
        /// <param name="dbAccess">
        /// The db access.
        /// </param>
        public MsSqlGetStatsFunction([NotNull] IDbAccess dbAccess)
            : base(dbAccess)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets SortOrder.
        /// </summary>
        public override int SortOrder => 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// The supported operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <returns>
        /// True if the operation is supported.
        /// </returns>
        public override bool IsSupportedOperation([NotNull] string operationName)
        {
            return operationName.Equals("getstats", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// The get db type and size from string.
        /// </summary>
        /// <param name="providerData">
        ///  The provider data.
        /// </param>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The get db type and size from string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static bool GetDbTypeAndSizeFromString(string providerData, out SqlDbType dbType, out int size)
        {
            size = -1;
            dbType = SqlDbType.NVarChar;

            if (providerData.IsNotSet())
            {
                return false;
            }

            // split the data
            var chunk = providerData.Split(';');

            // get the data type and ignore case...
            dbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), chunk[1], true);

            if (chunk.Length <= 2)
            {
                return true;
            }

            // handle size...
            if (!int.TryParse(chunk[2], out size))
            {
                throw new ArgumentException($"Unable to parse as integer: {chunk[2]}");
            }

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The run operation.
        /// </summary>
        /// <param name="sqlConnection">The sql connection.</param>
        /// <param name="dbTransaction">The db unit of work.</param>
        /// <param name="dbfunctionType">The dbfunction type.</param>
        /// <param name="operationName">The operation name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The run operation.
        /// </returns>
        protected override bool RunOperation(
            SqlConnection sqlConnection,
            IDbTransaction dbTransaction,
            DBFunctionType dbfunctionType,
            string operationName,
            IEnumerable<KeyValuePair<string, object>> parameters,
            out object result)
        {
            // create statistic SQL...
            var sb = new StringBuilder();

            sb.AppendLine("DECLARE @TableName sysname");
            sb.AppendLine("DECLARE cur_showfragmentation CURSOR FOR");
            sb.AppendLine(
                "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{objectQualifier}%'");
            sb.AppendLine("OPEN cur_showfragmentation");
            sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
            sb.AppendLine("WHILE @@FETCH_STATUS = 0");
            sb.AppendLine("BEGIN");
            sb.AppendLine("DBCC SHOWCONTIG (@TableName)");
            sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
            sb.AppendLine("END");
            sb.AppendLine("CLOSE cur_showfragmentation");
            sb.AppendLine("DEALLOCATE cur_showfragmentation");

            using (var cmd = this.DbAccess.GetCommand(sb.ToString(), CommandType.Text))
            {
                this.DbAccess.ExecuteNonQuery(cmd, dbTransaction);
            }

            result = this._sqlMessages.Select(s => s.Message).ToDelimitedString("\r\n");

            return true;
        }

        #endregion
    }
}