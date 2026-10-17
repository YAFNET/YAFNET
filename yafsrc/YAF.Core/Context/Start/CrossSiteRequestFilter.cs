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
/// Refuses api requests that another web site can send with the login cookie of the user (cross site request
/// forgery): a form of another site can only post form or text content without own headers. The scripts of the
/// forum send json or an own header, a browser only sends them to another site after asking it (CORS), the
/// forum does not allow that.
/// </summary>
public class CrossSiteRequestFilter : ActionFilterAttribute
{
    /// <summary>
    /// The headers the scripts of the forum send.
    /// </summary>
    private static readonly string[] OwnHeaders = ["RequestVerificationToken", "X-Requested-With"];

    /// <summary>
    /// Refuses a request that could come from a form of another web site.
    /// </summary>
    /// <param name="actionContext">The action context.</param>
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        if (IsForgeable(actionContext.Request))
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
        }
    }

    /// <summary>
    /// Checks if a request could be sent by a form of another web site.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>Returns if the request is refused.</returns>
    public static bool IsForgeable(HttpRequestMessage request)
    {
        // reading requests don't change anything
        if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head || request.Method == HttpMethod.Options)
        {
            return false;
        }

        if (Array.Exists(OwnHeaders, header => request.Headers.Contains(header)))
        {
            return false;
        }

        var mediaType = request.Content?.Headers.ContentType?.MediaType;

        return !string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase);
    }
}
