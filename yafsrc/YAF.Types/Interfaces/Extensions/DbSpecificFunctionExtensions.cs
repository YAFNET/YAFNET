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
namespace YAF.Types.Interfaces.Extensions
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Types.Interfaces.Data;

    #endregion

    public static class SortableOperationExtensions
    {
        public static IOrderedEnumerable<T> BySortOrder<T>(this IEnumerable<T> sortEnumerable)
            where T : IDbSortableOperation
        {
            return sortEnumerable.OrderBy(x => x.SortOrder);
        }

        /// <summary>
        /// The is operation supported.
        /// </summary>
        /// <param name="functions">
        /// The functions.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <returns>
        /// The is operation supported.
        /// </returns>
        public static IEnumerable<T> WhereOperationSupported<T>([NotNull] this IEnumerable<T> checkSupported, [NotNull] string operationName)
            where T : IDbSortableOperation
        {
            CodeContracts.ArgumentNotNull(checkSupported, "checkSupported");
            CodeContracts.ArgumentNotNull(operationName, "operationName");

            return checkSupported.Where(x => x.IsSupportedOperation(operationName));
        }
    }

    /// <summary>
    /// The db specific function extensions.
    /// </summary>
    public static class DbSpecificFunctionExtensions
    {
        #region Public Methods

        /// <summary>
        /// Returns IEnumerable where the provider name is supported.
        /// </summary>
        /// <param name="functions">
        /// The functions.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <returns>
        /// The is operation supported.
        /// </returns>
        [NotNull]
        public static IEnumerable<IDbSpecificFunction> WhereProviderName([NotNull] this IEnumerable<IDbSpecificFunction> functions, [NotNull] string providerName)
        {
            CodeContracts.ArgumentNotNull(functions, "functions");
            CodeContracts.ArgumentNotNull(providerName, "providerName");

            return functions.Where(p => string.Equals(p.ProviderName, providerName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}