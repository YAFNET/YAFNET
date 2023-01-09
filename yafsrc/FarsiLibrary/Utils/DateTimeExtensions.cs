using System;
using FarsiLibrary.Utils.Formatter;
using FarsiLibrary.Utils.Internals;

namespace FarsiLibrary.Utils;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts the DateTime to a PersianDate equivalant.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static PersianDate ToPersianDate(this DateTime dateTime)
    {
        if (dateTime < CultureHelper.PersianCulture.Calendar.MinSupportedDateTime ||
            dateTime > CultureHelper.PersianCulture.Calendar.MaxSupportedDateTime)
        {
            return null;
        }

        return new PersianDate(dateTime);
    }

    public static PersianDate Combine(this PersianDate datePart, PersianDate timePart)
    {
        return new(datePart.Year, datePart.Month, datePart.Day, timePart.Hour, timePart.Minute, timePart.Second, timePart.Millisecond);
    }

    public static PersianDate EndOfMonth(this PersianDate dateTime)
    {
        var dt = dateTime.ToDateTime();
        var start = StartOfMonth(dateTime).ToDateTime();
        var pc = CultureHelper.PersianCalendar;
        var nextMonth = pc.AddMonths(start, 1);

        return pc.AddDays(nextMonth, -1);
    }

    public static PersianDate StartOfMonth(this PersianDate dateTime)
    {
        return new(dateTime.Year, dateTime.Month, 1);
    }

    public static PersianDate EndOfWeek(this PersianDate dateTime)
    {
        var dt = dateTime.ToDateTime();
        var diff = GetEndOfWeekDiff(dt);
        var pc = CultureHelper.PersianCalendar;

        return pc.AddDays(dt, diff);
    }

    public static PersianDate StartOfWeek(this PersianDate dateTime)
    {
        var dt = dateTime.ToDateTime();
        var diff = GetStartOfWeekDiff(dt);
        var pc = CultureHelper.PersianCalendar;
            
        return pc.AddDays(dt, -diff);
    }

    /// <summary>
    /// Converts the PersianDate to a DateTime equivalant.
    /// </summary>
    /// <param name="persianDate"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(this PersianDate persianDate)
    {
        return PersianDateConverter.ToGregorianDateTime(persianDate);
    }

    private static int GetStartOfWeekDiff(DateTime dateTime)
    {
        var diff = dateTime.DayOfWeek switch
            {
                DayOfWeek.Saturday => 0,
                DayOfWeek.Sunday => 1,
                DayOfWeek.Monday => 2,
                DayOfWeek.Tuesday => 3,
                DayOfWeek.Wednesday => 4,
                DayOfWeek.Thursday => 5,
                DayOfWeek.Friday => 6,
                _ => 0
            };

        return diff;
    }

    public static string ToPrettyTime(this DateTime date)
    {
        var pretty = new PrettyTime();
        return pretty.Format(date);
    }

    private static int GetEndOfWeekDiff(DateTime dateTime)
    {
        var diff = dateTime.DayOfWeek switch
            {
                DayOfWeek.Saturday => 6,
                DayOfWeek.Sunday => 5,
                DayOfWeek.Monday => 4,
                DayOfWeek.Tuesday => 3,
                DayOfWeek.Wednesday => 2,
                DayOfWeek.Thursday => 1,
                DayOfWeek.Friday => 0,
                _ => 0
            };

        return diff;
    }
}