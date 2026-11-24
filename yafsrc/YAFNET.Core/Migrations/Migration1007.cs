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
/// Version 1007 Migrations
/// </summary>
[Description("Adds the board to the event log entries, the existing entries get the board of their user.")]
public class Migration1007 : MigrationBase
{
    /// <summary>
    /// Migrations
    /// </summary>
    public override void Up()
    {
        if (this.Db.ColumnExists<EventLog>(x => x.BoardID))
        {
            return;
        }

        this.Db.AddColumn<EventLog>(x => x.BoardID);

        // the existing entries get the board of their user (the entries without a user stay for every board)
        var userIds = this.Db.ColumnDistinct<int>(
            this.Db.From<EventLog>().Where(e => e.UserID != null).Select(e => e.UserID));

        if (userIds.Count == 0)
        {
            return;
        }

        var boards = this.Db.Select<User>(u => Sql.In(u.ID, userIds)).GroupBy(u => u.BoardID);

        foreach (var board in boards)
        {
            var boardUsers = board.Select(u => (int?)u.ID).ToList();

            this.Db.UpdateOnly(() => new EventLog { BoardID = board.Key }, e => Sql.In(e.UserID, boardUsers));
        }
    }
}
