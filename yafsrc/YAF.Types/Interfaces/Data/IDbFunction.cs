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

namespace YAF.Types.Interfaces.Data
{
    #region Using

    using System.Collections.Generic;
    using System.Data;

    #endregion

    /// <summary>
    ///     The DB function interface.
    /// </summary>
    public interface IDbFunction
    {
        #region Public Properties

        /// <summary>
        ///     Gets GetData.
        /// </summary>
        dynamic GetData { get; }

        /// <summary>
        ///     Gets GetDataSet.
        /// </summary>
        dynamic GetDataSet { get; }

        /// <summary>
        ///     Gets Query.
        /// </summary>
        dynamic Query { get; }

        /// <summary>
        ///     Gets Scalar.
        /// </summary>
        dynamic Scalar { get; }

        /// <summary>
        /// Gets all the Db Specific Functions
        /// </summary>
        IEnumerable<IDbSpecificFunction> DbSpecificFunctions { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create session.
        /// </summary>
        /// <param name="isolationLevel"> </param>
        /// <returns>
        /// The <see cref="IDbFunctionSession"/>.
        /// </returns>
        IDbFunctionSession CreateSession(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        #endregion
    }
}