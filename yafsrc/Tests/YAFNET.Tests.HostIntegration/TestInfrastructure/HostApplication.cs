/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
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

namespace YAF.Tests.HostIntegration.TestInfrastructure;

using System.Data.Common;
using System.Data.SQLite;
using System.IO;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Identity;
using YAF.Core.Services;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Identity;

/// <summary>
/// What the forum knows about the current user.
/// </summary>
public record ForumIdentity(string IdentityName, string ForumUserName, bool IsGuest, bool IsAdmin, string UserStore);

/// <summary>
/// What the host application knows about the current user.
/// </summary>
public record HostAccount(string UserName, bool IsRegisteredUserByClaim);

/// <summary>
/// Who sets up ASP.NET Core Identity in the application.
/// </summary>
public enum HostIdentityMode
{
    /// <summary>
    /// The forum, like in the forum application.
    /// </summary>
    Forum,

    /// <summary>
    /// The host application, for the identity models of the forum. The accounts stay in the forum stores.
    /// </summary>
    HostWithForumTypes,

    /// <summary>
    /// The host application, for its own user and role types in its own EF Core database.
    /// The forum gets adapter stores.
    /// </summary>
    HostWithOwnTypes
}

/// <summary>
/// A web application that hosts the forum, and can manage the accounts with its own
/// ASP.NET Core Identity (its own stores, options and cookie).
/// </summary>
public sealed class HostApplication : IAsyncDisposable
{
    /// <summary>
    /// The name of the forum.
    /// </summary>
    public const string ForumName = "Host Integration Forum";

    /// <summary>
    /// The name of the admin user.
    /// </summary>
    public const string AdminName = "HostAdmin";

    /// <summary>
    /// The password of all users.
    /// </summary>
    public const string Password = "Host-Password-1234";

    /// <summary>
    /// The password length the host requires, different from the default of the forum.
    /// </summary>
    public const int RequiredPasswordLength = 14;

    private readonly string contentRoot;

    private HostApplication(WebApplication app, string contentRoot)
    {
        this.DatabaseFile = Path.Combine(contentRoot, "yafnet.db");
        this.App = app;
        this.contentRoot = contentRoot;
    }

    /// <summary>
    /// Gets the running application.
    /// </summary>
    public WebApplication App { get; }

    /// <summary>
    /// Gets the SQLite database file of the forum.
    /// </summary>
    public string DatabaseFile { get; }

    /// <summary>
    /// Gets the names of the users in the identity database of the host.
    /// </summary>
    public IList<string> HostUserNames => this.ReadHostDatabase(db => [.. db.Users.Select(u => u.UserName)]);

    /// <summary>
    /// Gets the names of the roles in the identity database of the host.
    /// </summary>
    public IList<string> HostRoleNames => this.ReadHostDatabase(db => [.. db.Roles.Select(r => r.Name)]);

    /// <summary>
    /// Builds and starts the host application, without installing the forum.
    /// </summary>
    public async static Task<HostApplication> StartAsync(HostIdentityMode mode)
    {
        var contentRoot = Directory.CreateTempSubdirectory("yaf-host-").FullName;

        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                // the forum pages, view components and controllers are application parts of the forum assembly
                ApplicationName = "YAF",
                ContentRootPath = contentRoot,
                WebRootPath = FindWebRoot(),
                EnvironmentName = Environments.Development
            });

        builder.WebHost.UseTestServer();

        builder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    $"Data Source={Path.Combine(contentRoot, "yafnet.db")};Version=3;",
                ["BoardConfiguration:BoardID"] = "1",
                ["BoardConfiguration:CategoryID"] = "0",
                ["BoardConfiguration:ConnectionProviderName"] = "System.Data.SQLite",
                ["BoardConfiguration:DatabaseObjectQualifier"] = "yaf_",
                ["BoardConfiguration:DatabaseOwner"] = "dbo",
                ["BoardConfiguration:SqlCommandTimeout"] = "99999",
                ["BoardConfiguration:URLRewritingMode"] = "Unicode",
                ["BoardConfiguration:AllowLoginAndLogoff"] = "true",
                ["BoardConfiguration:UseHttpsRedirection"] = "false",
                ["BoardConfiguration:UseRateLimiter"] = "true",
                // all requests of the tests come from the same address
                ["BoardConfiguration:RateLimiterPermitLimit"] = "1000",
                ["BoardConfiguration:HostManagesAccounts"] = (mode != HostIdentityMode.Forum).ToString()
            });

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        builder.Host.ConfigureYafLogging();

        builder.Host.ConfigureContainer<ContainerBuilder>(
            containerBuilder =>
            {
                containerBuilder.RegisterYafModules();

                // the data provider of the forum application is loaded too and registers its own factory
                containerBuilder.RegisterInstance<DbProviderFactory>(SQLiteFactory.Instance).ExternallyOwned();
            });

        switch (mode)
        {
            case HostIdentityMode.HostWithForumTypes:
                // the host owns the sign-in: its options and its cookie, the accounts stay in the forum stores
                builder.Services.AddIdentity<AspNetUsers, AspNetRoles>(ConfigureHostIdentity)
                    .AddDefaultTokenProviders();

                break;

            case HostIdentityMode.HostWithOwnTypes:
                builder.Services.AddDbContext<HostDbContext>(
                    options => options.UseSqlite($"Data Source={Path.Combine(contentRoot, "host.db")}"));

                builder.Services.AddIdentity<HostUser, HostRole>(ConfigureHostIdentity)
                    .AddEntityFrameworkStores<HostDbContext>()
                    .AddDefaultTokenProviders();

                // the forum reads and writes the users and roles of the host through its own models
                builder.Services.AddScoped<IUserStore<AspNetUsers>, ForumUserStoreAdapter>();
                builder.Services.AddScoped<IRoleStore<AspNetRoles>, ForumRoleStoreAdapter>();

                break;
        }

        if (mode != HostIdentityMode.Forum)
        {
            builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/host/login");
        }

        builder.Services.AddRazorPages(options => options.Conventions.AddPageRoute("/SiteMap", "Sitemap.xml"));

        builder.Services.AddYafCore(builder.Configuration, "/");

        var app = builder.Build();

        if (mode == HostIdentityMode.HostWithOwnTypes)
        {
            using var scope = app.Services.CreateScope();

            await scope.ServiceProvider.GetRequiredService<HostDbContext>().Database.EnsureCreatedAsync();
        }

        app.UseDeveloperExceptionPage();

        app.RegisterAutofac();

        app.UseYafCore(BoardContext.Current.ServiceLocator);

        app.MapRazorPages();

        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapControllers();

        app.MapYafHubs();

        if (mode == HostIdentityMode.HostWithOwnTypes)
        {
            MapHostEndpoints(
                app,
                (name, email) => new HostUser { UserName = name, Email = email, EmailConfirmed = true, IsApproved = true },
                name => new HostRole { Name = name });
        }
        else
        {
            MapHostEndpoints(
                app,
                (name, email) => new AspNetUsers { UserName = name, Email = email, EmailConfirmed = true, IsApproved = true },
                name => new AspNetRoles(name));
        }

        await app.StartAsync();

        return new HostApplication(app, contentRoot);
    }

    /// <summary>
    /// Creates a client that keeps its cookies, like a browser.
    /// </summary>
    public HttpClient CreateBrowser()
    {
        var client = new HttpClient(new CookieHandler(this.App.GetTestServer().CreateHandler()))
        {
            BaseAddress = new Uri("https://localhost/")
        };

        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36");

        return client;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        // only stopped: disposing the container disposes the database provider factory,
        // which is shared by all applications of the process
        await this.App.StopAsync();

        SQLiteConnection.ClearAllPools();
        SqliteConnection.ClearAllPools();

        try
        {
            Directory.Delete(this.contentRoot, true);
        }
        catch (IOException)
        {
            // the database file can still be locked
        }
    }

    /// <summary>
    /// The pages of the host application on the identity managers of its user and role types:
    /// installing the forum, registering and signing in.
    /// </summary>
    private static void MapHostEndpoints<TUser, TRole>(
        WebApplication app,
        Func<string, string, TUser> newUser,
        Func<string, TRole> newRole)
        where TUser : class
        where TRole : class
    {
        // the path contains "/Install", so the forum does not redirect it to its install wizard
        app.MapPost(
            "/host/Install",
            async (HttpContext httpContext) =>
            {
                // there are no board settings before the board is created
                BoardContext.Current.BoardSettings = new BoardSettings();

                // the identity options of the forum read the board settings
                var users = httpContext.RequestServices.GetRequiredService<UserManager<TUser>>();
                var roles = httpContext.RequestServices.GetRequiredService<RoleManager<TRole>>();

                var installService = BoardContext.Current.Get<InstallService>();

                installService.InitializeDatabase();

                ThrowIfFailed(await users.CreateAsync(newUser(AdminName, "hostadmin@example.com"), Password));
                ThrowIfFailed(await roles.CreateAsync(newRole("Administrators")));
                ThrowIfFailed(await roles.CreateAsync(newRole("Registered Users")));
                ThrowIfFailed(await users.AddToRoleAsync(await users.FindByNameAsync(AdminName), "Administrators"));

                var admin = await BoardContext.Current.Get<AspNetUsersManager>().FindByNameAsync(AdminName);

                await installService.InitializeForumAsync(
                    Guid.NewGuid(),
                    ForumName,
                    "A forum inside of a host application",
                    "en-US",
                    "forum@example.com",
                    "YAFLogo.svg",
                    "https://localhost",
                    admin.UserName,
                    admin.Email,
                    admin.Id);

                return Results.Ok();
            });

        // what the forum knows about the current user
        app.MapGet(
            "/host/whoami",
            () =>
            {
                var context = BoardContext.Current;

                // loads the user of the page
                var forumUser = context.PageUser;

                return Results.Ok(
                    new ForumIdentity(
                        context.MembershipUser?.UserName,
                        forumUser.Name,
                        context.IsGuest,
                        context.IsAdmin,
                        context.Get<IUserStore<AspNetUsers>>().GetType().Name));
            });

        // what the host knows about the current user
        app.MapGet(
            "/host/account",
            async (HttpContext httpContext, UserManager<TUser> users) =>
            {
                var user = await users.GetUserAsync(httpContext.User);

                return Results.Ok(
                    new HostAccount(
                        user == null ? null : await users.GetUserNameAsync(user),
                        httpContext.User.IsInRole("Registered Users")));
            });

        app.MapPost(
            "/host/register",
            async (string name, UserManager<TUser> users) =>
            {
                ThrowIfFailed(await users.CreateAsync(newUser(name, $"{name.ToLowerInvariant()}@example.com"), Password));

                // like the forum page for registering: the roles of the groups for new users, then the forum user
                var user = await BoardContext.Current.Get<AspNetUsersManager>().FindByNameAsync(name);
                var boardId = BoardContext.Current.PageBoardID;
                var rolesHelper = BoardContext.Current.Get<IAspNetRolesHelper>();

                await rolesHelper.SetupUserRolesAsync(boardId, user);

                var forumUserId = await rolesHelper.CreateForumUserAsync(user, name, boardId);

                return forumUserId.HasValue ? Results.Ok(forumUserId) : Results.Problem("No forum user was created.");
            });

        app.MapPost(
            "/host/login",
            async (string name, string password, SignInManager<TUser> signInManager) =>
                (await signInManager.PasswordSignInAsync(name, password ?? Password, false, false)).Succeeded
                    ? Results.Ok()
                    : Results.Unauthorized());
    }

    private static void ThrowIfFailed(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private List<string> ReadHostDatabase(Func<HostDbContext, List<string>> read)
    {
        using var scope = this.App.Services.CreateScope();

        var db = scope.ServiceProvider.GetService<HostDbContext>();

        return db == null ? [] : read(db);
    }

    private static void ConfigureHostIdentity(IdentityOptions options)
    {
        options.Password.RequiredLength = RequiredPasswordLength;
        options.User.RequireUniqueEmail = true;
    }

    /// <summary>
    /// Finds the wwwroot folder of the forum (languages, resources and static files).
    /// </summary>
    private static string FindWebRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var webRoot = Path.Combine(directory.FullName, "YetAnotherForum.NET", "wwwroot");

            if (Directory.Exists(webRoot))
            {
                return webRoot;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("The wwwroot folder of YetAnotherForum.NET was not found.");
    }

    /// <summary>
    /// Stores and sends cookies, the test server handler does not.
    /// </summary>
    private sealed class CookieHandler(HttpMessageHandler innerHandler) : DelegatingHandler(innerHandler)
    {
        private readonly CookieContainer cookies = new();

        async override protected Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var header = this.cookies.GetCookieHeader(request.RequestUri);

            if (header.Length > 0)
            {
                request.Headers.Add("Cookie", header);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.Headers.TryGetValues("Set-Cookie", out var values))
            {
                foreach (var value in values)
                {
                    this.cookies.SetCookies(request.RequestUri, value);
                }
            }

            return response;
        }
    }
}
