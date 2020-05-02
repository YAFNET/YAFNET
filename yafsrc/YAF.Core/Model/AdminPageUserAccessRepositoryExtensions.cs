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
namespace YAF.Core.Model
{
    using System.Data;
    using System.Linq;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The admin page user access repository extensions.
    /// </summary>
    public static class AdminPageUserAccessRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Lists the forum.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="pageName">
        /// The page Name.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> .
        /// </returns>
        public static DataTable List(this IRepository<AdminPageUserAccess> repository, int userId, string pageName)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.adminpageaccess_list(UserID: userId, PageName: pageName);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageName">
        /// The page name.
        /// </param>
        public static void Save(this IRepository<AdminPageUserAccess> repository, int userId, string pageName)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            if (!repository.Get(a => a.UserID == userId && a.PageName == pageName).Any())
            {
                repository.Insert(new AdminPageUserAccess { UserID = userId, PageName = pageName });
            }
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageName">
        /// The page name.
        /// </param>
        public static void Delete(this IRepository<AdminPageUserAccess> repository, int userId, string pageName)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.Delete(u => u.UserID == userId && u.PageName == pageName);
        }

        #endregion
    }
}