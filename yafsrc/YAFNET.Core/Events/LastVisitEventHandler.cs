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

namespace YAF.Core.Events;

using System;

using YAF.Types.Attributes;

/// <summary>
/// The last visit handler.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerScope)]
public class LastVisitEventHandler : IHandleEvent<ForumPagePreLoadEvent>
{
    /// <summary>
    /// The context.
    /// </summary>
    private readonly IHttpContextAccessor context;

    /// <summary>
    /// Initializes a new instance of the <see cref="LastVisitEventHandler"/> class.
    /// </summary>
    /// <param name="yafSession">
    /// The YAF session.
    /// </param>
    /// <param name="contextAccessor">
    /// The context Accessor.
    /// </param>
    public LastVisitEventHandler(
        ISessionService yafSession, IHttpContextAccessor contextAccessor)
    {
        this.context = contextAccessor;
        this.YafSession = yafSession;
    }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 1000;

    /// <summary>
    /// Gets or sets YAF Session.
    /// </summary>
    public ISessionService YafSession { get; set; }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    ///     The event.
    /// </param>
    public void Handle(ForumPagePreLoadEvent @event)
    {
        const string PreviousVisitKey = "PreviousVisit";

        if (!BoardContext.Current.IsGuest && BoardContext.Current.PageData.Item2.Item1.PreviousVisit.HasValue
                                          && !this.YafSession.LastVisit.HasValue)
        {
            this.YafSession.LastVisit = BoardContext.Current.PageData.Item2.Item1.PreviousVisit.Value;
        }
        else if (BoardContext.Current.IsGuest && !this.YafSession.LastVisit.HasValue)
        {
            if (this.context.HttpContext.Request.Cookies.Keys.Contains(PreviousVisitKey))
            {
                // have previous visit cookie...
                var previousVisitInsecure = this.context.HttpContext.Request.Cookies[PreviousVisitKey];

                try
                {
                    this.YafSession.LastVisit = DateTime.Parse(previousVisitInsecure, CultureInfo.InvariantCulture);
                }
                catch
                {
                    this.YafSession.LastVisit = DateTimeHelper.SqlDbMinTime();
                }
            }
            else
            {
                this.YafSession.LastVisit = DateTimeHelper.SqlDbMinTime();
            }

            // set the last visit cookie...
            this.context.HttpContext.Response.Cookies.Append(
                PreviousVisitKey,
                DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                new CookieOptions { Expires = DateTime.Now.AddMonths(6) });
        }
    }
}