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
namespace YAF.Types.Models
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;

    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     A class which represents the yaf_AccessMask table in the Yaf Database.
    /// </summary>
    [Serializable]
    [Table(Name = "AccessMask")]
    public partial class AccessMask : IEntity, IDataLoadable
    {
        #region Fields

        /// <summary>
        /// The _ access mask id.
        /// </summary>
        private int _AccessMaskID;

        /// <summary>
        /// The _ board id.
        /// </summary>
        private int _BoardID;

        /// <summary>
        /// The _ flags.
        /// </summary>
        private int _Flags;

        /// <summary>
        /// The _ name.
        /// </summary>
        private string _Name;

        /// <summary>
        /// The _ sort order.
        /// </summary>
        private short _SortOrder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessMask"/> class.
        /// </summary>
        public AccessMask()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the access mask id.
        /// </summary>
        public int AccessMaskID
        {
            get
            {
                return this._AccessMaskID;
            }

            set
            {
                this._AccessMaskID = value;
            }
        }

        /// <summary>
        /// Gets or sets the board id.
        /// </summary>
        public int BoardID
        {
            get
            {
                return this._BoardID;
            }

            set
            {
                this._BoardID = value;
            }
        }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public int Flags
        {
            get
            {
                return this._Flags;
            }

            set
            {
                this._Flags = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                this._Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public short SortOrder
        {
            get
            {
                return this._SortOrder;
            }

            set
            {
                this._SortOrder = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The load from dictionary.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="clear">
        /// The clear.
        /// </param>
        public void LoadFromDictionary(IDictionary<string, object> dictionary, bool clear = true)
        {
            if (dictionary.ContainsKey("AccessMaskID"))
            {
                this._AccessMaskID = dictionary["AccessMaskID"].ToType<int>();
            }
            else if (clear)
            {
                this._AccessMaskID = default(int);
            }

            if (dictionary.ContainsKey("BoardID"))
            {
                this._BoardID = dictionary["BoardID"].ToType<int>();
            }
            else if (clear)
            {
                this._BoardID = default(int);
            }

            if (dictionary.ContainsKey("Name"))
            {
                this._Name = dictionary["Name"].ToType<string>();
            }
            else if (clear)
            {
                this._Name = default(string);
            }

            if (dictionary.ContainsKey("Flags"))
            {
                this._Flags = dictionary["Flags"].ToType<int>();
            }
            else if (clear)
            {
                this._Flags = default(int);
            }

            if (dictionary.ContainsKey("SortOrder"))
            {
                this._SortOrder = dictionary["SortOrder"].ToType<short>();
            }
            else if (clear)
            {
                this._SortOrder = default(short);
            }

            this.OnLoadedFromDictionary(dictionary, clear);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on created.
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// The on loaded from dictionary.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="clear">
        /// The clear.
        /// </param>
        partial void OnLoadedFromDictionary(IDictionary<string, object> dictionary, bool clear = true);

        #endregion
    }
}