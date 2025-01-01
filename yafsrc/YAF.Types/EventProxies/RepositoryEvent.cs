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
namespace YAF.Types.EventProxies;

/// <summary>
///     The repository event type.
/// </summary>
public enum RepositoryEventType
{
    /// <summary>
    ///     The new.
    /// </summary>
    New,

    /// <summary>
    ///     The update.
    /// </summary>
    Update,

    /// <summary>
    ///     The delete.
    /// </summary>
    Delete
}

/// <summary>
/// The repository event.
/// </summary>
/// <typeparam name="T">
/// The Typed Parameter
/// </typeparam>
public class RepositoryEvent<T> : IAmEvent
    where T : class, IEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryEvent{T}"/> class. 
    /// </summary>
    /// <param name="repositoryEventType">
    /// The repository event type. 
    /// </param>
    /// <param name="entityId">
    /// The entity id. 
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    public RepositoryEvent(RepositoryEventType repositoryEventType, int? entityId, T entity = null)
    {
        this.RepositoryEventType = repositoryEventType;
        this.EntityId = entityId;
        this.Entity = entity;
    }

    /// <summary>
    /// Gets or sets the entity.
    /// </summary>
    public T Entity { get; set; }

    /// <summary>
    ///     Gets or sets the entity id.
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    ///     Gets or sets the repository event type.
    /// </summary>
    public RepositoryEventType RepositoryEventType { get; set; }
}