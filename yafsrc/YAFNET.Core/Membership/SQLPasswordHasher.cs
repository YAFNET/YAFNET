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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Membership;

using System;

using Microsoft.AspNetCore.Identity;

using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;

/// <summary>
/// The SQL password hasher.
/// </summary>
public class SQLPasswordHasher : PasswordHasher<AspNetUsers>
{
    /// <summary>
    /// The verify hashed password.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="hashedPassword">
    /// The hashed password.
    /// </param>
    /// <param name="providedPassword">
    /// The provided password.
    /// </param>
    /// <returns>
    /// The <see cref="Microsoft.AspNetCore.Identity.PasswordVerificationResult"/>.
    /// </returns>
    public override PasswordVerificationResult VerifyHashedPassword(AspNetUsers user, string hashedPassword, string providedPassword)
    {
        if (hashedPassword.IsNotSet())
        {
            return PasswordVerificationResult.Failed;
        }

        var passwordProperties = hashedPassword.Split('|');

        if (passwordProperties.Length != 3)
        {
            return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }

        var passwordHash = passwordProperties[0];
        var salt = passwordProperties[2];

        var passwordFormat = passwordProperties[1].ToEnum<MembershipPasswordFormat>();

        var encryptedPassword = EncryptPassword(providedPassword,
            passwordFormat,
            salt);

        var password = passwordFormat == MembershipPasswordFormat.Hashed
            ? passwordHash
            : providedPassword;

        return string.Equals(
            encryptedPassword,
            password,
            StringComparison.CurrentCultureIgnoreCase)
            ? PasswordVerificationResult.SuccessRehashNeeded
            : PasswordVerificationResult.Failed;
    }

    /// <summary>
    /// The encrypt password.
    /// </summary>
    /// <param name="clearPassword">
    /// The clear Password.
    /// </param>
    /// <param name="passwordFormat">
    /// The password format.
    /// </param>
    /// <param name="salt">
    /// The salt.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private static string EncryptPassword(string clearPassword, MembershipPasswordFormat passwordFormat, string salt)
    {
        return passwordFormat switch
            {
                MembershipPasswordFormat.Clear => clearPassword,
                MembershipPasswordFormat.Hashed => HashHelper.Hash(
                    clearPassword,
                    Config.LegacyMembershipHashAlgorithmType,
                    salt,
                    Config.LegacyMembershipHashHex,
                    Config.LegacyMembershipHashCase,
                    null,
                    false),
                _ => clearPassword
            };
    }
}