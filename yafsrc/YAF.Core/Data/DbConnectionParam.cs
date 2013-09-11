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
    using YAF.Types;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     The db connection param.
    /// </summary>
    public struct DbConnectionParam : IDbConnectionParam
    {
        #region Fields

        private int _id;

        private string _name;

        private string _value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbConnectionParam" /> class.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="defaultValue">
        ///     The value.
        /// </param>
        /// <param name="visible">
        ///     The visible.
        /// </param>
        public DbConnectionParam(int id, string name, [NotNull] string defaultValue = null)
        {
            this._id = id;
            this._name = name;
            this._value = defaultValue ?? string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets ID.
        /// </summary>
        public int ID
        {
            get
            {
                return this._id;
            }

            set
            {
                this._id = value;
            }
        }

        /// <summary>
        ///     Gets or sets Label.
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        ///     Gets or sets DefaultValue.
        /// </summary>
        public string Value
        {
            get
            {
                return this._value;
            }
        }

        #endregion
    }
}