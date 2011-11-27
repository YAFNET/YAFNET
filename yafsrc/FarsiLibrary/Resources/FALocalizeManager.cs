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
