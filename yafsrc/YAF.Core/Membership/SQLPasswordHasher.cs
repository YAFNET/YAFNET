/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

namespace YAF.Core.Membership
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Security;

    using Microsoft.AspNet.Identity;

    using YAF.Utils.Helpers;

    /// <summary>
    /// The SQL password hasher.
    /// </summary>
    public class SQLPasswordHasher : PasswordHasher
    {
        /// <summary>
        /// The hash password.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string HashPassword(string password)
        {
            return base.HashPassword(password);
        }

        private const int _pbkdf2IterCount = 1000;

        private const int _pbkdf2SubkeyLength = 256 / 8;

        private const int _saltSize = 128 / 8;

        internal static string BinaryToHex(byte[] data)
        {
            var hex = new char[data.Length * 2];
            for (var iter = 0; iter < data.Length; iter++)
            {
                var hexChar = (byte)(data[iter] >> 4);
                hex[iter * 2] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
                hexChar = (byte)(data[iter] & 0xF);
                hex[iter * 2 + 1] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
            }

            return new string(hex);
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }

            return areSame;
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var passwordProperties = hashedPassword.Split('|');

            if (passwordProperties.Length != 3)
            {
                return base.VerifyHashedPassword(hashedPassword, providedPassword);
            }
            else
            {
                var passwordHash = passwordProperties[0];
                var salt = passwordProperties[2];

                return string.Equals(
                    this.EncryptPassword(providedPassword, MembershipPasswordFormat.Hashed, salt),
                    passwordHash,
                    StringComparison.CurrentCultureIgnoreCase)
                    ? PasswordVerificationResult.SuccessRehashNeeded
                    : PasswordVerificationResult.Failed;
            }
        }


        private string EncryptPassword(string pass, MembershipPasswordFormat passwordFormat, string salt)
        {
            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    return pass;
                case MembershipPasswordFormat.Hashed:
                    return HashHelper.Hash(
                        pass,
                        HashHelper.HashAlgorithmType.SHA1,
                        salt,
                        false,
                        HashHelper.HashCaseType.None,
                        "",
                        false);
                case MembershipPasswordFormat.Encrypted:
                    var passwordManager = new YafMembershipProvider();
                    return passwordManager.GetClearTextPassword(pass);
            }

            return pass;
        }
    }
}