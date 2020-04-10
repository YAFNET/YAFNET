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
namespace YAF.Configuration
{
    #region Using

    using YAF.Configuration.Pattern;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The YAF Factory provider.
    /// </summary>
    public static class FactoryProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The builder factory.
        /// </summary>
        private static ITypeFactoryInstance<IUrlBuilder> builderFactory;

        /// <summary>
        /// The user display name factory.
        /// </summary>
        private static ITypeFactoryInstance<IUserDisplayName> userDisplayNameFactory;

        #endregion

        #region Properties

        /// <summary>
        /// Gets UrlBuilder.
        /// </summary>
        public static IUrlBuilder UrlBuilder
        {
            get
            {
                if (builderFactory == null)
                {
                    builderFactory = new TypeFactoryInstanceApplicationBoardScope<IUrlBuilder>(UrlBuilderType);
                }

                return builderFactory.Get();
            }
        }

        /// <summary>
        /// Gets current <see cref="IUserDisplayName"/>.
        /// </summary>
        public static IUserDisplayName UserDisplayName
        {
            get
            {
                if (userDisplayNameFactory == null)
                {
                    userDisplayNameFactory =
                        new TypeFactoryInstanceApplicationBoardScope<IUserDisplayName>(UserDisplayNameType);
                }

                return userDisplayNameFactory.Get();
            }
        }

        /// <summary>
        /// Gets UrlBuilderType.
        /// </summary>
        private static string UrlBuilderType
        {
            get
            {
                var urlAssembly = Config.GetProvider("UrlBuilder");

                if (urlAssembly.IsSet())
                {
                    return urlAssembly;
                }

                if (Config.IsDotNetNuke)
                {
                    urlAssembly = "YAF.DotNetNuke.DotNetNukeUrlBuilder,YAF.DotNetNuke.Module";
                }
                else if (Config.IsMojoPortal)
                {
                    urlAssembly = "YAF.Mojo.MojoPortalUrlBuilder,YAF.Mojo";
                }
                else if (Config.IsRainbow)
                {
                    urlAssembly = "yaf_rainbow.RainbowUrlBuilder,yaf_rainbow";
                }
                else if (Config.IsPortal)
                {
                    urlAssembly = "Portal.UrlBuilder,Portal";
                }
                else if (Config.EnableURLRewriting)
                {
                    urlAssembly = "YAF.Core.URLBuilder.AdvancedUrlRewriter,YAF.Core";
                }
                else
                {
                    urlAssembly = "YAF.Configuration.DefaultUrlBuilder";
                }

                return urlAssembly;
            }
        }

        /// <summary>
        /// Gets UserDisplayNameType.
        /// </summary>
        private static string UserDisplayNameType =>
            Config.GetProvider("UserDisplayName").IsSet()
                ? Config.GetProvider("UserDisplayName")
                : "YAF.Core.DefaultUserDisplayName,YAF.Core";

        #endregion
    }
}