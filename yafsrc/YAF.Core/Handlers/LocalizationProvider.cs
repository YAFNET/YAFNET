/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core
{
    #region Using

    using System;
    using System.Globalization;
    using System.Threading;

    using YAF.Core.Services.Localization;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Classes.Data;

    #endregion

    /// <summary>
    /// The localization handler.
    /// </summary>
    public class LocalizationProvider
    {
        #region Constants and Fields

        /// <summary>
        ///   The _init culture.
        /// </summary>
        private bool _initCulture;

        /// <summary>
        ///   The _init localization.
        /// </summary>
        private bool _initLocalization;

        /// <summary>
        ///   The _localization.
        /// </summary>
        private ILocalization _localization;

        /// <summary>
        ///   The _trans page.
        /// </summary>
        private string _transPage = string.Empty;

        #endregion

        #region Events

        /// <summary>
        ///   The after init.
        /// </summary>
        public event EventHandler<EventArgs> AfterInit;

        /// <summary>
        ///   The before init.
        /// </summary>
        public event EventHandler<EventArgs> BeforeInit;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Localization.
        /// </summary>
        public ILocalization Localization
        {
            get
            {
                if (!this._initLocalization)
                {
                    this.InitLocalization();
                }

                if (!this._initCulture)
                {
                    this.InitCulture();
                }

                return this._localization;
            }

            set
            {
                this._localization = value;
                this._initLocalization = value != null;
                this._initCulture = value != null;
            }
        }

        /// <summary>
        ///   Current TransPage for Localization
        /// </summary>
        public string TranslationPage
        {
            get
            {
                return this._transPage;
            }

            set
            {
                if (value != this._transPage)
                {
                    this._transPage = value;

                    if (this._initLocalization)
                    {
                        // re-init localization
                        this.Localization = null;
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the culture and UI culture to the browser's accept language
        /// </summary>
        protected void InitCulture()
        {
            if (!this._initCulture)
            {
                try
                {
                    string cultureCode = string.Empty;

                    /*string [] tmp = YafContext.Current.Get<HttpRequestBase>().UserLanguages;
                              if ( tmp != null )
                              {
                                  cultureCode = tmp [0];
                                  if ( cultureCode.IndexOf( ';' ) >= 0 )
                                  {
                                      cultureCode = cultureCode.Substring( 0, cultureCode.IndexOf( ';' ) ).Replace( '_', '-' );
                                  }
                              }
                              else
                              {
                                  cultureCode = "en-US";
                              }*/
                    cultureCode = this._localization.LanguageCode;

                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureCode);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
                }

#if DEBUG
                catch (Exception ex)
                {
                    YafContext.Current.Get<ILogger>()
                              .Error(ex, "Error In Loading User Language for UserID {0}".FormatWith(YafContext.Current.PageUserID));

                    throw new ApplicationException(string.Format("Error getting User Language.{0}{1}", Environment.NewLine, ex));
                }

#else
				catch ( Exception )
				{
					// set to default...
					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture( "en-US" );
					Thread.CurrentThread.CurrentUICulture = new CultureInfo( "en-US" );
				}
#endif

                // mark as setup...
                this._initCulture = true;
            }
        }

        /// <summary>
        /// Set up the localization
        /// </summary>
        protected void InitLocalization()
        {
            if (!this._initLocalization)
            {
                if (this.BeforeInit != null)
                {
                    this.BeforeInit(this, new EventArgs());
                }

                this.Localization = new YafLocalization(this.TranslationPage);

                if (this.AfterInit != null)
                {
                    this.AfterInit(this, new EventArgs());
                }
            }
        }

        #endregion
    }
}