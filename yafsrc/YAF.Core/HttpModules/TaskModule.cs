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
namespace YAF.Core
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI;

    using Autofac;

    using YAF.Core.Context;
    using YAF.Core.Helpers;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;

    #endregion

    /// <summary>
    /// Lifecycle module used to throw events around...
    /// </summary>
    public class TaskModule : IHttpModule, IHaveServiceLocator
    {
        #region Constants and Fields

        /// <summary>
        ///   The _app instance.
        /// </summary>
        private HttpApplication appInstance;

        /// <summary>
        ///   The _module initialized.
        /// </summary>
        private bool moduleInitialized;

        /// <summary>
        ///   Gets or sets the logger associated with the object.
        /// </summary>
        [Inject]
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the service locator.
        /// </summary>
        [Inject]
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Implemented Interfaces

        #region IHttpModule

        /// <summary>
        /// Bootstrapping fun
        /// </summary>
        /// <param name="httpApplication">
        /// The http application.
        /// </param>
        public void Init([NotNull] HttpApplication httpApplication)
        {
            CodeContracts.VerifyNotNull(httpApplication, "httpApplication");

            if (this.moduleInitialized)
            {
                return;
            }

            // create a lock so no other instance can affect the static variable
            lock (this)
            {
                if (!this.moduleInitialized)
                {
                    this.appInstance = httpApplication;

                    // set the httpApplication as early as possible...
                    GlobalContainer.Container.Resolve<CurrentHttpApplicationStateBaseProvider>().Instance =
                        new HttpApplicationStateWrapper(httpApplication.Application);

                    GlobalContainer.Container.Resolve<IInjectServices>().Inject(this);

                    this.moduleInitialized = true;

                    this.appInstance.PreRequestHandlerExecute += this.ApplicationPreRequestHandlerExecute;
                }
            }

            // app init notification...
            this.Get<IRaiseEvent>().RaiseIssolated(new HttpApplicationInitEvent(this.appInstance), null);
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        void IHttpModule.Dispose()
        {
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The application pre request handler execute.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ApplicationPreRequestHandlerExecute([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (HttpContext.Current.CurrentHandler == null || !(HttpContext.Current.CurrentHandler is Page))
            {
                return;
            }

            var page = HttpContext.Current.CurrentHandler as Page;

            try
            {
                // call from BoardContext only -- so that the events have access to the full BoardContext lifecycle.
                BoardContext.Current.Get<IRaiseEvent>().RaiseIssolated(
                    new EventPreRequestPageExecute(page),
                    (m, ex) => this.Logger.Fatal(ex, $"Failed to Call Event Pre Request Page Execute Event {m}"));
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Exception in PreRequestHandlerExecute.");
            }
        }

        #endregion
    }
}