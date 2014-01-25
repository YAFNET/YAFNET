using System;
using System.Data.Common;

using YAF.Core.Data.Profiling;
using YAF.Data.MsSql;
using YAF.Types.Interfaces.Data;

namespace YAF.Providers.Utils
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using YAF.Types.Interfaces;

    public class BaseProviderDb
    {
        internal static ConcurrentDictionary<string, string> ProviderConnectionStrings = new ConcurrentDictionary<string, string>();

        protected IDbAccess DbAccess
        {
            get { return _dbAccess.Value; }
        }

        /// <summary>
        ///   The _db access.
        /// </summary>
        private readonly Lazy<IDbAccess> _dbAccess = null;

        protected BaseProviderDb(string connectionStringAppKeyName)
        {
            this._dbAccess = new Lazy<IDbAccess>(() =>
            {
                var access = new MsSqlDbAccess((p) => DbProviderFactories.GetFactory(p), new QueryProfile());
                var old = access.Information.ConnectionString;
                access.Information.ConnectionString = () =>
                {
                    string connStr;
                    return ProviderConnectionStrings.TryGetValue(connectionStringAppKeyName, out connStr) ? connStr : old();
                };

                return access;
            });
        }
    }
}