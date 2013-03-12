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
namespace YAF.Types.Models
{
    #region Using

    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     A class which represents the yaf_Smiley table in the Yaf Database.
    /// </summary>
    [Serializable]
    public partial class Smiley : IEntity, IHaveID, IHaveBoardID
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Smiley" /> class.
        /// </summary>
        public Smiley()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the smiley id.
        /// </summary>
        [AutoIncrement]
        [Alias("SmileyID")]
        public int ID { get; set; }

        /// <summary>
        ///     Gets or sets the board id.
        /// </summary>
        public int BoardID { get; set; }

        /// <summary>
        ///     Gets or sets the code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Gets or sets the emoticon.
        /// </summary>
        public string Emoticon { get; set; }

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     Gets or sets the sort order.
        /// </summary>
        public byte SortOrder { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The on created.
        /// </summary>
        partial void OnCreated();

        #endregion
    }
}