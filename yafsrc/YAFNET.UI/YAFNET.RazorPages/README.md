![YAFLogo](https://raw.githubusercontent.com/YAFNET/YAFNET/master/yafsrc/YetAnotherForum.NET/wwwroot/images/Logos/YAFLogo.svg)


[![Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Me-ff5e5b?logo=ko-fi&logoColor=white)](https://ko-fi.com/w8tcha)

[YAF.NET](https://yetanotherforum.net) Razor Pages Client Library  

## Prerequisites:
* ASP.NET Core 11.0 (YAF v4.x)

# Quick Guide

## Add the YAF Nuget Packages
1. Install the [YAFNET.RazorPages](https://www.nuget.org/packages/YAFNET.RazorPages/) Package via Nuget Browser or the console

[![NuGet](https://img.shields.io/nuget/v/YAFNET.RazorPages.svg)](https://nuget.org/packages/YAFNET.RazorPages)

``` cmd
> dotnet add package YAFNET.RazorPages
```

2. Depending on which Data Provider you want top use add the package ..

[**YAFNET.Data.SqlServer**](https://www.nuget.org/packages/YAFNET.Data.SqlServer/) Package via Nuget Browser or the console

[![NuGet](https://img.shields.io/nuget/v/YAFNET.Data.SqlServer.svg)](https://nuget.org/packages/YAFNET.Data.SqlServer)

``` cmd
> dotnet add package YAFNET.Data.SqlServer
```

[**YAF.Data.MySQL**](https://www.nuget.org/packages/YAFNET.Data.MySQL/) Package via Nuget Browser or the console

[![NuGet](https://img.shields.io/nuget/v/YAFNET.Data.MySQL.svg)](https://nuget.org/packages/YAFNET.Data.MySQL)

``` cmd
> dotnet add package YAFNET.Data.MySQL
```

[**YAFNET.Data.PostgreSQL**](https://www.nuget.org/packages/YAFNET.Data.PostgreSQL/) Package via Nuget Browser or the console

[![NuGet](https://img.shields.io/nuget/v/YAFNET.Data.PostgreSQL.svg)](https://nuget.org/packages/YAFNET.Data.PostgreSQL)

``` cmd
> dotnet add package YAFNET.Data.PostgreSQL
```

[**YAFNET.Data.Sqlite**](https://www.nuget.org/packages/YAFNET.Data.Sqlite/) Package via Nuget Browser or the console

[![NuGet](https://img.shields.io/nuget/v/YAFNET.Data.Sqlite.svg)](https://nuget.org/packages/YAFNET.Data.Sqlite)

``` cmd
> dotnet add package YAFNET.Data.Sqlite
```

## Getting Started

### Program.cs
In the `Program.cs` add the following.

``` csharp
 var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureYafLogging();

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterYafModules();
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddAreaPageRoute("Forums", "/SiteMap", "/Sitemap.xml");
}).AddYafRazorPages(builder.Environment);

builder.Services.AddYafCore(builder.Configuration, "/Forums");

// only needed for blazor
builder.Services.AddServerSideBlazor();

var app = builder.Build();

....

app.RegisterAutofac();

app.UseAntiXssMiddleware();

app.UseStaticFiles();

app.UseSession();

app.UseYafCore(BoardContext.Current.ServiceLocator, app.Environment);

app.UseRobotsTxt(app.Environment);

app.MapRazorPages();

app.MapAreaControllerRoute(
    name: "default",
    areaName: "Forums",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.MapHub<NotificationHub>("/NotificationHub");
app.MapHub<ChatHub>("/ChatHub");

await app.RunAsync();
```

Now the Forum is ready and you can add a link to your `_Layout.cshtml`

``` csharp
 <li class="nav-item">
     <a class="nav-link" asp-area="Forums" asp-page="/Index">Forums</a>
 </li>
```


### Host application manages the accounts
If the application has its own login and account pages, set `HostManagesAccounts` in the `BoardConfiguration` section of the `appsettings.json`.

``` json
"BoardConfiguration": {
    "HostManagesAccounts": true
}
```

`AddYafCore` then does not add the identity options and the authentication cookie of the forum. The identity options keep their defaults unless you set them. Add the `Identity.Application` cookie before calling `AddYafCore`, for example with `AddIdentity`, because the forum refreshes and ends sign-ins with it. To keep the accounts in your own tables, register your own `IUserStore<AspNetUsers>` and `IRoleStore<AspNetRoles>`; they replace the forum stores.

``` csharp
builder.Services.AddIdentity<AspNetUsers, AspNetRoles>()
    .AddUserStore<MyUserStore>()
    .AddRoleStore<MyRoleStore>();

builder.Services.AddYafCore(builder.Configuration, "/Forums");
```

The login and register links of the forum go to the `Account/Login` and `Account/Register` pages of the forum area (with the `ReturnUrl`), redirect them to the pages of your application.

## Community Support Forum

See a real live YAF Forum by visiting the YetAnotherForum.NET community support forum: https://yetanotherforum.net/forums. Also, get your questions answered by the YAF community.

## License

Yet Another Forum.NET is licensed under the Apache 2.0 license. 


### Yet Another Forum Community Support

If you have any questions, please visit the YAF Community Support forum: [https://yetanotherforum.net/forums](https://yetanotherforum.net/forums), or visit the Wiki for More Informations.