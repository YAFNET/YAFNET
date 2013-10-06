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
namespace YAF.Types.Extensions
{
    using System.Reflection;

    /// <summary>
    /// The property info extensions.
    /// </summary>
    public static class PropertyInfoExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get value as.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetValueAs<T>([NotNull] this PropertyInfo propertyInfo, object obj, object[] index = null)
        {
            CodeContracts.VerifyNotNull(propertyInfo, "propertyInfo");

            return propertyInfo.GetValue(obj, index).ToType<T>();
        }

        #endregion
    }
}