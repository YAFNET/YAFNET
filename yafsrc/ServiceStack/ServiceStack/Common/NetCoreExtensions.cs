// ***********************************************************************
// <copyright file="NetCoreExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#if NET7_0_OR_GREATER

using System.Data.Common;
using System.Net.Sockets;

namespace ServiceStack;

/// <summary>
/// Class NetCoreExtensions.
/// </summary>
public static class NetCoreExtensions
{
    /// <summary>
    /// Closes the specified socket.
    /// </summary>
    /// <param name="socket">The socket.</param>
    public static void Close(this Socket socket)
    {
        socket.Dispose();
    }

    /// <summary>
    /// Closes the specified reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    public static void Close(this DbDataReader reader)
    {
        reader.Dispose();
    }
}

#endif