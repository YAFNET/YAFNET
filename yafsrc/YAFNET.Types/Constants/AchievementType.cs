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

namespace YAF.Types.Constants;

/// <summary>
/// What an achievement counts (the achievements of a type are listed together, the type is their category).
/// </summary>
public enum AchievementType
{
    /// <summary>
    /// The posts of the user (the post count of the user, the forums without a post count are not counted).
    /// </summary>
    Posts = 1,

    /// <summary>
    /// The topics the user has started.
    /// </summary>
    Topics = 2,

    /// <summary>
    /// The reactions (likes) the posts of the user have received.
    /// </summary>
    ReactionsReceived = 3,

    /// <summary>
    /// The reactions the user has given to the posts of others.
    /// </summary>
    ReactionsGiven = 4,

    /// <summary>
    /// The days since the user has joined.
    /// </summary>
    MembershipDays = 5,

    /// <summary>
    /// The reputation points of the user.
    /// </summary>
    Reputation = 6
}
