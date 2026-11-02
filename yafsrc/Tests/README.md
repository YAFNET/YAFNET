# YAF.NET Tests

This folder contains the automated tests for YAF.NET 4.x. All projects use [NUnit](https://nunit.org/) and target .NET 10.0.

| Project                          | What it tests                                                                      | Needs                                    |
|----------------------------------|------------------------------------------------------------------------------------|------------------------------------------|
| `YAFNET.Tests`                   | Unit and repository tests (services, helpers, data access, migrations)             | Nothing, uses an in-memory SQLite database |
| `YAFNET.Tests.UITests`           | End-to-end browser tests with [Playwright](https://playwright.dev/dotnet/), run against every database provider | Docker, Chromium for Playwright |
| `YAFNET.Tests.MiddlewareTests`   | HTTP level tests (security headers, banned IPs, sitemap)                           | An installed forum database              |
| `YAFNET.Tests.LighthouseTests`   | [Lighthouse](https://developer.chrome.com/docs/lighthouse) audits of the forum pages | An installed forum database            |
| `YAFNET.Tests.Utils`             | Shared helpers: test configuration, Docker Compose fixture, in-process test host   | -                                        |

`YAFNET.Tests` is part of every solution (`YAF.NET-SqlServer.slnx`, `YAF.NET-MySql.slnx`, ...) and is run by the CI build. All other test projects are only in `YAF.NET-Core.slnx`.

All commands below are run from the `yafsrc` folder.

## Unit tests

```bash
dotnet test Tests/YAFNET.Tests/YAFNET.Tests.csproj
```

The tests create their own in-memory SQLite database, no setup is needed.

With code coverage (Cobertura report in `Tests/YAFNET.Tests/TestResults`):

```bash
dotnet test Tests/YAFNET.Tests/YAFNET.Tests.csproj --settings Tests/YAFNET.Tests/coverage.runsettings --collect "XPlat Code Coverage"
```

## UI tests

The UI tests drive a real browser through the forum: they run the installation wizard, register test users and then test the user and admin pages.

### Requirements

- [Docker](https://www.docker.com/) with the Compose plugin (`docker compose`), running
- The [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- PowerShell (`pwsh`). After the build the project runs `playwright install chromium` to download the browser. If that fails, run it manually:

  ```powershell
  pwsh Tests/YAFNET.Tests.UITests/bin/Debug/net10.0/playwright.ps1 install chromium
  ```

- The ports `8001` (forum) and `2525` (test mail server) must be free

### How a run works

By default (`"UseDocker": true`) each test class runs once for every database provider: SQL Server, MySQL, PostgreSQL and SQLite. For each provider the test

1. builds the forum image from the local source and starts the stack with the matching `docker-compose-<Db>-test.yml` file,
2. waits until the forum answers on <http://localhost:8001>,
3. installs the forum with the installation wizard and registers the test users,
4. runs the tests in a headless Chromium browser,
5. stops the stack with `docker compose down -v` (the database is deleted).

A stack is reused by all test classes of the same provider and only replaced when a class for another provider starts. Mails sent by the forum are caught by an in-process SMTP server on port `2525`.

### Running the tests

All providers (this takes a while, every provider builds its own image):

```bash
dotnet test Tests/YAFNET.Tests.UITests/YAFNET.Tests.UITests.csproj
```

Only one or some providers, with the `YAF_TEST_SCENARIOS` environment variable:

```bash
# bash
YAF_TEST_SCENARIOS=Sqlite dotnet test Tests/YAFNET.Tests.UITests/YAFNET.Tests.UITests.csproj
```

```powershell
# PowerShell
$env:YAF_TEST_SCENARIOS = "SqlServer,MySql"
dotnet test Tests/YAFNET.Tests.UITests/YAFNET.Tests.UITests.csproj
```

Valid names are `SqlServer`, `MySql`, `PostgreSQL` and `Sqlite`. SQLite starts the fastest and is the best choice while working on a test.

Only some tests, with an NUnit filter:

```bash
dotnet test Tests/YAFNET.Tests.UITests/YAFNET.Tests.UITests.csproj --filter "FullyQualifiedName~AdminPagesTests"
```

Show the test progress (Docker commands, browser console, failed HTTP requests) while it runs:

```bash
dotnet test Tests/YAFNET.Tests.UITests/YAFNET.Tests.UITests.csproj --logger "console;verbosity=detailed"
```

The tests can also be run from the Visual Studio or Rider test explorer. Set `YAF_TEST_SCENARIOS` as a system or user environment variable and restart the IDE to limit the providers there.

### Configuration

The settings are in `Tests/YAFNET.Tests.UITests/appSettings.json` (section `TestSettings`). Each value can be overridden with an environment variable, e.g. `TestSettings__RecordVideos=false`.

| Setting                                  | Description                                                                   |
|------------------------------------------|-------------------------------------------------------------------------------|
| `UseDocker`                              | `true`: test against the Docker stacks. `false`: see [Without Docker](#without-docker) |
| `NewInstall`                             | Run the installation wizard and register the test users before the tests     |
| `RecordVideos`                           | Record a video of each browser session                                        |
| `TestForumUrl`                           | Forum url, only used without Docker                                           |
| `TestMailPort`                           | Port of the test SMTP server                                                  |
| `AdminUserName` / `AdminPassword`        | Admin account created by the installation wizard                             |
| `TestUserName` / `TestUserPassword`, `TestUserName2` / `TestUser2Password` | Test users registered after the installation |

Other environment variables:

| Variable               | Description                                                                      |
|------------------------|----------------------------------------------------------------------------------|
| `YAF_TEST_SCENARIOS`   | Comma separated list of the providers to test (default: all)                     |
| `YAF_TEST_DOCKER_LOGS` | `1`: print the forum container log before the stack is removed (server exceptions) |
| `COMPOSE_DIR`          | Folder of the compose files, if they are not found automatically                 |

### Results

- **Screenshots** of failed tests: `screenshots/<TestName>.png`
- **Videos** (with `RecordVideos`): `videos/`

Both folders are created in the output folder of the test project (`Tests/YAFNET.Tests.UITests/bin/<Configuration>/net10.0`).

### Without Docker

With `"UseDocker": false` the tests start the forum in-process on `TestForumUrl`, using the configuration of the `YetAnotherForum.NET` project (`appsettings.json`) and its database. Only the provider that matches the configured `BoardConfiguration:ConnectionProviderName` is tested, the other providers are skipped. Set `NewInstall` to `false` if that database is already installed.

### Troubleshooting

- **The forum shows "No data for board ID: 1", or the installation fails right at the start.** A previous run was stopped during the installation and left a half-installed database behind. Remove the stack and its volume, then run again:

  ```bash
  docker compose -f docker-compose-Sqlite-test.yml down -v --remove-orphans
  ```

  Use the compose file of the provider that was running. Don't stop a run while it installs the forum.
- **Port 8001 or 2525 is in use.** Stop the forum or stack that uses it (`docker ps`), e.g. a stack started with one of the non-test compose files.
- **The forum does not come up.** Run with `YAF_TEST_DOCKER_LOGS=1` and the detailed console logger to see the container log. SQL Server needs about 30 seconds until it is ready.
- **Requests fail with HTTP 429.** The test compose files raise the rate limit (`BoardConfiguration__RateLimiterPermitLimit`), because all test requests come from one IP address. Keep that setting when you change the compose files.

### Writing a UI test

- Derive the test class from `DatabaseTestBase` (`Infrastructure/DatabaseTestBase.cs`) and pass the `ComposeScenario` through the constructor. The class then runs once per provider.
- Use `this.Fixture.Context` (Playwright `IBrowserContext`, e.g. `this.Fixture.Context.GotoPageAsync(...)`) or `this.Fixture.Page`, together with `this.TestSettings`.
- Mails sent by the forum can be checked with `this.Fixture.SmtpServer`.
- Tests are grouped in `AdminTests` and `UserTests`, page helpers (installation wizard etc.) are in `Pages`.

To try out local changes in the browser with the same setup as the tests:

```bash
docker compose -f docker-compose-Sqlite-test.yml up --build -d
```

## Middleware and Lighthouse tests

Both projects start the forum in-process on `TestForumUrl` from their `appSettings.json` (default `https://localhost:44308/`), using the configuration and database of the `YetAnotherForum.NET` project. Install the forum on that database first, then run:

```bash
dotnet test Tests/YAFNET.Tests.MiddlewareTests/YAFNET.Tests.MiddlewareTests.csproj
dotnet test Tests/YAFNET.Tests.LighthouseTests/YAFNET.Tests.LighthouseTests.csproj
```
