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

namespace YAF.Core.Services
{
    #region Using

    using System.Collections.Generic;
    using System.Web;

    using YAF.Core.Context;
    using YAF.Core.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The IP Info Service
    /// </summary>
    public class IpInfoService : IIpInfoService, IHaveServiceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IpInfoService"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public IpInfoService([NotNull] IServiceLocator serviceLocator)
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
        /// Get the User IP Locator
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<string, string> GetUserIpLocator()
        {
            return this.GetUserIpLocator(this.Get<HttpRequestBase>().GetUserRealIPAddress());
        }

        /// <summary>
        /// Get the User IP Locator
        /// </summary>
        /// <param name="ipAddress">
        /// The IP Address.
        /// </param>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<string, string> GetUserIpLocator(string ipAddress)
        {
            if (ipAddress.IsNotSet())
            {
                return null;
            }

            var userIpLocator = new IPDetails().GetData(
                ipAddress,
                "text",
                false,
                BoardContext.Current.CurrentForumPage.Localization.Culture.Name,
                string.Empty,
                string.Empty);

            if (userIpLocator == null)
            {
                return null;
            }

            if (userIpLocator["StatusCode"] == "OK")
            {
                return userIpLocator;
            }

            this.Get<ILogger>().Log(
                null,
                this,
                $"Geolocation Service reports: {userIpLocator["StatusMessage"]}",
                EventLogTypes.Information);

            return null;
        }
    }
}