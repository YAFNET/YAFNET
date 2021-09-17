// ***********************************************************************
// <copyright file="StringBuilderPool.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Text.Pools
{
    // Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

    using System.Text;

    /// <summary>
    /// Class StringBuilderPool.
    /// </summary>
    public static class StringBuilderPool
    {
        /// <summary>
        /// Allocates this instance.
        /// </summary>
        /// <returns>StringBuilder.</returns>
        public static StringBuilder Allocate()
        {
            return SharedPools.Default<StringBuilder>().AllocateAndClear();
        }

        /// <summary>
        /// Frees the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void Free(StringBuilder builder)
        {
            SharedPools.Default<StringBuilder>().ClearAndFree(builder);
        }

        /// <summary>
        /// Returns the and free.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>System.String.</returns>
        public static string ReturnAndFree(StringBuilder builder)
        {
            SharedPools.Default<StringBuilder>().ForgetTrackedObject(builder);
            return builder.ToString();
        }
    }
}