/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Handlers
{
    #region Using

    using System;

    using YAF.Core.Services.Localization;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The localization handler.
    /// </summary>
    public class LocalizationProvider
    {
        #region Constants and Fields

        /// <summary>
        ///   The localization is Initialized flag.
        /// </summary>
        private bool initLocalization;

        /// <summary>
        ///   The localization.
        /// </summary>
        private ILocalization localization;

        /// <summary>
        ///   The trans page.
        /// </summary>
        private string transPage = string.Empty;

        #endregion

        #region Events

        /// <summary>
        ///   The after initializing Event.
        /// </summary>
        public event EventHandler<EventArgs> AfterInit;

        /// <summary>
        ///   The before initializing Event.
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
                if (!this.initLocalization)
                {
                    this.InitLocalization();
                }

                return this.localization;
            }

            set
            {
                this.localization = value;
                this.initLocalization = value != null;
            }
        }

        /// <summary>
        ///   Gets or sets the Current TransPage for Localization
        /// </summary>
        public string TranslationPage
        {
            get => this.transPage;

            set
            {
                if (value == this.transPage)
                {
                    return;
                }

                this.transPage = value;

                if (this.initLocalization)
                {
                    // re-init localization
                    this.Localization = null;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set up the localization
        /// </summary>
        protected void InitLocalization()
        {
            if (this.initLocalization)
            {
                return;
            }

            this.BeforeInit?.Invoke(this, new EventArgs());

            this.Localization = new Localization(this.TranslationPage);

            this.AfterInit?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}