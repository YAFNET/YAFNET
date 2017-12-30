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
namespace YAF
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Services.Startup;
    using YAF.Types;
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
        public IServiceLocator ServiceLocator
        {
            get
            {
                return YafContext.Current.ServiceLocator;
            }
        }

        /// <summary>
        /// Gets the page context.
        /// </summary>
        /// <value>
        /// The page context.
        /// </value>
        public YafContext PageContext
        {
            get
            {
                return YafContext.Current;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the Error event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void Page_Error([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<StartupInitializeDb>().Initialized)
            {
                return;
            }

            var error = this.Get<HttpServerUtilityBase>().GetLastError();

            if (error.GetType() == typeof(HttpException) && error.InnerException is ViewStateException
                || error.Source.Contains("ViewStateException"))
            {
                if (this.Get<YafBoardSettings>().LogViewStateError)
                {
                    this.Get<ILogger>()
                        .Log(YafContext.Current.PageUserID, error.Source, error, EventLogTypes.Information);
                }
            }
            else
            {
                this.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        error.Source,
                        error);
            }
        }

        #endregion
    }
}