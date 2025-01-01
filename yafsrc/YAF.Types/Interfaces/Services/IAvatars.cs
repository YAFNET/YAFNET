﻿/* Yet Another Forum.NET
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
namespace YAF.Types.Interfaces.Services;

/// <summary>
/// The Avatars interface.
/// </summary>
public interface IAvatars
{
    /// <summary>
    /// The get avatar url for current user.
    /// </summary>
    /// <returns>
    /// Returns the Avatar Url
    /// </returns>
    string GetAvatarUrlForCurrentUser();

    /// <summary>
    /// The get avatar url for user.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// Returns the Avatar Url
    /// </returns>
    string GetAvatarUrlForUser(User user);

    /// <summary>
    /// The get avatar url for user.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="avatarString">
    /// The avatarString.
    /// </param>
    /// <param name="hasAvatarImage">
    /// The hasAvatarImage.
    /// </param>
    /// <returns>
    /// Returns the Avatar Url
    /// </returns>
    string GetAvatarUrlForUser(int userId, string avatarString, bool hasAvatarImage);
}