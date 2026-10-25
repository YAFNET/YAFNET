# Running YAF.NET with Docker

Prebuilt images are published on Docker Hub as [`yafnet/yafnet`](https://hub.docker.com/r/yafnet/yafnet), one tag per database provider. A Docker Compose file for each provider starts the forum together with its database.

| Database             | Compose file                     | Image tag                         |
|----------------------|----------------------------------|-----------------------------------|
| Microsoft SQL Server | `docker-compose-SqlServer.yml`   | `yafnet/yafnet:latest-sqlserver`  |
| MySQL / MariaDB      | `docker-compose-MySql.yml`       | `yafnet/yafnet:latest-mysql`      |
| PostgreSQL           | `docker-compose-PostgreSQL.yml`  | `yafnet/yafnet:latest-postgresql` |
| SQLite               | `docker-compose-Sqlite.yml`      | `yafnet/yafnet:latest-sqlite`     |

## Requirements

- Docker with the Compose plugin (`docker compose`, Compose v2)
- Run all commands from the `yafsrc` folder. The compose files mount the database init scripts from `./docker/...`, so they only work from there.

## Quick start

1. Open the compose file for your database and change the default password `MyPass@word` (see [Passwords](#passwords)).
2. Start the stack:

   ```bash
   docker compose -f docker-compose-SqlServer.yml up -d
   ```

3. Open <http://localhost:8001> in your browser. On the first start you are redirected to the installation wizard, which creates the database tables and the admin account.

## What the stack contains

Taking the SQL Server file as an example:

| Service           | Purpose                                                                   | Host port           |
|-------------------|---------------------------------------------------------------------------|---------------------|
| `yafnet`          | The forum, listening on port 89 inside the container                      | `8001`              |
| `sql-server-db`   | The database server                                                       | `8002`              |
| `sql-server-init` | Runs once: creates the database `yaf` if it does not exist, then exits    | -                   |

The forum waits until the database server is healthy and the init container has finished. SQL Server takes about 30 seconds until it is ready, so the forum is not reachable right away.

The MySQL (`mariadb`, `mariadb-init`) and PostgreSQL (`postgres-db`, `postgres-init`) files are built the same way. The SQLite file only contains the `yafnet` service.

To change the host port of the forum, edit the `ports` entry of the `yafnet` service, e.g. `"80:89"`. Port `8002` exposes the database to the host and can be removed if you don't need it.

## Configuration

All settings from `appsettings.json` can be overridden with environment variables in the `environment` section of the `yafnet` service. Nested keys are separated with a double underscore:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=...
  - BoardConfiguration__ConnectionProviderName=Microsoft.Data.SqlClient
  - MailConfiguration__Host=smtp.example.com
  - MailConfiguration__Port=587
  - MailConfiguration__UserName=forum@example.com
  - MailConfiguration__Password=secret
```

A list of all board configuration settings can be found in the [wiki](https://github.com/YAFNET/YAFNET/wiki/BoardConfiguration-Settings).

### Passwords

The default password `MyPass@word` appears in several places of each compose file. Change all of them to the same value:

- **SQL Server:** `MSSQL_SA_PASSWORD` of `sql-server-db` and `sql-server-init`, the `healthcheck` command of `sql-server-db`, and the connection string of `yafnet`
- **MySQL:** `MARIADB_ROOT_PASSWORD` of `mariadb` and `mariadb-init`, and the connection string of `yafnet`
- **PostgreSQL:** `POSTGRES_PASSWORD` of `postgres-db`, `PGPASSWORD` of `postgres-init`, and the connection string of `yafnet`

The database password is only applied when the database volume is created for the first time. To change it later, change it in the database itself, or remove the volume (this deletes all data).

### Mail

The forum needs a working SMTP server for registration mails, password resets and notifications. The compose files contain the mail settings as commented-out examples: uncomment them and set `MailConfiguration__Host` and `MailConfiguration__Port` (and `UserName` / `Password` if the server requires authentication) to your mail server.

## Data

The database is stored in a named volume (`mssql-data`, `mysql-data`, `postgresql-data` or `sqlite-data`), so it survives `docker compose down` and container updates. With SQLite the database file is `/data/yafnet.db` in the volume `sqlite-data`.

`docker compose down -v` **removes the volume and deletes all forum data.**

## Common commands

```bash
# show the logs of the forum
docker compose -f docker-compose-SqlServer.yml logs -f yafnet

# stop the stack (data is kept)
docker compose -f docker-compose-SqlServer.yml down

# update to the latest image
docker compose -f docker-compose-SqlServer.yml pull
docker compose -f docker-compose-SqlServer.yml up -d
```

## Building the image yourself

Each provider has its own Dockerfile in this folder (`docker/yaf-sqlserver`, `docker/yaf-mysql`, `docker/yaf-postgresql`, `docker/yaf-sqlite`). The build context is the `yafsrc` folder:

```bash
docker build -f docker/yaf-sqlserver/Dockerfile -t yafnet:sqlserver .
```

To use your own image, replace the `image` of the `yafnet` service in the compose file with your tag.

## Test compose files

The `docker-compose-*-test.yml` files are used by the UI tests (`DockerComposeFixture`). They build the image from the local source instead of pulling it, send mails to a test SMTP server on the host (`host.docker.internal:2525`) and raise the rate limit, because all test requests come from one IP address. They can also be used to try out local changes:

```bash
docker compose -f docker-compose-SqlServer-test.yml up --build -d
```

Don't use them for a real forum.
