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

namespace YAF.Core.Helpers;

using System;
using System.Data.Common;
using System.Linq;

/// <summary>
/// The checks of the install wizard (the wizard is shown to every visitor of a forum that is not installed yet).
/// </summary>
public static class InstallHelper
{
    /// <summary>
    /// The text shown instead of a secret value of the connection string.
    /// </summary>
    public const string Mask = "********";

    /// <summary>
    /// The keys of the connection string values that are secrets.
    /// </summary>
    private static readonly string[] SecretKeys = ["password", "pwd"];

    /// <summary>
    /// The connection string without its secrets (the password of the database), it is shown on the test page of the
    /// wizard.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>The connection string with the masked secrets, an empty string if it can not be read.</returns>
    public static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return string.Empty;
        }

        try
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            foreach (var key in builder.Keys.Cast<string>().ToList()
                         .Where(key => SecretKeys.Contains(key.Trim(), StringComparer.OrdinalIgnoreCase)))
            {
                builder[key] = Mask;
            }

            return builder.ConnectionString;
        }
        catch (ArgumentException)
        {
            // a connection string that can't be read is not shown (it may contain a password)
            return string.Empty;
        }
    }

    /// <summary>
    /// Checks the values of the new board and its host admin.
    /// </summary>
    /// <param name="forumName">The name of the board.</param>
    /// <param name="forumEmail">The email address of the board.</param>
    /// <param name="forumBaseUrl">The base url of the board.</param>
    /// <param name="userName">The name of the host admin.</param>
    /// <param name="adminEmail">The email address of the host admin.</param>
    /// <param name="password">The password of the host admin.</param>
    /// <param name="confirmPassword">The password entered again.</param>
    /// <returns>
    /// <see cref="MissingValue"/> if a value is missing, <see cref="PasswordsDifferent"/> if the passwords differ, or
    /// <c>null</c> if the values can be used.
    /// </returns>
    public static string CheckCreateForum(
        string forumName,
        string forumEmail,
        string forumBaseUrl,
        string userName,
        string adminEmail,
        string password,
        string confirmPassword)
    {
        string[] required = [forumName, forumEmail, forumBaseUrl, userName, adminEmail, password, confirmPassword];

        if (required.Any(string.IsNullOrWhiteSpace))
        {
            return MissingValue;
        }

        // the host admin can only log in with the password: a typo locked the only admin out
        return string.Equals(password, confirmPassword, StringComparison.Ordinal) ? null : PasswordsDifferent;
    }

    /// <summary>
    /// The result of <see cref="CheckCreateForum"/> when a value is missing (the INSTALL text tag of the message).
    /// </summary>
    public const string MissingValue = "NEED_VALUE";

    /// <summary>
    /// The result of <see cref="CheckCreateForum"/> when the two passwords are different.
    /// </summary>
    public const string PasswordsDifferent = "PASSWORDS_DIFFERENT";
}
