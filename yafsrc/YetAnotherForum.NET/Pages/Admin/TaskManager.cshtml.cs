
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

using System.Collections.Generic;

using YAF.Core.Extensions;

/// <summary>
/// The Task Manager Admin Page
/// </summary>
public class TaskManagerModel : AdminPage
{
    [BindProperty]
    public ICollection<IBackgroundTask> Tasks { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskManagerModel"/> class.
    /// </summary>
    public TaskManagerModel()
        : base("ADMIN_TASKMANAGER", ForumPages.Admin_TaskManager)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_TASKMANAGER", "TITLE"), string.Empty);
    }

    /// <summary>
    /// binds data for this control
    /// </summary>
    protected void BindData()
    {
        this.Tasks = this.Get<ITaskModuleManager>().TaskManagerSnapshot.Values;
    }

    /// <summary>
    /// Formats the TimeSpan
    /// </summary>
    /// <param name="time">
    /// The time item.
    /// </param>
    /// <returns>
    /// returns the formatted time span.
    /// </returns>
    public string FormatTimeSpan(DateTime time)
    {
        var elapsed = DateTime.UtcNow.Subtract(time);

        return $"{elapsed.Days:D2}:{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.BindData();
    }
}