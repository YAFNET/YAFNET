/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using System.Threading.Tasks;

namespace YAF.Core.Model;

using System;
using System.Security.Cryptography;
using System.Text;

using YAF.Types.Models;

/// <summary>
///     The check email repository extensions.
/// </summary>
public static class CheckEmailRepositoryExtensions
{
    /// <param name="repository">
    /// The repository.
    /// </param>
    extension(IRepository<CheckEmail> repository)
    {
        /// <summary>
        /// Saves the key of a verification link. Only a hash of the key is stored: the keys (identity tokens)
        /// are longer than the column, and the stored value can't be used as a link.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="key">
        /// The key of the link.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        public Task SaveAsync(int userId,
            string key,
            string email)
        {
            return repository.InsertAsync(
                new CheckEmail
                {
                    UserID = userId, Email = email.ToLower(), Created = DateTime.UtcNow, Hash = HashKey(key)
                });
        }

        /// <summary>
        /// Very confusingly named function that finds a user record with associated check email and returns a user if it's found.
        /// </summary>
        /// <param name="key">
        /// The key of the link.
        /// </param>
        /// <returns>
        /// The <see cref="CheckEmail"/>.
        /// </returns>
        public async Task<CheckEmail> UpdateAsync(string key)
        {
            if (key.IsNotSet())
            {
                return null;
            }

            var hash = HashKey(key);

            // links sent before the update contain the stored key itself
            var mail = await repository.GetSingleAsync(c => c.Hash == hash || c.Hash == key);

            return mail;
        }
    }

    /// <summary>
    /// The stored hash of a verification key (hex SHA-256, 64 characters).
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The hash.</returns>
    public static string HashKey(string key)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)));
    }
}