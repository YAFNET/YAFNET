// ***********************************************************************
// <copyright file="IEntityStore.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace ServiceStack.Data;

/// <summary>
/// Interface IEntityStore
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IEntityStore : IDisposable
{
    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id">The identifier.</param>
    /// <returns>T.</returns>
    T GetById<T>(object id);

    /// <summary>
    /// Gets the by ids.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ids">The ids.</param>
    /// <returns>IList&lt;T&gt;.</returns>
    IList<T> GetByIds<T>(ICollection ids);

    /// <summary>
    /// Stores the specified entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity">The entity.</param>
    /// <returns>T.</returns>
    T Store<T>(T entity);

    /// <summary>
    /// Stores all.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="entities">The entities.</param>
    void StoreAll<TEntity>(IEnumerable<TEntity> entities);

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity">The entity.</param>
    void Delete<T>(T entity);

    /// <summary>
    /// Deletes the by identifier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id">The identifier.</param>
    void DeleteById<T>(object id);

    /// <summary>
    /// Deletes the by ids.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ids">The ids.</param>
    void DeleteByIds<T>(ICollection ids);

    /// <summary>
    /// Deletes all.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    void DeleteAll<TEntity>();
}