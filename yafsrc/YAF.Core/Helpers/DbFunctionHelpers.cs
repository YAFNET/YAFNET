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

namespace YAF.Core.Helpers
{
    using System;
    using System.Linq;

    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Data;

    public static class DbFunctionHelper
    {
        private static T ValidateAndExecute<T>(this IDbFunction dbFunction, string operationName, Func<IDbFunction, T> func)
        {
            if (!dbFunction.DbSpecificFunctions.WhereOperationSupported(operationName).Any())
            {
                throw new InvalidOperationException(@"Database Provider does not support operation ""{0}"".".FormatWith(operationName));
            }

            return func(dbFunction);
        }

        public static int GetDBSize([NotNull] this IDbFunction dbFunction)
        {
            CodeContracts.VerifyNotNull(dbFunction, "dbFunction");

            return dbFunction.ValidateAndExecute<int>("DBSize", f => f.GetScalar<int>(s => s.DBSize()));
        }
    }
}