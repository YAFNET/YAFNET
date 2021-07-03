/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
namespace YAF.Data.MsSql
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using YAF.Core.Data;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The MS SQL Database access.
    /// </summary>
    public class MsSqlDbAccess : DbAccessBase
    {
        /// <summary>
        /// The provider type name.
        /// </summary>
        public const string ProviderTypeName = "System.Data.SqlClient";

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDbAccess"/> class.
        /// </summary>
        /// <param name="dbProviderFactory">
        /// The database provider factory.
        /// </param>
        public MsSqlDbAccess([NotNull] Func<string, DbProviderFactory> dbProviderFactory)
            : base(dbProviderFactory, new MsSqlDbInformation())
        {
        }

        #endregion
    }
}