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

namespace YAF.Core.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;

    using Microsoft.Owin.Security;

    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Interfaces;

    /// <summary>
    /// The identity helper class.
    /// </summary>
    public static class IdentityHelper
    {
        /// <summary>
        /// Gets the provider names.
        /// </summary>
        /// <returns>
        /// Returns the Provider Names
        /// </returns>
        public static IEnumerable<string> GetProviderNames()
        {
            return BoardContext.Current.Get<IAuthenticationManager>()
                .GetExternalAuthenticationTypes().Select(t => t.AuthenticationType);
        }

        /// <summary>
        /// The register external login.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="redirectUrl">
        /// The redirect url.
        /// </param>
        public static void RegisterExternalLogin(
            HttpContext context,
            [NotNull] string provider,
            [NotNull] string redirectUrl)
        {
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            BoardContext.Current.Get<IAuthenticationManager>().Challenge(properties, provider);
        }
    }
}