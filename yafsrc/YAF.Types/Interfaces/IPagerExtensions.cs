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
namespace YAF.Types.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Types.Interfaces;

    /// <summary>
    ///     The pager extensions.
    /// </summary>
    public static class IPagerExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Uses the pager to convert the list into a properly skipped and paged list.
        /// </summary>
        /// <param name="list">
        /// The enumerable.
        /// </param>
        /// <param name="pager">
        /// The pager.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IList<T> GetPaged<T>([NotNull] this IList<T> list, [NotNull] IPager pager)
        {
            CodeContracts.VerifyNotNull(list, "list");
            CodeContracts.VerifyNotNull(pager, "pager");

            pager.Count = list.Count;

            return list.Skip(pager.SkipIndex()).Take(pager.PageSize).ToList();
        }

        /// <summary>
        /// The page count.
        /// </summary>
        /// <param name="pager">
        /// The pager. 
        /// </param>
        /// <returns>
        /// The <see cref="int"/> . 
        /// </returns>
        public static int PageCount(this IPager pager)
        {
            CodeContracts.VerifyNotNull(pager, "pager");

            return (int)Math.Ceiling((double)pager.Count / pager.PageSize);
        }

        /// <summary>
        /// The skip index.
        /// </summary>
        /// <param name="pager">
        /// The pager. 
        /// </param>
        /// <returns>
        /// The <see cref="int"/> . 
        /// </returns>
        public static int SkipIndex([NotNull] this IPager pager)
        {
            CodeContracts.VerifyNotNull(pager, "pager");

            return (int)Math.Ceiling((double)pager.CurrentPageIndex * pager.PageSize);
        }

        #endregion
    }
}