﻿/* Yet Another Forum.NET
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

namespace YAF.Core.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

using YAF.Types.Extensions.Data;

/// <summary>
///     The repository extensions.
/// </summary>
public static class IRepositoryExtensions
{
    /// <summary>
    /// The delete by id.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public static bool DeleteAll<T>(this IRepository<T> repository)
        where T : class, IEntity, new()
    {
        var success = repository.DbAccess.Execute(db => db.Connection.DeleteAll<T>()) == 1;

        if (success)
        {
            repository.FireDeleted();
        }

        return success;
    }

    /// <summary>
    /// The delete.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="criteria">
    /// The criteria.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static int Delete<T>(this IRepository<T> repository, Expression<Func<T, bool>> criteria)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.Execute(db => db.Connection.Delete(criteria));
    }

    /// <summary>
    /// The delete by id.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public static bool DeleteById<T>(this IRepository<T> repository, int id)
        where T : class, IEntity, IHaveID, new()
    {
        var success = repository.DbAccess.Execute(db => db.Connection.DeleteById<T>(id)) == 1;
        if (success)
        {
            repository.FireDeleted(id);
        }

        return success;
    }

    /// <summary>
    /// Deletes all typeof `T with ids in the list
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="ids">The ids.</param>
    /// <returns>Returns if deleting was successful or not</returns>
    public static void DeleteByIds<T>(this IRepository<T> repository, IEnumerable<int> ids)
        where T : class, IEntity, IHaveID, new()
    {
        var enumerable = ids.ToList();

        repository.DbAccess.Execute(db => db.Connection.DeleteByIds<T>(enumerable));

        enumerable.ForEach(id => repository.FireDeleted(id));
    }

    /// <summary>
    /// Get a all entities by the board Id or current board id if none is specified.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// Returns all entities by the board Id or current board id if none is specified
    /// </returns>
    public static IList<T> GetByBoardId<T>(this IRepository<T> repository, int? boardId = null)
        where T : IEntity, IHaveBoardID, new()
    {
        var newBoardId = boardId ?? repository.BoardID;

        return repository.DbAccess.Execute(db => db.Connection.Where<T>(new { BoardID = newBoardId }));
    }

    /// <summary>
    /// Inserts the entity. Updates the entity with the id if successful.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    /// <param name="selectIdentity">
    /// if set to <c>true</c> [select identity].
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public static int Insert<T>(
        this IRepository<T> repository,
        T entity,
        bool selectIdentity = true)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.Execute(db => db.Connection.Insert(entity, selectIdentity)).ToType<int>();
    }

    /// <summary>
    /// Uses the most optimal approach to bulk insert multiple rows for each RDBMS provider.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="inserts">The inserts.</param>
    public static void BulkInsert<T>(this IRepository<T> repository, IEnumerable<T> inserts)
        where T : IEntity
    {
        repository.DbAccess.Execute(
            db =>
            {
                db.Connection.BulkInsert(inserts, new BulkInsertConfig { Mode = BulkInsertMode.Sql });
                return inserts.Count();
            });
    }

    /// <summary>
    /// Update or Insert entity.
    /// </summary>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public static int Upsert<T>(
        this IRepository<T> repository,
        T entity)
        where T : class, IEntity, IHaveID, new()
    {
        var newId = entity.ID;

        if (entity.ID > 0)
        {
            repository.Update(entity);

            repository.FireUpdated(entity.ID);
        }
        else
        {
            newId = repository.Insert(entity);

            repository.FireNew(newId);
        }

        return newId;
    }

    /// <summary>
    /// The update.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="entity">
    /// The entity.
    /// </param>
    /// <param name="transaction">
    /// The transaction.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public static bool Update<T>(
        this IRepository<T> repository,
        T entity)
        where T : class, IEntity, new()
    {
        var success = repository.DbAccess.Update(entity) > 0;

        return success;
    }

    /// <summary>
    /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
    /// Numeric fields generates an increment sql which is useful to increment counters, etc...
    /// avoiding concurrency conflicts
    ///   db.UpdateAdd(() =&gt; new Person { Age = 5 }, where: p =&gt; p.LastName == "Hendrix");
    ///   UPDATE "Person" SET "Age" = "Age" + 5 WHERE ("LastName" = 'Hendrix')
    ///   db.UpdateAdd(() =&gt; new Person { Age = 5 });
    ///   UPDATE "Person" SET "Age" = "Age" + 5
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="updateFields">
    /// The update Fields.
    /// </param>
    /// <param name="where">
    /// The where.
    /// </param>
    /// <param name="commandFilter">
    /// The command Filter.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public static int UpdateAdd<T>(
        this IRepository<T> repository,
        Expression<Func<T>> updateFields,
        Expression<Func<T, bool>> where = null,
        Action<IDbCommand> commandFilter = null)
        where T : class, IEntity, IHaveID, new()
    {
        return repository.DbAccess.UpdateAdd(updateFields, where, commandFilter);
    }

    /// <summary>
    /// Update/Insert the specified entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="where">The where.</param>
    public static void Upsert<T>(
        this IRepository<T> repository,
        T entity,
        Expression<Func<T, bool>> where)
        where T : class, IEntity
    {
        repository.DbAccess.Upsert(entity, where);
    }

    /// <summary>
    ///  Update only fields in the specified expression that matches the where condition (if any), E.g:
    ///
    ///   db.UpdateOnly(() => new Person { FirstName = "JJ" }, where: p => p.LastName == "Hendrix");
    ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
    ///
    ///   db.UpdateOnly(() => new Person { FirstName = "JJ" });
    ///   UPDATE "Person" SET "FirstName" = 'JJ'
    /// </summary>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="updateFields">The update fields.</param>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public static int UpdateOnly<T>(
        this IRepository<T> repository,
        Expression<Func<T>> updateFields,
        Expression<Func<T, bool>> where = null)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.UpdateOnly(updateFields, where);
    }

    /// <summary>
    /// Checks whether a Table Exists.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool TableExists<T>(
        this IRepository<T> repository)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.TableExists<T>();
    }

    /// <summary>
    /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
    /// <para>
    /// db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="where">
    /// The where.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool Exists<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> where = null)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.Exists(where);
    }

    /// <summary>
    /// Counts the specified criteria.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>Returns the Row Count</returns>
    public static long Count<T>(this IRepository<T> repository, Expression<Func<T, bool>> criteria)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.Execute(db => db.Connection.Count(criteria));
    }

    /// <summary>
    /// Gets a single entity by its ID.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="includeReference">
    /// Load References.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="T"/> .
    /// </returns>
    public static T GetById<T>(this IRepository<T> repository, int id, bool includeReference = false)
        where T : IEntity, IHaveID, new()
    {
        return repository.DbAccess.Execute(
            db => includeReference ? db.Connection.LoadSingleById<T>(id) : db.Connection.SingleById<T>(id));
    }

    /// <summary>
    /// Returns results using the supplied primary key ids. E.g:
    /// <para>
    /// db.SelectByIds&lt;Person&gt;(new[] { 1, 2, 3 })
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="idValues">
    /// The id Values.
    /// </param>
    /// <returns>
    /// The <see cref="List"/>.
    /// </returns>
    public static List<T> GetByIds<T>(this IRepository<T> repository, IEnumerable idValues)
        where T : IEntity, new()
    {
        return repository.DbAccess.Execute(db => db.Connection.SelectByIds<T>(idValues));
    }

    /// <summary>
    /// Gets a single entity by its ID.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The <see cref="T" /> .
    /// </returns>
    public static T GetSingle<T>(this IRepository<T> repository, Expression<Func<T, bool>> criteria)
        where T : IEntity, new()
    {
        return repository.DbAccess.Execute(db => db.Connection.Single(criteria));
    }

    /// <summary>
    /// Gets the list of entities by the criteria.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>Returns the list of entities</returns>
    public static List<T> Get<T>(this IRepository<T> repository, Expression<Func<T, bool>> criteria)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.Execute(db => db.Connection.Select(criteria));
    }

    /// <summary>
    /// Gets the list of all entities
    /// </summary>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <returns>
    /// Returns the list of entities
    /// </returns>
    public static List<T> GetAll<T>(this IRepository<T> repository)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.Execute(db => db.Connection.Select<T>());
    }

    /// <summary>
    /// Gets the paged list of entities by the criteria.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="criteria">The criteria.</param>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <returns>
    /// Returns the list of entities
    /// </returns>
    public static List<T> GetPaged<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> criteria,
        int? pageIndex = 0,
        int? pageSize = 10000000)
        where T : class, IEntity, IHaveID, new()
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<T>();

        expression.Where(criteria).OrderByDescending(item => item.ID).Page(pageIndex + 1, pageSize);

        return repository.DbAccess.Execute(db => db.Connection.Select(expression));
    }
}