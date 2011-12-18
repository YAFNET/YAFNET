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
    using FarsiLibrary.Resources;

    public abstract class AbstractTimeUnit : ITimeUnit
    {
        public AbstractTimeUnit()
        {
            MaxQuantity = 0;
            MillisPerUnit = 1;
            LoadStringKeys();
        }

        protected FALocalizeManager LocalizeManager
        {
            get { return FALocalizeManager.Instance; }
        }

        public ITimeFormat Format { get; set; }
        public string Name { get; set; }
        public string PluralName { get; set; }
        public double MaxQuantity { get; set; }
        public double MillisPerUnit { get; set; }

        private void LoadStringKeys()
        {
            var resPrefix = GetResourcePrefix();
            var pattern = LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "Pattern");
            var futurePrefix = LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "FuturePrefix");
            var futureSuffix = LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "FutureSuffix");
            var pastPrefix = LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "PastPrefix");
            var pastSuffix = LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "PastSuffix");

            Name = LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "Name");
            PluralName = LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "PluralName");

            Format = new BasicTimeFormat().SetPattern(pattern)
                                          .SetFuturePrefix(futurePrefix)
                                          .SetFutureSuffix(futureSuffix)
                                          .SetPastPrefix(pastPrefix)
                                          .SetPastSuffix(pastSuffix);
        }

        protected abstract string GetResourcePrefix();
    }
}