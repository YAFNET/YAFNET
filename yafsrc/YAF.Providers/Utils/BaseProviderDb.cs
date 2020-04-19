namespace YAF.Providers.Utils
{
    using System;
    using System.Collections.Concurrent;
    using System.Data.Common;

    using YAF.Core.Data.Profiling;
    using YAF.Data.MsSql;
    using YAF.Types.Interfaces.Data;

    public class BaseProviderDb
    {
        internal static ConcurrentDictionary<string, string> ProviderConnectionStrings = new ConcurrentDictionary<string, string>();

        protected IDbAccess DbAccess => this._dbAccess.Value;

        /// <summary>
        ///   The _db access.
        /// </summary>
        private readonly Lazy<IDbAccess> _dbAccess;

        protected BaseProviderDb(string connectionStringAppKeyName)
        {
            this._dbAccess = new Lazy<IDbAccess>(() =>
            {
                var access = new MsSqlDbAccess(DbProviderFactories.GetFactory, new QueryProfile());
                var old = access.Information.ConnectionString;
                access.Information.ConnectionString = () => ProviderConnectionStrings.TryGetValue(connectionStringAppKeyName, out var connStr) ? connStr : old();

                return access;
            });
        }
    }
}