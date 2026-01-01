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

namespace YAF.Core.BaseModules;

using YAF.Types.Attributes;

/// <summary>
/// The page bb code registration.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerScope)]
public class PageBBCodeRegistration : BaseForumModule, IHandleEvent<ForumPagePostLoadEvent>
{
    /// <summary>
    /// Gets Order.
    /// </summary>
    public int Order => 1000;

    /// <summary>
    /// The Page BB Code Registration
    /// </summary>
    /// <param name="event">
    ///     The event.
    /// </param>
    public void Handle(ForumPagePostLoadEvent @event)
    {
        switch (this.PageContext.CurrentForumPage.PageName)
        {
            case ForumPages.MyMessages:
            case ForumPages.Search:
            case ForumPages.Posts:
            case ForumPages.Post:
            case ForumPages.UserProfile:
                this.Get<IBBCodeService>().RegisterCustomBBCodeInlineElements();
                break;
        }
    }
}