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

using System;
using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using YAF.Types.Extensions;

namespace YAF.Tests.Utils;

public class CustomStartupFilter(string ipAddress) : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            if (ipAddress.IsNotSet())
            {
                next(app);
            }

            app.UseMiddleware<FakeRemoteIpAddressMiddleware>(ipAddress);

            next(app);
        };
    }
}

public class FakeRemoteIpAddressMiddleware(RequestDelegate next, string ipAddress)
{
    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);

        await next(httpContext);
    }
}