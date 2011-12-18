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
    public interface ITimeUnit
    {
        ITimeFormat Format { get; }

        /// <summary>
        /// The number of milliseconds represented by each 
        /// instance of this TimeUnit.
        /// </summary>
        double MillisPerUnit { get; }

        /// <summary>
        /// The maximum quantity of this Unit to be used as a threshold for the next
        /// largest Unit (e.g. if one <code>Second</code> represents 1000ms, and
        /// <code>Second</code> has a maxQuantity of 5, then if the difference
        /// between compared timestamps is larger than 5000ms, PrettyTime will move
        /// on to the next smallest TimeUnit for calculation; <code>Minute</code>, by
        /// default)
        /// </summary>
        double MaxQuantity { get; }

        /// <summary>
        /// The grammatically singular name for this unit of time. (e.g. one "second")
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The grammatically plural name for this unit of time. (e.g. many "seconds")
        /// </summary>
        /// <returns></returns>
        string PluralName { get; }
    }

}