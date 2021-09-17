// ***********************************************************************
// <copyright file="SqliteStringConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.Sqlite.Converters
{
    using ServiceStack.OrmLite.Converters;

    /// <summary>
    /// Class SqliteStringConverter.
    /// Implements the <see cref="StringConverter" />
    /// </summary>
    /// <seealso cref="StringConverter" />
    public class SqliteStringConverter : StringConverter
    {
        /// <summary>
        /// Gets the maximum column definition.
        /// </summary>
        /// <value>The maximum column definition.</value>
        public override string MaxColumnDefinition => UseUnicode ? "NVARCHAR(1000000)" : "VARCHAR(1000000)";

        /// <summary>
        /// Gets the maximum length of the variable character.
        /// </summary>
        /// <value>The maximum length of the variable character.</value>
        public override int MaxVarCharLength => 1000000;
    }
}