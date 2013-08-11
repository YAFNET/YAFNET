/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Types.Objects
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The Google User
    /// </summary>
    [DataContract]
    public class GoogleUser
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>
        /// The user ID.
        /// </value>
        [DataMember(Name = "id")]
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [DataMember(Name = "name")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [DataMember(Name = "given_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [DataMember(Name = "family_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the profile URL.
        /// </summary>
        /// <value>
        /// The profile URL.
        /// </value>
        [DataMember(Name = "link")]
        public string ProfileURL { get; set; }

        /// <summary>
        /// Gets or sets the profile image.
        /// </summary>
        /// <value>
        /// The profile image.
        /// </value>
        [DataMember(Name = "picture")]
        public string ProfileImage { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>
        /// The locale.
        /// </value>
        [DataMember(Name = "locale")]
        public string Locale { get; set; }
    }
}