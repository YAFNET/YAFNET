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

using ServiceStack.DataAnnotations;

namespace YAF.Types.Models;

/// <summary>
/// A class which represents the yaf_vaccess_user views.
/// Implements the <see cref="YAF.Types.Interfaces.Data.IEntity" />
/// </summary>
/// <seealso cref="YAF.Types.Interfaces.Data.IEntity" />
[Serializable]
[Alias("vaccess_user")]
public class VaccessUser : IEntity
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    [AutoIncrement]
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the forum identifier.
    /// </summary>
    /// <value>The forum identifier.</value>
    public int? ForumID { get; set; }

    /// <summary>
    /// Gets or sets the access mask identifier.
    /// </summary>
    /// <value>The access mask identifier.</value>
    public int? AccessMaskID { get; set; }

    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    /// <value>The group identifier.</value>
    public int? GroupID { get; set; }

    /// <summary>
    /// Gets or sets the read access.
    /// </summary>
    /// <value>The read access.</value>
    public int? ReadAccess { get; set; }

    /// <summary>
    /// Gets or sets the post access.
    /// </summary>
    /// <value>The post access.</value>
    public int? PostAccess { get; set; }

    /// <summary>
    /// Gets or sets the reply access.
    /// </summary>
    /// <value>The reply access.</value>
    public int? ReplyAccess { get; set; }

    /// <summary>
    /// Gets or sets the priority access.
    /// </summary>
    /// <value>The priority access.</value>
    public int? PriorityAccess { get; set; }

    /// <summary>
    /// Gets or sets the poll access.
    /// </summary>
    /// <value>The poll access.</value>
    public int? PollAccess { get; set; }

    /// <summary>
    /// Gets or sets the vote access.
    /// </summary>
    /// <value>The vote access.</value>
    public int? VoteAccess { get; set; }

    /// <summary>
    /// Gets or sets the moderator access.
    /// </summary>
    /// <value>The moderator access.</value>
    public int? ModeratorAccess { get; set; }

    /// <summary>
    /// Gets or sets the edit access.
    /// </summary>
    /// <value>The edit access.</value>
    public int? EditAccess { get; set; }

    /// <summary>
    /// Gets or sets the delete access.
    /// </summary>
    /// <value>The delete access.</value>
    public int? DeleteAccess { get; set; }

    /// <summary>
    /// Gets or sets the admin group.
    /// </summary>
    /// <value>The admin group.</value>
    public int? AdminGroup { get; set; }
}