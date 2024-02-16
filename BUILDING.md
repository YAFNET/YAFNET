# Building YAF.NET

Building the YAF.NET v4.x.x Source with the command line...

# Command-Line Build

## Pre-Requisites

You will need the following software installed:

1. [.NET 8.0 SDK or higher](https://dotnet.microsoft.com/download/visual-studio-sdks)
2. [Node.js](https://nodejs.org/en/download/current)

## Execution

1. Install Node Packages
from inside the ..\yafsrc\YetAnotherForum.NET folder install the node packages

```
> npm install
```

2. Compile the *JS* and *CSS* files

```
> grunt
```

3. Build the YAF.NET Source from the ..\yafsrc folder

for Ms SQL Server
```
> dotnet build YAF.NET-SqlServer.sln
```

for MySQL
```
> dotnet build YAF.NET-MySql.sln
```

for PostgreSQL
```
> dotnet build YAF.NET-PostgreSQL.sln
```

for SQLite
```
> dotnet build YAF.NET-Sqlite.sln
```

# Visual Studio Build

## Pre-Requisites

1. Visual Studio 2022 or higher
2. [.NET 8.0 SDK or higher](https://dotnet.microsoft.com/download/visual-studio-sdks)
3. [Node.js](https://nodejs.org/en/download/current)
4. [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner)

## Execution

1. Open `YAF.NET-SqlServer.sln` in Visual Studio for the Ms Sql server Version (Or the `YAF.NET-MySql.sln`, `YAF.NET-PostgreSQL.sln`, `YAF.NET-Sqlite.sln` depending on which database you want to use).
2. Install the node packages from the Task Runner Explorer

![taskrunner](https://github.com/YAFNET/YAFNET/assets/722575/ee732d52-4440-4973-94cc-6b985f51c4cb)

3. Run the Default Grunt Task to Compile the *JS* and *CSS* Files

![taskrunner2](https://github.com/YAFNET/YAFNET/assets/722575/a67bd6da-f17f-4a09-a721-c5658f591992)

4. Build the entire solution, or run the Web Project
