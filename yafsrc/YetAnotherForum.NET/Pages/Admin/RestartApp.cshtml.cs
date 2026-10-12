/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

namespace YAF.Pages.Admin;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using YAF.Core.Extensions;

/// <summary>
/// The Admin Restart App Page.
/// </summary>
public class RestartAppModel : AdminPage
{
    /// <summary>
    /// The application lifetime.
    /// </summary>
    private readonly IHostApplicationLifetime applicationLifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="RestartAppModel"/> class.
    /// </summary>
    /// <param name="appLifetime">
    /// The app Lifetime.
    /// </param>
    public RestartAppModel(IHostApplicationLifetime appLifetime)
        : base("ADMIN_RESTARTAPP", ForumPages.Admin_RestartApp)
    {
        this.applicationLifetime = appLifetime;
    }

    /// <summary>
    /// Only the host admin can use this page: it stops the application of all boards,
    /// it can't be granted to the admins of a board.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
    {
        if (!this.PageBoardContext.PageUser.UserFlags.IsHostAdmin)
        {
            context.Result = this.Get<ILinkBuilder>().AccessDenied();
        }
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex()
            .AddLink(this.GetText("ADMIN_RESTARTAPP", "TITLE"));
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application is restarting.
    /// </summary>
    public bool IsRestarting { get; set; }

    /// <summary>
    /// Renders the "restarting" view and stops the application after the response was sent.
    /// The hosting environment (IIS, Windows Service, systemd, Docker) is responsible for starting it again.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult"/>.
    /// </returns>
    public IActionResult OnPost()
    {
        this.IsRestarting = true;

        this.Response.OnCompleted(() =>
            {
                _ = Task.Run(async () =>
                    {
                        // give the client time to receive and render the response
                        await Task.Delay(TimeSpan.FromSeconds(1), CancellationToken.None);

                        this.applicationLifetime.StopApplication();
                    },
                    CancellationToken.None);

                return Task.CompletedTask;
            });

        return this.Page();
    }
}