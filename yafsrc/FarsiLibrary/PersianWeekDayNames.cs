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
    using System.Collections.Generic;

    internal class PersianWeekDayNames
    {
        #region fields

        public string Shanbeh = "شنبه";
        public string Yekshanbeh = "یکشنبه";
        public string Doshanbeh = "دوشنبه";
        public string Seshanbeh = "ﺳﻪشنبه";
        public string Chaharshanbeh = "چهارشنبه";
        public string Panjshanbeh = "پنجشنبه";
        public string Jomeh = "جمعه";

        public string Sh = "ش";
        public string Ye = "ی";
        public string Do = "د";
        public string Se = "س";
        public string Ch = "چ";
        public string Pa = "پ";
        public string Jo = "ج";

        private readonly List<string> days;
        private readonly List<string> daysAbbr;
        private static PersianWeekDayNames instance;

        #endregion

        #region Ctor

        private PersianWeekDayNames()
        {
            days = new List<string>
                       {
                           Yekshanbeh,
                           Doshanbeh,
                           Seshanbeh,
                           Chaharshanbeh,
                           Panjshanbeh,
                           Jomeh,
                           Shanbeh,
                       };

            daysAbbr = new List<string>
                           {
                               Ye,
                               Do,
                               Se,
                               Ch,
                               Pa,
                               Jo,
                               Sh,
                           };
        }

        #endregion

        #region Indexer

        public static PersianWeekDayNames Default
        {
            get
            {
                if(instance == null)
                    instance = new PersianWeekDayNames();

                return instance;
            }
        }

        #endregion

        #region Props

        internal List<string> Days
        {
            get { return days; }
        }

        internal List<string> DaysAbbr
        {
            get { return daysAbbr; }
        }

        #endregion
    }
}
