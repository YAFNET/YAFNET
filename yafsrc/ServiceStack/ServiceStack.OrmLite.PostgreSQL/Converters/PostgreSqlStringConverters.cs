// ***********************************************************************
// <copyright file="PostgreSqlStringConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite.PostgreSQL.Converters
{
    using ServiceStack.DataAnnotations;
    using ServiceStack.OrmLite.Converters;

    /// <summary>
    /// Class PostgreSqlStringConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.StringConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.StringConverter" />
    public class PostgreSqlStringConverter : StringConverter
    {
        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "TEXT";

        //https://dba.stackexchange.com/questions/189876/size-limit-of-character-varying-postgresql
        /// <summary>
        /// Gets the maximum length of the variable character.
        /// </summary>
        /// <value>The maximum length of the variable character.</value>
        public override int MaxVarCharLength => UseUnicode ? 10485760 / 2 : 10485760;

        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <param name="stringLength">Length of the string.</param>
        /// <returns>System.String.</returns>
        public override string GetColumnDefinition(int? stringLength)
        {
            //PostgreSQL doesn't support NVARCHAR when UseUnicode = true so just use TEXT
            if (stringLength == null || stringLength == StringLengthAttribute.MaxText)
                return ColumnDefinition;

            return $"VARCHAR({stringLength.Value})";
        }
    }

    /// <summary>
    /// Class PostgreSqlCharArrayConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.CharArrayConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.CharArrayConverter" />
    public class PostgreSqlCharArrayConverter : CharArrayConverter
    {
        /// <summary>
        /// SQL Column Definition used in CREATE Table.
        /// </summary>
        /// <value>The column definition.</value>
        public override string ColumnDefinition => "TEXT";

        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <param name="stringLength">Length of the string.</param>
        /// <returns>System.String.</returns>
        public override string GetColumnDefinition(int? stringLength)
        {
            return ColumnDefinition;
        }
    }
}