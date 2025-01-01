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
        var utcOffSet = timeZoneInfo.BaseUtcOffset;
        var timeZone = utcOffSet < TimeSpan.Zero
                           ? $"-{utcOffSet:hh}"
                           : utcOffSet.ToString("hh");

        return (timeZone.ToType<decimal>() * 60).ToType<int>();
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

        var delta = ts.TotalSeconds;

        switch (delta)
        {
            case < 1 * Minute:
                var text = ts.Seconds == 1
                    ? localizer.GetText("RELATIVE_TIME", "S")
                    : localizer.GetTextFormatted("SS", ts.Seconds);

                return isFuture
                    ? localizer.GetTextFormatted(
                        "FUTURE", text
                    )
                    : localizer.GetTextFormatted(
                        "PAST",
                        text);
            case < 2 * Minute:
                return isFuture
                           ? localizer.GetTextFormatted("FUTURE", localizer.GetText("RELATIVE_TIME", "M"))
                           : localizer.GetTextFormatted("PAST", localizer.GetText("RELATIVE_TIME", "M"));
            case < 45 * Minute:
                return isFuture
                           ? localizer.GetTextFormatted(
                               "FUTURE",
                               localizer.GetText("RELATIVE_TIME", "MM").FormatWith(ts.Minutes))
                           : localizer.GetTextFormatted(
                               "PAST",
                               localizer.GetText("RELATIVE_TIME", "MM").FormatWith(ts.Minutes));
            case < 90 * Minute:
                return isFuture
                           ? localizer.GetTextFormatted("FUTURE", localizer.GetText("RELATIVE_TIME", "H"))
                           : localizer.GetTextFormatted("PAST", localizer.GetText("RELATIVE_TIME", "H"));
            case < 24 * Hour:
                return isFuture
                           ? localizer.GetTextFormatted("FUTURE", localizer.GetTextFormatted("HH", ts.Hours))
                           : localizer.GetTextFormatted("PAST", localizer.GetTextFormatted("HH", ts.Hours));
            case < 48 * Hour:
                return isFuture
                           ? localizer.GetTextFormatted("FUTURE", localizer.GetText("RELATIVE_TIME", "D"))
                           : localizer.GetTextFormatted("PAST", localizer.GetText("RELATIVE_TIME", "D"));
            case < 30 * Day:
                return isFuture
                           ? localizer.GetTextFormatted("FUTURE", localizer.GetTextFormatted("DD", ts.Days))
                           : localizer.GetTextFormatted("PAST", localizer.GetTextFormatted("DD", ts.Days));
            case < 12 * Month:
                {
                    var months = Convert.ToInt32(Math.Floor((double) ts.Days / 30));

                    var monthText = months <= 1
                        ? localizer.GetText("RELATIVE_TIME", "MO")
                        : localizer.GetTextFormatted("MOMO", months);

                    return isFuture
                               ? localizer.GetTextFormatted(
                                   "FUTURE",
                                   monthText)
                               : localizer.GetTextFormatted(
                                   "PAST",
                                   monthText);
                }
            default:
                {
                    var years = Convert.ToInt32(Math.Floor((double) ts.Days / 365));

                    var yearsText = years <= 1
                        ? localizer.GetText("RELATIVE_TIME", "Y")
                        : localizer.GetTextFormatted("YY", years);

                    return isFuture
                        ? localizer.GetTextFormatted(
                            "FUTURE", yearsText
                        )
                        : localizer.GetTextFormatted(
                            "PAST",
                            yearsText);
                }
        }
    }

    [GeneratedRegex(@"^[\-?\+?\d]*$")]
    private static partial Regex LocalRegex();
}