/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using YAF.Types.Attributes;

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

        var mainDiv = new TagBuilder("div");

        mainDiv.AddCssClass(isLastItem ? "row" : "row border-bottom mb-3 pb-3");

        var titleColumn = new TagBuilder("div");

        titleColumn.AddCssClass("col-md-8");

        var header = new TagBuilder("h5");

        var topicIcon = new TagBuilder("a");

        topicIcon.AddCssClass("topic-icon-legend-popvover");
            
        topicIcon.MergeAttribute("tabindex", "0");
        topicIcon.MergeAttribute("role", "button");
        topicIcon.MergeAttribute("aria-label", "topic indicator icon");
        topicIcon.MergeAttribute("data-bs-toggle", "popover");
        topicIcon.MergeAttribute("href", "#!");

        topicIcon.InnerHtml.AppendHtml(GetTopicIcon(topic, lastRead, htmlHelper));

        header.InnerHtml.AppendHtml(topicIcon);

        var priorityMessage = RenderPriorityMessage(topic, htmlHelper);

        if (priorityMessage != null)
        {
            header.InnerHtml.AppendHtml(priorityMessage);
        }

        var topicLink = new TagBuilder("a");

        topicLink.AddCssClass("topic-starter-popover");

        topicLink.MergeAttribute("href", context.Get<LinkBuilder>().GetTopicLink(topic.LinkTopicID, topic.Subject));
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
            "calender-day", 
            "clock",
            "text-secondary");

        topicLink.Attributes.Add(
            "data-bs-content",
            $@"{topicStarterLink.RenderToString()}{dateTimeIcon.RenderToString()}{span}{formattedStartedDatetime}</span>");

        if (topic.LastMessageID.HasValue && topic.LastPosted > lastRead)
        {
            var success = new TagBuilder("span");
                
            success.AddCssClass("badge bg-success me-1");

            success.InnerHtml.Append(context.Get<ILocalization>().GetText("NEW_POSTS"));

            header.InnerHtml.AppendHtml(success);
        }

        header.InnerHtml.AppendHtml(topicLink);

        if (!topic.TopicMovedID.HasValue)
        {
            // Render Replies & Views
            var repliesLabel = new TagBuilder("span");

            repliesLabel.AddCssClass("badge bg-light text-dark ms-1 me-1");

            repliesLabel.MergeAttribute("title", context.Get<ILocalization>().GetText("MODERATE", "REPLIES"));

            repliesLabel.MergeAttribute("data-bs-toggle", "tooltip");

            repliesLabel.InnerHtml.AppendHtml(htmlHelper.Icon("comment", " ", "far"));
            repliesLabel.InnerHtml.Append(FormatReplies(topic));

            header.InnerHtml.AppendHtml(repliesLabel);

            var viewsLabel = new TagBuilder("span");

            viewsLabel.AddCssClass("badge bg-light text-dark");

            viewsLabel.MergeAttribute("title", context.Get<ILocalization>().GetText("MODERATE", "VIEWS"));
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

        var topicDescription = HtmlHelper.StripHtml(topic.Description);

        if (topicDescription.IsSet())
        {
            var descriptionHeader = new TagBuilder("h6");

            descriptionHeader.AddCssClass("card-subtitle text-muted");

            descriptionHeader.InnerHtml.Append(topicDescription);

            titleColumn.InnerHtml.AppendHtml(descriptionHeader);
        }

        mainDiv.InnerHtml.AppendHtml(titleColumn);

        // No Last Post
        if (!topic.LastMessageID.HasValue)
        {
            return content.AppendHtml(mainDiv);
        }

        var lastPostColumn = new TagBuilder("div");

        lastPostColumn.AddCssClass("col-md-4 text-secondary");

        lastPostColumn.InnerHtml.AppendHtml(htmlHelper.LocalizedText("LASTPOST"));

        var lastPostedDateTime = topic.LastPosted;

        var formattedDatetime = context.BoardSettings.ShowRelativeTime
                                    ? lastPostedDateTime.Value.ToRelativeTime()
                                    : context.Get<IDateTimeService>().Format(DateTimeFormat.BothTopic, lastPostedDateTime);

        var userLast = htmlHelper.UserLink(
            topic.LastUserID.Value,
            context.BoardSettings.EnableDisplayName ? topic.LastUserDisplayName : topic.LastUserName,
            topic.LastUserSuspended,
            topic.LastUserStyle,
            true);

        var infoLastPost = new TagBuilder("a");

        infoLastPost.AddCssClass("btn btn-link btn-sm topic-link-popover");

        infoLastPost.MergeAttribute(
            "data-bs-content",
            $@"{userLast.RenderToString()}{dateTimeIcon.RenderToString()}{span}{formattedDatetime}</span>");
        infoLastPost.MergeAttribute("href", "#!");
        infoLastPost.MergeAttribute("data-bs-toggle", "popover");

        infoLastPost.InnerHtml.AppendHtml(htmlHelper.Icon("info-circle", "text-secondary"));

        infoLastPost.InnerHtml.AppendHtml(context.Get<ILocalization>().GetTextFormatted("by", context.BoardSettings.EnableDisplayName
            ? topic.LastUserDisplayName
            : topic.LastUserName));

        lastPostColumn.InnerHtml.AppendHtml(infoLastPost);

        var gotoLastPost = new TagBuilder("a");

        gotoLastPost.AddCssClass("btn btn-outline-secondary btn-sm");

        gotoLastPost.MergeAttribute(
            "href",
            context.Get<LinkBuilder>().GetLink(
                ForumPages.Posts,
                new { t = topic.TopicID, name = topic.Subject }));

        gotoLastPost.MergeAttribute("data-bs-toggle", "tooltip");
        gotoLastPost.MergeAttribute("title", context.Get<ILocalization>().GetText("GO_LAST_POST"));

        gotoLastPost.InnerHtml.AppendHtml(htmlHelper.Icon("share-square"));

        lastPostColumn.InnerHtml.AppendHtml(gotoLastPost);

        mainDiv.InnerHtml.AppendHtml(lastPostColumn);

        return content.AppendHtml(mainDiv);
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
    private static IHtmlContent GetTopicIcon([NotNull] PagedTopic item, DateTime lastRead, IHtmlHelper htmlHelper)
    {
        var context = BoardContext.Current;

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
            context.Get<ISessionService>().UnreadTopics++;

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
    private static IHtmlContent RenderPriorityMessage([NotNull] PagedTopic item, [NotNull] IHtmlHelper htmlHelper)
    {
        var priorityLabel = new TagBuilder("span");

        if (item.TopicMovedID.HasValue)
        {
            priorityLabel.InnerHtml.AppendHtml(htmlHelper.IconHeader("arrows-alt", "ICONLEGEND", "MOVED", "", " "));

            priorityLabel.AddCssClass("badge bg-secondary me-1");

            return priorityLabel;
        }

        if (item.PollID.HasValue)
        {
            priorityLabel.InnerHtml.AppendHtml(htmlHelper.IconHeader("poll-h", "ICONLEGEND", "POLL", "", " "));

            priorityLabel.AddCssClass("badge bg-secondary me-1");

            return priorityLabel;
        }

        switch (item.Priority)
        {
            case 1:
                priorityLabel.InnerHtml.AppendHtml(
                    htmlHelper.IconHeader("thumbtack", "ICONLEGEND", "STICKY", "", " "));

                priorityLabel.AddCssClass("badge bg-warning text-dark me-1");

                return priorityLabel;
            case 2:
                priorityLabel.InnerHtml.AppendHtml(
                    htmlHelper.IconHeader("bullhorn", "ICONLEGEND", "ANNOUNCEMENT", "", " "));

                priorityLabel.AddCssClass("badge bg-primary me-1");

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
    private static IHtmlContent FormatTopicName([NotNull] PagedTopic item, [NotNull] IHtmlHelper htmlHelper)
    {
        var context = BoardContext.Current;

        var topicLabel = new TagBuilder("span");

        var topicSubject = context.Get<IBadWordReplace>().Replace(htmlHelper.HtmlEncode(item.Subject));

        topicLabel.InnerHtml.AppendHtml(topicSubject);

        var styles = context.BoardSettings.UseStyledTopicTitles
                         ? context.Get<IStyleTransform>().Decode(item.Styles)
                         : string.Empty;

        if (styles.IsNotSet())
        {
            return topicLabel;
        }

        topicLabel.MergeAttribute("style", htmlHelper.HtmlEncode(styles));

        return topicLabel;
    }

    /// <summary>
    /// Formats the replies.
    /// </summary>
    /// <returns>
    /// Returns the formatted replies.
    /// </returns>
    private static string FormatReplies([NotNull] PagedTopic item)
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
    private static string FormatViews([NotNull] PagedTopic item)
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
    /// <param name="topicID">
    /// The topic ID.
    /// </param>
    private static IHtmlContent CreatePostPager([NotNull] PagedTopic item, [NotNull] int count, [NotNull] int pageSize, [NotNull] int topicID)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        const int NumToDisplay = 4;
        var pageCount = IPagerExtensions.PageCount(count, pageSize);

        if (pageCount <= 1)
        {
            // No Paging
            return content;
        }

        var buttonGroup = new TagBuilder("nav");

        buttonGroup.AddCssClass("ms-2 d-inline-flex");

        var pagination = new TagBuilder("ul");

        pagination.AddCssClass("pagination pagination-sm");

        if (pageCount > NumToDisplay)
        {
            var pageItemFirst = new TagBuilder("li");

            pageItemFirst.AddCssClass("page-item");

            var pageLinkFirst = new TagBuilder("a");

            pageLinkFirst.AddCssClass("page-link");

            pageLinkFirst.MergeAttribute(
                "href",
                context.Get<LinkBuilder>().GetLink(ForumPages.Posts, new { t = topicID, name = item.Subject }));

            pageLinkFirst.InnerHtml.Append("1");

            pageItemFirst.InnerHtml.AppendHtml(pageLinkFirst);
            pagination.InnerHtml.AppendHtml(pageItemFirst);

            // show links from the end
            for (var i = pageCount - (NumToDisplay - 1); i < pageCount; i++)
            {
                var post = i + 1;

                var pageItem = new TagBuilder("li");

                pageItem.AddCssClass("page-item");

                var pageLink = new TagBuilder("a");

                pageLink.AddCssClass("page-link");

                pageLink.MergeAttribute(
                    "href",
                    context.Get<LinkBuilder>().GetLink(ForumPages.Posts, new { t = topicID, name = item.Subject, p = post }));

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

                var pageItem = new TagBuilder("li");

                pageItem.AddCssClass("page-item");

                var pageLink = new TagBuilder("a");

                pageLink.AddCssClass("page-link");

                pageLink.MergeAttribute(
                    "href",
                    context.Get<LinkBuilder>().GetLink(ForumPages.Posts, new { t = topicID, name = item.Subject, p = post }));

                pageLink.InnerHtml.Append(post.ToString());

                pageItem.InnerHtml.AppendHtml(pageLink);
                pagination.InnerHtml.AppendHtml(pageItem);
            }
        }

        buttonGroup.InnerHtml.AppendHtml(pagination);

        return content.AppendHtml(buttonGroup);
    }
}