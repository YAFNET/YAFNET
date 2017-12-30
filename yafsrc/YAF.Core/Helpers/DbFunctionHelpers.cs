/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

namespace YAF.Core.Helpers
{
    using System;
    using System.Linq;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// The DB Function Helpers
    /// </summary>
    public static class DbFunctionHelper
    {
        /// <summary>
        /// Validates the and execute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbFunction">The database function.</param>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">@Database Provider does not support operation {0}..FormatWith(operationName)</exception>
        private static T ValidateAndExecute<T>(
            this IDbFunction dbFunction,
            string operationName,
            Func<IDbFunction, T> func)
        {
            if (!dbFunction.DbSpecificFunctions.WhereOperationSupported(operationName).Any())
            {
                throw new InvalidOperationException(
                    @"Database Provider does not support operation ""{0}"".".FormatWith(operationName));
            }

            return func(dbFunction);
        }

        /// <summary>
        /// Gets the size of the database.
        /// </summary>
        /// <param name="dbFunction">The database function.</param>
        /// <returns>Returns the size of the database</returns>
        public static int GetDBSize([NotNull] this IDbFunction dbFunction)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            return dbFunction.ValidateAndExecute("DBSize", f => f.GetScalar<int>(s => s.DBSize()));
        }

        /// <summary>
        /// Gets the current SQL Engine Edition.
        /// </summary>
        /// <param name="dbFunction">The database function.</param>
        /// <returns>
        /// Returns the current SQL Engine Edition.
        /// </returns>
        public static string GetSQLEngine([NotNull] this IDbFunction dbFunction)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            return dbFunction.ValidateAndExecute("GetSQLEngine", f => f.GetScalar<string>(s => s.GetSQLEngine()));
        }

        /// <summary>
        /// Determines whether [is full text supported].
        /// </summary>
        /// <param name="dbFunction">The database function.</param>
        /// <returns>
        /// Returns if full text is supported by the server or not
        /// </returns>
        public static bool IsFullTextSupported([NotNull] this IDbFunction dbFunction)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            return dbFunction.ValidateAndExecute(
                "IsFullTextSupported",
                f => f.GetScalar<bool>(s => s.IsFullTextSupported()));
        }
    }
}