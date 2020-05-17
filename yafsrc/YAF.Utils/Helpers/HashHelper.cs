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
namespace YAF.Utils.Helpers
{
    #region Using

    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The hash helper.
    /// </summary>
    public static class HashHelper
    {
        #region Enums

        /// <summary>
        /// The hash algorithm type.
        /// </summary>
        public enum HashAlgorithmType
        {
            /// <summary>
            /// The MD5.
            /// </summary>
            MD5,

            /// <summary>
            /// The SHA1.
            /// </summary>
            SHA1,

            /// <summary>
            /// The SHA256.
            /// </summary>
            SHA256,

            /// <summary>
            /// The SHA384.
            /// </summary>
            SHA384,

            /// <summary>
            /// The SHA512.
            /// </summary>
            SHA512
        }

        /// <summary>
        /// The hash case type.
        /// </summary>
        public enum HashCaseType
        {
            /// <summary>
            /// The upper.
            /// </summary>
            Upper,

            /// <summary>
            /// The lower.
            /// </summary>
            Lower,

            /// <summary>
            /// The none.
            /// </summary>
            None
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a password buffer from salt and password ready for hashing/encrypting
        /// </summary>
        /// <param name="salt">
        /// Salt to be applied to hashing algorithm
        /// </param>
        /// <param name="clearString">
        /// Clear string to hash
        /// </param>
        /// <param name="standardComp">
        /// Use Standard asp.net membership method of creating the buffer
        /// </param>
        /// <returns>
        /// Salted Password as Byte Array 
        /// </returns>
        [NotNull]
        public static byte[] GeneratePasswordBuffer(
            [NotNull] string salt,
            [NotNull] string clearString,
            bool standardComp)
        {
            var unencodedBytes = Encoding.Unicode.GetBytes(clearString);
            var saltBytes = Convert.FromBase64String(salt);
            var buffer = new byte[unencodedBytes.Length + saltBytes.Length];

            if (standardComp)
            {
                // Compliant with ASP.NET Membership method of hash/salt
                Buffer.BlockCopy(saltBytes, 0, buffer, 0, saltBytes.Length);
                Buffer.BlockCopy(unencodedBytes, 0, buffer, saltBytes.Length, unencodedBytes.Length);
            }
            else
            {
                Buffer.BlockCopy(unencodedBytes, 0, buffer, 0, unencodedBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, buffer, unencodedBytes.Length - 1, saltBytes.Length);
            }

            return buffer;
        }

        /// <summary>
        /// Hashes a clear string to the given hash type
        /// </summary>
        /// <param name="clearString">
        /// Clear string to hash
        /// </param>
        /// <param name="hashAlgorithmType">
        /// hash Algorithm to be used
        /// </param>
        /// <param name="salt">
        /// Salt to be applied to hashing algorithm
        /// </param>
        /// <param name="hashHex">
        /// The hash Hex.
        /// </param>
        /// <param name="hashCaseType">
        /// The hash Case.
        /// </param>
        /// <param name="hashRemoveChars">
        /// The hash Remove Chars.
        /// </param>
        /// <param name="standardComp">
        /// The standard Comp.
        /// </param>
        /// <returns>
        /// Hashed String as Hex or Base64 
        /// </returns>
        public static string Hash(
            [NotNull] string clearString,
            HashAlgorithmType hashAlgorithmType = HashAlgorithmType.SHA1,
            [CanBeNull] string salt = null,
            bool hashHex = true,
            HashCaseType hashCaseType = HashCaseType.Upper,
            [CanBeNull] string hashRemoveChars = null,
            bool standardComp = true)
        {
            CodeContracts.VerifyNotNull(clearString, "clearString");

            byte[] buffer;

            if (salt.IsSet())
            {
                buffer = GeneratePasswordBuffer(salt, clearString, standardComp);
            }
            else
            {
                var unencodedBytes = Encoding.UTF8.GetBytes(clearString); // UTF8 used to maintain compatibility
                buffer = new byte[unencodedBytes.Length];
                Buffer.BlockCopy(unencodedBytes, 0, buffer, 0, unencodedBytes.Length);
            }

            var hashedBytes = Hash(buffer, hashAlgorithmType); // Hash

            var hashedString = hashHex ? hashedBytes.ToHexString() : Convert.ToBase64String(hashedBytes);

            // Adjust the case of the hash output
            switch (hashCaseType)
            {
                case HashCaseType.Upper:
                    hashedString = hashedString.ToUpper();
                    break;
                case HashCaseType.Lower:
                    hashedString = hashedString.ToLower();
                    break;
            }

            if (hashRemoveChars.IsSet())
            {
                hashedString = hashRemoveChars.Aggregate(
                    hashedString,
                    (current, removeChar) => current.Replace(removeChar.ToString(), string.Empty));
            }

            return hashedString;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Hashes clear bytes to given hash type
        /// </summary>
        /// <param name="clearBytes">
        /// Clear bytes to hash
        /// </param>
        /// <param name="hashAlgorithmType">
        /// hash Algorithm to be used
        /// </param>
        /// <returns>
        /// Hashed bytes 
        /// </returns>
        [NotNull]
        private static byte[] Hash([NotNull] byte[] clearBytes, [NotNull] HashAlgorithmType hashAlgorithmType)
        {
            CodeContracts.VerifyNotNull(clearBytes, "clearBytes");

            return HashAlgorithm.Create(hashAlgorithmType.GetStringValue()).ComputeHash(clearBytes);
        }

        #endregion
    }
}