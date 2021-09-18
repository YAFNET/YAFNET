// ***********************************************************************
// <copyright file="ParseAsType.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Text
{
    /// <summary>
    /// Enum ParseAsType
    /// </summary>
    [Flags]
    public enum ParseAsType
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The bool
        /// </summary>
        Bool = 2,
        /// <summary>
        /// The byte
        /// </summary>
        Byte = 4,
        /// <summary>
        /// The s byte
        /// </summary>
        SByte = 8,
        /// <summary>
        /// The int16
        /// </summary>
        Int16 = 16,
        /// <summary>
        /// The int32
        /// </summary>
        Int32 = 32,
        /// <summary>
        /// The int64
        /// </summary>
        Int64 = 64,
        /// <summary>
        /// The u int16
        /// </summary>
        UInt16 = 128,
        /// <summary>
        /// The u int32
        /// </summary>
        UInt32 = 256,
        /// <summary>
        /// The u int64
        /// </summary>
        UInt64 = 512,
        /// <summary>
        /// The decimal
        /// </summary>
        Decimal = 1024,
        /// <summary>
        /// The double
        /// </summary>
        Double = 2048,
        /// <summary>
        /// The single
        /// </summary>
        Single = 4096
    }
}