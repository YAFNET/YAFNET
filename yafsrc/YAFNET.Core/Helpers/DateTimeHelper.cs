/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Core.Helpers;

using System;
using System.Text.RegularExpressions;

using YAF.Core.Context;

/// <summary>
/// DateTime Helper
/// </summary>
public static partial class DateTimeHelper
{
    /// <summary>
    /// The second.
    /// </summary>
    private const int Second = 1;

    /// <summary>
    /// The minute.
    /// </summary>
    private const int Minute = 60 * Second;

    /// <summary>
    /// The hour.
    /// </summary>
    private const int Hour = 60 * Minute;

    /// <summary>
    /// The day.
    /// </summary>
    private const int Day = 24 * Hour;

    /// <summary>
    /// The month.
    /// </summary>
    private const int Month = 30 * Day;

    /// <summary>
    /// the SQL compatible DateTime Min Value
    /// </summary>
    /// <returns>
    /// Returns the SQL compatible DateTime Min Value
    /// </returns>
    public static DateTime SqlDbMinTime()
    {
        return DateTime.MinValue.AddYears(1902);
    }

    /// <summary>
    /// Gets the time zone by id
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>Returns the Time Zone Info</returns>
    public static TimeZoneInfo GetTimeZoneInfo(string input)
    {
        if (LocalRegex().IsMatch(input))
        {
            return TimeZoneInfo.Local;
        }

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(input);
        }
        catch (Exception)
        {
            return TimeZoneInfo.Local;
        }
    }

    /// <summary>
    /// Gets the time zone offset.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>
    /// Returns the Offset
    /// </returns>
    public static int GetTimeZoneOffset(string input)
    {
        TimeZoneInfo timeZone;

        try
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(input);
        }
        catch (Exception)
        {
            timeZone = TimeZoneInfo.Local;
        }

        return GetTimeZoneOffset(timeZone);
    }

    /// <summary>
    /// Gets the time zone offset.
    /// </summary>
    /// <param name="timeZoneInfo">The time zone information.</param>
    /// <returns>Returns the Offset</returns>
    public static int GetTimeZoneOffset(TimeZoneInfo timeZoneInfo)
    {
        // TotalMinutes preserves both the sign and the minutes component of the offset,
        // unlike formatting BaseUtcOffset with the "hh" custom format (which drops minutes
        // entirely, truncating half/quarter-hour offsets such as +05:30 or +05:45 to whole hours).
        return (int)timeZoneInfo.BaseUtcOffset.TotalMinutes;
    }

    /// <summary>
    /// The date diff day.
    /// </summary>
    /// <param name="startDate">
    /// The start date.
    /// </param>
    /// <param name="endDate">
    /// The end date.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static int DateDiffDay(DateTime startDate, DateTime endDate)
    {
        return (endDate.Date - startDate.Date).Days;
    }

    /// <summary>
    /// Returns the relative version of the provided DateTime, relative to now. E.g.: "2 days ago", or "in 6 months".
    /// References: https://stackoverflow.com/a/5427203
    /// </summary>
    /// <param name="dateTime">The DateTime to compare to Now</param>
    /// <returns>A friendly string</returns>
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var localizer = BoardContext.Current.Get<ILocalization>();

        if (DateTime.UtcNow.Ticks == dateTime.Ticks)
        {
            return localizer.GetText("RELATIVE_TIME", "NOW");
        }

        var isFuture = DateTime.UtcNow.Ticks < dateTime.Ticks;
        var ts = DateTime.UtcNow.Ticks < dateTime.Ticks
                     ? new TimeSpan(dateTime.Ticks - DateTime.UtcNow.Ticks)
                     : new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks);

        // the texts are always read from RELATIVE_TIME, a tag without the page is searched on all pages,
        // e.g. "SS" (seconds) is also the country South Sudan
        string Text(string tag, object value = null) =>
            value is null ? localizer.GetText("RELATIVE_TIME", tag) : localizer.GetText("RELATIVE_TIME", tag).FormatWith(value);

        var text = ts.TotalSeconds switch
        {
            < 1 * Minute => ts.Seconds == 1 ? Text("S") : Text("SS", ts.Seconds),
            < 2 * Minute => Text("M"),
            < 45 * Minute => Text("MM", ts.Minutes),
            < 90 * Minute => Text("H"),
            < 24 * Hour => Text("HH", ts.Hours),
            < 48 * Hour => Text("D"),
            < 30 * Day => Text("DD", ts.Days),
            < 12 * Month => ts.Days / 30 <= 1 ? Text("MO") : Text("MOMO", ts.Days / 30),
            _ => ts.Days / 365 <= 1 ? Text("Y") : Text("YY", ts.Days / 365)
        };

        return Text(isFuture ? "FUTURE" : "PAST", text);
    }

    [GeneratedRegex(@"^[\-?\+?\d]*$")]
    private static partial Regex LocalRegex();
}