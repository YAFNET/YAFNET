// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypedUserFind.cs" company="">
//   
// </copyright>
// <summary>
//   The i typed user find.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Types.Objects
{
	using System;
	using System.Collections.Generic;

	using YAF.Types.Interfaces;

	/// <summary>
	/// The i typed user find.
	/// </summary>
	public interface ITypedUserFind : IDataLoadable
	{
		#region Properties

		/// <summary>
		///   Gets or sets AutoWatchTopics.
		/// </summary>
		bool? AutoWatchTopics { get; set; }

		/// <summary>
		///   Gets or sets Avatar.
		/// </summary>
		string Avatar { get; set; }

		/// <summary>
		///   Gets or sets AvatarImage.
		/// </summary>
		byte[] AvatarImage { get; set; }

		/// <summary>
		///   Gets or sets AvatarImageType.
		/// </summary>
		string AvatarImageType { get; set; }

		/// <summary>
		///   Gets or sets BoardID.
		/// </summary>
		int? BoardID { get; set; }

		/// <summary>
		///   Gets or sets Culture.
		/// </summary>
		string Culture { get; set; }

		/// <summary>
		///   Gets or sets DailyDigest.
		/// </summary>
		bool? DailyDigest { get; set; }

		/// <summary>
		///   Gets or sets DisplayName.
		/// </summary>
		string DisplayName { get; set; }

		/// <summary>
		///   Gets or sets Email.
		/// </summary>
		string Email { get; set; }

		/// <summary>
		///   Gets or sets Flags.
		/// </summary>
		int? Flags { get; set; }

		/// <summary>
		///   Gets or sets IP.
		/// </summary>
		string IP { get; set; }

		/// <summary>
		///   Gets or sets IsActiveExcluded.
		/// </summary>
		bool? IsActiveExcluded { get; set; }

		/// <summary>
		///   Gets or sets IsAdmin.
		/// </summary>
		bool? IsAdmin { get; set; }

		/// <summary>
		///   Gets or sets IsApproved.
		/// </summary>
		bool? IsApproved { get; set; }

		/// <summary>
		///   Gets or sets IsCaptchaExcluded.
		/// </summary>
		bool? IsCaptchaExcluded { get; set; }

		/// <summary>
		///   Gets or sets IsDST.
		/// </summary>
		bool? IsDST { get; set; }

		/// <summary>
		///   Gets or sets IsDirty.
		/// </summary>
		bool? IsDirty { get; set; }

		/// <summary>
		///   Gets or sets a value indicating whether IsGuest.
		/// </summary>
		bool IsGuest { get; set; }

		/// <summary>
		///   Gets or sets Joined.
		/// </summary>
		DateTime? Joined { get; set; }

		/// <summary>
		///   Gets or sets LanguageFile.
		/// </summary>
		string LanguageFile { get; set; }

		/// <summary>
		///   Gets or sets LastVisit.
		/// </summary>
		DateTime? LastVisit { get; set; }

		/// <summary>
		///   Gets or sets Name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		///   Gets or sets NotificationType.
		/// </summary>
		int? NotificationType { get; set; }

		/// <summary>
		///   Gets or sets NumPosts.
		/// </summary>
		int? NumPosts { get; set; }

		/// <summary>
		///   Gets or sets OverrideDefaultThemes.
		/// </summary>
		bool? OverrideDefaultThemes { get; set; }

		/// <summary>
		///   Gets or sets PMNotification.
		/// </summary>
		bool? PMNotification { get; set; }

		/// <summary>
		///   Gets or sets Password.
		/// </summary>
		string Password { get; set; }

		/// <summary>
		///   Gets or sets Points.
		/// </summary>
		int? Points { get; set; }

		/// <summary>
		///   Gets or sets ProviderUserKey.
		/// </summary>
		object ProviderUserKey { get; set; }

		/// <summary>
		///   Gets or sets RankID.
		/// </summary>
		int? RankID { get; set; }

		/// <summary>
		///   Gets or sets Signature.
		/// </summary>
		string Signature { get; set; }

		/// <summary>
		///   Gets or sets Suspended.
		/// </summary>
		DateTime? Suspended { get; set; }

		/// <summary>
		///   Gets or sets TextEditor.
		/// </summary>
		string TextEditor { get; set; }

		/// <summary>
		///   Gets or sets ThemeFile.
		/// </summary>
		string ThemeFile { get; set; }

		/// <summary>
		///   Gets or sets TimeZone.
		/// </summary>
		int? TimeZone { get; set; }

		/// <summary>
		///   Gets or sets UseSingleSignOn.
		/// </summary>
		bool? UseSingleSignOn { get; set; }

		/// <summary>
		///   Gets or sets UserID.
		/// </summary>
		int? UserID { get; set; }

		#endregion
	}
}