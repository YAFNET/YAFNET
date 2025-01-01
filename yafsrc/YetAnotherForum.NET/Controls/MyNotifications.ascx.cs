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

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
/// The profile Timeline
/// </summary>
public partial class MyNotifications : BaseUserControl
{
    /// <summary>
    /// Gets or sets the item count.
    /// </summary>
    protected int ItemCount { get; set; }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.WasMentioned.Text = this.GetText("WAS_MENTIONED");
        this.ReceivedThanks.Text = this.GetText("RECEIVED_THANKS");
        this.WasQuoted.Text = this.GetText("WAS_QUOTED");
        this.WatchForumReply.Text = this.GetText("WATCH_FORUM_REPLY");
        this.WatchTopicReply.Text = this.GetText("WATCH_TOPIC_REPLY");

        if (this.IsPostBack)
        {
            return;
        }

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        try
        {
            this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = "5";
        }

        this.BindData();
    }

    /// <summary>
    /// The get first item class.
    /// </summary>
    /// <param name="itemIndex">
    /// The item index.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    protected string GetFirstItemClass(int itemIndex)
    {
        return itemIndex > 0 ? "border-right" : string.Empty;
    }

    /// <summary>
    /// The get last item class.
    /// </summary>
    /// <param name="itemIndex">
    /// The item index.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    protected string GetLastItemClass(int itemIndex)
    {
        return itemIndex == this.ItemCount - 1 ? string.Empty : "border-right";
    }

    /// <summary>
    /// The activity stream_ on item data bound.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ActivityStream_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        var activity = (Tuple<Activity, User, Topic>)e.Item.DataItem;

        var messageHolder = e.Item.FindControlAs<PlaceHolder>("Message");
        var displayDateTime = e.Item.FindControlAs<DisplayDateTime>("DisplayDateTime");
        var markRead = e.Item.FindControlAs<ThemeButton>("MarkRead");
        var iconLabel = e.Item.FindControlAs<Label>("Icon");

        var message = string.Empty;
        var icon = string.Empty;

        if (!activity.Item1.MessageID.HasValue)
        {
            return;
        }

        var topicLink = new ThemeButton
                            {
                                NavigateUrl =
                                    this.Get<LinkBuilder>().GetLink(
                                        ForumPages.Posts,
                                        new { m = activity.Item1.MessageID.Value, name = activity.Item3.TopicName }),
                                Type = ButtonStyle.None,
                                Text = activity.Item3.TopicName,
                                Icon = "comment",
                                IconCssClass = "far"
                            };

        var userLink = new UserLink {
                                        UserID = activity.Item1.FromUserID.Value,
                                        Suspended = activity.Item2.Suspended,
                                        Style = activity.Item2.UserStyle,
                                        ReplaceName = activity.Item2.DisplayOrUserName()
                                    };

        if (activity.Item1.ActivityFlags.ReceivedThanks && activity.Item1.FromUserID.HasValue)
        {
            icon = "heart";
            message = this.GetTextFormatted(
                "RECEIVED_THANKS_MSG",
                userLink.RenderToString(),
                topicLink.RenderToString());
        }

        if (activity.Item1.ActivityFlags.WasMentioned && activity.Item1.FromUserID.HasValue)
        {
            icon = "at";
            message = this.GetTextFormatted(
                "WAS_MENTIONED_MSG",
                userLink.RenderToString(),
                topicLink.RenderToString());
        }

        if (activity.Item1.ActivityFlags.WasQuoted && activity.Item1.FromUserID.HasValue)
        {
            icon = "quote-left";
            message = this.GetTextFormatted(
                "WAS_QUOTED_MSG",
                userLink.RenderToString(),
                topicLink.RenderToString());
        }

        if (activity.Item1.ActivityFlags.WatchForumReply && activity.Item1.FromUserID.HasValue)
        {
            icon = "comments";
            message = this.GetTextFormatted(
                "WATCH_FORUM_MSG",
                userLink.RenderToString(),
                topicLink.RenderToString());
        }

        if (activity.Item1.ActivityFlags.WatchTopicReply && activity.Item1.FromUserID.HasValue)
        {
            icon = "comment";
            message = this.GetTextFormatted(
                "WATCH_TOPIC_MSG",
                userLink.RenderToString(),
                topicLink.RenderToString());
        }

        var notify = activity.Item1.Notification ? "text-success" : "text-secondary";

        iconLabel.Text = $@"<i class=""fas fa-circle fa-stack-2x {notify}""></i>
               <i class=""fas fa-{icon} fa-stack-1x fa-inverse""></i>";

        displayDateTime.DateTime = activity.Item1.Created;

        messageHolder.Controls.Add(new Literal
                                       {
                                           Text = message
                                       });

        if (!activity.Item1.Notification)
        {
            return;
        }

        markRead.CommandArgument = activity.Item1.MessageID.Value.ToString();
        markRead.Visible = true;
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PagerTop_PageChange(object sender, EventArgs e)
    {
        // rebind
        this.BindData();
    }

    /// <summary>
    /// Mark all Activity as read
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void MarkAll_Click(object sender, EventArgs e)
    {
        this.GetRepository<Activity>().MarkAllAsRead(this.PageBoardContext.PageUserID);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));

        this.BindData();
    }

    /// <summary>
    /// The activity stream_ on item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ActivityStream_OnItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "read")
        {
            return;
        }

        this.GetRepository<Activity>().UpdateNotification(
            this.PageBoardContext.PageUserID,
            e.CommandArgument.ToType<int>());

        this.BindData();
    }

    /// <summary>
    /// The update filter click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UpdateFilterClick(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Reset Filter
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ResetClick(object sender, EventArgs e)
    {
        this.WasMentioned.Checked = true;
        this.ReceivedThanks.Checked = true;
        this.WasQuoted.Checked = true;
        this.WatchForumReply.Checked = true;
        this.WatchTopicReply.Checked = true;

        this.BindData();
    }

    /// <summary>
    /// The page size selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

        var stream = this.GetRepository<Activity>().Notifications(this.PageBoardContext.PageUserID);

        if (!this.WasMentioned.Checked)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WasMentioned);
        }

        if (!this.ReceivedThanks.Checked)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.ReceivedThanks);
        }

        if (!this.WasQuoted.Checked)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WasQuoted);
        }

        if (!this.WatchForumReply.Checked)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WatchForumReply);
        }

        if (!this.WatchTopicReply.Checked)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WatchTopicReply);
        }

        stream.RemoveAll(a => a.Item1.ActivityFlags.GivenThanks);

        var paged = stream
            .Skip(this.PagerTop.CurrentPageIndex * this.PagerTop.PageSize).Take(this.PagerTop.PageSize).ToList();

        this.ActivityStream.DataSource = paged;

        if (paged.Any())
        {
            this.PagerTop.Count = stream.Count;

            this.ItemCount = paged.Count;

            this.NoItems.Visible = false;
        }
        else
        {
            this.PagerTop.Count = 0;
            this.ItemCount = 0;

            this.NoItems.Visible = true;
        }

        this.DataBind();
    }
}