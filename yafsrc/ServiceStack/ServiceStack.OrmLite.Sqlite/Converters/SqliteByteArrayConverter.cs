// ***********************************************************************
// <copyright file="SqliteByteArrayConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Sqlite.Converters;

using System;

using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class SqliteByteArrayConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.ByteArrayConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.ByteArrayConverter" />
public class SqliteByteArrayConverter : ByteArrayConverter
{
    /// <summary>
    /// Converts to quotedstring.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public override string ToQuotedString(Type fieldType, object value)
    {
        return "x'" + BitConverter.ToString((byte[])value).Replace("-", "") + "'";
    }
}