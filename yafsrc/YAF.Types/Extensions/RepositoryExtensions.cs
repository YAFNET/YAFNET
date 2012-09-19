/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
    #region Using

    using YAF.Types;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    /// The repository extensions.
    /// </summary>
    public static class RepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The fire deleted.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void FireDeleted<T>(this IRepository<T> repository, int? id = null)
            where T : IEntity
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            repository.DbEvent.Raise(new RepositoryEvent<T>(RepositoryEventType.Delete, id));
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
        /// <typeparam name="T">
        /// </typeparam>
        public static void FireNew<T>([NotNull] this IRepository<T> repository, int? id = null)
            where T : IEntity
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            repository.DbEvent.Raise(new RepositoryEvent<T>(RepositoryEventType.New, id));
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
        /// <typeparam name="T">
        /// </typeparam>
        public static void FireUpdated<T>(this IRepository<T> repository, int? id = null)
            where T : IEntity
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            repository.DbEvent.Raise(new RepositoryEvent<T>(RepositoryEventType.Update, id));
        }

        #endregion
    }
}