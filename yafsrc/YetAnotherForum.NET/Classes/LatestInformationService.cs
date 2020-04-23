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

namespace YAF.Classes
{
    using System;
    using System.Web;

    using YAF.Core.Context;
    using YAF.RegisterV2;
#if DEBUG
    using YAF.Types.Extensions;
#endif
    using YAF.Types.Interfaces;

    /// <summary>
    /// LatestInformation service class
    /// </summary>
    public class LatestInformationService : IHaveServiceLocator
    {
        /// <summary>
        /// Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

        /// <summary>
        /// Gets the latest version information.
        /// </summary>
        /// <returns>Returns the LatestVersionInformation</returns>
        public byte[] GetLatestVersion()
        {
            if (this.Get<HttpApplicationStateBase>()["YafRegistrationLatestInformation"] is byte[]
                    latestInfo)
            {
                return latestInfo;
            }

            try
            {
                var reg = new RegisterV2 { Timeout = 30000 };

                // load the latest version
                latestInfo = reg.LatestVersion();

                if (latestInfo != null)
                {
                    this.Get<HttpApplicationStateBase>().Set("YafRegistrationLatestInformation", latestInfo);
                }
            }
#if DEBUG
            catch (Exception x)
            {
                this.Get<ILogger>().Error(x, "Exception In LatestInformationService");
#else
            catch (Exception)
            {
#endif
                return null;
            }

            return latestInfo;
        }
    }
}