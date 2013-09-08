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

using YAF.Types.Flags;

namespace YAF.Types.Models
{
    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the User table.
    /// </summary>
    [Serializable]
    [Table(Name = "User")]
    public partial class User : IEntity, IHaveID, IHaveBoardID
    {
        partial void OnCreated();

        public User()
        {
            this.OnCreated();
        }

        #region Properties

        [AutoIncrement]
        [Alias("UserID")]
        public int ID { get; set; }

        public int BoardID { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public DateTime Joined { get; set; }

        public DateTime LastVisit { get; set; }

        public string IP { get; set; }

        public int NumPosts { get; set; }

        public int TimeZone { get; set; }

        public string Avatar { get; set; }

        public string Signature { get; set; }

        public byte[] AvatarImage { get; set; }

        public int RankID { get; set; }

        public DateTime? Suspended { get; set; }

        public string LanguageFile { get; set; }

        public string ThemeFile { get; set; }

        public int Flags { get; set; }

        public bool PMNotification { get; set; }

        public int Points { get; set; }

        public bool? IsAdmin { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsActiveExcluded { get; set; }

        public string ProviderUserKey { get; set; }

        public bool OverrideDefaultThemes { get; set; }

        public string AvatarImageType { get; set; }

        public bool AutoWatchTopics { get; set; }

        public string DisplayName { get; set; }

        [Alias("CultureUser")]
        public string Culture { get; set; }

        public int? NotificationType { get; set; }

        public bool DailyDigest { get; set; }

        public string TextEditor { get; set; }

        public bool UseSingleSignOn { get; set; }

        public bool? IsGuest { get; set; }

        public bool? IsCaptchaExcluded { get; set; }

        public bool? IsDST { get; set; }

        public bool? IsDirty { get; set; }

        public bool IsFacebookUser { get; set; }

        public bool IsTwitterUser { get; set; }

        public string UserStyle { get; set; }

        public int StyleFlags { get; set; }

        public bool? IsUserStyle { get; set; }

        public bool? IsGroupStyle { get; set; }

        public bool? IsRankStyle { get; set; }

        #endregion
    }
}