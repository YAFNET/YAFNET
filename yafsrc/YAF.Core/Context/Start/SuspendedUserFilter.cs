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

namespace YAF.Core.Context.Start;

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

/// <summary>
/// Refuses the api requests of suspended users: the pages show the suspension (info page), but the scripts of the
/// pages could still thank posts, vote, upload files or send friend requests.
/// </summary>
public class SuspendedUserFilter : ActionFilterAttribute
{
    /// <summary>
    /// Refuses the request of a suspended user.
    /// </summary>
    /// <param name="actionContext">The action context.</param>
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        if (IsSuspended(BoardContext.Current))
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
        }
    }

    /// <summary>
    /// Checks if the user of the request is suspended (an expired suspension is lifted by the next page).
    /// </summary>
    /// <param name="context">The board context.</param>
    /// <returns>Returns if the user is suspended.</returns>
    public static bool IsSuspended(BoardContext context)
    {
        return context is { IsSuspended: true } && context.SuspendedUntil > DateTime.UtcNow;
    }
}
