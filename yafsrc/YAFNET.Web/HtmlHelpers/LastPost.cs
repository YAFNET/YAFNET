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
/// The last post of a forum or topic (who wrote it and when), for the forum list, the topic list and the active discussions.
/// </summary>
public class LastPostItem
{
    /// <summary>
    /// Gets or sets the topic id.
    /// </summary>
    public int TopicID { get; set; }

    /// <summary>
    /// Gets or sets the topic name.
    /// </summary>
    public string TopicName { get; set; }

    /// <summary>
    /// Gets or sets the styles of the topic name (not decoded).
    /// </summary>
    public string TopicStyles { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the topic is shown (the topic list shows only the user and the time).
    /// </summary>
    public bool ShowTopic { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the last post is new for the current user.
    /// </summary>
    public bool IsNew { get; set; }

    /// <summary>
    /// Gets or sets the id of the last message.
    /// </summary>
    public int? MessageID { get; set; }

    /// <summary>
    /// Gets or sets the forum id (shown when the forum name is set).
    /// </summary>
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets the forum name, the active discussions show the forum of the topic.
    /// </summary>
    public string ForumName { get; set; }

    /// <summary>
    /// Gets or sets the user id of the last poster.
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the name of the last poster.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the display name of the last poster.
    /// </summary>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the style of the last poster (not decoded).
    /// </summary>
    public string UserStyle { get; set; }

    /// <summary>
    /// Gets or sets the suspended date of the last poster.
    /// </summary>
    public DateTime? UserSuspended { get; set; }

    /// <summary>
    /// Gets or sets the avatar url of the last poster (no avatar when not set).
    /// </summary>
    public string AvatarUrl { get; set; }

    /// <summary>
    /// Gets or sets the time of the last post.
    /// </summary>
    public DateTime Posted { get; set; }
}

/// <summary>
/// Renders the last post of a forum or topic.
/// </summary>
public static class LastPostHtmlHelper
{
    /// <summary>
    /// The maximum length of the topic name.
    /// </summary>
    private const int TopicNameLength = 50;

    /// <summary>
    /// The last post of a forum in the forum list (or "no posts").
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <param name="avatars">
    /// The avatar urls of the last posters (see IAvatars.GetAvatarUrlsForUsers), no avatar when null.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent ForumLastPost(
        this IHtmlHelper htmlHelper,
        ForumRead forum,
        IReadOnlyDictionary<int, string> avatars)
    {
        var context = BoardContext.Current;

        if (!forum.ReadAccess || !forum.LastPosted.HasValue || !forum.LastUserID.HasValue)
        {
            var empty = new TagBuilder(HtmlTag.Div);

            empty.AddCssClass("last-post last-post-empty");

            empty.InnerHtml.Append(context.Get<ILocalization>().GetText("DEFAULT", "NO_POSTS"));

            return empty;
        }

        var lastRead = context.Get<IReadTrackCurrentUser>().GetForumTopicRead(
            forum.ForumID,
            forum.LastTopicID,
            forum.LastForumAccess ?? DateTimeHelper.SqlDbMinTime(),
            forum.LastTopicAccess ?? DateTimeHelper.SqlDbMinTime());

        string avatarUrl = null;

        avatars?.TryGetValue(forum.LastUserID.Value, out avatarUrl);

        return htmlHelper.LastPost(
            new LastPostItem
            {
                TopicID = forum.LastTopicID,
                TopicName = forum.LastTopicName,
                TopicStyles = forum.LastTopicStyles,
                IsNew = forum.LastPosted.Value > lastRead,
                MessageID = forum.LastMessageID,
                UserID = forum.LastUserID.Value,
                UserName = forum.LastUser,
                UserDisplayName = forum.LastUserDisplayName,
                UserStyle = forum.Style,
                UserSuspended = forum.LastUserSuspended,
                AvatarUrl = avatarUrl,
                Posted = forum.LastPosted.Value
            });
    }

    /// <summary>
    /// The last post: the avatar of the poster, the topic, the poster and the time, and a button to the post.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="item">
    /// The last post.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent LastPost(this IHtmlHelper htmlHelper, LastPostItem item)
    {
        var context = BoardContext.Current;
        var localization = context.Get<ILocalization>();
        var linkBuilder = context.Get<ILinkBuilder>();

        var lastPost = new TagBuilder(HtmlTag.Div);

        lastPost.AddCssClass("last-post");

        if (item.AvatarUrl.IsSet())
        {
            var avatar = new TagBuilder(HtmlTag.Img) { TagRenderMode = TagRenderMode.SelfClosing };

            avatar.AddCssClass("last-post-avatar");

            // the name follows, the avatar is decoration
            avatar.MergeAttribute(HtmlAttribute.Src, item.AvatarUrl);
            avatar.MergeAttribute(HtmlAttribute.Alt, string.Empty);
            avatar.MergeAttribute("loading", "lazy");
            avatar.MergeAttribute("aria-hidden", "true");

            lastPost.InnerHtml.AppendHtml(avatar);
        }

        var body = new TagBuilder(HtmlTag.Div);

        body.AddCssClass("last-post-body");

        var topicLink = linkBuilder.GetTopicLink(item.TopicID, item.TopicName);

        if (item.ShowTopic)
        {
            body.InnerHtml.AppendHtml(RenderTopic(item, topicLink, context));
        }
        else
        {
            var label = new TagBuilder(HtmlTag.Div);

            label.AddCssClass("last-post-label");

            label.InnerHtml.Append(localization.GetText("DEFAULT", "LASTPOST"));

            body.InnerHtml.AppendHtml(label);
        }

        body.InnerHtml.AppendHtml(RenderMeta(htmlHelper, item, context));

        lastPost.InnerHtml.AppendHtml(body);

        // straight to the newest post of the topic (the topic link goes to the first unread post)
        var jump = new TagBuilder(HtmlTag.A);

        jump.AddCssClass("btn btn-sm btn-outline-secondary last-post-jump");

        jump.MergeAttribute(
            HtmlAttribute.Href,
            item.MessageID.HasValue
                ? linkBuilder.GetLink(
                    ForumPages.Posts,
                    new
                    {
                        t = item.TopicID,
                        m = item.MessageID.Value,
                        name = context.Get<IBadWordReplace>().Replace(item.TopicName)
                    })
                : topicLink);

        var jumpTitle = localization.GetText("DEFAULT", "GO_LAST_POST");

        jump.MergeAttribute(HtmlAttribute.Title, jumpTitle);
        jump.MergeAttribute(HtmlAttribute.AriaLabel, jumpTitle);
        jump.MergeAttribute("data-bs-toggle", "tooltip");

        jump.InnerHtml.AppendHtml(htmlHelper.Icon("arrow-right", marginEnd: false));

        lastPost.InnerHtml.AppendHtml(jump);

        return lastPost;
    }

    /// <summary>
    /// The topic of the last post (with the new posts badge).
    /// </summary>
    /// <param name="item">
    /// The last post.
    /// </param>
    /// <param name="topicLink">
    /// The topic link.
    /// </param>
    /// <param name="context">
    /// The board context.
    /// </param>
    /// <returns>
    /// The <see cref="TagBuilder"/>.
    /// </returns>
    private static TagBuilder RenderTopic(LastPostItem item, string topicLink, BoardContext context)
    {
        var topicLine = new TagBuilder(HtmlTag.Div);

        topicLine.AddCssClass("last-post-topic-line");

        var link = new TagBuilder(HtmlTag.A);

        link.AddCssClass("last-post-topic");

        link.MergeAttribute(HtmlAttribute.Href, topicLink);

        var topicName = context.Get<IBadWordReplace>().Replace(item.TopicName ?? string.Empty);

        // the whole name, the link shows only the beginning of a long one
        if (topicName.Length > TopicNameLength)
        {
            link.MergeAttribute(HtmlAttribute.Title, topicName);
        }

        if (context.BoardSettings.UseStyledTopicTitles && item.TopicStyles.IsSet())
        {
            // the tag builder encodes the attribute value itself
            link.MergeAttribute(HtmlAttribute.Style, context.Get<IStyleTransform>().Decode(item.TopicStyles));
        }

        link.InnerHtml.Append(topicName.Truncate(TopicNameLength));

        topicLine.InnerHtml.AppendHtml(link);

        if (!item.IsNew)
        {
            return topicLine;
        }

        var badge = new TagBuilder(HtmlTag.Span);

        badge.AddCssClass("badge text-bg-success");

        badge.InnerHtml.Append(context.Get<ILocalization>().GetText("DEFAULT", "NEW_POSTS"));

        topicLine.InnerHtml.AppendHtml(badge);

        return topicLine;
    }

    /// <summary>
    /// The poster, the time and the forum of the last post.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="item">
    /// The last post.
    /// </param>
    /// <param name="context">
    /// The board context.
    /// </param>
    /// <returns>
    /// The <see cref="TagBuilder"/>.
    /// </returns>
    private static TagBuilder RenderMeta(IHtmlHelper htmlHelper, LastPostItem item, BoardContext context)
    {
        var meta = new TagBuilder(HtmlTag.Div);

        meta.AddCssClass("last-post-meta");

        meta.InnerHtml.AppendHtml(
            htmlHelper.UserLink(
                item.UserID,
                context.BoardSettings.EnableDisplayName && item.UserDisplayName.IsSet()
                    ? item.UserDisplayName
                    : item.UserName,
                item.UserSuspended,
                item.UserStyle,
                item.UserID == context.GuestUserID));

        meta.InnerHtml.AppendHtml(Separator());

        meta.InnerHtml.AppendHtml(htmlHelper.DisplayDateTime(DateTimeFormat.BothTopic, item.Posted));

        if (item.ForumName.IsNotSet())
        {
            return meta;
        }

        meta.InnerHtml.AppendHtml(Separator());

        var forumLink = new TagBuilder(HtmlTag.A);

        forumLink.AddCssClass("last-post-forum");

        forumLink.MergeAttribute(
            HtmlAttribute.Href,
            context.Get<ILinkBuilder>().GetForumLink(item.ForumID, item.ForumName));

        forumLink.InnerHtml.AppendHtml(htmlHelper.Icon("comments", "text-secondary"));
        forumLink.InnerHtml.Append(item.ForumName);

        meta.InnerHtml.AppendHtml(forumLink);

        return meta;
    }

    /// <summary>
    /// The dot between the parts of the meta line.
    /// </summary>
    /// <returns>
    /// The <see cref="TagBuilder"/>.
    /// </returns>
    internal static TagBuilder Separator()
    {
        var separator = new TagBuilder(HtmlTag.Span);

        separator.AddCssClass("last-post-separator");
        separator.MergeAttribute("aria-hidden", "true");
        separator.InnerHtml.AppendHtml("&middot;");

        return separator;
    }
}
