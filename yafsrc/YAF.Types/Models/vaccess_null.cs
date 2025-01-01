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
namespace YAF.Types.Models;

/// <summary>
///     A class which represents the yaf_vaccess_null views.
/// </summary>
[Serializable]
public partial class vaccess_null : IEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="vaccess_user"/> class.
    /// </summary>
    public vaccess_null()
    {
        this.OnCreated();
    }

    [Required]
    public int UserID { get; set; }
    [Required]
    public int ForumID { get; set; }
    [Required]
    public int AccessMaskID { get; set; }
    public int? GroupID { get; set; }
    public int? ReadAccess { get; set; }
    public int? PostAccess { get; set; }
    public int? ReplyAccess { get; set; }
    public int? PriorityAccess { get; set; }
    public int? PollAccess { get; set; }
    public int? VoteAccess { get; set; }
    public int? ModeratorAccess { get; set; }
    public int? EditAccess { get; set; }
    public int? DeleteAccess { get; set; }
    public int? AdminGroup { get; set; }

    /// <summary>
    /// The on created.
    /// </summary>
    partial void OnCreated();
}