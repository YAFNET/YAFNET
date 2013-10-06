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

namespace YAF.Types
{
    using System;

    /// <summary>
    ///     Provides functions used for code contracts.
    /// </summary>
    public static class CodeContracts
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Validates argument (obj) is not <see langword="null" />. Throws exception
        ///     if it is.
        /// </summary>
        /// <typeparam name="T">
        ///     type of the argument that's being verified
        /// </typeparam>
        /// <param name="obj">value of argument to verify not null</param>
        /// <param name="argumentName">name of the argument</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="obj" /> is
        ///     <c>null</c>.
        /// </exception>
        [ContractAnnotation("obj:null => halt")]
        public static void VerifyNotNull<T>([CanBeNull] T obj, string argumentName) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(argumentName, String.Format("{0} cannot be null", argumentName));
            }
        }

        #endregion
    }
}