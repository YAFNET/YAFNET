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
namespace YAF.Core.Model
{
    using System.Collections.Generic;
    using System.Data;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The Registry repository extensions.
    /// </summary>
    public static class RegistyRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Lists the specified repository.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="boardId">The board identifier.</param>
        /// <returns></returns>
        public static DataTable List(
            this IRepository<Registry> repository,
            string settingName = null,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.registry_list(Name: settingName, BoardID: boardId);
        }

        /// <summary>
        /// Saves the specified repository.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        /// <param name="boardId">The board identifier.</param>
        public static void Save(
            this IRepository<Registry> repository,
            string settingName,
            string settingValue,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.registry_save(Name: settingName, Value: settingValue, BoardID: boardId);

            repository.FireUpdated();
        }

        /// <summary>
        /// Gets the registry setting by name.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="settingName">Name of the setting.</param>
        /// <returns>Returns the Setting Value</returns>
        public static string GetRegistrySetting(this IRepository<Registry> repository, string settingName)
        {
            CodeContracts.VerifyNotNull(repository, "repository");
            CodeContracts.VerifyNotNull(settingName, "settingName");

            try
            {
                using (var table = List(repository: repository, settingName: settingName))
                {
                    if (table.HasRows())
                    {
                        // get the version...
                        return table.Rows[0]["Value"].ToType<string>();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        #endregion
    }
}