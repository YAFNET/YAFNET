// ***********************************************************************
// <copyright file="SqlServerIntegerConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.SqlServer.Converters
{
    using System.Data;

    using ServiceStack.OrmLite.Converters;

    /// <summary>
    /// Class SqlServerSByteConverter.
    /// Implements the <see cref="SByteConverter" />
    /// throws unknown type exceptions in parameterized queries, e.g: p.DbType = DbType.SByte
    /// </summary>
    /// <seealso cref="SByteConverter" />
    public class SqlServerSByteConverter : SByteConverter
    {
        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Byte;
    }

    /// <summary>
    /// Class SqlServerUInt16Converter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.UInt16Converter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.UInt16Converter" />
    public class SqlServerUInt16Converter : UInt16Converter
    {
        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Int16;
    }

    /// <summary>
    /// Class SqlServerUInt32Converter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.UInt32Converter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.UInt32Converter" />
    public class SqlServerUInt32Converter : UInt32Converter
    {
        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Int32;
    }

    /// <summary>
    /// Class SqlServerUInt64Converter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.UInt64Converter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.UInt64Converter" />
    public class SqlServerUInt64Converter : UInt64Converter
    {
        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <value>The type of the database.</value>
        public override DbType DbType => DbType.Int64;
    }
}