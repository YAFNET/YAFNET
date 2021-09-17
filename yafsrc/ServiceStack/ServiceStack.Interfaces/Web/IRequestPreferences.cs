// ***********************************************************************
// <copyright file="IRequestPreferences.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Web
{
    /// <summary>
    /// Interface IRequestPreferences
    /// </summary>
    public interface IRequestPreferences
    {
        /// <summary>
        /// Gets a value indicating whether [accepts gzip].
        /// </summary>
        /// <value><c>true</c> if [accepts gzip]; otherwise, <c>false</c>.</value>
        bool AcceptsGzip { get; }

        /// <summary>
        /// Gets a value indicating whether [accepts deflate].
        /// </summary>
        /// <value><c>true</c> if [accepts deflate]; otherwise, <c>false</c>.</value>
        bool AcceptsDeflate { get; }
    }
}