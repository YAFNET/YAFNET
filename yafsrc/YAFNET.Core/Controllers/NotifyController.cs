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

using HtmlProperties;

namespace YAF.Core.Controllers;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Attributes;

/// <summary>
/// The Notifications controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class NotifyController : ForumBaseController
{
    /// <summary>
    /// Gets the paged attachments.
    /// </summary>
    /// <param name="pagedResults">
    /// The paged Results.
    /// </param>
    /// <returns>
    /// Returns the Attachment List as Grid Data Set
    /// </returns>
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GridDataSet))]
    [HttpPost("GetNotifications")]
    public Task<ActionResult<GridDataSet>> GetNotifications([FromBody] PagedResults pagedResults)
    {
        var userId = this.PageBoardContext.PageUserID;
        // the values come from the client
        var pageSize = System.Math.Clamp(pagedResults.PageSize, 1, 100);
        var pageNumber = System.Math.Max(pagedResults.PageNumber, 0);

        var activities = this.GetRepository<Activity>().GetPaged(
            a => a.UserID == userId && a.FromUserID.HasValue && a.Notification,
            pageNumber,
            pageSize);

        var attachmentItems = new List<AttachmentItem>();

        // the topic name is shown: only topics the user can (still) read
        var readableForums = this.GetRepository<VAccess>().Get(v => v.UserID == userId && v.ReadAccess > 0)
            .Select(v => v.ForumID).ToHashSet();

        activities.ForEach(
            activity =>
                {
                    var messageHolder = new HtmlContentBuilder();

                    var iconLabel = new TagBuilder(HtmlTag.Span);

                    iconLabel.AddCssClass("fa-stack");

                    var message = string.Empty;
                    var icon = string.Empty;

                    var topic = activity.TopicID.HasValue
                                    ? this.GetRepository<Topic>().GetById(activity.TopicID.Value)
                                    : null;

                    if (topic is null || !activity.MessageID.HasValue || topic.TopicFlags.IsDeleted
                        || !readableForums.Contains(topic.ForumID))
                    {
                        // the topic was deleted or moved to a forum the user can't read in the meantime
                        return;
                    }

                    // the whole item is the link, the topic name is only highlighted
                    var topicLink = new TagBuilder(HtmlTag.Span);

                    topicLink.AddCssClass("fw-semibold");
                    topicLink.InnerHtml.AppendHtml("<i class=\"fas fa-comment me-1\"></i>");
                    topicLink.InnerHtml.AppendHtml(this.HtmlEncode(topic.TopicName));

                    // the messages are html, user names are user input
                    var name = this.HtmlEncode(this.Get<IUserDisplayName>().GetNameById(activity.FromUserID.Value));

                    if (activity.ActivityFlags.ReceivedThanks)
                    {
                        icon = "heart";
                        message = this.GetTextFormatted(
                            "RECEIVED_THANKS_MSG",
                            name,
                            topicLink.RenderToString());
                    }

                    if (activity.ActivityFlags.WasMentioned)
                    {
                        icon = "at";
                        message = this.GetTextFormatted(
                            "WAS_MENTIONED_MSG",
                            name,
                            topicLink.RenderToString());
                    }

                    if (activity.ActivityFlags.WasQuoted)
                    {
                       icon = "quote-left";
                        message = this.GetTextFormatted(
                            "WAS_QUOTED_MSG",
                            name,
                            topicLink.RenderToString());
                    }

                    if (activity.ActivityFlags.WatchForumReply)
                    {
                        icon = "comments";
                        message = this.GetTextFormatted(
                            "WATCH_FORUM_MSG",
                            name,
                            topicLink.RenderToString());
                    }

                    if (activity.ActivityFlags.WatchTopicReply)
                    {
                        icon = "comment";
                        message = this.GetTextFormatted(
                            "WATCH_TOPIC_MSG",
                            name,
                            topicLink.RenderToString());
                    }

                    var notify = activity.Notification ? "text-success" : "text-secondary";

                    iconLabel.InnerHtml.AppendHtml($"""
                                                    <i class="fas fa-circle fa-stack-2x {notify}"></i>
                                                                                            <i class="fas fa-{icon} fa-stack-1x fa-inverse"></i>
                                                    """);

                    var link = this.Get<ILinkBuilder>().GetLink(
                        ForumPages.Post,
                        new { m = activity.MessageID.Value, name = topic.TopicName });

                    var created = this.PageBoardContext.BoardSettings.ShowRelativeTime
                                      ? activity.Created.ToRelativeTime()
                                      : this.Get<IDateTimeService>().FormatDateTime(activity.Created);

                    messageHolder.AppendHtml($"""
                                              <a href="{link}" class="dropdown-item text-wrap">
                                                  <div class="d-flex">
                                                      <div class="flex-shrink-0 me-2">{iconLabel.RenderToString()}</div>
                                                      <div class="flex-grow-1">
                                                          <p class="mb-1 small">{message}</p>
                                                          <p class="mb-0 small text-body-secondary"><i class="fas fa-clock me-1"></i>{created}</p>
                                                      </div>
                                                  </div>
                                              </a>
                                              """);

                    var attachment = new AttachmentItem
                    {
                                             FileName = messageHolder.RenderToString()
                                         };

                    attachmentItems.Add(attachment);
                });

        return Task.FromResult<ActionResult<GridDataSet>>(
            this.Ok(
                new GridDataSet
                    {
                        PageNumber = pageNumber,
                        TotalRecords =
                            activities.HasItems()
                                ? this.GetRepository<Activity>().Count(
                                    a => a.UserID == userId && a.FromUserID.HasValue
                                                             && a.Notification).ToType<int>()
                                : 0,
                        PageSize = pageSize,
                        AttachmentList = attachmentItems
                    }));
    }
}