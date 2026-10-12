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
/// Version 1005 Migrations
/// </summary>
[Description("Increase the push endpoint of the device subscriptions and the tag name to unlimited text.")]
public class Migration1005 : MigrationBase
{
    /// <summary>
    /// Migrations
    /// </summary>
    public override void Up()
    {
        // push services can send endpoints longer than the column, tags are entered by the users
        // (SQLite has no lengths, PostgreSQL created the columns as unlimited text)
        switch (OrmLiteConfig.DialectProvider.SQLServerName())
        {
            case "MySQL":
                // created with 255 characters (the length can't be read: the lookup filters by the schema
                // "dbo", MySQL has no such schema), the change to unlimited text can be repeated
                this.Db.AlterColumn<DeviceSubscription>(x => x.EndPoint);
                this.Db.AlterColumn<Tag>(x => x.TagName);
                break;
            case "Microsoft SQL Server":
                // created with 4000 characters, unlimited text has the length -1
                if (this.Db.ColumnMaxLength<DeviceSubscription>(x => x.EndPoint) > 0)
                {
                    this.Db.AlterColumn<DeviceSubscription>(x => x.EndPoint);
                }

                if (this.Db.ColumnMaxLength<Tag>(x => x.TagName) > 0)
                {
                    this.Db.AlterColumn<Tag>(x => x.TagName);
                }

                break;
        }
    }
}
