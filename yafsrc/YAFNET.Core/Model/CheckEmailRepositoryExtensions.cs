/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using YAF.Types.Models;

/// <summary>
///     The check email repository extensions.
/// </summary>
public static class CheckEmailRepositoryExtensions
{
    /// <summary>
    /// The save.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="hash">
    /// The hash.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    public static Task SaveAsync(
        this IRepository<CheckEmail> repository,
        int userId,
        string hash,
        string email)
    {
        return repository.InsertAsync(
            new CheckEmail
            {
                UserID = userId, Email = email.ToLower(), Created = DateTime.UtcNow, Hash = hash
            });
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
    /// The <see cref="CheckEmail"/>.
    /// </returns>
    public async static Task<CheckEmail> UpdateAsync(this IRepository<CheckEmail> repository, string hash)
    {
        var mail = await repository.GetSingleAsync(c => c.Hash == hash);

        return mail;
    }
}