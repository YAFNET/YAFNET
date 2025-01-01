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

namespace YAF.Types.Constants;

/// <summary>
/// Enumerates forum info messages.
/// </summary>
public enum InfoMessage
{
    /// <summary>
    /// after posting to moderated forum
    /// </summary>
    Moderated = 1,

    /// <summary>
    /// informs user he's suspended
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// informs user about registration email being sent
    /// </summary>
    RegistrationEmail = 3,

    /// <summary>
    /// access was denied
    /// </summary>
    AccessDenied = 4,

    /// <summary>
    /// informs user about feature being disabled by admin
    /// </summary>
    Disabled = 5,

    /// <summary>
    /// informs user about invalid input/request
    /// </summary>
    Invalid = 6,

    /// <summary>
    /// system error
    /// </summary>
    Failure = 7,

    /// <summary>
    /// requires cookies
    /// </summary>
    RequiresCookies = 8,

    /// <summary>
    /// The message for admin to ask access for admin pages viewing.
    /// </summary>
    HostAdminPermissionsAreRequired = 11
}