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
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The i db setup.
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
        /// The db provider factory. 
        /// </param>
        /// <param name="profiler">
        /// The profiler.
        /// </param>
        public MsSqlDbAccess([NotNull] Func<string, DbProviderFactory> dbProviderFactory, IProfileQuery profiler)
            : base(dbProviderFactory, profiler, new MsSqlDbInformation())
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The map parameters.
        /// </summary>
        /// <param name="cmd">
        /// The cmd. 
        /// </param>
        /// <param name="keyValueParams">
        /// The key value params. 
        /// </param>
        protected override void MapParameters(IDbCommand cmd, IEnumerable<KeyValuePair<string, object>> keyValueParams)
        {
            // convert to list so there is no chance of multiple iterations.
            var paramList = keyValueParams.ToList();

            // handle positional stored procedure parameter call
            if (cmd.CommandType == CommandType.StoredProcedure && paramList.Any() && !paramList.All(x => x.Key.IsSet()))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
                    $"EXEC {cmd.CommandText} {Enumerable.Range(0, paramList.Count).Select(x => $"@{x}").ToDelimitedString(",")}";

                // add params without "keys" as they need to be index (0, 1, 2, 3)...
                base.MapParameters(cmd, paramList.Select(x => new KeyValuePair<string, object>(null, x.Value)));
            }
            else
            {
                // map named parameters...
                base.MapParameters(cmd, paramList);
            }
        }

        #endregion
    }
}