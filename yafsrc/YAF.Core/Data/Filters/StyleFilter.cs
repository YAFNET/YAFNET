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
namespace YAF.Core.Data.Filters
{
    #region Using

    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Classes;
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
        private readonly string[] _styledNickOperations = new[]
                                                              {
                                                                  "active_list", 
                                                                  "active_listtopic", 
                                                                  "active_listforum", 
                                                                  "forum_moderators", 
                                                                  "topic_latest", 
                                                                  "shoutbox_getmessages", 
                                                                  "topic_latest", 
                                                                  "active_list_user", 
                                                                  "admin_list"
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
        public YafBoardSettings BoardSettings
        {
            get
            {
                return this.Get<YafBoardSettings>();
            }
        }

        /// <summary>
        /// Gets or sets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        ///     Gets the sort order.
        /// </summary>
        public int SortOrder
        {
            get
            {
                return 100;
            }
        }

        /// <summary>
        ///     The _style transform.
        /// </summary>
        public IStyleTransform StyleTransform
        {
            get
            {
                return this.Get<IStyleTransform>();
            }
        }

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
        public void Run(DbFunctionType dbfunctionType, string operationName, IEnumerable<KeyValuePair<string, object>> parameters, object data)
        {
            if (!this.ServiceLocator.IsYafContext() || !this._styledNickOperations.Contains(operationName.ToLower()) || dbfunctionType != DbFunctionType.DataTable)
            {
                return;
            }

            bool colorOnly = false;

            if (!this.BoardSettings.UseStyledNicks)
            {
                return;
            }

            var dataTable = (DataTable)data;
            this.StyleTransform.DecodeStyleByTable(dataTable, colorOnly);
        }

        #endregion
    }
}