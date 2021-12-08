// ***********************************************************************
// <copyright file="OrmLiteDialectProviderBase.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite
{
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
    using ServiceStack.Script;
    using ServiceStack.Text;

    /// <summary>
    /// Class OrmLiteDialectProviderBase.
    /// Implements the <see cref="ServiceStack.OrmLite.IOrmLiteDialectProvider" />
    /// </summary>
    /// <typeparam name="TDialect">The type of the t dialect.</typeparam>
    /// <seealso cref="ServiceStack.OrmLite.IOrmLiteDialectProvider" />
    public abstract class OrmLiteDialectProviderBase<TDialect>
        : IOrmLiteDialectProvider
        where TDialect : IOrmLiteDialectProvider
    {
        /// <summary>
        /// The log
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(IOrmLiteDialectProvider));

        /// <summary>
        /// Initializes a new instance of the <see cref="OrmLiteDialectProviderBase{TDialect}"/> class.
        /// </summary>
        protected OrmLiteDialectProviderBase()
        {
            this.Variables = new Dictionary<string, string>();
            this.StringSerializer = new JsvStringSerializer();
        }

        #region ADO.NET supported types

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
        #endregion

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

#if NET6_0
            RegisterConverter<DateOnly>(new DateOnlyConverter());
            RegisterConverter<TimeOnly>(new TimeOnlyConverter());
#endif
        }

        /// <summary>
        /// Gets the column type definition.
        /// </summary>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="fieldLength">Length of the field.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public string GetColumnTypeDefinition(Type columnType, int? fieldLength, int? scale)
        {
            var converter = this.GetConverter(columnType);
            if (converter != null)
            {
                if (converter is IHasColumnDefinitionPrecision customPrecisionConverter)
                    return customPrecisionConverter.GetColumnDefinition(fieldLength, scale);

                if (converter is IHasColumnDefinitionLength customLengthConverter)
                    return customLengthConverter.GetColumnDefinition(fieldLength);

                if (string.IsNullOrEmpty(converter.ColumnDefinition))
                    throw new ArgumentException($"{converter.GetType().Name} requires a ColumnDefinition");

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
        public Dictionary<string, string> Variables { get; set; }

        /// <summary>
        /// Gets or sets the execute filter.
        /// </summary>
        /// <value>The execute filter.</value>
        public IOrmLiteExecFilter ExecFilter { get; set; }

        /// <summary>
        /// The converters
        /// </summary>
        public Dictionary<Type, IOrmLiteConverter> Converters = new();

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
        public IStringSerializer StringSerializer { get; set; }

        /// <summary>
        /// The parameter name filter
        /// </summary>
        private Func<string, string> paramNameFilter;

        /// <summary>
        /// Gets or sets the parameter name filter.
        /// </summary>
        /// <value>The parameter name filter.</value>
        public Func<string, string> ParamNameFilter
        {
            get => this.paramNameFilter ?? OrmLiteConfig.ParamNameFilter;
            set => this.paramNameFilter = value;
        }

        /// <summary>
        /// The default value format
        /// </summary>
        public string DefaultValueFormat = " DEFAULT ({0})";

        /// <summary>
        /// The enum converter
        /// </summary>
        private EnumConverter enumConverter;

        /// <summary>
        /// Gets or sets the enum converter.
        /// </summary>
        /// <value>The enum converter.</value>
        public EnumConverter EnumConverter
        {
            get => this.enumConverter;
            set
            {
                value.DialectProvider = this;
                this.enumConverter = value;
            }
        }

        /// <summary>
        /// The row version converter
        /// </summary>
        private RowVersionConverter rowVersionConverter;

        /// <summary>
        /// Gets or sets the row version converter.
        /// </summary>
        /// <value>The row version converter.</value>
        public RowVersionConverter RowVersionConverter
        {
            get => this.rowVersionConverter;
            set
            {
                value.DialectProvider = this;
                this.rowVersionConverter = value;
            }
        }

        /// <summary>
        /// The reference type converter
        /// </summary>
        private ReferenceTypeConverter referenceTypeConverter;

        /// <summary>
        /// Gets or sets the reference type converter.
        /// </summary>
        /// <value>The reference type converter.</value>
        public ReferenceTypeConverter ReferenceTypeConverter
        {
            get => this.referenceTypeConverter;
            set
            {
                value.DialectProvider = this;
                this.referenceTypeConverter = value;
            }
        }

        /// <summary>
        /// The value type converter
        /// </summary>
        private ValueTypeConverter valueTypeConverter;

        /// <summary>
        /// Gets or sets the value type converter.
        /// </summary>
        /// <value>The value type converter.</value>
        public ValueTypeConverter ValueTypeConverter
        {
            get => this.valueTypeConverter;
            set
            {
                value.DialectProvider = this;
                this.valueTypeConverter = value;
            }
        }

        /// <summary>
        /// Removes the converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveConverter<T>()
        {
            if (this.Converters.TryRemove(typeof(T), out var converter))
                converter.DialectProvider = null;
        }

        /// <summary>
        /// Registers the converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter">The converter.</param>
        /// <exception cref="System.ArgumentNullException">converter</exception>
        public void RegisterConverter<T>(IOrmLiteConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

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
            return this.Converters.TryGetValue(type, out IOrmLiteConverter converter) ? converter : null;
        }

        /// <summary>
        /// Shoulds the quote value.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool ShouldQuoteValue(Type fieldType)
        {
            var converter = this.GetConverter(fieldType);
            return converter == null || converter is NativeValueOrmLiteConverter;
        }

        /// <summary>
        /// Froms the database row version.
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
                return this.RowVersionConverter;

            var converter = this.GetConverter(type);
            if (converter != null)
                return converter;

            if (type.IsEnum)
                return this.EnumConverter;

            return type.IsRefType() ? (IOrmLiteConverter)this.ReferenceTypeConverter : this.ValueTypeConverter;
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
                return this.RowVersionConverter;

            if (this.Converters.TryGetValue(fieldType, out var converter))
                return converter;

            if (fieldType.IsEnum)
                return this.EnumConverter;

            return fieldType.IsRefType() ? (IOrmLiteConverter)this.ReferenceTypeConverter : this.ValueTypeConverter;
        }

        /// <summary>
        /// Converts to dbvalue.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public virtual object ToDbValue(object value, Type type)
        {
            if (value == null || value is DBNull)
                return null;

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
            if (value == null || value is DBNull)
                return null;

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
            if (this.Converters.TryGetValue(type, out var converter))
                return converter.GetValue(reader, columnIndex, null);

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
        /// Quote the string so that it can be used inside an SQL-expression
        /// Escape quotes inside the string
        /// </summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <returns>System.String.</returns>
        public virtual string GetQuotedValue(string paramValue)
        {
            return "'" + paramValue.Replace("'", "''") + "'";
        }

        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns>System.String.</returns>
        public virtual string GetSchemaName(string schema)
        {
            return this.NamingStrategy.GetSchemaName(schema);
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns>System.String.</returns>
        public virtual string GetTableName(ModelDefinition modelDef) =>
            this.GetTableName(modelDef.ModelName, modelDef.Schema, useStrategy: true);

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="useStrategy">if set to <c>true</c> [use strategy].</param>
        /// <returns>System.String.</returns>
        public virtual string GetTableName(ModelDefinition modelDef, bool useStrategy) =>
            this.GetTableName(modelDef.ModelName, modelDef.Schema, useStrategy);

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns>System.String.</returns>
        public virtual string GetTableName(string table, string schema = null) =>
            this.GetTableName(table, schema, useStrategy: true);

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="useStrategy">if set to <c>true</c> [use strategy].</param>
        /// <returns>System.String.</returns>
        public virtual string GetTableName(string table, string schema, bool useStrategy)
        {
            if (useStrategy)
            {
                return schema != null
                    ? $"{this.QuoteIfRequired(this.NamingStrategy.GetSchemaName(schema))}.{this.QuoteIfRequired(this.NamingStrategy.GetTableName(table))}"
                    : this.QuoteIfRequired(this.NamingStrategy.GetTableName(table));
            }

            return schema != null
                ? $"{this.QuoteIfRequired(schema)}.{this.QuoteIfRequired(table)}"
                : this.QuoteIfRequired(table);
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
        /// <param name="modelDef">The model definition.</param>
        /// <returns>System.String.</returns>
        public virtual string GetQuotedTableName(ModelDefinition modelDef)
        {
            return this.GetQuotedTableName(modelDef.ModelName, modelDef.Schema);
        }

        /// <summary>
        /// Gets the name of the quoted table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns>System.String.</returns>
        public virtual string GetQuotedTableName(string tableName, string schema = null)
        {
            /*if (schema == null)
            {
                return this.GetQuotedName(this.NamingStrategy.GetTableName(tableName));
            }*/

            var escapedSchema = this.NamingStrategy.GetSchemaName(schema).Replace(".", "\".\"");

            return
                $"{this.GetQuotedName(escapedSchema)}.{this.GetQuotedName(this.NamingStrategy.GetTableName(tableName))}";
        }

        /// <summary>
        /// Gets the name of the quoted table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="useStrategy">if set to <c>true</c> [use strategy].</param>
        /// <returns>System.String.</returns>
        public virtual string GetQuotedTableName(string tableName, string schema, bool useStrategy) =>
            this.GetQuotedName(this.GetTableName(tableName, schema, useStrategy));

        /// <summary>
        /// Gets the name of the quoted column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        public virtual string GetQuotedColumnName(string columnName)
        {
            return this.GetQuotedName(this.NamingStrategy.GetColumnName(columnName));
        }

        /// <summary>
        /// Shoulds the quote.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool ShouldQuote(string name) =>
            !string.IsNullOrEmpty(name) && (name.IndexOf(' ') >= 0 || name.IndexOf('.') >= 0);

        /// <summary>
        /// Quotes if required.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public virtual string QuoteIfRequired(string name)
        {
            return this.ShouldQuote(name) ? this.GetQuotedName(name) : name;
        }

        /// <summary>
        /// Gets the name of the quoted.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public virtual string GetQuotedName(string name) =>
            name == null ? null : name.FirstCharEquals('"') ? name : '"' + name + '"';

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
                : this.GetQuotedName(name);
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
        /// Gets the column definition.
        /// </summary>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public virtual string GetColumnDefinition(FieldDefinition fieldDef)
        {
            var fieldDefinition = ResolveFragment(fieldDef.CustomFieldDefinition) ??
                                  GetColumnTypeDefinition(fieldDef.ColumnType, fieldDef.FieldLength, fieldDef.Scale);

            var sql = StringBuilderCache.Allocate();
            sql.Append($"{GetQuotedColumnName(fieldDef.FieldName)} {fieldDefinition}");

            if (fieldDef.IsPrimaryKey)
            {
                sql.Append(" PRIMARY KEY");
                if (fieldDef.AutoIncrement)
                {
                    sql.Append(" ").Append(AutoIncrementDefinition);
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
            var fieldDefinition = ResolveFragment(fieldDef.CustomFieldDefinition) ??
                                  GetColumnTypeDefinition(fieldDef.ColumnType, fieldDef.FieldLength, fieldDef.Scale);

            var sql = StringBuilderCache.Allocate();
            sql.Append($"{GetQuotedColumnName(fieldDef.FieldName)} {fieldDefinition}");

            // Check for Composite PrimaryKey First
            if (modelDef.CompositePrimaryKeys.Any())
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
                        sql.Append(" ").Append(AutoIncrementDefinition);
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
        /// <exception cref="System.NotImplementedException">Returning last inserted identity is not implemented on this DB Provider.</exception>
        public virtual long GetLastInsertId(IDbCommand dbCmd)
        {
            if (this.SelectIdentitySql == null)
                throw new NotImplementedException(
                    "Returning last inserted identity is not implemented on this DB Provider.");

            dbCmd.CommandText = this.SelectIdentitySql;
            return dbCmd.ExecLongScalar();
        }

        /// <summary>
        /// Gets the last insert identifier SQL suffix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException">Returning last inserted identity is not implemented on this DB Provider.</exception>
        public virtual string GetLastInsertIdSqlSuffix<T>()
        {
            if (this.SelectIdentitySql == null)
                throw new NotImplementedException(
                    "Returning last inserted identity is not implemented on this DB Provider.");

            return "; " + this.SelectIdentitySql;
        }

        /// <summary>
        /// Determines whether [is full select statement] [the specified SQL].
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns><c>true</c> if [is full select statement] [the specified SQL]; otherwise, <c>false</c>.</returns>
        public virtual bool IsFullSelectStatement(string sql) =>
            !string.IsNullOrEmpty(sql) && sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase);

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
                return sqlFilter.SqlFmt(this, filterParams);

            var modelDef = tableType.GetModelDefinition();
            var sql = StringBuilderCache.Allocate();
            sql.Append($"SELECT {this.GetColumnNames(modelDef)} FROM {this.GetQuotedTableName(modelDef)}");

            if (string.IsNullOrEmpty(sqlFilter))
                return StringBuilderCache.ReturnAndFree(sql);

            sqlFilter = sqlFilter.SqlFmt(this, filterParams);
            if (!sqlFilter.StartsWith("ORDER ", StringComparison.OrdinalIgnoreCase) &&
                !sqlFilter.StartsWith("LIMIT ", StringComparison.OrdinalIgnoreCase))
            {
                sql.Append(" WHERE ");
            }

            sql.Append(sqlFilter);

            return StringBuilderCache.ReturnAndFree(sql);
        }

        protected virtual void ApplyTags(StringBuilder sqlBuilder, ISet<string> tags)
        {
            if (tags != null && tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    sqlBuilder.AppendLine(GenerateComment(tag));
                }
                sqlBuilder.Append("\n");
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

            ApplyTags(sb, tags);

            sb.Append(selectExpression);
            sb.Append(bodyExpression);

            if (!string.IsNullOrEmpty(orderByExpression))
            {
                sb.Append(orderByExpression);
            }

            if ((queryType == QueryType.Select || (rows == 1 && offset is null or 0)) && (offset != null || rows != null))
            {
                sb.Append("\n");
                sb.Append(this.SqlLimit(offset, rows));
            }

            return StringBuilderCache.ReturnAndFree(sb);
        }

        public virtual string GenerateComment(in string text)
        {
            return $"-- {text}";
        }

        /// <summary>
        /// Gets the row version select column.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="tablePrefix">The table prefix.</param>
        /// <returns>SelectItem.</returns>
        public virtual SelectItem GetRowVersionSelectColumn(FieldDefinition field, string tablePrefix = null)
        {
            return new SelectItemColumn(this, field.FieldName, null, tablePrefix);
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
            var quotedPrefix = tablePrefix != null
                ? this.GetQuotedTableName(tablePrefix, modelDef.Schema)
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
                else
                {
                    sqlColumns[i] = new SelectItemColumn(this, field.FieldName, null, quotedPrefix);
                }
            }

            return sqlColumns;
        }

        /// <summary>
        /// Shoulds the skip insert.
        /// </summary>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldSkipInsert(FieldDefinition fieldDef) => fieldDef.ShouldSkipInsert();

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
            ICollection<string> insertFields)
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
                    continue;

                if (sbColumnNames.Length > 0)
                    sbColumnNames.Append(",");
                if (sbColumnValues.Length > 0)
                    sbColumnValues.Append(",");

                try
                {
                    sbColumnNames.Append(this.GetQuotedColumnName(fieldDef.FieldName));
                    sbColumnValues.Append(this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName)));

                    AddParameter(cmd, fieldDef);
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
                return null;

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
                return null;
            if (fieldDef.FieldType == typeof(Guid))
                return Guid.NewGuid();
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
                    continue;

                if (sbColumnNames.Length > 0)
                    sbColumnNames.Append(",");
                if (sbColumnValues.Length > 0)
                    sbColumnValues.Append(",");

                try
                {
                    sbColumnNames.Append(this.GetQuotedColumnName(fieldDef.FieldName));
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
                    continue;

                var value = entry.Value;

                if (sbColumnNames.Length > 0)
                    sbColumnNames.Append(",");
                if (sbColumnValues.Length > 0)
                    sbColumnValues.Append(",");

                try
                {
                    sbColumnNames.Append(this.GetQuotedColumnName(fieldDef.FieldName));
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
            dialectProvider.PrepareParameterizedUpdateStatement<T>(dbCmd);

            if (string.IsNullOrEmpty(dbCmd.CommandText))
                return null;

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
            for (int i = 0; i < dbParams.Count; i++)
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
                sql = Regex.Replace(sql, pattern, replacement);
            }

            return sql;
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
                    continue;

                try
                {
                    if ((fieldDef.IsPrimaryKey || fieldDef.IsRowVersion) && updateAllFields)
                    {
                        if (sqlFilter.Length > 0)
                            sqlFilter.Append(" AND ");

                        this.AppendFieldCondition(sqlFilter, fieldDef, cmd);

                        if (fieldDef.IsRowVersion)
                            hadRowVersion = true;

                        continue;
                    }

                    if (!updateAllFields && !updateFields.Contains(fieldDef.Name, StringComparer.OrdinalIgnoreCase))
                        continue;

                    if (sql.Length > 0)
                        sql.Append(", ");

                    sql.Append(this.GetQuotedColumnName(fieldDef.FieldName)).Append("=").Append(
                        this.GetParam(this.SanitizeFieldNameForParamName(fieldDef.FieldName), fieldDef.CustomUpdate));

                    this.AddParameter(cmd, fieldDef);
                }
                catch (Exception ex)
                {
                    OrmLiteUtils.HandleException(ex, "ERROR in PrepareParameterizedUpdateStatement(): " + ex.Message);
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
            sqlFilter.Append(this.GetQuotedColumnName(fieldDef.FieldName)).Append(" IS NULL");
        }

        /// <summary>
        /// Appends the field condition.
        /// </summary>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <param name="cmd">The command.</param>
        public virtual void AppendFieldCondition(StringBuilder sqlFilter, FieldDefinition fieldDef, IDbCommand cmd)
        {
            sqlFilter.Append(this.GetQuotedColumnName(fieldDef.FieldName)).Append("=").Append(
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
        /// <exception cref="System.ArgumentException">DELETE's must have at least 1 criteria</exception>
        public virtual bool PrepareParameterizedDeleteStatement<T>(
            IDbCommand cmd,
            IDictionary<string, object> deleteFieldValues)
        {
            if (deleteFieldValues == null || deleteFieldValues.Count == 0)
                throw new ArgumentException("DELETE's must have at least 1 criteria");

            var sqlFilter = StringBuilderCache.Allocate();
            var modelDef = typeof(T).GetModelDefinition();
            var hadRowVersion = false;

            cmd.Parameters.Clear();

            foreach (var fieldDef in modelDef.FieldDefinitions)
            {
                if (fieldDef.ShouldSkipDelete())
                    continue;

                if (!deleteFieldValues.TryGetValue(fieldDef.Name, out var fieldValue))
                    continue;

                if (fieldDef.IsRowVersion)
                    hadRowVersion = true;

                try
                {
                    if (sqlFilter.Length > 0)
                        sqlFilter.Append(" AND ");

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
                    OrmLiteUtils.HandleException(ex, "ERROR in PrepareParameterizedDeleteStatement(): " + ex.Message);
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
            this.InitDbParam(p, fieldDef.ColumnType);
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
        public virtual Task EnableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default) =>
            TypeConstants.EmptyTask;

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
        public virtual Task DisableIdentityInsertAsync<T>(IDbCommand cmd, CancellationToken token = default) =>
            TypeConstants.EmptyTask;

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
        public virtual Task EnableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default) =>
            TypeConstants.EmptyTask;

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
        public virtual Task DisableForeignKeysCheckAsync(IDbCommand cmd, CancellationToken token = default) =>
            TypeConstants.EmptyTask;

        /// <summary>
        /// Sets the parameter values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="obj">The object.</param>
        /// <exception cref="System.ArgumentException">Field Definition '{fieldName}' was not found</exception>
        public virtual void SetParameterValues<T>(IDbCommand dbCmd, object obj)
        {
            var modelDef = GetModel(typeof(T));
            var fieldMap = this.GetFieldDefinitionMap(modelDef);

            foreach (IDataParameter p in dbCmd.Parameters)
            {
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
                        throw new ArgumentException($"Field Definition '{fieldName}' was not found");
                }

                if (fieldDef.AutoId && p.Value != null)
                {
                    var existingId = fieldDef.GetValue(obj);
                    if (existingId is Guid existingGuid && existingGuid != default)
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
            var value = GetValueOrDbNull(fieldDef, obj);
            p.Value = value;

            SetParameterSize(fieldDef, p);
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
                return null;

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
                return null;

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
                return DBNull.Value;

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
                return DBNull.Value;

            var unquotedVal = this.GetQuotedValue(value, fieldDef.FieldType).TrimStart('\'').TrimEnd('\'');

            if (string.IsNullOrEmpty(unquotedVal))
                return DBNull.Value;

            return unquotedVal;
        }

        /// <summary>
        /// Prepares the update row statement.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="updateFields">The update fields.</param>
        /// <exception cref="System.Exception">No valid update properties provided (e.g. p => p.FirstName): " + dbCmd.CommandText</exception>
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
                    continue;

                try
                {
                    if (fieldDef.IsPrimaryKey && updateAllFields)
                    {
                        if (sqlFilter.Length > 0)
                            sqlFilter.Append(" AND ");

                        sqlFilter.Append(this.GetQuotedColumnName(fieldDef.FieldName)).Append("=").Append(
                            this.AddQueryParam(dbCmd, fieldDef.GetValue(objWithProperties), fieldDef).ParameterName);

                        continue;
                    }

                    if (!updateAllFields && !updateFields.Contains(fieldDef.Name, StringComparer.OrdinalIgnoreCase) ||
                        fieldDef.AutoIncrement)
                        continue;

                    if (sql.Length > 0)
                        sql.Append(", ");

                    sql.Append(this.GetQuotedColumnName(fieldDef.FieldName)).Append("=").Append(
                        this.GetUpdateParam(dbCmd, fieldDef.GetValue(objWithProperties), fieldDef));
                }
                catch (Exception ex)
                {
                    OrmLiteUtils.HandleException(ex, "ERROR in ToUpdateRowStatement(): " + ex.Message);
                }
            }

            var strFilter = StringBuilderCacheAlt.ReturnAndFree(sqlFilter);
            dbCmd.CommandText = $"UPDATE {this.GetQuotedTableName(modelDef)} " +
                                $"SET {StringBuilderCache.ReturnAndFree(sql)}{(strFilter.Length > 0 ? " WHERE " + strFilter : string.Empty)}";

            if (sql.Length == 0)
                throw new Exception(
                    "No valid update properties provided (e.g. p => p.FirstName): " + dbCmd.CommandText);
        }

        /// <summary>
        /// Prepares the update row statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <exception cref="System.Exception">No valid update properties provided (e.g. () => new Person { Age = 27 }): " + dbCmd.CommandText</exception>
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
                    continue;

                var value = entry.Value;

                try
                {
                    if (sql.Length > 0)
                    {
                        sql.Append(", ");
                    }

                    sql.Append(this.GetQuotedColumnName(fieldDef.FieldName));

                    sql.Append("=");

                    sql.Append(this.GetUpdateParam(dbCmd, value, fieldDef));
                }
                catch (Exception ex)
                {
                    OrmLiteUtils.HandleException(ex, "ERROR in PrepareUpdateRowStatement(cmd,args): " + ex.Message);
                }
            }

            dbCmd.CommandText = $"UPDATE {this.GetQuotedTableName(modelDef)} " +
                                $"SET {StringBuilderCache.ReturnAndFree(sql)}{(string.IsNullOrEmpty(sqlFilter) ? string.Empty : " ")}{sqlFilter}";

            if (sql.Length == 0)
                throw new Exception(
                    "No valid update properties provided (e.g. () => new Person { Age = 27 }): " + dbCmd.CommandText);
        }

        /// <summary>
        /// Prepares the update row add statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <exception cref="System.Exception">No valid update properties provided (e.g. () => new Person { Age = 27 }): " + dbCmd.CommandText</exception>
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
                    continue;

                var value = entry.Value;

                try
                {
                    if (sql.Length > 0)
                        sql.Append(", ");

                    var quotedFieldName = this.GetQuotedColumnName(fieldDef.FieldName);

                    if (fieldDef.FieldType.IsNumericType())
                    {
                        sql.Append(quotedFieldName).Append("=").Append(quotedFieldName).Append("+")
                            .Append(this.GetUpdateParam(dbCmd, value, fieldDef));
                    }
                    else
                    {
                        sql.Append(quotedFieldName).Append("=").Append(this.GetUpdateParam(dbCmd, value, fieldDef));
                    }
                }
                catch (Exception ex)
                {
                    OrmLiteUtils.HandleException(ex, "ERROR in PrepareUpdateRowAddStatement(): " + ex.Message);
                }
            }

            dbCmd.CommandText = $"UPDATE {this.GetQuotedTableName(modelDef)} " +
                                $"SET {StringBuilderCache.ReturnAndFree(sql)}{(string.IsNullOrEmpty(sqlFilter) ? string.Empty : " ")}{sqlFilter}";

            if (sql.Length == 0)
                throw new Exception(
                    "No valid update properties provided (e.g. () => new Person { Age = 27 }): " + dbCmd.CommandText);
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
                                        sqlFilter.Substring(0, deleteStatement.Length).ToUpper()
                                            .Equals(deleteStatement);

            if (isFullDeleteStatement)
                return sqlFilter.SqlFmt(this, filterParams);

            var modelDef = tableType.GetModelDefinition();
            sql.Append($"DELETE FROM {this.GetQuotedTableName(modelDef)}");

            if (string.IsNullOrEmpty(sqlFilter))
                return StringBuilderCache.ReturnAndFree(sql);

            sqlFilter = sqlFilter.SqlFmt(this, filterParams);
            sql.Append(" WHERE ");
            sql.Append(sqlFilter);

            return StringBuilderCache.ReturnAndFree(sql);
        }

        /// <summary>
        /// Determines whether [has insert return values] [the specified model definition].
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <returns><c>true</c> if [has insert return values] [the specified model definition]; otherwise, <c>false</c>.</returns>
        public virtual bool HasInsertReturnValues(ModelDefinition modelDef) =>
            modelDef.FieldDefinitions.Any(x => x.ReturnOnInsert);

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
                return null;

            if (!sql.StartsWith("{"))
                return sql;

            return this.Variables.TryGetValue(sql, out var variable) ? variable : null;
        }

        /// <summary>
        /// Gets the automatic identifier default value.
        /// </summary>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public virtual string GetAutoIdDefaultValue(FieldDefinition fieldDef) => null;

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
        public static IEnumerable<FieldDefinition> GetFieldDefinitions(ModelDefinition modelDef) =>
            modelDef.FieldDefinitions.OrderBy(fd => fd.Order);

        /// <summary>
        /// Converts to createschemastatement.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        public abstract string ToCreateSchemaStatement(string schemaName);

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
                if (fieldDef.CustomSelect != null || fieldDef.IsComputed && !fieldDef.IsPersisted)
                    continue;

                var columnDefinition = this.GetColumnDefinition(fieldDef, modelDef);

                if (columnDefinition == null)
                    continue;

                if (sbColumns.Length != 0)
                    sbColumns.Append(", \n  ");

                sbColumns.Append(columnDefinition);

                var sqlConstraint = this.GetCheckConstraint(modelDef, fieldDef);
                if (sqlConstraint != null)
                {
                    sbConstraints.Append(",\n" + sqlConstraint);
                }

                if (fieldDef.ForeignKey == null || OrmLiteConfig.SkipForeignKeys)
                    continue;

                var refModelDef = fieldDef.ForeignKey.ReferenceType.GetModelDefinition();
                sbConstraints.Append(
                    $", \n\n  CONSTRAINT {this.GetQuotedName(fieldDef.ForeignKey.GetForeignKeyName(modelDef, refModelDef, this.NamingStrategy, fieldDef))} " +
                    $"FOREIGN KEY ({this.GetQuotedColumnName(fieldDef.FieldName)}) " +
                    $"REFERENCES {this.GetQuotedTableName(refModelDef)} ({this.GetQuotedColumnName(refModelDef.PrimaryKey.FieldName)})");

                sbConstraints.Append(this.GetForeignKeyOnDeleteClause(fieldDef.ForeignKey));
                sbConstraints.Append(this.GetForeignKeyOnUpdateClause(fieldDef.ForeignKey));
            }

            var uniqueConstraints = this.GetUniqueConstraints(modelDef);
            if (uniqueConstraints != null)
            {
                sbConstraints.Append(",\n" + uniqueConstraints);
            }

            /*var compositePrimaryKey = GetCompositePrimaryKey(modelDef);
            if (compositePrimaryKey != null)
            {
                sbConstraints.Append(",\n" + compositePrimaryKey);
            }*/

            var sql = $"CREATE TABLE {this.GetQuotedTableName(modelDef)} " +
                      $"\n(\n  {StringBuilderCache.ReturnAndFree(sbColumns)}{StringBuilderCacheAlt.ReturnAndFree(sbConstraints)} \n); \n";

            return sql;
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
                    $"CONSTRAINT {this.GetUniqueConstraintName(x, this.GetTableName(modelDef).StripDbQuotes())} UNIQUE ({x.FieldNames.Map(f => modelDef.GetQuotedName(f, this)).Join(",")})");

            return constraints.Count > 0 ? constraints.Join(",\n") : null;
        }

        /// <summary>
        /// Gets the name of the unique constraint.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetUniqueConstraintName(UniqueConstraintAttribute constraint, string tableName) =>
            constraint.Name ?? $"UC_{tableName}_{constraint.FieldNames.Join("_")}";

        /// <summary>
        /// Gets the check constraint.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public virtual string GetCheckConstraint(ModelDefinition modelDef, FieldDefinition fieldDef)
        {
            if (fieldDef.CheckConstraint == null)
                return null;

            return
                $"CONSTRAINT CHK_{modelDef.Schema}_{modelDef.ModelName}_{fieldDef.FieldName} CHECK ({fieldDef.CheckConstraint})";
        }

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
                if (!fieldDef.IsIndexed) continue;

                var indexName = fieldDef.IndexName ?? this.GetIndexName(
                    fieldDef.IsUniqueIndex,
                    modelDef.ModelName.SafeVarName(),
                    fieldDef.FieldName);

                sqlIndexes.Add(
                    this.ToCreateIndexStatement(
                        fieldDef.IsUniqueIndex,
                        indexName,
                        modelDef,
                        fieldDef.FieldName,
                        isCombined: false,
                        fieldDef: fieldDef));
            }

            foreach (var compositeIndex in modelDef.CompositeIndexes)
            {
                var indexName = this.GetCompositeIndexName(compositeIndex, modelDef);

                var sb = StringBuilderCache.Allocate();
                foreach (var fieldName in compositeIndex.FieldNames)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");

                    var parts = fieldName.SplitOnLast(' ');
                    if (parts.Length == 2 &&
                        (parts[1].ToLower().StartsWith("desc") || parts[1].ToLower().StartsWith("asc")))
                    {
                        sb.Append(this.GetQuotedColumnName(parts[0])).Append(' ').Append(parts[1]);
                    }
                    else
                    {
                        sb.Append(this.GetQuotedColumnName(fieldName));
                    }
                }

                sqlIndexes.Add(
                    this.ToCreateIndexStatement(
                        compositeIndex.Unique,
                        indexName,
                        modelDef,
                        StringBuilderCache.ReturnAndFree(sb),
                        isCombined: true));
            }

            return sqlIndexes;
        }

        /// <summary>
        /// Doeses the table exist.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool DoesTableExist(IDbConnection db, string tableName, string schema = null)
        {
            return db.Exec(dbCmd => this.DoesTableExist(dbCmd, tableName, schema));
        }

        /// <summary>
        /// Does table exist as an asynchronous operation.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public virtual async Task<bool> DoesTableExistAsync(
            IDbConnection db,
            string tableName,
            string schema = null,
            CancellationToken token = default)
        {
            return await db.Exec(async dbCmd => await this.DoesTableExistAsync(dbCmd, tableName, schema, token));
        }

        /// <summary>
        /// Doeses the table exist.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool DoesTableExist(IDbCommand dbCmd, string tableName, string schema = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Doeses the table exist asynchronous.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> DoesTableExistAsync(
            IDbCommand dbCmd,
            string tableName,
            string schema = null,
            CancellationToken token = default)
        {
            return this.DoesTableExist(dbCmd, tableName, schema).InTask();
        }

        /// <summary>
        /// Doeses the column exist.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool DoesColumnExist(IDbConnection db, string columnName, string tableName, string schema = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Doeses the column exist asynchronous.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> DoesColumnExistAsync(
            IDbConnection db,
            string columnName,
            string tableName,
            string schema = null,
            CancellationToken token = default)
        {
            return this.DoesColumnExist(db, columnName, tableName, schema).InTask();
        }

        /// <summary>
        /// Gets the type of the column data.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string GetColumnDataType(
            IDbConnection db,
            string columnName,
            string tableName,
            string schema = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Columns the is nullable.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool ColumnIsNullable(
            IDbConnection db,
            string columnName,
            string tableName,
            string schema = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the maximum length of the column.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual long GetColumnMaxLength(
            IDbConnection db,
            string columnName,
            string tableName,
            string schema = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Doeses the sequence exist.
        /// </summary>
        /// <param name="dbCmd">The database command.</param>
        /// <param name="sequenceName">Name of the sequence.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool DoesSequenceExist(IDbCommand dbCmd, string sequenceName)
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
            return $"CREATE {(isUnique ? "UNIQUE" : string.Empty)}" +
                   (fieldDef?.IsClustered == true ? " CLUSTERED" : string.Empty) +
                   (fieldDef?.IsNonClustered == true ? " NONCLUSTERED" : string.Empty) +
                   $" INDEX {indexName} ON {this.GetQuotedTableName(modelDef)} " +
                   $"({(isCombined ? fieldName : this.GetQuotedColumnName(fieldName))}); \n";
        }

        /// <summary>
        /// Converts to createsequencestatements.
        /// </summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public virtual List<string> ToCreateSequenceStatements(Type tableType)
        {
            return new List<string>();
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

        /// <summary>
        /// Sequences the list.
        /// </summary>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public virtual List<string> SequenceList(Type tableType) => new();

        /// <summary>
        /// Sequences the list asynchronous.
        /// </summary>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        public virtual Task<List<string>> SequenceListAsync(Type tableType, CancellationToken token = default) =>
            new List<string>().InTask();

        // TODO : make abstract  ??
        /// <summary>
        /// Converts to existstatement.
        /// </summary>
        /// <param name="fromTableType">Type of from table.</param>
        /// <param name="objWithProperties">The object with properties.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        /// <returns>System.String.</returns>
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
        /// Gets the drop primary key constraint.
        /// </summary>
        /// <param name="modelDef">The model definition.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public virtual string GetDropPrimaryKeyConstraint(ModelDefinition modelDef, string name)
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

        /// <summary>
        /// Converts to addcolumnstatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public virtual string ToAddColumnStatement(Type modelType, FieldDefinition fieldDef)
        {
            var column = this.GetColumnDefinition(fieldDef);
            return $"ALTER TABLE {this.GetQuotedTableName(modelType.GetModelDefinition())} ADD COLUMN {column};";
        }

        /// <summary>
        /// Converts to altercolumnstatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <returns>System.String.</returns>
        public virtual string ToAlterColumnStatement(Type modelType, FieldDefinition fieldDef)
        {
            var column = this.GetColumnDefinition(fieldDef);
            return $"ALTER TABLE {this.GetQuotedTableName(modelType.GetModelDefinition())} MODIFY COLUMN {column};";
        }

        /// <summary>
        /// Converts to changecolumnnamestatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="fieldDef">The field definition.</param>
        /// <param name="oldColumnName">Old name of the column.</param>
        /// <returns>System.String.</returns>
        public virtual string ToChangeColumnNameStatement(
            Type modelType,
            FieldDefinition fieldDef,
            string oldColumnName)
        {
            var column = this.GetColumnDefinition(fieldDef);
            return
                $"ALTER TABLE {this.GetQuotedTableName(modelType.GetModelDefinition())} CHANGE COLUMN {this.GetQuotedColumnName(oldColumnName)} {column};";
        }

        /// <summary>
        /// Converts to addforeignkeystatement.
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
            var fieldName = sourceMD.GetFieldDefinition(field).FieldName;

            var referenceMD = ModelDefinition<TForeign>.Definition;
            var referenceFieldName = referenceMD.GetFieldDefinition(foreignField).FieldName;

            string name = this.GetQuotedName(
                foreignKeyName.IsNullOrEmpty()
                    ? "fk_" + sourceMD.ModelName + "_" + fieldName + "_" + referenceFieldName
                    : foreignKeyName);

            return $"ALTER TABLE {this.GetQuotedTableName(sourceMD)} " +
                   $"ADD CONSTRAINT {name} FOREIGN KEY ({this.GetQuotedColumnName(fieldName)}) " +
                   $"REFERENCES {this.GetQuotedTableName(referenceMD)} " +
                   $"({this.GetQuotedColumnName(referenceFieldName)})" +
                   $" {this.GetForeignKeyOnDeleteClause(new ForeignKeyConstraint(typeof(T), onDelete: this.FkOptionToString(onDelete)))}" +
                   $" {this.GetForeignKeyOnUpdateClause(new ForeignKeyConstraint(typeof(T), onUpdate: this.FkOptionToString(onUpdate)))};";
        }

        /// <summary>
        /// Converts to createindexstatement.
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
            var fieldName = sourceDef.GetFieldDefinition(field).FieldName;

            string name = this.GetQuotedName(
                indexName.IsNullOrEmpty()
                    ? (unique ? "uidx" : "idx") + "_" + sourceDef.ModelName + "_" + fieldName
                    : indexName);

            string command = $"CREATE {(unique ? "UNIQUE" : string.Empty)} " +
                             $"INDEX {name} ON {this.GetQuotedTableName(sourceDef)}" +
                             $"({this.GetQuotedColumnName(fieldName)});";
            return command;
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
                _ => "RESTRICT",
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
                return "NULL";

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
        /// <param name="db">The database.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="columnName">Name of the column.</param>
        public virtual void DropColumn(IDbConnection db, Type modelType, string columnName)
        {
            var provider = db.GetDialectProvider();
            var command = this.ToDropColumnStatement(modelType, columnName, provider);

            db.ExecuteSql(command);
        }

        /// <summary>
        /// Converts to dropcolumnstatement.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>System.String.</returns>
        protected virtual string ToDropColumnStatement(
            Type modelType,
            string columnName,
            IOrmLiteDialectProvider provider)
        {
            return $"ALTER TABLE {provider.GetQuotedTableName(modelType.GetModelDefinition())} " +
                   $"DROP COLUMN {provider.GetQuotedColumnName(columnName)};";
        }

        /// <summary>
        /// Converts to tablenamesstatement.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public virtual string ToTableNamesStatement(string schema) => throw new NotSupportedException();

        /// <summary>
        /// Return table, row count SQL for listing all tables with their row counts
        /// </summary>
        /// <param name="live">If true returns live current row counts of each table (slower), otherwise returns cached row counts from RDBMS table stats</param>
        /// <param name="schema">The table schema if any</param>
        /// <returns>System.String.</returns>
        public virtual string ToTableNamesWithRowCountsStatement(bool live, string schema) =>
            null; // returning null Fallsback to slow UNION N+1 COUNT(*) op

        /// <summary>
        /// SQLs the conflict.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="conflictResolution">The conflict resolution.</param>
        /// <returns>System.String.</returns>
        public virtual string SqlConflict(string sql, string conflictResolution) => sql; // NOOP

        /// <summary>
        /// SQLs the concat.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public virtual string SqlConcat(IEnumerable<object> args) => $"CONCAT({string.Join(", ", args)})";

        /// <summary>
        /// SQLs the currency.
        /// </summary>
        /// <param name="fieldOrValue">The field or value.</param>
        /// <returns>System.String.</returns>
        public virtual string SqlCurrency(string fieldOrValue) => this.SqlCurrency(fieldOrValue, "$");

        /// <summary>
        /// SQLs the currency.
        /// </summary>
        /// <param name="fieldOrValue">The field or value.</param>
        /// <param name="currencySymbol">The currency symbol.</param>
        /// <returns>System.String.</returns>
        public virtual string SqlCurrency(string fieldOrValue, string currencySymbol) =>
            this.SqlConcat(new List<string> { currencySymbol, fieldOrValue });

        /// <summary>
        /// SQLs the bool.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>System.String.</returns>
        public virtual string SqlBool(bool value) => value ? "true" : "false";

        /// <summary>
        /// SQLs the limit.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="rows">The rows.</param>
        /// <returns>System.String.</returns>
        public virtual string SqlLimit(int? offset = null, int? rows = null) =>
            rows == null && offset == null ? string.Empty :
            offset == null ? "LIMIT " + rows : "LIMIT " + rows.GetValueOrDefault(int.MaxValue) + " OFFSET " + offset;

        /// <summary>
        /// SQLs the cast.
        /// </summary>
        /// <param name="fieldOrValue">The field or value.</param>
        /// <param name="castAs">The cast as.</param>
        /// <returns>System.String.</returns>
        public virtual string SqlCast(object fieldOrValue, string castAs) => $"CAST({fieldOrValue} AS {castAs})";

        /// <summary>
        /// Gets the SQL random.
        /// </summary>
        /// <value>The SQL random.</value>
        public virtual string SqlRandom => "RAND()";

        // Async API's, should be overriden by Dialect Providers to use .ConfigureAwait(false)
        // Default impl below uses TaskAwaiter shim in async.cs
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

#if ASYNC
        /// <summary>
        /// Readers the each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="fn">The function.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;T&gt;&gt;.</returns>
        public virtual async Task<List<T>> ReaderEach<T>(
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
        public virtual async Task<Return> ReaderEach<Return>(
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
        public virtual async Task<T> ReaderRead<T>(IDataReader reader, Func<T> fn, CancellationToken token = default)
        {
            try
            {
                if (await this.ReadAsync(reader, token))
                    return fn();

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
                return new NotImplementedException(
                    "Returning last inserted identity is not implemented on this DB Provider.").InTask<long>();

            dbCmd.CommandText += "; " + this.SelectIdentitySql;

            return dbCmd.ExecLongScalarAsync(null, token);
        }

#else
        public Task<List<T>> ReaderEach<T>(IDataReader reader, Func<T> fn, CancellationToken token =
 new CancellationToken())
        {
            throw new NotImplementedException(OrmLiteUtils.AsyncRequiresNet45Error);
        }

        public Task<Return> ReaderEach<Return>(IDataReader reader, Action fn, Return source, CancellationToken token =
 new CancellationToken())
        {
            throw new NotImplementedException(OrmLiteUtils.AsyncRequiresNet45Error);
        }

        public Task<T> ReaderRead<T>(IDataReader reader, Func<T> fn, CancellationToken token = new CancellationToken())
        {
            throw new NotImplementedException(OrmLiteUtils.AsyncRequiresNet45Error);
        }

        public Task<long> InsertAndGetLastInsertIdAsync<T>(IDbCommand dbCmd, CancellationToken token)
        {
            throw new NotImplementedException(OrmLiteUtils.AsyncRequiresNet45Error);
        }
#endif

        /// <summary>
        /// Gets the UTC date function.
        /// </summary>
        /// <returns>System.String.</returns>
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
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string DatabaseSize(string database)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SQLs the version.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string SQLVersion()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SQLs the name of the server.
        /// </summary>
        /// <returns>System.String.</returns>
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
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string InnerRunSqlExecuteReader(IDbCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
