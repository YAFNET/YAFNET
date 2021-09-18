// ***********************************************************************
// <copyright file="ILogFactory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Logging
{
    /// <summary>
    /// Factory to create ILog instances
    /// </summary>
    public interface ILogFactory
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ILog.</returns>
        ILog GetLogger(Type type);

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>ILog.</returns>
        ILog GetLogger(string typeName);
    }
}
