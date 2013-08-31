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
namespace YAF.Types.Interfaces
{
    using System;
    using System.Data;
    using System.Web.Security;

    using YAF.Types.Constants;

    /// <summary>
    /// The UserData Interface
    /// </summary>
    public interface IUserData
    {
        /// <summary>
        ///   Gets a value indicating whether AutoWatchTopics.
        /// </summary>
        bool AutoWatchTopics { get; }

        /// <summary>
        ///   Gets Avatar.
        /// </summary>
        string Avatar { get; }

        /// <summary>
        ///   Gets Culture.
        /// </summary>
        string CultureUser { get; }

        /// <summary>
        ///   Gets DBRow.
        /// </summary>
        DataRow DBRow { get; }

        /// <summary>
        ///   Gets a value indicating whether  DST is Enabled.
        /// </summary>
        bool DSTUser { get; }

        /// <summary>
        /// Gets a value indicating whether DailyDigest.
        /// </summary>
        bool DailyDigest { get; }

        /// <summary>
        ///   Gets DisplayName.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        ///   Gets Email.
        /// </summary>
        string Email { get; }

        /// <summary>
        ///   Gets a value indicating whether HasAvatarImage.
        /// </summary>
        bool HasAvatarImage { get; }

        /// <summary>
        ///   Gets a value indicating whether IsActiveExcluded.
        /// </summary>
        bool IsActiveExcluded { get; }

        /// <summary>
        ///   Gets a value indicating whether IsGuest.
        /// </summary>
        bool IsGuest { get; }

        /// <summary>
        ///   Gets Joined.
        /// </summary>
        DateTime? Joined { get; }

        /// <summary>
        ///   Gets LanguageFile.
        /// </summary>
        string LanguageFile { get; }

        /// <summary>
        ///   Gets LastVisit.
        /// </summary>
        DateTime? LastVisit { get; }

        /// <summary>
        ///   Gets Membership.
        /// </summary>
        MembershipUser Membership { get; }

        /// <summary>
        /// Gets NotificationSetting.
        /// </summary>
        UserNotificationSetting NotificationSetting { get; }

        /// <summary>
        ///   Gets Number of Posts.
        /// </summary>
        int? NumPosts { get; }

        /// <summary>
        ///   Gets a value indicating whether UseMobileTheme.
        /// </summary>
        bool UseMobileTheme { get; }

        /// <summary>
        ///   Gets a value indicating whether PMNotification.
        /// </summary>
        bool PMNotification { get; }

        /// <summary>
        ///   Gets Points.
        /// </summary>
        int? Points { get; }

        /// <summary>
        ///   Gets Profile.
        /// </summary>
        IYafUserProfile Profile { get; }

        /// <summary>
        ///   Gets RankName.
        /// </summary>
        string RankName { get; }

        /// <summary>
        ///   Gets Signature.
        /// </summary>
        string Signature { get; }

        /// <summary>
        ///   Gets ThemeFile.
        /// </summary>
        string ThemeFile { get; }

        /// <summary>
        ///   Gets TimeZone.
        /// </summary>
        int? TimeZone { get; }

        /// <summary>
        ///   Gets UserID.
        /// </summary>
        int UserID { get; }

        /// <summary>
        ///   Gets UserName.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Gets the text editor.
        /// </summary>
        /// <value>
        /// The text editor.
        /// </value>
        string TextEditor { get; }
    }
}