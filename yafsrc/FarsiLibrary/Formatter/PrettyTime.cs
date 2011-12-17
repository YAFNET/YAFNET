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

namespace FarsiLibrary.Formatter
{
    using System;
    using System.Collections.Generic;

    using FarsiLibrary.Formatter.TimeUnits;

    public class PrettyTime
    {
        private DateTime baseDate;
        private IList<ITimeUnit> timeUnits;

        public PrettyTime() : this(DateTime.Now)
        {
        }

        public PrettyTime(DateTime baseDate)
        {
            this.baseDate = baseDate;
            InitTimeUnits();
        }

        private void InitTimeUnits()
        {
            timeUnits = new List<ITimeUnit>
            {
                new JustNow(),
                new Millisecond(),
                new Second(),
                new Minute(),
                new Hour(),
                new Day(),
                new Week(),
                new Month(),
                new Year(),
                new Decade(),
                new Century(),
                new Millennium()
           };
        }

        public Duration ApproximateDuration(DateTime then)
        {
            var difference = (then - baseDate).TotalMilliseconds;
            return CalculateDuration(difference);
        }

        private Duration CalculateDuration(double difference)
        {
            var absoluteDifference = Math.Abs(difference);
            var result = new Duration();

            for (int i = 0; i < timeUnits.Count; i++)
            {
                var unit = timeUnits[i];
                var millisPerUnit = Math.Abs(unit.MillisPerUnit);
                var quantity = Math.Abs(unit.MaxQuantity);
                var isLastUnit = i == timeUnits.Count - 1;

                if (quantity == 0 && !isLastUnit)
                {
                    quantity = timeUnits[i + 1].MillisPerUnit / unit.MillisPerUnit;
                }

                // does our unit encompass the time duration?
                if ((millisPerUnit * quantity > absoluteDifference) || isLastUnit)
                {
                    result.Unit = unit;
                    result.Quantity = difference / millisPerUnit;
                    result.Delta = difference - result.Quantity * millisPerUnit;
                    break;
                }
            }

            return result;
        }

        public string Format(DateTime then)
        {
            var duration = ApproximateDuration(then);
            var formatter = duration.Unit.Format;

            return formatter.Format(duration);
        }
    }
}