// ***********************************************************************
// <copyright file="TimeSpanConverter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Globalization;

namespace ServiceStack.Text.Support;

/// <summary>
/// Class TimeSpanConverter.
/// </summary>
public class TimeSpanConverter
{
    /// <summary>
    /// The minimum serialized value
    /// </summary>
    private const string MinSerializedValue = "-P10675199DT2H48M5.4775391S";
    /// <summary>
    /// The maximum serialized value
    /// </summary>
    private const string MaxSerializedValue = "P10675199DT2H48M5.4775391S";

    /// <summary>
    /// Converts to xsdduration.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <returns>System.String.</returns>
    public static string ToXsdDuration(TimeSpan timeSpan)
    {
        if (timeSpan == TimeSpan.MinValue)
        {
            return MinSerializedValue;
        }

        if (timeSpan == TimeSpan.MaxValue)
        {
            return MaxSerializedValue;
        }

        var sb = StringBuilderThreadStatic.Allocate();

        sb.Append(timeSpan.Ticks < 0 ? "-P" : "P");

        double ticks = timeSpan.Ticks;
        if (ticks < 0)
        {
            ticks = -ticks;
        }

        var totalSeconds = ticks / TimeSpan.TicksPerSecond;
        var wholeSeconds = (long)totalSeconds;
        var seconds = wholeSeconds;
        var sec = seconds >= 60 ? seconds % 60 : seconds;
        var min = (seconds /= 60) >= 60 ? seconds % 60 : seconds;
        var hours = (seconds /= 60) >= 24 ? seconds % 24 : seconds;
        var days = seconds / 24;
        var remainingSecs = sec + (totalSeconds - wholeSeconds);

        if (days > 0)
        {
            sb.Append(days + "D");
        }

        if (days == 0 || hours + min + sec + remainingSecs > 0)
        {
            sb.Append('T');
            if (hours > 0)
            {
                sb.Append(hours + "H");
            }

            if (min > 0)
            {
                sb.Append(min + "M");
            }

            if (remainingSecs > 0)
            {
                var secFmt = string.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", remainingSecs);
                secFmt = secFmt.TrimEnd('0').TrimEnd('.');
                sb.Append(secFmt + "S");
            }
            else if (sb.Length == 2) //PT
            {
                sb.Append("0S");
            }
        }

        return StringBuilderThreadStatic.ReturnAndFree(sb);
    }

    /// <summary>
    /// Froms the duration of the XSD.
    /// </summary>
    /// <param name="xsdDuration">Duration of the XSD.</param>
    /// <returns>TimeSpan.</returns>
    public static TimeSpan FromXsdDuration(string xsdDuration)
    {
        switch (xsdDuration)
        {
            case MinSerializedValue:
                return TimeSpan.MinValue;
            case MaxSerializedValue:
                return TimeSpan.MaxValue;
        }

        long days = 0;
        long hours = 0;
        long minutes = 0;
        decimal seconds = 0;
        long sign = 1;

        if (xsdDuration.StartsWith('-'))
        {
            sign = -1;
            xsdDuration = xsdDuration[1..]; //strip sign
        }

        var t = xsdDuration[1..].SplitOnFirst('T'); //strip P

        var hasTime = t.Length == 2;

        var d = t[0].SplitOnFirst('D');
        if (d.Length == 2)
        {
            if (long.TryParse(d[0], out var day))
            {
                days = day;
            }
        }
        if (hasTime)
        {
            var h = t[1].SplitOnFirst('H');
            if (h.Length == 2)
            {
                if (long.TryParse(h[0], out var hour))
                {
                    hours = hour;
                }
            }

            var m = h[^1].SplitOnFirst('M');
            if (m.Length == 2)
            {
                if (long.TryParse(m[0], out var min))
                {
                    minutes = min;
                }
            }

            var s = m[^1].SplitOnFirst('S');
            if (s.Length == 2)
            {
                if (decimal.TryParse(s[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var millis))
                {
                    seconds = millis;
                }
            }
        }

        var totalSecs = 0
                        + days * 24 * 60 * 60
                        + hours * 60 * 60
                        + minutes * 60
                        + seconds;

        var interval = (long)(totalSecs * TimeSpan.TicksPerSecond * sign);

        return TimeSpan.FromTicks(interval);
    }
}