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
namespace YAF.Core.Data
{
    #region Using

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    /// The basic repository.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class BasicRepository<T> : IRepository<T>
        where T : IEntity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicRepository{T}"/> class.
        /// </summary>
        /// <param name="dbFunction">
        /// The db function. 
        /// </param>
        /// <param name="dbAccess">
        /// The db Access.
        /// </param>
        /// <param name="raiseEvent">
        /// The raise Event. 
        /// </param>
        /// <param name="haveBoardId">
        /// The have Board Id. 
        /// </param>
        public BasicRepository(IDbFunction dbFunction, IDbAccess dbAccess, IRaiseEvent raiseEvent, IHaveBoardID haveBoardId)
        {
            this.DbFunction = dbFunction;
            this.DbAccess = dbAccess;
            this.DbEvent = raiseEvent;
            this.BoardID = haveBoardId.BoardID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the board id.
        /// </summary>
        public int BoardID { get; set; }

        /// <summary>
        /// Gets the db access.
        /// </summary>
        public IDbAccess DbAccess { get; private set; }

        /// <summary>
        ///     Gets the db event.
        /// </summary>
        public IRaiseEvent DbEvent { get; private set; }

        /// <summary>
        ///     Gets the db function.
        /// </summary>
        public IDbFunction DbFunction { get; private set; }

        #endregion
    }
}