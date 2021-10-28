// ***********************************************************************
// <copyright file="ByteArrayExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace ServiceStack
{
    /// <summary>
    /// Class ByteArrayExtensions.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Ares the equal.
        /// </summary>
        /// <param name="b1">The b1.</param>
        /// <param name="b2">The b2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool AreEqual(this byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;

            return !b1.Where((t, i) => t != b2[i]).Any();
        }

        /// <summary>
        /// Converts to sha1hash.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] ToSha1Hash(this byte[] bytes)
        {
            using var sha1 = SHA1.Create();
            return sha1.ComputeHash(bytes);
        }
    }

    /// <summary>
    /// Class ByteArrayComparer.
    /// Implements the <see cref="byte" />
    /// </summary>
    /// <seealso cref="byte" />
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static ByteArrayComparer Instance = new();

        /// <summary>
        /// Equalses the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Equals(byte[] left, byte[] right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }
            return left.SequenceEqual(right);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        /// <exception cref="System.ArgumentNullException">key</exception>
        public int GetHashCode(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return key.Sum(b => b);
        }
    }
}