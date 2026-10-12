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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Helpers;

using System;
using System.Linq;

/// <summary>
/// Checks the endpoints of web push subscriptions, they are sent by the browser (the client)
/// and the forum sends a request to them for every notification.
/// </summary>
public static class PushEndpointHelper
{
    /// <summary>
    /// The push services of the browsers (Chrome, Edge, Firefox, Safari, ...).
    /// </summary>
    private readonly static string[] PushServiceHosts =
    [
        "fcm.googleapis.com",
        "android.googleapis.com",
        "push.services.mozilla.com",
        "notify.windows.com",
        "push.apple.com"
    ];

    /// <summary>
    /// Only https endpoints of the known push services are used, otherwise the forum could be
    /// made to send requests to any (also internal) address.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <returns>Returns if the endpoint can be used.</returns>
    public static bool IsAllowedEndpoint(string endpoint)
    {
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps
            || !uri.IsDefaultPort || uri.UserInfo.Length > 0)
        {
            return false;
        }

        var host = uri.IdnHost.ToLowerInvariant();

        return PushServiceHosts.Any(
            service => host == service || host.EndsWith($".{service}", StringComparison.Ordinal));
    }
}
