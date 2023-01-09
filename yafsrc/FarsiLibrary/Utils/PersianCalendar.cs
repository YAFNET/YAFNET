namespace FarsiLibrary.Utils;

using System;
using System.Globalization;

using FarsiLibrary.Localization;
using FarsiLibrary.Utils.Exceptions;

/// <summary>
/// PersianCalendar calendar. Persian calendar, also named Jalaali calendar, was first based on Solar year by Omar Khayyam, the great Iranian poet, astrologer and scientist.
/// Jalaali calendar is approximately 365 days. Each of the first six months in the Jalaali calendar has 31 days, each of the next five months has 30 days, and the last month has 29 days in a common year and 30 days in a leap year. A leap year is a year that, when divided by 33, has a remainder of 1, 5, 9, 13, 17, 22, 26, or 30. For example, the year 1370 is a leap year because dividing it by 33 yields a remainder of 17. There are approximately 8 leap years in every 33 year cycle.
/// </summary>
[Serializable]
public sealed class PersianCalendar : Calendar
{
    private int twoDigitYearMax = 1399;

    internal const long MaxDateTimeTicks = 196036416000000000L;

    /// <summary>
    /// Maximum amount of month that can be added or 
    /// removed to / from a DateTime instance.
    /// </summary>
    internal const int MaxMonthDifference = 120000;

    /// <summary>
    /// Represents the current era.
    /// </summary>
    /// <remarks>The Persian calendar recognizes only A.P (Anno Persarum) era.</remarks>
    public const int PersianEra = 1;

    /// <summary>
    /// Returns a DateTime that is the specified number of months away from the specified DateTime.
    /// </summary>
    /// <param name="time">The DateTime instance to add.</param>
    /// <param name="months">The number of months to add.</param>
    /// <returns>The DateTime that results from adding the specified number of months to the specified DateTime.</returns>
    /// <remarks>
    /// The year part of the resulting DateTime is affected if the resulting month is beyond the last month of the current year. The day part of the resulting DateTime is also affected if the resulting day is not a valid day in the resulting month of the resulting year; it is changed to the last valid day in the resulting month of the resulting year. The time-of-day part of the resulting DateTime remains the same as the specified DateTime.
    /// 
    /// For example, if the specified month is Ordibehesht, which is the 2nd month and has 31 days, the specified day is the 31th day of that month, and the value of the months parameter is -3, the resulting year is one less than the specified year, the resulting month is Bahman, and the resulting day is the 30th day, which is the last day in Bahman.
    /// 
    /// If the value of the months parameter is negative, the resulting DateTime would be earlier than the specified DateTime.
    /// </remarks>
    public override DateTime AddMonths(DateTime time, int months)
    {
        if (Math.Abs(months) > MaxMonthDifference)
        {
            throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidMonth));
        }

        var year = GetYear(true, time);
        var month = GetMonth(false, time);
        var day = GetDayOfMonth(false, time);

        month += (year - 1) * 12 + months;
        year = (month - 1) / 12 + 1;
        month -= (year - 1) * 12;

        if (day > 29)
        {
            var maxday = GetDaysInMonth(false, year, month, 0);
            if (maxday < day) day = maxday;
        }

        DateTime dateTime;
        
        try
        {
            dateTime = this.ToDateTime(year, month, day, 0, 0, 0, 0) + time.TimeOfDay;
        }
        catch (Exception)
        {
            throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidDateTime));
        }

        return dateTime;
    }

    /// <summary>
    /// Algorithm Type
    /// </summary>
    public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.SolarCalendar;

    /// <summary>
    /// Maximum supported date time by this calendar.
    /// </summary>
    public override DateTime MaxSupportedDateTime => PersianDate.MaxValue;

    /// <summary>
    /// Minimum supported date time by this calendar.
    /// </summary>
    public override DateTime MinSupportedDateTime => PersianDate.MinValue;

    /// <summary>
    /// Returns a DateTime that is the specified number of years away from the specified DateTime.
    /// </summary>
    /// <param name="time">The DateTime instance to add.</param>
    /// <param name="years">The number of years to add.</param>
    /// <returns>The DateTime that results from adding the specified number of years to the specified DateTime.</returns>
    /// <remarks>
    /// The day part of the resulting DateTime is affected if the resulting day is not a valid day in the resulting month of the resulting year; it is changed to the last valid day in the resulting month of the resulting year. The time-of-day part of the resulting DateTime remains the same as the specified DateTime.
    /// 
    /// For example, Esfand has 29 days, except during leap years when it has 30 days. If the specified Date is the 30th day of Esfand in a leap year and the value of years is 1, the resulting Date will be the 29th day of Esfand in the following year.
    /// 
    /// If years is negative, the resulting DateTime would be earlier than the specified DateTime.
    /// </remarks>
    public override DateTime AddYears(DateTime time, int years)
    {
        var year = GetYear(true, time);
        var month = GetMonth(false, time);
        var day = GetDayOfMonth(false, time);
        year += years;
            
        if (day == 30 && month == 12)
        {
            if (!IsLeapYear(false, year, 0))
                day = 29;
        }
            
        try
        {
            return this.ToDateTime(year, month, day, 0, 0, 0, 0) + time.TimeOfDay;
        }
        catch (Exception)
        {
            throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidDateTime));
        }
    }

    /// <summary>
    /// Gets the day of the month in the specified DateTime.
    /// </summary>
    /// <param name="time">The DateTime instance to read.</param>
    /// <returns>An integer from 1 to 31 that represents the day of the month in time.</returns>
    public override int GetDayOfMonth(DateTime time)
    {
        return GetDayOfMonth(true, time);
    }

    private static int GetDayOfMonth(bool validate, DateTime time)
    {
        var days = GetDayOfYear(validate, time);
            
        for (var i = 0; i < 6; i++)
        {
            if (days <= 31)
            {
                return days;
            }

            days -= 31;
        }
            
        for (var i = 0; i < 5; i++)
        {
            if (days <= 30)
            {
                return days;
            }

            days -= 30;
        }

        return days;
    }

    /// <summary>
    /// Gets the day of the week in the specified DateTime.
    /// </summary>
    /// <param name="time">The DateTime instance to read.</param>
    /// <returns>A DayOfWeek value that represents the day of the week in time.</returns>
    /// <remarks>The DayOfWeek values are Sunday which indicates YekShanbe', Monday which indicates DoShanbe', Tuesday which indicates SeShanbe', Wednesday which indicates ChaharShanbe', Thursday which indicates PanjShanbe', Friday which indicates Jom'e, and Saturday which indicates Shanbe'.</remarks>
    public override DayOfWeek GetDayOfWeek(DateTime time)
    {
        return time.DayOfWeek;
    }

    /// <summary>
    /// Gets the day of the year in the specified DateTime.
    /// </summary>
    /// <param name="time">The DateTime instance to read.</param>
    /// <returns>An integer from 1 to 366 that represents the day of the year in time.</returns>
    public override int GetDayOfYear(DateTime time)
    {
        return GetDayOfYear(true, time);
    }

    private static int GetDayOfYear(bool validate, DateTime time)
    {
        int year;
        GetYearAndRemainingDays(validate, time, out year, out var days);
        return days;
    }

    /// <summary>
    /// Gets the number of days in the specified month.
    /// </summary>
    /// <param name="year">An integer that represents the year.</param>
    /// <param name="month">An integer that represents the month.</param>
    /// <param name="era">An integer that represents the era.</param>
    /// <returns>The number of days in the specified month in the specified year in the specified era.</returns>
    /// <remarks>For example, this method might return 29 or 30 for Esfand (month = 12), depending on whether year is a leap year.</remarks>
    public override int GetDaysInMonth(int year, int month, int era)
    {
        return GetDaysInMonth(true, year, month, era);
    }

    private static int GetDaysInMonth(bool validate, int year, int month, int era)
    {
        CheckEraRange(validate, era);
        CheckYearRange(validate, year);
        CheckMonthRange(validate, month);

        switch (month)
        {
            case < 7:
                return 31;
            case < 12:
                return 30;
        }

        return IsLeapYear(false, year, 0) ? 30 : 29;
    }

    /// <summary>
    /// Gets the number of days in the year specified by the year and era parameters.
    /// </summary>
    /// <param name="year">An integer that represents the year.</param>
    /// <param name="era">An integer that represents the era.</param>
    /// <returns>The number of days in the specified year in the specified era.</returns>
    /// <remarks>For example, this method might return 365 or 366, depending on whether year is a leap year.</remarks>
    public override int GetDaysInYear(int year, int era)
    {
        return GetDaysInYear(true, year, era);
    }

    private static int GetDaysInYear(bool validate, int year, int era)
    {
        return IsLeapYear(validate, year, era) ? 366 : 365;
    }

    /// <summary>
    /// Gets the era in the specified DateTime.
    /// </summary>
    /// <param name="time">The DateTime instance to read.</param>
    /// <returns>An integer that represents the era in time.</returns>
    /// <remarks>The Persian calendar recognizes one era: A.P. (Latin "Anno Persarum", which means "the year of/for Persians").</remarks>
    public override int GetEra(DateTime time)
    {
        CheckTicksRange(true, time);
        return PersianEra;
    }

    /// <summary>
    /// Gets the month in the specified DateTime.
    /// </summary>
    /// <param name="time">The DateTime instance to read.</param>
    /// <returns>An integer between 1 and 12 that represents the month in time.</returns>
    /// <remarks>Month 1 indicates Farvardin, month 2 indicates Ordibehesht, month 3 indicates Khordad, month 4 indicates Tir, month 5 indicates Amordad, month 6 indicates Shahrivar, month 7 indicates Mehr, month 8 indicates Aban, month 9 indicates Azar, month 10 indicates Dey, month 11 indicates Bahman, and month 12 indicates Esfand.</remarks>
    public override int GetMonth(DateTime time)
    {
        return GetMonth(true, time);
    }

    private static int GetMonth(bool validate, DateTime time)
    {
        var days = GetDayOfYear(validate, time);
        return days switch
            {
                <= 31 => 1,
                <= 62 => 2,
                <= 93 => 3,
                <= 124 => 4,
                <= 155 => 5,
                <= 186 => 6,
                <= 216 => 7,
                <= 246 => 8,
                <= 276 => 9,
                <= 306 => 10,
                _ => days <= 336 ? 11 : 12
            };
    }

    /// <summary>
    /// Gets the number of months in the year specified by the year and era parameters.
    /// </summary>
    /// <param name="year">An integer that represents the year.</param>
    /// <param name="era">An integer that represents the era.</param>
    /// <returns>The number of months in the specified year in the specified era.</returns>
    public override int GetMonthsInYear(int year, int era)
    {
        CheckEraRange(true, era);
        CheckYearRange(true, year);
        return 12;
    }

    /// <summary>
    /// Gets the year in the specified DateTime.
    /// </summary>
    /// <param name="time">The DateTime instance to read.</param>
    /// <returns>An integer between 1 and 9378 that represents the year in time.</returns>
    public override int GetYear(DateTime time)
    {
        return GetYear(true, time);
    }

    private static int GetYear(bool validate, DateTime time)
    {
        int days;
        GetYearAndRemainingDays(validate, time, out var year, out days);
        return year;
    }

    /// <summary>
    /// Determines whether the Date specified by the year, month, day, and era parameters is a leap day.
    /// </summary>
    /// <param name="year">An integer that represents the year.</param>
    /// <param name="month">An integer that represents the month.</param>
    /// <param name="day">An integer that represents the day.</param>
    /// <param name="era">An integer that represents the era.</param>
    /// <returns>true if the specified day is a leap day; otherwise, false.</returns>
    /// <remarks>
    /// In the Persian calendar leap years are applied every 4 or 5 years according to a certain pattern that iterates in a 2820-year cycle. A common year has 365 days and a leap year has 366 days.
    /// 
    /// A leap day is a day that occurs only in a leap year. In the Persian calendar, the 30th day of Esfand (month 12) is the only leap day.
    /// </remarks>
    public override bool IsLeapDay(int year, int month, int day, int era)
    {
        CheckEraRange(true, era);
        CheckYearRange(true, year);
        CheckMonthRange(true, month);
        return day == 30 && month == 12 && IsLeapYear(false, year, 0);
    }

    /// <summary>
    /// Determines whether the month specified by the year, month, and era parameters is a leap month.
    /// </summary>
    /// <param name="year">An integer that represents the year.</param>
    /// <param name="month">An integer that represents the month.</param>
    /// <param name="era">An integer that represents the era.</param>
    /// <returns>This method always returns false, unless overridden by a derived class.</returns>
    /// <remarks>
    /// In the Persian calendar leap years are applied every 4 or 5 years according to a certain pattern that iterates in a 2820-year cycle. A common year has 365 days and a leap year has 366 days.
    /// 
    /// A leap month is an entire month that occurs only in a leap year. The Persian calendar does not have any leap months.
    /// </remarks>
    public override bool IsLeapMonth(int year, int month, int era)
    {
        CheckEraRange(true, era);
        CheckYearRange(true, year);
        CheckMonthRange(true, month);

        return month == 12 && this.IsLeapYear(year);
    }

    /// <summary>
    /// Determines whether the month specified by the year, month, and era parameters is a leap month.
    /// </summary>
    /// <param name="year">An integer that represents the year.</param>
    /// <param name="month">An integer that represents the month.</param>
    /// <returns>This method always returns false, unless overridden by a derived class.</returns>
    /// <remarks>
    /// In the Persian calendar leap years are applied every 4 or 5 years according to a certain pattern that iterates in a 2820-year cycle. A common year has 365 days and a leap year has 366 days.
    /// 
    /// A leap month is an entire month that occurs only in a leap year. The Persian calendar does not have any leap months.
    /// </remarks>
    public override bool IsLeapMonth(int year, int month)
    {
        return this.IsLeapMonth(year, month, PersianEra);
    }

    /// <summary>
    /// Determines whether the year specified by the year and era parameters is a leap year.
    /// </summary>
    /// <param name="year">An integer that represents the year.</param>
    /// <param name="era">An integer that represents the era.</param>
    /// <returns>true if the specified year is a leap year; otherwise, false.</returns>
    /// <remarks>In the Persian calendar leap years are applied every 4 or 5 years according to a certain pattern that iterates in a 2820-year cycle. A common year has 365 days and a leap year has 366 days.</remarks>
    public override bool IsLeapYear(int year, int era)
    {
        return IsLeapYear(true, year, era);
    }

    private static bool IsLeapYear(bool validate, int year, int era)
    {
        CheckEraRange(validate, era);
        CheckYearRange(validate, year);

        return PersianDateConverter.IsJLeapYear(year);
    }

    /// <summary>
    /// Returns a DateTime that is set to the specified Date and time in the specified era.
    /// </summary>
    /// <param name="year">An integer that represents the year.</param>
    /// <param name="month">An integer that represents the month.</param>
    /// <param name="day">An integer that represents the day.</param>
    /// <param name="hour">An integer that represents the hour.</param>
    /// <param name="minute">An integer that represents the minute.</param>
    /// <param name="second">An integer that represents the second.</param>
    /// <param name="millisecond">An integer that represents the millisecond.</param>
    /// <param name="era">An integer that represents the era.</param>
    /// <returns>The DateTime instance set to the specified Date and time in the current era.</returns>
    public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
    {
        CheckEraRange(true, era);
        CheckDateRange(true, year, month, day);

        var days = day;
        for (var i = 1; i < month; i++)
        {
            if (i < 7)
            {
                days += 31;
            }
            else if (i < 12)
            {
                days += 30;
            }
        }

        // Total number of passed days from the start of the calendar
        days += 365 * year + NumberOfLeapYearsUntil(false, year);

        // following line validates the arguments of time
        var timePart = new DateTime(1, 1, 1, hour, minute, second, millisecond);
        var ticks = days * 864000000000L + timePart.Ticks + 195721056000000000L;

        DateTime dateTime;
        try
        {
            dateTime = new DateTime(ticks);
        }
        catch (Exception)
        {
            // If ticks go greater than DateTime.MaxValue.Ticks, this exception will be caught
            throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidMonthDay));
        }

        return dateTime;
    }

    /// <summary>
    /// Converts the specified two-digit year to a four-digit year by using the Globalization.PersianCalendar.TwoDigitYearMax property to determine the appropriate century.
    /// </summary>
    /// <param name="year">A two-digit integer that represents the year to convert.</param>
    /// <returns>An integer that contains the four-digit representation of year.</returns>
    /// <remarks>TwoDigitYearMax is the last year in the 100-year 
    /// range that can be represented by a two-digit year. The century is determined by finding the sole occurrence of the two-digit year within that 100-year range. For example, if TwoDigitYearMax is set to 1429, the 100-year range is from 1330 to 1429; therefore, a 2-digit value of 30 is interpreted as 1330, while a 2-digit value of 29 is interpreted as 1429.</remarks>
    public override int ToFourDigitYear(int year)
    {
        if (year != 0)
        {
            try
            {
                CheckYearRange(true, year);
            }
            catch (Exception)
            {
                // throw new System.ArgumentOutOfRangeException("Year", year, ResourceLibrary.GetString(CalendarKeys.InvalidYear, CalendarKeys.Root));
                throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidFourDigitYear));
            }
        }

        if (year > 99) 
            return year;
            
        var a = this.TwoDigitYearMax / 100;
            
        if (year > this.TwoDigitYearMax - a * 100)
            a--;

        return a * 100 + year;
    }

    /// <summary>
    /// Gets the list of eras in the PersianCalendar.
    /// </summary>
    /// <remarks>The Persian calendar recognizes one era: A.P. (Latin "Anno Persarum", which means "the year of/for Persians").</remarks>
    public override int[] Eras
    {
        get { return new[] { PersianEra }; }
    }

    /// <summary>
    /// Gets and sets the last year of a 100-year range that can be represented by a 2-digit year.
    /// </summary>
    /// <property_value>The last year of a 100-year range that can be represented by a 2-digit year.</property_value>
    /// <remarks>This property allows a 2-digit year to be properly translated to a 4-digit year. For example, if this property is set to 1429, the 100-year range is from 1330 to 1429; therefore, a 2-digit value of 30 is interpreted as 1330, while a 2-digit value of 29 is interpreted as 1429.</remarks>
    public override int TwoDigitYearMax
    {
        get => this.twoDigitYearMax;
        set
        {
            if (value is < 100 or < 9378)
                throw new InvalidOperationException("value should be between 100 and 9378");

            this.twoDigitYearMax = value;
        }
    }

    /// <summary>
    /// Gets the century of the specified DateTime.
    /// </summary>
    /// <param name="time">An instance of the DateTime class to read.</param>
    /// <returns>An integer from 1 to 94 that represents the century.</returns>
    /// <remarks>A century is a whole 100-year period. So the century 14 for example, represents years 1301 through 1400.</remarks>
    public int GetCentury(DateTime time)
    {
        return (GetYear(true, time) - 1) / 100 + 1;
    }

    /// <summary>
    /// Calculates the number of leap years until -but not including- the specified year.
    /// </summary>
    /// <param name="year">An integer between 1 and 9378</param>
    /// <returns>An integer representing the number of leap years that have occured by the year specified.</returns>
    /// <remarks>In the Persian calendar leap years are applied every 4 or 5 years according to a certain pattern that iterates in a 2820-year cycle. A common year has 365 days and a leap year has 366 days.</remarks>
    public int NumberOfLeapYearsUntil(int year)
    {
        return NumberOfLeapYearsUntil(true, year);
    }

    private static int NumberOfLeapYearsUntil(bool validate, int year)
    {
        CheckYearRange(validate, year);
        var count = 0;
        for (var i = 4; i < year; i++)
        {
            if (IsLeapYear(false, i, 0))
            {
                count++;
                i += 3;
            }
        }

        return count;
    }

    private static void CheckEraRange(bool validate, int era)
    {
        if (!validate)
        {
            return;
        }

        if (era is < 0 or > 1)
        {
            throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidEra));
        }
    }

    private static void CheckYearRange(bool validate, int year)
    {
        if (validate)
        {
            if (year is < 1 or > 9378)
            {
                throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidYear));
            }
        }
    }

    private static void CheckMonthRange(bool validate, int month)
    {
        if (validate)
        {
            if (month is < 1 or > 12)
            {
                throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidMonth));
            }
        }
    }

    private static void CheckDateRange(bool validate, int year, int month, int day)
    {
        if (validate)
        {
            var maxday = GetDaysInMonth(true, year, month, 0);
            if (day < 1 || maxday < day)
            {
                if (day == 30 && month == 12)
                {
                    throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidLeapYear));
                }

                throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidDay));
            }
        }
    }

    private static void CheckTicksRange(bool validate, DateTime time)
    {
        // Valid ticks represent times between 12:00:00.000 AM, 22/03/0622 CE and 11:59:59.999 PM, 31/12/9999 CE.
        if (validate)
        {
            var ticks = time.Ticks;
            if (ticks < 196037280000000000L)
            {
                throw new InvalidPersianDateException(FALocalizeManager.Instance.GetLocalizer().GetLocalizedString(StringID.PersianDate_InvalidDateTime));
            }
        }
    }

    private static void GetYearAndRemainingDays(bool validate, DateTime time, out int year, out int days)
    {
        CheckTicksRange(validate, time);
        days = (time - new DateTime(196036416000000000L)).Days;
        year = 1;
        var daysInNextYear = 365;

        while (days > daysInNextYear)
        {
            days -= daysInNextYear;
            year++;
            daysInNextYear = GetDaysInYear(false, year, 0);
        }
    }

    public static bool IsWithInSupportedRange(DateTime dateTime)
    {
        return dateTime < PersianDate.MaxValue &&
               dateTime > PersianDate.MinValue;
    }
}