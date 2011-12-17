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

    using FarsiLibrary.Internals;

    public class BasicTimeFormat : ITimeFormat
    {
        public static string NEGATIVE = "-";
        public static string SIGN = "%s";
        public static string QUANTITY = "%n";
        public static string UNIT = "%u";

        public double RoundingTolerance { get;set; }
        public string Pattern { get;set; }
        public string FuturePrefix { get; set; }
        public string FutureSuffix { get;set; }
        public string PastPrefix { get; set; }
        public string PastSuffix { get; set; }

        public BasicTimeFormat()
        {
            RoundingTolerance = 0;
            Pattern = string.Empty;
            FuturePrefix = string.Empty;
            FutureSuffix = string.Empty;
            PastPrefix = string.Empty;
            PastSuffix = string.Empty;
        }

        public string Format(Duration duration)
        {
            var sign = GetSign(duration);
            var quantity = GetQuantity(duration);
            var unit = GetGramaticallyCorrectName(duration, quantity);
            var result = ApplyPattern(sign, unit, quantity);

            result = Decorate(sign, result);

            return result;
        }

        private string Decorate(string sign, string result)
        {
            if (sign == NEGATIVE)
            {
                result = PastPrefix + " " + result + " " + PastSuffix;
            }
            else
            {
                result = FuturePrefix + " " + result + " " + FutureSuffix;
            }
            return result.Trim();
        }

        private string ApplyPattern(string sign, string unit, double quantity)
        {
            var result = Pattern.Replace(SIGN, sign);
            var number = FormatNumber(quantity);

            result = result.Replace(QUANTITY, number);
            result = result.Replace(UNIT, unit);

            return result;
        }

        private static string FormatNumber(double quantity)
        {
            return CultureHelper.IsFarsiCulture ? ToWords.ToString(quantity) : quantity.ToString();
        }

        private double GetQuantity(Duration duration)
        {
            var quantity = Math.Abs(duration.Quantity);

            if (duration.Delta != 0)
            {
                double threshold = Math.Abs((duration.Delta / duration.Unit.MillisPerUnit) * 100);
                if (threshold < RoundingTolerance)
                {
                    quantity = quantity + 1;
                }
            }
            return Math.Truncate(quantity);
        }

        private static string GetGramaticallyCorrectName(Duration d, double quantity)
        {
            var result = d.Unit.Name;
            var value = Math.Abs(quantity);
            if (value == 0 || value > 1)
            {
                result = d.Unit.PluralName;
            }
            return result;
        }

        private static string GetSign(Duration d)
        {
            return d.Quantity < 0 ? NEGATIVE : string.Empty;
        }

        public BasicTimeFormat SetPattern(string pattern)
        {
            Pattern = pattern;
            return this;
        }

        public BasicTimeFormat SetFuturePrefix(string futurePrefix)
        {
            FuturePrefix = futurePrefix.Trim();
            return this;
        }

        public BasicTimeFormat SetFutureSuffix(string futureSuffix)
        {
            FutureSuffix = futureSuffix.Trim();
            return this;
        }

        public BasicTimeFormat SetPastPrefix(string pastPrefix)
        {
            PastPrefix = pastPrefix.Trim();
            return this;
        }

        public BasicTimeFormat SetPastSuffix(string pastSuffix)
        {
            PastSuffix = pastSuffix.Trim();
            return this;
        }
    }
}