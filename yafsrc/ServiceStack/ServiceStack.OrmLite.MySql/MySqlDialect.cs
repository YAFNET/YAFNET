// ***********************************************************************
// <copyright file="MySqlDialect.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite;

using ServiceStack.OrmLite.MySql;

/// <summary>
/// Class MySqlDialect.
/// </summary>
public static class MySqlDialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => MySqlDialectProvider.Instance;
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static MySqlDialectProvider Instance => MySqlDialectProvider.Instance;
}

/// <summary>
/// Class MySql55Dialect.
/// </summary>
public static class MySql55Dialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => MySql55DialectProvider.Instance;
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static MySql55DialectProvider Instance => MySql55DialectProvider.Instance;
}