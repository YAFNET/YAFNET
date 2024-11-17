![YAFLogo](https://raw.githubusercontent.com/YAFNET/YAFNET/master/yafsrc/YetAnotherForum.NET/wwwroot/images/Logos/YAFLogo.svg)

Chat UI for [YAF.NET](https://yetanotherforum.net)

## Prerequisites:
* ASP.NET Core 9.0 (YAF v4.x)

## Usage
1. Install the [YAFNET.UI.Chat](https://www.nuget.org/packages/YAFNET.UI.Chat/) Package via Nuget Browser or the console

[![NuGet](https://img.shields.io/nuget/v/YAFNET.UI.Chat.svg)](https://nuget.org/packages/YAFNET.UI.Chat)

``` cmd
> dotnet add package YAFNET.YAFNET.UI.Chat
```

2. Add the `ChatHub` to the `Startup.cs`
``` csharp
 app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();

            endpoints.MapAreaControllerRoute(
                name: "default",
                areaName:"Forums",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllers();

            // other YAF Hubs
            endpoints.MapHub<NotificationHub>("/NotificationHub");
            endpoints.MapHub<ChatHub>("/ChatHub");

            // YAF Chat Hub
            endpoints.MapHub<AllChatHub>("/AllChatHub");
        });
```

## Community Support Forum

See a real live YAF Forum by visiting the YetAnotherForum.NET community support forum: https://yetanotherforum.net/forums. Also, get your questions answered by the YAF community.

## License

Yet Another Forum.NET is licensed under the Apache 2.0 license. 


### Yet Another Forum Community Support

If you have any questions, please visit the YAF Community Support forum: [https://yetanotherforum.net/forums](https://yetanotherforum.net/forums), or visit the Wiki for More Informations.