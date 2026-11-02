# Running YAF.NET with Docker

The full Docker guide is in the wiki: [Running YAF.NET with Docker](https://github.com/YAFNET/YAFNET/wiki/Docker). It covers configuration via environment variables, passwords, mail, data volumes, reverse proxies and the test compose files.

## Quick start

Prebuilt images are published on Docker Hub as [`yafnet/yafnet`](https://hub.docker.com/r/yafnet/yafnet). Run all commands from the `yafsrc` folder, because the compose files mount the database init scripts from `./docker/...`.

1. Change the default password `MyPass@word` in the compose file for your database.
2. Start the stack:

   ```bash
   docker compose -f docker-compose-SqlServer.yml up -d
   ```

3. Open <http://localhost:8001> and follow the installation wizard.

| Database             | Compose file                     |
|----------------------|----------------------------------|
| Microsoft SQL Server | `docker-compose-SqlServer.yml`   |
| MySQL / MariaDB      | `docker-compose-MySql.yml`       |
| PostgreSQL           | `docker-compose-PostgreSQL.yml`  |
| SQLite               | `docker-compose-Sqlite.yml`      |

This folder contains the Dockerfile of each provider (`yaf-sqlserver`, `yaf-mysql`, `yaf-postgresql`, `yaf-sqlite`) and the database init scripts.
