// ***********************************************************************
// <copyright file="SqlServerGuidConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.SqlServer.Converters
{
    using System;

    using ServiceStack.OrmLite.Converters;

    /// <summary>
    /// Class SqlServerGuidConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.GuidConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.GuidConverter" />
    public class SqlServerGuidConverter : GuidConverter
    {
        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "UniqueIdentifier";

        /// <summary>
        /// Converts to quotedstring.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string ToQuotedString(Type fieldType, object value)
        {
            var guidValue = (Guid)value;
            return $"CAST('{guidValue}' AS UNIQUEIDENTIFIER)";
        }
    }
}