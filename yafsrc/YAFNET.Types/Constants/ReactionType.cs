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
/// The reactions a user can give to a post (stored in the Thanks table).
/// </summary>
public enum ReactionType
{
    /// <summary>
    /// The like (thumbs up).
    /// </summary>
    Like = 1,

    /// <summary>
    /// The love (heart), the reaction of the thanks given before the reactions.
    /// </summary>
    Love = 2,

    /// <summary>
    /// The laugh.
    /// </summary>
    Laugh = 3,

    /// <summary>
    /// The hooray (party popper).
    /// </summary>
    Hooray = 4,

    /// <summary>
    /// The wow (surprised face).
    /// </summary>
    Wow = 5
}
