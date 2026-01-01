/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System.Threading.Tasks;

namespace YAF.Core.Model;

using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
///     The bb code repository extensions.
/// </summary>
public static class BBCodeRepositoryExtensions
{
    /// <param name="repository">
    /// The repository.
    /// </param>
    extension(IRepository<BBCode> repository)
    {
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="codeId">
        /// The b b code id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="onClickJs">
        /// The on click JS.
        /// </param>
        /// <param name="displayJs">
        /// The display JS.
        /// </param>
        /// <param name="editJs">
        /// The edit JS.
        /// </param>
        /// <param name="displayCss">
        /// The display CSS.
        /// </param>
        /// <param name="searchRegEx">
        /// The search Regular Expression.
        /// </param>
        /// <param name="replaceRegEx">
        /// The replace Regular Expression.
        /// </param>
        /// <param name="variables">
        /// The variables.
        /// </param>
        /// <param name="useModule">
        /// The use module.
        /// </param>
        /// <param name="useToolbar">
        /// Thee use Toolbar
        /// </param>
        /// <param name="moduleClass">
        /// The module class.
        /// </param>
        /// <param name="execOrder">
        /// The exec order.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        public Task SaveAsync(int? codeId,
            string name,
            string description,
            string onClickJs,
            string displayJs,
            string editJs,
            string displayCss,
            string searchRegEx,
            string replaceRegEx,
            string variables,
            bool? useModule,
            bool? useToolbar,
            string moduleClass,
            int execOrder,
            int? boardId = null)
        {
            return repository.UpsertAsync(
                new BBCode
                {
                    BoardID = boardId ?? repository.BoardID,
                    ID = codeId ?? 0,
                    Name = name,
                    Description = description,
                    OnClickJS = onClickJs,
                    DisplayJS = displayJs,
                    EditJS = editJs,
                    DisplayCSS = displayCss,
                    SearchRegex = searchRegEx,
                    ReplaceRegex = replaceRegEx,
                    Variables = variables,
                    UseModule = useModule,
                    UseToolbar = useToolbar,
                    ModuleClass = moduleClass,
                    ExecOrder = execOrder
                });
        }

        /// <summary>
        /// Get All BBCodes Paged by Board Id
        /// </summary>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        public List<BBCode> ListPaged(int boardId,
            int? pageIndex = 0,
            int? pageSize = 10000000)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<BBCode>();

            expression.Where(x => x.BoardID == boardId).OrderBy(item => item.Name)
                .Page(pageIndex + 1, pageSize);

            return repository.DbAccess.Execute(db => db.Connection.Select(expression));
        }
    }
}