/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Twitter;

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

        if (Config.FacebookSecretKey.IsSet() && Config.FacebookAPIKey.IsSet())
        {
            RegisterFacebookMiddleWare(app);
        }

        if (Config.TwitterConsumerSecret.IsSet() && Config.TwitterConsumerKey.IsSet())
        {
            RegisterTwitterMiddleWare(app);
        }

        if (Config.GoogleClientSecret.IsSet() && Config.GoogleClientID.IsSet())
        {
            RegisterGoogleMiddleWare(app);
        }

        /*if (Config.GitHubClientSecret.IsSet() && Config.GitHubClientID.IsSet())
        {
            RegisterGitHubMiddleWare(app);
        }

        if (Config.MicrosoftAccountClientSecret.IsSet() && Config.MicrosoftAccountClientID.IsSet())
        {
            RegisterMicrosoftAccountMiddleWare(app);
        }*/
    }

    /// <summary>
    /// Register facebook Authentication.
    /// </summary>
    /// <param name="app">
    /// The app builder.
    /// </param>
    private static void RegisterFacebookMiddleWare(IAppBuilder app)
    {
        var options = new FacebookAuthenticationOptions
                          {
                              AppId = Config.FacebookAPIKey,
                              AppSecret = Config.FacebookSecretKey,
                              Provider = new FacebookAuthenticationProvider
                                             {
                                                 OnAuthenticated = context =>
                                                     {
                                                         context.Identity.AddClaim(
                                                             new Claim("urn:facebook:email", context.Email, "XmlSchemaString", "Facebook"));
                                                         context.Identity.AddClaim(
                                                             new Claim("urn:facebook:id", context.Id, "XmlSchemaString", "Facebook"));
                                                         context.Identity.AddClaim(
                                                             new Claim("urn:facebook:name", context.Name, "XmlSchemaString", "Facebook"));

                                                         return Task.FromResult(0);
                                                     }
                                             }
                          };

        app.UseFacebookAuthentication(options);
    }

    /// <summary>
    /// Register Twitter Authentication.
    /// </summary>
    /// <param name="app">
    /// The app builder.
    /// </param>
    private static void RegisterTwitterMiddleWare(IAppBuilder app)
    {
        var options = new TwitterAuthenticationOptions
                          {
                              ConsumerKey = Config.TwitterConsumerKey,
                              ConsumerSecret = Config.TwitterConsumerSecret,
                              Provider = new TwitterAuthenticationProvider
                                             {
                                                 OnAuthenticated = context =>
                                                     {
                                                         context.Identity.AddClaim(
                                                             new Claim("urn:twitter:id", context.UserId, "XmlSchemaString", "Twitter"));
                                                         context.Identity.AddClaim(
                                                             new Claim("urn:twitter:name", context.ScreenName, "XmlSchemaString", "Twitter"));

                                                         return Task.FromResult(0);
                                                     }
                                             }
                          };

        app.UseTwitterAuthentication(options);
    }

    /// <summary>
    /// Register Google Authentication.
    /// </summary>
    /// <param name="app">
    /// The app builder.
    /// </param>
    private static void RegisterGoogleMiddleWare(IAppBuilder app)
    {
        var options = new GoogleOAuth2AuthenticationOptions
                          {
                              ClientId = Config.GoogleClientID,
                              ClientSecret = Config.GoogleClientSecret,
                              Provider = new GoogleOAuth2AuthenticationProvider
                                             {
                                                 OnAuthenticated = context =>
                                                     {
                                                         context.Identity.AddClaim(
                                                             new Claim("urn:google:email", context.Email, "XmlSchemaString", "Google"));
                                                         context.Identity.AddClaim(
                                                             new Claim("urn:google:id", context.Id, "XmlSchemaString", "Google"));
                                                         context.Identity.AddClaim(
                                                             new Claim("urn:google:name", context.Name, "XmlSchemaString", "Google"));

                                                         return Task.FromResult(0);
                                                     }
                                             }
                          };

        app.UseGoogleAuthentication(options);
    }

    /*

    /// <summary>
    /// Register GitHub Authentication.
    /// </summary>
    /// <param name="app">
    /// The app builder.
    /// </param>
    private static void RegisterGitHubMiddleWare(IAppBuilder app)
    {
        var options = new GitHubAuthenticationOptions
        {
            ClientId = Config.GitHubClientID,
            ClientSecret = Config.GitHubClientSecret,
            Provider = new GitHubAuthenticationProvider
            {
                OnAuthenticated = context =>
                {
                    BoardContext.Current.Get<ILogger>().Info(context.Email);
                    context.Identity.AddClaim(
                        new Claim("urn:github:email", context.Email, "XmlSchemaString", "GitHub"));
                    context.Identity.AddClaim(
                        new Claim("urn:github:id", context.Id, "XmlSchemaString", "GitHub"));
                    context.Identity.AddClaim(
                        new Claim("urn:github:name", context.Name, "XmlSchemaString", "GitHub"));
                    context.Identity.AddClaim(
                        new Claim("urn:github:username", context.UserName, "XmlSchemaString", "GitHub"));
                    context.Identity.AddClaim(
                        new Claim("urn:github:link", context.Link, "XmlSchemaString", "GitHub"));

                    return Task.FromResult(0);
                }
            }
        };

        app.UseGitHubAuthentication(options);
    }

    /// <summary>
    /// Register MicrosoftAccount Authentication.
    /// </summary>
    /// <param name="app">
    /// The app builder.
    /// </param>
    private static void RegisterMicrosoftAccountMiddleWare(IAppBuilder app)
    {
        var options = new MicrosoftAccountAuthenticationOptions
        {
            ClientId = Config.MicrosoftAccountClientID,
            ClientSecret = Config.MicrosoftAccountClientSecret,
            Provider = new MicrosoftAccountAuthenticationProvider
            {
                OnAuthenticated = context =>
                {
                    context.Identity.AddClaim(
                        new Claim("urn:microsoft:email", context.Email, "XmlSchemaString", "Microsoft"));
                    context.Identity.AddClaim(
                        new Claim("urn:microsoft:id", context.Id, "XmlSchemaString", "Microsoft"));
                    context.Identity.AddClaim(
                        new Claim("urn:microsoft:name", context.Name, "XmlSchemaString", "Microsoft"));

                    return Task.FromResult(0);
                }
            }
        };

        app.UseMicrosoftAccountAuthentication(options);
    }*/
}