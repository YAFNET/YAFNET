// ***********************************************************************
// <copyright file="IOrmLiteUpsertDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Data;

namespace ServiceStack.OrmLite;

/// <summary>
/// Optional dialect capability for preparing a native, primary-key based UPSERT statement.
/// Dialects which don't implement this interface use OrmLite's Save() behavior instead.
/// </summary>
public interface IOrmLiteUpsertDialectProvider
{
    bool SupportsUpsert { get; }

    void PrepareParameterizedUpsertStatement<T>(
        IDbCommand cmd,
        ICollection<string> insertFields = null,
        ICollection<string> updateOnly = null);
}