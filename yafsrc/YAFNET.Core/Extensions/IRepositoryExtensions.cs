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

namespace YAF.Core.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using YAF.Types.Interfaces.Data;

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
        repository.FireDeleted();

        return repository.DbAccess.Execute(db => db.Connection.Delete(criteria));
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
    /// <param name="token">
    /// The cancellation token
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static Task<int> DeleteAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> criteria,
                                           CancellationToken token = default)
        where T : class, IEntity, new()
    {
        repository.FireDeleted();

        return repository.DbAccess.ExecuteAsync(db => db.DeleteAsync(criteria, token: token));
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
            repository.FireDeleted();
        }

        return success;
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
    public async static Task<bool> DeleteByIdAsync<T>(this IRepository<T> repository, int id)
        where T : class, IEntity, IHaveID, new()
    {
        var success = await repository.DbAccess.ExecuteAsync(db => db.DeleteByIdAsync<T>(id)) == 1;
        if (success)
        {
            repository.FireDeleted();
        }

        return success;
    }

    /// <summary>
    /// Deletes all typeof `T with ids in the list
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="ids">The ids.</param>
    /// <param name="token">the cancellation token.</param>
    /// <returns>Returns if deleting was successful or not</returns>
    public async static Task DeleteByIdsAsync<T>(this IRepository<T> repository, IEnumerable<int> ids,
        CancellationToken token = default)
        where T : class, IEntity, IHaveID, new()
    {
        var enumerable = ids.ToList();

        await repository.DbAccess.ExecuteAsync(db => db.DeleteByIdsAsync<T>(enumerable, token: token));

        repository.FireDeleted();
    }

    /// <summary>
    /// Get all entities by the board Id or current board id if none is specified.
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
    /// Get all entities by the board Id or current board id if none is specified.
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
    public static Task<List<T>> GetByBoardIdAsync<T>(this IRepository<T> repository, int? boardId = null)
        where T : IEntity, IHaveBoardID, new()
    {
        var newBoardId = boardId ?? repository.BoardID;

        return repository.DbAccess.ExecuteAsync(db => db.WhereAsync<T>(new { BoardID = newBoardId }));
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
        ArgumentNullException.ThrowIfNull(entity);

        repository.FireNew();

        return repository.DbAccess.Execute(db => db.Connection.Insert(entity, selectIdentity)).ToType<int>();
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
    /// <param name="token"></param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public async static Task<int> InsertAsync<T>(
        this IRepository<T> repository,
        T entity,
        bool selectIdentity = true,
        CancellationToken token = default)
        where T : class, IEntity, new()
    {
        ArgumentNullException.ThrowIfNull(entity);

        var newId = await repository.DbAccess.ExecuteAsync(db => db.InsertAsync(entity, selectIdentity, token: token));

        repository.FireNew();

        return newId.ToType<int>();
    }

    /// <summary>
    /// Uses the most optimal approach to bulk insert multiple rows for each RDBMS provider.
    /// </summary>
    /// <typeparam name="T">The Model.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="inserts">The inserts.</param>
    public static void BulkInsert<T>(this IRepository<T> repository, IEnumerable<T> inserts)
        where T : IEntity
    {
        var insertList = inserts.ToList();

        repository.DbAccess.Execute(
            db =>
            {
                db.Connection.BulkInsert(insertList, new BulkInsertConfig
                {
                    Mode = BulkInsertMode.Sql
                });
                return insertList.Count;
            });
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
        ArgumentNullException.ThrowIfNull(entity);

        repository.DbAccess.Upsert(entity, where);
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
    public async static Task<int> UpsertAsync<T>(
        this IRepository<T> repository,
        T entity)
        where T : class, IEntity, IHaveID, new()
    {
        ArgumentNullException.ThrowIfNull(entity);

        var newId = entity.ID;

        if (entity.ID > 0)
        {
            await repository.UpdateAsync(entity);

            repository.FireUpdated();
        }
        else
        {
            newId = await repository.InsertAsync(entity);

            repository.FireNew();
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
        ArgumentNullException.ThrowIfNull(entity);

        var success = repository.DbAccess.Update(entity) > 0;

        repository.FireUpdated();

        return success;
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
    /// <param name="token">
    /// The cancellation token
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    /// <returns>
    /// The <see cref="bool"/> .
    /// </returns>
    public static Task<int> UpdateAsync<T>(
        this IRepository<T> repository,
        T entity,
        CancellationToken token = default)
        where T : class, IEntity, new()
    {
        ArgumentNullException.ThrowIfNull(entity);

        repository.FireUpdated();

        return repository.DbAccess.UpdateAsync(entity, token);
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
    public static Task<int> UpdateAddAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T>> updateFields,
        Expression<Func<T, bool>> where = null,
        Action<IDbCommand> commandFilter = null)
        where T : class, IEntity, IHaveID, new()
    {
        return repository.DbAccess.UpdateAddAsync(updateFields, where, commandFilter);
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
    public static Task<int> UpdateOnlyAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T>> updateFields,
        Expression<Func<T, bool>> where = null)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.UpdateOnlyAsync(updateFields, where);
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
    public static Task<bool> TableExistsAsync<T>(
        this IRepository<T> repository)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.TableExistsAsync<T>();
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
    public static Task<bool> ExistsAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> where = null)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.ExistsAsync(where);
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
    /// Counts the specified criteria.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>Returns the Row Count</returns>
    public static Task<long> CountAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> criteria)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.ExecuteAsync(db => db.CountAsync(criteria));
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
    public static Task<T> GetByIdAsync<T>(this IRepository<T> repository, int id, bool includeReference = false)
        where T : IEntity, IHaveID, new()
    {
        return repository.DbAccess.ExecuteAsync(
            db => includeReference ? db.LoadSingleByIdAsync<T>(id) : db.SingleByIdAsync<T>(id));
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
    /// Gets a single entity by its ID.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="criteria">The criteria.</param>
    /// <param name="cancellationToken">The Cancellation token</param>
    /// <returns>
    /// The <see cref="T" /> .
    /// </returns>
    public static Task<T> GetSingleAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        where T : IEntity, new()
    {
        return repository.DbAccess.ExecuteAsync(db => db.SingleAsync(criteria, token: cancellationToken));
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
        ArgumentNullException.ThrowIfNull(criteria);

        return repository.DbAccess.Execute(db => db.Connection.Select(criteria));
    }

    /// <summary>
    /// Gets the list of entities by the criteria.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="repository">The repository.</param>
    /// <param name="criteria">The criteria.</param>
    /// <param name="cancellationToken">The Cancellation token</param>
    /// <returns>Returns the list of entities</returns>
    public static Task<List<T>> GetAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        where T : class, IEntity, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);

        return repository.DbAccess.ExecuteAsync(db => db.SelectAsync(criteria, token: cancellationToken));
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
    public static Task<List<T>> GetAllAsync<T>(this IRepository<T> repository)
        where T : class, IEntity, new()
    {
        return repository.DbAccess.ExecuteAsync(db => db.SelectAsync<T>());
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
        ArgumentNullException.ThrowIfNull(criteria);

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<T>();

        expression.Where(criteria).OrderByDescending(item => item.ID).Page(pageIndex + 1, pageSize);

        return repository.DbAccess.Execute(db => db.Connection.Select(expression));
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
    public static Task<List<T>> GetPagedAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> criteria,
        int? pageIndex = 0,
        int? pageSize = 10000000)
        where T : class, IEntity, IHaveID, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<T>();

        expression.Where(criteria).OrderByDescending(item => item.ID).Page(pageIndex + 1, pageSize);

        return repository.DbAccess.ExecuteAsync(db => db.SelectAsync(expression));
    }
}