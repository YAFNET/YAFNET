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

namespace YAF.Core.Modules
{
    #region Using

    using System.Linq;

    using Autofac;
    using Autofac.Core;

    using YAF.Core.Services.Logger;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The logging module.
    /// </summary>
    public class LoggingModule : BaseModule
    {
        #region Methods

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="builder">
        ///     The builder.
        /// </param>
        protected override void Load([NotNull] ContainerBuilder builder)
        {
            CodeContracts.VerifyNotNull(builder, "builder");

            this.ComponentRegistry.Registered += (sender, e) => e.ComponentRegistration.Preparing += OnComponentPreparing;

            if (this.IsRegistered<ILoggerProvider>())
            {
                return;
            }

            builder.RegisterType<YafDbLoggerProvider>().As<ILoggerProvider>().SingleInstance();
            builder.Register(c => c.Resolve<ILoggerProvider>().Create(null)).SingleInstance();
        }

        /// <summary>
        ///     The on component preparing.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private static void OnComponentPreparing([NotNull] object sender, [NotNull] PreparingEventArgs e)
        {
            var t = e.Component.Activator.LimitType;
            e.Parameters =
                e.Parameters.Union(
                    new[]
                    {
                        new ResolvedParameter(
                            (p, i) => p.ParameterType == typeof(ILogger),
                            (p, i) => i.Resolve<ILoggerProvider>().Create(t))
                    });
        }

        #endregion
    }
}