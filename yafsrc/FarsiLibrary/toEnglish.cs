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
	/// <summary>
	/// Helper class to convert numbers to it's farsi equivalent. Use this class' methods to overcome a problem in displaying farsi numeric values.
	/// </summary>
    public sealed class toEnglish
    {
        /// <summary>
        /// Converts a Farsi number to it's English numeric values.
        /// </summary>
        /// <remarks>This method only converts the numbers in a string, and does not convert any non-numeric characters.</remarks>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string Convert(string num)
        {
            if (string.IsNullOrEmpty(num))
                return num;

            var result = string.Empty;

            for (var i = 0; i < num.Length; i++)
            {
                var numTemp = num.Substring(i, 1);
                switch (numTemp)
                {
                    case "۰":
                        result = result + "0";
                        break;
                    case "۱":
                        result = result + "1";
                        break;
                    case "۲":
                        result = result + "2";
                        break;
                    case "۳":
                        result = result + "3";
                        break;
                    case "۴":
                        result = result + "4";
                        break;
                    case "۵":
                        result = result + "5";
                        break;
                    case "۶":
                        result = result + "6";
                        break;
                    case "۷":
                        result = result + "7";
                        break;
                    case "۸":
                        result = result + "8";
                        break;
                    case "۹":
                        result = result + "9";
                        break;
                    default:
                        result = result + numTemp;
                        break;
                }
            }
            return (result);
        }
    }
}
