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
namespace YAF.Types.Extensions
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     The data extensions.
    /// </summary>
    public static class DataExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to dictionary.
        /// </summary>
        /// <param name="reader">
        /// The reader. 
        /// </param>
        /// <param name="comparer">
        /// The comparer. 
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<IDictionary<string, object>> ToDictionary(
            [NotNull] this IDataReader reader, [CanBeNull] IEqualityComparer<string> comparer = null)
        {
            CodeContracts.VerifyNotNull(reader, "reader");

            while (reader.Read())
            {
                var rowDictionary = new Dictionary<string, object>(comparer ?? StringComparer.OrdinalIgnoreCase);

                for (int fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)
                {
                    rowDictionary.Add(reader.GetName(fieldIndex), reader.GetValue(fieldIndex));
                }

                yield return rowDictionary;
            }

            yield break;
        }

        /// <summary>
        /// The to dictionary.
        /// </summary>
        /// <param name="dataRow">
        /// The data row. 
        /// </param>
        /// <param name="comparer">
        /// The comparer. 
        /// </param>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        [NotNull]
        public static IDictionary<string, object> ToDictionary(
            [NotNull] this DataRow dataRow, [CanBeNull] IEqualityComparer<string> comparer = null)
        {
            CodeContracts.VerifyNotNull(dataRow, "dataRow");

            return dataRow.Table.Columns
                .OfType<DataColumn>()
                .Select(c => c.ColumnName)
                .ToDictionary(k => k, v => dataRow[v] == DBNull.Value ? null : dataRow[v], comparer ?? StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The to dictionary.
        /// </summary>
        /// <param name="dataTable">
        /// The data table. 
        /// </param>
        /// <param name="comparer">
        /// The comparer. 
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [NotNull]
        public static IEnumerable<IDictionary<string, object>> ToDictionary(
            [NotNull] this DataTable dataTable, [CanBeNull] IEqualityComparer<string> comparer = null)
        {
            CodeContracts.VerifyNotNull(dataTable, "dataTable");

            var columns = dataTable.Columns.OfType<DataColumn>().Select(c => c.ColumnName);

            return dataTable
                .AsEnumerable()
                .Select(
                    dataRow =>
                    columns.ToDictionary(k => k, v => dataRow[v] == DBNull.Value ? null : dataRow[v], comparer ?? StringComparer.OrdinalIgnoreCase))
                .Cast<IDictionary<string, object>>();
        }

        /// <summary>
        /// The typed.
        /// </summary>
        /// <param name="dataTable">
        /// The data table. 
        /// </param>
        /// <param name="comparer">
        /// The comparer. 
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [NotNull]
        public static IEnumerable<T> Typed<T>(
            [NotNull] this DataTable dataTable, [CanBeNull] IEqualityComparer<string> comparer = null)
            where T : IDataLoadable, new()
        {
            return dataTable.ToDictionary(comparer).Select(
                d =>
                    {
                        var newObj = new T();
                        newObj.LoadFromDictionary(d);
                        return newObj;
                    });
        }

        /// <summary>
        /// The typed.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<T> Typed<T>(
            [NotNull] this IDataReader dataReader, [CanBeNull] IEqualityComparer<string> comparer = null)
            where T : IDataLoadable, new()
        {
            return dataReader
                .ToDictionary(comparer)
                .Select(
                    d =>
                        {
                            var newObj = new T();
                            newObj.LoadFromDictionary(d);
                            return newObj;
                        })
                .ToList();
        }

        #endregion
    }
}