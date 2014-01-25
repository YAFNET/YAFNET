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

namespace YAF.Data.MsSql.Functions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using YAF.Data.MsSql.Search;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Interfaces.Data;

    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class MsSqlSearchFunction : BaseMsSqlFunction
    {
        public MsSqlSearchFunction([NotNull] IDbAccess dbAccess)
            : base(dbAccess)
        {
        }

        public override int SortOrder
        {
            get
            {
                return 1000;
            }
        }

        public override bool IsSupportedOperation(string operationName)
        {
            return operationName.Equals("executesearch");
        }

        protected override bool RunOperation(
            SqlConnection sqlConnection,
            IDbTransaction dbTransaction,
            DbFunctionType dbfunctionType,
            string operationName,
            IEnumerable<KeyValuePair<string, object>> parameters,
            out object result)
        {
            var context = parameters.First().Value as ISearchContext;

            if (context != null)
            {
                var sql = new SearchBuilder().BuildSearchSql(context);

                using (var cmd = this.DbAccess.GetCommand(sql, CommandType.Text))
                {
                    result = this.DbAccess.GetReader(cmd, dbTransaction);
                    return true;
                }
            }

            // failure...
            result = null;
            return false;
        }
    }
}