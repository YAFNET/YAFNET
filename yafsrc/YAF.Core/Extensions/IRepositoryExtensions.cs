/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
    ///     The i repository extensions.
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
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/> . 
        /// </returns>
        public static bool DeleteByID<T>([NotNull] this IRepository<T> repository, int id) where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = repository.DbAccess.Execute(db => db.Delete<T>(x => x.ID == id)) == 1;
            if (success)
            {
                repository.FireDeleted(id);
            }

            return success;
        }

        /// <summary>
        /// Deletes all typeof `T with ids in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static bool DeleteByIDs<T>([NotNull] this IRepository<T> repository, IEnumerable<int> ids) where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = false;

            ids.ForEach(id =>
            {
                success = repository.DbAccess.Execute(db => db.Delete<T>(x => x.ID == id)) == 1;
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
        /// </typeparam>
        /// <returns>
        /// The <see cref="IList"/> . 
        /// </returns>
        public static IList<T> GetByBoardID<T>([NotNull] this IRepository<T> repository, int? boardId = null) where T : IEntity, IHaveBoardID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            int bId = boardId ?? repository.BoardID;

            return repository.DbAccess.Execute(db => db.Where<T>(new { BoardID = bId }));
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
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/> . 
        /// </returns>
        public static T GetByID<T>([NotNull] this IRepository<T> repository, int id) where T : IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbAccess.Execute(db => db.GetById<T>(id));
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
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/> . 
        /// </returns>
        public static bool Insert<T>([NotNull] this IRepository<T> repository, [NotNull] T entity, IDbTransaction transaction = null)
            where T : class, IEntity, IHaveID, new()
        {
            CodeContracts.VerifyNotNull(entity, "entity");
            CodeContracts.VerifyNotNull(repository, "repository");

            var insertId = repository.DbAccess.Insert(entity, transaction);
            
            if (insertId > 0)
            {
                entity.ID = insertId;
                repository.FireNew(insertId, entity);

                return true;
            }

            return false;
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