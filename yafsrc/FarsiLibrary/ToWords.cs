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

    /// <summary>
    /// Classes to convert a number to its persian written form. It accepts both an Integer or Long as input parameter.
    /// </summary>
    /// <exception>Thrown when input number is larger than 999999999999</exception>
    /// <example>
    /// An example on how to convert a Integer number to words.
    /// <code>
    ///		class MyClass 
    ///     {
    ///		   public static void Main() 
    ///        {
    ///		      Console.WriteLine(FarsiLibrary.Utils.ToWords.ToString(1452));
    ///		   }
    ///		}
    /// </code>
    /// </example>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static class ToWords
    {
        private static readonly string[] cvtText = new string[1000];

        static ToWords()
        {
            cvtText[0] = "صفر";
            cvtText[1] = "يک";
            cvtText[2] = "دو";
            cvtText[3] = "سه";
            cvtText[4] = "چهار";
            cvtText[5] = "پنج";
            cvtText[6] = "شش";
            cvtText[7] = "هفت";
            cvtText[8] = "هشت";
            cvtText[9] = "نه";
            cvtText[10] = "ده";
            cvtText[11] = "یازده";
            cvtText[12] = "دوازده";
            cvtText[13] = "سیزده";
            cvtText[14] = "چهارده";
            cvtText[15] = "پانزده";
            cvtText[16] = "شانزده";
            cvtText[17] = "هفده";
            cvtText[18] = "هجده";
            cvtText[19] = "نوزده";
            cvtText[20] = "بيست";
            cvtText[21] = "سی";
            cvtText[22] = "چهل";
            cvtText[23] = "پنجاه";
            cvtText[24] = "شصت";
            cvtText[25] = "هفتاد";
            cvtText[26] = "هشتاد";
            cvtText[27] = "نود";
            cvtText[28] = "صد";
            cvtText[29] = "هزار";
            cvtText[30] = "میلیون";
            cvtText[31] = "میلیارد";
            cvtText[100] = "صد";
            cvtText[200] = "دویست";
            cvtText[300] = "سیصد";
            cvtText[400] = "چهارصد";
            cvtText[500] = "پانصد";
            cvtText[600] = "ششصد";
            cvtText[700] = "هفتصد";
            cvtText[800] = "هشتصد";
            cvtText[900] = "نهصد";
        }

        private static string cvt100(long Number)
        {
            var x = (int)Number;
            var result = string.Empty;
            int t;

            System.Diagnostics.Debug.Assert(x < 999);

            if (x > 99)
            {
                t = x / 100;
                switch (t)
                {
                    case 1:
                        result = cvtText[100];
                        break;
                    case 2:
                        result = cvtText[200];
                        break;
                    case 3:
                        result = cvtText[300];
                        break;
                    case 4:
                        result = cvtText[400];
                        break;
                    case 5:
                        result = cvtText[500];
                        break;
                    case 6:
                        result = cvtText[600];
                        break;
                    case 7:
                        result = cvtText[700];
                        break;
                    case 8:
                        result = cvtText[800];
                        break;
                    case 9:
                        result = cvtText[900];
                        break;
                }

                x = x - (t * 100);

                if (x <= 0)
                {
                    return result;
                }

                result += String.Format(" {0} ", "و"); ;
            }

            if (x > 20)
            {
                t = x / 10;
                result = result + cvtText[t + 18];
                x = x - (t * 10);

                if (x <= 0)
                {
                    return result;
                }

                result += String.Format("{0}", " و "); ;
            }

            if (x > 0)
            {
                result += cvtText[x];
            }

            if (x == 0)
            {
                result += cvtText[0];
            }

            return result;
        }


        /// <overloads>Has two overloads.</overloads>
        /// <summary>Converts an integer number to its written form in Persian</summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ToString(int x)
        {
            return ToString(long.Parse(x.ToString()));
        }

        public static string ToString(double  x)
        {
            return ToString((long)x);
        }

        /// <summary>Converts a long number to its written form in Persian</summary>
        /// <param name="x"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToString(long x)
        {
            long t;
            string result = string.Empty;

            if (x > 999999999999)
                throw new ArgumentOutOfRangeException("x", "Number is too large to process");

            if (x < 0)
                throw new ArgumentOutOfRangeException("x", "Number should be a positive value");

            if (x > 999999999)
            {
                t = x / 1000000000;
                result += cvt100(t) + " " + cvtText[31];
                x = x - (t * 1000000000);

                if (x <= 0)
                {
                    return result;
                }

                result += String.Format("{0}", " و ");
            }

            if (x > 999999)
            {
                t = x / 1000000;
                result += cvt100(t) + " " + cvtText[30];
                x = x - (t * 1000000);

                if (x <= 0)
                {
                    return result;
                }

                result += String.Format("{0}", " و ");
            }

            if (x > 999)
            {
                t = x / 1000;
                result += cvt100(t) + " " + cvtText[29];
                x = x - (t * 1000);

                if (x <= 0)
                {
                    return result;
                }

                result += String.Format("{0}", " و "); ;
            }

            if (x >= 0)
            {
                result += cvt100(x);
            }

            return result;
        }
    }
}
