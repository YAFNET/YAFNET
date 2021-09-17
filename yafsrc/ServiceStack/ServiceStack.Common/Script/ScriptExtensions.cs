// ***********************************************************************
// <copyright file="ScriptExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.Script
{
    /// <summary>
    /// Class ScriptExtensions.
    /// </summary>
    public static class ScriptExtensions
    {
        /// <summary>
        /// Ins the stop filter.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ServiceStack.Script.StopFilterExecutionException"></exception>
        public static object InStopFilter(this Exception ex, ScriptScopeContext scope, object options)
        {
            throw new StopFilterExecutionException(scope, options, ex);
        }

        /// <summary>
        /// Ases the string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        public static string AsString(this object str) => str is IRawString r ? r.ToRawString() : str?.ToString();
    }
}