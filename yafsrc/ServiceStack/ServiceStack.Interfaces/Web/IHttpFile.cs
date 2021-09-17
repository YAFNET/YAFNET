// ***********************************************************************
// <copyright file="IHttpFile.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.IO;

namespace ServiceStack.Web
{
    /// <summary>
    /// Interface IHttpFile
    /// </summary>
    public interface IHttpFile
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        string FileName { get; }
        /// <summary>
        /// Gets the length of the content.
        /// </summary>
        /// <value>The length of the content.</value>
        long ContentLength { get; }
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        string ContentType { get; }
        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <value>The input stream.</value>
        Stream InputStream { get; }
    }
}