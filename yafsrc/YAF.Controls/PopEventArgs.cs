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

namespace YAF.Controls
{
    using System;

    using YAF.Types;

    /// <summary>
    /// The pop event args.
    /// </summary>
    public class PopEventArgs : EventArgs
    {
        #region Constants and Fields

        /// <summary>
        ///   The _item.
        /// </summary>
        private readonly string _item;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PopEventArgs"/> class.
        /// </summary>
        /// <param name="eventArgument">
        /// The event argument.
        /// </param>
        public PopEventArgs([NotNull] string eventArgument)
        {
            this._item = eventArgument;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Item.
        /// </summary>
        public string Item
        {
            get
            {
                return this._item;
            }
        }

        #endregion
    }
}