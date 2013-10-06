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
namespace YAF.Types.Interfaces.Data
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    /// The db specific function extensions.
    /// </summary>
    public static class IDbSpecificFunctionExtensions
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
            CodeContracts.VerifyNotNull(functions, "functions");
            CodeContracts.VerifyNotNull(providerName, "providerName");

            return functions.Where(p => string.Equals(p.ProviderName, providerName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}