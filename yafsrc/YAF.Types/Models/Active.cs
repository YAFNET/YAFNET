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
    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     A class which represents the Active table.
    /// </summary>
    [Serializable]
    public partial class Active : IEntity, IHaveBoardID
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Active"/> class.
        /// </summary>
        public Active()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the board id.
        /// </summary>
        public int BoardID { get; set; }

        /// <summary>
        /// Gets or sets the browser.
        /// </summary>
        public string Browser { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public int? Flags { get; set; }

        /// <summary>
        /// Gets or sets the forum id.
        /// </summary>
        public int? ForumID { get; set; }

        /// <summary>
        /// Gets or sets the forum page.
        /// </summary>
        public string ForumPage { get; set; }

        /// <summary>
        /// Gets or sets the ip.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the last active.
        /// </summary>
        public DateTime LastActive { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        public DateTime Login { get; set; }

        /// <summary>
        /// Gets or sets the platform.
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// Gets or sets the topic id.
        /// </summary>
        public int? TopicID { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public int UserID { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The on created.
        /// </summary>
        partial void OnCreated();

        #endregion
    }
}