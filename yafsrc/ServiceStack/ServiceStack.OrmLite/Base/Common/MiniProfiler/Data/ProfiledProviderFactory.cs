// ***********************************************************************
// <copyright file="ProfiledProviderFactory.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.MiniProfiler.Data;

using System.Data.Common;

/// <summary>
/// Wrapper for a db provider factory to enable profiling
/// </summary>
public class ProfiledProviderFactory : DbProviderFactory
{
    /// <summary>
    /// Every provider factory must have an Instance public field
    /// </summary>
    public static ProfiledProviderFactory Instance { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfiledProviderFactory" /> class.
    /// </summary>
    protected ProfiledProviderFactory() { }

    /// <summary>
    /// Gets the profiler.
    /// </summary>
    /// <value>The profiler.</value>
    protected IDbProfiler Profiler { get; }

    /// <summary>
    /// Gets the wrapped factory.
    /// </summary>
    /// <value>The wrapped factory.</value>
    protected DbProviderFactory WrappedFactory { get; }

    /// <summary>
    /// proxy
    /// </summary>
    /// <param name="profiler">The profiler.</param>
    /// <param name="wrappedFactory">The wrapped factory.</param>
    public ProfiledProviderFactory(IDbProfiler profiler, DbProviderFactory wrappedFactory)
    {
        this.Profiler = profiler;
        this.WrappedFactory = wrappedFactory;
    }

#if !NET9_0_OR_GREATER
    /// <summary>
    /// proxy
    /// </summary>
    /// <value><c>true</c> if this instance can create data source enumerator; otherwise, <c>false</c>.</value>
    public override bool CanCreateDataSourceEnumerator => WrappedFactory.CanCreateDataSourceEnumerator;

    /// <summary>
    /// proxy
    /// </summary>
    /// <returns>A new instance of <see cref="T:System.Data.Common.DbDataSourceEnumerator" />.</returns>
    public override DbDataSourceEnumerator CreateDataSourceEnumerator() =>
        WrappedFactory.CreateDataSourceEnumerator();
#endif

    /// <summary>
    /// proxy
    /// </summary>
    /// <returns>A new instance of <see cref="T:System.Data.Common.DbCommand" />.</returns>
    public override DbCommand CreateCommand()
    {
        return new ProfiledCommand(this.WrappedFactory.CreateCommand(), null, this.Profiler);
    }

    /// <summary>
    /// proxy
    /// </summary>
    /// <returns>A new instance of <see cref="T:System.Data.Common.DbConnection" />.</returns>
    public override DbConnection CreateConnection()
    {
        return new ProfiledConnection(this.WrappedFactory.CreateConnection(), this.Profiler);
    }

    /// <summary>
    /// proxy
    /// </summary>
    /// <returns>A new instance of <see cref="T:System.Data.Common.DbParameter" />.</returns>
    public override DbParameter CreateParameter()
    {
        return this.WrappedFactory.CreateParameter();
    }

    /// <summary>
    /// proxy
    /// </summary>
    /// <returns>A new instance of <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</returns>
    public override DbConnectionStringBuilder CreateConnectionStringBuilder()
    {
        return this.WrappedFactory.CreateConnectionStringBuilder();
    }

#if !NET9_0_OR_GREATER
    /// <summary>
    /// proxy
    /// </summary>
    /// <returns>A new instance of <see cref="T:System.Data.Common.DbCommandBuilder" />.</returns>
    public override DbCommandBuilder CreateCommandBuilder() =>
        WrappedFactory.CreateCommandBuilder();

    /// <summary>
    /// proxy
    /// </summary>
    /// <returns>A new instance of <see cref="T:System.Data.Common.DbDataAdapter" />.</returns>
    public override DbDataAdapter CreateDataAdapter() =>
        WrappedFactory.CreateDataAdapter();

    /// <summary>
    /// proxy
    /// </summary>
    /// <param name="state">One of the <see cref="T:System.Security.Permissions.PermissionState" /> values.</param>
    /// <returns>A <see cref="T:System.Security.CodeAccessPermission" /> object for the specified <see cref="T:System.Security.Permissions.PermissionState" />.</returns>
    public override System.Security.CodeAccessPermission CreatePermission(System.Security.Permissions.PermissionState state) =>
        WrappedFactory.CreatePermission(state);
#endif
}