/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2024 Ingo Herbote
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
/// Version 89 Migrations
/// </summary>
[Description("Remove Nntp Tables and Pm Limit Column from Group and Rank")]
public class Migration91 : MigrationBase
{
    /// <summary>
    /// Migrations
    /// </summary>
    public override void Up()
    {
        this.Db.DropTable<NntpTopic>();
        this.Db.DropTable<NntpForum>();
        this.Db.DropTable<NntpServer>();

        if (this.Db.ColumnExists<User>("PMNotification"))
        {
            this.Db.DropConstraint<User>(
                $"DF_{Config.DatabaseObjectQualifier}{nameof(User)}_PMNotification");

            this.Db.DropColumn<User>("PMNotification");
        }

        if (this.Db.ColumnExists<Rank>("PMLimit"))
        {
            this.Db.DropConstraint<Rank>(
                $"DF_{Config.DatabaseObjectQualifier}{nameof(Rank)}_PMLimit");

            this.Db.DropColumn<Rank>("PMLimit");
        }

        if (this.Db.ColumnExists<Group>("PMLimit"))
        {
            this.Db.DropConstraint<Group>(
                $"DF_{Config.DatabaseObjectQualifier}{nameof(Group)}_PMLimit");

            this.Db.DropColumn<Group>("PMLimit");
        }
    }
}