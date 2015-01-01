/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
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
    using System.Data;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// MS SQL Specific Functions
    /// </summary>
    public static class MsSqlSpecificFunctions
    {
        /// <summary>
        /// Gets the database size
        /// </summary>
        /// <param name="dbAccess">The database access.</param>
        /// <returns>
        /// integer value for database size
        /// </returns>
        public static int DBSize(this IDbAccess dbAccess)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");

            using (var cmd = dbAccess.GetCommand("SELECT sum(reserved_page_count) * 8.0 / 1024 FROM sys.dm_db_partition_stats", CommandType.Text))
            {
                return dbAccess.ExecuteScalar(cmd).ToType<int>();
            }
        }
    }
}