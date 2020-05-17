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

namespace YAF.Core.Helpers
{
    using System;
    using System.Linq;
    
    using YAF.Types;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// The DB Function Helpers
    /// </summary>
    public static class DbFunctionHelper
    {
        /// <summary>
        /// The validate and execute.
        /// </summary>
        /// <param name="dbFunction">
        /// The db function.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="func">
        /// The func.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private static T ValidateAndExecute<T>(
            this IDbFunction dbFunction,
            string operationName,
            Func<IDbFunction, T> func)
        {
            if (!dbFunction.DbSpecificFunctions.WhereOperationSupported(operationName).Any())
            {
                throw new InvalidOperationException(
                    $@"Database Provider does not support operation ""{operationName}"".");
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
        public static string GetSQLVersion([NotNull] this IDbFunction dbFunction)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            return dbFunction.ValidateAndExecute("GetSQLVersion", f => f.GetScalar<string>(s => s.GetSQLVersion()));
        }

        /// <summary>
        /// The shrink database.
        /// </summary>
        /// <param name="dbFunction">
        /// The db function.
        /// </param>
        public static void ShrinkDatabase([NotNull] this IDbFunction dbFunction)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            dbFunction.ValidateAndExecute("ShrinkDatabase", f => f.GetScalar<string>(s => s.ShrinkDatabase()));
        }

        /// <summary>
        /// The re index database.
        /// </summary>
        /// <param name="dbFunction">
        /// The db function.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReIndexDatabase([NotNull] this IDbFunction dbFunction)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            return dbFunction.ValidateAndExecute("ShrinkDatabase", f => f.GetScalar<string>(s => s.ReIndexDatabase()));
        }

        /// <summary>
        /// The system initialize execute scripts.
        /// </summary>
        /// <param name="dbFunction">
        /// The db function.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        /// <param name="scriptFile">
        /// The script file.
        /// </param>
        /// <param name="useTransactions">
        /// The use transactions.
        /// </param>
        public static void SystemInitializeExecuteScripts(
            [NotNull] this IDbFunction dbFunction,
            [NotNull] string script,
            [NotNull] string scriptFile,
            bool useTransactions)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            dbFunction.ValidateAndExecute(
                "SystemInitializeExecuteScripts",
                f => f.Scalar.SystemInitializeExecuteScripts(script, scriptFile, useTransactions));
        }

        /// <summary>
        /// The system initialize fix access.
        /// </summary>
        /// <param name="dbFunction">
        /// The db function.
        /// </param>
        /// <param name="grantAccess">
        /// The grant access.
        /// </param>
        public static void SystemInitializeFixAccess(
            [NotNull] this IDbFunction dbFunction,
            bool grantAccess)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            dbFunction.ValidateAndExecute(
                "SystemInitializeFixAccess",
                f => f.Scalar.SystemInitializeFixAccess(grantAccess));
        }

        /// <summary>
        /// The run sql.
        /// </summary>
        /// <param name="dbFunction">
        /// The db function.
        /// </param>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="useTransaction">
        /// The use transaction.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RunSQL(
            [NotNull] this IDbFunction dbFunction,
            string sql,
            bool useTransaction)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

           return dbFunction.ValidateAndExecute(
                "RunSQL",
                f => f.Scalar.RunSQL(sql, useTransaction));
        }

        /// <summary>
        /// The change recovery mode.
        /// </summary>
        /// <param name="dbFunction">
        /// The db function.
        /// </param>
        /// <param name="recoveryMode">
        /// The recovery mode.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ChangeRecoveryMode(
            [NotNull] this IDbFunction dbFunction,
            string recoveryMode)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            return dbFunction.ValidateAndExecute(
                "ChangeRecoveryMode",
                f => f.Scalar.ChangeRecoveryMode(recoveryMode));
        }
    }
}