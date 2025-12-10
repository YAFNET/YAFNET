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
}