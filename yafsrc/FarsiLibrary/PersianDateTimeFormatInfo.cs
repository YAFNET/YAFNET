/* Farsi Library - Working with Dates, Calendars, and DatePickers
 * http://www.codeproject.com/KB/selection/FarsiLibrary.aspx
 * 
 * Copyright (C) Hadi Eskandari
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 * OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace FarsiLibrary
{
    using System;

    public sealed class PersianDateTimeFormatInfo
    {
        public static string[] AbbreviatedDayNames
        {
            get { return PersianWeekDayNames.Default.DaysAbbr.ToArray(); }
        }

        public static string[] AbbreviatedMonthGenitiveNames
        {
            get { return PersianMonthNames.Default.Months.ToArray(); }
        }

        public static string[] AbbreviatedMonthNames
        {
            get { return PersianMonthNames.Default.Months.ToArray(); }
        }

        public static string AMDesignator
        {
            get { return "ق.ظ"; }
        }

        public static string DateSeparator
        {
            get { return "/"; }
        }

        public static string[] DayNames
        {
            get { return PersianWeekDayNames.Default.Days.ToArray(); }
        }

        public static DayOfWeek FirstDayOfWeek
        {
            get { return DayOfWeek.Saturday; }
        }

        public static string FullDateTimePattern
        {
            get { return "tt hh:mm:ss yyyy mmmm dd dddd"; }
        }

        public static string LongDatePattern
        {
            get { return "yyyy MMMM dd, dddd"; }
        }

        public static string LongTimePattern
        {
            get { return "hh:mm:ss tt"; }
        }

        public static string MonthDayPattern
        {
            get { return "dd MMMM"; }
        }

        public static string[] MonthGenitiveNames
        {
            get { return PersianMonthNames.Default.Months.ToArray(); }
        }

        public static string[] MonthNames
        {
            get { return PersianMonthNames.Default.Months.ToArray(); }
        }

        public static string PMDesignator
        {
            get { return "ب.ظ"; }
        }

        public static string ShortDatePattern
        {
            get { return "yyyy/MM/dd"; }
        }

        public static string[] ShortestDayNames
        {
            get { return PersianWeekDayNames.Default.DaysAbbr.ToArray(); }
        }

        public static string ShortTimePattern
        {
            get { return "hh:mm tt"; }
        }

        public static string TimeSeparator
        {
            get { return ":"; }
        }

        public static string YearMonthPattern
        {
            get { return "yyyy, MMMM"; }
        }

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
            switch (day)
            {
                case 0:
                    return DayOfWeek.Saturday;
                case 1:
                    return DayOfWeek.Sunday;
                case 2:
                    return DayOfWeek.Monday;
                case 3:
                    return DayOfWeek.Tuesday;
                case 4:
                    return DayOfWeek.Wednesday;
                case 5:
                    return DayOfWeek.Thursday;
                case 6:
                    return DayOfWeek.Friday;
                default:
                    throw new ArgumentOutOfRangeException("day", "invalid day value");
            }            
        }

        public static string GetWeekDayByIndex(int day)
        {
            return GetWeekDay(GetDayOfWeek(day));
        }

        public static string GetWeekDayAbbrByIndex(int day)
        {
            switch (day)
            {
                case 0:
                    return GetWeekDayAbbr(DayOfWeek.Saturday);
                case 1:
                    return GetWeekDayAbbr(DayOfWeek.Sunday);
                case 2:
                    return GetWeekDayAbbr(DayOfWeek.Monday);
                case 3:
                    return GetWeekDayAbbr(DayOfWeek.Tuesday);
                case 4:
                    return GetWeekDayAbbr(DayOfWeek.Wednesday);
                case 5:
                    return GetWeekDayAbbr(DayOfWeek.Thursday);
                case 6:
                    return GetWeekDayAbbr(DayOfWeek.Friday);
                default:
                    throw new ArgumentOutOfRangeException("day", "invalid day value");
            }
        }

        public static int GetDayIndex(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Sunday:
                    return 1;
                case DayOfWeek.Monday:
                    return 2;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 4;
                case DayOfWeek.Thursday:
                    return 5;
                case DayOfWeek.Friday:
                    return 6;
                case DayOfWeek.Saturday:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException("day");
            }
        }
    }
}
