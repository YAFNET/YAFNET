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

using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Attributes;

namespace YAF.Core.Controllers;

using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.OutputCaching;

using YAF.Core.BasePages;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Push Notification Subscription controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public partial class SubscriptionController : ForumBaseController
{
    /// <summary>
    /// Subscribes the device to the push Notifications.
    /// </summary>
    /// <param name="deviceSubscription">The device subscription.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [OutputCache]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("SubscribeDevice")]
    public async Task<IActionResult> SubscribeDevice([FromBody] DeviceSubscription deviceSubscription)
    {
        // guests get no notifications, the forum sends a request to the endpoint for every notification
        if (!this.Get<VapidConfiguration>().IsPwaEnabled() || this.PageBoardContext.IsGuest
            || deviceSubscription is null || !PushEndpointHelper.IsAllowedEndpoint(deviceSubscription.EndPoint)
            || deviceSubscription.P256dh.IsNotSet() || deviceSubscription.Auth.IsNotSet())
        {
            return this.BadRequest();
        }

        // MySQL creates the columns with 255 characters, the device is part of the key
        var userAgent = this.HttpContext.Request.Headers.UserAgent.ToString().EncodeToLength(this.HtmlEncode, 255);

        var device = UserAgentRegex().Replace(userAgent, "*");

        deviceSubscription.Device = device;
        deviceSubscription.UserAgent = userAgent;

        await this.GetRepository<DeviceSubscription>().AddSubscriptionAsync(deviceSubscription);

        return this.Ok();
    }

    [GeneratedRegex("[\\d|.]+")]
    private static partial Regex UserAgentRegex();

    /// <summary>
    /// Unsubscribe the device for the current user from push Notifications.
    /// </summary>
    /// <param name="deviceSubscription">The device subscription.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [OutputCache]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("UnSubscribeDevice")]
    public async Task<IActionResult> UnSubscribeDevice([FromBody] DeviceSubscription deviceSubscription)
    {
        if (!this.Get<VapidConfiguration>().IsPwaEnabled())
        {
            return this.BadRequest();
        }

        // the device is stored from the encoded user agent, see SubscribeDevice
        var userAgent = this.HttpContext.Request.Headers.UserAgent.ToString().EncodeToLength(this.HtmlEncode, 255);
        var device = UserAgentRegex().Replace(userAgent, "*");

        await this.GetRepository<DeviceSubscription>().DeleteSubscriptionAsync(device);

        return this.Ok();
    }
}