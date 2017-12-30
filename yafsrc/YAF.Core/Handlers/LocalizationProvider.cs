/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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