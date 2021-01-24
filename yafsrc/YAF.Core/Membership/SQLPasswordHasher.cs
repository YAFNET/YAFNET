/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Core.Membership
{
    using System;
    using System.Web.Security;

    using Microsoft.AspNet.Identity;

    using ServiceStack;

    using YAF.Core.Utilities.Helpers;

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
        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var passwordProperties = hashedPassword.Split('|');

            if (passwordProperties.Length != 3)
            {
                return base.VerifyHashedPassword(hashedPassword, providedPassword);
            }

            var passwordHash = passwordProperties[0];
            var salt = passwordProperties[2];

            return string.Equals(
                EncryptPassword(providedPassword, MembershipPasswordFormat.Hashed, salt),
                passwordHash,
                StringComparison.CurrentCultureIgnoreCase)
                ? PasswordVerificationResult.SuccessRehashNeeded
                : PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// The encrypt password.
        /// </summary>
        /// <param name="pass">
        /// The pass.
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
        private static string EncryptPassword(string pass, MembershipPasswordFormat passwordFormat, string salt)
        {
            var hashAlgorithmType = HashHelper.HashAlgorithmType.SHA1;

            try
            {
                Membership.HashAlgorithmType.ToEnum<HashHelper.HashAlgorithmType>();
            }
            catch (Exception exception)
            {
                hashAlgorithmType = HashHelper.HashAlgorithmType.SHA1;
            }

            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    return pass;
                case MembershipPasswordFormat.Hashed:
                    return HashHelper.Hash(
                        pass,
                        hashAlgorithmType,
                        salt,
                        false,
                        HashHelper.HashCaseType.None,
                        null,
                        false);
                case MembershipPasswordFormat.Encrypted:
                    var passwordManager = new YafMembershipProvider();
                    return passwordManager.GetClearTextPassword(pass);
            }

            return pass;
        }
    }
}