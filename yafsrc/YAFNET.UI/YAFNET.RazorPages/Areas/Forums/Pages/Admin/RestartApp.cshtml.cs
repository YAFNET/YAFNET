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

namespace YAF.Pages.Admin;

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
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex()
            .AddLink(this.GetText("ADMIN_RESTARTAPP", "TITLE"));
    }

    /// <summary>
    /// The on post async.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public void OnPost()
    {
        this.applicationLifetime.StopApplication();
    }
}