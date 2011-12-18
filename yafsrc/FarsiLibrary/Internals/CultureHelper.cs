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

namespace FarsiLibrary.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;

    using PersianCalendar = FarsiLibrary.PersianCalendar;

    /// <summary>
    /// Base culture information
    /// </summary>
    internal static class CultureHelper
    {
        private static CultureInfo faCulture;
        private static CultureInfo arCulture;
        private static CultureInfo internalfaCulture;
        private static readonly CultureInfo neuCulture = CultureInfo.InvariantCulture;
        private static readonly PersianCalendar pc = new PersianCalendar();
        private static readonly HijriCalendar hc = new HijriCalendar();
        private static readonly GregorianCalendar gc = new GregorianCalendar();
        private static readonly Dictionary<int, DayOfWeek> PersianDoW = new Dictionary<int, DayOfWeek>();
        private static readonly Dictionary<int, DayOfWeek> GregorianDoW = new Dictionary<int, DayOfWeek>();

        static CultureHelper()
        {
            CreatePersianDayOfWeekMap();
            CreateGregorianDayOfWeekMap();
        }

        private static void CreatePersianDayOfWeekMap()
        {
            PersianDoW.Add(0, DayOfWeek.Saturday);
            PersianDoW.Add(1, DayOfWeek.Sunday);
            PersianDoW.Add(2, DayOfWeek.Monday);
            PersianDoW.Add(3, DayOfWeek.Tuesday);
            PersianDoW.Add(4, DayOfWeek.Wednesday);
            PersianDoW.Add(5, DayOfWeek.Thursday);
            PersianDoW.Add(6, DayOfWeek.Friday);
        }

        private static void CreateGregorianDayOfWeekMap()
        {
            GregorianDoW.Add(0, DayOfWeek.Sunday);
            GregorianDoW.Add(1, DayOfWeek.Monday);
            GregorianDoW.Add(2, DayOfWeek.Tuesday);
            GregorianDoW.Add(3, DayOfWeek.Wednesday);
            GregorianDoW.Add(4, DayOfWeek.Thursday);
            GregorianDoW.Add(5, DayOfWeek.Friday);
            GregorianDoW.Add(6, DayOfWeek.Saturday);
        }

        public static Calendar PersianCalendar
        {
            get { return pc; }
        }

        /// <summary>
        /// Currently selected UICulture
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

        /// <summary>
        /// Instance of Arabic culture
        /// </summary>
        public static CultureInfo ArabicCulture
        {
            get
            {
                if(arCulture == null)
                    arCulture = new CultureInfo("ar-SA");

                return arCulture;
            }
        }

        /// <summary>
        /// Instance of Farsi culture
        /// </summary>
        public static CultureInfo FarsiCulture
        {
            get
            {
                if(faCulture == null)
                    faCulture = new CultureInfo("fa-IR");

                return faCulture;
            }
        }

        /// <summary>
        /// Instance of Persian Culture with correct date formatting.
        /// </summary>
        public static CultureInfo PersianCulture
        {
            get
            {
                return internalfaCulture ?? (internalfaCulture = new PersianCultureInfo());
            }
        }

        /// <summary>
        /// Instance of Neutral culture
        /// </summary>
        public static CultureInfo NeutralCulture
        {
            get { return neuCulture; }
        }

        /// <summary>
        /// Returns the day of week based on calendar.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="calendar"></param>
        /// <returns></returns>
        public static int GetDayOfWeek(DateTime dt, Calendar calendar)
        {
            var calendarType = calendar.GetType();
            if (calendarType == typeof (PersianCalendar) ||
                calendarType == typeof (System.Globalization.PersianCalendar))
            {
                return PersianDateTimeFormatInfo.GetDayIndex(dt.DayOfWeek);
            }

            return (int) dt.DayOfWeek;
        }

        /// <summary>
        /// Returns the default calendar for the current culture.
        /// </summary>
        /// <returns></returns>
        public static Calendar CurrentCalendar
        {
            get
            {
                if (IsFarsiCulture)
                {
                    return pc;
                }
                
                if (IsArabicCulture)
                {
                    return hc;
                }
                
                return gc;
            }
        }

        /// <summary>
        /// Finds the corresponding DayOfWeek in specified culture
        /// </summary>
        /// <param name="day"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static DayOfWeek GetCultureDayOfWeek(int day, CultureInfo culture)
        {
            if(culture.IsFarsiCulture())
            {
                return PersianDoW[day];
            }
            
            return GregorianDoW[day];
        }

        public static DateTime MinCultureDateTime
        {
            get { return CurrentCalendar.MinSupportedDateTime; }
        }

        public static DateTime MaxCultureDateTime
        {
            get { return CurrentCalendar.MaxSupportedDateTime; }
        }

        public static bool IsArabicCulture
        {
            get { return CurrentCulture.Equals(arCulture) || CurrentCulture.Name.Equals("ar", StringComparison.InvariantCultureIgnoreCase); }
        }

        public static bool IsDefaultCulture
        {
            get { return CurrentCulture.Equals(NeutralCulture); }
        }

        public static bool IsFarsiCulture
        {
            get { return IsCustomizedFarsiCulture || IsBuiltinFarsiCulture; }
        }

    	public static bool IsCustomizedFarsiCulture
    	{
			get { return CurrentCulture.Equals(PersianCulture); }
    	}

    	public static bool IsBuiltinFarsiCulture
    	{
			get { return CurrentCulture.Name.Equals("fa", StringComparison.InvariantCultureIgnoreCase); }
    	}
    }
}