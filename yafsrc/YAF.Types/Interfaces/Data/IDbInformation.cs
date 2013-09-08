using System;
using System.Collections.Generic;

namespace YAF.Types.Interfaces.Data
{
    public interface IDbInformation
    {
        /// <summary>
        /// Db Connection String
        /// </summary>
        Func<string> ConnectionString { get; set; }

        /// <summary>
        /// Db Provider Name
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        ///     Gets FullTextScript.
        /// </summary>
        string FullTextScript { get; }

        /// <summary>
        ///     Gets Scripts.
        /// </summary>
        IEnumerable<string> Scripts { get; }

        /// <summary>
        ///     Gets DbConnectionParameters.
        /// </summary>
        IEnumerable<IDbConnectionParam> DbConnectionParameters { get; }
    }
}