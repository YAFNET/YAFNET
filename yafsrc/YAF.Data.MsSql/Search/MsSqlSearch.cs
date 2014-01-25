/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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