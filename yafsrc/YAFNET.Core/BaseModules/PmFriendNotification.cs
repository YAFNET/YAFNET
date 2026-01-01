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
/// The Page PM & Friend Popup Module
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerScope)]
public class PmFriendNotification : BaseForumModule, IHandleEvent<ForumPagePostLoadEvent>
{
    /// <summary>
    /// Gets Order.
    /// </summary>
    public int Order => 1000;

    /// <summary>
    /// Show Pm or Buddy Request Notify
    /// </summary>
    /// <param name="event">
    ///     The event.
    /// </param>
    public void Handle(ForumPagePostLoadEvent @event)
    {
        // This happens when user logs in
        if (this.DisplayPmPopup() && this.PageContext.CurrentForumPage.PageName != ForumPages.MyMessages)
        {
            this.PageContext.ShowConfirmModal(
                this.GetText("COMMON", "UNREAD_MSG_TITLE"),
                this.GetTextFormatted("UNREAD_MSG2", this.PageContext.UnreadPrivate),
                this.GetText("COMMON", "YES"),
                this.GetText("COMMON", "NO"),
                this.Get<ILinkBuilder>().GetLink(ForumPages.MyMessages));

            // Avoid Showing Both Popups
            return;
        }

        if (!this.DisplayPendingBuddies()  || this.PageContext.CurrentForumPage.PageName == ForumPages.MyFriends)
        {
            return;
        }

        this.PageContext.ShowConfirmModal(
            this.GetText("BUDDY", "PENDINGBUDDIES_TITLE"),
            this.GetTextFormatted("PENDINGBUDDIES2", this.PageContext.PendingBuddies),
            this.GetText("COMMON", "YES"),
            this.GetText("COMMON", "NO"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.MyFriends));

        this.Get<ISessionService>().LastPendingBuddies = this.PageContext.LastPendingBuddies;
    }

    /// <summary>
    /// Displays the PM popup.
    /// </summary>
    /// <returns>
    /// The display pm popup.
    /// </returns>
    protected bool DisplayPmPopup()
    {
        return this.PageContext.UnreadPrivate > 0;
    }

    /// <summary>
    /// The last pending buddies.
    /// </summary>
    /// <returns>
    /// whether we should display the pending buddies notification or not
    /// </returns>
    protected bool DisplayPendingBuddies()
    {
        return this.PageContext.PendingBuddies > 0
               && this.PageContext.LastPendingBuddies > this.Get<ISessionService>().LastPendingBuddies;
    }
}