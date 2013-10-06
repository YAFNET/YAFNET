namespace YAF.Data.MsSql.Functions
{
    using System.Data;

    using YAF.Types;
    using YAF.Types.Interfaces.Data;

    public static class MsSqlSpecificFunctions
    {
        /// <summary>
        ///   Gets the database size
        /// </summary>
        /// <returns>intager value for database size</returns>
        public static int DBSize(this IDbAccess dbAccess)
        {
            CodeContracts.VerifyNotNull(dbAccess, "dbAccess");

            using (var cmd = dbAccess.GetCommand("select sum(cast(size as integer))/128 from sysfiles", CommandType.Text))
            {
                return (int)dbAccess.ExecuteScalar(cmd);
            }
        }
    }
}