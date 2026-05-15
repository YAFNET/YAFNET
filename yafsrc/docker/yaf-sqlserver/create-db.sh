#!/bin/bash
# docker/yaf-sqlserver/create-db.sh
#
# Runs inside the sql-server-init container.
# sql-server-db is already healthy at this point (depends_on condition).

set -e

SERVER="sql-server-db"
SA_PASSWORD="${MSSQL_SA_PASSWORD}"
SQLCMD="/opt/mssql-tools18/bin/sqlcmd"

echo ">> Creating database 'yaf' if it does not exist..."

$SQLCMD \
  -S "$SERVER" \
  -U sa \
  -P "$SA_PASSWORD" \
  -No \
  -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'yaf')
BEGIN
    CREATE DATABASE [yaf];
    PRINT 'Database yaf created.';
END
ELSE
BEGIN
    PRINT 'Database yaf already exists, skipping.';
END
"

echo ">> Done."
