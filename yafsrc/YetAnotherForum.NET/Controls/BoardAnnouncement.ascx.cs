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

namespace YAF.Controls;

/// <summary>
/// The forum welcome control which shows the current Time and the Last Visit Time of the Current User.
/// </summary>
public partial class BoardAnnouncement : BaseUserControl
{
    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    override protected void OnPreRender(EventArgs e)
    {
        if (this.PageBoardContext.BoardSettings.BoardAnnouncement.IsNotSet())
        {
            this.Visible = false;
            return;
        }

        var dateTime = Convert.ToDateTime(
            this.PageBoardContext.BoardSettings.BoardAnnouncementUntil,
            CultureInfo.InvariantCulture);

        // the end is saved in utc
        if (dateTime <= DateTime.UtcNow)
        {
            var boardSettings = this.PageBoardContext.BoardSettings;

            boardSettings.BoardAnnouncementUntil = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
            boardSettings.BoardAnnouncement = string.Empty;

            // save the settings to the database
            this.Get<BoardSettingsService>().SaveRegistry(boardSettings);

            // delete no show
            this.Visible = false;
            return;
        }

        this.Badge.CssClass = $"badge text-bg-{this.PageBoardContext.BoardSettings.BoardAnnouncementType} me-1";

        this.Announcement.CssClass = $"alert alert-{this.PageBoardContext.BoardSettings.BoardAnnouncementType} alert-dismissible";
        // formatted like a post: html is encoded, BB codes format it (it was shown as raw html, an admin with access to
        // the page could put a script on the pages of all users)
        this.Message.Text = this.Get<IFormatMessage>().Format(
            0,
            this.PageBoardContext.BoardSettings.BoardAnnouncement,
            new MessageFlags { IsHtml = false, IsBBCode = true },
            false,
            DateTime.UtcNow);

        this.DataBind();

        base.OnPreRender(e);
    }
}