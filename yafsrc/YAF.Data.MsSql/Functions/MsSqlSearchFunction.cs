/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

namespace YAF.Data.MsSql.Functions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using YAF.Data.MsSql.Search;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Interfaces.Data;

    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class MsSqlSearchFunction : BaseMsSqlFunction
    {
        public MsSqlSearchFunction([NotNull] IDbAccess dbAccess)
            : base(dbAccess)
        {
        }

        public override int SortOrder
        {
            get
            {
                return 1000;
            }
        }

        public override bool IsSupportedOperation(string operationName)
        {
            return operationName.Equals("executesearch");
        }

        protected override bool RunOperation(
            SqlConnection sqlConnection,
            IDbTransaction dbTransaction,
            DbFunctionType dbfunctionType,
            string operationName,
            IEnumerable<KeyValuePair<string, object>> parameters,
            out object result)
        {
            var context = parameters.First().Value as ISearchContext;

            if (context != null)
            {
                var sql = new SearchBuilder().BuildSearchSql(context);

                using (var cmd = this.DbAccess.GetCommand(sql, CommandType.Text))
                {
                    result = this.DbAccess.GetReader(cmd, dbTransaction);
                    return true;
                }
            }

            // failure...
            result = null;
            return false;
        }
    }
}