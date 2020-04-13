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
    using ServiceStack.OrmLite;

    using YAF.Core.Data;
    using YAF.Core.Events;
    using YAF.Types.Attributes;
    using YAF.Types.Interfaces.Events;

    /// <summary>
    /// The set MS SQL dialect event.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerDependancy, new[] { typeof(IHandleEvent<InitDatabaseProviderEvent>) })]
    public class SetMsSqlDialectEvent : IHandleEvent<InitDatabaseProviderEvent>
    {
        #region Public Properties

        /// <summary>
        ///     Gets the order.
        /// </summary>
        public int Order => 1000;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void Handle(InitDatabaseProviderEvent @event)
        {
            if (@event.ProviderName == MsSqlDbAccess.ProviderTypeName)
            {
                // set the OrmLite dialect provider...
                OrmLiteConfig.DialectProvider = YafSqlServer2012OrmLiteDialectProvider.Instance;
            }
        }

        #endregion
    }
}