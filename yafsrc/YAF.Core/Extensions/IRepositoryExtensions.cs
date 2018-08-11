/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using ServiceStack;
    using ServiceStack.OrmLite;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     The repository extensions.
    /// </summary>
    public static class IRepositoryExtensions
    {
        #region Public Methods and Operators

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
        public static bool DeleteAll<T>([NotNull] this IRepository<T> repository) where T : class, IEntity, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

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
        /// <param name="haveId">
        /// The have id.
        /// </param>
        /// <typeparam name="T">
        /// The type parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Delete<T>([NotNull] this IRepository<T> repository, [NotNull] IHaveID haveId) where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(haveId, "haveId");
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DeleteById(haveId.ID);
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
        public static bool DeleteById<T>([NotNull] this IRepository<T> repository, int id) where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

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
        /// <returns>Returns if deleting was succesfull or not</returns>
        public static bool DeleteByIds<T>([NotNull] this IRepository<T> repository, IEnumerable<int> ids) where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = false;

            var enumerable = ids.ToList();

            enumerable.ForEach(id =>
            {
                success = repository.DbAccess.Execute(db => db.Connection.Delete<T>(x => x.ID == id)) == 1;
            });

            if (success)
            {
                enumerable.ForEach(id => repository.FireDeleted(id));
            }

            return success;
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
        public static IList<T> GetByBoardId<T>([NotNull] this IRepository<T> repository, int? boardId = null) where T : IEntity, IHaveBoardID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var newboardId = boardId ?? repository.BoardID;

            return repository.DbAccess.Execute(db => db.Connection.Where<T>(new { BoardID = newboardId }));
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
        /// <param name="transaction">
        /// The transaction. 
        /// </param>
        /// <typeparam name="T">
        /// The type parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/> . 
        /// </returns>
        public static int Insert<T>([NotNull] this IRepository<T> repository, [NotNull] T entity, IDbTransaction transaction = null)
            where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(entity, "entity");
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbAccess.Execute(db => db.Connection.Insert(entity, true)).ToType<int>();
        }

        /// <summary>
        /// Update or Insert entity.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        /// The <see cref="bool" /> .
        /// </returns>
        public static bool Upsert<T>([NotNull] this IRepository<T> repository, [NotNull] T entity, IDbTransaction transaction = null)
            where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(entity, "entity");
            CodeContracts.VerifyNotNull(repository, "repository");

            return entity.ID > 0 ? repository.Update(entity) : repository.Insert(entity) > 0;
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
        public static bool Update<T>([NotNull] this IRepository<T> repository, [NotNull] T entity, IDbTransaction transaction = null)
            where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(entity, "entity");
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = repository.DbAccess.Update(entity, transaction) > 0;
            if (success)
            {
                repository.FireUpdated(entity.ID, entity);
            }

            return success;
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// Numeric fields generates an increment sql which is useful to increment counters, etc...
        /// avoiding concurrency conflicts
        /// 
        ///   db.UpdateAdd(() => new Person { Age = 5 }, where: p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "Age" = "Age" + 5 WHERE ("LastName" = 'Hendrix')
        /// 
        ///   db.UpdateAdd(() => new Person { Age = 5 });
        ///   UPDATE "Person" SET "Age" = "Age" + 5
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
        public static int UpdateAdd<T>([NotNull] this IRepository<T> repository, Expression<Func<T>> updateFields,
                                        Expression<Func<T, bool>> where = null,
                                        Action<IDbCommand> commandFilter = null)
            where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbAccess.UpdateAdd(updateFields, where, commandFilter);
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
        /// <typeparam name="T"></typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="updateFields">The update fields.</param>
        /// <param name="where">The where.</param>
        /// <param name="commandFilter">The command filter.</param>
        /// <returns></returns>
        public static int UpdateOnly<T>([NotNull] this IRepository<T> repository, Expression<Func<T>> updateFields,
                                         Expression<Func<T, bool>> where = null,
                                         Action<IDbCommand> commandFilter = null)
            where T : class, IEntity, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbAccess.UpdateOnly(updateFields, where, commandFilter);
        }

        /// <summary>
        /// Counts the specified criteria.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Returns the Row Count</returns>
        public static long Count<T>([NotNull] this IRepository<T> repository, Expression<Func<T, bool>> criteria)
            where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");
            CodeContracts.VerifyNotNull(criteria, "criteria");

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
        /// <typeparam name="T">
        /// The type parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/> . 
        /// </returns>
        public static T GetById<T>([NotNull] this IRepository<T> repository, int id) where T : IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbAccess.Execute(db => db.Connection.SingleById<T>(id));
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
        public static T GetSingle<T>([NotNull] this IRepository<T> repository, Expression<Func<T, bool>> criteria) where T : IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbAccess.Execute(db => db.Connection.Single<T>(criteria));
        }

        /// <summary>
        /// Gets the list of entities by the criteria.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Returns the list of entities</returns>
        public static List<T> Get<T>([NotNull] this IRepository<T> repository, Expression<Func<T, bool>> criteria)
            where T : class, IEntity, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");
            CodeContracts.VerifyNotNull(criteria, "criteria");

            return repository.DbAccess.Execute(db => db.Connection.Select<T>(criteria));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="anonType">Type of the anon.</param>
        /// <returns>Returns results from an arbitrary parameterized raw sql query</returns>
        public static List<T> SqlList<T>([NotNull] this IRepository<T> repository, string sql, object anonType)
        {
            return repository.DbAccess.Execute(
                db => db.Connection.SqlList<T>(
                    string.Format("{0}{1}", Config.DatabaseObjectQualifier, sql),
                    cmd =>
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            foreach (var p in anonType.ToObjectDictionary())
                            {
                                cmd.AddParam(p.Key, p.Value);
                            }
                        }));
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
        public static List<T> GetPaged<T>([NotNull] this IRepository<T> repository, Expression<Func<T, bool>> criteria, int? pageIndex = 0, int? pageSize = 10000000)
            where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");
            CodeContracts.VerifyNotNull(criteria, "criteria");

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<T>();

            expression.Where(criteria).OrderByDescending(item => item.ID).Page(pageIndex + 1, pageSize);

            return repository.DbAccess.Execute(db => db.Connection.Select<T>(expression));
        }

        #endregion
    }
}