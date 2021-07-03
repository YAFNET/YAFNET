namespace ServiceStack.OrmLite
{
    using ServiceStack.OrmLite.Sqlite;

    public static class SqliteDialect
    {
        public static IOrmLiteDialectProvider Provider => SqliteOrmLiteDialectProvider.Instance;
        public static SqliteOrmLiteDialectProvider Instance => SqliteOrmLiteDialectProvider.Instance;
    }
}