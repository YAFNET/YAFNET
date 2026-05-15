#!/bin/bash
# docker/yaf-mariadb/create-db.sh
#
# Runs inside the mariadb-init container.
# mariadb is already healthy at this point (depends_on condition).
 
set -e
 
SERVER="mariadb"
ROOT_PASSWORD="${MARIADB_ROOT_PASSWORD}"
 
echo ">> Creating database 'yaf' if it does not exist..."
 
mariadb \
  -h "$SERVER" \
  -u root \
  -p"$ROOT_PASSWORD" \
  -e "CREATE DATABASE IF NOT EXISTS \`yaf\`;"
 
echo ">> Database 'yaf' created (or already existed)."
echo ">> Done."
 