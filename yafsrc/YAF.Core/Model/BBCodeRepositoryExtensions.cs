/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Core.Model
{
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The bb code repository extensions.
    /// </summary>
    public static class BBCodeRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
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
        /// The on click js. 
        /// </param>
        /// <param name="displayJs">
        /// The display js. 
        /// </param>
        /// <param name="editJs">
        /// The edit js. 
        /// </param>
        /// <param name="displayCss">
        /// The display css. 
        /// </param>
        /// <param name="searchRegEx">
        /// The search reg ex. 
        /// </param>
        /// <param name="replaceRegEx">
        /// The replace reg ex. 
        /// </param>
        /// <param name="variables">
        /// The variables. 
        /// </param>
        /// <param name="useModule">
        /// The use module. 
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
        public static void Save(
            this IRepository<BBCode> repository, 
            int? codeId, 
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
            string moduleClass, 
            int execOrder, 
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.Upsert(
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
                        ModuleClass = moduleClass,
                        ExecOrder = execOrder
                    });
        }

        #endregion
    }
}