// ***********************************************************************
// <copyright file="ILogWithContext.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Logging
{
    using System;

    /// <summary>
    /// Interface ILogWithContext
    /// Implements the <see cref="ServiceStack.Logging.ILogWithException" />
    /// </summary>
    /// <seealso cref="ServiceStack.Logging.ILogWithException" />
    public interface ILogWithContext : ILogWithException
    {
        /// <summary>
        /// Pushes a property on the current log context, returning an <see cref="IDisposable" />
        /// to remove the property again from the async context.
        /// </summary>
        /// <param name="key">Property Name</param>
        /// <param name="value">Property Value</param>
        /// <returns>Interface for popping the property off the stack</returns>
        IDisposable PushProperty(string key, object value);
    }

    /// <summary>
    /// Class ILogWithContextExtensions.
    /// </summary>
    public static class ILogWithContextExtensions
    {
        /// <summary>
        /// Pushes a property on the current log context, returning an <see cref="IDisposable" />
        /// to remove the property again from the async context.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="key">Property Name</param>
        /// <param name="value">Property Value</param>
        /// <returns>Interface for popping the property off the stack</returns>
        public static IDisposable PushProperty(this ILog logger, string key, object value)
        {
            return (logger as ILogWithContext)?.PushProperty(key, value);
        }
    }
}
