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

    internal class PersianMonthNames
    {
        #region Fields

        public string Farvardin = "فروردین";
        public string Ordibehesht = "ارديبهشت";
        public string Khordad = "خرداد";
        public string Tir = "تير";
        public string Mordad = "مرداد";
        public string Shahrivar = "شهریور";
        public string Mehr = "مهر";
        public string Aban = "آبان";
        public string Azar = "آذر";
        public string Day = "دی";
        public string Bahman = "بهمن";
        public string Esfand = "اسفند";
        private readonly List<string> months;
        private static PersianMonthNames instance;

        #endregion

        #region Ctor

        private PersianMonthNames()
        {
            months = new List<string>
                         {
                             Farvardin,
                             Ordibehesht,
                             Khordad,
                             Tir,
                             Mordad,
                             Shahrivar,
                             Mehr,
                             Aban,
                             Azar,
                             Day,
                             Bahman,
                             Esfand,
                             ""
                         };
        }

        #endregion

        #region Singleton

        public static PersianMonthNames Default
        {
            get
            {
                if(instance == null)
                    instance = new PersianMonthNames();

                return instance;
            }
        }

        #endregion

        #region Indexer

        internal List<string> Months
        {
            get { return months; }
        }

        public string this[int month]
        {
            get { return months[month]; }
        }

        #endregion
    }
}
