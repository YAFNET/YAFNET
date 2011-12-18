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
    using System;
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// Localizer class to work with internal localized strings.
    /// </summary>
    public class FALocalizeManager
    {
        #region Fields

        private readonly FALocalizer fa = new FALocalizer();
        private readonly ARLocalizer ar = new ARLocalizer();
        private readonly ENLocalizer en = new ENLocalizer();
        private BaseLocalizer customLocalizer;
        private static FALocalizeManager instance;

        #endregion

        #region Ctor

        private FALocalizeManager()
        {
            FarsiCulture = new CultureInfo("fa-IR");
            ArabicCulture = new CultureInfo("ar-SA");
            InvariantCulture = CultureInfo.InvariantCulture;
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when Localizer has changed.
        /// </summary>
        public event EventHandler LocalizerChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Returns an instance of the localized based on CurrentUICulture of the thread.
        /// </summary>
        /// <returns></returns>
        public BaseLocalizer GetLocalizer()
        {
            return GetLocalizerByCulture(Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>
        /// Returns a localizer instance based on the culture.
        /// </summary>
        internal BaseLocalizer GetLocalizerByCulture(CultureInfo ci)
        {
            if (customLocalizer != null)
                return customLocalizer;
            
            if (ci.Equals(FarsiCulture))
            {
                return fa;
            }
            
            if (ci.Equals(ArabicCulture))
            {
                return ar;
            }
            
            return en;
        }

        #endregion

        #region Props

        /// <summary>
        /// Singleton Instance of FALocalizeManager.
        /// </summary>
        public static FALocalizeManager Instance
        {
            get
            {
                if(instance == null)
                    instance = new FALocalizeManager();

                return instance;
            }
        }

        /// <summary>
        /// Custom culture, when set , is used across all controls.
        /// </summary>
        public CultureInfo CustomCulture
        {
            get;
            set;
        }

        /// <summary>
        /// Farsi Culture
        /// </summary>
        public CultureInfo FarsiCulture
        {
            get;
            private set;
        }

        /// <summary>
        /// Arabic Culture
        /// </summary>
        public CultureInfo ArabicCulture
        {
            get;
            private set;
        }

        /// <summary>
        /// Invariant Culture
        /// </summary>
        public CultureInfo InvariantCulture
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or Sets a new instance of Localizer. If this value is initialized (default is null), Localize Manager class will use the custom class provided, to interpret localized strings.
        /// </summary>
        public BaseLocalizer CustomLocalizer
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

        /// <summary>
        /// Fires the LocalizerChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnLocalizerChanged(EventArgs e)
        {
            if (LocalizerChanged != null)
                LocalizerChanged(null, e);
        }

        internal bool IsCustomArabicCulture
        {
            get { return CustomCulture != null && CustomCulture.Equals(ArabicCulture); }
        }

        internal bool IsCustomFarsiCulture
        {
            get { return CustomCulture != null && CustomCulture.Equals(FarsiCulture); }
        }

        internal bool IsThreadCultureFarsi
        {
            get { return Thread.CurrentThread.CurrentUICulture.Equals(FarsiCulture); }
        }

        internal bool IsThreadCultureArabic
        {
            get { return Thread.CurrentThread.CurrentUICulture.Equals(ArabicCulture); }
        }

        #endregion
    }
}
