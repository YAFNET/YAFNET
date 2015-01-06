/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
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
    using System.Collections.Generic;
    using System.Data;

    using ServiceStack.OrmLite;

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

            return repository.DeleteByID(haveId.ID);
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
        public static bool DeleteByID<T>([NotNull] this IRepository<T> repository, int id) where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = repository.DbAccess.Execute(db => db.Connection.Delete<T>(x => x.ID == id)) == 1;
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
        /// <returns></returns>
        public static bool DeleteByIDs<T>([NotNull] this IRepository<T> repository, IEnumerable<int> ids) where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = false;

            ids.ForEach(id =>
            {
                success = repository.DbAccess.Execute(db => db.Connection.Delete<T>(x => x.ID == id)) == 1;
            });

            if (success)
            {
                ids.ForEach(id => repository.FireDeleted(id));
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
        /// The <see cref="IList"/> . 
        /// </returns>
        public static IList<T> GetByBoardID<T>([NotNull] this IRepository<T> repository, int? boardId = null) where T : IEntity, IHaveBoardID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var newboardID = boardId ?? repository.BoardID;

            return repository.DbAccess.Execute(db => db.Connection.Where<T>(new { BoardID = newboardID }));
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
        public static T GetByID<T>([NotNull] this IRepository<T> repository, int id) where T : IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbAccess.Execute(db => db.Connection.SingleById<T>(id));
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
        public static bool Insert<T>([NotNull] this IRepository<T> repository, [NotNull] T entity, IDbTransaction transaction = null)
            where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(entity, "entity");
            CodeContracts.VerifyNotNull(repository, "repository");

            var insertId = repository.DbAccess.Insert(entity, transaction, true).ToType<int>();

            if (insertId <= 0)
            {
                return false;
            }

            entity.ID = insertId;
            repository.FireNew(insertId, entity);

            return true;
        }

        /// <summary>
        /// Update or Insert entity.
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
        public static bool Upsert<T>([NotNull] this IRepository<T> repository, [NotNull] T entity, IDbTransaction transaction = null)
            where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(entity, "entity");
            CodeContracts.VerifyNotNull(repository, "repository");

            return entity.ID > 0 ? repository.Update(entity) : repository.Insert(entity);
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

        #endregion
    }
}