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

using System;
using System.Globalization;

namespace FarsiLibrary.Resources
{
    /// <summary>
    /// Localizer class to work with internal localized strings.
    /// </summary>
    public class FALocalizeManager
    {
        #region Ctor

        private FALocalizeManager()
        {
        }

        #endregion

        #region Fields

        private static CultureInfo farsiCulture = new CultureInfo("fa-IR");
        private static CultureInfo arabicCulture = new CultureInfo("ar-SA");
        private static CultureInfo englishCulture = CultureInfo.InvariantCulture;
        private static CultureInfo customCulture = null;
        
        private static FALocalizer fa = new FALocalizer();
        private static ARLocalizer ar = new ARLocalizer();
        private static ENLocalizer en = new ENLocalizer();
        private static BaseLocalizer customLocalizer = null;

        public static event EventHandler LocalizerChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a localizer instance based on the culture.
        /// </summary>
        public static BaseLocalizer GetLocalizerByCulture(CultureInfo ci)
        {
            if (customLocalizer != null)
                return customLocalizer;
            
            if (ci.Equals(farsiCulture))
            {
                return fa;
            }
            else if (ci.Equals(arabicCulture))
            {
                return ar;
            }
            else
            {
                return en;
            }
        }

        #endregion

        #region Props

        /// <summary>
        /// Custom culture, when set , is used across all controls.
        /// </summary>
        public static CultureInfo CustomCulture
        {
            get { return customCulture; }
            set { customCulture = value; }
        }
        
        /// <summary>
        /// Farsi Culture
        /// </summary>
        public static CultureInfo FarsiCulture
        {
            get { return farsiCulture; }
        }

        /// <summary>
        /// Arabic Culture
        /// </summary>
        public static CultureInfo ArabicCulture
        {
            get { return arabicCulture; }
        }

        /// <summary>
        /// Invariant Culture
        /// </summary>
        public static CultureInfo InvariantCulture
        {
            get { return englishCulture; }
        }

        /// <summary>
        /// Gets or Sets a new instance of Localizer. If this value is initialized (default is null), Localize Manager class will use the custom class provided, to interpret localized strings.
        /// </summary>
        public static BaseLocalizer CustomLocalizer
        {
            get { return customLocalizer; }
            set
            {
                if(customLocalizer == value)
                    return;

                customLocalizer = value;
                OnLocalizerChanged(EventArgs.Empty);
            }
        }
        
        #endregion

        #region Methods
        
        protected static void OnLocalizerChanged(EventArgs e)
        {
            if (LocalizerChanged != null)
                LocalizerChanged(null, e);
        }

        #endregion
    }
}
