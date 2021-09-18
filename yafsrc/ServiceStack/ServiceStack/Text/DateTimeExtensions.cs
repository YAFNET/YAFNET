// ***********************************************************************
// <copyright file="DateTimeExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Globalization;
using ServiceStack.Text.Common;

namespace ServiceStack.Text
{
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
        private static readonly DateTime UnixEpochDateTimeUtc = new(UnixEpoch, DateTimeKind.Utc);
        /// <summary>
        /// The unix epoch date time unspecified
        /// </summary>
        private static readonly DateTime UnixEpochDateTimeUnspecified = new(UnixEpoch, DateTimeKind.Unspecified);
        /// <summary>
        /// The minimum date time UTC
        /// </summary>
        private static readonly DateTime MinDateTimeUtc = new(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Froms the unix time.
        /// </summary>
        /// <param name="unixTime">The unix time.</param>
        /// <returns>System.DateTime.</returns>
        public static DateTime FromUnixTime(this int unixTime)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
        }

        /// <summary>
        /// Converts to unixtimems.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>long.</returns>
        public static long ToUnixTimeMs(this DateTime dateTime)
        {
            var universal = ToDateTimeSinceUnixEpoch(dateTime);
            return (long)universal.TotalMilliseconds;
        }

        /// <summary>
        /// Converts to unixtime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>long.</returns>
        public static long ToUnixTime(this DateTime dateTime)
        {
            return dateTime.ToDateTimeSinceUnixEpoch().Ticks / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// Converts to datetimesinceunixepoch.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.TimeSpan.</returns>
        private static TimeSpan ToDateTimeSinceUnixEpoch(this DateTime dateTime)
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

        /// <summary>
        /// Converts to unixtimems.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns>long.</returns>
        public static long ToUnixTimeMs(this long ticks)
        {
            return (ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Froms the unix time ms.
        /// </summary>
        /// <param name="msSince1970">The ms since1970.</param>
        /// <returns>System.DateTime.</returns>
        public static DateTime FromUnixTimeMs(this long msSince1970)
        {
            return UnixEpochDateTimeUtc + TimeSpan.FromMilliseconds(msSince1970);
        }

        /// <summary>
        /// Froms the unix time ms.
        /// </summary>
        /// <param name="msSince1970">The ms since1970.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>System.DateTime.</returns>
        public static DateTime FromUnixTimeMs(this long msSince1970, TimeSpan offset)
        {
            return DateTime.SpecifyKind(UnixEpochDateTimeUnspecified + TimeSpan.FromMilliseconds(msSince1970) + offset, DateTimeKind.Local);
        }

        /// <summary>
        /// Rounds to second.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.DateTime.</returns>
        public static DateTime RoundToSecond(this DateTime dateTime)
        {
            return new(dateTime.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond, dateTime.Kind);
        }

        /// <summary>
        /// Converts to timeoffsetstring.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns>string.</returns>
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
        /// <returns>System.TimeSpan.</returns>
        public static TimeSpan FromTimeOffsetString(this string offsetString)
        {
            if (!offsetString.Contains(":"))
                offsetString = offsetString.Insert(offsetString.Length - 2, ":");

            offsetString = offsetString.TrimStart('+');

            return TimeSpan.Parse(offsetString);
        }

        /// <summary>
        /// Converts to stableuniversaltime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.DateTime.</returns>
        public static DateTime ToStableUniversalTime(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            return dateTime == DateTime.MinValue ? MinDateTimeUtc : PclExport.Instance.ToStableUniversalTime(dateTime);
        }
    }

}
