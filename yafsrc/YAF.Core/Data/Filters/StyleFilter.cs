/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Data.Filters
{
    #region Using

    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Configuration;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    ///     The style filter.
    /// </summary>
    public class StyleFilter : IDbDataFilter, IHaveServiceLocator
    {
        #region Fields

        /// <summary>
        ///     The _styled nick operations.
        /// </summary>
        private readonly string[] _styledNickOperations =
        {
            "active_list", "active_listtopic", "active_listforum", "forum_moderators", "topic_latest",
            "topic_latest", "active_list_user", "admin_list"
        };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleFilter"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service Locator.
        /// </param>
        public StyleFilter(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The _board settings.
        /// </summary>
        public BoardSettings BoardSettings => this.Get<BoardSettings>();

        /// <summary>
        /// Gets or sets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        ///     Gets the sort order.
        /// </summary>
        public int SortOrder => 100;

        /// <summary>
        ///     The _style transform.
        /// </summary>
        public IStyleTransform StyleTransform => this.Get<IStyleTransform>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The is supported operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSupportedOperation(string operationName)
        {
            return this._styledNickOperations.Contains(operationName.ToLower());
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="dbfunctionType">
        /// The dbfunction type.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public void Run(DBFunctionType dbfunctionType, string operationName, IEnumerable<KeyValuePair<string, object>> parameters, object data)
        {
            if (!this.ServiceLocator.IsBoardContext() || !this._styledNickOperations.Contains(operationName.ToLower()) || dbfunctionType != DBFunctionType.DataTable)
            {
                return;
            }

            if (!this.BoardSettings.UseStyledNicks)
            {
                return;
            }

            var dataTable = (DataTable)data;
            this.StyleTransform.DecodeStyleByTable(dataTable);
        }

        #endregion
    }
}