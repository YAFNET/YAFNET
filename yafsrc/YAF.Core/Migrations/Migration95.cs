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
/// Version 95 Migrations
/// </summary>
[Description("Hide the existing forums from the users without read access.")]
public class Migration95 : MigrationBase
{
    /// <summary>
    /// Migrations
    /// </summary>
    public override void Up()
    {
        // the option "hide if no access" had no effect, forums without read access were never listed: the existing
        // forums get the option, so they stay hidden now that forums without the option are listed with "no access"
        var forums = this.Db.Select<Forum>(f => (f.Flags & 2) != 2);

        forums.ForEach(
            forum =>
                {
                    var flags = forum.Flags | 2;

                    this.Db.UpdateOnly(() => new Forum { Flags = flags }, f => f.ID == forum.ID);
                });
    }
}
