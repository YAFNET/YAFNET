﻿/* Farsi Library - Working with Dates, Calendars, and DatePickers
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

    using FarsiLibrary.Internals;

    /// <summary>Class to convert PersianDate into normal DateTime value and vice versa.
    /// <seealso cref="PersianDate"/>
    /// </summary>
    /// <remarks>
    /// You can use <c>FarsiLibrary.Utils.FarsiDate.Now</c> property to access current Date.
    /// </remarks>
    public class PersianDateConverter
    {
        #region Fields

        private const double Solar = 365.25;
        private const int GYearOff = 226894;
        private static readonly int[,] GDayTable = new[,] { { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }, { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 } };
        private static readonly int[,] JDayTable = new[,] { { 31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29 }, { 31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 30 } };
        private static readonly string[] weekdays = new[] { "شنبه", "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };
        private static readonly string[] weekdaysabbr = new[] { "ش", "ی", "د", "س", "چ", "پ", "ج" };

        #endregion

        #region Methods

        /// <summary>
        /// Checks if a specified Persian year is a leap one.
        /// </summary>
        /// <param name="jyear"></param>
        /// <returns>returns 1 if the year is leap, otherwise returns 0.</returns>
        private static int JLeap(int jyear)
        {
            //Is jalali year a leap year?
            int tmp;

            Math.DivRem(jyear, 33, out tmp);
            if ((tmp == 1) || (tmp == 5) || (tmp == 9) || (tmp == 13) || (tmp == 17) || (tmp == 22) || (tmp == 26) || (tmp == 30))
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Checks if a year is a leap one.
        /// </summary>
        /// <param name="jyear">Year to check</param>
        /// <returns>true if the year is leap</returns>
        public static bool IsJLeapYear(int jyear)
        {
            return JLeap(jyear) == 1;
        }

        /// <summary>
        /// Checks if a specified Gregorian year is a leap one.
        /// </summary>
        /// <param name="gyear"></param>
        /// <returns>returns 1 if the year is leap, otherwise returns 0.</returns>
        private static int GLeap(int gyear)
        {
            //Is gregorian year a leap year?
            int Mod4, Mod100, Mod400;

            Math.DivRem(gyear, 4, out Mod4);
            Math.DivRem(gyear, 100, out Mod100);
            Math.DivRem(gyear, 400, out Mod400);

            if (((Mod4 == 0) && (Mod100 != 0)) || (Mod400 == 0))
            {
                return 1;
            }

            return 0;
        }

        private static int GregDays(int gYear, int gMonth, int gDay)
        {
            //Calculate total days of gregorian from calendar base
            var Div4 = (gYear - 1) / 4;
            var Div100 = (gYear - 1) / 100;
            var Div400 = (gYear - 1) / 400;
            var leap = GLeap(gYear);

            for (var i = 0; i < gMonth - 1; i++)
            {
                gDay = gDay + GDayTable[leap, i];
            }

            return ((gYear - 1) * 365 + gDay + Div4 - Div100 + Div400);
        }

        private static int JLeapYears(int jYear)
        {
            int i;
            var Div33 = jYear / 33;
            var cycle = jYear - (Div33 * 33);
            var leap = (Div33 * 8);

            if (cycle > 0)
            {
                for (i = 1; i <= 18; i = i + 4)
                {
                    if (i > cycle)
                        break;

                    leap++;
                }
            }

            if (cycle > 21)
            {
                for (i = 22; i <= 31; i = i + 4)
                {
                    if (i > cycle)
                        break;

                    leap++;
                }

            }
            return leap;
        }

        internal static int JalaliDays(int jYear, int jMonth, int jDay)
        {
            //Calculate total days of jalali years from the base calendar
            var leap = JLeap(jYear);
            for (var i = 0; i < jMonth - 1; i++)
            {
                jDay = jDay + JDayTable[leap, i];
            }

            leap = JLeapYears(jYear - 1);
            var iTotalDays = ((jYear - 1) * 365 + leap + jDay);

            return iTotalDays;
        }

        /// <summary>Converts a Gregorian Date of type <c>System.DateTime</c> class to Persian Date.</summary>
        /// <param name="date">DateTime to evaluate</param>
        /// <returns>string representation of Jalali Date</returns>
        public static PersianDate ToPersianDate(string date)
        {
            return ToPersianDate(DateTime.Parse(date, CultureHelper.NeutralCulture));
        }

        /// <summary>
        /// Converts a Gregorian Date of type <c>String</c> and a <c>TimeSpan</c> into a Persian Date.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static PersianDate ToPersianDate(string date, TimeSpan time)
        {
            var pd = ToPersianDate(date);
            pd.Hour = time.Hours;
            pd.Minute = time.Minutes;
            pd.Second = time.Seconds;

            return pd;
        }

        /// <summary>
        /// Converts a Gregorian Date of type <c>String</c> class to Persian Date.
        /// </summary>
        /// <param name="dt">Date to evaluate</param>
        /// <returns>string representation of Jalali Date.</returns>
        public static PersianDate ToPersianDate(DateTime dt)
        {
            var gyear = dt.Year;
            var gmonth = dt.Month;
            var gday = dt.Day;
            int i;

            //Calculate total days from the base of gregorian calendar
            var iTotalDays = GregDays(gyear, gmonth, gday);
            iTotalDays = iTotalDays - GYearOff;

            //Calculate total jalali years passed
            var jyear = (int)(iTotalDays / (Solar - 0.25 / 33));
            //Calculate passed leap years
            var leap = JLeapYears(jyear);

            //Calculate total days from the base of jalali calendar
            var jday = iTotalDays - (365 * jyear + leap);

            //Calculate the correct year of jalali calendar
            jyear++;

            if (jday == 0)
            {
                jyear--;
                jday = JLeap(jyear) == 1 ? 366 : 365;
            }
            else
            {
                if ((jday == 366) && (JLeap(jyear) != 1))
                {
                    jday = 1;
                    jyear++;
                }
            }

            //Calculate correct month of jalali calendar
            leap = JLeap(jyear);
            for (i = 0; i <= 12; i++)
            {
                if (jday <= JDayTable[leap, i])
                {
                    break;
                }
                jday = jday - JDayTable[leap, i];
            }

            var iJMonth = i + 1;

            return new PersianDate(jyear, iJMonth, jday, dt.Hour, dt.Minute, dt.Second);
        }

        /// <summary>
        /// Converts a Persian Date of type <c>String</c> to Gregorian Date of type <c>DateTime</c> class.
        /// </summary>
        /// <param name="date">Date to evaluate</param>
        /// <returns>Gregorian DateTime representation of evaluated Jalali Date.</returns>
        public static DateTime ToGregorianDateTime(string date)
        {
            var pd = new PersianDate(date);
            return DateTime.Parse(ToGregorianDate(pd), CultureHelper.NeutralCulture);
        }

        public static DateTime ToGregorianDateTime(PersianDate date)
        {
            return DateTime.Parse(ToGregorianDate(date), CultureHelper.NeutralCulture);
        }

        /// <summary>
        /// Converts a Persian Date of type <c>String</c> to Gregorian Date of type <c>String</c>.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Gregorian DateTime representation in string format of evaluated Jalali Date.</returns>
        public static string ToGregorianDate(PersianDate date)
        {
            var jyear = date.Year;
            var jmonth = date.Month;
            var jday = date.Day;

            //Continue
            int i;

            var totalDays = JalaliDays(jyear, jmonth, jday);
            totalDays = totalDays + GYearOff;

            var gyear = (int)(totalDays / (Solar - 0.25 / 33));
            var Div4 = gyear / 4;
            var Div100 = gyear / 100;
            var Div400 = gyear / 400;
            var gdays = totalDays - (365 * gyear) - (Div4 - Div100 + Div400);
            gyear = gyear + 1;

            if (gdays == 0)
            {
                gyear--;
                gdays = GLeap(gyear) == 1 ? 366 : 365;
            }
            else
            {
                if (gdays == 366 && GLeap(gyear) != 1)
                {
                    gdays = 1;
                    gyear++;
                }
            }

            var leap = GLeap(gyear);
            for (i = 0; i <= 12; i++)
            {
                if (gdays <= GDayTable[leap, i])
                {
                    break;
                }

                gdays = gdays - GDayTable[leap, i];
            }

            var iGMonth = i + 1;
            var iGDay = gdays;

            return (Util.toDouble(iGMonth) + "/" + Util.toDouble(iGDay) + "/" + gyear + " " + Util.toDouble(date.Hour) + ":" + Util.toDouble(date.Minute) + ":" + Util.toDouble(date.Second));
        }

        internal static string DayOfWeek(PersianDate date)
        {
            if (!date.IsNull)
            {
                var dt = ToGregorianDateTime(date);
                return DayOfWeek(dt);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets Persian Weekday name from specified Gregorian Date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal static string DayOfWeek(DateTime date)
        {
            var DayOfWeek = date.DayOfWeek;
            string day = string.Empty;

            switch (DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    day = PersianWeekDayNames.Default.Shanbeh;
                    break;
                case DayOfWeek.Sunday:
                    day = PersianWeekDayNames.Default.Yekshanbeh;
                    break;
                case DayOfWeek.Monday:
                    day = PersianWeekDayNames.Default.Doshanbeh;
                    break;
                case DayOfWeek.Tuesday:
                    day = PersianWeekDayNames.Default.Seshanbeh;
                    break;
                case DayOfWeek.Wednesday:
                    day = PersianWeekDayNames.Default.Chaharshanbeh;
                    break;
                case DayOfWeek.Thursday:
                    day = PersianWeekDayNames.Default.Panjshanbeh;
                    break;
                case DayOfWeek.Friday:
                    day = PersianWeekDayNames.Default.Jomeh;
                    break;
            }

            return day;
        }

        /// <summary>
        /// Returns number of days in specified month number.
        /// </summary>
        /// <param name="MonthNo">Month no to evaluate in integer</param>
        /// <returns>number of days in the evaluated month</returns>
        internal static int MonthDays(int MonthNo)
        {
            return (JDayTable[1, MonthNo - 1]);
        }

        #endregion
    }
}