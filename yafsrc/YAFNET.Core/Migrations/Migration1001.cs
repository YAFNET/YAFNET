/* Yet Another Forum.NET
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
/// Version 1001 Migrations
/// </summary>
[Description("Adds Description to the board table, and adds the Referer and Country column to the Active table. Imports old PM from YAF 3 in to the new private message system")]
public class Migration1001 : MigrationBase
{
    /// <summary>
    /// Migrations
    /// </summary>
    public override void Up()
    {
        if (!this.Db.ColumnExists<Board>(x => x.Description))
        {
            this.Db.AddColumn<Board>(x => x.Description);
        }

        if (!this.Db.ColumnExists<Active>(x => x.Referer))
        {
            this.Db.AddColumn<Active>(x => x.Referer);
        }

        if (!this.Db.ColumnExists<Active>(x => x.Country))
        {
            this.Db.AddColumn<Active>(x => x.Country);
        }

        if (!this.Db.TableExists<PMessage>())
        {
            return;
        }

        this.ImportOldMessages();
    }

    /// <summary>
    /// Imports old PM from YAF 3 in to the new private message system
    /// </summary>
    private void ImportOldMessages()
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<PMessage>();

        expression.Join<UserPMessage>((a, b) => a.ID == b.PMessageID);

        var oldMessages = this.Db.SelectMulti<PMessage, UserPMessage>(expression);

        // Import Messages
        foreach (var oldMessage in oldMessages)
        {
            var flags = new PrivateMessageFlags
            {
                IsRead = true
            };

            this.Db.Insert(
                new PrivateMessage
                {
                    Body = oldMessage.Item1.Body,
                    Created = oldMessage.Item1.Created,
                    Flags = flags.BitValue,
                    FromUserId = oldMessage.Item1.FromUserID,
                    ToUserId = oldMessage.Item2.UserID
                });
        }

        // Delete old tables
        this.Db.DropTable<UserPMessage>();
        this.Db.DropTable<PMessage>();
    }
}