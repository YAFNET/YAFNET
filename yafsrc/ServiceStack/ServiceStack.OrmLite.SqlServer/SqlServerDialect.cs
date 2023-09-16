// ***********************************************************************
// <copyright file="SqlServerDialect.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.SqlServer;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class SqlServerDialect.
/// </summary>
public static class SqlServerDialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => SqlServer2012Dialect.Instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SqlServerOrmLiteDialectProvider Instance => SqlServer2012Dialect.Instance;
}

/// <summary>
/// Class SqlServer2012Dialect.
/// </summary>
public static class SqlServer2012Dialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => SqlServer2012OrmLiteDialectProvider.Instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SqlServer2012OrmLiteDialectProvider Instance => SqlServer2012OrmLiteDialectProvider.Instance;
}

/// <summary>
/// Class SqlServer2014Dialect.
/// </summary>
public static class SqlServer2014Dialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => SqlServer2014OrmLiteDialectProvider.Instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SqlServer2014OrmLiteDialectProvider Instance => SqlServer2014OrmLiteDialectProvider.Instance;
}

/// <summary>
/// Class SqlServer2016Dialect.
/// </summary>
public static class SqlServer2016Dialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => SqlServer2016OrmLiteDialectProvider.Instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SqlServer2016OrmLiteDialectProvider Instance => SqlServer2016OrmLiteDialectProvider.Instance;
}

/// <summary>
/// Class SqlServer2017Dialect.
/// </summary>
public static class SqlServer2017Dialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => SqlServer2017OrmLiteDialectProvider.Instance;

    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SqlServer2017OrmLiteDialectProvider Instance => SqlServer2017OrmLiteDialectProvider.Instance;
}

/// <summary>
/// Class SqlServer2019Dialect.
/// </summary>
public static class SqlServer2019Dialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => SqlServer2019OrmLiteDialectProvider.Instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SqlServer2019OrmLiteDialectProvider Instance => SqlServer2019OrmLiteDialectProvider.Instance;
}

/// <summary>
/// Class SqlServer2022Dialect.
/// </summary>
public static class SqlServer2022Dialect
{
    /// <summary>
    /// Gets the provider.
    /// </summary>
    /// <value>The provider.</value>
    public static IOrmLiteDialectProvider Provider => SqlServer2022OrmLiteDialectProvider.Instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SqlServer2022OrmLiteDialectProvider Instance => SqlServer2022OrmLiteDialectProvider.Instance;
}