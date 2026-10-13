/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
        this.DropColumn<User>("PMNotification");
        this.DropColumn<Rank>("PMLimit");
        this.DropColumn<Group>("PMLimit");
    }

    /// <summary>
    /// Drops a column if it exists, with its default constraint (SQL Server, other databases have none).
    /// </summary>
    /// <typeparam name="T">The table model.</typeparam>
    /// <param name="columnName">The column name.</param>
    private void DropColumn<T>(string columnName)
    {
        if (!this.Db.ColumnExists<T>(columnName))
        {
            return;
        }

        var constraintName = this.Db.GetConstraint<T>(columnName);

        if (constraintName.IsSet())
        {
            this.Db.DropConstraint<T>(constraintName);
        }

        this.Db.DropColumn<T>(columnName);
    }
}