/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using ServiceStack.DataAnnotations;

namespace YAF.Types.Models;

using System.Text.RegularExpressions;

using YAF.Types.Constants;
using YAF.Types.Extensions;

/// <summary>
/// A class which represents the User table.
/// </summary>
[Serializable]
[UniqueConstraint(nameof(BoardID), nameof(Name))]
public class User : IEntity, IHaveBoardID, IHaveID
{
    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    public User()
    {
        try
        {
            this.Points = 1;
            this.PageSize = 5;
            this.Activity = true;
        }
        catch (Exception)
        {
            this.UserFlags.IsGuest = true;
            this.ProviderUserKey = null;
            this.PageSize = 5;
            this.Activity = true;
        }
    }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [Alias("UserID")]
    [AutoIncrement]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the board id.
    /// </summary>
    [References(typeof(Board))]
    [Required]
    [Index]
    public int BoardID { get; set; }

    /// <summary>
    /// Gets or sets the provider user key.
    /// </summary>
    [StringLength(255)]
    public string ProviderUserKey { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [Required]
    [Index]
    [StringLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    [Required]
    [Index]
    [StringLength(255)]
    [Default("")]
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    [StringLength(255)]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the joined.
    /// </summary>
    [Required]
    [Index]
    public DateTime Joined { get; set; }

    /// <summary>
    /// Gets or sets the last visit.
    /// </summary>
    [Required]
    public DateTime LastVisit { get; set; }

    /// <summary>
    /// Gets or sets the IP Address.
    /// </summary>
    [StringLength(39)]
    public string IP { get; set; }

    /// <summary>
    /// Gets or sets the number of posts.
    /// </summary>
    [Required]
    public int NumPosts { get; set; }

    /// <summary>
    /// Gets or sets the time zone.
    /// </summary>
    public string TimeZone { get; set; }

    /// <summary>
    ///   Gets TimeZone.
    /// </summary>
    [Ignore]
    public TimeZoneInfo TimeZoneInfo
    {
        get
        {
            TimeZoneInfo timeZoneInfo;

            try
            {
                var tz = this.TimeZone;

                if (Regex.IsMatch(tz, @"^[\-?\+?\d]*$",
                        RegexOptions.None,
                        TimeSpan.FromMilliseconds(100)))
                {
                    return TimeZoneInfo.Local;
                }

                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(tz);
            }
            catch (Exception)
            {
                timeZoneInfo = TimeZoneInfo.Local;
            }

            return timeZoneInfo;
        }
    }

    /// <summary>
    /// Gets or sets the avatar.
    /// </summary>
    [StringLength(255)]
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the signature.
    /// </summary>
    [CustomField(OrmLiteVariables.MaxTextUnicode)]
    public string Signature { get; set; }

    /// <summary>
    /// Gets or sets the avatar image.
    /// </summary>
    public byte[] AvatarImage { get; set; }

    /// <summary>
    /// Gets or sets the avatar image type.
    /// </summary>
    [StringLength(50)]
    public string AvatarImageType { get; set; }

    /// <summary>
    /// Gets or sets the rank id.
    /// </summary>
    [References(typeof(Rank))]
    [Required]
    public int RankID { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the language file.
    /// </summary>
    [StringLength(50)]
    public string LanguageFile { get; set; }

    /// <summary>
    /// Gets or sets the theme file.
    /// </summary>
    [StringLength(50)]
    public string ThemeFile { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether auto watch topics.
    /// </summary>
    [Required]
    [Default(typeof(bool), "1")]
    public bool AutoWatchTopics { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether daily digest.
    /// </summary>
    [Required]
    [Default(typeof(bool), "0")]
    public bool DailyDigest { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether activity.
    /// </summary>
    [Required]
    [Default(typeof(bool), "1")]
    public bool Activity { get; set; }

    /// <summary>
    /// Gets or sets the notification type.
    /// </summary>
    [Default(10)]
    public int? NotificationType { get; set; }

    /// <summary>
    /// Gets NotificationSetting.
    /// </summary>
    [Ignore]
    public UserNotificationSetting NotificationSetting
    {
        get
        {
            var value = this.NotificationType ?? 0;

            return value.ToEnum<UserNotificationSetting>();
        }
    }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    [Required]
    [Default(0)]
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the user flags.
    /// </summary>
    [Ignore]
    public UserFlags UserFlags
    {
        get => new(this.Flags);

        set => this.Flags = value.BitValue;
    }

    /// <summary>
    /// Gets or sets the block flags.
    /// </summary>
    [Required]
    [Default(0)]
    public int BlockFlags { get; set; }

    /// <summary>
    /// Gets or sets the block.
    /// </summary>
    [Ignore]
    public UserBlockFlags Block
    {
        get => new(this.BlockFlags);

        set => this.BlockFlags = value.BitValue;
    }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    [Required]
    [Default(1)]
    public int Points { get; set; }

    /// <summary>
    /// Gets or sets the culture.
    /// </summary>
    [StringLength(10)]
    public string Culture { get; set; }

    /// <summary>
    /// Gets or sets the user style.
    /// </summary>
    [Index]
    [StringLength(510)]
    public string UserStyle { get; set; }

    /// <summary>
    /// Gets or sets the suspended reason.
    /// </summary>
    [CustomField(OrmLiteVariables.MaxTextUnicode)]
    public string SuspendedReason { get; set; }

    /// <summary>
    /// Gets or sets the suspended by.
    /// </summary>
    [Required]
    [Default(0)]
    public int SuspendedBy { get; set; }

    /// <summary>
    /// Gets or sets the Page Size.
    /// </summary>
    [Required]
    [Default(5)]
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use Dark Mode or not.
    /// </summary>
    [Ignore]
    public bool DarkMode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="User"/> is selected.
    /// </summary>
    /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
    [Ignore]
    public bool Selected { get; set; }
}