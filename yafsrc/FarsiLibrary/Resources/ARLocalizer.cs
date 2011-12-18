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

namespace FarsiLibrary.Resources
{
    /// <summary>
    /// Localizer class used to get string values of Arabic language.
    /// </summary>
    public class ARLocalizer : FALocalizer
    {
        /// <summary>
        /// Gets a localized string for Arabic culture, for specified <see cref="StringID"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override string GetLocalizedString(StringID id)
        {
            switch (id)
            {
                case StringID.FAMonthView_None: return "امح";
                case StringID.FAMonthView_Today: return "اليوم";
            }

            return base.GetLocalizedString(id);
        }
    }
}
