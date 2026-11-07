# Building YAF.NET

The full build instructions for YAF.NET v4 are in the wiki:

- [Build with the command line](https://github.com/YAFNET/YAFNET/wiki/Build-with-Command-Line)
- [Build with Visual Studio](https://github.com/YAFNET/YAFNET/wiki/Build-with-Visual-Studio)

## Quick start

Prerequisites: the [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and [Node.js](https://nodejs.org/).

```bash
cd yafsrc/YetAnotherForum.NET
npm install
npm run build                           # compile the JS and CSS files (build.js)

cd ..
dotnet build YAF.NET-SqlServer.slnx     # or YAF.NET-MySql / -PostgreSQL / -Sqlite
```

How to run the unit and UI tests is described in the [tests README](yafsrc/Tests/README.md).
