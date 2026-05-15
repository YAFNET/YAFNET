#!/bin/sh
# docker/yaf-postgresql/create-db.sh
#
# Runs inside the postgres-init container.
# postgres-db is already healthy at this point (depends_on condition).
 
set -e
 
SERVER="postgres-db"
PGPASSWORD="${PGPASSWORD}"
PSQL="psql"
 
echo ">> Creating database 'yaf' if it does not exist..."
 
$PSQL \
  -h "$SERVER" \
  -U postgres \
  -c "SELECT 1 FROM pg_database WHERE datname = 'yaf'" \
  | grep -q 1 \
  && echo ">> Database yaf already exists, skipping." \
  || ($PSQL -h "$SERVER" -U postgres -c "CREATE DATABASE yaf;" \
      && echo ">> Database yaf created.")
 
echo ">> Done."
 