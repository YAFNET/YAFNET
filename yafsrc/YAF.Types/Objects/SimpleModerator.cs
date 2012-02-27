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
namespace YAF.Types.Objects
{
    using System;

    /// <summary>
    /// The moderator.
    /// </summary>
    [Serializable]
    public class SimpleModerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModerator"/> class.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="forumName">
        /// The forum Name.
        /// </param>
        /// <param name="moderatorID">
        /// The moderator id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="avatar">
        /// The avatar.
        /// </param>
        /// <param name="avatarImage">
        /// The avatar Image.
        /// </param>
        /// <param name="displayName">
        /// The display Name.
        /// </param>
        /// <param name="style">
        /// The style.
        /// </param>
        /// <param name="isGroup">
        /// The is group.
        /// </param>
        public SimpleModerator(
            long forumID, string forumName, long moderatorID,  string name, string email, string avatar, bool avatarImage, string displayName, string style, bool isGroup)
        {
            this.ForumID = forumID;
            this.ForumName = forumName;
            this.ModeratorID = moderatorID;
            this.Name = name;
            this.Email = email;
            this.Avatar = avatar;
            this.AvatarImage = avatarImage;
            this.DisplayName = displayName;
            this.Style = style;
            this.IsGroup = isGroup;
        }

        /// <summary>
        /// Gets or sets ForumID.
        /// </summary>
        public long ForumID { get; set; }

        /// <summary>
        /// Gets or sets Forum Name.
        /// </summary>
        public string ForumName { get; set; }

        /// <summary>
        /// Gets or sets ModeratorID.
        /// </summary>
        public long ModeratorID { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Avatar.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [avatar image].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [avatar image]; otherwise, <c>false</c>.
        /// </value>
        public bool AvatarImage { get; set; }

        /// <summary>
        /// Gets or sets Display Name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Style.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsGroup.
        /// </summary>
        public bool IsGroup { get; set; }
    }
}