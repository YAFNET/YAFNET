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
namespace YAF.Utils.Helpers
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The db helper.
    /// </summary>
    public static class DBHelper
    {
        #region Public Methods

        /// <summary>
        /// Converts <paramref name="columnName"/> in a <see cref="DataTable"/> to a generic List of type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the list.
        /// </typeparam>
        /// <param name="dataTable">
        /// The data table.
        /// </param>
        /// <param name="columnName">
        /// Name of the column.
        /// </param>
        /// <returns>
        /// The get column as list.
        /// </returns>
        [NotNull]
        public static List<T> GetColumnAsList<T>([NotNull] this DataTable dataTable, [NotNull] string columnName)
        {
            return (from x in dataTable.AsEnumerable() select x.Field<T>(columnName)).ToList();
        }

        /// <summary>
        /// Converts the first column of a <see cref="DataTable"/> to a generic List of type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the list.
        /// </typeparam>
        /// <param name="dataTable">
        /// The data table.
        /// </param>
        /// <returns>
        /// The get first column as list.
        /// </returns>
        [NotNull]
        public static List<T> GetFirstColumnAsList<T>([NotNull] this DataTable dataTable)
        {
            return (from x in dataTable.AsEnumerable() select x.Field<T>(0)).ToList();
        }

        /// <summary>
        /// Gets the first row (<see cref="DataRow"/>) of a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="dt">
        /// The Data Table.
        /// </param>
        /// <returns>
        /// The get first row.
        /// </returns>
        public static DataRow GetFirstRow([NotNull] this DataTable dt)
        {
            return dt.HasRows() ? dt.Rows[0] : null;
        }

        /// <summary>
        /// Gets the specified column of the first row as the type specified. If not available returns default value.
        /// </summary>
        /// <typeparam name="T">
        /// Type if column to return
        /// </typeparam>
        /// <param name="dt">
        /// <see cref="DataTable"/> to pull from
        /// </param>
        /// <param name="columnName">
        /// Name of column to convert
        /// </param>
        /// <param name="defaultValue">
        /// value to return if something is not available
        /// </param>
        /// <returns>
        /// The get first row column as value.
        /// </returns>
        public static T GetFirstRowColumnAsValue<T>(
            [NotNull] this DataTable dt, [NotNull] string columnName, T defaultValue)
        {
            if (dt.HasRows() && dt.Columns.Contains(columnName))
            {
                if (dt.Rows[0][columnName] != DBNull.Value)
                {
                    return dt.Rows[0][columnName].ToType<T>();
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the first row of the data table or redirects to invalid request
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The get first row or invalid.
        /// </returns>
        [CanBeNull]
        public static DataRow GetFirstRowOrInvalid([NotNull] this DataTable dt)
        {
            var row = dt.GetFirstRow();

            if (row != null)
            {
                return row;
            }

            // fail...
            YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);

            return null;
        }

        /// <summary>
        /// Tests if an DB object (in <see cref="DataRow"/>) is <see cref="DBNull"/>.Value, <see langword="null"/> or empty.
        /// </summary>
        /// <param name="columnValue">The column value.</param>
        /// <returns>
        /// The is <see langword="null"/> or empty db field.
        /// </returns>
        public static bool IsNullOrEmptyDBField([NotNull] this object columnValue)
        {
            return columnValue == DBNull.Value || columnValue.ToString().IsNotSet();
        }

        /// <summary>
        /// Selects a typed list of rows using the <paramref name="createNew"/> delegate.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the list.
        /// </typeparam>
        /// <param name="dataTable">
        /// The data table.
        /// </param>
        /// <param name="createNew">
        /// The create new.
        /// </param>
        /// <returns>
        /// The select typed list.
        /// </returns>
        [NotNull]
        public static IEnumerable<T> SelectTypedList<T>(
            [NotNull] this DataTable dataTable, [NotNull] Func<DataRow, T> createNew)
        {
            CodeContracts.VerifyNotNull(dataTable, "dataTable");
            CodeContracts.VerifyNotNull(createNew, "createNew");

            return dataTable.AsEnumerable().Select(createNew);
        }

        /// <summary>
        /// The to trace string.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// Returns the Debug String
        /// </returns>
        public static string ToDebugString([NotNull] this IDbCommand command)
        {
            CodeContracts.VerifyNotNull(command, "command");

            var debugString = command.CommandText;

            try
            {
                if (command.Parameters != null && command.Parameters.Count > 0)
                {
                    var parameters = command.Parameters.Cast<DbParameter>().ToList();

                    var sqlParams =
                        parameters.Select(
                            p =>
                            @"@{0} = {1}".FormatWith(
                                p.ParameterName, p.Value == null ? "NULL" : "'{0}'".FormatWith(p.Value.ToString().Replace("'", "''"))));

                    debugString += " " + sqlParams.ToDelimitedString(", ");
                }
            }
            catch (Exception ex)
            {
                debugString += "Error in getting parameters {0}".FormatWith(ex);
            }

            return debugString;
        }

        #endregion
    }
}