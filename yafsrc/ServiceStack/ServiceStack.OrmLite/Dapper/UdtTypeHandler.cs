// ***********************************************************************
// <copyright file="UdtTypeHandler.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class SqlMapper.
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// A type handler for data-types that are supported by the underlying provider, but which need
    /// a well-known UdtTypeName to be specified
    /// </summary>
    public class UdtTypeHandler : ITypeHandler
    {
        /// <summary>
        /// The udt type name
        /// </summary>
        private readonly string udtTypeName;
        /// <summary>
        /// Creates a new instance of UdtTypeHandler with the specified <see cref="UdtTypeHandler" />.
        /// </summary>
        /// <param name="udtTypeName">The user defined type name.</param>
        /// <exception cref="System.ArgumentException">Cannot be null or empty</exception>
        public UdtTypeHandler(string udtTypeName)
        {
            if (string.IsNullOrEmpty(udtTypeName)) throw new ArgumentException("Cannot be null or empty", udtTypeName);
            this.udtTypeName = udtTypeName;
        }

        /// <summary>
        /// Parse a database value back to a typed value
        /// </summary>
        /// <param name="destinationType">The type to parse to</param>
        /// <param name="value">The value from the database</param>
        /// <returns>The typed value</returns>
        object ITypeHandler.Parse(Type destinationType, object value)
        {
            return value is DBNull ? null : value;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        void ITypeHandler.SetValue(IDbDataParameter parameter, object value)
        {
#pragma warning disable 0618
            parameter.Value = SanitizeParameterValue(value);
#pragma warning restore 0618
            if (!(value is DBNull)) StructuredHelper.ConfigureUDT(parameter, udtTypeName);
        }
    }
}