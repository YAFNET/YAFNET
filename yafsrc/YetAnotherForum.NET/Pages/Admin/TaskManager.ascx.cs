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

namespace YAF.Pages.Admin;

/// <summary>
/// The Task Manager Admin Page
/// </summary>
public partial class TaskManager : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskManager"/> class. 
    /// </summary>
    public TaskManager()
        : base("ADMIN_TASKMANAGER", ForumPages.Admin_TaskManager)
    {
    }

    /// <summary>
    /// Format access mask setting color formatting.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Set access mask flags are rendered green, rest black.
    /// </returns>
    protected string GetItemColor(bool enabled)
    {
        // show enabled flag red
        return enabled ? "badge text-bg-success" : "badge text-bg-danger";
    }

    /// <summary>
    /// Get a user friendly item name.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Item Name.
    /// </returns>
    protected string GetItemName(bool enabled)
    {
        return enabled ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
    }

    /// <summary>
    /// binds data for this control
    /// </summary>
    protected void BindData()
    {
        this.taskRepeater.DataSource = this.Get<ITaskModuleManager>().TaskManagerSnapshot;
        this.taskRepeater.DataBind();
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
    protected string FormatTimeSpan(DateTime time)
    {
        var elapsed = DateTime.UtcNow.Subtract(time);

        return $"{elapsed.Days:D2}:{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_TASKMANAGER", "TITLE"), string.Empty);
    }
}