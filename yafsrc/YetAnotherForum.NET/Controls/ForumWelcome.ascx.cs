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

namespace YAF.Controls;

/// <summary>
/// The forum welcome control which shows the current Time and the Last Visit Time of the Current User.
/// </summary>
public partial class ForumWelcome : BaseUserControl
{
    /// <summary>
    /// Handles the PreRender event
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    override protected void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        this.TimeNow.Text = this.GetTextFormatted("Current_Time", this.Get<IDateTimeService>().FormatTime(DateTime.UtcNow));

        var lastVisit = this.Get<ISession>().LastVisit;

        if (lastVisit.HasValue && lastVisit.Value != DateTimeHelper.SqlDbMinTime())
        {
            this.LastVisitHolder.Visible = true;
            this.TimeLastVisit.Text = this.GetTextFormatted(
                "last_visit", this.Get<IDateTimeService>().FormatDateTime(lastVisit.Value));
        }
        else
        {
            this.LastVisitHolder.Visible = false;
        }
    }
}