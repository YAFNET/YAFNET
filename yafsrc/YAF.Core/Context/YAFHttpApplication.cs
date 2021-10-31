/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Core.Context
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.UI;

    using Autofac;

    using YAF.Configuration;
    using YAF.Core.Context.Start;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Services.Startup;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Services;

    /// <summary>
    /// The YAF HttpApplication.
    /// </summary>
    public abstract class YafHttpApplication : HttpApplication, IHaveServiceLocator
    {
        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

        /// <summary>
        /// Log Application Errors
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Application_Error([NotNull] object sender, [NotNull] EventArgs e)
        {
            var exception = this.Server.GetLastError();
            var userId = BoardContext.Current?.PageUserID;

            this.Application["Exception"] = exception.ToString();
            this.Application["ExceptionMessage"] = exception.Message;

            if (exception.GetType() == typeof(HttpException) && exception.InnerException is ViewStateException
                || exception.Source.Contains("ViewStateException"))
            {
                bool logViewStateError;

                try
                {
                    logViewStateError = this.Get<BoardSettings>().LogViewStateError;
                }
                catch (Exception)
                {
                    logViewStateError = true;
                }

                if (logViewStateError)
                {
                    this.Get<ILoggerService>().Log(
                        exception.Message,
                        EventLogTypes.Error,
                        exception: exception,
                        userId: userId);
                }
            }
            else
            {
                this.Get<ILoggerService>().Log(
                    exception.Message,
                    EventLogTypes.Error,
                    exception: exception,
                    userId: userId);
            }
        }

        /// <summary>
        /// The application_ end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void Application_End([NotNull] object sender, [NotNull] EventArgs e)
        {
            // make sure the BoardContext is disposed of...
            BoardContext.Current.Dispose();

            if (!this.Get<StartupInitializeDb>().Initialized)
            {
                return;
            }

            if (BoardContext.Current.BoardSettings.AbandonSessionsForDontTrack
                && (BoardContext.Current.Vars.AsBoolean("DontTrack") ?? false)
                && this.Get<HttpSessionStateBase>().IsNewSession)
            {
                // remove session
                this.Get<HttpSessionStateBase>().Abandon();
            }
        }

        /// <summary>
        /// The application_ start.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void Application_Start([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Pass a delegate to the Configure method.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            ScriptManagerHelper.RegisterJQuery();
        }

        /// <summary>
        /// The session_ start.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Session_Start([NotNull] object sender, [NotNull] EventArgs e)
        {
            // run startup services...
            this.RunStartupServices();

            // set the httpApplication as early as possible...
            GlobalContainer.Container.Resolve<CurrentHttpApplicationStateBaseProvider>().Instance =
                new HttpApplicationStateWrapper(this.Application);

            // app init notification...
            this.Get<IRaiseEvent>().RaiseIssolated(new HttpApplicationInitEvent(this), null);
        }
    }
}