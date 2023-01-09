using System;

namespace FarsiLibrary.Utils;

public sealed class PersianDateTimeFormatInfo
{
    public static string[] AbbreviatedDayNames => PersianWeekDayNames.Default.DaysAbbr.ToArray();

    public static string[] AbbreviatedMonthGenitiveNames => PersianMonthNames.Default.Months.ToArray();

    public static string[] AbbreviatedMonthNames => PersianMonthNames.Default.Months.ToArray();

    public static string AMDesignator => "ق.ظ";

    public static string DateSeparator => "/";

    public static string[] DayNames => PersianWeekDayNames.Default.Days.ToArray();

    public static DayOfWeek FirstDayOfWeek => DayOfWeek.Saturday;

    public static string FullDateTimePattern => "tt hh:mm:ss yyyy mmmm dd dddd";

    public static string LongDatePattern => "yyyy MMMM dd, dddd";

    public static string LongTimePattern => "hh:mm:ss tt";

    public static string MonthDayPattern => "dd MMMM";

    public static string[] MonthGenitiveNames => PersianMonthNames.Default.Months.ToArray();

    public static string[] MonthNames => PersianMonthNames.Default.Months.ToArray();

    public static string PMDesignator => "ب.ظ";

    public static string ShortDatePattern => "yyyy/MM/dd";

    public static string[] ShortestDayNames => PersianWeekDayNames.Default.DaysAbbr.ToArray();

    public static string ShortTimePattern => "hh:mm tt";

    public static string TimeSeparator => ":";

    public static string YearMonthPattern => "yyyy, MMMM";

    public static string GetWeekDay(DayOfWeek day)
    {
        return DayNames[(int) day];
    }

    public static string GetWeekDayAbbr(DayOfWeek day)
    {
        return AbbreviatedDayNames[(int)day];
    }

    public static DayOfWeek GetDayOfWeek(int day)
    {
        return day switch
            {
                0 => DayOfWeek.Saturday,
                1 => DayOfWeek.Sunday,
                2 => DayOfWeek.Monday,
                3 => DayOfWeek.Tuesday,
                4 => DayOfWeek.Wednesday,
                5 => DayOfWeek.Thursday,
                6 => DayOfWeek.Friday,
                _ => throw new ArgumentOutOfRangeException(nameof(day), "invalid day value")
            };
    }

    public static string GetWeekDayByIndex(int day)
    {
        return GetWeekDay(GetDayOfWeek(day));
    }

    public static string GetWeekDayAbbrByIndex(int day)
    {
        return day switch
            {
                0 => GetWeekDayAbbr(DayOfWeek.Saturday),
                1 => GetWeekDayAbbr(DayOfWeek.Sunday),
                2 => GetWeekDayAbbr(DayOfWeek.Monday),
                3 => GetWeekDayAbbr(DayOfWeek.Tuesday),
                4 => GetWeekDayAbbr(DayOfWeek.Wednesday),
                5 => GetWeekDayAbbr(DayOfWeek.Thursday),
                6 => GetWeekDayAbbr(DayOfWeek.Friday),
                _ => throw new ArgumentOutOfRangeException(nameof(day), "invalid day value")
            };
    }

    public static int GetDayIndex(DayOfWeek day)
    {
        return day switch
            {
                DayOfWeek.Sunday => 1,
                DayOfWeek.Monday => 2,
                DayOfWeek.Tuesday => 3,
                DayOfWeek.Wednesday => 4,
                DayOfWeek.Thursday => 5,
                DayOfWeek.Friday => 6,
                DayOfWeek.Saturday => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(day))
            };
    }
}