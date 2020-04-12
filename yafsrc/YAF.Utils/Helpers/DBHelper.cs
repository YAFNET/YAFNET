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
namespace YAF.Utils.Helpers
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The DB helper.
    /// </summary>
    public static class DBHelper
    {
        #region Public Methods

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
            if (!dt.HasRows() || !dt.Columns.Contains(columnName))
            {
                return defaultValue;
            }

            return dt.Rows[0][columnName] != DBNull.Value ? dt.Rows[0][columnName].ToType<T>() : defaultValue;
        }

        /// <summary>
        /// Gets the first row of the data table or redirects to invalid request
        /// </summary>
        /// <param name="dataTable">
        /// The data Table.
        /// </param>
        /// <returns>
        /// The get first row or invalid.
        /// </returns>
        [CanBeNull]
        public static DataRow GetFirstRowOrInvalid([NotNull] this DataTable dataTable)
        {
            var row = dataTable.GetFirstRow();

            if (row != null)
            {
                return row;
            }

            // fail...
            BuildLink.RedirectInfoPage(InfoMessage.Invalid);

            return null;
        }

        /// <summary>
        /// Tests if an DB object (in <see cref="DataRow"/>) is <see cref="DBNull"/>.Value, <see langword="null"/> or empty.
        /// </summary>
        /// <param name="columnValue">The column value.</param>
        /// <returns>
        /// The is <see langword="null"/> or empty database field.
        /// </returns>
        public static bool IsNullOrEmptyDBField([NotNull] this object columnValue)
        {
            return columnValue == null || columnValue == DBNull.Value || columnValue.ToString().IsNotSet();
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

        #endregion
    }
}