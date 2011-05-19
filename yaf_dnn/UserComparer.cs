/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.DotNetNuke
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
        /// The compare.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The compare.
        /// </returns>
        public int Compare(object x, object y)
        {
            var comparer = new Comparer(CultureInfo.CurrentCulture);

            return comparer.Compare(((UserInfo)x).UserID, ((UserInfo)y).UserID);
        }

        #endregion
    }
}