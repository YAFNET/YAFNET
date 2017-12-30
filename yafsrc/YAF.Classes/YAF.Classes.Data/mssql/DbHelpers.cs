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

namespace YAF.Classes.Data
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    using YAF.Types;
    using YAF.Types.Extensions;

    /// <summary>
    /// The DB helpers.
    /// </summary>
    public static class DbHelpers
    {
        /// <summary>
        /// Gets a value indicating whether YAF should be optimized for use with very large number of forums. 
        /// Before enabling you should know exactly what you do and what's it all for. Ask developers.
        /// </summary>
        public static bool LargeForumTree
        {
            get
            {
                return Config.LargeForumTree;
            }
        }

        /// <summary>
        /// Creates new SQL Command based on command text applying all qualifiers to the name.
        /// </summary>
        /// <param name="commandText">
        /// Command text to qualify.
        /// </param>
        /// <param name="isText">
        /// Determines whether command text is text or stored procedure.
        /// </param>
        /// <returns>
        /// New SQL Command
        /// </returns>
        public static SqlCommand GetCommand([NotNull] string commandText, bool isText)
        {
            return GetCommand(commandText, isText, null);
        }

        /// <summary>
        /// Creates new SQL Command based on command text applying all qualifiers to the name.
        /// </summary>
        /// <param name="commandText">
        /// Command text to qualify.
        /// </param>
        /// <param name="isText">
        /// Determines whether command text is text or stored procedure.
        /// </param>
        /// <param name="connection">
        /// Connection to use with command.
        /// </param>
        /// <returns>
        /// New SQL Command
        /// </returns>
        public static SqlCommand GetCommand(
            [NotNull] string commandText,
            bool isText,
            [NotNull] IDbConnection connection)
        {
            return isText
                       ? new SqlCommand
                             {
                                 CommandType = CommandType.Text,
                                 CommandText = GetCommandTextReplaced(commandText),
                                 Connection = connection as SqlConnection,
                                 CommandTimeout = Config.SqlCommandTimeout.ToType<int>()
                             }
                       : GetCommand(commandText);
        }

        /// <summary>
        /// Creates new SQL Command calling stored procedure applying all qualifiers to the name.
        /// </summary>
        /// <param name="storedProcedure">
        /// Base of stored procedure name.
        /// </param>
        /// <returns>
        /// New SQL Command
        /// </returns>
        [NotNull]
        public static SqlCommand GetCommand([NotNull] string storedProcedure)
        {
            return GetCommand(storedProcedure, null);
        }

        /// <summary>
        /// Creates new SQL Command calling stored procedure applying all qualifiers to the name.
        /// </summary>
        /// <param name="storedProcedure">
        /// Base of stored procedure name.
        /// </param>
        /// <param name="connection">
        /// Connection to use with command.
        /// </param>
        /// <returns>
        /// New SQL Command
        /// </returns>
        [NotNull]
        public static SqlCommand GetCommand([NotNull] string storedProcedure, [NotNull] DbConnection connection)
        {
            var cmd = new SqlCommand
                          {
                              CommandType = CommandType.StoredProcedure,
                              CommandText = GetObjectName(storedProcedure),
                              Connection = connection as SqlConnection,
                              CommandTimeout = int.Parse(Config.SqlCommandTimeout)
                          };

            return cmd;
        }

        /// <summary>
        /// Gets command text replaced with {databaseOwner} and {objectQualifier}.
        /// </summary>
        /// <param name="commandText">
        /// Test to transform.
        /// </param>
        /// <returns>
        /// The get command text replaced.
        /// </returns>
        [NotNull]
        public static string GetCommandTextReplaced([NotNull] string commandText)
        {
            commandText = commandText.Replace("{databaseOwner}", Config.DatabaseOwner);
            commandText = commandText.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);

            return commandText;
        }

        /// <summary>
        /// Creates a Connection String from the parameters.
        /// </summary>
        /// <param name="parm1">The parm1.</param>
        /// <param name="parm2">The parm2.</param>
        /// <param name="parm3">The parm3.</param>
        /// <param name="parm4">The parm4.</param>
        /// <param name="parm5">The parm5.</param>
        /// <param name="parm6">The parm6.</param>
        /// <param name="parm7">The parm7.</param>
        /// <param name="parm8">The parm8.</param>
        /// <param name="parm9">The parm9.</param>
        /// <param name="parm10">The parm10.</param>
        /// <param name="parm11">if set to <c>true</c> [parm11].</param>
        /// <param name="parm12">if set to <c>true</c> [parm12].</param>
        /// <param name="parm13">if set to <c>true</c> [parm13].</param>
        /// <param name="parm14">if set to <c>true</c> [parm14].</param>
        /// <param name="parm15">if set to <c>true</c> [parm15].</param>
        /// <param name="parm16">if set to <c>true</c> [parm16].</param>
        /// <param name="parm17">if set to <c>true</c> [parm17].</param>
        /// <param name="parm18">if set to <c>true</c> [parm18].</param>
        /// <param name="parm19">if set to <c>true</c> [parm19].</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="userPassword">The user password.</param>
        /// <returns>
        /// The get connection string.
        /// </returns>
        public static string GetConnectionString(
            [NotNull] string parm1,
            [NotNull] string parm2,
            [NotNull] string parm3,
            [NotNull] string parm4,
            [NotNull] string parm5,
            [NotNull] string parm6,
            [NotNull] string parm7,
            [NotNull] string parm8,
            [NotNull] string parm9,
            [NotNull] string parm10,
            bool parm11,
            bool parm12,
            bool parm13,
            bool parm14,
            bool parm15,
            bool parm16,
            bool parm17,
            bool parm18,
            bool parm19,
            [NotNull] string userID,
            [NotNull] string userPassword)
        {
            // TODO: Parameters should be in a List<ConnectionParameters>
            var connBuilder = new SqlConnectionStringBuilder { DataSource = parm1, InitialCatalog = parm2 };

            if (parm11)
            {
                connBuilder.IntegratedSecurity = true;
            }
            else
            {
                connBuilder.UserID = userID;
                connBuilder.Password = userPassword;
            }

            return connBuilder.ConnectionString;
        }

        /// <summary>
        /// Gets qualified object name
        /// </summary>
        /// <param name="name">
        /// Base name of an object
        /// </param>
        /// <returns>
        /// Returns qualified object name of format {databaseOwner}.{objectQualifier}name
        /// </returns>
        public static string GetObjectName([NotNull] string name)
        {
            return "[{0}].[{1}{2}]".FormatWith(Config.DatabaseOwner, Config.DatabaseObjectQualifier, name);
        }
    }
}