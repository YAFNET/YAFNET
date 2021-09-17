// ***********************************************************************
// <copyright file="ICompressor.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack
{
    /// <summary>
    /// Interface ICompressor
    /// </summary>
    public interface ICompressor
    {
        /// <summary>
        /// Compresses the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        string Compress(string source);
    }
}