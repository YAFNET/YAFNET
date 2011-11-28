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
    using FarsiLibrary.Resources;

    using System.Globalization;

    /// <summary>
	/// Helper class to convert numbers to it's farsi equivalent. Use this class' methods to overcome a problem in displaying farsi numeric values.
	/// </summary>
	public sealed class toFarsi
	{

        /// <summary>
        /// Converts a number in string format e.g. 14500 to its localized version, if <c>Localized</c> value is set to <c>true</c>.
        /// </summary>
        /// <param name="EnglishNumber"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string Convert(string EnglishNumber, CultureInfo culture)
        {
			string numEnglish = "";
			string numTemp  = "";

            for(int i=0;i<EnglishNumber.Length;i++) 
			{
				numTemp = EnglishNumber.Substring(i,1);
				switch(numTemp) 
				{
					case "0" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_0);
						break;
					case "1" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_1);
						break;
					case "2" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_2);
						break;
					case "3" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_3);
						break;
					case "4" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_4);
						break;
					case "5" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_5);
						break;
					case "6" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_6);
						break;
					case "7" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_7);
						break;
					case "8" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_8);
						break;
					case "9" :
                        numEnglish = numEnglish + FALocalizeManager.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_9);
						break;
					default :
						numEnglish = numEnglish + numTemp;
						break;
				}
			}

			return (numEnglish);
        }

		/// <summary>
		/// Converts an English number to it's Farsi value.
		/// </summary>
		/// <remarks>This method only converts the numbers in a string, and does not convert any non-numeric characters.</remarks>
		/// <param name="EnglishNumber"></param>
		/// <returns></returns>
		public static string Convert(string EnglishNumber)
		{
			string numEnglish = "";
			string numTemp  = "";

			for(int i=0;i<EnglishNumber.Length;i++) 
			{
				numTemp = EnglishNumber.Substring(i,1);
				switch(numTemp) 
				{
					case "0" : 
						numEnglish = numEnglish + "۰";
						break;
					case "1" : 
						numEnglish = numEnglish + "۱";
						break;
					case "2" :
						numEnglish = numEnglish + "۲";
						break;
					case "3" :
						numEnglish = numEnglish + "۳"; 
						break;
					case "4" :
						numEnglish = numEnglish + "۴";
						break;
					case "5" : 
						numEnglish = numEnglish + "۵";
						break;
					case "6" : 
						numEnglish = numEnglish + "۶";
						break;
					case "7" : 
						numEnglish = numEnglish + "۷";
						break;
					case "8" : 
						numEnglish = numEnglish + "۸";
						break;
					case "9" : 
						numEnglish = numEnglish + "۹";
						break;
					default :
						numEnglish = numEnglish + numTemp;
						break;
				}
			}

			return (numEnglish);
		}
	}
}
