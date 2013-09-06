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

namespace YAF.Data.MsSql.Search
{
    using System.Collections.Generic;

    using YAF.Types.Attributes;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    [ExportService(ServiceLifetimeScope.OwnedByContainer, new[] { typeof(ISearch) })]
    public class MsSqlSearch : ISearch
    {
        #region Fields

        private readonly IDbFunction _dbFunction;

        #endregion

        #region Constructors and Destructors

        public MsSqlSearch(IDbFunction dbFunction)
        {
            this._dbFunction = dbFunction;
        }

        #endregion

        #region Public Methods and Operators

        public IEnumerable<SearchResult> Execute(ISearchContext context)
        {
            using (var session = this._dbFunction.CreateSession())
            {
                return session.GetTyped<SearchResult>(r => r.executesearch(context));
            }
        }

        #endregion
    }
}