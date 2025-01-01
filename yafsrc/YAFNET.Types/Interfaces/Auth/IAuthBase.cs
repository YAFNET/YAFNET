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

namespace YAF.Types.Interfaces.Auth;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Interface For oAUTH
/// </summary>
public interface IAuthBase
{
    /// <summary>
    /// Generates the login URL.
    /// </summary>
    /// <param name="generatePopUpUrl">if set to <c>true</c> [generate pop up URL].</param>
    /// <param name="connectCurrentUser">if set to <c>true</c> [connect current user].</param>
    /// <returns>Returns the Login URL</returns>
    string GenerateLoginUrl(bool generatePopUpUrl, bool connectCurrentUser = false);

    /// <summary>
    /// Logins the or create user.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="message">The message.</param>
    /// <returns>Returns if Login was successful or not</returns>
    bool LoginOrCreateUser(HttpRequest request, string parameters, out string message);

    /// <summary>
    /// Connects the user.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="message">The message.</param>
    /// <returns>Returns if the connect was successful or not</returns>
    bool ConnectUser(HttpRequest request, string parameters, out string message);
}