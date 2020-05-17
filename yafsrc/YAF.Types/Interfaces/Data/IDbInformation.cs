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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The DB Information Interface
    /// </summary>
    public interface IDbInformation
    {
        /// <summary>
        /// Gets or sets the DB Connection String
        /// </summary>
        Func<string> ConnectionString { get; set; }

        /// <summary>
        /// Gets the DB Provider Name
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Gets the full text upgrade script.
        /// </summary>
        /// <value>
        /// The full text upgrade script.
        /// </value>
        string FullTextUpgradeScript { get; }

        /// <summary>
        ///     Gets the Install Script List.
        /// </summary>
        IEnumerable<string> InstallScripts { get; }

        /// <summary>
        ///     Gets the Upgrade Script List.
        /// </summary>
        IEnumerable<string> UpgradeScripts { get; }
        
        /// <summary>
        ///     Gets the YAF Provider Upgrade script list
        /// </summary>
        IEnumerable<string> IdentityUpgradeScripts { get; }

        /// <summary>
        /// Gets the DB Connection Parameters.
        /// </summary>
        IDbConnectionParam[] DbConnectionParameters { get; }

        /// <summary>
        /// Builds a connection string.
        /// </summary>
        /// <param name="parameters">The Connection Parameters</param>
        /// <returns>Returns the Connection String</returns>
        string BuildConnectionString(IEnumerable<IDbConnectionParam> parameters);
    }
}