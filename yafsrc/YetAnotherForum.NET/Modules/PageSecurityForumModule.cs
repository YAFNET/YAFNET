﻿/* Yet Another Forum.NET
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

namespace YAF.Modules;

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;

/// <summary>
/// Module that handles individual page security features -- needs to be expanded.
/// </summary>
[Module("Page Security Module", "Tiny Gecko", 1)]
public class PageSecurityForumModule : SimpleBaseForumModule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PageSecurityForumModule"/> class.
    /// </summary>
    /// <param name="pagePreLoad">
    /// The page pre load.
    /// </param>
    public PageSecurityForumModule(IFireEvent<ForumPagePreLoadEvent> pagePreLoad)
    {
        pagePreLoad.HandleEvent += this.PagePreLoad_HandleEvent;
    }

    /// <summary>
    /// The page pre load_ handle event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void PagePreLoad_HandleEvent(
        object sender,
        EventConverterArgs<ForumPagePreLoadEvent> e)
    {
        // no security features for login/logout pages
        if (this.CurrentForumPage.IsAccountPage)
        {
            return;
        }

        // check if login is required
        if (this.PageBoardContext.BoardSettings.RequireLogin && this.PageBoardContext.IsGuest
                                                             && this.CurrentForumPage.IsProtected)
        {
            // redirect to login page if login is required
            this.CurrentForumPage.RedirectNoAccess();
        }

        // check if it's a "registered user only page" and check permissions.
        if (this.CurrentForumPage.IsRegisteredPage && this.CurrentForumPage.User == null)
        {
            this.CurrentForumPage.RedirectNoAccess();
        }

        // not totally necessary... but provides another layer of protection...
        if (this.CurrentForumPage.IsAdminPage && !this.PageBoardContext.IsAdmin)
        {
            this.Get<LinkBuilder>().AccessDenied();
            return;
        }

        // handle security features...
        if (this.CurrentForumPage.PageType == ForumPages.Account_Register && this.PageBoardContext.BoardSettings.DisableRegistrations)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }
    }
}