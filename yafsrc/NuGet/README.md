![YAF.NET logo](https://raw.githubusercontent.com/YAFNET/YAFNET/master/yafsrc/YetAnotherForum.NET/wwwroot/images/Logos/YAFLogo.svg)

[![License](https://img.shields.io/github/license/yafnet/yafnet)](https://github.com/YAFNET/YAFNET/blob/master/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/YAFNET.Core.svg)](https://www.nuget.org/packages/YAFNET.Core)
[![Docker pulls](https://img.shields.io/docker/pulls/yafnet/yafnet)](https://hub.docker.com/r/yafnet/yafnet)
[![Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Me-ff5e5b?logo=ko-fi&logoColor=white)](https://ko-fi.com/w8tcha)

**YetAnotherForum.NET** (YAF.NET) is an open source forum for ASP.NET Core. This package is one of the libraries that make up YAF.NET 4.x.

See a live YAF.NET forum in action at the [community support forum](https://yetanotherforum.net/Forums).

## Packages

| Package | Description |
|---------|-------------|
| [YAFNET.RazorPages](https://www.nuget.org/packages/YAFNET.RazorPages) | The forum UI (Razor Pages Class Library). Start here to add YAF.NET to your application. |
| [YAFNET.Core](https://www.nuget.org/packages/YAFNET.Core) | Core library: services, events, mail, search, ... |
| [YAFNET.Web](https://www.nuget.org/packages/YAFNET.Web) | Web helpers, controllers and tag helpers |
| [YAFNET.Types](https://www.nuget.org/packages/YAFNET.Types) | Models, interfaces and constants |
| [YAFNET.Configuration](https://www.nuget.org/packages/YAFNET.Configuration) | Configuration library |
| [YAFNET.UI.Chat](https://www.nuget.org/packages/YAFNET.UI.Chat) | Optional chat UI |
| [YAFNET.Data.SqlServer](https://www.nuget.org/packages/YAFNET.Data.SqlServer) | Data provider for Microsoft SQL Server |
| [YAFNET.Data.MySql](https://www.nuget.org/packages/YAFNET.Data.MySql) | Data provider for MySQL / MariaDB |
| [YAFNET.Data.PostgreSQL](https://www.nuget.org/packages/YAFNET.Data.PostgreSQL) | Data provider for PostgreSQL |
| [YAFNET.Data.Sqlite](https://www.nuget.org/packages/YAFNET.Data.Sqlite) | Data provider for SQLite |

## Getting started

Install the forum UI and one data provider:

```cmd
dotnet add package YAFNET.RazorPages
dotnet add package YAFNET.Data.SqlServer
```

Then register YAF.NET in your `Program.cs` as described in the [YAFNET.RazorPages readme](https://www.nuget.org/packages/YAFNET.RazorPages#readme-body-tab).

Prebuilt Docker images are available as [yafnet/yafnet](https://hub.docker.com/r/yafnet/yafnet), see the [Docker guide](https://github.com/YAFNET/YAFNET/wiki/Docker).

## Documentation

- [New installation](https://github.com/YAFNET/YAFNET/wiki#getting-started) (IIS, Linux with nginx, Docker)
- [Upgrade from YAF.NET 3.x or older](https://github.com/YAFNET/YAFNET/wiki/Upgrade-to-YAF-v4)
- [Troubleshooting](https://github.com/YAFNET/YAFNET/wiki/Troubleshooting)
- [Release notes](https://github.com/YAFNET/YAFNET/blob/master/CHANGES.md)
- [Source code](https://github.com/YAFNET/YAFNET)

## Support

- Ask questions and get help from the community at the [YAF.NET support forum](https://yetanotherforum.net/Forums).
- Report bugs and request features in the [issue tracker](https://github.com/YAFNET/YAFNET/issues).
- Please do **not** report security vulnerabilities in public issues, see the [security policy](https://github.com/YAFNET/YAFNET/blob/master/SECURITY.md).

## License

YAF.NET is licensed under the [Apache License 2.0](https://github.com/YAFNET/YAFNET/blob/master/LICENSE.md).
