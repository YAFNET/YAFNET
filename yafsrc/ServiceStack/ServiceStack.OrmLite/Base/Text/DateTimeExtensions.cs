// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="ServiceStack, Inc.">
//   Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>
//   Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;

using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// A fast, standards-based, serialization-issue free DateTime serializer.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// The unix epoch
    /// </summary>
    public const long UnixEpoch = 621355968000000000L;
    /// <summary>
    /// The unix epoch date time UTC
    /// </summary>
    private readonly static DateTime UnixEpochDateTimeUtc = new(UnixEpoch, DateTimeKind.Utc);
    /// <summary>
    /// The unix epoch date time unspecified
    /// </summary>
    public readonly static DateTime UnixEpochDateTimeUnspecified = new(UnixEpoch, DateTimeKind.Unspecified);
    /// <summary>
    /// The minimum date time UTC
    /// </summary>
    private readonly static DateTime MinDateTimeUtc = new(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Froms the unix time.
    /// </summary>
    /// <param name="unixTime">The unix time.</param>
    /// <returns>DateTime.</returns>
    public static DateTime FromUnixTime(this int unixTime)
    {
        return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
    }

    /// <param name="dateTime">The date time.</param>
    extension(DateTime dateTime)
    {
        /// <summary>
        /// Converts to unixtimems.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long ToUnixTimeMs()
        {
            var universal = ToDateTimeSinceUnixEpoch(dateTime);
            return (long)universal.TotalMilliseconds;
        }

        /// <summary>
        /// Converts to unixtime.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long ToUnixTime()
        {
            return (dateTime.ToDateTimeSinceUnixEpoch().Ticks) / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// Converts to datetimesinceunixepoch.
        /// </summary>
        /// <returns>TimeSpan.</returns>
        private TimeSpan ToDateTimeSinceUnixEpoch()
        {
            var dtUtc = dateTime;
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dtUtc = dateTime.Kind == DateTimeKind.Unspecified && dateTime > DateTime.MinValue && dateTime < DateTime.MaxValue
                    ? DateTime.SpecifyKind(dateTime.Subtract(DateTimeSerializer.LocalTimeZone.GetUtcOffset(dateTime)), DateTimeKind.Utc)
                    : dateTime.ToStableUniversalTime();
            }

            var universal = dtUtc.Subtract(UnixEpochDateTimeUtc);
            return universal;
        }
    }

    /// <summary>
    /// Converts to unixtimems.
    /// </summary>
    /// <param name="ticks">The ticks.</param>
    /// <returns>System.Int64.</returns>
    public static long ToUnixTimeMs(this long ticks)
    {
        return (ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
    }

    /// <param name="dateOnly">The date only.</param>
    extension(DateOnly dateOnly)
    {
        /// <summary>
        /// Converts to unixtimems.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long ToUnixTimeMs()
        {
            return dateOnly.ToDateTime(default, DateTimeKind.Utc).ToUnixTimeMs();
        }

        /// <summary>
        /// Converts to unixtime.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long ToUnixTime()
        {
            return dateOnly.ToDateTime(default, DateTimeKind.Utc).ToUnixTime();
        }
    }

    /// <param name="msSince1970">The ms since1970.</param>
    extension(long msSince1970)
    {
        /// <summary>
        /// Froms the unix time ms.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime FromUnixTimeMs()
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromMilliseconds(msSince1970);
        }

        /// <summary>
        /// Froms the unix time ms.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>DateTime.</returns>
        public DateTime FromUnixTimeMs(TimeSpan offset)
        {
            return DateTime.SpecifyKind(UnixEpochDateTimeUnspecified + TimeSpan.FromMilliseconds(msSince1970) + offset, DateTimeKind.Local);
        }
    }

    /// <param name="dateTime">The date time.</param>
    extension(DateTime dateTime)
    {
        /// <summary>
        /// Rounds to second.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime RoundToSecond()
        {
            return new DateTime((dateTime.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond, dateTime.Kind);
        }

        /// <summary>
        /// Truncates the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>DateTime.</returns>
        public DateTime Truncate(TimeSpan timeSpan)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
    }

    /// <summary>
    /// Converts to timeoffsetstring.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="seperator">The seperator.</param>
    /// <returns>System.String.</returns>
    public static string ToTimeOffsetString(this TimeSpan offset, string seperator = "")
    {
        var hours = Math.Abs(offset.Hours).ToString(CultureInfo.InvariantCulture);
        var minutes = Math.Abs(offset.Minutes).ToString(CultureInfo.InvariantCulture);
        return (offset < TimeSpan.Zero ? "-" : "+")
               + (hours.Length == 1 ? "0" + hours : hours)
               + seperator
               + (minutes.Length == 1 ? "0" + minutes : minutes);
    }

    /// <summary>
    /// Froms the time offset string.
    /// </summary>
    /// <param name="offsetString">The offset string.</param>
    /// <returns>TimeSpan.</returns>
    public static TimeSpan FromTimeOffsetString(this string offsetString)
    {
        if (!offsetString.Contains(":"))
        {
            offsetString = offsetString.Insert(offsetString.Length - 2, ":");
        }

        offsetString = offsetString.TrimStart('+');

        return TimeSpan.Parse(offsetString);
    }

    /// <summary>
    /// Converts to stableuniversaltime.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>DateTime.</returns>
    public static DateTime ToStableUniversalTime(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            return dateTime;
        }

        if (dateTime == DateTime.MinValue)
        {
            return MinDateTimeUtc;
        }

        return PclExport.Instance.ToStableUniversalTime(dateTime);
    }
}