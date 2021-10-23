// ***********************************************************************
// <copyright file="SqlMapper.DeserializerState.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;

namespace ServiceStack.OrmLite.Dapper
{
    /// <summary>
    /// Class SqlMapper.
    /// </summary>
    public static partial class SqlMapper
    {
        /// <summary>
        /// Struct DeserializerState
        /// </summary>
        private struct DeserializerState
        {
            /// <summary>
            /// The hash
            /// </summary>
            public readonly int Hash;
            /// <summary>
            /// The function
            /// </summary>
            public readonly Func<IDataReader, object> Func;

            /// <summary>
            /// Initializes a new instance of the <see cref="DeserializerState"/> struct.
            /// </summary>
            /// <param name="hash">The hash.</param>
            /// <param name="func">The function.</param>
            public DeserializerState(int hash, Func<IDataReader, object> func)
            {
                Hash = hash;
                Func = func;
            }
        }
    }
}
