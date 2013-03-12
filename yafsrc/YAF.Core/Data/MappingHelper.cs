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
namespace YAF.Core.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using Omu.ValueInjecter;

    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     The mapping helper.
    /// </summary>
    public static class MappingHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Maps multiple objects to entities.
        /// </summary>
        /// <param name="objects">
        /// The objects.
        /// </param>
        /// <typeparam name="TTypeIn">
        /// </typeparam>
        /// <typeparam name="TTypeOut">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<TTypeOut> ToMapped<TTypeIn, TTypeOut>(this IEnumerable<TTypeIn> objects)
            where TTypeIn : class
            where TTypeOut : IEntity, new()
        {
            return objects.Select(obj => obj.ToMappedEntity<TTypeOut>());
        }

        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="inputObject">
        /// The input object.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public static TEntity ToMappedEntity<TEntity>(this object inputObject) where TEntity : IEntity, new()
        {
            var entity = new TEntity();

            entity.InjectFrom(inputObject);

            return entity;
        }

        #endregion
    }
}