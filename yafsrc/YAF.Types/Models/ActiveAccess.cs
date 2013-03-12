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
    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     A class which represents the ActiveAccess table.
    /// </summary>
    [Serializable]
    public partial class ActiveAccess : IEntity, IHaveBoardID
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveAccess"/> class.
        /// </summary>
        public ActiveAccess()
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
        /// Gets or sets a value indicating whether delete access.
        /// </summary>
        public bool DeleteAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether download access.
        /// </summary>
        public bool DownloadAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether edit access.
        /// </summary>
        public bool EditAccess { get; set; }

        /// <summary>
        /// Gets or sets the forum id.
        /// </summary>
        public int ForumID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is admin.
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is forum moderator.
        /// </summary>
        public bool IsForumModerator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is guest x.
        /// </summary>
        public bool IsGuestX { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is moderator.
        /// </summary>
        public bool IsModerator { get; set; }

        /// <summary>
        /// Gets or sets the last active.
        /// </summary>
        public DateTime? LastActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether moderator access.
        /// </summary>
        public bool ModeratorAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether poll access.
        /// </summary>
        public bool PollAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether post access.
        /// </summary>
        public bool PostAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether priority access.
        /// </summary>
        public bool PriorityAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether read access.
        /// </summary>
        public bool ReadAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reply access.
        /// </summary>
        public bool ReplyAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether upload access.
        /// </summary>
        public bool UploadAccess { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether vote access.
        /// </summary>
        public bool VoteAccess { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The on created.
        /// </summary>
        partial void OnCreated();

        #endregion
    }
}