/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
	using System;
	using System.Collections.Generic;

	using YAF.Classes;
	using YAF.Types.Interfaces.Extensions;
	using YAF.Types.Objects;
	using YAF.Utils;

	/// <summary>
	/// </summary>
	public class TypedUserFind : ITypedUserFind
	{
		/// <summary>
		///   Gets or sets AutoWatchTopics.
		/// </summary>
		public bool? AutoWatchTopics { get; set; }

		/// <summary>
		///   Gets or sets Avatar.
		/// </summary>
		public string Avatar { get; set; }

		/// <summary>
		///   Gets or sets AvatarImage.
		/// </summary>
		public byte[] AvatarImage { get; set; }

		/// <summary>
		///   Gets or sets AvatarImageType.
		/// </summary>
		public string AvatarImageType { get; set; }

		/// <summary>
		///   Gets or sets BoardID.
		/// </summary>
		public int? BoardID { get; set; }

		/// <summary>
		///   Gets or sets Culture.
		/// </summary>
		public string Culture { get; set; }

		/// <summary>
		///   Gets or sets DailyDigest.
		/// </summary>
		public bool? DailyDigest { get; set; }

		/// <summary>
		///   Gets or sets DisplayName.
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		///   Gets or sets Email.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		///   Gets or sets Flags.
		/// </summary>
		public int? Flags { get; set; }

		/// <summary>
		///   Gets or sets IP.
		/// </summary>
		public string IP { get; set; }

		/// <summary>
		///   Gets or sets IsActiveExcluded.
		/// </summary>
		public bool? IsActiveExcluded { get; set; }

		/// <summary>
		///   Gets or sets IsAdmin.
		/// </summary>
		public bool? IsAdmin { get; set; }

		/// <summary>
		///   Gets or sets IsApproved.
		/// </summary>
		public bool? IsApproved { get; set; }

		/// <summary>
		///   Gets or sets IsCaptchaExcluded.
		/// </summary>
		public bool? IsCaptchaExcluded { get; set; }

		/// <summary>
		///   Gets or sets IsDST.
		/// </summary>
		public bool? IsDST { get; set; }

		/// <summary>
		///   Gets or sets IsDirty.
		/// </summary>
		public bool? IsDirty { get; set; }

		/// <summary>
		///   Gets or sets a value indicating whether IsGuest.
		/// </summary>
		public bool IsGuest { get; set; }

		/// <summary>
		///   Gets or sets Joined.
		/// </summary>
		public DateTime? Joined { get; set; }

		/// <summary>
		///   Gets or sets LanguageFile.
		/// </summary>
		public string LanguageFile { get; set; }

		/// <summary>
		///   Gets or sets LastVisit.
		/// </summary>
		public DateTime? LastVisit { get; set; }

		/// <summary>
		///   Gets or sets Name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///   Gets or sets NotificationType.
		/// </summary>
		public int? NotificationType { get; set; }

		/// <summary>
		///   Gets or sets NumPosts.
		/// </summary>
		public int? NumPosts { get; set; }

		/// <summary>
		///   Gets or sets OverrideDefaultThemes.
		/// </summary>
		public bool? OverrideDefaultThemes { get; set; }

		/// <summary>
		///   Gets or sets PMNotification.
		/// </summary>
		public bool? PMNotification { get; set; }

		/// <summary>
		///   Gets or sets Password.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		///   Gets or sets Points.
		/// </summary>
		public int? Points { get; set; }

		/// <summary>
		///   Gets or sets ProviderUserKey.
		/// </summary>
		public object ProviderUserKey { get; set; }

		/// <summary>
		///   Gets or sets RankID.
		/// </summary>
		public int? RankID { get; set; }

		/// <summary>
		///   Gets or sets Signature.
		/// </summary>
		public string Signature { get; set; }

		/// <summary>
		///   Gets or sets Suspended.
		/// </summary>
		public DateTime? Suspended { get; set; }

		/// <summary>
		///   Gets or sets TextEditor.
		/// </summary>
		public string TextEditor { get; set; }

		/// <summary>
		///   Gets or sets ThemeFile.
		/// </summary>
		public string ThemeFile { get; set; }

		/// <summary>
		///   Gets or sets TimeZone.
		/// </summary>
		public int? TimeZone { get; set; }

		/// <summary>
		///   Gets or sets UseSingleSignOn.
		/// </summary>
		public bool? UseSingleSignOn { get; set; }

		/// <summary>
		///   Gets or sets UserID.
		/// </summary>
		public int? UserID { get; set; }

		/// <summary>
		/// The load from dictionary.
		/// </summary>
		/// <param name="dictionary">The dictionary.
		///  </param><param name="clear">The clear.
		///  </param>
		public void LoadFromDictionary(IDictionary<string, object> dictionary, bool clear)
		{
			this.UserID = dictionary.GetValueOrDefault("UserID", clear ? default(int?) : this.UserID).ToType<int?>();
			this.BoardID = dictionary.GetValueOrDefault("BoardID", clear ? default(int?) : this.BoardID).ToType<int?>();
			this.Name = dictionary.GetValueOrDefault("Name", clear ? default(string) : this.Name).ToType<string>();
			this.Password = dictionary.GetValueOrDefault("Password", clear ? default(string) : this.Password).ToType<string>();
			this.Email = dictionary.GetValueOrDefault("Email", clear ? default(string) : this.Email).ToType<string>();
			this.Joined = dictionary.GetValueOrDefault("Joined", clear ? default(DateTime?) : this.Joined).ToType<DateTime?>();
			this.LastVisit = dictionary.GetValueOrDefault("LastVisit", clear ? default(DateTime?) : this.LastVisit).ToType<DateTime?>();
			this.IP = dictionary.GetValueOrDefault("IP", clear ? default(string) : this.IP).ToType<string>();
			this.NumPosts = dictionary.GetValueOrDefault("NumPosts", clear ? default(int?) : this.NumPosts).ToType<int?>();
			this.TimeZone = dictionary.GetValueOrDefault("TimeZone", clear ? default(int?) : this.TimeZone).ToType<int?>();
			this.Avatar = dictionary.GetValueOrDefault("Avatar", clear ? default(string) : this.Avatar).ToType<string>();
			this.Signature = dictionary.GetValueOrDefault("Signature", clear ? default(string) : this.Signature).ToType<string>();
			this.AvatarImage = dictionary.GetValueOrDefault("AvatarImage", clear ? default(byte[]) : this.AvatarImage).ToType<byte[]>();
			this.RankID = dictionary.GetValueOrDefault("RankID", clear ? default(int?) : this.RankID).ToType<int?>();
			this.Suspended = dictionary.GetValueOrDefault("Suspended", clear ? default(DateTime?) : this.Suspended).ToType<DateTime?>();
			this.LanguageFile = dictionary.GetValueOrDefault("LanguageFile", clear ? default(string) : this.LanguageFile).ToType<string>();
			this.ThemeFile = dictionary.GetValueOrDefault("ThemeFile", clear ? default(string) : this.ThemeFile).ToType<string>();
			this.Flags = dictionary.GetValueOrDefault("Flags", clear ? default(int?) : this.Flags).ToType<int?>();
			this.PMNotification = dictionary.GetValueOrDefault("PMNotification", clear ? default(bool?) : this.PMNotification).ToType<bool?>();
			this.Points = dictionary.GetValueOrDefault("Points", clear ? default(int?) : this.Points).ToType<int?>();
			this.IsAdmin = dictionary.GetValueOrDefault("IsAdmin", clear ? default(bool?) : this.IsAdmin).ToType<bool?>();
			this.IsApproved = dictionary.GetValueOrDefault("IsApproved", clear ? default(bool?) : this.IsApproved).ToType<bool?>();
			this.IsActiveExcluded = dictionary.GetValueOrDefault("IsActiveExcluded", clear ? default(bool?) : this.IsActiveExcluded).ToType<bool?>();
			this.ProviderUserKey = dictionary.GetValueOrDefault("ProviderUserKey", clear ? default(string) : this.ProviderUserKey).ToType<string>();
			this.OverrideDefaultThemes = dictionary.GetValueOrDefault("OverrideDefaultThemes", clear ? default(bool?) : this.OverrideDefaultThemes).ToType<bool?>();
			this.AvatarImageType = dictionary.GetValueOrDefault("AvatarImageType", clear ? default(string) : this.AvatarImageType).ToType<string>();
			this.AutoWatchTopics = dictionary.GetValueOrDefault("AutoWatchTopics", clear ? default(bool?) : this.AutoWatchTopics).ToType<bool?>();
			this.DisplayName = dictionary.GetValueOrDefault("DisplayName", clear ? default(string) : this.DisplayName).ToType<string>();
			this.Culture = dictionary.GetValueOrDefault("Culture", clear ? default(string) : this.Culture).ToType<string>();
			this.NotificationType = dictionary.GetValueOrDefault("NotificationType", clear ? default(int?) : this.NotificationType).ToType<int?>();
			this.DailyDigest = dictionary.GetValueOrDefault("DailyDigest", clear ? default(bool?) : this.DailyDigest).ToType<bool?>();
			this.TextEditor = dictionary.GetValueOrDefault("TextEditor", clear ? default(string) : this.TextEditor).ToType<string>();
			this.UseSingleSignOn = dictionary.GetValueOrDefault("UseSingleSignOn", clear ? default(bool?) : this.UseSingleSignOn).ToType<bool?>();
			this.IsGuest = dictionary.GetValueOrDefault("IsGuest", clear ? default(bool?) : this.IsGuest).ToType<bool>();
			this.IsCaptchaExcluded = dictionary.GetValueOrDefault("IsCaptchaExcluded", clear ? default(bool?) : this.IsCaptchaExcluded).ToType<bool?>();
			this.IsDST = dictionary.GetValueOrDefault("IsDST", clear ? default(bool?) : this.IsDST).ToType<bool?>();
			this.IsDirty = dictionary.GetValueOrDefault("IsDirty", clear ? default(bool?) : this.IsDirty).ToType<bool?>();
			this.ProviderUserKey = this.IsGuest
			                       	? null
			                       	: ObjectExtensions.ConvertObjectToType(
			                       		dictionary["ProviderUserKey"].ToType<string>(), Config.ProviderKeyType);
		}
	}
}