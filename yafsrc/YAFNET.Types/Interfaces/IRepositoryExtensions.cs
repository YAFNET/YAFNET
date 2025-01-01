/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Types.Interfaces;

using YAF.Types.EventProxies;

/// <summary>
///     The repository extensions.
/// </summary>
public static class IRepositoryExtensions
{
    /// <summary>
    /// The fire deleted.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    /// <typeparam name="T">
    /// The Typed Parameter
    /// </typeparam>
    public static void FireDeleted<T>(this IRepository<T> repository, int? id = null, T entity = null) where T : class, IEntity
    {
        repository.DbEvent.Raise(new RepositoryEvent<T>(RepositoryEventType.Delete, id, entity));
    }

    /// <summary>
    /// The fire deleted.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    /// <typeparam name="T">
    /// The Typed Parameter
    /// </typeparam>
    public static void FireDeleted<T>(this IRepository<T> repository, T entity) where T : class, IEntity, IHaveID
    {
        repository.DbEvent.Raise(new RepositoryEvent<T>(RepositoryEventType.Delete, entity.ID, entity));
    }

    /// <summary>
    /// The fire new.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    /// <typeparam name="T">
    /// The Typed Parameter
    /// </typeparam>
    public static void FireNew<T>(this IRepository<T> repository, int? id = null, T entity = null) where T : class, IEntity
    {
        repository.DbEvent.Raise(new RepositoryEvent<T>(RepositoryEventType.New, id, entity));
    }

    /// <summary>
    /// The fire new.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    /// <typeparam name="T">
    /// The Typed Parameter
    /// </typeparam>
    public static void FireNew<T>(this IRepository<T> repository, T entity) where T : class, IEntity, IHaveID
    {
        repository.DbEvent.Raise(new RepositoryEvent<T>(RepositoryEventType.New, entity.ID, entity));
    }

    /// <summary>
    /// The fire updated.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    /// <typeparam name="T">
    /// The Typed Parameter
    /// </typeparam>
    public static void FireUpdated<T>(this IRepository<T> repository, int? id = null, T entity = null) where T : class, IEntity
    {
        repository.DbEvent.Raise(new RepositoryEvent<T>(RepositoryEventType.Update, id, entity));
    }

    /// <summary>
    /// The fire updated.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    /// <typeparam name="T">
    /// The Typed Parameter
    /// </typeparam>
    public static void FireUpdated<T>(this IRepository<T> repository, T entity) where T : class, IEntity, IHaveID
    {
        repository.DbEvent.Raise(new RepositoryEvent<T>(RepositoryEventType.Update, entity.ID, entity));
    }
}