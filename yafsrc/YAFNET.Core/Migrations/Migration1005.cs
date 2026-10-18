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

using System.Data;

using ServiceStack.DataAnnotations;

using YAF.Types.Models;

/// <summary>
/// Version 1005 Migrations
/// </summary>
[Description("Increase the push endpoint of the device subscriptions, the tag name and the report text to unlimited text, hide the existing forums from the users without read access.")]
public class Migration1005 : MigrationBase
{
    /// <summary>
    /// Migrations
    /// </summary>
    public override void Up()
    {
        HideExistingForums(this.Db);

        if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
        {
            return;
        }

        // push services can send endpoints longer than the column, tags are entered by the users:
        // MySQL created the columns with 255 characters, SQL Server with 4000
        // (unlimited text has no length, a negative one or a larger one)
        if (this.Db.ColumnMaxLength<DeviceSubscription>(x => x.EndPoint) is > 0 and <= 4000)
        {
            this.Db.AlterColumn<DeviceSubscription>(x => x.EndPoint);
        }

        if (this.Db.ColumnMaxLength<Tag>(x => x.TagName) is > 0 and <= 4000)
        {
            this.Db.AlterColumn<Tag>(x => x.TagName);
        }

        // every report of the same user and post is added to the report text
        if (this.Db.ColumnMaxLength<MessageReportedAudit>(x => x.ReportText) is > 0 and <= 4000)
        {
            this.Db.AlterColumn<MessageReportedAudit>(x => x.ReportText);
        }
    }

    /// <summary>
    /// The option "hide if no access" had no effect, forums without read access were never listed: the existing
    /// forums get the option, so they stay hidden now that forums without the option are listed with "no access".
    /// </summary>
    /// <param name="db">The database connection.</param>
    private static void HideExistingForums(IDbConnection db)
    {
        var forums = db.Select<Forum>(f => (f.Flags & 2) != 2);

        forums.ForEach(
            forum =>
                {
                    var flags = forum.Flags | 2;

                    db.UpdateOnly(() => new Forum { Flags = flags }, f => f.ID == forum.ID);
                });
    }
}
