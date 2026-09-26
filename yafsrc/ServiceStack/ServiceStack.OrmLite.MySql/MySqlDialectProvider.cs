// ***********************************************************************
// <copyright file="MySqlDialectProvider.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.MySql;

using System;
using System.Collections.Generic;
using System.Data;

using global::MySql.Data.MySqlClient;

using ServiceStack.OrmLite.MySql.Converters;

/// <summary>
/// Class MySqlDialectProvider.
/// Implements the <see cref="MySqlDialectProvider" />
/// </summary>
/// <seealso cref="MySqlDialectProvider" />
public class MySqlDialectProvider : MySqlDialectProviderBase<MySqlDialectProvider>
{
    /// <summary>
    /// The instance
    /// </summary>
    public static MySqlDialectProvider Instance = new();

    /// <summary>
    /// The text column definition
    /// </summary>
    private const string TextColumnDefinition = "TEXT";

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="options">The options.</param>
    /// <returns>IDbConnection.</returns>
    public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
    {
        return new MySqlConnection(connectionString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlDialectProvider" /> class.
    /// </summary>
    public MySqlDialectProvider()
    {
        this.RegisterConverter<DateTime>(new MySqlDateTimeConverter());
    }

    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <returns>IDbDataParameter.</returns>
    public override IDbDataParameter CreateParam()
    {
        return new MySqlParameter();
    }

    public override void BulkInsert<T>(IDbConnection db, IEnumerable<T> objs, BulkInsertConfig config = null)
    {
        config ??= new();
        if (config.Mode == BulkInsertMode.Sql)
        {
            base.BulkInsert(db, objs, config);
            return;
        }

        using var fs = CreateTempFileStream();
        CreateBulkLoader(db, objs, fs).Load(fs);
    }

    public async override Task BulkInsertAsync<T>(IDbConnection db, IEnumerable<T> objs, BulkInsertConfig config = null, CancellationToken token = default)
    {
        config ??= new BulkInsertConfig();
        if (config.Mode == BulkInsertMode.Sql)
        {
            await base.BulkInsertAsync(db, objs, config, token).ConfigAwait();
            return;
        }

        await using var fs = CreateTempFileStream();
        await CreateBulkLoader(db, objs, fs).LoadAsync(fs, token).ConfigAwait();
    }

    private static FileStream CreateTempFileStream() => new(Path.GetTempFileName(),
        FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);

    private static MySqlBulkLoader CreateBulkLoader<T>(IDbConnection db, IEnumerable<T> objs, Stream stream)
    {
        var mysqlConn = (MySqlConnection)db.ToDbConnection();

        CsvSerializer.SerializeToStream(objs, stream);
        stream.Position = 0;

        var dialect = db.Dialect();
        var modelDef = ModelDefinition<T>.Definition;

        var bulkLoader = new MySqlBulkLoader(mysqlConn)
        {
            Local = true,
            TableName = dialect.GetQuotedTableName(modelDef),
            CharacterSet = "UTF8",
            NumberOfLinesToSkip = 1,
            FieldTerminator = ",",
            FieldQuotationCharacter = '"',
            FieldQuotationOptional = true,
            EscapeCharacter = '\\',
            LineTerminator = Environment.NewLine
        };

        var columns = CsvSerializer.PropertiesFor<T>()
            .Select(x => dialect.GetQuotedColumnName(modelDef.GetFieldDefinition(x.PropertyName)));
        bulkLoader.Columns.AddRange(columns);
        return bulkLoader;
    }
}



/// <summary>
/// Class MySql55DialectProvider.
/// Implements the <see cref="MySqlDialectProvider" />
/// </summary>
/// <seealso cref="MySqlDialectProvider" />
public class MySql55DialectProvider : MySqlDialectProviderBase<MySqlDialectProvider>
{
    /// <summary>
    /// The instance
    /// </summary>
    public static MySql55DialectProvider Instance = new();

    /// <summary>
    /// The text column definition
    /// </summary>
    private const string TextColumnDefinition = "TEXT";

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="options">The options.</param>
    /// <returns>IDbConnection.</returns>
    public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
    {
        return new MySqlConnection(connectionString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySql55DialectProvider" /> class.
    /// </summary>
    public MySql55DialectProvider()
    {
        this.RegisterConverter<DateTime>(new MySql55DateTimeConverter());
        this.RegisterConverter<string>(new MySql55StringConverter());
        this.RegisterConverter<char[]>(new MySql55CharArrayConverter());
    }

    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <returns>IDbDataParameter.</returns>
    public override IDbDataParameter CreateParam()
    {
        return new MySqlParameter();
    }
}