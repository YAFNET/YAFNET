// ***********************************************************************
// <copyright file="DictionaryRow.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace ServiceStack.OrmLite;

/// <summary>
/// Interface IDynamicRow
/// </summary>
public interface IDynamicRow
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    Type Type { get; }
}

/// <summary>
/// Interface IDynamicRow
/// Implements the <see cref="ServiceStack.OrmLite.IDynamicRow" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ServiceStack.OrmLite.IDynamicRow" />
public interface IDynamicRow<T> : IDynamicRow
{
    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <value>The fields.</value>
    T Fields { get; }
}

/// <summary>
/// Struct DictionaryRow
/// </summary>
public struct DictionaryRow : IDynamicRow<Dictionary<string, object>>
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type { get; }
    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <value>The fields.</value>
    public Dictionary<string, object> Fields { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryRow" /> struct.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="fields">The fields.</param>
    public DictionaryRow(Type type, Dictionary<string, object> fields)
    {
        this.Type = type;
        this.Fields = fields;
    }
}

/// <summary>
/// Struct ObjectRow
/// Implements the <see cref="object" />
/// </summary>
/// <seealso cref="object" />
public struct ObjectRow : IDynamicRow<object>
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public Type Type { get; }
    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <value>The fields.</value>
    public object Fields { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectRow" /> struct.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="fields">The fields.</param>
    public ObjectRow(Type type, object fields)
    {
        this.Type = type;
        this.Fields = fields;
    }
}

/// <summary>
/// Class DynamicRowUtils.
/// </summary>
public static class DynamicRowUtils
{
    /// <param name="row">The row.</param>
    extension(object row)
    {
        /// <summary>
        /// Converts to filtertype.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.Object.</returns>
        internal object ToFilterType<T>()
        {
            return ToFilterType(row, typeof(T));
        }

        /// <summary>
        /// Converts to filtertype.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        internal object ToFilterType(Type type)
        {
            return row == null
                ? null
                : type.IsInstanceOfType(row)
                    ? row
                    : row switch {
                        Dictionary<string, object> obj => new DictionaryRow(type, obj),
                        _ => new ObjectRow(type, row)
                    };
        }
    }
}