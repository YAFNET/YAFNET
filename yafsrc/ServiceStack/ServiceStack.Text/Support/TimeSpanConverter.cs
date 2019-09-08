using System;

namespace ServiceStack.Text.Support
{
    public class TimeSpanConverter
    {
        public static string ToXsdDuration(TimeSpan timeSpan)
        {
            var sb = StringBuilderThreadStatic.Allocate();

            sb.Append(timeSpan.Ticks < 0 ? "-P" : "P");

            double ticks = Math.Abs(timeSpan.Ticks);
            var totalSeconds = ticks / TimeSpan.TicksPerSecond;
            var wholeSeconds = (int) totalSeconds;
            var seconds = wholeSeconds;
            var sec = seconds >= 60 ? seconds % 60 : seconds;
            var min = (seconds = seconds / 60) >= 60 ? seconds % 60 : seconds;
            var hours = (seconds = seconds / 60) >= 24 ? seconds % 24 : seconds;
            var days = seconds / 24;
            var remainingSecs = sec + (totalSeconds - wholeSeconds);

            if (days > 0)
                sb.Append($"{days}D");

            if (days == 0 || hours + min + sec + remainingSecs > 0)
            {
                sb.Append("T");
                if (hours > 0)
                    sb.Append($"{hours}H");

                if (min > 0)
                    sb.Append($"{min}M");

                if (remainingSecs > 0)
                {
                    var secFmt = $"{remainingSecs:0.0000000}";
                    secFmt = secFmt.TrimEnd('0').TrimEnd('.');
                    sb.Append($"{secFmt}S");
                }
                else if (sb.Length == 2)
                {
                    // PT
                    sb.Append("0S");
                }
            }

            return StringBuilderThreadStatic.ReturnAndFree(sb);
        }

        public static TimeSpan FromXsdDuration(string xsdDuration)
        {
            var days = 0;
            var hours = 0;
            var minutes = 0;
            decimal seconds = 0;
            var sign = 1;

            if (xsdDuration.StartsWith("-", StringComparison.Ordinal))
            {
                sign = -1;
                xsdDuration = xsdDuration.Substring(1); // strip sign
            }

            var t = xsdDuration.Substring(1).SplitOnFirst('T'); // strip P

            var hasTime = t.Length == 2;

            var d = t[0].SplitOnFirst('D');
            if (d.Length == 2)
            {
                int day;
                if (int.TryParse(d[0], out day))
                    days = day;
            }

            if (hasTime)
            {
                var h = t[1].SplitOnFirst('H');
                if (h.Length == 2)
                {
                    int hour;
                    if (int.TryParse(h[0], out hour))
                        hours = hour;
                }

                var m = h[h.Length - 1].SplitOnFirst('M');
                if (m.Length == 2)
                {
                    int min;
                    if (int.TryParse(m[0], out min))
                        minutes = min;
                }

                var s = m[m.Length - 1].SplitOnFirst('S');
                if (s.Length == 2)
                {
                    decimal millis;
                    if (decimal.TryParse(s[0], out millis))
                        seconds = millis;
                }
            }

            var totalSecs = 0
                    + days * 24 * 60 * 60
                    + hours * 60 * 60
                    + minutes * 60
                    + seconds;

            var interval = (long) (totalSecs * TimeSpan.TicksPerSecond * sign);

            return TimeSpan.FromTicks(interval);
        }
    }
}
