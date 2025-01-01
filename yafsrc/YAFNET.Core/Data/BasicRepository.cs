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

namespace YAF.Core.Data;

/// <summary>
/// The basic repository.
/// </summary>
/// <typeparam name="T">
/// </typeparam>
public class BasicRepository<T> : IRepository<T>
    where T : IEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicRepository{T}"/> class.
    /// </summary>
    /// <param name="dbAccess">
    /// The database Access.
    /// </param>
    /// <param name="raiseEvent">
    /// The raise Event.
    /// </param>
    /// <param name="haveBoardId">
    /// The have Board Id.
    /// </param>
    public BasicRepository(
        IDbAccess dbAccess,
        IRaiseEvent raiseEvent,
        IHaveBoardID haveBoardId)
    {
        this.DbAccess = dbAccess;
        this.DbEvent = raiseEvent;
        this.BoardID = haveBoardId.BoardID;
    }

    /// <summary>
    ///     Gets or sets the board id.
    /// </summary>
    public int BoardID { get; set; }

    /// <summary>
    /// Gets the database access.
    /// </summary>
    public IDbAccess DbAccess { get; }

    /// <summary>
    ///     Gets the database event.
    /// </summary>
    public IRaiseEvent DbEvent { get; }
}