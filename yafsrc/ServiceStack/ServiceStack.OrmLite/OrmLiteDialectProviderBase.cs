// ***********************************************************************
// <copyright file="OrmLiteDialectProviderBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using ServiceStack.OrmLite.Base.Common;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ServiceStack.DataAnnotations;
using ServiceStack.Logging;
using ServiceStack.OrmLite.Converters;

/// <summary>
/// Class OrmLiteDialectProviderBase.
/// Implements the <see cref="ServiceStack.OrmLite.IOrmLiteDialectProvider" />
/// </summary>
/// <typeparam name="TDialect">The type of the t dialect.</typeparam>
/// <seealso cref="ServiceStack.OrmLite.IOrmLiteDialectProvider" />
public abstract class OrmLiteDialectProviderBase<TDialect>
    : IOrmLiteDialectProvider, IOrmLiteUpsertDialectProvider
    where TDialect : IOrmLiteDialectProvider
{
    /// <summary>
    /// The log
    /// </summary>
    protected readonly static ILog Log = LogManager.GetLogger(typeof(IOrmLiteDialectProvider));
    public virtual DbKind Kind => DbKind.Unknown;

    /// <summary>
    /// Gets or sets the maximum number of params used in an IN list before it's split into batches
    /// or replaced with a single array param by dialects that support it
    /// </summary>
    /// <value>The maximum in list parameters.</value>
    public int MaxInListParams { get; set; } = 1000;

    /// <summary>
    /// Whether InsertAll, UpdateAll, UpsertAll and SaveAll send their rows with DbBatch, where the ADO.NET provider supports it
    /// </summary>
    /// <value><c>true</c> if [use database batch]; otherwise, <c>false</c>.</value>
    public bool UseDbBatch { get; set; } = true;

    /// <summary>
    /// The maximum number of statements sent in a single DbBatch
    /// </summary>
    /// <value>The size of the batch.</value>
    public int BatchSize { get; set; } = 1000;

    /// <summary>
    /// Whether the ADO.NET provider returns the rows affected by each statement of a batch
    /// </summary>
    /// <value><c>true</c> if [supports batch rows affected]; otherwise, <c>false</c>.</value>
    public virtual bool SupportsBatchRowsAffected => true;

    private OrmLiteRetryPolicy retryPolicy;
    /// <summary>
    /// Null when nothing is retried, e.g. with OrmLiteRetry.None, so statements skip retrying as they do without a
    /// policy
    /// </summary>
    public OrmLiteRetryPolicy RetryPolicy
    {
        get
        {
            if (!this.SupportsRetries)
            {
                return null;
            }

            var policy = this.retryPolicy ?? OrmLiteConfig.RetryPolicy;
            return policy?.MaxRetries > 0 ? policy : null;
        }
        set => this.retryPolicy = value;
    }

    public virtual bool SupportsRetries => true;

    /// <summary>
    /// Recognizes a lost connection, dialects override it to recognize their driver's temporary errors
    /// </summary>
    public virtual TransientError GetTransientError(Exception ex) =>
        ex is System.Data.Common.DbException && OrmLiteRetryExec.IsConnectionLost(ex)
            ? TransientError.MaybeApplied
            : TransientError.None;

    /// <summary>
    /// Converts to a statement returning the affected rows of an UPDATE or DELETE statement.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="isDelete">if set to <c>true</c> [is delete].</param>
    /// <param name="returnFields">The fields to return, or null for all fields.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public virtual string ToReturningStatement(string sql, ModelDefinition modelDef, bool isDelete, ICollection<FieldDefinition> returnFields = null) =>
        throw new NotSupportedException($"{GetType().Name} doesn't support returning rows from UPDATE and DELETE statements");

    /// <summary>
    /// The query that returns the query plan of the SQL
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="sql">The SQL.</param>
    /// <param name="analyze">if set to <c>true</c> runs the query to return the actual plan.</param>
    /// <returns>ExplainQuery.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public virtual ExplainQuery ToExplainQuery(IDbConnection db, string sql, bool analyze) =>
        throw new NotSupportedException($"{this.GetType().Name} doesn't support returning query plans");

    /// <summary>
    /// The columns of a RETURNING clause: all the table's columns, or only the returnFields
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="returnFields">The return fields.</param>
    /// <returns>System.String.</returns>
    protected virtual string GetReturningColumns(ModelDefinition modelDef, ICollection<FieldDefinition> returnFields)
    {
        if (returnFields == null || returnFields.Count == 0)
        {
            return this.GetColumnNames(modelDef);
        }

        // The table's columns are in the same order as its fields
        var allColumns = this.GetColumnNames(modelDef, null);
        var sqlColumns = new List<SelectItem>();
        for (var i = 0; i < allColumns.Length; i++)
        {
            if (returnFields.Contains(modelDef.FieldDefinitions[i]))
            {
                sqlColumns.Add(allColumns[i]);
            }
        }

        return sqlColumns.ToArray().ToSelectString();
    }
    /* ADO.NET UNDERSTOOD DATA TYPES:
                COUNTER	DbType.Int64
                AUTOINCREMENT	DbType.Int64
                IDENTITY	DbType.Int64
                LONG	DbType.Int64
                TINYINT	DbType.Byte
                INTEGER	DbType.Int64
                INT	DbType.Int32
                VARCHAR	DbType.String
                NVARCHAR	DbType.String
                CHAR	DbType.String
                NCHAR	DbType.String
                TEXT	DbType.String
                NTEXT	DbType.String
                STRING	DbType.String
                DOUBLE	DbType.Double
                FLOAT	DbType.Double
                REAL	DbType.Single
                BIT	DbType.Boolean
                YESNO	DbType.Boolean
                LOGICAL	DbType.Boolean
                BOOL	DbType.Boolean
                NUMERIC	DbType.Decimal
                DECIMAL	DbType.Decimal
                MONEY	DbType.Decimal
                CURRENCY	DbType.Decimal
                TIME	DbType.DateTime
                DATE	DbType.DateTime
                TIMESTAMP	DbType.DateTime
                DATETIME	DbType.DateTime
                BLOB	DbType.Binary
                BINARY	DbType.Binary
                VARBINARY	DbType.Binary
                IMAGE	DbType.Binary
                GENERAL	DbType.Binary
                OLEOBJECT	DbType.Binary
                GUID	DbType.Guid
                UNIQUEIDENTIFIER	DbType.Guid
                MEMO	DbType.String
                NOTE	DbType.String
                LONGTEXT	DbType.String
                LONGCHAR	DbType.String
                SMALLINT	DbType.Int16
                BIGINT	DbType.Int64
                LONGVARCHAR	DbType.String
                SMALLDATE	DbType.DateTime
                SMALLDATETIME	DbType.DateTime
             */

    /// <summary>
    /// Initializes the column type map.
    /// </summary>
    protected void InitColumnTypeMap()
    {
        this.EnumConverter = new EnumConverter();
        this.RowVersionConverter = new RowVersionConverter();
        this.ReferenceTypeConverter = new ReferenceTypeConverter();
        this.ValueTypeConverter = new ValueTypeConverter();

        this.RegisterConverter<string>(new StringConverter());
        this.RegisterConverter<char>(new CharConverter());
        this.RegisterConverter<char[]>(new CharArrayConverter());
        this.RegisterConverter<byte[]>(new ByteArrayConverter());

        this.RegisterConverter<byte>(new ByteConverter());
        this.RegisterConverter<sbyte>(new SByteConverter());
        this.RegisterConverter<short>(new Int16Converter());
        this.RegisterConverter<ushort>(new UInt16Converter());
        this.RegisterConverter<int>(new Int32Converter());
        this.RegisterConverter<uint>(new UInt32Converter());
        this.RegisterConverter<long>(new Int64Converter());
        this.RegisterConverter<ulong>(new UInt64Converter());

        this.RegisterConverter<ulong>(new UInt64Converter());

        this.RegisterConverter<float>(new FloatConverter());
        this.RegisterConverter<double>(new DoubleConverter());
        this.RegisterConverter<decimal>(new DecimalConverter());

        this.RegisterConverter<Guid>(new GuidConverter());
        this.RegisterConverter<TimeSpan>(new TimeSpanAsIntConverter());
        this.RegisterConverter<DateTime>(new DateTimeConverter());
        this.RegisterConverter<DateTimeOffset>(new DateTimeOffsetConverter());

#if NET10_0_OR_GREATER
        this.RegisterConverter<DateOnly>(new DateOnlyConverter());
        this.RegisterConverter<TimeOnly>(new TimeOnlyConverter());
#endif
    }

    /// <summary>
    /// Use JSON for serializing Complex Types, which lets typed queries read their properties, e.g:
    /// <para>db.Select&lt;Customer&gt;(x => x.Address.City == "London")</para>
    /// </summary>
    public virtual bool UseJson
    {
        get => this.StringSerializer.IsJsonSerializer();
        set => this.StringSerializer = value ? new JsonStringSerializer() : new JsvStringSerializer();
    }

    /// <summary>
    /// Gets the column type definition.
    /// </summary>
    /// <param name="columnType">Type of the column.</param>
    /// <param name="fieldLength">Length of the field.</param>
    /// <param name="scale">The scale.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="ArgumentException">$"{converter.GetType().Name} requires a ColumnDefinition</exception>
    /// <exception cref="System.ArgumentException"></exception>
    public string GetColumnTypeDefinition(Type columnType, int? fieldLength, int? scale)
    {
        var converter = this.GetConverter(columnType);
        if (converter != null)
        {
            if (converter is IHasColumnDefinitionPrecision customPrecisionConverter)
            {
                return customPrecisionConverter.GetColumnDefinition(fieldLength, scale);
            }

            if (converter is IHasColumnDefinitionLength customLengthConverter)
            {
                return customLengthConverter.GetColumnDefinition(fieldLength);
            }

            if (string.IsNullOrEmpty(converter.ColumnDefinition))
            {
                throw new ArgumentException($"{converter.GetType().Name} requires a ColumnDefinition");
            }

            return converter.ColumnDefinition;
        }

        var stringConverter = columnType.IsRefType() ? this.ReferenceTypeConverter :
                              columnType.IsEnum ? this.EnumConverter : (IHasColumnDefinitionLength)this.ValueTypeConverter;

        return stringConverter.GetColumnDefinition(fieldLength);
    }

    /// <summary>
    /// Initializes the database parameter.
    /// </summary>
    /// <param name="dbParam">The database parameter.</param>
    /// <param name="columnType">Type of the column.</param>
    public virtual void InitDbParam(IDbDataParameter dbParam, Type columnType)
    {
        var converter = this.GetConverterBestMatch(columnType);
        converter.InitDbParam(dbParam, columnType);
    }

    /// <summary>
    /// Creates the parameter.
    /// </summary>
    /// <returns>IDbDataParameter.</returns>
    public abstract IDbDataParameter CreateParam();

    /// <summary>
    /// Gets the variables.
    /// </summary>
    /// <value>The variables.</value>
    public Dictionary<string, string> Variables { get; set; } = [];

    /// <summary>
    /// Gets or sets the execute filter.
    /// </summary>
    /// <value>The execute filter.</value>
    public IOrmLiteExecFilter ExecFilter { get; set; }

    /// <summary>
    /// The converters
    /// </summary>
    public Dictionary<Type, IOrmLiteConverter> Converters = [];

    /// <summary>
    /// The automatic increment definition
    /// </summary>
    public string AutoIncrementDefinition = "AUTOINCREMENT"; // SqlServer express limit

    /// <summary>
    /// Gets the decimal converter.
    /// </summary>
    /// <value>The decimal converter.</value>
    public DecimalConverter DecimalConverter => (DecimalConverter)this.Converters[typeof(decimal)];

    /// <summary>
    /// Gets the string converter.
    /// </summary>
    /// <value>The string converter.</value>
    public StringConverter StringConverter => (StringConverter)this.Converters[typeof(string)];

    /// <summary>
    /// Invoked when a DB Connection is opened
    /// </summary>
    /// <value>The on open connection.</value>
    public Action<IDbConnection> OnOpenConnection { get; set; }

    /// <summary>
    /// Gets or sets the on dispose connection.
    /// </summary>
    /// <value>The on dispose connection.</value>
    public Action<IDbConnection> OnDisposeConnection { get; set; }

    /// <summary>
    /// Gets or sets the on before execute non query.
    /// </summary>
    /// <value>The on before execute non query.</value>
    public Action<IDbCommand> OnBeforeExecuteNonQuery { get; set; }

    /// <summary>
    /// Gets or sets the on after execute non query.
    /// </summary>
    /// <value>The on after execute non query.</value>
    public Action<IDbCommand> OnAfterExecuteNonQuery { get; set; }

    /// <summary>
    /// The one time connection commands run
    /// </summary>
    internal int OneTimeConnectionCommandsRun;

    /// <summary>
    /// Enable Bulk Inserts from CSV files
    /// </summary>
    public bool AllowLoadLocalInfile
    {
        set => this.OneTimeConnectionCommands.Add($"SET GLOBAL LOCAL_INFILE={value.ToString().ToUpper()};");
    }

    /// <summary>
    /// Gets the one time connection commands.
    /// </summary>
    /// <value>The one time connection commands.</value>
    public List<string> OneTimeConnectionCommands { get; } = [];

    /// <summary>
    /// Gets the connection commands.
    /// </summary>
    /// <value>The connection commands.</value>
    public List<string> ConnectionCommands { get; } = [];

    /// <summary>
    /// Gets or sets the parameter string.
    /// </summary>
    /// <value>The parameter string.</value>
    public string ParamString { get; set; } = "@";

    /// <summary>
    /// Gets or sets the naming strategy.
    /// </summary>
    /// <value>The naming strategy.</value>
    public INamingStrategy NamingStrategy { get; set; } = new OrmLiteDefaultNamingStrategy();

    /// <summary>
    /// Gets or sets the string serializer.
    /// </summary>
    /// <value>The string serializer.</value>
    public IStringSerializer StringSerializer { get; set; } = new JsvStringSerializer();

    /// <summary>
    /// Gets or sets the parameter name filter.
    /// </summary>
    /// <value>The parameter name filter.</value>
    public Func<string, string> ParamNameFilter
    {
        get => field ?? OrmLiteConfig.ParamNameFilter;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether [supports schema].
    /// </summary>
    /// <value><c>true</c> if [supports schema]; otherwise, <c>false</c>.</value>
    public virtual bool SupportsSchema => true;

    /// <summary>
    /// The default value format
    /// </summary>
    public string DefaultValueFormat = " DEFAULT ({0})";

    /// <summary>
    /// Gets or sets the enum converter.
    /// </summary>
    /// <value>The enum converter.</value>
    public EnumConverter EnumConverter
    {
        get;
        set
        {
            value.DialectProvider = this;
            field = value;
        }
    }

    /// <summary>
    /// Gets or sets the row version converter.
    /// </summary>
    /// <value>The row version converter.</value>
    public RowVersionConverter RowVersionConverter
    {
        get;
        set
        {
            value.DialectProvider = this;
            field = value;
        }
    }

    /// <summary>
    /// Gets or sets the reference type converter.
    /// </summary>
    /// <value>The reference type converter.</value>
    public ReferenceTypeConverter ReferenceTypeConverter
    {
        get;
        set
        {
            value.DialectProvider = this;
            field = value;
        }
    }

    /// <summary>
    /// Gets or sets the value type converter.
    /// </summary>
    /// <value>The value type converter.</value>
    public ValueTypeConverter ValueTypeConverter
    {
        get;
        set
        {
            value.DialectProvider = this;
            field = value;
        }
    }

    /// <summary>
    /// Removes the converter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RemoveConverter<T>()
    {
        if (this.Converters.TryRemove(typeof(T), out var converter))
        {
            converter.DialectProvider = null;
        }
    }

    /// <summary>
    /// Initializes the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public virtual void Init(string connectionString) { }

    /// <summary>
    /// Registers the converter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="converter">The converter.</param>
    /// <exception cref="ArgumentNullException">nameof(converter)</exception>
    /// <exception cref="System.ArgumentNullException">converter</exception>
    public void RegisterConverter<T>(IOrmLiteConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

        converter.DialectProvider = this;
        this.Converters[typeof(T)] = converter;
    }

    /// <summary>
    /// Gets the explicit Converter registered for a specific type
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>IOrmLiteConverter.</returns>
    public IOrmLiteConverter GetConverter(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return this.Converters.GetValueOrDefault(type);
    }

    /// <summary>
    /// Shoulds the quote value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public virtual bool ShouldQuoteValue(Type fieldType)
    {
        var converter = this.GetConverter(fieldType);
        return converter is null or NativeValueOrmLiteConverter;
    }

    /// <summary>
    /// From the database row version.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public virtual object FromDbRowVersion(Type fieldType, object value)
    {
        return this.RowVersionConverter.FromDbValue(fieldType, value);
    }

    /// <summary>
    /// Return best matching converter, falling back to Enum, Value or Ref Type Converters
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>IOrmLiteConverter.</returns>
    public IOrmLiteConverter GetConverterBestMatch(Type type)
    {
        if (type == typeof(RowVersionConverter))
        {
            return this.RowVersionConverter;
        }

        var converter = this.GetConverter(type);
        if (converter != null)
        {
            return converter;
        }

        if (type.IsEnum)
        {
            return this.EnumConverter;
        }

        return type.IsRefType() ? this.ReferenceTypeConverter : this.ValueTypeConverter;
    }

    /// <summary>
    /// Gets the converter best match.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>IOrmLiteConverter.</returns>
    public virtual IOrmLiteConverter GetConverterBestMatch(FieldDefinition fieldDef)
    {
        var fieldType = Nullable.GetUnderlyingType(fieldDef.FieldType) ?? fieldDef.FieldType;

        if (fieldDef.IsRowVersion)
        {
            return this.RowVersionConverter;
        }

        if (fieldDef.VectorDimensions != null)
        {
            return this.VectorConverter;
        }

        if (this.Converters.TryGetValue(fieldType, out var converter))
        {
            return converter;
        }

        if (fieldType.IsEnum)
        {
            return this.EnumConverter;
        }

        return fieldType.IsRefType() ? this.ReferenceTypeConverter : this.ValueTypeConverter;
    }

    /// <summary>
    /// Converts to dbvalue.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public virtual object ToDbValue(object value, Type type)
    {
        if (value is null or DBNull)
        {
            return null;
        }

        var converter = this.GetConverterBestMatch(type);
        try
        {
            return converter.ToDbValue(type, value);
        }
        catch (Exception ex)
        {
            Log.Error(
                $"Error in {converter.GetType().Name}.ToDbValue() value '{value.GetType().Name}' and Type '{type.Name}'",
                ex);
            throw;
        }
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public virtual object FromDbValue(object value, Type type)
    {
        if (value is null or DBNull)
        {
            return null;
        }

        var converter = this.GetConverterBestMatch(type);
        try
        {
            return converter.FromDbValue(type, value);
        }
        catch (Exception ex)
        {
            Log.Error(
                $"Error in {converter.GetType().Name}.FromDbValue() value '{value.GetType().Name}' and Type '{type.Name}'",
                ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Object.</returns>
    public object GetValue(IDataReader reader, int columnIndex, Type type)
    {
        // if (this.Converters.TryGetValue(type, out var converter))
        // Resolve Nullable<T> to T's converter (as GetConverter does), otherwise e.g. a DateTime? column
        // in SQLite (stored as TEXT) is read raw as a string and fails in the converter's FromDbValue cast
        var converter = this.GetConverter(type);
        if (converter != null)
        {
            return converter.GetValue(reader, columnIndex, null);
        }

        return reader.GetValue(columnIndex);
    }

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="values">The values.</param>
    /// <returns>System.Int32.</returns>
    public virtual int GetValues(IDataReader reader, object[] values)
    {
        return reader.GetValues(values);
    }

    /// <summary>
    /// Creates the connection.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="options">The options.</param>
    /// <returns>IDbConnection.</returns>
    public abstract IDbConnection CreateConnection(string filePath, Dictionary<string, string> options);

    /// <summary>
    /// Returns an unquoted table name (inc schema if exists), using naming strategy 
    /// </summary>
    public virtual string UnquotedTable(TableRef tableRef)
    {
        if (tableRef.QuotedName != null)
        {
            return tableRef.QuotedName.Replace("\"", "");
        }

        var schema = tableRef.GetSchemaName();
        var alias = tableRef.ModelDef?.Alias;
        if (alias != null)
        {
            return schema != null
                ? this.JoinSchema(this.NamingStrategy.GetSchemaName(schema), this.NamingStrategy.GetTableAlias(alias))
                : this.NamingStrategy.GetTableAlias(alias);
        }

        var tableName = tableRef.GetTableName();
        return schema != null
            ? this.JoinSchema(this.NamingStrategy.GetSchemaName(schema), this.NamingStrategy.GetTableName(tableName))
            : this.NamingStrategy.GetTableName(tableName);
    }

    /// <summary>
    /// Returns an quoted table name (inc schema if exists), using naming strategy 
    /// </summary>
    public virtual string QuoteTable(TableRef tableRef)
    {
        var useVerbatim = tableRef.QuotedName;
        if (useVerbatim != null)
        {
            return useVerbatim;
        }

        if (tableRef.ModelDef != null)
        {
            return this.GetQuotedTableName(tableRef.ModelDef);
        }

        var schema = tableRef.GetSchemaName();
        return schema != null
            ? this.QuoteSchema(this.NamingStrategy.GetSchemaName(schema), this.NamingStrategy.GetTableName(tableRef.Name))
            : tableRef.Name != null
                ? this.GetQuotedName(this.NamingStrategy.GetTableName(tableRef.Name))
                : null;
    }

    /// <summary>
    /// Return unquoted table name only (i.e. without schema), using naming strategy
    /// </summary>
    /// <param name="tableRef"></param>
    /// <returns></returns>
    public virtual string GetTableNameOnly(TableRef tableRef)
    {
        return tableRef.QuotedName?.LastRightPart('.').StripDbQuotes() ??
               (tableRef.ModelDef != null
                   ? this.NamingStrategy.GetTableName(tableRef.ModelDef)
                   : this.NamingStrategy.GetTableName(tableRef.Name));
    }

    public virtual string GetSchemaName(TableRef tableRef)
    {
        return this.NamingStrategy.GetSchemaName(tableRef.GetSchemaName());
    }

    /// <summary>
    /// Gets the table name with brackets.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.String.</returns>
    public virtual string GetTableNameWithBrackets<T>()
    {
        var modelDef = typeof(T).GetModelDefinition();
        return this.GetTableNameWithBrackets(modelDef.ModelName, modelDef.Schema);
    }

    /// <summary>
    /// Gets the table name with brackets.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetTableNameWithBrackets(ModelDefinition modelDef)
    {
        return this.GetTableNameWithBrackets(modelDef.ModelName, modelDef.Schema);
    }

    /// <summary>
    /// Gets the table name with brackets.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public virtual string GetTableNameWithBrackets(string tableName, string schema = null)
    {
        return $"[{this.NamingStrategy.GetSchemaName(schema)}].[{this.NamingStrategy.GetTableName(tableName)}]";
    }

    /// <summary>
    /// Gets the name of the quoted table.
    /// </summary>
    /// <param name="modelType">Type of the model.</param>
    /// <returns>System.String.</returns>
    public virtual string GetQuotedTableName(Type modelType)
    {
        return this.GetQuotedTableName(modelType.GetModelDefinition());
    }

    /// <summary>
    /// Gets the name of the quoted table.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetQuotedTableName(ModelDefinition modelDef)
    {
        if (modelDef == null)
        {
            return null;
        }

        var schema = modelDef.Schema;
        if (modelDef.Alias != null)
        {
            return schema == null
                ? this.GetQuotedName(this.NamingStrategy.GetTableAlias(modelDef.Alias))
                : this.QuoteSchema(this.NamingStrategy.GetSchemaName(schema), this.NamingStrategy.GetTableAlias(modelDef.Alias));
        }

        return schema == null
            ? this.GetQuotedName(this.NamingStrategy.GetTableName(modelDef.Name))
            : this.QuoteSchema(this.NamingStrategy.GetSchemaName(schema), this.NamingStrategy.GetTableName(modelDef.Name));
    }

    /// <summary>
    /// Return a quoted schema + table name (does not use naming strategy)
    /// </summary>
    public virtual string QuoteSchema(string schema, string table)
    {
        if (string.IsNullOrEmpty(schema))
        {
            return string.IsNullOrEmpty(table)
                ? null
                : this.GetQuotedName(table);
        }

        return JoinSchema(QuoteSchemaName(schema), GetQuotedName(table));
    }

    /// <summary>
    /// Quotes each part of a multi-part schema name individually, e.g. db.dbo => "db"."dbo"
    /// </summary>
    public virtual string QuoteSchemaName(string schema) => schema == null ? null : schema.IndexOf('.') >= 0
        ? string.Join(".", schema.Split('.').Select(GetQuotedName))
        : GetQuotedName(schema);

    public virtual string JoinSchema(string schema, string table)
    {
        return schema != null
            ? schema + "." + table
            : table;
    }

    public virtual string GetQuotedColumnName(FieldDefinition fieldDef)
    {
        if (fieldDef == null)
        {
            return null;
        }

        return this.GetQuotedName(fieldDef.Alias != null
            ? this.NamingStrategy.GetColumnAlias(fieldDef.Alias)
            : this.GetQuotedColumnName(fieldDef.Name));
    }

    /// <summary>
    /// Gets the name of the quoted column.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>System.String.</returns>
    public virtual string GetQuotedColumnName(string columnName)
    {
        return columnName == null ? null : this.GetQuotedName(this.NamingStrategy.GetColumnName(columnName));
    }

    public virtual string GetColumnName(FieldDefinition fieldDef)
    {
        if (fieldDef == null)
        {
            return null;
        }

        return fieldDef.Alias != null
            ? this.NamingStrategy.GetColumnAlias(fieldDef.Alias)
            : this.NamingStrategy.GetColumnName(fieldDef.Name);
    }

    /// <summary>
    /// Shoulds the quote.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public virtual bool ShouldQuote(string name)
    {
        return !string.IsNullOrEmpty(name) && (name.Contains(' ') || name.Contains('.'));
    }

    /// <summary>
    /// Quotes if required.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string QuoteIfRequired(string name)
    {
        return this.ShouldQuote(name) ? this.GetQuotedName(name) : name;
    }

    protected char QuoteChar = '"';

    /// <summary>
    /// Gets the name of the quoted.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string GetQuotedName(string name)
    {
        if (name == null)
        {
            return null;
        }

        if (IsQuotedName(name, this.QuoteChar))
        {
            return name;
        }

        var quoteStr = this.QuoteChar.ToString();
        return this.QuoteChar + name.Replace(quoteStr, quoteStr + quoteStr) + this.QuoteChar;
    }

    /// <summary>
    /// Whether name is a regular identifier that's safe to emit unquoted, i.e. starts with a letter or '_'
    /// and only contains letters, digits, '_' or any of the dialect-specific allowedChars (e.g. '$', '#')
    /// </summary>
    public static bool IsRegularIdentifier(string name, string allowedChars = null)
    {
        if (string.IsNullOrEmpty(name) || !(char.IsLetter(name[0]) || name[0] == '_'))
        {
            return false;
        }

        foreach (var c in name)
        {
            if (!(char.IsLetterOrDigit(c) || c == '_' || (allowedChars != null && allowedChars.IndexOf(c) >= 0)))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Whether name is already a single well-formed quoted identifier, i.e. wrapped in quoteChar with any
    /// embedded quoteChar escaped by doubling. Prevents identifier breakout via names like: "a"; DROP TABLE b; --"
    /// </summary>
    public static bool IsQuotedName(string name, char quoteChar)
    {
        if (name == null || name.Length < 2 || name[0] != quoteChar || name[name.Length - 1] != quoteChar)
        {
            return false;
        }

        for (var i = 1; i < name.Length - 1; i++)
        {
            if (name[i] != quoteChar)
            {
                continue;
            }

            if (i + 1 < name.Length - 1 && name[i + 1] == quoteChar)
            {
                i++;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Gets the name of the quoted.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    public virtual string GetQuotedName(string name, string schema)
    {
        return schema != null
                   ? $"{this.GetQuotedName(schema)}.{this.GetQuotedName(name)}"
                   : name != null
                       ? this.GetQuotedName(name)
                       : null;
    }

    public virtual string GetQuotedValue(string paramValue)
    {
        return "'" + paramValue.Replace("'", "''") + "'";
    }

    /// <summary>
    /// Sanitizes the name of the field name for parameter.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    public virtual string SanitizeFieldNameForParamName(string fieldName)
    {
        return OrmLiteConfig.SanitizeFieldNameForParamNameFn(fieldName);
    }

    /// <summary>
    /// Replaces {Property} references in SQL from attributes with their quoted column name, e.g. in
    /// [Index(Where)] and [Compute(expression)]
    /// </summary>
    public virtual string ResolveColumnRefs(ModelDefinition modelDef, string sql)
    {
        if (modelDef == null || string.IsNullOrEmpty(sql) || sql.IndexOf('{') < 0)
        {
            return sql;
        }

        return ColumnRefRegex.Replace(sql, m => modelDef.GetFieldDefinition(m.Groups[1].Value) is { } fieldDef
            ? this.GetQuotedColumnName(fieldDef)
            : m.Value);
    }
    private readonly static Regex ColumnRefRegex = new(@"\{(\w+)\}", RegexOptions.Compiled);

    /// <summary>
    /// The column type of a field: its [CustomField], the vector type of a [Vector] or the type of its converter
    /// </summary>
    protected virtual string GetFieldTypeDefinition(FieldDefinition fieldDef) =>
        this.ResolveFragment(fieldDef.CustomFieldDefinition) ?? (fieldDef.VectorDimensions != null
            ? this.GetVectorColumnDefinition(fieldDef.VectorDimensions.Value, fieldDef.VectorPrecision)
            : this.GetColumnTypeDefinition(fieldDef.ColumnType, fieldDef.FieldLength, fieldDef.Scale));

    public IOrmLiteConverter VectorConverter { get; set; } = new VectorConverter();

    /// <summary>
    /// The column type of a [Vector] with these dimensions
    /// </summary>
    public virtual string GetVectorColumnDefinition(int dimensions) =>
        throw new NotSupportedException($"{this.GetType().Name} doesn't support [Vector] columns");

    /// <summary>
    /// The column type of a [Vector] with these dimensions and precision
    /// </summary>
    public virtual string GetVectorColumnDefinition(int dimensions, VectorPrecision precision) =>
        precision == VectorPrecision.Single
            ? this.GetVectorColumnDefinition(dimensions)
            : throw new NotSupportedException($"{this.GetType().Name} doesn't support half-precision [Vector] columns");

    public virtual string ToVectorParam(string param, int dimensions) => param;

    public virtual string ToVectorParam(string param, int dimensions, VectorPrecision precision) =>
        this.ToVectorParam(param, dimensions);

    public virtual List<string> ToVectorSearchStatements(VectorSearchOptions options) => [];

    public virtual string ToVectorDistance(VectorDistance distance, string vector, string other) =>
        throw new NotSupportedException($"{this.GetType().Name} doesn't support vector distances");

    /// <summary>
    /// The statement that creates the vector index of an indexed [Vector] column, or null if the RDBMS has none
    /// </summary>
    protected virtual string ToCreateVectorIndexStatement(ModelDefinition modelDef, FieldDefinition fieldDef, string indexName) => null;

    /// <summary>
    /// What's selected for a [Vector] column if it can't be read as it is, or null to select the column
    /// </summary>
    protected virtual string GetVectorSelectExpression(string quotedColumn) => null;

    public virtual string ToSelectColumn(FieldDefinition fieldDef, string quotedColumn) =>
        fieldDef?.VectorDimensions != null ? this.GetVectorSelectExpression(quotedColumn) ?? quotedColumn : quotedColumn;

    /// <summary>
    /// A column the RDBMS generates from its [Compute("expression")], which is stored when it's [Persisted]
    /// </summary>
    protected virtual string GetGeneratedColumnDefinition(FieldDefinition fieldDef)
    {
        var columnType = this.GetFieldTypeDefinition(fieldDef);
        return $"{this.GetQuotedColumnName(fieldDef)} {columnType} GENERATED ALWAYS AS " +
               $"({this.ResolveColumnRefs(fieldDef.ModelDef, fieldDef.ComputeExpression)}) " +
               (fieldDef.IsPersisted ? "STORED" : "VIRTUAL");
    }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetColumnDefinition(FieldDefinition fieldDef)
    {
        if (fieldDef.IsRowStart || fieldDef.IsRowEnd)
        {
            return this.GetSystemTimeColumnDefinition(fieldDef);
        }

        if (fieldDef.IsGenerated)
        {
            return this.GetGeneratedColumnDefinition(fieldDef);
        }

        var fieldDefinition = this.GetFieldTypeDefinition(fieldDef);

        var sql = StringBuilderCache.Allocate();
        sql.Append($"{this.GetQuotedColumnName(fieldDef)} {fieldDefinition}");

        if (fieldDef.IsPrimaryKey)
        {
            sql.Append(" PRIMARY KEY");
            if (fieldDef.AutoIncrement)
            {
                sql.Append(' ').Append(this.AutoIncrementDefinition);
            }
        }
        else
        {
            sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
        }

        if (fieldDef.IsUniqueConstraint)
        {
            sql.Append(" UNIQUE");
        }

        var defaultValue = this.GetDefaultValue(fieldDef);

        if (!string.IsNullOrEmpty(defaultValue))
        {
            sql.AppendFormat(this.DefaultValueFormat, defaultValue);
        }

        return StringBuilderCache.ReturnAndFree(sql);
    }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetColumnDefinition(FieldDefinition fieldDef, ModelDefinition modelDef)
    {
        if (fieldDef.IsRowStart || fieldDef.IsRowEnd)
        {
            return this.GetSystemTimeColumnDefinition(fieldDef);
        }

        if (fieldDef.IsGenerated)
        {
            return this.GetGeneratedColumnDefinition(fieldDef);
        }

        var fieldDefinition = this.GetFieldTypeDefinition(fieldDef);

        var sql = StringBuilderCache.Allocate();
        sql.Append($"{this.GetQuotedColumnName(fieldDef)} {fieldDefinition}");

        // Check for Composite PrimaryKey First
        if (modelDef.CompositePrimaryKeys.Count != 0)
        {
            sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
        }
        else
        {
            if (fieldDef.IsPrimaryKey)
            {
                sql.Append(" PRIMARY KEY");
                if (fieldDef.AutoIncrement)
                {
                    sql.Append(' ').Append(this.AutoIncrementDefinition);
                }
            }
            else
            {
                sql.Append(fieldDef.IsNullable ? " NULL" : " NOT NULL");
            }
        }

        if (fieldDef.IsUniqueConstraint)
        {
            sql.Append(" UNIQUE");
        }

        var defaultValue = this.GetDefaultValue(fieldDef);
        if (!string.IsNullOrEmpty(defaultValue))
        {
            sql.AppendFormat(this.DefaultValueFormat, defaultValue);
        }

        return StringBuilderCache.ReturnAndFree(sql);
    }

    /// <summary>
    /// Gets or sets the select identity SQL.
    /// </summary>
    /// <value>The select identity SQL.</value>
    public virtual string SelectIdentitySql { get; set; }

    /// <summary>
    /// Gets the last insert identifier.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    /// <exception cref="NotImplementedException">Returning last inserted identity is not implemented on this DB Provider.</exception>
    /// <exception cref="System.NotImplementedException">Returning last inserted identity is not implemented on this DB Provider.</exception>
    public virtual long GetLastInsertId(IDbCommand dbCmd)
    {
        if (this.SelectIdentitySql == null)
        {
            throw new NotImplementedException(
                "Returning last inserted identity is not implemented on this DB Provider.");
        }

        dbCmd.CommandText = this.SelectIdentitySql;
        return dbCmd.ExecLongScalar();
    }

    /// <summary>
    /// Gets the last insert identifier SQL suffix.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException">Returning last inserted identity is not implemented on this DB Provider.</exception>
    /// <exception cref="System.NotImplementedException">Returning last inserted identity is not implemented on this DB Provider.</exception>
    public virtual string GetLastInsertIdSqlSuffix<T>()
    {
        if (this.SelectIdentitySql == null)
        {
            throw new NotImplementedException(
                "Returning last inserted identity is not implemented on this DB Provider.");
        }

        return "; " + this.SelectIdentitySql;
    }

    /// <summary>
    /// Determines whether [is full select statement] [the specified SQL].
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <returns><c>true</c> if [is full select statement] [the specified SQL]; otherwise, <c>false</c>.</returns>
    public virtual bool IsFullSelectStatement(string sql)
    {
        return !string.IsNullOrEmpty(sql)
               && (sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
                   || sql.TrimStart().StartsWith("WITH ", StringComparison.OrdinalIgnoreCase)); // common table expressions
    }

    // Fmt
    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>System.String.</returns>
    public virtual string ToSelectStatement(Type tableType, string sqlFilter, params object[] filterParams)
    {
        if (this.IsFullSelectStatement(sqlFilter))
        {
            return sqlFilter.SqlFmt(this, filterParams);
        }

        var modelDef = tableType.GetModelDefinition();
        var sql = StringBuilderCache.Allocate();
        sql.Append($"SELECT {this.GetColumnNames(modelDef)} FROM {this.GetQuotedTableName(modelDef)}");

        if (string.IsNullOrEmpty(sqlFilter))
        {
            return StringBuilderCache.ReturnAndFree(sql);
        }

        sqlFilter = sqlFilter.SqlFmt(this, filterParams);
        if (!sqlFilter.StartsWith("ORDER ", StringComparison.OrdinalIgnoreCase) &&
            !sqlFilter.StartsWith("LIMIT ", StringComparison.OrdinalIgnoreCase))
        {
            sql.Append(" WHERE ");
        }

        sql.Append(sqlFilter);

        return StringBuilderCache.ReturnAndFree(sql);
    }

    /// <summary>
    /// Applies the tags.
    /// </summary>
    /// <param name="sqlBuilder">The SQL builder.</param>
    /// <param name="tags">The tags.</param>
    protected virtual void ApplyTags(StringBuilder sqlBuilder, ISet<string> tags)
    {
        if (tags is {Count: > 0})
        {
            foreach (var tag in tags)
            {
                sqlBuilder.AppendLine(this.GenerateComment(tag));
            }

            sqlBuilder.Append('\n');
        }
    }

    /// <summary>
    /// Converts to selectstatement.
    /// </summary>
    /// <param name="queryType">Type of the query.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="selectExpression">The select expression.</param>
    /// <param name="bodyExpression">The body expression.</param>
    /// <param name="orderByExpression">The order by expression.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="rows">The rows.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>System.String.</returns>
    public virtual string ToSelectStatement(
        QueryType queryType,
        ModelDefinition modelDef,
        string selectExpression,
        string bodyExpression,
        string orderByExpression = null,
        int? offset = null,
        int? rows = null,
        ISet<string> tags = null)
    {
        var sb = StringBuilderCache.Allocate();

        this.ApplyTags(sb, tags);

        sb.Append(selectExpression);
        sb.Append(bodyExpression);

        if (!string.IsNullOrEmpty(orderByExpression))
        {
            sb.Append(orderByExpression);
        }

        if ((queryType == QueryType.Select || (rows == 1 && offset is null or 0)) && (offset != null || rows != null))
        {
            sb.Append('\n');
            sb.Append(this.SqlLimit(offset, rows));
        }

        return StringBuilderCache.ReturnAndFree(sb);
    }

    /// <summary>
    /// Generates a SQL comment.
    /// </summary>
    /// <param name="text">The comment text.</param>
    /// <returns>The generated SQL.</returns>
    public virtual string GenerateComment(in string text)
    {
        return $"-- {text}";
    }

    /// <summary>
    /// Creates the ormlite connection.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="namedConnection">The named connection.</param>
    /// <returns>ServiceStack.OrmLite.OrmLiteConnection.</returns>
    public virtual OrmLiteConnection CreateOrmLiteConnection(OrmLiteConnectionFactory factory, string namedConnection = null)
    {
        return new OrmLiteConnection(factory)
        {
            NamedConnection = namedConnection
        };
    }

    public virtual string ToReadOnlySessionStatement(bool readOnly) => null;

    /// <summary>
    /// Initializes the connection.
    /// </summary>
    /// <param name="dbConn">The database connection.</param>
    public virtual void InitConnection(IDbConnection dbConn)
    {
        if (dbConn is OrmLiteConnection ormLiteConn)
        {
            ormLiteConn.ConnectionId = Guid.NewGuid();
        }

        if (Interlocked.CompareExchange(ref this.OneTimeConnectionCommandsRun, 1, 0) == 0)
        {
            foreach (var command in this.OneTimeConnectionCommands)
            {
                using var cmd = dbConn.CreateCommand();
                cmd.ExecNonQuery(command);
            }
        }

        foreach (var command in this.ConnectionCommands)
        {
            using var cmd = dbConn.CreateCommand();
            cmd.ExecNonQuery(command);
        }

        this.OnOpenConnection?.Invoke(dbConn);
    }

    /// <summary>
    /// Gets the row version select column.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="tablePrefix">The table prefix.</param>
    /// <returns>SelectItem.</returns>
    public virtual SelectItem GetRowVersionSelectColumn(FieldDefinition field, string tablePrefix = null)
    {
        return new SelectItemColumn(this, field, tablePrefix);
    }

    /// <summary>
    /// Gets the row version column.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="tablePrefix">The table prefix.</param>
    /// <returns>System.String.</returns>
    public virtual string GetRowVersionColumn(FieldDefinition field, string tablePrefix = null)
    {
        return this.GetRowVersionSelectColumn(field, tablePrefix).ToString();
    }

    /// <summary>
    /// Gets the column names.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetColumnNames(ModelDefinition modelDef)
    {
        return this.GetColumnNames(modelDef, null).ToSelectString();
    }

    /// <summary>
    /// Gets the column names.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="tablePrefix">The table prefix.</param>
    /// <returns>SelectItem[].</returns>
    public virtual SelectItem[] GetColumnNames(ModelDefinition modelDef, string tablePrefix)
    {
        // If tablePrefix is the same as the table alias, use the quoted table name instead
        var quotedPrefix = tablePrefix != null
            ? tablePrefix == modelDef.Alias
                ? this.GetQuotedTableName(modelDef)
                : this.QuoteTable(new TableRef(modelDef.Schema, tablePrefix))
            : string.Empty;

        var sqlColumns = new SelectItem[modelDef.FieldDefinitions.Count];
        for (var i = 0; i < sqlColumns.Length; ++i)
        {
            var field = modelDef.FieldDefinitions[i];

            if (field.CustomSelect != null)
            {
                sqlColumns[i] = new SelectItemExpression(this, field.CustomSelect, field.FieldName);
            }
            else if (field.IsRowVersion)
            {
                sqlColumns[i] = this.GetRowVersionSelectColumn(field, quotedPrefix);
            }
            else if (field.VectorDimensions != null && this.GetVectorSelectExpression(
                         (quotedPrefix.Length > 0 ? quotedPrefix + "." : "") + this.GetQuotedColumnName(field)) is { } vectorSelect)
            {
                sqlColumns[i] = new SelectItemExpression(this, vectorSelect, field.FieldName);
            }
            else
            {
                sqlColumns[i] = new SelectItemColumn(this, field, quotedPrefix);
            }
        }

        return sqlColumns;
    }

    /// <summary>
    /// Shoulds the skip insert.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected virtual bool ShouldSkipInsert(FieldDefinition fieldDef)
    {
        return fieldDef.ShouldSkipInsert();
    }

    /// <summary>
    /// Columns the name only.
    /// </summary>
    /// <param name="columnExpr">The column expr.</param>
    /// <returns>System.String.</returns>
    public virtual string ColumnNameOnly(string columnExpr)
    {
        var nameOnly = columnExpr.LastRightPart('.');
        var ret = nameOnly.StripDbQuotes();
        return ret;
    }

    /// <summary>
    /// Gets the insert field definitions.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>FieldDefinition[].</returns>
    public virtual FieldDefinition[] GetInsertFieldDefinitions(
        ModelDefinition modelDef,
        ICollection<string> insertFields = null)
    {
        var insertColumns = insertFields?.Map(this.ColumnNameOnly);
        return insertColumns != null
                   ? this.NamingStrategy.GetType() == typeof(OrmLiteDefaultNamingStrategy)
                         ?
                         modelDef.GetOrderedFieldDefinitions(insertColumns)
                         : modelDef.GetOrderedFieldDefinitions(
                             insertColumns,
                             name => this.NamingStrategy.GetColumnName(name))
                   : modelDef.FieldDefinitionsArray;
    }

    /// <summary>
    /// Appends the insert row value SQL.
    /// </summary>
    /// <param name="sbColumnValues">The sb column values.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="obj">The object.</param>
    public virtual void AppendInsertRowValueSql(StringBuilder sbColumnValues, FieldDefinition fieldDef, object obj)
    {
        if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
        {
            return;
        }

        try
        {
            if (fieldDef.AutoId)
            {
                var dbValue = this.GetInsertDefaultValue(fieldDef);
                sbColumnValues.Append(dbValue != null ? this.GetQuotedValue(dbValue.ToString()) : "NULL");
            }
            else
            {
                sbColumnValues.Append(this.GetQuotedValue(fieldDef.GetValue(obj), fieldDef.FieldType));
            }
        }
        catch (Exception ex)
        {
            Log.Error("ERROR in ToInsertRowStatement(): " + ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Converts to insertrowsql.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    public virtual string ToInsertRowSql<T>(T obj, ICollection<string> insertFields = null)
    {
        var sbColumnNames = StringBuilderCache.Allocate();
        var sbColumnValues = StringBuilderCacheAlt.Allocate();
        var modelDef = obj.GetType().GetModelDefinition();

        var fieldDefs = this.GetInsertFieldDefinitions(modelDef, insertFields);

        foreach (var fieldDef in fieldDefs)
        {
            if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
            {
                continue;
            }

            if (sbColumnNames.Length > 0)
            {
                sbColumnNames.Append(',');
            }

            sbColumnNames.Append(this.GetQuotedColumnName(fieldDef));

            if (sbColumnValues.Length > 0)
            {
                sbColumnValues.Append(',');
            }

            this.AppendInsertRowValueSql(sbColumnValues, fieldDef, obj);
        }

        var sql = $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) " +
                  $"VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})";

        return sql;
    }

    /// <summary>
    /// Converts to insertrowssql.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objs">The objs.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    public virtual string ToInsertRowsSql<T>(IEnumerable<T> objs, ICollection<string> insertFields = null)
    {
        var modelDef = ModelDefinition<T>.Definition;
        var sb = StringBuilderCache.Allocate()
            .Append($"INSERT INTO {this.GetQuotedTableName(modelDef)} (");

        var fieldDefs = this.GetInsertFieldDefinitions(modelDef, insertFields: insertFields);
        var i = 0;
        foreach (var fieldDef in fieldDefs)
        {
            if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
            {
                continue;
            }

            if (i++ > 0)
            {
                sb.Append(',');
            }

            sb.Append(this.GetQuotedColumnName(fieldDef));
        }

        sb.Append(") VALUES");

        var count = 0;
        foreach (var obj in objs)
        {
            count++;
            sb.AppendLine();
            sb.Append('(');
            i = 0;
            foreach (var fieldDef in fieldDefs)
            {
                if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
                {
                    continue;
                }

                if (i++ > 0)
                {
                    sb.Append(',');
                }

                this.AppendInsertRowValueSql(sb, fieldDef, obj);
            }

            sb.Append("),");
        }

        if (count == 0)
        {
            return string.Empty;
        }

        sb.Length--;
        sb.AppendLine(";");
        var sql = StringBuilderCache.ReturnAndFree(sb);
        return sql;
    }

    /// <summary>
    /// Bulks the insert.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="db">The database.</param>
    /// <param name="objs">The objs.</param>
    /// <param name="config">The configuration.</param>
    public virtual void BulkInsert<T>(IDbConnection db, IEnumerable<T> objs, BulkInsertConfig config = null)
    {
        config ??= new BulkInsertConfig();
        foreach (var batch in objs.BatchesOf(config.BatchSize))
        {
            var sql = this.ToInsertRowsSql(batch, insertFields: config.InsertFields);
            db.ExecuteSql(sql);
        }
    }

    public async virtual Task BulkInsertAsync<T>(IDbConnection db, IEnumerable<T> objs, BulkInsertConfig config = null, CancellationToken token = default)
    {
        config ??= new BulkInsertConfig();
        foreach (var batch in objs.BatchesOf(config.BatchSize))
        {
            var sql = this.ToInsertRowsSql(batch, insertFields: config.InsertFields);
            await db.ExecuteSqlAsync(sql, token: token).ConfigAwait();
        }
    }

    // What a bulk upsert does: load the rows into a temporary staging table, then upsert them from it
    protected sealed class BulkUpsertPlan
    {
        public FieldDefinition PrimaryKey;
        /// <summary>The quoted name of the temporary table the rows are loaded into</summary>
        public string StagingTable;
        /// <summary>The fields loaded into the staging table: those that are inserted or updated</summary>
        public List<FieldDefinition> FieldDefs;
        public string CreateStagingTableSql;
        public string UpsertSql;
        public string DropStagingTableSql;
    }

    /// <summary>
    /// Returns null when rows have to be upserted one at a time: the RDBMS has no native upsert, or the connection
    /// has filters or write rules for the table, which a single statement can't apply to the rows it updates
    /// </summary>
    protected virtual BulkUpsertPlan CreateBulkUpsertPlan<T>(IDbConnection db, ICollection<string> updateOnly, BulkInsertConfig config)
    {
        var modelDef = ModelDefinition<T>.Definition;
        var primaryKey = modelDef.FieldDefinitions.FirstOrDefault(x => x.IsPrimaryKey)
            ?? throw new NotSupportedException($"'{typeof(T).Name}' does not have a primary key");
        var updateFieldDefs = OrmLiteWriteCommandExtensions.GetUpsertUpdateFieldDefinitions(modelDef, updateOnly);

        if (!this.SupportsUpsert || db.Exec(dbCmd => dbCmd.HasFilters<T>() || dbCmd.HasWriteRules<T>()))
        {
            return null;
        }

        var requestedInsertFields = GetInsertFieldDefinitions(modelDef, config.InsertFields).ToSet();
        requestedInsertFields.Add(primaryKey);
        var insertFieldDefs = modelDef.FieldDefinitions
            .Where(x => requestedInsertFields.Contains(x) && (!this.ShouldSkipInsert(x) || x.AutoId || x.IsPrimaryKey))
            .ToList();

        var fieldDefs = modelDef.FieldDefinitions
            .Where(x => insertFieldDefs.Contains(x) || updateFieldDefs.Contains(x))
            .ToList();

        var stagingTable = this.GetBulkStagingTableName("ormlite_stage_" + Guid.NewGuid().ToString("N"));
        return new BulkUpsertPlan
        {
            PrimaryKey = primaryKey,
            StagingTable = stagingTable,
            FieldDefs = fieldDefs,
            CreateStagingTableSql = this.ToCreateBulkStagingTableStatement(modelDef, stagingTable, fieldDefs),
            UpsertSql = this.ToBulkUpsertStatement(modelDef, stagingTable, insertFieldDefs, updateFieldDefs),
            DropStagingTableSql = this.ToDropBulkStagingTableStatement(stagingTable),
        };
    }

    // Rows with an [AutoIncrement] primary key that hasn't been assigned are new, and are inserted for the RDBMS
    // to assign it. The others are loaded into the staging table.
    private static IEnumerable<T> GetRowsToStage<T>(IEnumerable<T> objs, FieldDefinition primaryKey, List<T> newRows)
    {
        if (!primaryKey.AutoIncrement)
        {
            return objs;
        }

        var defaultId = primaryKey.FieldType.GetDefaultValue();
        IEnumerable<T> RowsWithIds()
        {
            foreach (var obj in objs)
            {
                var id = primaryKey.GetValue(obj);
                if (id == null || Equals(id, defaultId))
                {
                    newRows.Add(obj);
                }
                else
                {
                    yield return obj;
                }
            }
        }
        return RowsWithIds();
    }

    public virtual void BulkUpsert<T>(IDbConnection db, IEnumerable<T> objs, ICollection<string> updateOnly = null, BulkInsertConfig config = null)
    {
        config ??= new();
        var plan = this.CreateBulkUpsertPlan<T>(db, updateOnly, config);
        if (plan == null)
        {
            if (updateOnly == null)
            {
                db.UpsertAll(objs);
            }
            else
            {
                db.UpsertAll(objs, updateOnly.ToArray());
            }

            return;
        }

        var newRows = new List<T>();
        db.ExecuteSql(plan.CreateStagingTableSql);
        try
        {
            this.BulkLoad(db, plan.StagingTable, plan.FieldDefs, GetRowsToStage(objs, plan.PrimaryKey, newRows), config);

            db.Exec(dbCmd =>
            {
                // Rows keep the primary keys they were given
                if (plan.PrimaryKey.AutoIncrement)
                {
                    this.EnableIdentityInsert<T>(dbCmd);
                }

                try
                {
                    return dbCmd.ExecuteSql(plan.UpsertSql);
                }
                finally
                {
                    if (plan.PrimaryKey.AutoIncrement)
                    {
                        this.DisableIdentityInsert<T>(dbCmd);
                    }
                }
            });
        }
        finally
        {
            db.ExecuteSql(plan.DropStagingTableSql);
        }

        if (newRows.Count > 0)
        {
            db.BulkInsert(newRows, config);
        }
    }

    public async virtual Task BulkUpsertAsync<T>(IDbConnection db, IEnumerable<T> objs, ICollection<string> updateOnly = null, BulkInsertConfig config = null, CancellationToken token = default)
    {
        config ??= new();
        var plan = this.CreateBulkUpsertPlan<T>(db, updateOnly, config);
        if (plan == null)
        {
            if (updateOnly == null)
            {
                await db.UpsertAllAsync(objs, token).ConfigAwait();
            }
            else
            {
                await db.UpsertAllAsync(objs, updateOnly.ToArray(), token).ConfigAwait();
            }

            return;
        }

        var newRows = new List<T>();
        await db.ExecuteSqlAsync(plan.CreateStagingTableSql, token: token).ConfigAwait();
        try
        {
            await this.BulkLoadAsync(db, plan.StagingTable, plan.FieldDefs, GetRowsToStage(objs, plan.PrimaryKey, newRows), config, token).ConfigAwait();

            await db.Exec(async dbCmd =>
            {
                if (plan.PrimaryKey.AutoIncrement)
                {
                    await this.EnableIdentityInsertAsync<T>(dbCmd, token).ConfigAwait();
                }

                try
                {
                    return await dbCmd.ExecuteSqlAsync(plan.UpsertSql, token).ConfigAwait();
                }
                finally
                {
                    if (plan.PrimaryKey.AutoIncrement)
                    {
                        await this.DisableIdentityInsertAsync<T>(dbCmd, token).ConfigAwait();
                    }
                }
            }).ConfigAwait();
        }
        finally
        {
            await db.ExecuteSqlAsync(plan.DropStagingTableSql, token: token).ConfigAwait();
        }

        if (newRows.Count > 0)
        {
            await db.BulkInsertAsync(newRows, config, token).ConfigAwait();
        }
    }

    /// <summary>
    /// The quoted name of a temporary table of this connection
    /// </summary>
    protected virtual string GetBulkStagingTableName(string name) => this.GetQuotedName(name);

    /// <summary>
    /// Creates an empty temporary table with the same columns as the fields of a table
    /// </summary>
    protected virtual string ToCreateBulkStagingTableStatement(ModelDefinition modelDef, string stagingTable, List<FieldDefinition> fieldDefs) =>
        $"CREATE TEMPORARY TABLE {stagingTable} AS " +
        $"SELECT {fieldDefs.Map(GetQuotedColumnName).Join(",")} FROM {this.GetQuotedTableName(modelDef)} WHERE 1=0";

    protected virtual string ToDropBulkStagingTableStatement(string stagingTable) => "DROP TABLE " + stagingTable;

    /// <summary>
    /// The statement that inserts the rows of the staging table with a new primary key, and updates the
    /// updateFieldDefs of those with an existing one
    /// </summary>
    protected virtual string ToBulkUpsertStatement(ModelDefinition modelDef, string stagingTable,
        List<FieldDefinition> insertFieldDefs, List<FieldDefinition> updateFieldDefs) =>
        throw new NotSupportedException($"{this.GetType().Name} does not support bulk upserts");

    // INSERT INTO Table (columns) SELECT columns FROM staging
    protected string GetBulkUpsertInsertSql(ModelDefinition modelDef, string stagingTable, List<FieldDefinition> insertFieldDefs) =>
        $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({insertFieldDefs.Map(GetQuotedColumnName).Join(",")}) " +
        $"SELECT {insertFieldDefs.Map(x => GetBulkUpsertValue(x.CustomInsert, GetQuotedColumnName(x))).Join(",")} FROM {stagingTable}";

    // The value a column is inserted or updated with: the staged column, in the field's [CustomInsert] or [CustomUpdate]
    protected static string GetBulkUpsertValue(string customFormat, string stagedColumn) =>
        customFormat != null ? string.Format(customFormat, stagedColumn) : stagedColumn;

    /// <summary>
    /// The value of a row that's loaded into a table, where an [AutoId] that hasn't been assigned gets a new one
    /// </summary>
    protected object GetBulkLoadValue(FieldDefinition fieldDef, object obj)
    {
        var value = fieldDef.GetValue(obj);
        return fieldDef.AutoId && (value == null || Equals(value, fieldDef.FieldType.GetDefaultValue()))
            ? this.GetInsertDefaultValue(fieldDef)
            : value;
    }

    // INSERT INTO table (columns) VALUES (row),(row),...
    private string ToBulkLoadSql<T>(string quotedTable, List<FieldDefinition> fieldDefs, IEnumerable<T> rows)
    {
        var sb = StringBuilderCache.Allocate()
            .Append("INSERT INTO ").Append(quotedTable).Append(" (")
            .Append(fieldDefs.Map(GetQuotedColumnName).Join(",")).Append(") VALUES ");

        var count = 0;
        foreach (var row in rows)
        {
            sb.Append(count++ > 0 ? ",\n(" : "\n(");
            for (var i = 0; i < fieldDefs.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                sb.Append(this.GetQuotedValue(this.GetBulkLoadValue(fieldDefs[i], row), fieldDefs[i].FieldType));
            }
            sb.Append(')');
        }
        var sql = StringBuilderCache.ReturnAndFree(sb);
        return count > 0 ? sql : null;
    }

    /// <summary>
    /// Loads rows into a table with the fields of the model, using the fastest way the RDBMS has
    /// </summary>
    protected virtual void BulkLoad<T>(IDbConnection db, string quotedTable, List<FieldDefinition> fieldDefs, IEnumerable<T> rows, BulkInsertConfig config)
    {
        foreach (var batch in rows.BatchesOf(config.BatchSize))
        {
            var sql = ToBulkLoadSql(quotedTable, fieldDefs, batch);
            if (sql != null)
            {
                db.ExecuteSql(sql);
            }
        }
    }

    protected async virtual Task BulkLoadAsync<T>(IDbConnection db, string quotedTable, List<FieldDefinition> fieldDefs, IEnumerable<T> rows, BulkInsertConfig config, CancellationToken token)
    {
        foreach (var batch in rows.BatchesOf(config.BatchSize))
        {
            var sql = ToBulkLoadSql(quotedTable, fieldDefs, batch);
            if (sql != null)
            {
                await db.ExecuteSqlAsync(sql, token: token).ConfigAwait();
            }
        }
    }
    /// <summary>
    /// Converts to insertrowstatement.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    public virtual string ToInsertRowStatement(
        IDbCommand cmd,
        object objWithProperties,
        ICollection<string> insertFields = null)
    {
        var sbColumnNames = StringBuilderCache.Allocate();
        var sbColumnValues = StringBuilderCacheAlt.Allocate();
        var modelDef = objWithProperties.GetType().GetModelDefinition();

        var fieldDefs = this.GetInsertFieldDefinitions(modelDef, insertFields);
        foreach (var fieldDef in fieldDefs)
        {
            if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
            {
                continue;
            }

            if (sbColumnNames.Length > 0)
            {
                sbColumnNames.Append(',');
            }

            if (sbColumnValues.Length > 0)
            {
                sbColumnValues.Append(',');
            }

            try
            {
                sbColumnNames.Append(this.GetQuotedColumnName(fieldDef));
                sbColumnValues.Append(this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName)));

                this.AddParameter(cmd, fieldDef);
            }
            catch (Exception ex)
            {
                Log.Error("ERROR in ToInsertRowStatement(): " + ex.Message, ex);
                throw;
            }
        }

        var sql =
            $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) " +
            $"VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})";

        return sql;
    }

    /// <summary>
    /// Converts to insertstatement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="item">The item.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <returns>System.String.</returns>
    public virtual string ToInsertStatement<T>(IDbCommand dbCmd, T item, ICollection<string> insertFields = null)
    {
        dbCmd.Parameters.Clear();
        var dialectProvider = dbCmd.GetDialectProvider();
        dialectProvider.PrepareParameterizedInsertStatement<T>(dbCmd, insertFields);

        if (string.IsNullOrEmpty(dbCmd.CommandText))
        {
            return null;
        }

        dialectProvider.SetParameterValues<T>(dbCmd, item);

        return this.MergeParamsIntoSql(dbCmd.CommandText, this.ToArray(dbCmd.Parameters));
    }

    /// <summary>
    /// Gets the insert default value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.Object.</returns>
    protected virtual object GetInsertDefaultValue(FieldDefinition fieldDef)
    {
        if (!fieldDef.AutoId)
        {
            return null;
        }

        if (fieldDef.FieldType == typeof(Guid))
        {
            return Guid.NewGuid();
        }

        return null;
    }

    /// <summary>
    /// Prepares the parameterized insert statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="insertFields">The insert fields.</param>
    /// <param name="shouldInclude">The should include.</param>
    public virtual void PrepareParameterizedInsertStatement<T>(
        IDbCommand cmd,
        ICollection<string> insertFields = null,
        Func<FieldDefinition, bool> shouldInclude = null)
    {
        var sbColumnNames = StringBuilderCache.Allocate();
        var sbColumnValues = StringBuilderCacheAlt.Allocate();
        var modelDef = typeof(T).GetModelDefinition();

        cmd.Parameters.Clear();

        var fieldDefs = this.GetInsertFieldDefinitions(modelDef, insertFields);
        foreach (var fieldDef in fieldDefs)
        {
            if (fieldDef.ShouldSkipInsert() && shouldInclude?.Invoke(fieldDef) != true)
            {
                continue;
            }

            if (sbColumnNames.Length > 0)
            {
                sbColumnNames.Append(',');
            }

            if (sbColumnValues.Length > 0)
            {
                sbColumnValues.Append(',');
            }

            try
            {
                sbColumnNames.Append(this.GetQuotedColumnName(fieldDef));
                sbColumnValues.Append(
                    this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName), fieldDef.CustomInsert));

                var p = this.AddParameter(cmd, fieldDef);

                if (fieldDef.AutoId)
                {
                    p.Value = this.GetInsertDefaultValue(fieldDef);
                }
            }
            catch (Exception ex)
            {
                Log.Error("ERROR in PrepareParameterizedInsertStatement(): " + ex.Message, ex);
                throw;
            }
        }

        cmd.CommandText =
            $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) " +
            $"VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})";
    }

    public virtual bool SupportsUpsert => false;

    public virtual string ToUpsertReturningStatement(string sql, ModelDefinition modelDef, ICollection<FieldDefinition> returnFields = null) => null;

    public virtual void PrepareParameterizedUpsertStatement<T>(IDbCommand cmd,
        ICollection<string> insertFields = null, ICollection<string> updateOnly = null) =>
        throw new NotSupportedException($"{GetType().Name} does not support native UPSERT statements");

    protected void PrepareUpsertFields<T>(IDbCommand cmd,
        ICollection<string> insertFields,
        ICollection<string> updateOnly,
        out ModelDefinition modelDef,
        out List<FieldDefinition> insertFieldDefs,
        out List<FieldDefinition> updateFieldDefs)
    {
        modelDef = typeof(T).GetModelDefinition();
        var primaryKey = modelDef.FieldDefinitions.FirstOrDefault(x => x.IsPrimaryKey)
                         ?? throw new NotSupportedException($"'{typeof(T).Name}' does not have a primary key");

        var requestedInsertFields = GetInsertFieldDefinitions(modelDef, insertFields).ToSet();
        requestedInsertFields.Add(primaryKey);

        insertFieldDefs = modelDef.FieldDefinitions
            .Where(x => requestedInsertFields.Contains(x)
                        && (!ShouldSkipInsert(x) || x.AutoId || x.IsPrimaryKey))
            .ToList();

        var updateAllFields = updateOnly == null;
        var requestedUpdateFields = updateAllFields
            ? null
            : GetInsertFieldDefinitions(modelDef, updateOnly).ToSet();

        updateFieldDefs = modelDef.FieldDefinitions
            .Where(x => !x.IsPrimaryKey
                        && !x.IsRowVersion
                        && !x.ShouldSkipUpdate()
                        && (updateAllFields || requestedUpdateFields.Contains(x)))
            .ToList();

        cmd.Parameters.Clear();
        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (!insertFieldDefs.Contains(fieldDef) && !updateFieldDefs.Contains(fieldDef))
            {
                continue;
            }

            var p = AddParameter(cmd, fieldDef);
            if (fieldDef.AutoId)
            {
                p.Value = this.GetInsertDefaultValue(fieldDef);
            }
        }
    }

    protected string GetUpsertInsertSql(ModelDefinition modelDef, IEnumerable<FieldDefinition> insertFieldDefs)
    {
        var fields = insertFieldDefs.ToList();
        var columnNames = fields.Map(GetQuotedColumnName).Join(",");
        var columnValues = fields.Map(x =>
            this.GetParam(SanitizeFieldNameForParamName(x.FieldName), x.CustomInsert)).Join(",");
        return $"INSERT INTO {GetQuotedTableName(modelDef)} ({columnNames}) VALUES ({columnValues})";
    }

    protected string GetUpsertUpdateSql(IEnumerable<FieldDefinition> updateFieldDefs, string targetPrefix = null)
    {
        return updateFieldDefs.Map(x =>
            (targetPrefix ?? "") + GetQuotedColumnName(x) + "=" +
            this.GetParam(SanitizeFieldNameForParamName(x.FieldName), x.CustomUpdate)).Join(", ");
    }

    /// <summary>
    /// Prepares the insert row statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="args">The arguments.</param>
    public virtual void PrepareInsertRowStatement<T>(IDbCommand dbCmd, Dictionary<string, object> args)
    {
        var sbColumnNames = StringBuilderCache.Allocate();
        var sbColumnValues = StringBuilderCacheAlt.Allocate();
        var modelDef = typeof(T).GetModelDefinition();

        dbCmd.Parameters.Clear();

        foreach (var entry in args)
        {
            var fieldDef = modelDef.AssertFieldDefinition(entry.Key);
            if (fieldDef.ShouldSkipInsert())
            {
                continue;
            }

            var value = entry.Value;

            if (sbColumnNames.Length > 0)
            {
                sbColumnNames.Append(',');
            }

            if (sbColumnValues.Length > 0)
            {
                sbColumnValues.Append(',');
            }

            try
            {
                sbColumnNames.Append(this.GetQuotedColumnName(fieldDef));
                sbColumnValues.Append(this.GetInsertParam(dbCmd, value, fieldDef));
            }
            catch (Exception ex)
            {
                Log.Error("ERROR in PrepareInsertRowStatement(): " + ex.Message, ex);
                throw;
            }
        }

        dbCmd.CommandText =
            $"INSERT INTO {this.GetQuotedTableName(modelDef)} ({StringBuilderCache.ReturnAndFree(sbColumnNames)}) " +
            $"VALUES ({StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)})";
    }

    /// <summary>
    /// Gets the insert columns statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.String.</returns>
    public virtual string GetInsertColumnsStatement<T>()
    {
        var sbColumnNames = StringBuilderCache.Allocate();
        var modelDef = typeof(T).GetModelDefinition();

        var fieldDefs = modelDef.FieldDefinitions.OrderBy(fd => fd.Order);

        foreach (var fieldDef in fieldDefs)
        {
            if (this.ShouldSkipInsert(fieldDef) && !fieldDef.AutoId)
            {
                continue;
            }

            if (sbColumnNames.Length > 0)
            {
                sbColumnNames.Append(',');
            }

            try
            {
                sbColumnNames.Append(this.GetQuotedColumnName(fieldDef));
            }
            catch (Exception ex)
            {
                Log.Error("ERROR in ToInsertRowStatement(): " + ex.Message, ex);
                throw;
            }
        }

        return StringBuilderCache.ReturnAndFree(sbColumnNames);
    }

    /// <summary>
    /// Converts to updatestatement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="item">The item.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <returns>System.String.</returns>
    public virtual string ToUpdateStatement<T>(IDbCommand dbCmd, T item, ICollection<string> updateFields = null)
    {
        dbCmd.Parameters.Clear();
        var dialectProvider = dbCmd.GetDialectProvider();
        dialectProvider.PrepareParameterizedUpdateStatement<T>(dbCmd, updateFields);

        if (string.IsNullOrEmpty(dbCmd.CommandText))
        {
            return null;
        }

        dialectProvider.SetParameterValues<T>(dbCmd, item);

        return this.MergeParamsIntoSql(dbCmd.CommandText, this.ToArray(dbCmd.Parameters));
    }

    /// <summary>
    /// Converts to array.
    /// </summary>
    /// <param name="dbParams">The database parameters.</param>
    /// <returns>IDbDataParameter[].</returns>
    IDbDataParameter[] ToArray(IDataParameterCollection dbParams)
    {
        var to = new IDbDataParameter[dbParams.Count];
        for (var i = 0; i < dbParams.Count; i++)
        {
            to[i] = (IDbDataParameter)dbParams[i];
        }

        return to;
    }

    /// <summary>
    /// Merges the parameters into SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="dbParams">The database parameters.</param>
    /// <returns>System.String.</returns>
    public virtual string MergeParamsIntoSql(string sql, IEnumerable<IDbDataParameter> dbParams)
    {
        foreach (var dbParam in dbParams)
        {
            var quotedValue = dbParam.Value != null
                                  ? this.GetQuotedValue(dbParam.Value, dbParam.Value.GetType())
                                  : "null";

            var pattern = dbParam.ParameterName + @"(,|\s|\)|$)";
            var replacement = quotedValue.Replace("$", "$$") + "$1";
            sql = Regex.Replace(sql, pattern, replacement,
                RegexOptions.None,
                TimeSpan.FromMilliseconds(100));
        }

        return sql;
    }

    // Load Self Table.RefTableId PK
    /// <summary>
    /// Gets the reference self SQL.
    /// </summary>
    /// <typeparam name="From">The type of from.</typeparam>
    /// <param name="refQ">The reference q.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="refSelf">The reference self.</param>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetRefSelfSql<From>(SqlExpression<From> refQ, ModelDefinition modelDef, FieldDefinition refSelf, ModelDefinition refModelDef, FieldDefinition refId)
    {
        refQ.Select(this.GetQuotedColumnName(modelDef, refSelf));
        refQ.OrderBy().ClearLimits(); // clear any ORDER BY or LIMIT's in Sub Select's

        var subSqlRef = refQ.ToMergedParamsSelectStatement();

        var sqlRef = $"SELECT {this.GetColumnNames(refModelDef)} " +
                     $"FROM {this.GetQuotedTableName(refModelDef)} " +
                     $"WHERE {this.GetQuotedColumnName(refId)} " +
                     $"IN ({subSqlRef})";

        if (OrmLiteConfig.LoadReferenceSelectFilter != null)
        {
            sqlRef = OrmLiteConfig.LoadReferenceSelectFilter(refModelDef.ModelType, sqlRef);
        }

        return sqlRef;
    }

    /// <summary>
    /// Gets the reference field SQL.
    /// </summary>
    /// <param name="subSql">The sub SQL.</param>
    /// <param name="refModelDef">The reference model definition.</param>
    /// <param name="refField">The reference field.</param>
    /// <returns>System.String.</returns>
    public virtual string GetRefFieldSql(string subSql, ModelDefinition refModelDef, FieldDefinition refField)
    {
        var sqlRef = $"SELECT {this.GetColumnNames(refModelDef)} " +
                     $"FROM {this.GetQuotedTableName(refModelDef)} " +
                     $"WHERE {this.GetQuotedColumnName(refField)} " +
                     $"IN ({subSql})";

        if (OrmLiteConfig.LoadReferenceSelectFilter != null)
        {
            sqlRef = OrmLiteConfig.LoadReferenceSelectFilter(refModelDef.ModelType, sqlRef);
        }

        return sqlRef;
    }

    /// <summary>
    /// Gets the field reference SQL.
    /// </summary>
    /// <param name="subSql">The sub SQL.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="fieldRef">The field reference.</param>
    /// <returns>System.String.</returns>
    public virtual string GetFieldReferenceSql(string subSql, FieldDefinition fieldDef, FieldReference fieldRef)
    {
        var refModelDef = fieldRef.RefModelDef;

        var useSubSql = $"SELECT {this.GetQuotedColumnName(fieldRef.RefIdFieldDef)} FROM "
            + subSql.RightPart("FROM");

        var pk = this.GetQuotedColumnName(refModelDef.PrimaryKey);
        var sqlRef = $"SELECT {pk}, {this.GetQuotedColumnName(fieldRef.RefFieldDef)} " +
                     $"FROM {this.GetQuotedTableName(refModelDef)} " +
                     $"WHERE {pk} " +
                     $"IN ({useSubSql})";

        if (OrmLiteConfig.LoadReferenceSelectFilter != null)
        {
            sqlRef = OrmLiteConfig.LoadReferenceSelectFilter(refModelDef.ModelType, sqlRef);
        }

        return sqlRef;
    }

    /// <summary>
    /// Prepares the parameterized update statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <returns>If had RowVersion</returns>
    public virtual bool PrepareParameterizedUpdateStatement<T>(
        IDbCommand cmd,
        ICollection<string> updateFields = null)
    {
        var sql = StringBuilderCache.Allocate();
        var sqlFilter = StringBuilderCacheAlt.Allocate();
        var modelDef = typeof(T).GetModelDefinition();
        var hadRowVersion = false;
        var updateAllFields = updateFields == null || updateFields.Count == 0;

        cmd.Parameters.Clear();

        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (fieldDef.ShouldSkipUpdate())
            {
                continue;
            }

            try
            {
                if ((fieldDef.IsPrimaryKey || fieldDef.IsRowVersion) && updateAllFields)
                {
                    if (sqlFilter.Length > 0)
                    {
                        sqlFilter.Append(" AND ");
                    }

                    this.AppendFieldCondition(sqlFilter, fieldDef, cmd);

                    if (fieldDef.IsRowVersion)
                    {
                        hadRowVersion = true;
                    }

                    continue;
                }

                if (!updateAllFields && !updateFields.Contains(fieldDef.Name, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (sql.Length > 0)
                {
                    sql.Append(", ");
                }

                sql.Append(this.GetQuotedColumnName(fieldDef)).Append('=').Append(
                    this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName), fieldDef.CustomUpdate));

                this.AddParameter(cmd, fieldDef);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ERROR in PrepareParameterizedUpdateStatement(): {ex.Message}");
                if (OrmLiteConfig.ThrowOnError)
                {
                    throw;
                }
            }
        }

        if (sql.Length > 0)
        {
            var strFilter = StringBuilderCacheAlt.ReturnAndFree(sqlFilter);
            cmd.CommandText = $"UPDATE {this.GetQuotedTableName(modelDef)} " +
                              $"SET {StringBuilderCache.ReturnAndFree(sql)} {(strFilter.Length > 0 ? "WHERE " + strFilter : string.Empty)}";
        }
        else
        {
            cmd.CommandText = string.Empty;
        }

        return hadRowVersion;
    }

    /// <summary>
    /// Appends the null field condition.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="fieldDef">The field definition.</param>
    public virtual void AppendNullFieldCondition(StringBuilder sqlFilter, FieldDefinition fieldDef)
    {
        sqlFilter.Append(this.GetQuotedColumnName(fieldDef)).Append(" IS NULL");
    }

    /// <summary>
    /// Appends the field condition.
    /// </summary>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="cmd">The command.</param>
    public virtual void AppendFieldCondition(StringBuilder sqlFilter, FieldDefinition fieldDef, IDbCommand cmd)
    {
        sqlFilter.Append(this.GetQuotedColumnName(fieldDef)).Append('=').Append(
            this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName)));

        this.AddParameter(cmd, fieldDef);
    }

    /// <summary>
    /// Prepares the parameterized delete statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="deleteFieldValues">The delete field values.</param>
    /// <returns>If had RowVersion</returns>
    /// <exception cref="ArgumentException">DELETE's must have at least 1 criteria</exception>
    /// <exception cref="System.ArgumentException">DELETE's must have at least 1 criteria</exception>
    public virtual bool PrepareParameterizedDeleteStatement<T>(
        IDbCommand cmd,
        IDictionary<string, object> deleteFieldValues)
    {
        if (deleteFieldValues == null || deleteFieldValues.Count == 0)
        {
            throw new ArgumentException("DELETE's must have at least 1 criteria");
        }

        var sqlFilter = StringBuilderCache.Allocate();
        var modelDef = typeof(T).GetModelDefinition();
        var hadRowVersion = false;

        cmd.Parameters.Clear();

        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (fieldDef.ShouldSkipDelete())
            {
                continue;
            }

            if (!deleteFieldValues.TryGetValue(fieldDef.Name, out var fieldValue))
            {
                continue;
            }

            if (fieldDef.IsRowVersion)
            {
                hadRowVersion = true;
            }

            try
            {
                if (sqlFilter.Length > 0)
                {
                    sqlFilter.Append(" AND ");
                }

                if (fieldValue != null)
                {
                    this.AppendFieldCondition(sqlFilter, fieldDef, cmd);
                }
                else
                {
                    this.AppendNullFieldCondition(sqlFilter, fieldDef);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ERROR in PrepareParameterizedDeleteStatement(): {ex.Message}");

                if (OrmLiteConfig.ThrowOnError)
                {
                    throw;
                }
            }
        }

        cmd.CommandText =
            $"DELETE FROM {this.GetQuotedTableName(modelDef)} WHERE {StringBuilderCache.ReturnAndFree(sqlFilter)}";

        return hadRowVersion;
    }

    /// <summary>
    /// Prepares the stored procedure statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="obj">The object.</param>
    public virtual void PrepareStoredProcedureStatement<T>(IDbCommand cmd, T obj)
    {
        cmd.CommandText = this.ToExecuteProcedureStatement(obj);
        cmd.CommandType = CommandType.StoredProcedure;
    }

    /// <summary>
    /// Used for adding updated DB params in INSERT and UPDATE statements
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>IDbDataParameter.</returns>
    protected IDbDataParameter AddParameter(IDbCommand cmd, FieldDefinition fieldDef)
    {
        var p = cmd.CreateParameter();
        this.SetParameter(fieldDef, p);
        this.InitUpdateParam(p);
        cmd.Parameters.Add(p);
        return p;
    }

    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="p">The p.</param>
    public virtual void SetParameter(FieldDefinition fieldDef, IDbDataParameter p)
    {
        p.ParameterName = this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName));
        if (fieldDef.VectorDimensions != null)
        {
            this.VectorConverter.InitDbParam(p, fieldDef.ColumnType);
        }
        else
        {
            this.InitDbParam(p, fieldDef.ColumnType);
        }
    }

    /// <summary>
    /// Enables the identity insert.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    public virtual void EnableIdentityInsert<T>(IDbCommand cmd)
    {
    }

    /// <summary>
    /// Enables the identity insert asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public virtual Task EnableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default)
    {
        return TypeConstants.EmptyTask;
    }

    /// <summary>
    /// Disables the identity insert.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    public virtual void DisableIdentityInsert<T>(IDbCommand cmd)
    {
    }

    /// <summary>
    /// Disables the identity insert asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public virtual Task DisableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default)
    {
        return TypeConstants.EmptyTask;
    }

    /// <summary>
    /// Enables the foreign keys check.
    /// </summary>
    /// <param name="cmd">The command.</param>
    public virtual void EnableForeignKeysCheck(IDbCommand cmd)
    {
    }

    /// <summary>
    /// Enables the foreign keys check asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public virtual Task EnableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return TypeConstants.EmptyTask;
    }

    /// <summary>
    /// Disables the foreign keys check.
    /// </summary>
    /// <param name="cmd">The command.</param>
    public virtual void DisableForeignKeysCheck(IDbCommand cmd)
    {
    }

    /// <summary>
    /// Disables the foreign keys check asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public virtual Task DisableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return TypeConstants.EmptyTask;
    }

    /// <summary>
    /// Sets the parameter values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="obj">The object.</param>
    /// <exception cref="ArgumentException">$"Field Definition '{fieldName}' was not found</exception>
    /// <exception cref="System.ArgumentException">Field Definition '{fieldName}' was not found</exception>
    public virtual void SetParameterValues<T>(IDbCommand dbCmd, object obj)
    {
        var modelDef = GetModel(typeof(T));
        var fieldMap = this.GetFieldDefinitionMap(modelDef);

        foreach (IDataParameter p in dbCmd.Parameters)
        {
            if (OrmLiteConnectionFiltersApi.IsFilterParam(p.ParameterName))
            {
                continue; // params of the connection's filter conditions
            }

            var fieldName = this.ToFieldName(p.ParameterName);
            fieldMap.TryGetValue(fieldName, out var fieldDef);

            if (fieldDef == null)
            {
                if (this.ParamNameFilter != null)
                {
                    fieldDef = modelDef.GetFieldDefinition(
                        name => string.Equals(
                            this.ParamNameFilter(name),
                            fieldName,
                            StringComparison.OrdinalIgnoreCase));
                }

                if (fieldDef == null)
                {
                    throw new ArgumentException("Field Definition was not found", fieldName);
                }
            }

            if (fieldDef.AutoId && p.Value != null)
            {
                var existingId = fieldDef.GetValue(obj);
                if (existingId is Guid existingGuid && existingGuid != Guid.Empty)
                {
                    p.Value = existingGuid; // Use existing value if not default
                }

                fieldDef.SetValue(obj, p.Value); // Auto populate default values
                continue;
            }

            this.SetParameterValue(fieldDef, p, obj);
        }
    }

    /// <summary>
    /// Gets the field definition map.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>Dictionary&lt;System.String, FieldDefinition&gt;.</returns>
    public Dictionary<string, FieldDefinition> GetFieldDefinitionMap(ModelDefinition modelDef)
    {
        return modelDef.GetFieldDefinitionMap(this.SanitizeFieldNameForParamName);
    }

    /// <summary>
    /// Sets the parameter value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="p">The p.</param>
    /// <param name="obj">The object.</param>
    public virtual void SetParameterValue(FieldDefinition fieldDef, IDataParameter p, object obj)
    {
        var value = this.GetValueOrDbNull(fieldDef, obj);
        p.Value = value;

        this.SetParameterSize(fieldDef, p);
    }

    /// <summary>
    /// Sets the size of the parameter.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="p">The p.</param>
    protected virtual void SetParameterSize(FieldDefinition fieldDef, IDataParameter p)
    {
        if (p.Value is string s && p is IDbDataParameter dataParam && dataParam.Size > 0 && s.Length > dataParam.Size)
        {
            // db param Size set in StringConverter
            dataParam.Size = s.Length;
        }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="obj">The object.</param>
    /// <returns>System.Object.</returns>
    protected virtual object GetValue(FieldDefinition fieldDef, object obj)
    {
        return this.GetFieldValue(fieldDef, fieldDef.GetValue(obj));
    }

    /// <summary>
    /// Gets the field value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public object GetFieldValue(FieldDefinition fieldDef, object value)
    {
        if (value == null)
        {
            return null;
        }

        var converter = this.GetConverterBestMatch(fieldDef);
        try
        {
            return converter.ToDbValue(fieldDef.FieldType, value);
        }
        catch (Exception ex)
        {
            Log.Error(
                $"Error in {converter.GetType().Name}.ToDbValue() for field '{fieldDef.Name}' of Type '{fieldDef.FieldType}' with value '{value.GetType().Name}'",
                ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the field value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public object GetFieldValue(Type fieldType, object value)
    {
        if (value == null)
        {
            return null;
        }

        var converter = this.GetConverterBestMatch(fieldType);
        try
        {
            return converter.ToDbValue(fieldType, value);
        }
        catch (Exception ex)
        {
            Log.Error(
                $"Error in {converter.GetType().Name}.ToDbValue() for field of Type '{fieldType}' with value '{value.GetType().Name}'",
                ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the value or database null.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="obj">The object.</param>
    /// <returns>System.Object.</returns>
    protected virtual object GetValueOrDbNull(FieldDefinition fieldDef, object obj)
    {
        var value = this.GetValue(fieldDef, obj);
        if (value == null)
        {
            return DBNull.Value;
        }

        return value;
    }

    /// <summary>
    /// Gets the quoted value or database null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="obj">The object.</param>
    /// <returns>System.Object.</returns>
    protected virtual object GetQuotedValueOrDbNull<T>(FieldDefinition fieldDef, object obj)
    {
        var value = fieldDef.GetValue(obj);

        if (value == null)
        {
            return DBNull.Value;
        }

        var unquotedVal = this.GetQuotedValue(value, fieldDef.FieldType).TrimStart('\'').TrimEnd('\'');

        if (string.IsNullOrEmpty(unquotedVal))
        {
            return DBNull.Value;
        }

        return unquotedVal;
    }

    /// <summary>
    /// Prepares the update row statement.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <exception cref="Exception">No valid update properties provided (e.g. p => p.FirstName): " + dbCmd.CommandText</exception>
    /// <exception cref="System.Exception">No valid update properties provided (e.g. p =&gt; p.FirstName): " + dbCmd.CommandText</exception>
    public virtual void PrepareUpdateRowStatement(
        IDbCommand dbCmd,
        object objWithProperties,
        ICollection<string> updateFields = null)
    {
        var sql = StringBuilderCache.Allocate();
        var sqlFilter = StringBuilderCacheAlt.Allocate();
        var modelDef = objWithProperties.GetType().GetModelDefinition();
        var updateAllFields = updateFields == null || updateFields.Count == 0;

        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (fieldDef.ShouldSkipUpdate())
            {
                continue;
            }

            try
            {
                if (fieldDef.IsPrimaryKey && updateAllFields)
                {
                    if (sqlFilter.Length > 0)
                    {
                        sqlFilter.Append(" AND ");
                    }

                    sqlFilter.Append(this.GetQuotedColumnName(fieldDef)).Append('=').Append(
                        this.AddQueryParam(dbCmd, fieldDef.GetValue(objWithProperties), fieldDef).ParameterName);

                    continue;
                }

                if (!updateAllFields && !updateFields.Contains(fieldDef.Name, StringComparer.OrdinalIgnoreCase) ||
                    fieldDef.AutoIncrement)
                {
                    continue;
                }

                if (sql.Length > 0)
                {
                    sql.Append(", ");
                }

                sql.Append(this.GetQuotedColumnName(fieldDef)).Append('=').Append(
                    this.GetUpdateParam(dbCmd, fieldDef.GetValue(objWithProperties), fieldDef));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ERROR in ToUpdateRowStatement(): {ex.Message}");

                if (OrmLiteConfig.ThrowOnError)
                {
                    throw;
                }
            }
        }

        var strFilter = StringBuilderCacheAlt.ReturnAndFree(sqlFilter);
        dbCmd.CommandText = $"UPDATE {this.GetQuotedTableName(modelDef)} " +
                            $"SET {StringBuilderCache.ReturnAndFree(sql)}{(strFilter.Length > 0 ? " WHERE " + strFilter : string.Empty)}";

        if (sql.Length == 0)
        {
            throw new Exception(
                "No valid update properties provided (e.g. p => p.FirstName): " + dbCmd.CommandText);
        }
    }

    /// <summary>
    /// Prepares the update row statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <exception cref="Exception">No valid update properties provided (e.g. () => new Person { Age = 27 }): " + dbCmd.CommandText</exception>
    /// <exception cref="System.Exception">No valid update properties provided (e.g. () =&gt; new Person { Age = 27 }): " + dbCmd.CommandText</exception>
    public virtual void PrepareUpdateRowStatement<T>(
        IDbCommand dbCmd,
        Dictionary<string, object> args,
        string sqlFilter)
    {
        var sql = StringBuilderCache.Allocate();
        var modelDef = typeof(T).GetModelDefinition();

        foreach (var entry in args)
        {
            var fieldDef = modelDef.AssertFieldDefinition(entry.Key);
            if (fieldDef.ShouldSkipUpdate() || fieldDef.IsPrimaryKey || fieldDef.AutoIncrement)
            {
                continue;
            }

            var value = entry.Value;

            try
            {
                if (sql.Length > 0)
                {
                    sql.Append(", ");
                }

                sql.Append(this.GetQuotedColumnName(fieldDef));

                sql.Append('=');

                sql.Append(this.GetUpdateParam(dbCmd, value, fieldDef));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ERROR in PrepareUpdateRowStatement(cmd,args): {ex.Message}");
                if (OrmLiteConfig.ThrowOnError)
                {
                    throw;
                }
            }
        }

        dbCmd.CommandText = $"UPDATE {this.GetQuotedTableName(modelDef)} " +
                            $"SET {StringBuilderCache.ReturnAndFree(sql)}{(string.IsNullOrEmpty(sqlFilter) ? string.Empty : " ")}{sqlFilter}";

        if (sql.Length == 0)
        {
            throw new Exception(
                "No valid update properties provided (e.g. () => new Person { Age = 27 }): " + dbCmd.CommandText);
        }
    }

    /// <summary>
    /// Prepares the upsert row statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="model">The model.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <exception cref="Exception">$"No valid update properties provided (e.g. () => new Person {{ Age = 27 }}): {dbCmd.CommandText}</exception>
    public virtual void PrepareUpsertRowStatement<T>(
        IDbCommand dbCmd,
        T model,
        string sqlFilter)
    {
        var sql = StringBuilderCache.Allocate();
        var modelDef = typeof(T).GetModelDefinition();

        var sbColumnNames = StringBuilderCache.Allocate();
        var sbColumnValues = StringBuilderCacheAlt.Allocate();


        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (fieldDef.ShouldSkipUpdate() || fieldDef.IsPrimaryKey || fieldDef.AutoIncrement)
            {
                continue;
            }

            var value = fieldDef.GetValue(model);

            try
            {
                // Set column name
                if (sbColumnNames.Length > 0)
                {
                    sbColumnNames.Append(',');
                }

                sbColumnNames.Append(this.GetQuotedColumnName(fieldDef));


                if (sql.Length > 0)
                {
                    sql.Append(", ");
                }

                sql.Append(this.GetQuotedColumnName(fieldDef));

                sql.Append('=');

                // set column values
                var updateValue = this.GetUpdateParam(dbCmd, value, fieldDef);

                sql.Append(updateValue);

                if (sbColumnValues.Length > 0)
                {
                    sbColumnValues.Append(',');
                }

                sbColumnValues.Append(updateValue);
            }
            catch (Exception ex)
            {
                OrmLiteUtils.HandleException(ex, "ERROR in PrepareUpdateRowStatement(cmd,args): " + ex.Message);
            }
        }

        var upsertSql = StringBuilderCache.Allocate();

        upsertSql.Append($" UPDATE {this.GetQuotedTableName(modelDef)} ");
        upsertSql.Append(
            $" SET {StringBuilderCache.ReturnAndFree(sql)}{(string.IsNullOrEmpty(sqlFilter) ? string.Empty : " ")}{sqlFilter};");

        upsertSql.Append(
            $" INSERT INTO {this.GetQuotedTableName(modelDef)}({StringBuilderCache.ReturnAndFree(sbColumnNames)}) ");
        upsertSql.Append($" SELECT {StringBuilderCacheAlt.ReturnAndFree(sbColumnValues)}");

        upsertSql.Append(" WHERE NOT EXISTS (");

        upsertSql.Append(
            $"SELECT 1 FROM {this.GetQuotedTableName(modelDef)} {(string.IsNullOrEmpty(sqlFilter) ? string.Empty : " ")}{sqlFilter}");

        upsertSql.Append(");");

        dbCmd.CommandText = StringBuilderCache.ReturnAndFree(upsertSql);

        if (upsertSql.Length == 0)
        {
            throw new Exception(
                $"No valid update properties provided (e.g. () => new Person {{ Age = 27 }}): {dbCmd.CommandText}");
        }
    }

    /// <summary>
    /// Prepares the update row add statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <exception cref="Exception">No valid update properties provided (e.g. () => new Person { Age = 27 }): " + dbCmd.CommandText</exception>
    /// <exception cref="System.Exception">No valid update properties provided (e.g. () =&gt; new Person { Age = 27 }): " + dbCmd.CommandText</exception>
    public virtual void PrepareUpdateRowAddStatement<T>(
        IDbCommand dbCmd,
        Dictionary<string, object> args,
        string sqlFilter)
    {
        var sql = StringBuilderCache.Allocate();
        var modelDef = typeof(T).GetModelDefinition();

        foreach (var entry in args)
        {
            var fieldDef = modelDef.AssertFieldDefinition(entry.Key);
            if (fieldDef.ShouldSkipUpdate() || fieldDef.AutoIncrement || fieldDef.IsPrimaryKey ||
                fieldDef.IsRowVersion || fieldDef.Name == OrmLiteConfig.IdField)
            {
                continue;
            }

            var value = entry.Value;

            try
            {
                if (sql.Length > 0)
                {
                    sql.Append(", ");
                }

                var quotedFieldName = this.GetQuotedColumnName(fieldDef);

                if (fieldDef.FieldType.IsNumericType())
                {
                    sql.Append(quotedFieldName).Append('=').Append(quotedFieldName).Append('+')
                        .Append(this.GetUpdateParam(dbCmd, value, fieldDef));
                }
                else
                {
                    sql.Append(quotedFieldName).Append('=').Append(this.GetUpdateParam(dbCmd, value, fieldDef));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ERROR in PrepareUpdateRowAddStatement(): {ex.Message}");
                if (OrmLiteConfig.ThrowOnError)
                {
                    throw;
                }
            }
        }

        dbCmd.CommandText = $"UPDATE {this.GetQuotedTableName(modelDef)} " +
                            $"SET {StringBuilderCache.ReturnAndFree(sql)}{(string.IsNullOrEmpty(sqlFilter) ? string.Empty : " ")}{sqlFilter}";

        if (sql.Length == 0)
        {
            throw new Exception(
                "No valid update properties provided (e.g. () => new Person { Age = 27 }): " + dbCmd.CommandText);
        }
    }

    /// <summary>
    /// Converts to deletestatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>System.String.</returns>
    public virtual string ToDeleteStatement(Type tableType, string sqlFilter, params object[] filterParams)
    {
        var sql = StringBuilderCache.Allocate();
        const string deleteStatement = "DELETE ";

        var isFullDeleteStatement = !string.IsNullOrEmpty(sqlFilter) && sqlFilter.Length > deleteStatement.Length &&
                                    sqlFilter[..deleteStatement.Length].ToUpper()
                                        .Equals(deleteStatement);

        if (isFullDeleteStatement)
        {
            return sqlFilter.SqlFmt(this, filterParams);
        }

        var modelDef = tableType.GetModelDefinition();
        sql.Append($"DELETE FROM {this.GetQuotedTableName(modelDef)}");

        if (string.IsNullOrEmpty(sqlFilter))
        {
            return StringBuilderCache.ReturnAndFree(sql);
        }

        sqlFilter = sqlFilter.SqlFmt(this, filterParams);
        sql.Append(" WHERE ");
        sql.Append(sqlFilter);

        return StringBuilderCache.ReturnAndFree(sql);
    }

    /// <summary>
    /// Read each field of a row individually instead of with a single IDataReader.GetValues() call
    /// </summary>
    /// <value><c>true</c> if [deoptimize reader]; otherwise, <c>false</c>.</value>
    public bool DeoptimizeReader { get; set; }

    /// <summary>
    /// Determines whether [has insert return values] [the specified model definition].
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns><c>true</c> if [has insert return values] [the specified model definition]; otherwise, <c>false</c>.</returns>
    public virtual bool HasInsertReturnValues(ModelDefinition modelDef)
    {
        return modelDef.FieldDefinitions.Any(x => x.ReturnOnInsert);
    }

    /// <summary>
    /// Gets the table hint that locks the selected rows for SqlExpression.ForUpdate().
    /// </summary>
    /// <param name="skipLocked">if set to <c>true</c> [skip locked].</param>
    /// <returns>System.String.</returns>
    public virtual string GetForUpdateTableHint(bool skipLocked) => null;

    /// <summary>
    /// Gets the clause that locks the selected rows for SqlExpression.ForUpdate().
    /// </summary>
    /// <param name="lockTable">The lock table.</param>
    /// <param name="skipLocked">if set to <c>true</c> [skip locked].</param>
    /// <returns>System.String.</returns>
    public virtual string GetForUpdateClause(string lockTable, bool skipLocked) =>
        "FOR UPDATE" + (lockTable != null ? " OF " + lockTable : "") + (skipLocked ? " SKIP LOCKED" : "");

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    public string GetDefaultValue(Type tableType, string fieldName)
    {
        var modelDef = tableType.GetModelDefinition();
        var fieldDef = modelDef.AssertFieldDefinition(fieldName);
        return this.GetDefaultValue(fieldDef);
    }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDefaultValue(FieldDefinition fieldDef)
    {
        var defaultValue = fieldDef.DefaultValue;
        if (string.IsNullOrEmpty(defaultValue))
        {
            return fieldDef.AutoId ? this.GetAutoIdDefaultValue(fieldDef) : null;
        }

        return this.ResolveFragment(defaultValue);
    }

    /// <summary>
    /// Resolves the fragment.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <returns>System.String.</returns>
    public virtual string ResolveFragment(string sql)
    {
        if (string.IsNullOrEmpty(sql))
        {
            return null;
        }

        return !sql.StartsWith('{') ? sql : this.Variables.GetValueOrDefault(sql);
    }

    /// <summary>
    /// Gets the automatic identifier default value.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetAutoIdDefaultValue(FieldDefinition fieldDef)
    {
        return null;
    }

    /// <summary>
    /// Gets or sets the create table fields strategy.
    /// </summary>
    /// <value>The create table fields strategy.</value>
    public Func<ModelDefinition, IEnumerable<FieldDefinition>> CreateTableFieldsStrategy { get; set; } =
        GetFieldDefinitions;

    /// <summary>
    /// Gets the field definitions.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>List&lt;FieldDefinition&gt;.</returns>
    public static IEnumerable<FieldDefinition> GetFieldDefinitions(ModelDefinition modelDef)
    {
        return modelDef.FieldDefinitions.OrderBy(fd => fd.Order);
    }

    /// <summary>
    /// Converts to createschemastatement.
    /// </summary>
    /// <param name="schemaName">Name of the schema.</param>
    /// <returns>System.String.</returns>
    public abstract string ToCreateSchemaStatement(string schemaName);

    /// <summary>
    /// Gets the schemas.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Collections.Generic.List&lt;string&gt;.</returns>
    public virtual List<string> GetSchemas(IDbCommand dbCmd)
    {
        return ["default"];
    }

    /// <summary>
    /// Gets the schema tables.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Collections.Generic.Dictionary&lt;string, System.Collections.Generic.List&lt;string&gt;&gt;.</returns>
    public virtual Dictionary<string, List<string>> GetSchemaTables(IDbCommand dbCmd)
    {
        return [];
    }

    /// <summary>
    /// Doeses the schema exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="schemaName">Name of the schema.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public abstract bool DoesSchemaExist(IDbCommand dbCmd, string schemaName);

    /// <summary>
    /// Doeses the schema exist asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public virtual Task<bool> DoesSchemaExistAsync(
        IDbCommand dbCmd,
        string schema,
        CancellationToken token = default)
    {
        return this.DoesSchemaExist(dbCmd, schema).InTask();
    }

    /// <summary>
    /// Converts to createtablestatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>System.String.</returns>
    public virtual string ToCreateTableStatement(Type tableType)
    {
        var sbColumns = StringBuilderCache.Allocate();
        var sbConstraints = StringBuilderCacheAlt.Allocate();

        var modelDef = tableType.GetModelDefinition();
        foreach (var fieldDef in this.CreateTableFieldsStrategy(modelDef))
        {
            if (fieldDef.ShouldSkipCreate())
            {
                continue;
            }

            var columnDefinition = this.GetColumnDefinition(fieldDef, modelDef);

            if (columnDefinition == null)
            {
                continue;
            }

            if (sbColumns.Length != 0)
            {
                sbColumns.Append(", \n  ");
            }

            sbColumns.Append(columnDefinition);

            var sqlConstraint = this.GetCheckConstraint(modelDef, fieldDef);
            if (sqlConstraint != null)
            {
                sbConstraints.Append(",\n" + sqlConstraint);
            }

            if (fieldDef.ForeignKey == null || OrmLiteConfig.SkipForeignKeys)
            {
                continue;
            }

            var refModelDef = fieldDef.ForeignKey.ReferenceType.GetModelDefinition();
            sbConstraints.Append(
                $", \n\n  CONSTRAINT {this.GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, this.NamingStrategy, fieldDef))} " +
                $"FOREIGN KEY ({this.GetQuotedColumnName(fieldDef)}) " +
                $"REFERENCES {this.GetQuotedTableName(refModelDef)} ({this.GetQuotedColumnName(refModelDef.PrimaryKey)})");

            sbConstraints.Append(this.GetForeignKeyOnDeleteClause(fieldDef.ForeignKey));
            sbConstraints.Append(this.GetForeignKeyOnUpdateClause(fieldDef.ForeignKey));
        }

        var uniqueConstraints = this.GetUniqueConstraints(modelDef);
        if (uniqueConstraints != null)
        {
            sbConstraints.Append(",\n" + uniqueConstraints);
        }

        if (modelDef.CompositePrimaryKeys.Count != 0)
        {
            sbConstraints.Append(",\n");

            sbConstraints.Append($" CONSTRAINT {this.GetPrimaryKeyName(modelDef)} PRIMARY KEY (");

            sbConstraints.Append(
                modelDef.CompositePrimaryKeys[0].FieldNames.Map(f => modelDef.GetQuotedName(f, this))
                    .Join(","));

            sbConstraints.Append(") ");
        }

        var sql =
            $"CREATE TABLE {this.GetQuotedTableName(modelDef)} \n(\n  {StringBuilderCache.ReturnAndFree(sbColumns)}{StringBuilderCacheAlt.ReturnAndFree(sbConstraints)} \n); \n";

        return this.WithSystemVersioning(modelDef, sql);
    }

    /// <summary>
    /// Gets the unique constraints.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetUniqueConstraints(ModelDefinition modelDef)
    {
        var constraints = modelDef.UniqueConstraints.Map(
            x =>
                $"CONSTRAINT {this.GetUniqueConstraintName(x, this.GetTableNameOnly(new TableRef(modelDef)))} UNIQUE ({x.FieldNames.Map(f => modelDef.GetQuotedName(f, this)).Join(",")})");

        return constraints.Count > 0 ? constraints.Join(",\n") : null;
    }

    /// <summary>
    /// Gets the name of the unique constraint.
    /// </summary>
    /// <param name="constraint">The constraint.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>System.String.</returns>
    protected virtual string GetUniqueConstraintName(UniqueConstraintAttribute constraint, string tableName)
    {
        return constraint.Name ?? $"UC_{tableName}_{constraint.FieldNames.Join("_")}";
    }

    /// <summary>
    /// Gets the check constraint.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetCheckConstraint(ModelDefinition modelDef, FieldDefinition fieldDef)
    {
        var constraint = fieldDef.CheckConstraint;
        if (fieldDef.CheckEnum)
        {
            var enumConstraint = this.GetEnumCheckConstraint(fieldDef);
            constraint = constraint == null
                ? enumConstraint
                : $"({constraint}) AND {enumConstraint}";
        }

        if (constraint == null)
        {
            return null;

        }

        return
            $"CONSTRAINT CHK_{modelDef.Schema}_{modelDef.ModelName}_{fieldDef.FieldName} CHECK ({constraint})";
    }

    /// <summary>
    /// The condition a [CheckEnum] column is constrained by, which only allows the values of its Enum as they're
    /// stored, e.g: "Status" IN ('New','Shipped')
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    protected virtual string GetEnumCheckConstraint(FieldDefinition fieldDef)
    {
        var enumType = fieldDef.FieldType;
        if (!enumType.IsEnum)
        {
            throw new NotSupportedException($"[CheckEnum] is only valid on Enum properties, {fieldDef.Name} is a {enumType.Name}");
        }

        if (enumType.HasAttributeCached<FlagsAttribute>())
        {
            throw new NotSupportedException($"[CheckEnum] isn't supported on [Flags] Enums like {enumType.Name}, whose values are combined");
        }

        var values = new List<string>();
        foreach (var value in Enum.GetValues(enumType))
        {
            var quotedValue = this.GetQuotedValue(value, enumType);
            if (!values.Contains(quotedValue))
            {
                values.Add(quotedValue);
            }
        }

        return $"{this.GetQuotedColumnName(fieldDef)} IN ({string.Join(",", values)})";
    }

    /// <summary>
    /// The statements that add the [Description] of a table and its columns as comments
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public virtual List<string> ToCreateCommentStatements(Type tableType) => [];

    /// <summary>
    /// Converts to postcreatetablestatement.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string ToPostCreateTableStatement(ModelDefinition modelDef)
    {
        return null;
    }

    /// <summary>
    /// Converts to postdroptablestatement.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string ToPostDropTableStatement(ModelDefinition modelDef)
    {
        return null;
    }

    public virtual string ToDropTableStatement(ModelDefinition modelDef) =>
        $"DROP TABLE {this.GetQuotedTableName(modelDef)}";

    /// <summary>
    /// Whether the RDBMS can keep previous versions of a table's rows, from [SystemVersioned]
    /// </summary>
    public virtual bool SupportsSystemVersioning => false;

    protected NotSupportedException SystemVersioningNotSupported() => new(
        $"{this.GetType().Name} doesn't support system-versioned tables, which need SQL Server 2016+ or MariaDB 10.3+");

    public virtual string ToSystemTimeClause(string condition) => this.SupportsSystemVersioning
        ? "FOR SYSTEM_TIME " + condition
        : throw this.SystemVersioningNotSupported();

    public virtual DateTime ToSystemTime(DateTime time) => time;

    /// <summary>
    /// The column of a [RowStart] or [RowEnd] property of a [SystemVersioned] table
    /// </summary>
    protected virtual string GetSystemTimeColumnDefinition(FieldDefinition fieldDef) => throw this.SystemVersioningNotSupported();

    /// <summary>
    /// Makes the CREATE TABLE statement of a [SystemVersioned] table create it system-versioned
    /// </summary>
    protected virtual string ToSystemVersionedTableStatement(ModelDefinition modelDef, string createTableSql) =>
        throw this.SystemVersioningNotSupported();

    // The [RowStart] and [RowEnd] fields of a [SystemVersioned] table, which has both or neither
    protected static void GetSystemTimeFields(ModelDefinition modelDef, out FieldDefinition rowStart, out FieldDefinition rowEnd)
    {
        rowStart = modelDef.FieldDefinitions.FirstOrDefault(x => x.IsRowStart);
        rowEnd = modelDef.FieldDefinitions.FirstOrDefault(x => x.IsRowEnd);
        if ((rowStart == null) != (rowEnd == null))
        {
            throw new NotSupportedException($"{modelDef.Name} needs both a [RowStart] and a [RowEnd] property, or neither");
        }
    }

    // The CREATE TABLE statement, system-versioned when the table is [SystemVersioned]
    protected string WithSystemVersioning(ModelDefinition modelDef, string createTableSql) => modelDef.IsSystemVersioned
        ? this.ToSystemVersionedTableStatement(modelDef, createTableSql)
        : createTableSql;

    // Adds column and table definitions to a CREATE TABLE statement, before and after the ) that ends its columns
    protected static string AddToCreateTable(string createTableSql, string definitions, string tableOptions)
    {
        var endPos = createTableSql.LastIndexOf(')');
        if (endPos < 0)
        {
            throw new NotSupportedException("Could not find the end of the columns in: " + createTableSql);
        }

        return createTableSql.Substring(0, endPos).TrimEnd() +
               (string.IsNullOrEmpty(definitions) ? "" : ",\n  " + definitions) +
               " \n)" + tableOptions + createTableSql.Substring(endPos + 1);
    }

    /// <summary>
    /// Gets the foreign key on delete clause.
    /// </summary>
    /// <param name="foreignKey">The foreign key.</param>
    /// <returns>System.String.</returns>
    public virtual string GetForeignKeyOnDeleteClause(ForeignKeyConstraint foreignKey)
    {
        return !string.IsNullOrEmpty(foreignKey.OnDelete) ? " ON DELETE " + foreignKey.OnDelete : string.Empty;
    }

    /// <summary>
    /// Gets the foreign key on update clause.
    /// </summary>
    /// <param name="foreignKey">The foreign key.</param>
    /// <returns>System.String.</returns>
    public virtual string GetForeignKeyOnUpdateClause(ForeignKeyConstraint foreignKey)
    {
        return !string.IsNullOrEmpty(foreignKey.OnUpdate) ? " ON UPDATE " + foreignKey.OnUpdate : string.Empty;
    }

    /// <summary>
    /// Converts to createindexstatements.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public virtual List<string> ToCreateIndexStatements(Type tableType)
    {
        var sqlIndexes = new List<string>();

        var modelDef = tableType.GetModelDefinition();
        foreach (var fieldDef in modelDef.FieldDefinitions)
        {
            if (!fieldDef.IsIndexed)
            {
                continue;
            }

            var indexName = fieldDef.IndexName ?? this.GetIndexName(
                                fieldDef.IsUniqueIndex,
                                modelDef.ModelName.SafeVarName(),
                                fieldDef.FieldName);

            if (fieldDef.VectorDimensions != null)
            {
                // A vector index where the RDBMS has one, as other indexes can't be used to find similar vectors
                var sqlVectorIndex = ToCreateVectorIndexStatement(modelDef, fieldDef, indexName);
                if (sqlVectorIndex != null)
                {
                    sqlIndexes.Add(sqlVectorIndex);
                }

                continue;
            }

            var keyColumns = this.GetIndexKeyColumns(modelDef, fieldDef.IsUniqueIndex, this.GetQuotedColumnName(fieldDef), fieldDef.IndexInclude);
            sqlIndexes.Add(this.WithIndexOptions(modelDef, fieldDef.IndexInclude, fieldDef.IndexWhere, keyColumns != null
                ? ToCreateIndexStatement(fieldDef.IsUniqueIndex, indexName, modelDef, keyColumns, isCombined: true, fieldDef: fieldDef)
                : ToCreateIndexStatement(fieldDef.IsUniqueIndex, indexName, modelDef, fieldDef.FieldName, isCombined: false, fieldDef: fieldDef)));
        }

        foreach (var compositeIndex in modelDef.CompositeIndexes)
        {
            var indexName = this.GetCompositeIndexName(compositeIndex, modelDef);

            var sb = StringBuilderCache.Allocate();
            foreach (var fieldName in compositeIndex.FieldNames)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                var parts = fieldName.SplitOnLast(' ');
                if (parts.Length == 2 &&
                    (parts[1].StartsWith("desc", StringComparison.CurrentCultureIgnoreCase) || parts[1].StartsWith("asc", StringComparison.CurrentCultureIgnoreCase)))
                {
                    var name = parts[0];
                    var fieldDef = modelDef.GetFieldDefinition(name);
                    sb.Append(fieldDef != null ? this.GetQuotedColumnName(fieldDef) : this.GetQuotedColumnName(name)).Append(' ').Append(parts[1]);
                }
                else
                {
                    var fieldDef = modelDef.GetFieldDefinition(fieldName);
                    sb.Append(fieldDef != null ? this.GetQuotedColumnName(fieldDef) : this.GetQuotedColumnName(fieldName));
                }
            }

            var columns = StringBuilderCache.ReturnAndFree(sb);
            sqlIndexes.Add(this.WithIndexOptions(modelDef, compositeIndex.Include, compositeIndex.Where,
                ToCreateIndexStatement(compositeIndex.Unique, indexName, modelDef,
                    GetIndexKeyColumns(modelDef, compositeIndex.Unique, columns, compositeIndex.Include) ?? columns,
                    isCombined: true)));
        }

        return sqlIndexes;
    }

    /// <summary>
    /// Whether indexes can keep other columns with INCLUDE. Where they can't, the columns of a covering index
    /// are added to the key columns of non-unique indexes instead.
    /// </summary>
    protected virtual bool SupportsIndexInclude => false;

    /// <summary>
    /// Whether an index can be limited to the rows matching a condition
    /// </summary>
    protected virtual bool SupportsFilteredIndexes => true;

    private string GetQuotedIndexColumn(ModelDefinition modelDef, string name) =>
        modelDef.GetFieldDefinition(name) is { } fieldDef
            ? this.GetQuotedColumnName(fieldDef)
            : this.GetQuotedColumnName(name);

    // The key columns with the columns to include, for an RDBMS without INCLUDE. Returns null if they're unchanged.
    private string GetIndexKeyColumns(ModelDefinition modelDef, bool isUnique, string keyColumns, string[] include)
    {
        // Adding columns to a unique index would change which rows it allows
        if (include == null || include.Length == 0 || this.SupportsIndexInclude || isUnique)
        {
            return null;
        }

        return keyColumns + ", " + string.Join(", ", include.Map(x => GetQuotedIndexColumn(modelDef, x)));
    }

    // Adds the INCLUDE and WHERE of a covering or filtered index to its CREATE INDEX statement
    private string WithIndexOptions(ModelDefinition modelDef, string[] include, string where, string sql)
    {
        var options = "";
        if (include is { Length: > 0 } && this.SupportsIndexInclude)
        {
            options += $" INCLUDE ({string.Join(", ", include.Map(x => GetQuotedIndexColumn(modelDef, x)))})";
        }

        if (!string.IsNullOrEmpty(where))
        {
            if (!this.SupportsFilteredIndexes)
            {
                throw new NotSupportedException($"{this.GetType().Name} doesn't support filtered indexes, used by {modelDef.Name}");
            }

            options += " WHERE " + this.ResolveColumnRefs(modelDef, where);
        }
        if (options.Length == 0)
        {
            return sql;
        }

        var endPos = sql.LastIndexOf(';');
        return endPos >= 0
            ? sql.Substring(0, endPos) + options + sql.Substring(endPos)
            : sql + options;
    }

    /// <summary>
    /// Doeses the table exist.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public virtual bool DoesTableExist(IDbConnection db, TableRef tableRef)
    {
        return db.Exec(dbCmd => this.DoesTableExist(dbCmd, tableRef));
    }

    /// <summary>
    /// Does table exist as an asynchronous operation.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
    public async virtual Task<bool> DoesTableExistAsync(
        IDbConnection db,
        TableRef tableRef,
        CancellationToken token = default)
    {
        return await db.Exec(async dbCmd => await this.DoesTableExistAsync(dbCmd, tableRef, token));
    }

    /// <summary>
    /// Doeses the table exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual bool DoesTableExist(IDbCommand dbCmd, TableRef tableRef)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Doeses the table exist asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public virtual Task<bool> DoesTableExistAsync(
        IDbCommand dbCmd,
        TableRef tableRef,
        CancellationToken token = default)
    {
        return this.DoesTableExist(dbCmd, tableRef).InTask();
    }

    /// <summary>
    /// Doeses the column exist.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual bool DoesColumnExist(IDbConnection db, string columnName, TableRef tableRef)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Doeses the column exist asynchronous.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public virtual Task<bool> DoesColumnExistAsync(
        IDbConnection db,
        string columnName,
        TableRef tableRef,
        CancellationToken token = default)
    {
        return this.DoesColumnExist(db, columnName, tableRef).InTask();
    }

    /// <summary>
    /// Gets the type of the column data.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string GetColumnDataType(
        IDbConnection db,
        string columnName,
        TableRef tableRef)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Columns the is nullable.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual bool ColumnIsNullable(
        IDbConnection db,
        string columnName,
        TableRef tableRef)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the maximum length of the column.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <param name="tableRef">The table reference.</param>
    /// <returns>System.Int64.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual long GetColumnMaxLength(
        IDbConnection db,
        string columnName,
        TableRef tableRef)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Doeses the sequence exist.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sequence">Name of the sequence.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual bool DoesSequenceExist(IDbCommand dbCmd, string sequence)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Doeses the sequence exist asynchronous.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="sequenceName">Name of the sequence.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public virtual Task<bool> DoesSequenceExistAsync(
        IDbCommand dbCmd,
        string sequenceName,
        CancellationToken token = default)
    {
        return this.DoesSequenceExist(dbCmd, sequenceName).InTask();
    }

    /// <summary>
    /// Gets the name of the index.
    /// </summary>
    /// <param name="isUnique">if set to <c>true</c> [is unique].</param>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>System.String.</returns>
    protected virtual string GetIndexName(bool isUnique, string modelName, string fieldName)
    {
        return $"{(isUnique ? "u" : string.Empty)}idx_{modelName}_{fieldName}".ToLower();
    }

    /// <summary>
    /// Gets the name of the composite index.
    /// </summary>
    /// <param name="compositeIndex">Index of the composite.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    protected virtual string GetCompositeIndexName(CompositeIndexAttribute compositeIndex, ModelDefinition modelDef)
    {
        return compositeIndex.Name ?? this.GetIndexName(
                   compositeIndex.Unique,
                   modelDef.ModelName.SafeVarName(),
                   string.Join("_", compositeIndex.FieldNames.Map(x => x.LeftPart(' ')).ToArray()));
    }

    /// <summary>
    /// Gets the composite index name with schema.
    /// </summary>
    /// <param name="compositeIndex">Index of the composite.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    protected virtual string GetCompositeIndexNameWithSchema(
        CompositeIndexAttribute compositeIndex,
        ModelDefinition modelDef)
    {
        return compositeIndex.Name ?? this.GetIndexName(
                   compositeIndex.Unique,
                   (modelDef.IsInSchema
                        ? modelDef.Schema + "_" + this.GetQuotedTableName(modelDef)
                        : this.GetQuotedTableName(modelDef)).SafeVarName(),
                   string.Join("_", compositeIndex.FieldNames.ToArray()));
    }

    /// <summary>
    /// Converts to createindexstatement.
    /// </summary>
    /// <param name="isUnique">if set to <c>true</c> [is unique].</param>
    /// <param name="indexName">Name of the index.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="isCombined">if set to <c>true</c> [is combined].</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    protected virtual string ToCreateIndexStatement(
        bool isUnique,
        string indexName,
        ModelDefinition modelDef,
        string fieldName,
        bool isCombined = false,
        FieldDefinition fieldDef = null)
    {
        fieldDef ??= modelDef.GetFieldDefinition(fieldName);
        return "CREATE" + (isUnique ? " UNIQUE" : string.Empty) +
               (fieldDef?.IsClustered == true ? " CLUSTERED" : string.Empty) +
               (fieldDef?.IsNonClustered == true ? " NONCLUSTERED" : string.Empty) +
               $" INDEX {indexName} ON {this.GetQuotedTableName(modelDef)} " +
               $"({(isCombined
                   ? fieldName
                   : fieldDef != null
                       ? this.GetQuotedColumnName(fieldDef)
                       : this.GetQuotedColumnName(fieldName))}); \n";
    }

    /// <summary>
    /// Converts to createsequencestatements.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public virtual List<string> ToCreateSequenceStatements(Type tableType)
    {
        return [];
    }

    /// <summary>
    /// Converts to createsequencestatement.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="sequenceName">Name of the sequence.</param>
    /// <returns>System.String.</returns>
    public virtual string ToCreateSequenceStatement(Type tableType, string sequenceName)
    {
        return string.Empty;
    }

    public virtual string ToResetSequenceStatement(Type tableType, string columnName, int value)
    {
        return "";
    }

    /// <summary>
    /// Converts to createsavepoint.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string ToCreateSavePoint(string name)
    {
        return $"SAVEPOINT {name}";
    }

    /// <summary>
    /// Converts to releasesavepoint.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string ToReleaseSavePoint(string name)
    {
        return $"RELEASE SAVEPOINT {name}";
    }

    /// <summary>
    /// Converts to rollbacksavepoint.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string ToRollbackSavePoint(string name)
    {
        return $"ROLLBACK TO SAVEPOINT {name}";
    }

    /// <summary>
    /// Sequences the list.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public virtual List<string> SequenceList(Type tableType)
    {
        return [];
    }

    /// <summary>
    /// Sequences the list asynchronous.
    /// </summary>
    /// <param name="tableType">Type of the table.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
    public virtual Task<List<string>> SequenceListAsync(Type tableType, CancellationToken token = default)
    {
        return new List<string>().InTask();
    }

    // TODO : make abstract  ??
    /// <summary>
    /// Converts to existstatement.
    /// </summary>
    /// <param name="fromTableType">Type of from table.</param>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string ToExistStatement(
        Type fromTableType,
        object objWithProperties,
        string sqlFilter,
        params object[] filterParams)
    {
        throw new NotImplementedException();
    }

    // TODO : make abstract  ??
    /// <summary>
    /// Converts to selectfromprocedurestatement.
    /// </summary>
    /// <param name="fromObjWithProperties">From object with properties.</param>
    /// <param name="outputModelType">Type of the output model.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <param name="filterParams">The filter parameters.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string ToSelectFromProcedureStatement(
        object fromObjWithProperties,
        Type outputModelType,
        string sqlFilter,
        params object[] filterParams)
    {
        throw new NotImplementedException();
    }

    // TODO : make abstract  ??
    /// <summary>
    /// Converts to executeprocedurestatement.
    /// </summary>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <returns>System.String.</returns>
    public virtual string ToExecuteProcedureStatement(object objWithProperties)
    {
        return null;
    }

    /// <summary>
    /// Gets the model.
    /// </summary>
    /// <param name="modelType">Type of the model.</param>
    /// <returns>ModelDefinition.</returns>
    protected static ModelDefinition GetModel(Type modelType)
    {
        return modelType.GetModelDefinition();
    }

    /// <summary>
    /// SQLs the expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>SqlExpression&lt;T&gt;.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual SqlExpression<T> SqlExpression<T>()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates the parameterized delete statement.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="objWithProperties">The object with properties.</param>
    /// <returns>IDbCommand.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public IDbCommand CreateParameterizedDeleteStatement(IDbConnection connection, object objWithProperties)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the drop function.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="functionName">Name of the function.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDropFunction(string database, string functionName)
    {
        return null;
    }

    /// <summary>
    /// Gets the create view.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="selectSql">The select SQL.</param>
    /// <returns>System.String.</returns>
    public virtual string GetCreateView(string database, ModelDefinition modelDef, StringBuilder selectSql)
    {
        return null;
    }

    /// <summary>
    /// Gets the drop view.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDropView(string database, ModelDefinition modelDef)
    {
        return null;
    }

    /// <summary>
    /// Gets the create index view.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <param name="selectSql">The select SQL.</param>
    /// <returns>System.String.</returns>
    public virtual string GetCreateIndexView(ModelDefinition modelDef, string name, string selectSql)
    {
        return null;
    }

    /// <summary>
    /// Gets the drop index view.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDropIndexView(ModelDefinition modelDef, string name)
    {
        return null;
    }

    /// <summary>
    /// Gets the drop index constraint.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDropIndexConstraint(ModelDefinition modelDef, string name = null)
    {
        return null;
    }

    /// <summary>
    /// Gets the add composite primary key sql command.
    /// </summary>
    /// <param name="database">The database name.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldNameA">The field name a.</param>
    /// <param name="fieldNameB">The field name b.</param>
    /// <returns>Returns the SQL Command</returns>
    public virtual string GetAddCompositePrimaryKey(string database, ModelDefinition modelDef, string fieldNameA, string fieldNameB)
    {
        return null;
    }

    /// <summary>
    /// Gets the name of the primary key.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>Returns the Primary Key Name</returns>
    public virtual string GetPrimaryKeyName(ModelDefinition modelDef)
    {
        return null;
    }

    /// <summary>
    /// Gets the drop primary key constraint.
    /// </summary>
    /// <param name="database">The database name.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name)
    {
        return null;
    }

    /// <summary>
    /// Gets the drop primary key constraint.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <param name="fieldNameA">The field name a.</param>
    /// <param name="fieldNameB">The field name b.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDropPrimaryKeyConstraint(string database, ModelDefinition modelDef, string name, string fieldNameA, string fieldNameB)
    {
        return null;
    }

    /// <summary>
    /// Gets the drop foreign key constraint.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDropForeignKeyConstraint(ModelDefinition modelDef, string name)
    {
        return null;
    }

    /// <summary>
    /// Gets the name of the constraint.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>string.</returns>
    public virtual string GetConstraintName(string database, ModelDefinition modelDef, string fieldName)
    {
        return null;
    }


    /// <summary>
    /// Gets the drop constraint.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDropConstraint(ModelDefinition modelDef, string name)
    {
        return null;
    }

    /// <summary>
    /// Gets the drop foreign key constraints.
    /// </summary>
    /// <param name="modelDef">The model definition.</param>
    /// <returns>System.String.</returns>
    public virtual string GetDropForeignKeyConstraints(ModelDefinition modelDef)
    {
        return null;
    }

    public virtual ColumnSchema[] GetSchemaColumns(IDbConnection db, string quotedTable) =>
        db.GetTableColumns($"SELECT * FROM {quotedTable} WHERE 1=0");

    public virtual ColumnSchema[] GetModelSchemaColumns(IDbConnection db, List<FieldDefinition> fieldDefs)
    {
        var tempTable = this.GetBulkStagingTableName("ormlite_diff_" + Guid.NewGuid().ToString("N"));
        // Named constraints have to be unique, which the constraints of the model's table already are
        var columns = fieldDefs.Map(x => this.GetColumnDefinition(x.DefaultValueConstraint == null
            ? x
            : x.Clone(f =>
            {
                f.ModelDef = x.ModelDef;
                f.DefaultValueConstraint = null;
            })));
        db.ExecuteSql(this.ToCreateTempTableStatement(tempTable, columns.Join(",\n  ")));
        try
        {
            var schemaColumns = this.GetSchemaColumns(db, tempTable);
            // The defaults the model's columns are created with, as the database writes them
            var defaults = this.GetColumnDefaults(db, tempTable);
            if (defaults != null)
            {
                foreach (var column in schemaColumns)
                {
                    column.DefaultValue = defaults.TryGetValue(column.ColumnName, out var defaultValue) ? defaultValue : null;
                }
            }
            return schemaColumns;
        }
        finally
        {
            db.ExecuteSql(this.ToDropBulkStagingTableStatement(tempTable));
        }
    }

    protected virtual string ToCreateTempTableStatement(string tempTable, string columnDefinitions) =>
        $"CREATE TEMPORARY TABLE {tempTable} (\n  {columnDefinitions}\n)";

    public virtual List<string> GetTableIndexNames(IDbConnection db, TableRef tableRef) => null;

    public virtual List<IndexSchema> GetTableIndexes(IDbConnection db, TableRef tableRef) => null;

    /// <summary>
    /// The indexes of the rows of a query with name, is_unique, is_constraint and columns, a comma-separated list of
    /// its key columns in order
    /// </summary>
    protected static List<IndexSchema> ToIndexSchemas(List<Dictionary<string, object>> rows) =>
        rows.Map(row => new IndexSchema
        {
            Name = RowValue(row, "name")?.ToString(),
            IsUnique = Convert.ToBoolean(RowValue(row, "is_unique") ?? false),
            IsConstraint = Convert.ToBoolean(RowValue(row, "is_constraint") ?? false),
            IsPrimaryKey = Convert.ToBoolean(RowValue(row, "is_primary_key") ?? false),
            Columns = RowList(row, "columns"),
            Include = RowList(row, "include"),
            Where = RowValue(row, "where_condition")?.ToString() is { Length: > 0 } where ? where : null,
        });

    public virtual List<CheckConstraintSchema> GetCheckConstraints(IDbConnection db, string quotedTable) => null;

    public virtual List<string> GetModelIndexConditions(IDbConnection db, List<FieldDefinition> fieldDefs,
        List<IndexSchema> indexes) => null;

    /// <summary>
    /// The WHERE conditions of indexes created on a temporary table with the columns of fieldDefs, as they're read by
    /// readConditions with the temporary table's quoted name, by index name
    /// </summary>
    protected List<string> ReadModelIndexConditions(IDbConnection db, List<FieldDefinition> fieldDefs,
        List<IndexSchema> indexes, Func<string, Dictionary<string, string>> readConditions)
    {
        var tempTable = this.GetBulkStagingTableName("ormlite_diff_" + Guid.NewGuid().ToString("N"));
        db.ExecuteSql(this.ToCreateTempTableStatement(tempTable, this.GetTempColumnDefinitions(fieldDefs).Join(",\n  ")));
        try
        {
            // Index names have to be unique in some databases, e.g. in PostgreSQL's schemas
            var prefix = "ix_" + Guid.NewGuid().ToString("N").Substring(0, 12) + "_";
            for (var i = 0; i < indexes.Count; i++)
            {
                db.ExecuteSql($"CREATE INDEX {this.GetQuotedName(prefix + i)} ON {tempTable} " +
                              $"({indexes[i].Columns.Map(this.GetQuotedName).Join(", ")}) WHERE {indexes[i].Where}");
            }
            var written = readConditions(tempTable);
            return indexes.Select((_, i) => written.TryGetValue(prefix + i, out var condition) ? condition : null).ToList();
        }
        finally
        {
            db.ExecuteSql(this.ToDropBulkStagingTableStatement(tempTable));
        }
    }

    // The columns of a temporary table that's created with the columns of a model, without the names of their default
    // constraints, which have to be unique in some databases
    protected List<string> GetTempColumnDefinitions(List<FieldDefinition> fieldDefs) =>
        fieldDefs.Map(x => this.GetColumnDefinition(x.DefaultValueConstraint == null
            ? x
            : x.Clone(f =>
            {
                f.ModelDef = x.ModelDef;
                f.DefaultValueConstraint = null;
            })));

    public virtual List<CheckConstraintSchema> GetModelCheckConstraints(IDbConnection db, List<FieldDefinition> fieldDefs,
        List<CheckConstraintSchema> checks)
    {
        var tempTable = this.GetBulkStagingTableName("ormlite_diff_" + Guid.NewGuid().ToString("N"));
        var columns = this.GetTempColumnDefinitions(fieldDefs);
        // Constraint names have to be unique in some databases, e.g. in SQL Server's tempdb
        var prefix = "chk_" + Guid.NewGuid().ToString("N").Substring(0, 12) + "_";
        for (var i = 0; i < checks.Count; i++)
        {
            columns.Add($"CONSTRAINT {this.GetQuotedName(prefix + i)} CHECK ({checks[i].Condition})");
        }
        db.ExecuteSql(this.ToCreateTempTableStatement(tempTable, columns.Join(",\n  ")));
        try
        {
            var created = this.GetCheckConstraints(db, tempTable);
            if (created == null)
            {
                return null;
            }

            return checks.Select((check, i) => new CheckConstraintSchema
            {
                Name = check.Name,
                Condition = created.FirstOrDefault(x => string.Equals(x.Name, prefix + i, StringComparison.OrdinalIgnoreCase))?.Condition,
            }).ToList();
        }
        finally
        {
            db.ExecuteSql(this.ToDropBulkStagingTableStatement(tempTable));
        }
    }

    /// <summary>
    /// The check constraints of the rows of a query with name and condition columns
    /// </summary>
    protected static List<CheckConstraintSchema> ToCheckConstraintSchemas(List<Dictionary<string, object>> rows) =>
        rows.Map(row => new CheckConstraintSchema
        {
            Name = RowValue(row, "name")?.ToString(),
            Condition = RowValue(row, "condition")?.ToString(),
        });

    public virtual Dictionary<string, string> GetColumnDefaults(IDbConnection db, string quotedTable) => null;

    public virtual List<ForeignKeySchema> GetTableForeignKeys(IDbConnection db, TableRef tableRef) => null;

    /// <summary>
    /// The default values of the rows of a query with name and value columns
    /// </summary>
    protected static Dictionary<string, string> ToColumnDefaults(List<Dictionary<string, object>> rows)
    {
        var to = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows)
        {
            var name = RowValue(row, "name")?.ToString();
            if (name != null)
            {
                to[name] = RowValue(row, "value")?.ToString();
            }
        }
        return to;
    }

    /// <summary>
    /// The foreign keys of the rows of a query with name, columns and ref_columns, comma-separated lists of their
    /// columns in order, ref_table, on_delete and on_update
    /// </summary>
    protected static List<ForeignKeySchema> ToForeignKeySchemas(List<Dictionary<string, object>> rows) =>
        rows.Map(row => new ForeignKeySchema
        {
            Name = RowValue(row, "name")?.ToString(),
            Columns = RowList(row, "columns"),
            RefTable = RowValue(row, "ref_table")?.ToString(),
            RefColumns = RowList(row, "ref_columns"),
            OnDelete = RowValue(row, "on_delete")?.ToString(),
            OnUpdate = RowValue(row, "on_update")?.ToString(),
        });

    private static object RowValue(Dictionary<string, object> row, string name)
    {
        foreach (var entry in row)
        {
            if (string.Equals(entry.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                return entry.Value is DBNull ? null : entry.Value;
            }
        }
        return null;
    }

    private static List<string> RowList(Dictionary<string, object> row, string name) =>
        (RowValue(row, name)?.ToString() ?? "").Split([','], StringSplitOptions.RemoveEmptyEntries).ToList();

    /// <summary>
    /// Converts to alter column statement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public virtual string ToAddColumnStatement(TableRef tableRef, FieldDefinition fieldDef)
    {
        return $"ALTER TABLE {this.QuoteTable(tableRef)} ADD COLUMN {this.GetColumnDefinition(fieldDef)};";
    }

    /// <summary>
    /// Converts to altercolumnstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <returns>System.String.</returns>
    public virtual string ToAlterColumnStatement(TableRef tableRef, FieldDefinition fieldDef)
    {
        return
            $"ALTER TABLE {this.QuoteTable(tableRef)} MODIFY COLUMN {this.GetColumnDefinition(fieldDef)};";
    }

    /// <summary>
    /// Converts to changecolumnnamestatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="oldColumn">The old column.</param>
    /// <returns>System.String.</returns>
    public virtual string ToChangeColumnNameStatement(TableRef tableRef, FieldDefinition fieldDef, string oldColumn)
    {
        return
            $"ALTER TABLE {this.QuoteTable(tableRef)} CHANGE COLUMN {this.GetQuotedColumnName(oldColumn)} {this.GetColumnDefinition(fieldDef)};";
    }

    /// <summary>
    /// Converts to renamecolumnstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="oldColumn">The old column.</param>
    /// <param name="newColumn">The new column.</param>
    /// <returns>System.String.</returns>
    public virtual string ToRenameColumnStatement(TableRef tableRef, string oldColumn, string newColumn)
    {
        return
            $"ALTER TABLE {this.QuoteTable(tableRef)} RENAME COLUMN {this.GetQuotedColumnName(oldColumn)} TO {this.GetQuotedColumnName(newColumn)};";
    }

    /// <summary>
    /// Converts to add foreign key statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TForeign">The type of the t foreign.</typeparam>
    /// <param name="field">The field.</param>
    /// <param name="foreignField">The foreign field.</param>
    /// <param name="onUpdate">The on update.</param>
    /// <param name="onDelete">The on delete.</param>
    /// <param name="foreignKeyName">Name of the foreign key.</param>
    /// <returns>System.String.</returns>
    public virtual string ToAddForeignKeyStatement<T, TForeign>(
        Expression<Func<T, object>> field,
        Expression<Func<TForeign, object>> foreignField,
        OnFkOption onUpdate,
        OnFkOption onDelete,
        string foreignKeyName = null)
    {
        var sourceMD = ModelDefinition<T>.Definition;
        var fieldDef = sourceMD.GetFieldDefinition(field);

        var referenceMD = ModelDefinition<TForeign>.Definition;
        var referenceFieldDef = referenceMD.GetFieldDefinition(foreignField);

        var name = this.GetQuotedName(
            foreignKeyName.IsNullOrEmpty()
                ? "fk_" + sourceMD.ModelName + "_" + fieldDef.FieldName + "_" + referenceFieldDef.FieldName
                : foreignKeyName);

        return $"ALTER TABLE {this.GetQuotedTableName(sourceMD)} " +
               $"ADD CONSTRAINT {name} FOREIGN KEY ({this.GetQuotedColumnName(fieldDef)}) " +
               $"REFERENCES {this.GetQuotedTableName(referenceMD)} " +
               $"({this.GetQuotedColumnName(referenceFieldDef)})" +
               $" {this.GetForeignKeyOnDeleteClause(new ForeignKeyConstraint(typeof(T), onDelete: this.FkOptionToString(onDelete)))}" +
               $" {this.GetForeignKeyOnUpdateClause(new ForeignKeyConstraint(typeof(T), onUpdate: this.FkOptionToString(onUpdate)))};";
    }

    /// <summary>
    /// Converts to dropforeignkeystatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="foreignKeyName">Name of the foreign key.</param>
    /// <returns>string.</returns>
    public virtual string ToDropForeignKeyStatement(TableRef tableRef, string foreignKeyName)
    {
        return $"ALTER TABLE {this.QuoteTable(tableRef)} DROP CONSTRAINT {this.GetQuotedName(foreignKeyName)};";
    }

    public virtual string ToAddForeignKeyStatement(TableRef tableRef, FieldDefinition fieldDef)
    {
        var refModelDef = fieldDef.ForeignKey.ReferenceType.GetModelDefinition();
        var name = fieldDef.ForeignKey.GetForeignKeyName(fieldDef.ModelDef ?? tableRef.ModelDef, refModelDef, this.NamingStrategy, fieldDef);
        return $"ALTER TABLE {this.QuoteTable(tableRef)} " +
               $"ADD CONSTRAINT {this.GetQuotedName(name)} FOREIGN KEY ({this.GetQuotedColumnName(fieldDef)}) " +
               $"REFERENCES {this.GetQuotedTableName(refModelDef)} ({this.GetQuotedColumnName(refModelDef.PrimaryKey)})" +
               $"{this.GetForeignKeyOnDeleteClause(fieldDef.ForeignKey)}{this.GetForeignKeyOnUpdateClause(fieldDef.ForeignKey)};";
    }

    public virtual string GetFullTextIndexName(ModelDefinition modelDef) =>
        modelDef.FullTextIndex?.Name ?? this.NamingStrategy.ApplyNameRestrictions("ftx_" + this.GetTableNameOnly(new TableRef(modelDef)));

    protected NotSupportedException FullTextNotSupported() =>
        new($"{this.GetType().Name} doesn't support full-text indexes");

    public virtual List<string> ToCreateFullTextIndexStatements(ModelDefinition modelDef) => throw this.FullTextNotSupported();

    public virtual List<string> ToDropFullTextIndexStatements(ModelDefinition modelDef) => throw this.FullTextNotSupported();

    public virtual bool SupportsFullTextSearch(IDbConnection db) => false;

    public virtual bool HasFullTextIndex(IDbConnection db, ModelDefinition modelDef) => false;

    public virtual bool IsFullTextIndexUpToDate(IDbConnection db, ModelDefinition modelDef) => true;

    public virtual bool FullTextMatchesEachTerm => false;

    public virtual List<string> ToFullTextSearch(List<FullTextTerm> terms) => throw this.FullTextNotSupported();

    public virtual string ToFullTextMatch(FullTextColumns columns, List<string> parameters) => throw this.FullTextNotSupported();

    public virtual string ToFullTextRank(FullTextColumns columns, List<string> parameters) => throw this.FullTextNotSupported();

    // The quoted columns of a [FullTextIndex] without their table, e.g. to create its index
    protected List<string> GetFullTextColumnNames(ModelDefinition modelDef) =>
        modelDef.GetFullTextFields().Map(this.GetQuotedColumnName);

    public virtual string ToAddConstraintStatement(TableRef tableRef, string constraint) =>
        $"ALTER TABLE {this.QuoteTable(tableRef)} ADD {constraint};";

    public virtual string ToDropUniqueConstraintStatement(TableRef tableRef, string constraintName) =>
        $"ALTER TABLE {this.QuoteTable(tableRef)} DROP CONSTRAINT {this.GetQuotedName(constraintName)};";

    public virtual string ToDropCheckConstraintStatement(TableRef tableRef, string constraintName) =>
        $"ALTER TABLE {this.QuoteTable(tableRef)} DROP CONSTRAINT {this.GetQuotedName(constraintName)};";

    public virtual string ToAlterColumnDefaultStatement(TableRef tableRef, FieldDefinition fieldDef)
    {
        var defaultValue = this.GetDefaultValue(fieldDef);
        return $"ALTER TABLE {this.QuoteTable(tableRef)} ALTER COLUMN {this.GetQuotedColumnName(fieldDef)} " +
               (string.IsNullOrEmpty(defaultValue) ? "DROP DEFAULT;" : $"SET DEFAULT {defaultValue};");
    }

    /// <summary>
    /// Converts to dropconstraintstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="constraintName">Name of the constraint.</param>
    /// <returns>System.String.</returns>
    public virtual string ToDropConstraintStatement(TableRef tableRef, string constraintName)
    {
        return null;
    }

    /// <summary>
    /// Converts to create index statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field">The field.</param>
    /// <param name="indexName">Name of the index.</param>
    /// <param name="unique">if set to <c>true</c> [unique].</param>
    /// <returns>System.String.</returns>
    public virtual string ToCreateIndexStatement<T>(
        Expression<Func<T, object>> field,
        string indexName = null,
        bool unique = false)
    {
        var sourceDef = ModelDefinition<T>.Definition;
        var fieldDef = sourceDef.GetFieldDefinition(field);

        var name = this.GetQuotedName(
            indexName.IsNullOrEmpty()
                ? (unique ? "uidx" : "idx") + "_" + sourceDef.ModelName + "_" + fieldDef.FieldName
                : indexName);

        var command = $"CREATE {(unique ? "UNIQUE " : string.Empty)}" +
                      $"INDEX {name} ON {this.GetQuotedTableName(sourceDef)}" +
                      $"({this.GetQuotedColumnName(fieldDef)});";
        return command;
    }

    /// <summary>
    /// Create Drop Index statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexName">Name of the index.</param>
    /// <returns>System.String.</returns>
    public virtual string ToDropIndexStatement<T>(string indexName)
    {
        return $"DROP INDEX IF EXISTS {this.GetQuotedName(indexName)};";
    }

    /// <summary>
    /// Fks the option to string.
    /// </summary>
    /// <param name="option">The option.</param>
    /// <returns>System.String.</returns>
    protected virtual string FkOptionToString(OnFkOption option)
    {
        return option switch
            {
                OnFkOption.Cascade => "CASCADE",
                OnFkOption.NoAction => "NO ACTION",
                OnFkOption.SetNull => "SET NULL",
                OnFkOption.SetDefault => "SET DEFAULT",
                _ => "RESTRICT"
            };
    }

    /// <summary>
    /// Gets the quoted value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns>System.String.</returns>
    public virtual string GetQuotedValue(object value, Type fieldType)
    {
        if (value == null || value == DBNull.Value)
        {
            return "NULL";
        }

        var converter = value.GetType().IsEnum ? this.EnumConverter : this.GetConverterBestMatch(fieldType);

        try
        {
            return converter.ToQuotedString(fieldType, value);
        }
        catch (Exception ex)
        {
            Log.Error(
                $"Error in {converter.GetType().Name}.ToQuotedString() value '{converter.GetType().Name}' and Type '{value.GetType().Name}'",
                ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the parameter value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns>System.Object.</returns>
    public virtual object GetParamValue(object value, Type fieldType)
    {
        return this.ToDbValue(value, fieldType);
    }

    /// <summary>
    /// Initializes the query parameter.
    /// </summary>
    /// <param name="param">The parameter.</param>
    public virtual void InitQueryParam(IDbDataParameter param)
    {
    }

    /// <summary>
    /// Initializes the update parameter.
    /// </summary>
    /// <param name="param">The parameter.</param>
    public virtual void InitUpdateParam(IDbDataParameter param)
    {
    }

    /// <summary>
    /// Escapes the wildcards.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public virtual string EscapeWildcards(string value)
    {
        return value?.Replace("^", @"^^").Replace(@"\", @"^\").Replace("_", @"^_").Replace("%", @"^%");
    }

    /// <summary>
    /// Gets the load children sub select.
    /// </summary>
    /// <typeparam name="From">The type of from.</typeparam>
    /// <param name="expr">The expr.</param>
    /// <returns>System.String.</returns>
    public virtual string GetLoadChildrenSubSelect<From>(SqlExpression<From> expr)
    {
        var modelDef = expr.ModelDef;
        expr.UnsafeSelect(this.GetQuotedColumnName(modelDef, modelDef.PrimaryKey));

        var subSql = expr.ToSelectStatement(QueryType.Select);

        return subSql;
    }

    /// <summary>
    /// Converts to rowcountstatement.
    /// </summary>
    /// <param name="innerSql">The inner SQL.</param>
    /// <returns>System.String.</returns>
    public virtual string ToRowCountStatement(string innerSql)
    {
        return $"SELECT COUNT(*) FROM ({innerSql}) AS COUNT";
    }

    /// <summary>
    /// Drops the column.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <param name="column">The column.</param>
    /// <returns>System.String.</returns>
    public virtual string ToDropColumnStatement(TableRef tableRef, string column)
    {
        return $"ALTER TABLE {this.QuoteTable(tableRef)} DROP COLUMN {this.GetQuotedColumnName(column)};";
    }

    /// <summary>
    /// Converts to tablenamesstatement.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public virtual string ToTableNamesStatement(string schema)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Return table, row count SQL for listing all tables with their row counts
    /// </summary>
    /// <param name="live">If true returns live current row counts of each table (slower), otherwise returns cached row counts from RDBMS table stats</param>
    /// <param name="schema">The table schema if any</param>
    /// <returns>System.String.</returns>
    public virtual string ToTableNamesWithRowCountsStatement(bool live, string schema)
    {
        return null;
        // returning null Fallsback to slow UNION N+1 COUNT(*) op
    }

    /// <summary>
    /// SQLs the conflict.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="conflictResolution">The conflict resolution.</param>
    /// <returns>System.String.</returns>
    public virtual string SqlConflict(string sql, string conflictResolution)
    {
        return sql;
        // NOOP
    }

    /// <summary>
    /// SQLs the concat.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>System.String.</returns>
    public virtual string SqlConcat(IEnumerable<object> args)
    {
        return $"CONCAT({string.Join(", ", args)})";
    }

    /// <summary>
    /// SQLs the currency.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <returns>System.String.</returns>
    public virtual string SqlCurrency(string fieldOrValue)
    {
        return this.SqlCurrency(fieldOrValue, "$");
    }

    /// <summary>
    /// SQLs the currency.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="currencySymbol">The currency symbol.</param>
    /// <returns>System.String.</returns>
    public virtual string SqlCurrency(string fieldOrValue, string currencySymbol)
    {
        return this.SqlConcat(new List<string> { currencySymbol, fieldOrValue });
    }

    /// <summary>
    /// SQLs the bool.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    /// <returns>System.String.</returns>
    public virtual string SqlBool(bool value)
    {
        return value ? "true" : "false";
    }

    /// <summary>
    /// SQLs the limit.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="rows">The rows.</param>
    /// <returns>System.String.</returns>
    public virtual string SqlLimit(int? offset = null, int? rows = null)
    {
        return rows == null && offset == null ? string.Empty :
            offset == null ? "LIMIT " + rows : "LIMIT " + rows.GetValueOrDefault(int.MaxValue) + " OFFSET " + offset;
    }

    /// <summary>
    /// SQLs the cast.
    /// </summary>
    /// <param name="fieldOrValue">The field or value.</param>
    /// <param name="castAs">The cast as.</param>
    /// <returns>System.String.</returns>
    public virtual string SqlCast(object fieldOrValue, string castAs)
    {
        return $"CAST({fieldOrValue} AS {castAs})";
    }

    /// <summary>
    /// Gets the SQL random.
    /// </summary>
    /// <value>The SQL random.</value>
    public virtual string SqlRandom => "RAND()";

    public virtual string SqlDateFormat(string quotedColumn, string format) => $"strftime({GetQuotedValue(format)},{quotedColumn})";
    public virtual string SqlChar(int charCode) => $"CHAR({charCode})";

    // Async API's, should be overriden by Dialect Providers to use .ConfigureAwait(false)
    // Default impl below uses TaskAwaiter shim in async.cs

    /// <summary>
    /// Gets a value indicating whether [supports asynchronous].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [supports asynchronous]; otherwise, <c>false</c>.</value>
    public virtual bool SupportsAsync => false;


    /// <summary>
    /// Opens the asynchronous.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public virtual Task OpenAsync(IDbConnection db, CancellationToken token = default)
    {
        db.Open();
        return TaskResult.Finished;
    }

    /// <summary>
    /// Executes the reader asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;IDataReader&gt;.</returns>
    public virtual Task<IDataReader> ExecuteReaderAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return cmd.ExecuteReader().InTask();
    }

    /// <summary>
    /// Executes the non query asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    public virtual Task<int> ExecuteNonQueryAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return cmd.ExecuteNonQuery().InTask();
    }

    /// <summary>
    /// Executes the scalar asynchronous.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Object&gt;.</returns>
    public virtual Task<object> ExecuteScalarAsync(IDbCommand cmd, CancellationToken token = default)
    {
        return cmd.ExecuteScalar().InTask();
    }

    /// <summary>
    /// Reads the asynchronous.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public virtual Task<bool> ReadAsync(IDataReader reader, CancellationToken token = default)
    {
        return reader.Read().InTask();
    }

    /// <summary>
    /// Readers the each.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="fn">The function.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
    public async virtual Task<List<T>> ReaderEach<T>(
        IDataReader reader,
        Func<T> fn,
        CancellationToken token = default)
    {
        try
        {
            var to = new List<T>();
            while (await this.ReadAsync(reader, token))
            {
                var row = fn();
                to.Add(row);
            }

            return to;
        }
        finally
        {
            reader.Dispose();
        }
    }

    /// <summary>
    /// Readers the each.
    /// </summary>
    /// <typeparam name="Return">The type of the return.</typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="fn">The function.</param>
    /// <param name="source">The source.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;Return&gt;.</returns>
    public async virtual Task<Return> ReaderEach<Return>(
        IDataReader reader,
        Action fn,
        Return source,
        CancellationToken token = default)
    {
        try
        {
            while (await this.ReadAsync(reader, token))
            {
                fn();
            }

            return source;
        }
        finally
        {
            reader.Dispose();
        }
    }

    /// <summary>
    /// Readers the read.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader">The reader.</param>
    /// <param name="fn">The function.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    public async virtual Task<T> ReaderRead<T>(IDataReader reader, Func<T> fn, CancellationToken token = default)
    {
        try
        {
            if (await this.ReadAsync(reader, token))
            {
                return fn();
            }

            return default;
        }
        finally
        {
            reader.Dispose();
        }
    }

    /// <summary>
    /// Inserts the and get last insert identifier asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int64&gt;.</returns>
    public virtual Task<long> InsertAndGetLastInsertIdAsync<T>(IDbCommand dbCmd, CancellationToken token)
    {
        if (this.SelectIdentitySql == null)
        {
            return new NotImplementedException(
                "Returning last inserted identity is not implemented on this DB Provider.").InTask<long>();
        }

        dbCmd.CommandText += "; " + this.SelectIdentitySql;

        return dbCmd.ExecLongScalarAsync(null, token);
    }

    /// <summary>
    /// Gets the UTC date function.
    /// </summary>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string GetUtcDateFunction()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Dates the difference function.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="date1">The date1.</param>
    /// <param name="date2">The date2.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string DateDiffFunction(string interval, string date1, string date2)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Determines whether [is null function] [the specified expression].
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="alternateValue">The alternate value.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string IsNullFunction(string expression, object alternateValue)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Converts the flag.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string ConvertFlag(string expression)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Databases the fragmentation information.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string DatabaseFragmentationInfo(string database)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Databases the size.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string DatabaseSize(string database)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// SQLs the version.
    /// </summary>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string SQLVersion()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// SQLs the name of the server.
    /// </summary>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string SQLServerName()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Shrinks the database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string ShrinkDatabase(string database)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Res the index database.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="objectQualifier">The object qualifier.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string ReIndexDatabase(string database, string objectQualifier)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Changes the recovery mode.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="mode">The mode.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string ChangeRecoveryMode(string database, string mode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Just runs the SQL command according to specifications.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>Returns the Results</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual string InnerRunSqlExecuteReader(IDbCommand command)
    {
        throw new NotImplementedException();
    }
}
