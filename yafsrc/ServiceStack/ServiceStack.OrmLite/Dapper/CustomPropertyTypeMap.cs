// ***********************************************************************
// <copyright file="CustomPropertyTypeMap.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Reflection;

namespace ServiceStack.OrmLite.Dapper
{
    /// <summary>
    /// Implements custom property mapping by user provided criteria (usually presence of some custom attribute with column to member mapping)
    /// </summary>
    public sealed class CustomPropertyTypeMap : SqlMapper.ITypeMap
    {
        /// <summary>
        /// The type
        /// </summary>
        private readonly Type _type;
        /// <summary>
        /// The property selector
        /// </summary>
        private readonly Func<Type, string, PropertyInfo> _propertySelector;

        /// <summary>
        /// Creates custom property mapping
        /// </summary>
        /// <param name="type">Target entity type</param>
        /// <param name="propertySelector">Property selector based on target type and DataReader column name</param>
        /// <exception cref="System.ArgumentNullException">type</exception>
        /// <exception cref="System.ArgumentNullException">propertySelector</exception>
        public CustomPropertyTypeMap(Type type, Func<Type, string, PropertyInfo> propertySelector)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _propertySelector = propertySelector ?? throw new ArgumentNullException(nameof(propertySelector));
        }

        /// <summary>
        /// Always returns default constructor
        /// </summary>
        /// <param name="names">DataReader column names</param>
        /// <param name="types">DataReader column types</param>
        /// <returns>Default constructor</returns>
        public ConstructorInfo FindConstructor(string[] names, Type[] types) =>
            _type.GetConstructor(Type.EmptyTypes);

        /// <summary>
        /// Always returns null
        /// </summary>
        /// <returns>ConstructorInfo.</returns>
        public ConstructorInfo FindExplicitConstructor() => null;

        /// <summary>
        /// Not implemented as far as default constructor used for all cases
        /// </summary>
        /// <param name="constructor">Constructor to resolve</param>
        /// <param name="columnName">DataReader column name</param>
        /// <returns>Mapping implementation</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns property based on selector strategy
        /// </summary>
        /// <param name="columnName">DataReader column name</param>
        /// <returns>Poperty member map</returns>
        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            var prop = _propertySelector(_type, columnName);
            return prop != null ? new SimpleMemberMap(columnName, prop) : null;
        }
    }
}
