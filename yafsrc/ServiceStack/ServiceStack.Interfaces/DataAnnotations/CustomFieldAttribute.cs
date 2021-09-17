// ***********************************************************************
// <copyright file="CustomFieldAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack.DataAnnotations
{
    /// <summary>
    /// Class CustomFieldAttribute.
    /// Implements the <see cref="ServiceStack.AttributeBase" />
    /// </summary>
    /// <seealso cref="ServiceStack.AttributeBase" />
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomFieldAttribute : AttributeBase
    {
        /// <summary>
        /// Gets or sets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        public string Sql { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldAttribute"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public CustomFieldAttribute(string sql)
        {
            Sql = sql;
        }
    }

    /// <summary>
    /// Class PgSqlJsonAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlJsonAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlJsonAttribute"/> class.
        /// </summary>
        public PgSqlJsonAttribute() : base("json") { }
    }

    /// <summary>
    /// Class PgSqlJsonBAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlJsonBAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlJsonBAttribute"/> class.
        /// </summary>
        public PgSqlJsonBAttribute() : base("jsonb") { }
    }

    /// <summary>
    /// Class PgSqlHStoreAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlHStoreAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlHStoreAttribute"/> class.
        /// </summary>
        public PgSqlHStoreAttribute() : base("hstore") { }
    }

    /// <summary>
    /// Class PgSqlTextArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlTextArrayAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlTextArrayAttribute"/> class.
        /// </summary>
        public PgSqlTextArrayAttribute() : base("text[]") { }
    }

    /// <summary>
    /// Class PgSqlShortArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlShortArrayAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlShortArrayAttribute"/> class.
        /// </summary>
        public PgSqlShortArrayAttribute() : base("short[]") { }
    }

    /// <summary>
    /// Class PgSqlIntArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlIntArrayAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlIntArrayAttribute"/> class.
        /// </summary>
        public PgSqlIntArrayAttribute() : base("integer[]") { }
    }

    /// <summary>
    /// Class PgSqlBigIntArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.PgSqlLongArrayAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.PgSqlLongArrayAttribute" />
    public class PgSqlBigIntArrayAttribute : PgSqlLongArrayAttribute { }
    /// <summary>
    /// Class PgSqlLongArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlLongArrayAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlLongArrayAttribute"/> class.
        /// </summary>
        public PgSqlLongArrayAttribute() : base("bigint[]") { }
    }

    /// <summary>
    /// Class PgSqlFloatArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlFloatArrayAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlFloatArrayAttribute"/> class.
        /// </summary>
        public PgSqlFloatArrayAttribute() : base("real[]") { }
    }

    /// <summary>
    /// Class PgSqlDoubleArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlDoubleArrayAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlDoubleArrayAttribute"/> class.
        /// </summary>
        public PgSqlDoubleArrayAttribute() : base("double precision[]") { }
    }

    /// <summary>
    /// Class PgSqlDecimalArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlDecimalArrayAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlDecimalArrayAttribute"/> class.
        /// </summary>
        public PgSqlDecimalArrayAttribute() : base("numeric[]") { }
    }

    /// <summary>
    /// Class PgSqlTimestampArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlTimestampArrayAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlTimestampArrayAttribute"/> class.
        /// </summary>
        public PgSqlTimestampArrayAttribute() : base("timestamp[]") { }
    }

    /// <summary>
    /// Class PgSqlTimestampTzArrayAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    public class PgSqlTimestampTzArrayAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlTimestampTzArrayAttribute"/> class.
        /// </summary>
        public PgSqlTimestampTzArrayAttribute() : base("timestamp with time zone[]") { }
    }

    /// <summary>
    /// Class PgSqlTimestampAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    [Obsolete("Use [PgSqlTimestampArray]")]
    public class PgSqlTimestampAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlTimestampAttribute"/> class.
        /// </summary>
        public PgSqlTimestampAttribute() : base("timestamp[]") { }
    }

    /// <summary>
    /// Class PgSqlTimestampTzAttribute.
    /// Implements the <see cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    /// </summary>
    /// <seealso cref="ServiceStack.DataAnnotations.CustomFieldAttribute" />
    [Obsolete("Use [PgSqlTimestampTzArray]")]
    public class PgSqlTimestampTzAttribute : CustomFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PgSqlTimestampTzAttribute"/> class.
        /// </summary>
        public PgSqlTimestampTzAttribute() : base("timestamp with time zone[]") { }
    }

}