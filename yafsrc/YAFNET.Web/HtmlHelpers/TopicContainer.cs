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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Web.HtmlHelpers;

/// <summary>
/// Class TopicContainerHtmlHelper.
/// </summary>
public static class TopicContainerHtmlHelper
{
    /// <summary>
    /// The active users.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="topic">
    /// The Paged Topic
    /// </param>
    /// <param name="isLastItem">
    /// Indicate if it is last item in list
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent TopicContainer(
        this IHtmlHelper htmlHelper,
        PagedTopic topic,
        bool isLastItem)
    {
        var content = new HtmlContentBuilder();

        var context = BoardContext.Current;

        var lastRead = context.Get<IReadTrackCurrentUser>().GetForumTopicRead(
            topic.ForumID,
            topic.TopicID,
            topic.LastForumAccess ?? DateTimeHelper.SqlDbMinTime(),
            topic.LastTopicAccess ?? DateTimeHelper.SqlDbMinTime());

        var mainDiv = new TagBuilder(HtmlTag.Div);

        mainDiv.AddCssClass(isLastItem ? "row" : "row border-bottom mb-3 pb-3");

        var titleColumn = new TagBuilder(HtmlTag.Div);

        titleColumn.AddCssClass("col-md-8");

        var header = new TagBuilder(HtmlTag.H5);

        var topicIcon = new TagBuilder(HtmlTag.A);

        topicIcon.AddCssClass("topic-icon-legend-popvover");

        topicIcon.MergeAttribute(HtmlAttribute.Tabindex, "0");
        topicIcon.MergeAttribute(HtmlAttribute.Role, HtmlTag.Button);
        topicIcon.MergeAttribute(HtmlAttribute.AriaLabel, "topic indicator icon");
        topicIcon.MergeAttribute("data-bs-toggle", "popover");
        topicIcon.MergeAttribute(HtmlAttribute.Href, "#!");

        topicIcon.InnerHtml.AppendHtml(GetTopicIcon(topic, lastRead, htmlHelper));

        header.InnerHtml.AppendHtml(topicIcon);

        var priorityMessage = RenderPriorityMessage(topic, htmlHelper);

        if (priorityMessage is not null)
        {
            header.InnerHtml.AppendHtml(priorityMessage);
        }

        var topicLink = new TagBuilder(HtmlTag.A);

        topicLink.AddCssClass("topic-starter-popover");

        topicLink.MergeAttribute(HtmlAttribute.Href, context.Get<ILinkBuilder>().GetTopicLink(topic.LinkTopicID, topic.Subject));
        topicLink.MergeAttribute("data-bs-toggle", "popover");

        topicLink.InnerHtml.AppendHtml(FormatTopicName(topic, htmlHelper));

        var topicStartedDateTime = topic.Posted;

        var formattedStartedDatetime = context.BoardSettings.ShowRelativeTime
                                           ? topicStartedDateTime.ToRelativeTime()
                                           : context.Get<IDateTimeService>().Format(DateTimeFormat.BothTopic, topicStartedDateTime);

        var topicStarterLink = htmlHelper.UserLink(
            topic.UserID,
            context.BoardSettings.EnableDisplayName ? topic.StarterDisplay : topic.Starter,
            topic.StarterSuspended,
            topic.StarterStyle,
            true);

        var span = context.BoardSettings.ShowRelativeTime ? @"<span class=""popover-timeago"">" : "<span>";

        var dateTimeIcon = htmlHelper.IconBadge(
            "calendar-day",
            "clock",
            "text-secondary");

        topicLink.Attributes.Add(
            "data-bs-content",
            $"{topicStarterLink.RenderToString()}{dateTimeIcon.RenderToString()}{span}{formattedStartedDatetime}</span>");

        if (topic.LastMessageID.HasValue && topic.LastPosted > lastRead)
        {
            var success = new TagBuilder(HtmlTag.Span);

            success.AddCssClass("badge text-bg-success me-1");

            success.InnerHtml.Append(context.Get<ILocalization>().GetText("NEW_POSTS"));

            header.InnerHtml.AppendHtml(success);
        }

        header.InnerHtml.AppendHtml(topicLink);

        if (!topic.TopicMovedID.HasValue)
        {
            // Render Replies & Views
            var repliesLabel = new TagBuilder(HtmlTag.Span);

            repliesLabel.AddCssClass("badge text-light-emphasis bg-light-subtle ms-1 me-1");

            repliesLabel.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("MODERATE", "REPLIES"));

            repliesLabel.MergeAttribute("data-bs-toggle", "tooltip");

            repliesLabel.InnerHtml.AppendHtml(htmlHelper.Icon("comment", " ", "far"));
            repliesLabel.InnerHtml.Append(FormatReplies(topic));

            header.InnerHtml.AppendHtml(repliesLabel);

            var viewsLabel = new TagBuilder(HtmlTag.Span);

            viewsLabel.AddCssClass("badge text-light-emphasis bg-light-subtle");

            viewsLabel.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("MODERATE", "VIEWS"));
            viewsLabel.MergeAttribute("data-bs-toggle", "tooltip");

            viewsLabel.InnerHtml.AppendHtml(htmlHelper.Icon("eye", " ", "far"));
            viewsLabel.InnerHtml.Append(FormatViews(topic));

            header.InnerHtml.AppendHtml(viewsLabel);
        }

        // Render Pager
        var actualPostCount = topic.Replies + 1;

        if (context.BoardSettings.ShowDeletedMessages)
        {
            // add deleted posts not included in replies...
            actualPostCount += topic.NumPostsDeleted;
        }

        header.InnerHtml.AppendHtml(
            CreatePostPager(
                topic,
                actualPostCount,
                context.BoardSettings.PostsPerPage,
                topic.LinkTopicID));

        titleColumn.InnerHtml.AppendHtml(header);

        var topicDescription = HtmlTagHelper.StripHtml(topic.Description);

        if (topicDescription.IsSet())
        {
            RenderTopicDescription(topicDescription, titleColumn);
        }

        mainDiv.InnerHtml.AppendHtml(titleColumn);

        // No Last Post
        return !topic.LastMessageID.HasValue
            ? content.AppendHtml(mainDiv)
            : RenderLastPost(htmlHelper, topic, context, dateTimeIcon, span, mainDiv, content);
    }

    /// <summary>
    /// Renders the last post.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="topic">The topic.</param>
    /// <param name="context">The context.</param>
    /// <param name="dateTimeIcon">The date time icon.</param>
    /// <param name="span">The span.</param>
    /// <param name="mainDiv">The main div.</param>
    /// <param name="content">The content.</param>
    /// <returns>IHtmlContentBuilder.</returns>
    private static IHtmlContentBuilder RenderLastPost(IHtmlHelper htmlHelper, PagedTopic topic, BoardContext context,
        IHtmlContent dateTimeIcon, string span, TagBuilder mainDiv, HtmlContentBuilder content)
    {
        var lastPostColumn = new TagBuilder(HtmlTag.Div);

        lastPostColumn.AddCssClass("col-md-4 text-secondary");

        lastPostColumn.InnerHtml.AppendHtml(htmlHelper.LocalizedText("LASTPOST"));

        var lastPostedDateTime = topic.LastPosted;

        var formattedDatetime = context.BoardSettings.ShowRelativeTime
                                    ? lastPostedDateTime!.Value.ToRelativeTime()
                                    : context.Get<IDateTimeService>().Format(DateTimeFormat.BothTopic, lastPostedDateTime);
        var userLast = htmlHelper.UserLink(
        topic.LastUserID!.Value,
        context.BoardSettings.EnableDisplayName ? topic.LastUserDisplayName : topic.LastUserName,
            topic.LastUserSuspended,
            topic.LastUserStyle,
            true);

        var infoLastPost = new TagBuilder(HtmlTag.A);

        infoLastPost.AddCssClass("btn btn-link btn-sm topic-link-popover");
        infoLastPost.MergeAttribute(
        "data-bs-content",
            $"{userLast.RenderToString()}{dateTimeIcon.RenderToString()}{span}{formattedDatetime}</span>");
        infoLastPost.MergeAttribute(HtmlAttribute.Href, "#!");
        infoLastPost.MergeAttribute("data-bs-toggle", "popover");

        infoLastPost.InnerHtml.AppendHtml(htmlHelper.Icon("info-circle", "text-secondary"));

        infoLastPost.InnerHtml.AppendHtml(context.Get<ILocalization>().GetTextFormatted("by", context.BoardSettings.EnableDisplayName
            ? topic.LastUserDisplayName
            : topic.LastUserName));

        lastPostColumn.InnerHtml.AppendHtml(infoLastPost);

        var gotoLastPost = new TagBuilder(HtmlTag.A);

        gotoLastPost.AddCssClass("btn btn-outline-secondary btn-sm");

        gotoLastPost.MergeAttribute(
            HtmlAttribute.Href,
            context.Get<ILinkBuilder>().GetTopicLink(topic.TopicID, topic.Subject));

        gotoLastPost.MergeAttribute("data-bs-toggle", "tooltip");
        gotoLastPost.MergeAttribute(HtmlAttribute.Title, context.Get<ILocalization>().GetText("GO_LAST_POST"));

        gotoLastPost.InnerHtml.AppendHtml(htmlHelper.Icon("share-square", marginEnd: false));

        lastPostColumn.InnerHtml.AppendHtml(gotoLastPost);

        mainDiv.InnerHtml.AppendHtml(lastPostColumn);

        return content.AppendHtml(mainDiv);
    }

    private static void RenderTopicDescription(string topicDescription, TagBuilder titleColumn)
    {
        var descriptionHeader = new TagBuilder(HtmlTag.H6);

        descriptionHeader.AddCssClass("card-subtitle text-body-secondary");

        descriptionHeader.InnerHtml.Append(topicDescription);

        titleColumn.InnerHtml.AppendHtml(descriptionHeader);
    }

    /// <summary>
    /// Gets the topic image.
    /// </summary>
    /// <param name="item">
    ///     The item.
    /// </param>
    /// <param name="lastRead">
    ///     The last Read.
    /// </param>
    /// <param name="htmlHelper"></param>
    /// <returns>
    /// Returns the Topic Image
    /// </returns>
    private static IHtmlContent GetTopicIcon(PagedTopic item, DateTime lastRead, IHtmlHelper htmlHelper)
    {
        var lastPosted = item.LastPosted ?? DateTimeHelper.SqlDbMinTime();

        var topicFlags = new TopicFlags(item.TopicFlags);
        var forumFlags = new ForumFlags(item.ForumFlags);

        var isHotTopic = IsPopularTopic(lastPosted, item);

        if (item.TopicMovedID.HasValue)
        {
            return htmlHelper.IconStack(
                "comment",
                "text-secondary",
                "arrows-alt",
                "fa-inverse",
                "fa-1x");
        }

        var topic = isHotTopic ? "fire" : "comment";

        if (lastPosted > lastRead)
        {
            if (topicFlags.IsLocked || forumFlags.IsLocked)
            {
                return htmlHelper.IconStack(
                    "comment",
                    "text-success",
                    "lock",
                    "fa-inverse",
                    "fa-1x");
            }

            return htmlHelper.IconStack(
                "comment",
                "text-success",
                topic,
                "fa-inverse",
                "fa-1x");
        }

        if (topicFlags.IsLocked || forumFlags.IsLocked)
        {
            return htmlHelper.IconStack(
                "comment",
                "text-secondary",
                "lock",
                "fa-inverse",
                "fa-1x");
        }

        return htmlHelper.IconStack(
            "comment",
            "text-secondary",
            topic,
            "fa-inverse",
            "fa-1x");
    }

    /// <summary>
    /// Checks if the Topic is Hot or not
    /// </summary>
    /// <param name="lastPosted">
    /// The last Posted DateTime.
    /// </param>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Returns if the Topic is Hot or not
    /// </returns>
    private static bool IsPopularTopic(DateTime lastPosted, PagedTopic item)
    {
        if (lastPosted > DateTime.Now.AddDays(-BoardContext.Current.BoardSettings.PopularTopicDays))
        {
            return item.Replies >= BoardContext.Current.BoardSettings.PopularTopicReplys ||
                   item.Views >= BoardContext.Current.BoardSettings.PopularTopicViews;
        }

        return false;
    }

    /// <summary>
    /// Creates the status message text for a topic. (i.e. Moved, Poll, Sticky, etc.)
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <param name="htmlHelper"></param>
    private static TagBuilder RenderPriorityMessage(PagedTopic item, IHtmlHelper htmlHelper)
    {
        var priorityLabel = new TagBuilder(HtmlTag.Span);

        if (item.TopicMovedID.HasValue)
        {
            priorityLabel.InnerHtml.AppendHtml(htmlHelper.IconHeader("arrows-alt", "ICONLEGEND", "MOVED", "", " "));

            priorityLabel.AddCssClass("badge text-bg-secondary me-1");

            return priorityLabel;
        }

        if (item.PollID.HasValue)
        {
            priorityLabel.InnerHtml.AppendHtml(htmlHelper.IconHeader("poll-h", "ICONLEGEND", "POLL", "", " "));

            priorityLabel.AddCssClass("badge text-bg-secondary me-1");

            return priorityLabel;
        }

        switch (item.Priority)
        {
            case 1:
                priorityLabel.InnerHtml.AppendHtml(
                    htmlHelper.IconHeader("thumbtack", "ICONLEGEND", "STICKY", "", " "));

                priorityLabel.AddCssClass("badge text-bg-warning me-1");

                return priorityLabel;
            case 2:
                priorityLabel.InnerHtml.AppendHtml(
                    htmlHelper.IconHeader("bullhorn", "ICONLEGEND", "ANNOUNCEMENT", "", " "));

                priorityLabel.AddCssClass("badge text-bg-primary me-1");

                return priorityLabel;
        }

        return null;
    }

    /// <summary>
    /// Format the Topic Name and Add Status Icon/Text
    /// if enabled and available
    /// </summary>
    /// <returns>
    /// Returns the Topic Name (with Status Icon)
    /// </returns>
    private static TagBuilder FormatTopicName(PagedTopic item, IHtmlHelper htmlHelper)
    {
        var context = BoardContext.Current;

        var topicLabel = new TagBuilder(HtmlTag.Span);

        var topicSubject = context.Get<IBadWordReplace>().Replace(htmlHelper.HtmlEncode(item.Subject));

        topicLabel.InnerHtml.AppendHtml(topicSubject);

        var styles = context.BoardSettings.UseStyledTopicTitles
                         ? context.Get<IStyleTransform>().Decode(item.Styles)
                         : string.Empty;

        if (styles.IsNotSet())
        {
            return topicLabel;
        }

        topicLabel.MergeAttribute(HtmlAttribute.Style, htmlHelper.HtmlEncode(styles));

        return topicLabel;
    }

    /// <summary>
    /// Formats the replies.
    /// </summary>
    /// <returns>
    /// Returns the formatted replies.
    /// </returns>
    private static string FormatReplies(PagedTopic item)
    {
        var repStr = "&nbsp;";

        var replies = item.Replies;
        var numDeleted = item.NumPostsDeleted;

        if (replies < 0)
        {
            return repStr;
        }

        if (BoardContext.Current.BoardSettings.ShowDeletedMessages && numDeleted > 0)
        {
            repStr = $"{replies + numDeleted:N0}";
        }
        else
        {
            repStr = $"{replies:N0}";
        }

        return repStr;
    }

    /// <summary>
    /// Formats the views.
    /// </summary>
    /// <returns>
    /// Returns the formatted views.
    /// </returns>
    private static string FormatViews(PagedTopic item)
    {
        var views = item.Views;
        return item.TopicMovedID.HasValue ? "&nbsp;" : $"{views:N0}";
    }

    /// <summary>
    /// Create pager for post.
    /// </summary>
    /// <param name="item">
    /// The Paged Topic
    /// </param>
    /// <param name="count">
    /// The count.
    /// </param>
    /// <param name="pageSize">
    /// The page Size.
    /// </param>
    /// <param name="topicId">
    /// The topic ID.
    /// </param>
    private static IHtmlContent CreatePostPager(PagedTopic item, int count, int pageSize, int topicId)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        const int numToDisplay = 4;
        var pageCount = IPagerExtensions.PageCount(count, pageSize);

        if (pageCount <= 1)
        {
            // No Paging
            return content;
        }

        var buttonGroup = new TagBuilder(HtmlTag.Nav);

        buttonGroup.AddCssClass("ms-2 d-inline-flex");

        var pagination = new TagBuilder(HtmlTag.Ul);

        pagination.AddCssClass("pagination pagination-sm");

        if (pageCount > numToDisplay)
        {
            var pageItemFirst = new TagBuilder(HtmlTag.Li);

            pageItemFirst.AddCssClass("page-item");

            var pageLinkFirst = new TagBuilder(HtmlTag.A);

            pageLinkFirst.AddCssClass("page-link");

            pageLinkFirst.MergeAttribute(
                HtmlAttribute.Href,
                context.Get<ILinkBuilder>().GetTopicLink(topicId, item.Subject));

            pageLinkFirst.InnerHtml.Append("1");

            pageItemFirst.InnerHtml.AppendHtml(pageLinkFirst);
            pagination.InnerHtml.AppendHtml(pageItemFirst);

            // show links from the end
            for (var i = pageCount - (numToDisplay - 1); i < pageCount; i++)
            {
                var post = i + 1;

                var pageItem = new TagBuilder(HtmlTag.Li);

                pageItem.AddCssClass("page-item");

                var pageLink = new TagBuilder(HtmlTag.A);

                pageLink.AddCssClass("page-link");

                pageLink.MergeAttribute(
                    HtmlAttribute.Href,
                    context.Get<ILinkBuilder>().GetLink(ForumPages.Posts, new { t = topicId, name = item.Subject, p = post }));

                pageLink.InnerHtml.Append(post.ToString());

                pageItem.InnerHtml.AppendHtml(pageLink);
                pagination.InnerHtml.AppendHtml(pageItem);
            }
        }
        else
        {
            for (var i = 0; i < pageCount; i++)
            {
                var post = i + 1;

                var pageItem = new TagBuilder(HtmlTag.Li);

                pageItem.AddCssClass("page-item");

                var pageLink = new TagBuilder(HtmlTag.A);

                pageLink.AddCssClass("page-link");

                pageLink.MergeAttribute(
                    HtmlAttribute.Href,
                    context.Get<ILinkBuilder>().GetLink(ForumPages.Posts, new { t = topicId, name = item.Subject, p = post }));

                pageLink.InnerHtml.Append(post.ToString());

                pageItem.InnerHtml.AppendHtml(pageLink);
                pagination.InnerHtml.AppendHtml(pageItem);
            }
        }

        buttonGroup.InnerHtml.AppendHtml(pagination);

        return content.AppendHtml(buttonGroup);
    }
}