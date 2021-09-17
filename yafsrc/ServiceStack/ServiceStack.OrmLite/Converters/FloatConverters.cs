// ***********************************************************************
// <copyright file="FloatConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Converters
{
    using System;
    using System.Data;
    using System.Globalization;

    /// <summary>
    /// Class FloatConverter.
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.NativeValueOrmLiteConverter" />
    public class FloatConverter : NativeValueOrmLiteConverter
    {
        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "DOUBLE";

        /// <summary>
        /// Used in DB Params. Defaults to DbType.String
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Single;

        /// <summary>
        /// Converts to dbvalue.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object ToDbValue(Type fieldType, object value)
        {
            return this.ConvertNumber(fieldType, value);
        }

        /// <summary>
        /// Value from DB to Populate on POCO Data Model with
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object FromDbValue(Type fieldType, object value)
        {
            return this.ConvertNumber(fieldType, value);
        }

        /// <summary>
        /// Converts to Quoted string.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string ToQuotedString(Type fieldType, object value)
        {
            var typeCode = fieldType.GetTypeCode();

            return typeCode switch {
                TypeCode.Single => Convert.ToSingle(value).ToString(CultureInfo.InvariantCulture),
                TypeCode.Double => Convert.ToDouble(value).ToString(CultureInfo.InvariantCulture),
                TypeCode.Decimal => Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture),
                _ => base.ToQuotedString(fieldType, value)
            };
        }
    }

    /// <summary>
    /// Class DoubleConverter.
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.FloatConverter" />
    public class DoubleConverter : FloatConverter
    {
        /// <summary>
        /// Used in DB Params. Defaults to DbType.String
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Double;
    }

    /// <summary>
    /// Class DecimalConverter.
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.FloatConverter" />
    /// <seealso cref="ServiceStack.OrmLite.IHasColumnDefinitionPrecision" />
    public class DecimalConverter : FloatConverter, IHasColumnDefinitionPrecision
    {
        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>The precision.</value>
        public int Precision { get; set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public int Scale { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalConverter" /> class.
        /// </summary>
        public DecimalConverter()
            : this(18, 12) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalConverter" /> class.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        public DecimalConverter(int precision, int scale)
        {
            this.Precision = precision;
            this.Scale = scale;
        }

        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => GetColumnDefinition(Precision, Scale);

        /// <summary>
        /// Used in DB Params. Defaults to DbType.String
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Decimal;

        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>System.String.</returns>
        public virtual string GetColumnDefinition(int? precision, int? scale)
        {
            return $"DECIMAL({precision.GetValueOrDefault(Precision)},{scale.GetValueOrDefault(Scale)})";
        }
    }
}