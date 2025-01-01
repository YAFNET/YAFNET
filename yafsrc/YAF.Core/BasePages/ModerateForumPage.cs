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

namespace YAF.Core.BasePages;

using System;

using YAF.Types.Constants;

/// <summary>
/// The moderate forum page.
/// </summary>
public class ModerateForumPage : ForumPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModerateForumPage"/> class.
    /// </summary>
    public ModerateForumPage()
        : this(null, ForumPages.Board)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModerateForumPage"/> class.
    /// </summary>
    /// <param name="transPage">
    /// The trans page.
    /// </param>
    /// <param name="pageType">
    /// The page Type.
    /// </param>
    public ModerateForumPage(string transPage, ForumPages pageType)
        : base(transPage, pageType)
    {
        this.Load += this.ModeratePage_Load;
    }

    /// <summary>
    /// Gets PageName.
    /// </summary>
    public override string PageName => $"moderate_{base.PageName}";

    /// <summary>
    /// Handles the Load event of the ModeratePage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void ModeratePage_Load(object sender, EventArgs e)
    {
        // Only moderators and admins are allowed here
        if (!(this.PageBoardContext.IsModeratorInAnyForum || this.PageBoardContext.IsAdmin))
        {
            this.Get<LinkBuilder>().AccessDenied();
        }
    }
}