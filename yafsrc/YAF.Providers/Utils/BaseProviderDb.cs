using System;
using System.Data.Common;
using YAF.Core;
using YAF.Core.Data.Profiling;
using YAF.Data.MsSql;
using YAF.Providers.Membership;
using YAF.Types.Interfaces.Data;

namespace YAF.Providers.Utils
{
    public class BaseProviderDb
    {
        protected IDbAccessV2 DbAccess
        {
            get { return _dbAccess.Value; }
        }

        /// <summary>
        ///   The _db access.
        /// </summary>
        private readonly Lazy<IDbAccessV2> _dbAccess = null;

        protected BaseProviderDb(string connectionStringAppKeyName)
        {
            this._dbAccess = new Lazy<IDbAccessV2>(() =>
            {
                var access = new MsSqlDbAccess((p) => DbProviderFactories.GetFactory(p), new QueryProfile());
                var old = access.Information.ConnectionString;
                access.Information.ConnectionString = () =>
                {
                    if (YafContext.Application[connectionStringAppKeyName] != null)
                    {
                        return YafContext.Application[connectionStringAppKeyName] as string;
                    }

                    return old();
                };

                return access;
            });
        }
    }
}