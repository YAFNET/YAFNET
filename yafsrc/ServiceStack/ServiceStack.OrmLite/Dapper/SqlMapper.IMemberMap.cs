// ***********************************************************************
// <copyright file="SqlMapper.IMemberMap.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Reflection;

namespace ServiceStack.OrmLite.Dapper
{
    /// <summary>
    /// Class SqlMapper.
    /// </summary>
    public static partial class SqlMapper
    {
        /// <summary>
        /// Implements this interface to provide custom member mapping
        /// </summary>
        public interface IMemberMap
        {
            /// <summary>
            /// Source DataReader column name
            /// </summary>
            /// <value>The name of the column.</value>
            string ColumnName { get; }

            /// <summary>
            /// Target member type
            /// </summary>
            /// <value>The type of the member.</value>
            Type MemberType { get; }

            /// <summary>
            /// Target property
            /// </summary>
            /// <value>The property.</value>
            PropertyInfo Property { get; }

            /// <summary>
            /// Target field
            /// </summary>
            /// <value>The field.</value>
            FieldInfo Field { get; }

            /// <summary>
            /// Target constructor parameter
            /// </summary>
            /// <value>The parameter.</value>
            ParameterInfo Parameter { get; }
        }
    }
}
