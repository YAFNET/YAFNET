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
namespace YAF.Core.BasePages
{
    #region Using

    using System;
    using System.Globalization;
    using System.Threading;
    using System.Web;
    using System.Web.UI;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Services.Startup;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Optional forum page base providing some helper functions.
    /// </summary>
    public class ForumPageBase : Page, IHaveServiceLocator, IRequireStartupServices
    {
        #region Properties

        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

        /// <summary>
        /// Gets the page context.
        /// </summary>
        /// <value>
        /// The page context.
        /// </value>
        public BoardContext PageContext => BoardContext.Current;

        #endregion

        #region Public Methods

        /// <summary>
        /// The initialize culture.
        /// </summary>
        protected override void InitializeCulture()
        {
            var language = "en-US";

            if (this.Session["language"] != null)
            {
                language = this.Session["language"].ToString();
            }
            else
            {
                // Detect User's Language.
                if (this.Request.UserLanguages != null)
                {
                    // Set the Language.
                    language = this.Request.UserLanguages[0];
                }
            }

            SetLanguageUsingThread(language);
        }

        /// <summary>
        /// Handles the Error event of the Page control.
        /// </summary>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected override void OnError(EventArgs e)
        {
            if (!this.Get<StartupInitializeDb>().Initialized)
            {
                return;
            }

            var error = this.Get<HttpServerUtilityBase>().GetLastError();

            if (error.GetType() == typeof(HttpException) && error.InnerException is ViewStateException
                || error.Source.Contains("ViewStateException"))
            {
                if (this.Get<BoardSettings>().LogViewStateError)
                {
                    this.Get<ILogger>()
                        .Log(BoardContext.Current.PageUserID, error.Source, error, EventLogTypes.Information);
                }
            }
            else
            {
                this.Get<ILogger>()
                    .Log(
                        BoardContext.Current.PageUserID,
                        error.Source,
                        error);
            }

            base.OnError(e);
        }

        /// <summary>
        /// The set language using thread.
        /// </summary>
        /// <param name="selectedLanguage">
        /// The selected language.
        /// </param>
        private static void SetLanguageUsingThread(string selectedLanguage)
        {
            var info = CultureInfo.CreateSpecificCulture(selectedLanguage);

            Thread.CurrentThread.CurrentUICulture = info;
            Thread.CurrentThread.CurrentCulture = info;
        }

        #endregion
    }
}