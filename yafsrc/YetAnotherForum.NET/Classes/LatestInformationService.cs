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

namespace YAF.Classes
{
    using System;
    using System.Web;

    using YAF.Core;
    using YAF.RegisterV2;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// LatestInformation service class
    /// </summary>
    public class LatestInformationService : IHaveServiceLocator
    {
        /// <summary>
        /// Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator
        {
            get
            {
                return YafContext.Current.ServiceLocator;
            }
        }

        /// <summary>
        /// Gets the latest version information.
        /// </summary>
        /// <returns>Returns the LatestVersionInformation</returns>
        public LatestVersionInformation GetLatestVersionInformation()
        {
            var latestInfo =
                this.Get<HttpApplicationStateBase>()["YafRegistrationLatestInformation"] as LatestVersionInformation;

            if (latestInfo != null)
            {
                return latestInfo;
            }

            try
            {
                using (var reg = new RegisterV2())
                {
                    reg.Timeout = 30000;

                    // load the latest info -- but only provide the current version information and the user's two-letter language information. Nothing trackable.))
                    latestInfo = reg.LatestInfo(YafForumInfo.AppVersionCode, "US");

                    if (latestInfo != null)
                    {
                        this.Get<HttpApplicationStateBase>().Set("YafRegistrationLatestInformation", latestInfo);
                    }
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