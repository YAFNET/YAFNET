/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Context.Start;

using System;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;

using Owin;

using YAF.Core.Identity;
using YAF.Types.Models.Identity;

/// <summary>
/// The startup.
/// </summary>
public partial class Startup
{
    /// <summary>
    /// Gets or sets the data protection provider.
    /// </summary>
    public static IDataProtectionProvider DataProtectionProvider { get; set; }

    /// <summary>
    /// Configure the Authentication.
    /// </summary>
    /// <param name="app">
    /// The app.
    /// </param>
    public void ConfigureAuth(IAppBuilder app)
    {
        if (Config.IsDotNetNuke)
        {
            return;
        }

        app.UseCookieAuthentication(
            new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Account/Login"),
                    Provider = new CookieAuthenticationProvider
                                   {
                                       OnValidateIdentity =
                                           SecurityStampValidator.OnValidateIdentity<AspNetUsersManager, AspNetUsers>(
                                               TimeSpan.FromMinutes(30),
                                               (manager, user) => Task.FromResult(
                                                   manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie)))
                                   }
                });

        DataProtectionProvider = app.GetDataProtectionProvider();

        app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
    }
}