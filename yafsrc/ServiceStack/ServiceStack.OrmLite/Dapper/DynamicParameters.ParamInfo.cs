// ***********************************************************************
// <copyright file="DynamicParameters.ParamInfo.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Class DynamicParameters.
/// Implements the <see cref="ServiceStack.OrmLite.Dapper.SqlMapper.IDynamicParameters" />
/// Implements the <see cref="ServiceStack.OrmLite.Dapper.SqlMapper.IParameterLookup" />
/// Implements the <see cref="ServiceStack.OrmLite.Dapper.SqlMapper.IParameterCallbacks" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.Dapper.SqlMapper.IDynamicParameters" />
/// <seealso cref="ServiceStack.OrmLite.Dapper.SqlMapper.IParameterLookup" />
/// <seealso cref="ServiceStack.OrmLite.Dapper.SqlMapper.IParameterCallbacks" />
public partial class DynamicParameters
{
    /// <summary>
    /// Class ParamInfo. This class cannot be inherited.
    /// </summary>
    private sealed class ParamInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }
        /// <summary>
        /// Gets or sets the parameter direction.
        /// </summary>
        /// <value>The parameter direction.</value>
        public ParameterDirection ParameterDirection { get; set; }
        /// <summary>
        /// Gets or sets the type of the database.
        /// </summary>
        /// <value>The type of the database.</value>
        public DbType? DbType { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int? Size { get; set; }
        /// <summary>
        /// Gets or sets the attached parameter.
        /// </summary>
        /// <value>The attached parameter.</value>
        public IDbDataParameter AttachedParam { get; set; }
        /// <summary>
        /// Gets or sets the output callback.
        /// </summary>
        /// <value>The output callback.</value>
        internal Action<object, DynamicParameters> OutputCallback { get; set; }
        /// <summary>
        /// Gets or sets the output target.
        /// </summary>
        /// <value>The output target.</value>
        internal object OutputTarget { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [came from template].
        /// </summary>
        /// <value><c>true</c> if [came from template]; otherwise, <c>false</c>.</value>
        internal bool CameFromTemplate { get; set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>The precision.</value>
        public byte? Precision { get; set; }
        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public byte? Scale { get; set; }
    }
}