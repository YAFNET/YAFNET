/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Core.Services.Migrations
{
    using ServiceStack.OrmLite;

    using System.Data;

    using YAF.Core.Context;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// Version 81 Migrations
    /// </summary>
    public class V81_Migration : IRepositoryMigration, IHaveServiceLocator
    {
        /// <summary>
        /// Migrate Repositories (Database).
        /// </summary>
        /// <param name="dbAccess">
        /// The Database access.
        /// </param>
        public void MigrateDatabase(IDbAccess dbAccess)
        {
            dbAccess.Execute(
                dbCommand =>
                    {
                        this.UpgradeTable(this.GetRepository<ProfileDefinition>(), dbAccess, dbCommand);

                        this.UpgradeTable(this.GetRepository<User>(), dbAccess, dbCommand);

                        ///////////////////////////////////////////////////////////

                        return true;
                    });
        }

        private void UpgradeTable(IRepository<User> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnExists<User>("Password"))
            {
                dbCommand.Connection.DropColumn<User>("Password");
            }
        }

        private void UpgradeTable(IRepository<ProfileDefinition> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<ProfileDefinition>(x => x.ShowOnRegisterPage))
            {
                dbCommand.Connection.AddColumn<ProfileDefinition>(x => x.ShowOnRegisterPage);
            }
        }

        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;
    }
}