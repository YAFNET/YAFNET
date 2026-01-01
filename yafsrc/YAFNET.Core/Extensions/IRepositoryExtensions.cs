/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    extension<T>(IRepository<T> repository) where T : class, IEntity, new()
    {
        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public bool DeleteAll()
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
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Delete(Expression<Func<T, bool>> criteria)
        {
            repository.FireDeleted();

            return repository.DbAccess.Execute(db => db.Connection.Delete(criteria));
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <param name="token">
        /// The cancellation token
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public Task<int> DeleteAsync(Expression<Func<T, bool>> criteria,
            CancellationToken token = default)
        {
            repository.FireDeleted();

            return repository.DbAccess.ExecuteAsync(db => db.DeleteAsync(criteria, token: token));
        }
    }

    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    extension<T>(IRepository<T> repository) where T : class, IEntity, IHaveID, new()
    {
        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public bool DeleteById(int id)
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
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public async Task<bool> DeleteByIdAsync(int id)
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
        /// <param name="ids">The ids.</param>
        /// <param name="token">the cancellation token.</param>
        /// <returns>Returns if deleting was successful or not</returns>
        public async Task DeleteByIdsAsync(IEnumerable<int> ids,
            CancellationToken token = default)
        {
            var enumerable = ids.ToList();

            await repository.DbAccess.ExecuteAsync(db => db.DeleteByIdsAsync<T>(enumerable, token: token));

            repository.FireDeleted();
        }
    }

    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    extension<T>(IRepository<T> repository) where T : IEntity, IHaveBoardID, new()
    {
        /// <summary>
        /// Get all entities by the board Id or current board id if none is specified.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// Returns all entities by the board Id or current board id if none is specified
        /// </returns>
        public IList<T> GetByBoardId(int? boardId = null)
        {
            var newBoardId = boardId ?? repository.BoardID;

            return repository.DbAccess.Execute(db => db.Connection.Where<T>(new { BoardID = newBoardId }));
        }

        /// <summary>
        /// Get all entities by the board Id or current board id if none is specified.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// Returns all entities by the board Id or current board id if none is specified
        /// </returns>
        public Task<List<T>> GetByBoardIdAsync(int? boardId = null)
        {
            var newBoardId = boardId ?? repository.BoardID;

            return repository.DbAccess.ExecuteAsync(db => db.WhereAsync<T>(new { BoardID = newBoardId }));
        }
    }

    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    extension<T>(IRepository<T> repository) where T : class, IEntity, new()
    {
        /// <summary>
        /// Inserts the entity. Updates the entity with the id if successful.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="selectIdentity">
        /// if set to <c>true</c> [select identity].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public int Insert(T entity,
            bool selectIdentity = true)
        {
            ArgumentNullException.ThrowIfNull(entity);

            repository.FireNew();

            return repository.DbAccess.Execute(db => db.Connection.Insert(entity, selectIdentity)).ToType<int>();
        }

        /// <summary>
        /// Inserts the entity. Updates the entity with the id if successful.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="selectIdentity">
        /// if set to <c>true</c> [select identity].
        /// </param>
        /// <param name="token"></param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public async Task<int> InsertAsync(T entity,
            bool selectIdentity = true,
            CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var newId = await repository.DbAccess.ExecuteAsync(db => db.InsertAsync(entity, selectIdentity, token: token));

            repository.FireNew();

            return newId.ToType<int>();
        }
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

    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    extension<T>(IRepository<T> repository) where T : class, IEntity, new()
    {
        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public bool Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var success = repository.DbAccess.Update(entity) > 0;

            repository.FireUpdated();

            return success;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="token">
        /// The cancellation token
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public Task<int> UpdateAsync(T entity,
            CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            repository.FireUpdated();

            return repository.DbAccess.UpdateAsync(entity, token);
        }
    }

    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    extension<T>(IRepository<T> repository) where T : class, IEntity, IHaveID, new()
    {
        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// Numeric fields generates an increment sql which is useful to increment counters, etc...
        /// avoiding concurrency conflicts
        ///   db.UpdateAdd(() =&gt; new Person { Age = 5 }, where: p =&gt; p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "Age" = "Age" + 5 WHERE ("LastName" = 'Hendrix')
        ///   db.UpdateAdd(() =&gt; new Person { Age = 5 });
        ///   UPDATE "Person" SET "Age" = "Age" + 5
        /// </summary>
        /// <param name="updateFields">
        /// The update Fields.
        /// </param>
        /// <param name="where">
        /// The where.
        /// </param>
        /// <param name="commandFilter">
        /// The command Filter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public int UpdateAdd(Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null)
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
        /// <param name="updateFields">
        /// The update Fields.
        /// </param>
        /// <param name="where">
        /// The where.
        /// </param>
        /// <param name="commandFilter">
        /// The command Filter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public Task<int> UpdateAddAsync(Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null,
            Action<IDbCommand> commandFilter = null)
        {
            return repository.DbAccess.UpdateAddAsync(updateFields, where, commandFilter);
        }
    }

    /// <param name="repository">The repository.</param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    extension<T>(IRepository<T> repository) where T : class, IEntity, new()
    {
        /// <summary>
        ///  Update only fields in the specified expression that matches the where condition (if any), E.g:
        /// 
        ///   db.UpdateOnly(() => new Person { FirstName = "JJ" }, where: p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// 
        ///   db.UpdateOnly(() => new Person { FirstName = "JJ" });
        ///   UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public int UpdateOnly(Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null)
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
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public Task<int> UpdateOnlyAsync(Expression<Func<T>> updateFields,
            Expression<Func<T, bool>> where = null)
        {
            return repository.DbAccess.UpdateOnlyAsync(updateFields, where);
        }

        /// <summary>
        /// Checks whether a Table Exists.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool TableExists()
        {
            return repository.DbAccess.TableExists<T>();
        }

        /// <summary>
        /// Checks whether a Table Exists.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public Task<bool> TableExistsAsync()
        {
            return repository.DbAccess.TableExistsAsync<T>();
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
        /// <para>
        /// db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))
        /// </para>
        /// </summary>
        /// <param name="where">
        /// The where.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Exists(Expression<Func<T, bool>> where = null)
        {
            return repository.DbAccess.Exists(where);
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
        /// <para>
        /// db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))
        /// </para>
        /// </summary>
        /// <param name="where">
        /// The where.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> where = null)
        {
            return repository.DbAccess.ExistsAsync(where);
        }

        /// <summary>
        /// Counts the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Returns the Row Count</returns>
        public long Count(Expression<Func<T, bool>> criteria)
        {
            return repository.DbAccess.Execute(db => db.Connection.Count(criteria));
        }

        /// <summary>
        /// Counts the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Returns the Row Count</returns>
        public Task<long> CountAsync(Expression<Func<T, bool>> criteria)
        {
            return repository.DbAccess.ExecuteAsync(db => db.CountAsync(criteria));
        }
    }

    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <typeparam name="T">
    /// The type parameter.
    /// </typeparam>
    extension<T>(IRepository<T> repository) where T : IEntity, IHaveID, new()
    {
        /// <summary>
        /// Gets a single entity by its ID.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="includeReference">
        /// Load References.
        /// </param>
        /// <returns>
        /// The <see cref="T"/> .
        /// </returns>
        public T GetById(int id, bool includeReference = false)
        {
            return repository.DbAccess.Execute(
                db => includeReference ? db.Connection.LoadSingleById<T>(id) : db.Connection.SingleById<T>(id));
        }

        /// <summary>
        /// Gets a single entity by its ID.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="includeReference">
        /// Load References.
        /// </param>
        /// <returns>
        /// The <see cref="T"/> .
        /// </returns>
        public Task<T> GetByIdAsync(int id, bool includeReference = false)
        {
            return repository.DbAccess.ExecuteAsync(
                db => includeReference ? db.LoadSingleByIdAsync<T>(id) : db.SingleByIdAsync<T>(id));
        }
    }

    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    extension<T>(IRepository<T> repository) where T : IEntity, new()
    {
        /// <summary>
        /// Returns results using the supplied primary key ids. E.g:
        /// <para>
        /// db.SelectByIds&lt;Person&gt;(new[] { 1, 2, 3 })
        /// </para>
        /// </summary>
        /// <param name="idValues">
        /// The id Values.
        /// </param>
        public List<T> GetByIds(IEnumerable idValues)
        {
            return repository.DbAccess.Execute(db => db.Connection.SelectByIds<T>(idValues));
        }

        /// <summary>
        /// Gets a single entity by its ID.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// The <see cref="T" /> .
        /// </returns>
        public T GetSingle(Expression<Func<T, bool>> criteria)
        {
            return repository.DbAccess.Execute(db => db.Connection.Single(criteria));
        }

        /// <summary>
        /// Gets a single entity by its ID.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="cancellationToken">The Cancellation token</param>
        /// <returns>
        /// The <see cref="T" /> .
        /// </returns>
        public Task<T> GetSingleAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return repository.DbAccess.ExecuteAsync(db => db.SingleAsync(criteria, token: cancellationToken));
        }
    }

    /// <param name="repository">The repository.</param>
    /// <typeparam name="T">The type parameter.</typeparam>
    extension<T>(IRepository<T> repository) where T : class, IEntity, new()
    {
        /// <summary>
        /// Gets the list of entities by the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Returns the list of entities</returns>
        public List<T> Get(Expression<Func<T, bool>> criteria)
        {
            ArgumentNullException.ThrowIfNull(criteria);

            return repository.DbAccess.Execute(db => db.Connection.Select(criteria));
        }

        /// <summary>
        /// Gets the list of entities by the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="cancellationToken">The Cancellation token</param>
        /// <returns>Returns the list of entities</returns>
        public Task<List<T>> GetAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(criteria);

            return repository.DbAccess.ExecuteAsync(db => db.SelectAsync(criteria, token: cancellationToken));
        }

        /// <summary>
        /// Gets the list of all entities
        /// </summary>
        /// <returns>
        /// Returns the list of entities
        /// </returns>
        public List<T> GetAll()
        {
            return repository.DbAccess.Execute(db => db.Connection.Select<T>());
        }

        /// <summary>
        /// Gets the list of all entities
        /// </summary>
        /// <returns>
        /// Returns the list of entities
        /// </returns>
        public Task<List<T>> GetAllAsync()
        {
            return repository.DbAccess.ExecuteAsync(db => db.SelectAsync<T>());
        }
    }

    /// <param name="repository">The repository.</param>
    /// <typeparam name="T">The type parameter.</typeparam>
    extension<T>(IRepository<T> repository) where T : class, IEntity, IHaveID, new()
    {
        /// <summary>
        /// Gets the paged list of entities by the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// Returns the list of entities
        /// </returns>
        public List<T> GetPaged(Expression<Func<T, bool>> criteria,
            int? pageIndex = 0,
            int? pageSize = 10000000)
        {
            ArgumentNullException.ThrowIfNull(criteria);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<T>();

            expression.Where(criteria).OrderByDescending(item => item.ID).Page(pageIndex + 1, pageSize);

            return repository.DbAccess.Execute(db => db.Connection.Select(expression));
        }

        /// <summary>
        /// Gets the paged list of entities by the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// Returns the list of entities
        /// </returns>
        public Task<List<T>> GetPagedAsync(Expression<Func<T, bool>> criteria,
            int? pageIndex = 0,
            int? pageSize = 10000000)
        {
            ArgumentNullException.ThrowIfNull(criteria);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<T>();

            expression.Where(criteria).OrderByDescending(item => item.ID).Page(pageIndex + 1, pageSize);

            return repository.DbAccess.ExecuteAsync(db => db.SelectAsync(expression));
        }
    }
}