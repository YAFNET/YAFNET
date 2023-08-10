/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF;

using System;
using System.Globalization;
using System.IO;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using UAParser.Extensions;

using YAF.Configuration;
using YAF.Core;
using YAF.Core.Context;
using YAF.Core.Hubs;
using YAF.Core.Middleware;
using YAF.Core.Modules;
using YAF.Types;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// The startup.
/// </summary>
public class Startup : IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">
    /// The configuration.
    /// </param>
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// The configure services.
    /// </summary>
    /// <param name="services">
    /// The services.
    /// </param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddControllers();
        services.AddSignalR();

        services.AddMemoryCache();

        services.AddUserAgentParser();

        services.Configure<IdentityOptions>(
            options =>
            {
                // Password settings.
                options.Password.RequireDigit = BoardContext.Current.BoardSettings.PasswordRequireDigit;
                options.Password.RequireLowercase = BoardContext.Current.BoardSettings.PasswordRequireLowercase;
                options.Password.RequireNonAlphanumeric =
                    BoardContext.Current.BoardSettings.PasswordRequireNonLetterOrDigit;
                options.Password.RequireUppercase = BoardContext.Current.BoardSettings.PasswordRequireUppercase;
                options.Password.RequiredLength = BoardContext.Current.BoardSettings.MinRequiredPasswordLength;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

        services.AddDefaultIdentity<AspNetUsers>(o => o.SignIn.RequireConfirmedAccount = true)
            .AddUserManager<AspNetUserManager<AspNetUsers>>().AddRoles<AspNetRoles>().AddDefaultTokenProviders();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(4);
            options.Cookie.Name = ".YAFNET.Session";
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.HttpOnly = true;
        });

        services.AddHttpContextAccessor();

        // Mail Configuration
        services.Configure<MailConfiguration>(this.Configuration.GetSection("MailConfiguration"));

        // Board Configuration
        services.Configure<BoardConfiguration>(this.Configuration.GetSection("BoardConfiguration"));

        var boardConfig = this.Configuration.GetSection("BoardConfiguration").Get<BoardConfiguration>();

        var authenticationBuilder = services.AddAuthentication();

        authenticationBuilder.AddCookie(
            options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Info";
                options.SlidingExpiration = true;
            });

        if (boardConfig.GoogleClientSecret.IsSet() && boardConfig.GoogleClientID.IsSet())
        {
            authenticationBuilder.AddGoogle(
                AuthService.google.ToString(),
                options =>
                {
                    options.ClientId = boardConfig.GoogleClientID;
                    options.ClientSecret = boardConfig.GoogleClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;

                    options.ClaimActions.MapJsonKey("urn:google:email", "email", "string");
                    options.ClaimActions.MapJsonKey("urn:google:id", "id", "string");
                    options.ClaimActions.MapJsonKey("urn:google:name", "name", "string");
                });
        }

        if (boardConfig.FacebookSecretKey.IsSet() && boardConfig.FacebookAPIKey.IsSet())
        {
            authenticationBuilder.AddFacebook(
                AuthService.facebook.ToString(),
                options =>
                {
                    options.ClientId = boardConfig.FacebookAPIKey;
                    options.ClientSecret = boardConfig.FacebookSecretKey;
                    options.SignInScheme = IdentityConstants.ExternalScheme;

                    options.Scope.Add("email");

                    options.Fields.Add("name");
                    options.Fields.Add("email");

                    options.ClaimActions.MapJsonKey("urn:facebook:email", "email", "string");
                    options.ClaimActions.MapJsonKey("urn:facebook:id", "id", "string");
                    options.ClaimActions.MapJsonKey("urn:facebook:name", "name", "string");
                });
        }

        if (boardConfig.TwitterConsumerSecret.IsSet() && boardConfig.TwitterConsumerKey.IsSet())
        {
            authenticationBuilder.AddTwitter(
                AuthService.twitter.ToString(),
                options =>
                {
                    options.ConsumerKey = boardConfig.TwitterConsumerKey;
                    options.ConsumerSecret = boardConfig.TwitterConsumerSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;

                    options.ClaimActions.MapJsonKey("urn:twitter:id", "id", "string");
                    options.ClaimActions.MapJsonKey("urn:twitter:name", "name", "string");
                });
        }

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] {
                                              new CultureInfo("ar"),
                                              new CultureInfo("zh-CN"),
                                              new CultureInfo("zh-TW"),
                                              new CultureInfo("cs"),
                                              new CultureInfo("da"),
                                              new CultureInfo("nl"),
                                              new CultureInfo("en-US"),
                                              new CultureInfo("et"),
                                              new CultureInfo("fi"),
                                              new CultureInfo("fr"),
                                              new CultureInfo("de-DE"),
                                              new CultureInfo("he"),
                                              new CultureInfo("it"),
                                              new CultureInfo("lt"),
                                              new CultureInfo("no"),
                                              new CultureInfo("fa"),
                                              new CultureInfo("pl"),
                                              new CultureInfo("pt"),
                                              new CultureInfo("ro"),
                                              new CultureInfo("ru"),
                                              new CultureInfo("sk"),
                                              new CultureInfo("es"),
                                              new CultureInfo("sv"),
                                              new CultureInfo("tr"),
                                              new CultureInfo("vi")
                                          };

            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });
        
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>().AddScoped(
              x => x.GetRequiredService<IUrlHelperFactory>()
                  .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));

        services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

        services.AddOptions();
    }

    /// <summary>
    /// The configure container.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule<BootstrapModule>();

        builder.Register(k => k.Resolve<IComponentContext>().Resolve<IOptions<MailConfiguration>>().Value)
            .As<MailConfiguration>();
        builder.Register(k => k.Resolve<IComponentContext>().Resolve<IOptions<BoardConfiguration>>().Value)
            .As<BoardConfiguration>();
    }

    /// <summary>
    /// The configure.
    /// </summary>
    /// <param name="app">
    /// The app.
    /// </param>
    /// <param name="env">
    /// The env.
    /// </param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            //app.UseExceptionHandler("/Error");
            app.UseDeveloperExceptionPage();

            app.UseHsts();
        }

        app.UseAntiXssMiddleware();

        app.UseStaticFiles();

        app.UseSession();

        GlobalContainer.AutoFacContainer = app.ApplicationServices.GetAutofacRoot();

        GlobalContainer.AutoFacContainer.Resolve<IInjectServices>().Inject(this);

        ServiceLocatorAccess.CurrentServiceProvider = GlobalContainer.AutoFacContainer.Resolve<IServiceLocator>();

        if (this.Get<BoardConfiguration>().UseHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }

        // Database Connection String
        Config.ConnectionString = this.Configuration.GetConnectionString("DefaultConnection");
        Config.ConnectionProviderName = this.Get<BoardConfiguration>().ConnectionProviderName;
        Config.DatabaseObjectQualifier = this.Get<BoardConfiguration>().DatabaseObjectQualifier;
        Config.SqlCommandTimeout = this.Get<BoardConfiguration>().SqlCommandTimeout;
        Config.DatabaseOwner = this.Get<BoardConfiguration>().DatabaseOwner;
        Config.BoardID = this.Get<BoardConfiguration>().BoardID;
        Config.CategoryID = this.Get<BoardConfiguration>().CategoryID;
        Config.UrlRewritingMode = this.Get<BoardConfiguration>().UrlRewritingMode;

        // Legacy Settings
        Config.LegacyMembershipHashAlgorithmType = this.Get<BoardConfiguration>().LegacyMembershipHashAlgorithmType;
        Config.LegacyMembershipHashCase = this.Get<BoardConfiguration>().LegacyMembershipHashCase;
        Config.LegacyMembershipHashHex = this.Get<BoardConfiguration>().LegacyMembershipHashHex;

        app.UseMiddleware<InitializeDb>();

        var baseDir = env.WebRootPath;

        AppDomain.CurrentDomain.SetData("SearchDataDirectory", Path.Combine(baseDir, "Search_Data"));

        app.Use(
            (httpContext, nextMiddleware) =>
            {
                // app init notification...
                this.Get<IRaiseEvent>().RaiseIssolated(new HttpContextInitEvent(httpContext), null);

                return nextMiddleware();
            });

        app.UseMiddleware<CheckBannedIps>();
        app.UseMiddleware<CheckBannedUserAgents>();

        app.UseRouting();

        var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(localizationOptions);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapWhen(
            context => context.Request.Path.ToString().Contains("sitemap.xml", StringComparison.InvariantCultureIgnoreCase),
            appBranch => appBranch.UseMiddleware<SiteMapMiddleware>());

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllers();
            endpoints.MapHub<NotificationHub>("/NotificationHub");
            endpoints.MapHub<ChatHub>("/ChatHub");
        });
    }
}