// ***********************************************************************
// <copyright file="OrmLiteConfigOptions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System;

using ServiceStack.Data;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class OrmLiteConfigOptions.
/// </summary>
public class OrmLiteConfigOptions
{
    public IDbConnectionFactory? DbFactory { private set; get; }
    
    public void Init(string connectionString, IOrmLiteDialectProvider dialectProvider)
    {
        if (DbFactory != null)
        {
            throw new InvalidOperationException("DbFactory is already set");
        }

        DbFactory = new OrmLiteConnectionFactory(connectionString, dialectProvider);
    }
}

public class OrmLiteConfigurationBuilder(IDbConnectionFactory dbFactory)
{
    public IDbConnectionFactory DbFactory { get; } = dbFactory;
    
    public OrmLiteConfigurationBuilder AddConnection(string name, string? connectionString, IOrmLiteDialectProvider? dialectProvider = null)
    {
        DbFactory.RegisterConnection(name, connectionString, dialectProvider ?? OrmLiteConfig.DialectProvider);
        return this;
    }
}
