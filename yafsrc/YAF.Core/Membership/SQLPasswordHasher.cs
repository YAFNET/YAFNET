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

namespace YAF.Core.Membership;

using System;
using System.Text;
using System.Web.Security;

using Microsoft.AspNet.Identity;

/// <summary>
/// The SQL password hasher.
/// </summary>
public class SQLPasswordHasher : PasswordHasher
{
    /// <summary>
    /// The verify hashed password.
    /// </summary>
    /// <param name="hashedPassword">
    /// The hashed password.
    /// </param>
    /// <param name="providedPassword">
    /// The provided password.
    /// </param>
    /// <returns>
    /// The <see cref="PasswordVerificationResult"/>.
    /// </returns>
    public override PasswordVerificationResult VerifyHashedPassword(
        string hashedPassword,
        string providedPassword)
    {
        if (hashedPassword.IsNotSet())
        {
            return PasswordVerificationResult.Failed;
        }

        // migrated legacy passwords are stored as "password|format|salt",
        // the format and the (base64) salt never contain the separator, a clear text password can
        var lastSeparator = hashedPassword.LastIndexOf('|');
        var formatSeparator = lastSeparator > 0 ? hashedPassword.LastIndexOf('|', lastSeparator - 1) : -1;

        if (formatSeparator < 0)
        {
            return base.VerifyHashedPassword(hashedPassword, providedPassword);
        }

        var passwordHash = hashedPassword.Substring(0, formatSeparator);
        var format = hashedPassword.Substring(formatSeparator + 1, lastSeparator - formatSeparator - 1);
        var salt = hashedPassword.Substring(lastSeparator + 1);

        // unknown formats fail, they must never be compared as clear text
        if (!Enum.TryParse<MembershipPasswordFormat>(format, out var passwordFormat)
            || !Enum.IsDefined(typeof(MembershipPasswordFormat), passwordFormat))
        {
            return PasswordVerificationResult.Failed;
        }

        return passwordFormat switch
            {
                // passwords are case-sensitive
                MembershipPasswordFormat.Clear => FixedTimeEquals(providedPassword, passwordHash, false),

                // ASP.NET membership hashes (salt + password) and hashes of the old YAF provider (password + salt)
                MembershipPasswordFormat.Hashed => FixedTimeEquals(
                                                       Hash(providedPassword, salt, true),
                                                       passwordHash,
                                                       Config.LegacyMembershipHashHex)
                                                   || FixedTimeEquals(
                                                       Hash(providedPassword, salt, false),
                                                       passwordHash,
                                                       Config.LegacyMembershipHashHex),

                // encrypted with the machine key of the web.config
                MembershipPasswordFormat.Encrypted => FixedTimeEquals(
                    providedPassword,
                    Decrypt(passwordHash),
                    false),
                _ => false
            }
                   ? PasswordVerificationResult.SuccessRehashNeeded
                   : PasswordVerificationResult.Failed;
    }

    /// <summary>
    /// Hashes the password like the ASP.NET membership (or the old YAF provider) did.
    /// </summary>
    /// <param name="clearPassword">The clear password.</param>
    /// <param name="salt">The salt.</param>
    /// <param name="standardComp">Use the ASP.NET membership order (salt + password).</param>
    /// <returns>The hash, or null if the salt is invalid.</returns>
    private static string Hash(string clearPassword, string salt, bool standardComp)
    {
        try
        {
            return HashHelper.Hash(
                clearPassword,
                Config.LegacyMembershipHashAlgorithmType,
                salt,
                Config.LegacyMembershipHashHex,
                Config.LegacyMembershipHashCase,
                null,
                standardComp);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    /// <summary>
    /// Decrypts a password that was encrypted with the machine key.
    /// </summary>
    /// <param name="encryptedPassword">The encrypted password.</param>
    /// <returns>The clear password, or null if it can't be decrypted.</returns>
    private static string Decrypt(string encryptedPassword)
    {
        try
        {
            return new YafMembershipProvider().GetClearTextPassword(encryptedPassword);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Compares the strings in a time that doesn't depend on the position of the first difference.
    /// </summary>
    /// <param name="provided">The provided value.</param>
    /// <param name="stored">The stored value.</param>
    /// <param name="ignoreCase">Ignore the case (for hex values).</param>
    /// <returns>Returns if the values are equal.</returns>
    private static bool FixedTimeEquals(string provided, string stored, bool ignoreCase)
    {
        if (provided is null || stored is null)
        {
            return false;
        }

        if (ignoreCase)
        {
            provided = provided.ToUpperInvariant();
            stored = stored.ToUpperInvariant();
        }

        var providedBytes = Encoding.UTF8.GetBytes(provided);
        var storedBytes = Encoding.UTF8.GetBytes(stored);

        var difference = providedBytes.Length ^ storedBytes.Length;

        for (var i = 0; i < providedBytes.Length && i < storedBytes.Length; i++)
        {
            difference |= providedBytes[i] ^ storedBytes[i];
        }

        return difference == 0;
    }
}
