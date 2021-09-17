// ***********************************************************************
// <copyright file="SqlMapper.ITypeHandler.cs" company="ServiceStack, Inc.">
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
        /// Implement this interface to perform custom type-based parameter handling and value parsing
        /// </summary>
        public interface ITypeHandler
        {
            /// <summary>
            /// Assign the value of a parameter before a command executes
            /// </summary>
            /// <param name="parameter">The parameter to configure</param>
            /// <param name="value">Parameter value</param>
            void SetValue(IDbDataParameter parameter, object value);

            /// <summary>
            /// Parse a database value back to a typed value
            /// </summary>
            /// <param name="destinationType">The type to parse to</param>
            /// <param name="value">The value from the database</param>
            /// <returns>The typed value</returns>
            object Parse(Type destinationType, object value);
        }
    }
}
