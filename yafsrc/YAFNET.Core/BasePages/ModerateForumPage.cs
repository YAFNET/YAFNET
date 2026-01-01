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

namespace YAF.Core.BasePages;

/// <summary>
/// The moderate forum page.
/// </summary>
public class ModerateForumPage : ForumPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModerateForumPage"/> class.
    /// </summary>
    /// <param name="transPage">
    /// The trans page.
    /// </param>
    /// <param name="page">
    /// The page.
    /// </param>
    public ModerateForumPage(string transPage, ForumPages page)
        : base(transPage, page)
    {
    }

    /// <summary>
    /// The on page handler executing.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        // Only moderators or admins are allowed here
        if (!(this.PageBoardContext.IsModeratorInAnyForum || this.PageBoardContext.IsAdmin))
        {
            context.Result = this.Get<ILinkBuilder>().AccessDenied();
        }
    }
}