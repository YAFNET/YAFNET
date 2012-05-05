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

namespace YAF.DotNetNuke.Utils
{
    using System.Collections;
    using System.Globalization;

    using global::DotNetNuke.Entities.Users;

    /// <summary>
    /// Sort the User List by UserId
    /// </summary>
    public class UserComparer : IComparer
    {
        #region Implemented Interfaces

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The compare.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.-or- <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other. </exception>
        public int Compare(object x, object y)
        {
            var comparer = new Comparer(CultureInfo.CurrentCulture);

            return comparer.Compare(((UserInfo)x).UserID, ((UserInfo)y).UserID);
        }

        #endregion
    }
}