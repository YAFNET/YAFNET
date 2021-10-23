// ***********************************************************************
// <copyright file="MySqlIntegerConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Data;
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite.MySql.Converters
{
    /// <summary>
    /// Class MySqlIntegerConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.IntegerConverter" />
    public abstract class MySqlIntegerConverter : IntegerConverter
    {
        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "INT(11)";
    }

    /// <summary>
    /// Class MySqlByteConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    public class MySqlByteConverter : MySqlIntegerConverter
    {
        /// <summary>
        /// Used in DB Params. Defaults to DbType.String
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Byte;
    }

    /// <summary>
    /// Class MySqlSByteConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    public class MySqlSByteConverter : MySqlIntegerConverter
    {
        /// <summary>
        /// Used in DB Params. Defaults to DbType.String
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.SByte;
    }

    /// <summary>
    /// Class MySqlInt16Converter.
    /// Implements the <see cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    public class MySqlInt16Converter : MySqlIntegerConverter
    {
        /// <summary>
        /// Used in DB Params. Defaults to DbType.String
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Int16;
    }

    /// <summary>
    /// Class MySqlUInt16Converter.
    /// Implements the <see cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    public class MySqlUInt16Converter : MySqlIntegerConverter
    {
        /// <summary>
        /// Used in DB Params. Defaults to DbType.String
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.UInt16;
    }

    /// <summary>
    /// Class MySqlInt32Converter.
    /// Implements the <see cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    public class MySqlInt32Converter : MySqlIntegerConverter
    {
    }

    /// <summary>
    /// Class MySqlUInt32Converter.
    /// Implements the <see cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.MySql.Converters.MySqlIntegerConverter" />
    public class MySqlUInt32Converter : MySqlIntegerConverter
    {
        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.UInt32;
    }
}