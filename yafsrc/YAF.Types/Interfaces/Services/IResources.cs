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
/// The Resources interface.
/// </summary>
public interface IResources
{
    /// <summary>
    /// Gets the forum user info as JSON string for the hover cards
    /// </summary>
    /// <param name="context">The context.</param>
    void GetUserInfo(HttpContext context);

    /// <summary>
    /// Gets the list of all Custom BB Codes
    /// </summary>
    /// <param name="context">The context.</param>
    void GetCustomBBCodes(HttpContext context);

    /// <summary>
    /// Get all Mentioned Users
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    void GetMentionUsers(HttpContext context);

    /// <summary>
    /// Gets the Default Text Avatar
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    void GetTextAvatar(HttpContext context);

    /// <summary>
    /// The get response local avatar.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    void GetResponseLocalAvatar(HttpContext context);
}