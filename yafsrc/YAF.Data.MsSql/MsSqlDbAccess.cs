/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Data.MsSql
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using YAF.Classes;
    using YAF.Core.Data;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The i db setup.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerDependancy, "System.Data.SqlClient", new Type[] { typeof(IDbAccessV2) })]
    public class MsSqlDbAccess : DbAccessBase
    {
        #region Constants and Fields

        /// <summary>
        /// Lists the UI parameters...
        /// </summary>
        protected DbConnectionParam[] DbParameters = new[]
            {
                new DbConnectionParam(0, "Password", string.Empty, true), new DbConnectionParam(1, "Data Source", "(local)", true), 
                new DbConnectionParam(2, "Initial Catalog", string.Empty, true), new DbConnectionParam(11, "Use Integrated Security", "true", true)
            };

        /// <summary>
        /// The _script list.
        /// </summary>
        private static readonly string[] _scriptList =
            {
                "mssql/tables.sql", "mssql/indexes.sql", "mssql/views.sql", "mssql/constraints.sql",
                "mssql/triggers.sql", "mssql/functions.sql", "mssql/procedures.sql",
                "mssql/providers/tables.sql", "mssql/providers/indexes.sql", "mssql/providers/procedures.sql"
            };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDbAccess"/> class.
        /// </summary>
        /// <param name="dbProviderFactory">
        /// The db provider factory. 
        /// </param>
        public MsSqlDbAccess([NotNull] Func<string, DbProviderFactory> dbProviderFactory)
            : base(dbProviderFactory, "System.Data.SqlClient", Config.ConnectionString)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether PanelGetStats.
        /// </summary>
        public static bool PanelGetStats
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether PanelRecoveryMode.
        /// </summary>
        public static bool PanelRecoveryMode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether PanelReindex.
        /// </summary>
        public static bool PanelReindex
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether PanelShrink.
        /// </summary>
        public static bool PanelShrink
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether PasswordPlaceholderVisible.
        /// </summary>
        public static bool PasswordPlaceholderVisible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets DbConnectionParameters.
        /// </summary>
        public override IEnumerable<IDbConnectionParam> DbConnectionParameters
        {
            get
            {
                return this.DbParameters;
            }
        }

        /// <summary>
        /// Gets or sets FullTextScript.
        /// </summary>
        [NotNull]
        public override string FullTextScript
        {
            get
            {
                return "mssql/fulltext.sql";
            }
        }

        /// <summary>
        /// Gets Scripts.
        /// </summary>
        public override IEnumerable<string> Scripts
        {
            get
            {
                return _scriptList;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The map parameters.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="keyValueParams">
        /// The key value params.
        /// </param>
        protected override void MapParameters(IDbCommand cmd, IEnumerable<KeyValuePair<string, object>> keyValueParams)
        {
            // convert to list so there is no chance of multiple iterations.
            var paramList = keyValueParams.ToList();

            // handle positional stored procedure parameter call
            if (cmd.CommandType == CommandType.StoredProcedure && paramList.Any() && !paramList.All(x => x.Key.IsSet()))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format(
                    "EXEC {0} {1}",
                    cmd.CommandText,
                    Enumerable.Range(0, paramList.Count()).Select(x => string.Format("@{0}", x)).ToDelimitedString(","));

                // add params without "keys" as they need to be index (0, 1, 2, 3)...
                base.MapParameters(cmd, paramList.Select(x => new KeyValuePair<string, object>(null, x.Value)));
            }
            else
            {
                // map named parameters...
                base.MapParameters(cmd, paramList);
            }
        }

        #endregion
    }
}