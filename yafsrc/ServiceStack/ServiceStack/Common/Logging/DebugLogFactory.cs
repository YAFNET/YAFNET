// ***********************************************************************
// <copyright file="DebugLogFactory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;

namespace ServiceStack.Logging
{
    /// <summary>
    /// Creates a Debug Logger, that logs all messages to: System.Diagnostics.Debug
    /// Made public so its testable
    /// </summary>
    public class DebugLogFactory : ILogFactory
    {
        /// <summary>
        /// The debug enabled
        /// </summary>
        private readonly bool debugEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogFactory"/> class.
        /// </summary>
        /// <param name="debugEnabled">if set to <c>true</c> [debug enabled].</param>
        public DebugLogFactory(bool debugEnabled = true)
        {
            this.debugEnabled = debugEnabled;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ILog.</returns>
        public ILog GetLogger(Type type)
        {
            return new DebugLogger(type) { IsDebugEnabled = debugEnabled };
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>ILog.</returns>
        public ILog GetLogger(string typeName)
        {
            return new DebugLogger(typeName) { IsDebugEnabled = debugEnabled };
        }
    }
}
