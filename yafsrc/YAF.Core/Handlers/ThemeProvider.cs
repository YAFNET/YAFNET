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

    using YAF.Core.Context;
    using YAF.Core.Services;
    using YAF.Types.Exceptions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The theme handler.
    /// </summary>
    public class ThemeProvider
    {
        #region Constants and Fields

        /// <summary>
        ///   The init theme.
        /// </summary>
        private bool initTheme;

        /// <summary>
        ///   The theme.
        /// </summary>
        private ITheme theme;

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
        ///   Gets or sets Theme.
        /// </summary>
        public ITheme Theme
        {
            get
            {
                if (!this.initTheme)
                {
                    this.InitTheme();
                }

                return this.theme;
            }

            set
            {
                this.theme = value;
                this.initTheme = value != null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the theme class up for usage
        /// </summary>
        /// <exception cref="CantLoadThemeException"><c>CantLoadThemeException</c>.</exception>
        private void InitTheme()
        {
            if (this.initTheme)
            {
                return;
            }

            this.BeforeInit?.Invoke(this, new EventArgs());

            string themeFile;

            if (BoardContext.Current.Page != null && BoardContext.Current.Page["ThemeFile"] != null &&
                BoardContext.Current.BoardSettings.AllowUserTheme)
            {
                // use user-selected theme
                themeFile = BoardContext.Current.Page["ThemeFile"].ToString();
            }
            else if (BoardContext.Current.Page != null && BoardContext.Current.Page["ForumTheme"] != null)
            {
                themeFile = BoardContext.Current.Page["ForumTheme"].ToString();
            }
            else
            {
                themeFile = BoardContext.Current.BoardSettings.Theme;
            }

            if (!Services.Theme.IsValidTheme(themeFile))
            {
                themeFile = "yaf";
            }

            // create the theme class
            this.Theme = new Theme(themeFile);

            this.AfterInit?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}