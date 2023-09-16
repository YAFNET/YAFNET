// ***********************************************************************
// <copyright file="SqlServerOrmLiteDialectProviderVersions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.SqlServer.Converters;

namespace ServiceStack.OrmLite.SqlServer;

public class SqlServer2016OrmLiteDialectProvider : SqlServer2014OrmLiteDialectProvider
{
    public SqlServer2016OrmLiteDialectProvider() : base()
    {
        base.RegisterConverter<string>(new SqlServerJsonStringConverter());
    }

    public static new SqlServer2016OrmLiteDialectProvider Instance = new();

    public override SqlExpression<T> SqlExpression<T>() => new SqlServer2016Expression<T>(this);
}

public class SqlServer2017OrmLiteDialectProvider : SqlServer2016OrmLiteDialectProvider
{
    public static new SqlServer2017OrmLiteDialectProvider Instance = new();
}

public class SqlServer2019OrmLiteDialectProvider : SqlServer2017OrmLiteDialectProvider
{
    public static new SqlServer2019OrmLiteDialectProvider Instance = new();
}

public class SqlServer2022OrmLiteDialectProvider : SqlServer2019OrmLiteDialectProvider
{
    public static new SqlServer2022OrmLiteDialectProvider Instance = new();
}