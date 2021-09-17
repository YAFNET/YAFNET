// ***********************************************************************
// <copyright file="SqlServerStringConverters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
#if MSDATA
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite.SqlServer.Converters
{
    /// <summary>
    /// Class SqlServerStringConverter.
    /// Implements the <see cref="ServiceStack.OrmLite.Converters.StringConverter" />
    /// </summary>
    /// <seealso cref="ServiceStack.OrmLite.Converters.StringConverter" />
    public class SqlServerStringConverter : StringConverter
    {
        /// <summary>
        /// Gets the maximum column definition.
        /// </summary>
        /// <value>The maximum column definition.</value>
        public override string MaxColumnDefinition => UseUnicode ? "NVARCHAR(MAX)" : "VARCHAR(MAX)";

        /// <summary>
        /// Gets the maximum length of the variable character.
        /// </summary>
        /// <value>The maximum length of the variable character.</value>
        public override int MaxVarCharLength => UseUnicode ? 4000 : 8000;

        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <param name="stringLength">Length of the string.</param>
        /// <returns>System.String.</returns>
        public override string GetColumnDefinition(int? stringLength)
        {
            if (stringLength.GetValueOrDefault() == StringLengthAttribute.MaxText)
                return MaxColumnDefinition;

            var safeLength = Math.Min(
                stringLength.GetValueOrDefault(StringLength),
                UseUnicode ? 4000 : 8000);

            return UseUnicode
                ? $"NVARCHAR({safeLength})"
                : $"VARCHAR({safeLength})";
        }

        /// <summary>
        /// Initializes the database parameter.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="fieldType">Type of the field.</param>
        public override void InitDbParam(IDbDataParameter p, Type fieldType)
        {
            base.InitDbParam(p, fieldType);

            if (!(p is SqlParameter sqlParam)) return;

            if (!UseUnicode)
            {
                sqlParam.SqlDbType = SqlDbType.VarChar;
            }
        }
    }
}