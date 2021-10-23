// ***********************************************************************
// <copyright file="PostgreSqlDateTimeConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters
{
    using ServiceStack.OrmLite.Converters;

    /// <summary>
    /// Class PostgreSqlDateTimeConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.DateTimeConverter" />
    public class PostgreSqlDateTimeConverter : DateTimeConverter
    {
        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "timestamp";
    }
}