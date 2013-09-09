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
namespace YAF.Types.Objects
{
    #region Using

    using System;
    using System.Data;

    using YAF.Classes;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The typed user find.
    /// </summary>
    public class TypedUserFind
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedUserFind"/> class.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        public TypedUserFind(DataRow row)
        {
            this.UserID = row.Field<int?>("UserID");
            this.BoardID = row.Field<int?>("BoardID");
            this.Name = row.Field<string>("Name");
            this.Password = row.Field<string>("Password");
            this.Email = row.Field<string>("Email");
            this.Joined = row.Field<DateTime?>("Joined");
            this.LastVisit = row.Field<DateTime?>("LastVisit");
            this.IP = row.Field<string>("IP");
            this.NumPosts = row.Field<int?>("NumPosts");
            this.TimeZone = row.Field<int?>("TimeZone");
            this.Avatar = row.Field<string>("Avatar");
            this.Signature = row.Field<string>("Signature");
            this.AvatarImage = row.Field<byte[]>("AvatarImage");
            this.RankID = row.Field<int?>("RankID");
            this.Suspended = row.Field<DateTime?>("Suspended");
            this.LanguageFile = row.Field<string>("LanguageFile");
            this.ThemeFile = row.Field<string>("ThemeFile");
            this.Flags = row.Field<int?>("Flags");
            this.PMNotification = row.Field<bool?>("PMNotification");
            this.Points = row.Field<int?>("Points");
            this.IsApproved = row.Field<bool?>("IsApproved");
            this.IsActiveExcluded = row.Field<bool?>("IsActiveExcluded");
            this.UseMobileTheme = row.Field<bool?>("OverrideDefaultThemes");
            this.AvatarImageType = row.Field<string>("AvatarImageType");
            this.AutoWatchTopics = row.Field<bool?>("AutoWatchTopics");
            this.TextEditor = row.Field<string>("TextEditor");
            this.DisplayName = row.Field<string>("DisplayName");
            this.Culture = row.Field<string>("Culture");
            this.NotificationType = row.Field<int?>("NotificationType");
            this.DailyDigest = row.Field<bool?>("DailyDigest");
            this.IsGuest = row.Field<bool>("IsGuest");
            this.ProviderUserKey = this.IsGuest ? null : ObjectExtensions.ConvertObjectToType(row.Field<string>("ProviderUserKey"), Config.ProviderKeyType);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets AutoWatchTopics.
        /// </summary>
        public bool? AutoWatchTopics { get; set; }

        /// <summary>
        /// Gets or sets TextEditor.
        /// </summary>
        public string TextEditor { get; set; }

        /// <summary>
        /// Gets or sets Avatar.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets AvatarImage.
        /// </summary>
        public byte[] AvatarImage { get; set; }

        /// <summary>
        /// Gets or sets AvatarImageType.
        /// </summary>
        public string AvatarImageType { get; set; }

        /// <summary>
        /// Gets or sets BoardID.
        /// </summary>
        public int? BoardID { get; set; }

        /// <summary>
        /// Gets or sets Culture.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets DailyDigest.
        /// </summary>
        public bool? DailyDigest { get; set; }

        /// <summary>
        /// Gets or sets DisplayName.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Flags.
        /// </summary>
        public int? Flags { get; set; }

        /// <summary>
        /// Gets or sets IP.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets IsActiveExcluded.
        /// </summary>
        public bool? IsActiveExcluded { get; set; }

        /// <summary>
        /// Gets or sets IsApproved.
        /// </summary>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Gets or sets IsGuest.
        /// </summary>
        public bool IsGuest { get; set; }

        /// <summary>
        /// Gets or sets Joined.
        /// </summary>
        public DateTime? Joined { get; set; }

        /// <summary>
        /// Gets or sets LanguageFile.
        /// </summary>
        public string LanguageFile { get; set; }

        /// <summary>
        /// Gets or sets LastVisit.
        /// </summary>
        public DateTime? LastVisit { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets NotificationType.
        /// </summary>
        public int? NotificationType { get; set; }

        /// <summary>
        /// Gets or sets NumPosts.
        /// </summary>
        public int? NumPosts { get; set; }

        /// <summary>
        /// Gets or sets UseMobileTheme.
        /// </summary>
        public bool? UseMobileTheme { get; set; }

        /// <summary>
        /// Gets or sets PMNotification.
        /// </summary>
        public bool? PMNotification { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets Points.
        /// </summary>
        public int? Points { get; set; }

        /// <summary>
        /// Gets or sets ProviderUserKey.
        /// </summary>
        public object ProviderUserKey { get; set; }

        /// <summary>
        /// Gets or sets RankID.
        /// </summary>
        public int? RankID { get; set; }

        /// <summary>
        /// Gets or sets Signature.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Gets or sets Suspended.
        /// </summary>
        public DateTime? Suspended { get; set; }

        /// <summary>
        /// Gets or sets ThemeFile.
        /// </summary>
        public string ThemeFile { get; set; }

        /// <summary>
        /// Gets or sets TimeZone.
        /// </summary>
        public int? TimeZone { get; set; }

        /// <summary>
        /// Gets or sets UserID.
        /// </summary>
        public int? UserID { get; set; }

        #endregion
    }
}