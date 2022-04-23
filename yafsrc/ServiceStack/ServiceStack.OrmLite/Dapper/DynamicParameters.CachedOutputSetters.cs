// ***********************************************************************
// <copyright file="DynamicParameters.CachedOutputSetters.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections;

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
    // The type here is used to differentiate the cache by type via generics
    // ReSharper disable once UnusedTypeParameter
    /// <summary>
    /// Class CachedOutputSetters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class CachedOutputSetters<T>
    {
        // Intentional, abusing generics to get our cache splits
        // ReSharper disable once StaticMemberInGenericType
        /// <summary>
        /// The cache
        /// </summary>
        public static readonly Hashtable Cache = new Hashtable();
    }
}