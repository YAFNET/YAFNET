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

## Data volumes

Everything the forum needs to keep is stored in named volumes, so it survives an update (`docker compose pull` and `up -d` recreate the container):

| Volume | Path in the container | Content |
|---|---|---|
| `mssql-data`, `mysql-data`, `postgresql-data`, `sqlite-data` | depends on the database | the database |
| `yaf-uploads` | `/app/wwwroot/uploads` | the uploaded files (attachments) |
| `yaf-images` | `/app/wwwroot/images` | avatars, logos, category, forum and medal images |
| `yaf-backups` | `/app/App_Data/Backups` | the database backups (Admin > Database > Backup) |

A new, empty volume is filled with the files of the image (e.g. the default medals) when it is mounted the first time. After that the volume keeps its content: images that a newer version adds to `wwwroot/images` are not copied into an existing `yaf-images` volume.

With `MultiBoardFolders` every board has its own folders (`wwwroot/<board id>/uploads` and `/images`), add a volume for each of them.

## Real-time updates and more than one container

The real-time updates (new topics and posts without reloading the page) and the notifications use SignalR. A SignalR message only reaches the browsers connected to the same container, so the compose files run one forum container. To run several containers behind a load balancer, a SignalR backplane (e.g. Redis) and sticky sessions are needed, this is not set up. Without them, switch the real-time updates off in the host settings.

This folder contains the Dockerfile of each provider (`yaf-sqlserver`, `yaf-mysql`, `yaf-postgresql`, `yaf-sqlite`) and the database init scripts.
