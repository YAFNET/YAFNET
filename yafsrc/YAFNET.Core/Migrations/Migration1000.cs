﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Migrations;

using ServiceStack.DataAnnotations;

using YAF.Types.Models;

/// <summary>
/// Version 1000 Migrations
/// </summary>
[Description("Remove pm limit columns from Group and Rank table")]
public class Migration1000 : MigrationBase
{
    /// <summary>
    /// Migrations
    /// </summary>
    public override void Up()
    {
        const string pmNotificationColumnName = "PMNotification";
        const string pmLimitColumnName = "PMLimit";

        if (this.Db.ColumnExists<User>(pmNotificationColumnName))
        {
            var constraintName = this.Db.GetConstraint<User>(pmNotificationColumnName);

            if (constraintName.IsSet())
            {
                this.Db.DropConstraint<User>(constraintName);

                this.Db.DropColumn<User>(pmNotificationColumnName);
            }
        }

        if (this.Db.ColumnExists<Rank>(pmLimitColumnName))
        {
            var constraintName = this.Db.GetConstraint<User>(pmLimitColumnName);

            if (constraintName.IsSet())
            {
                this.Db.DropConstraint<Rank>(constraintName);

                this.Db.DropColumn<Rank>(pmLimitColumnName);
            }
        }

        if (this.Db.ColumnExists<Group>(pmLimitColumnName))
        {
            var constraintName = this.Db.GetConstraint<User>(pmLimitColumnName);

            if (!constraintName.IsSet())
            {
                return;
            }

            this.Db.DropConstraint<Group>(constraintName);

            this.Db.DropColumn<Group>(pmLimitColumnName);
        }
    }
}