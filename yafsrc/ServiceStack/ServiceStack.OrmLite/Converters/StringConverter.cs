// ***********************************************************************
// <copyright file="StringConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite.Base.Text;

namespace ServiceStack.OrmLite.Converters;

/// <summary>
/// Class StringConverter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteConverter" />
/// Implements the <see cref="ServiceStack.OrmLite.IHasColumnDefinitionLength" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteConverter" />
/// <seealso cref="ServiceStack.OrmLite.IHasColumnDefinitionLength" />
public class StringConverter : OrmLiteConverter, IHasColumnDefinitionLength
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringConverter" /> class.
    /// </summary>
    public StringConverter() : this(8000) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringConverter" /> class.
    /// </summary>
    /// <param name="stringLength">Length of the string.</param>
    public StringConverter(int stringLength)
    {
        this.StringLength = stringLength;
    }

    /// <summary>
    /// Gets or sets a value indicating whether [use unicode].
    /// </summary>
    /// <value><c>true</c> if [use unicode]; otherwise, <c>false</c>.</value>
    public bool UseUnicode { get; set; }

    /// <summary>
    /// Gets or sets the length of the string.
    /// </summary>
    /// <value>The length of the string.</value>
    public int StringLength { get; set; }

    /// <summary>
    /// Gets the maximum length of the variable character.
    /// </summary>
    /// <value>The maximum length of the variable character.</value>
    public virtual int MaxVarCharLength => this.UseUnicode ? 8000 : 4000;

    /// <summary>
    /// The maximum column definition
    /// </summary>
    protected string maxColumnDefinition;
    /// <summary>
    /// Gets or sets the maximum column definition.
    /// </summary>
    /// <value>The maximum column definition.</value>
    public virtual string MaxColumnDefinition
    {
        get => this.maxColumnDefinition ?? this.ColumnDefinition;
        set => this.maxColumnDefinition = value;
    }

    /// <summary>
    /// SQL Column Definition used in CREATE Table.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => this.GetColumnDefinition(this.StringLength);

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="stringLength">Length of the string.</param>
    /// <returns>System.String.</returns>
    public virtual string GetColumnDefinition(int? stringLength)
    {
        if (stringLength.GetValueOrDefault() == StringLengthAttribute.MaxText)
        {
            return this.MaxColumnDefinition;
        }

        return this.UseUnicode
                   ? $"NVARCHAR({stringLength.GetValueOrDefault(this.StringLength)})"
                   : $"VARCHAR({stringLength.GetValueOrDefault(this.StringLength)})";
    }

    /// <summary>
    /// Customize how DB Param is initialized. Useful for supporting RDBMS-specific Types.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="fieldType">Type of the field.</param>
    public override void InitDbParam(IDbDataParameter p, Type fieldType)
    {
        base.InitDbParam(p, fieldType);

        if (p.Size == 0 && fieldType == typeof(string))
        {
            p.Size = this.UseUnicode
                         ? Math.Min(this.StringLength, 4000)
                         : this.StringLength;
        }
    }

    /// <summary>
    /// Value from DB to Populate on POCO Data Model with
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is string strValue)
        {
            if (OrmLiteConfig.StringFilter != null)
            {
                return OrmLiteConfig.StringFilter(strValue);
            }
        }

        return value.ToString();
    }
}

/// <summary>
/// Class CharConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.StringConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.StringConverter" />
public class CharConverter : StringConverter
{
    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <value>The column definition.</value>
    public override string ColumnDefinition => "CHAR(1)";

    /// <summary>
    /// Used in DB Params. Defaults to DbType.String
    /// </summary>
    /// <value>The type of the database.</value>
    public override DbType DbType => DbType.StringFixedLength;

    /// <summary>
    /// Gets the column definition.
    /// </summary>
    /// <param name="stringLength">Length of the string.</param>
    /// <returns>System.String.</returns>
    public override string GetColumnDefinition(int? stringLength)
    {
        return this.ColumnDefinition;
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is char)
        {
            return value;
        }

        if (value is string strValue)
        {
            return strValue[0];
        }

        if (value.GetType().IsIntegerType())
        {
            return (char)(int)this.ConvertNumber(typeof(int), value);
        }

        return (char)value;
    }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        if (value != null && value.GetType().IsEnum)
        {
            return EnumConverter.ToCharValue(value);
        }

        if (value is int i)
        {
            return (char)i;
        }

        return base.ToDbValue(fieldType, value);
    }
}

/// <summary>
/// Class CharArrayConverter.
/// Implements the <see cref="ServiceStack.OrmLite.Converters.StringConverter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Converters.StringConverter" />
public class CharArrayConverter : StringConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharArrayConverter" /> class.
    /// </summary>
    public CharArrayConverter() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="CharArrayConverter" /> class.
    /// </summary>
    /// <param name="stringLength">Length of the string.</param>
    public CharArrayConverter(int stringLength) : base(stringLength) { }

    /// <summary>
    /// Parameterized value in parameterized queries
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object ToDbValue(Type fieldType, object value)
    {
        var chars = (char[])value;
        return new string(chars);
    }

    /// <summary>
    /// Froms the database value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public override object FromDbValue(Type fieldType, object value)
    {
        if (value is char[])
        {
            return value;
        }

        if (value is string strValue)
        {
            return strValue.ToCharArray();
        }

        return (char[])value;
    }
}