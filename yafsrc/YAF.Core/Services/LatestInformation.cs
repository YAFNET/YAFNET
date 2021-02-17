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

namespace YAF.Core.Services
{
    using System;
    using System.Dynamic;
    using System.Net;

    using ServiceStack;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
    /// LatestInformation service class
    /// </summary>
    public class LatestInformation : IHaveServiceLocator, ILatestInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LatestInformation"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public LatestInformation([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        /// <summary>
        /// Gets the latest version information.
        /// </summary>
        /// <returns>Returns the LatestVersionInformation</returns>
        public dynamic GetLatestVersion()
        {
            dynamic version = new ExpandoObject();

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                                       SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                var test = "https://api.github.com/repos/YAFNET/YAFNET/releases/latest".GetJsonFromUrl(
                    x => x.UserAgent = "YAF.NET");

                var json = DynamicJson.Deserialize(test);

                var tagName = (string)json.tag_name;
                var date = System.DateTime.SpecifyKind(
                    System.DateTime.Parse(json.published_at),
                    DateTimeKind.Unspecified);

                version.UpgradeUrl = (string)json.assets[2].browser_download_url;
                version.VersionDate = date;
                version.Version = tagName.Replace("v", string.Empty);
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Error(x, "Exception In LatestInformationService");
                return null;
            }

            return version;
        }
    }
}