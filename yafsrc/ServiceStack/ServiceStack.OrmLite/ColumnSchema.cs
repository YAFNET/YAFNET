// ***********************************************************************
// <copyright file="ColumnSchema.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Text;

using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite;

/// <summary>
/// Class ColumnSchema.
/// </summary>
public class ColumnSchema
{
    /// <summary>
    /// Gets or sets the name of the column.
    /// </summary>
    /// <value>The name of the column.</value>
    public string ColumnName { get; set; }
    /// <summary>
    /// Gets or sets the column ordinal.
    /// </summary>
    /// <value>The column ordinal.</value>
    public int ColumnOrdinal { get; set; }
    /// <summary>
    /// Gets or sets the size of the column.
    /// </summary>
    /// <value>The size of the column.</value>
    public int ColumnSize { get; set; }
    /// <summary>
    /// Gets or sets the numeric precision.
    /// </summary>
    /// <value>The numeric precision.</value>
    public int NumericPrecision { get; set; }
    /// <summary>
    /// Gets or sets the numeric scale.
    /// </summary>
    /// <value>The numeric scale.</value>
    public int NumericScale { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is unique.
    /// </summary>
    /// <value><c>true</c> if this instance is unique; otherwise, <c>false</c>.</value>
    public bool IsUnique { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is key.
    /// </summary>
    /// <value><c>true</c> if this instance is key; otherwise, <c>false</c>.</value>
    public bool IsKey { get; set; }
    /// <summary>
    /// Gets or sets the name of the base server.
    /// </summary>
    /// <value>The name of the base server.</value>
    public string BaseServerName { get; set; }
    /// <summary>
    /// Gets or sets the name of the base catalog.
    /// </summary>
    /// <value>The name of the base catalog.</value>
    public string BaseCatalogName { get; set; }
    /// <summary>
    /// Gets or sets the name of the base column.
    /// </summary>
    /// <value>The name of the base column.</value>
    public string BaseColumnName { get; set; }
    /// <summary>
    /// Gets or sets the name of the base schema.
    /// </summary>
    /// <value>The name of the base schema.</value>
    public string BaseSchemaName { get; set; }
    /// <summary>
    /// Gets or sets the name of the base table.
    /// </summary>
    /// <value>The name of the base table.</value>
    public string BaseTableName { get; set; }
    /// <summary>
    /// Gets or sets the type of the data.
    /// </summary>
    /// <value>The type of the data.</value>
    public Type DataType { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [allow database null].
    /// </summary>
    /// <value><c>true</c> if [allow database null]; otherwise, <c>false</c>.</value>
    public bool AllowDBNull { get; set; }
    /// <summary>
    /// Gets or sets the type of the provider.
    /// </summary>
    /// <value>The type of the provider.</value>
    public int ProviderType { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is aliased.
    /// </summary>
    /// <value><c>true</c> if this instance is aliased; otherwise, <c>false</c>.</value>
    public bool IsAliased { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is expression.
    /// </summary>
    /// <value><c>true</c> if this instance is expression; otherwise, <c>false</c>.</value>
    public bool IsExpression { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is automatic increment.
    /// </summary>
    /// <value><c>true</c> if this instance is automatic increment; otherwise, <c>false</c>.</value>
    public bool IsAutoIncrement { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is row version.
    /// </summary>
    /// <value><c>true</c> if this instance is row version; otherwise, <c>false</c>.</value>
    public bool IsRowVersion { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is hidden.
    /// </summary>
    /// <value><c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
    public bool IsHidden { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is long.
    /// </summary>
    /// <value><c>true</c> if this instance is long; otherwise, <c>false</c>.</value>
    public bool IsLong { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is read only.
    /// </summary>
    /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly { get; set; }
    /// <summary>
    /// Gets or sets the type of the provider specific data.
    /// </summary>
    /// <value>The type of the provider specific data.</value>
    public Type ProviderSpecificDataType { get; set; }
    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue { get; set; }
    /// <summary>
    /// Gets or sets the name of the data type.
    /// </summary>
    /// <value>The name of the data type.</value>
    public string DataTypeName { get; set; }
    /// <summary>
    /// Gets or sets the type of the collation.
    /// </summary>
    /// <value>The type of the collation.</value>
    public string CollationType { get; set; }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        var sql = StringBuilderCache.Allocate();

        sql.Append(this.ColumnName.PadRight(20, ' ')).Append(' ');
        this.AppendDefinition(sql);

        return StringBuilderCache.ReturnAndFree(sql);
    }

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public string ColumnDefinition =>
        StringBuilderCache.ReturnAndFree(this.AppendDefinition(StringBuilderCache.Allocate()));

    /// <summary>
    /// Appends the definition.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <returns>StringBuilder.</returns>
    private StringBuilder AppendDefinition(StringBuilder sql)
    {
        sql.Append(this.DataTypeName.ToUpper());
        if (this.DataType.IsRealNumberType() && this.NumericPrecision > 0)
        {
            sql.Append('(');
            sql.Append(this.NumericPrecision);
            if (this.NumericScale > 0)
            {
                sql.Append(',');
                sql.Append(this.NumericScale);
            }

            sql.Append(')');
        }
        else if (!this.DataType.IsNumericType() && this.ColumnSize > 0)
        {
            sql.Append('(');
            sql.Append(this.ColumnSize);
            sql.Append(')');
        }

        if (this.IsKey)
        {
            sql.Append(" PRIMARY KEY");
            if (this.IsAutoIncrement)
            {
                sql.Append(' ').Append("AUTOINCREMENT");
            }
        }
        else
        {
            sql.Append(this.AllowDBNull ? " NULL" : " NOT NULL");

            if (this.IsUnique)
            {
                sql.Append(" UNIQUE");
            }
        }

        if (this.DefaultValue != null)
        {
            sql.AppendFormat(" DEFAULT ({0})", this.DefaultValue);
        }

        return sql;
    }
}