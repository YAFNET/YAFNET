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
    ///     A class which represents the yaf_Smiley table in the Yaf Database.
    /// </summary>
    [Serializable]
    [Table(Name = "Smiley")]
    public partial class Smiley : IEntity, IDataLoadable
    {
        #region Fields

        /// <summary>
        /// The _ board id.
        /// </summary>
        private int _BoardID;

        /// <summary>
        /// The _ code.
        /// </summary>
        private string _Code;

        /// <summary>
        /// The _ emoticon.
        /// </summary>
        private string _Emoticon;

        /// <summary>
        /// The _ icon.
        /// </summary>
        private string _Icon;

        /// <summary>
        /// The _ smiley id.
        /// </summary>
        private int _SmileyID;

        /// <summary>
        /// The _ sort order.
        /// </summary>
        private byte _SortOrder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Smiley"/> class.
        /// </summary>
        public Smiley()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

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
        /// Gets or sets the code.
        /// </summary>
        public string Code
        {
            get
            {
                return this._Code;
            }

            set
            {
                this._Code = value;
            }
        }

        /// <summary>
        /// Gets or sets the emoticon.
        /// </summary>
        public string Emoticon
        {
            get
            {
                return this._Emoticon;
            }

            set
            {
                this._Emoticon = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon
        {
            get
            {
                return this._Icon;
            }

            set
            {
                this._Icon = value;
            }
        }

        /// <summary>
        /// Gets or sets the smiley id.
        /// </summary>
        public int SmileyID
        {
            get
            {
                return this._SmileyID;
            }

            set
            {
                this._SmileyID = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public byte SortOrder
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
            if (dictionary.ContainsKey("SmileyID"))
            {
                this._SmileyID = dictionary["SmileyID"].ToType<int>();
            }
            else if (clear)
            {
                this._SmileyID = default(int);
            }

            if (dictionary.ContainsKey("BoardID"))
            {
                this._BoardID = dictionary["BoardID"].ToType<int>();
            }
            else if (clear)
            {
                this._BoardID = default(int);
            }

            if (dictionary.ContainsKey("Code"))
            {
                this._Code = dictionary["Code"].ToType<string>();
            }
            else if (clear)
            {
                this._Code = default(string);
            }

            if (dictionary.ContainsKey("Icon"))
            {
                this._Icon = dictionary["Icon"].ToType<string>();
            }
            else if (clear)
            {
                this._Icon = default(string);
            }

            if (dictionary.ContainsKey("Emoticon"))
            {
                this._Emoticon = dictionary["Emoticon"].ToType<string>();
            }
            else if (clear)
            {
                this._Emoticon = default(string);
            }

            if (dictionary.ContainsKey("SortOrder"))
            {
                this._SortOrder = dictionary["SortOrder"].ToType<byte>();
            }
            else if (clear)
            {
                this._SortOrder = default(byte);
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