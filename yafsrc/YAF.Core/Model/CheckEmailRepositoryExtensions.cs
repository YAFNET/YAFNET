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
namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The check email repository extensions.
    /// </summary>
    public static class CheckEmailRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable List(this IRepository<CheckEmail> repository, string email = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.checkemail_list(Email: email);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<CheckEmail> ListTyped(this IRepository<CheckEmail> repository, string email = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return session.GetTyped<CheckEmail>(r => r.checkemail_list(Email: email));
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        public static void Save(this IRepository<CheckEmail> repository, int? userID, [NotNull] string hash, [NotNull] string email)
        {
            CodeContracts.VerifyNotNull(hash, "hash");
            CodeContracts.VerifyNotNull(email, "email");
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.checkemail_save(UserID: userID, Hash: hash, Email: email.ToLower(), UTCTIMESTAMP: DateTime.UtcNow);
            repository.FireNew();
        }

        /// <summary>
        /// Very confusingly named function that finds a user record with associated check email and returns a user if it's found.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable Update(this IRepository<CheckEmail> repository, [NotNull] string hash)
        {
            CodeContracts.VerifyNotNull(hash, "hash");
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.checkemail_update(Hash: hash);
        }

        #endregion
    }
}