namespace ServiceStack.OrmLite.Sqlite
{
    using System.Data;
    using System.Data.SQLite;

    public class SqliteOrmLiteDialectProvider : SqliteOrmLiteDialectProviderBase
    {
        public static SqliteOrmLiteDialectProvider Instance = new SqliteOrmLiteDialectProvider();

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        public override IDbDataParameter CreateParam()
        {
            return new SQLiteParameter();
        }
    }
}