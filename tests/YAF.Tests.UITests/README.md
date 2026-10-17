# YAF.Tests.UITests

Browser tests (Playwright, Chromium) of the YAF.NET 3.x forum, without Docker.

The test run copies the built site (`yafsrc\YetAnotherForum.NET`) into `%TEMP%\yaf-uitests\site`, creates an empty
LocalDB database, starts IIS Express on the copy and installs the forum with the install wizard. Mails are written as
`.eml` files into `%TEMP%\yaf-uitests\mails` (the tests read the verification links from them).

## Requirements

- the site built with `yafsrc\YAF.NET-SqlServer.slnx` (the provider solutions share the `bin` folder of the site,
  build the SqlServer solution last)
- IIS Express (`C:\Program Files\IIS Express\iisexpress.exe`)
- SQL Server Express LocalDB (`(localdb)\MSSQLLocalDB`)
- the Playwright browser: `pwsh tests\YAF.Tests.UITests\bin\net481\playwright.ps1 install chromium`

## Running

```
dotnet test tests\YAF.Tests.UITests
```

## Settings (environment variables)

| Variable | Default | |
| --- | --- | --- |
| `YAF_UITEST_PORT` | `8091` | port of IIS Express |
| `YAF_UITEST_LOCALDB` | `(localdb)\MSSQLLocalDB` | SQL Server instance |
| `YAF_UITEST_DATABASE` | `YafUiTests` | database (dropped and created for every run) |
| `YAF_UITEST_HEADED` | | `1` shows the browser |
| `YAF_UITEST_KEEP` | | `1` keeps the site copy and the database after the run |
| `YAF_UITEST_COVERAGE_SESSION` | | runs IIS Express in this session of `dotnet-coverage` (see below) |

A failed test saves a screenshot in `bin\net481\screenshots`.

## Code coverage of the site

The coverage of the site (pages, `YAF.Web`, `YAF.Core`) is collected with `dotnet-coverage`
(`dotnet tool install -g dotnet-coverage`): start a collector in server mode, run the tests with its session id,
the tests shut the session down (the report is written) before they stop IIS Express.

```
dotnet-coverage collect --server-mode --session-id yafui --output ui.cobertura.xml --output-format cobertura
set YAF_UITEST_COVERAGE_SESSION=yafui
dotnet test tests\YAF.Tests.UITests
```

