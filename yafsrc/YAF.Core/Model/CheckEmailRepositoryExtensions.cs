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
namespace YAF.Core.Model;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using YAF.Types.Models;

/// <summary>
///     The check email repository extensions.
/// </summary>
public static class CheckEmailRepositoryExtensions
{
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
    public static IList<CheckEmail> ListTyped(
        this IRepository<CheckEmail> repository,
        string email = null)
    {
        return email.IsSet() ? repository.Get(mail => mail.Email == email) : repository.Get(null);
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
    /// <param name="hash">
    /// The hash.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    public static void Save(
        this IRepository<CheckEmail> repository,
        int? userId,
        string key,
        string email)
    {
        // only a hash of the key is stored, the key (a token of the identity) can be longer than the column
        repository.Insert(
            new CheckEmail
                {
                    UserID = userId.Value, Email = email.ToLower(), Created = DateTime.UtcNow, Hash = HashKey(key)
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
    public static CheckEmail Update(this IRepository<CheckEmail> repository, string key)
    {
        if (key.IsNotSet())
        {
            return null;
        }

        var hash = HashKey(key);

        // links sent before the update contain the stored key itself
        var mail = repository.GetSingle(c => c.Hash == hash || c.Hash == key);

        return mail;
    }

    /// <summary>
    /// The hash of a verification key (stored instead of the key).
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The SHA256 hash as hex string.
    /// </returns>
    public static string HashKey(string key)
    {
        using var sha = SHA256.Create();

        return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(key))).Replace("-", string.Empty);
    }
}