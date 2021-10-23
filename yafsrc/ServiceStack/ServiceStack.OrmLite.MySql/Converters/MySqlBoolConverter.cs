// ***********************************************************************
// <copyright file="MySqlBoolConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.MySql.Converters
{
    using ServiceStack.OrmLite.Converters;

    /// <summary>
    /// Class MySqlBoolConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.BoolAsIntConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.BoolAsIntConverter" />
    public class MySqlBoolConverter : BoolAsIntConverter
    {
        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "tinyint(1)";
    }
}