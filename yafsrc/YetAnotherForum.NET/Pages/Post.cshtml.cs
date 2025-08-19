/* Yet Another Forum.NET
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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System.Threading.Tasks;

namespace YAF.Pages;

using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Exceptions;
using YAF.Types.Extensions;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The Posts list page
/// </summary>
public class PostModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "PostsModel" /> class.
    /// </summary>
    public PostModel()
        : base("POSTS", ForumPages.Posts)
    {
    }

    /// <summary>
    /// Gets or sets the topic subject.
    /// </summary>
    /// <value>The topic subject.</value>
    [TempData]
    public string TopicSubject { get; set; }

    /// <summary>
    /// Gets or sets the announcements.
    /// </summary>
    [BindProperty]
    public List<PagedTopic> Announcements { get; set; }

    /// <summary>
    /// Gets or sets the topics.
    /// </summary>
    [BindProperty]
    public List<PagedMessage> Messages { get; set; }

    /// <summary>
    /// The create page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        if (this.PageBoardContext.PageTopic is null)
        {
            return;
        }

        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

        this.PageBoardContext.PageLinks.AddLink(
            this.Get<IBadWordReplace>().Replace(HttpUtility.HtmlDecode(this.PageBoardContext.PageTopic.TopicName)),
            string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet(string name, int m, int? p = null)
    {
        // in case topic is deleted or not existent
        if (this.PageBoardContext.PageTopic is null)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.Get<ISessionService>().SetPageData(new List<PagedMessage>());

        if (!this.PageBoardContext.IsGuest && this.PageBoardContext.PageUser.Activity)
        {
            this.GetRepository<Activity>().UpdateTopicNotification(
                this.PageBoardContext.PageUserID,
                this.PageBoardContext.PageTopicID);
        }

        if (this.PageBoardContext.IsGuest && !this.PageBoardContext.ForumReadAccess)
        {
            // attempt to get permission by redirecting to login...
            var result = this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);

            if (result != null)
            {
                return result;
            }
        }
        else if (!this.PageBoardContext.ForumReadAccess)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        // Clear Multi-quotes if topic is different
        if (this.Get<ISessionService>().MultiQuoteIds != null && !this.Get<ISessionService>().MultiQuoteIds
                .Exists(x => x.TopicID.Equals(this.PageBoardContext.PageTopicID)))
        {
            this.Get<ISessionService>().MultiQuoteIds = null;
        }

        var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.PageBoardContext.PageTopic.TopicName));

        this.TopicSubject = this.PageBoardContext.PageTopic.Description.IsSet()
                                ? $"{topicSubject} - <em>{this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.PageBoardContext.PageTopic.Description))}</em>"
                                : this.Get<IBadWordReplace>().Replace(topicSubject);

        return this.BindData(m, p);
    }

    /// <summary>
    /// Called when [get quick reply dialog].
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetQuickReply()
    {
        this.Get<IDataCache>().Set("TopicID", this.PageBoardContext.PageTopicID, TimeSpan.FromMinutes(5));

        return new PartialViewResult
        {
            ViewName = "Dialogs/QuickReply", ViewData = new ViewDataDictionary<QuickReplyModal>(this.ViewData, new QuickReplyModal())
        };
    }

    /// <summary>
    /// The post reply link_ click.
    /// </summary>
    public IActionResult OnPostReplyLink()
    {
        if (this.PageBoardContext.PageTopic.TopicFlags.IsLocked && !this.PageBoardContext.ForumModeratorAccess)
        {
            return this.PageBoardContext.Notify(this.GetText("WARN_TOPIC_LOCKED"), MessageTypes.warning);
        }

        if (this.PageBoardContext.PageForum.ForumFlags.IsLocked && !this.PageBoardContext.ForumModeratorAccess)
        {
            return this.PageBoardContext.Notify(this.GetText("WARN_FORUM_LOCKED"), MessageTypes.warning);
        }

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.PostMessage,
            new {
                    t = this.PageBoardContext.PageTopicID,
                    f = this.PageBoardContext.PageForumID,
                    name = this.PageBoardContext.PageTopic.TopicName
                });
    }

    /// <summary>
    /// The Previous topic click.
    /// </summary>
    public IActionResult OnPostPrevTopic()
    {
        var previousTopic = this.GetRepository<Topic>().FindPrevious(this.PageBoardContext.PageTopic);

        if (previousTopic is null)
        {
            return this.PageBoardContext.Notify(this.GetText("INFO_NOMORETOPICS"), MessageTypes.info);
        }

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Posts,
            new { t = previousTopic.ID, name = previousTopic.TopicName });
    }

    /// <summary>
    /// Watch Topic
    /// </summary>
    public async Task<IActionResult> OnPostTrackTopicAsync()
    {
        await this.GetRepository<WatchTopic>().AddAsync(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID);

        await this.HandleWatchTopicAsync();

        this.PageBoardContext.SessionNotify(this.GetText("INFO_WATCH_TOPIC"), MessageTypes.warning);

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Posts,
            new { t = this.PageBoardContext.PageTopic.ID, name = this.PageBoardContext.PageTopic.TopicName });
    }

    /// <summary>
    /// Un-Watch Topic
    /// </summary>
    public IActionResult OnPostUnTrackTopic()
    {
        this.GetRepository<WatchTopic>().Delete(
            w => w.TopicID == this.PageBoardContext.PageTopicID && w.UserID == this.PageBoardContext.PageUserID);

        this.PageBoardContext.SessionNotify(this.GetText("INFO_UNWATCH_TOPIC"), MessageTypes.info);

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Posts,
           new { t = this.PageBoardContext.PageTopic.ID, name = this.PageBoardContext.PageTopic.TopicName });
    }

    /// <summary>
    /// The unlock topic click.
    /// </summary>
    public IActionResult OnPostUnlockTopic()
    {
        if (!this.PageBoardContext.ForumModeratorAccess)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        var flags = this.PageBoardContext.PageTopic.TopicFlags;

        flags.IsLocked = false;

        this.GetRepository<Topic>().Lock(this.PageBoardContext.PageTopicID, flags.BitValue);

        this.PageBoardContext.SessionNotify(this.GetText("INFO_TOPIC_UNLOCKED"), MessageTypes.info);

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Posts,
            new { t = this.PageBoardContext.PageTopic.ID, name = this.PageBoardContext.PageTopic.TopicName });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private IActionResult BindData(int m, int? p = null)
    {
        var showDeleted = this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess ||
                          this.PageBoardContext.BoardSettings.ShowDeletedMessagesToAll;

        var findMessageId = this.GetFindMessageId(showDeleted, out var messagePosition, p, m);

        // Mark topic read
        this.Get<IReadTrackCurrentUser>().SetTopicRead(this.PageBoardContext.PageTopicID);
        this.Size = this.PageBoardContext.BoardSettings.PostsPerPage;

        var postList = this.GetRepository<Message>().PostListPaged(
            this.PageBoardContext.PageTopicID,
            this.PageBoardContext.PageUserID,
            !this.PageBoardContext.IsCrawler,
            showDeleted,
            DateTimeHelper.SqlDbMinTime(),
            DateTime.UtcNow,
            this.PageBoardContext.PageIndex,
            this.Size,
            messagePosition);

        if (postList.Count == 0)
        {
            var topicException = new NoPostsFoundForTopicException(
                this.PageBoardContext.PageTopicID,
                this.PageBoardContext.PageUserID,
                !this.PageBoardContext.IsCrawler,
                showDeleted,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                this.PageBoardContext.PageIndex,
                this.Size,
                messagePosition);

            this.Get<ILogger<PostsModel>>().Error(topicException, "No posts were found for topic");

            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        var firstPost = postList.FirstOrDefault();

        if (findMessageId > 0)
        {
            if (p != firstPost!.PageIndex)
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Posts,
                    new {
                        m = findMessageId, p = firstPost.PageIndex, name = this.PageBoardContext.PageTopic.TopicName,
                        t = this.PageBoardContext.PageTopic.ID
                    });
            }

            // move to this message on load...
            if (!this.PageBoardContext.IsCrawler && this.PageBoardContext.BoardSettings.ScrollToPost)
            {
                this.PageBoardContext.RegisterJsBlock(
                    JavaScriptBlocks.LoadGotoAnchor($"post{findMessageId}"));
            }
        }
        else
        {
            // move to this message on load...
            if (!this.PageBoardContext.IsCrawler && this.PageBoardContext.BoardSettings.ScrollToPost)
            {
                this.PageBoardContext.RegisterJsBlock(JavaScriptBlocks.LoadGotoAnchor($"post{firstPost!.MessageID}"));
            }
        }

        this.Messages = postList;

        this.Get<ISessionService>().SetPageData(this.Messages);

        return this.Page();
    }

    /// <summary>
    /// The delete topic.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteTopicAsync()
    {
        if (!this.PageBoardContext.ForumModeratorAccess)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        await this.GetRepository<Topic>().DeleteAsync(this.PageBoardContext.PageForumID, this.PageBoardContext.PageTopicID, true);

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Topics,
           new { f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name });
    }

    /// <summary>
    /// The email topic click.
    /// </summary>
    public IActionResult OnPostEmailTopic()
    {
        return this.PageBoardContext.IsGuest
                   ? this.PageBoardContext.Notify(this.GetText("WARN_EMAILLOGIN"), MessageTypes.warning)
                   : this.Get<ILinkBuilder>().Redirect(
                       ForumPages.EmailTopic,
                      new {t = this.PageBoardContext.PageTopicID});
    }

    /// <summary>
    /// Share topic on Reddit.
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostReddit()
    {
        var topicUrl = this.Get<ILinkBuilder>().GetLink(
            ForumPages.Posts,
            new {t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName});

        var redditUrl =
            $"https://www.reddit.com/submit?url={HttpUtility.UrlEncode(topicUrl)}&title={HttpUtility.UrlEncode(this.PageBoardContext.PageTopic.TopicName)}";

        return this.Redirect(redditUrl);
    }

    /// <summary>
    /// The lock topic click.
    /// </summary>
    public IActionResult OnPostLockTopic()
    {
        if (!this.PageBoardContext.ForumModeratorAccess)
        {
            // "You are not a forum moderator.
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        var flags = this.PageBoardContext.PageTopic.TopicFlags;

        flags.IsLocked = true;

        this.GetRepository<Topic>().Lock(this.PageBoardContext.PageTopicID, flags.BitValue);

        this.PageBoardContext.SessionNotify(this.GetText("INFO_TOPIC_LOCKED"), MessageTypes.info);

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Posts,
            new {t = this.PageBoardContext.PageTopic.ID, name = this.PageBoardContext.PageTopic.TopicName});    }

    /// <summary>
    /// The next topic click.
    /// </summary>
    public IActionResult OnPostNextTopic()
    {
        var nextTopic = this.GetRepository<Topic>().FindNext(this.PageBoardContext.PageTopic);

        if (nextTopic is null)
        {
            return this.PageBoardContext.Notify(this.GetText("INFO_NOMORETOPICS"), MessageTypes.info);
        }

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Posts,
            new { t = nextTopic.ID, name = nextTopic.TopicName });
    }

    /// <summary>
    /// Called when [get move topic dialog].
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetMoveTopic()
    {
        this.Get<IDataCache>().Set("TopicID", this.PageBoardContext.PageTopicID, TimeSpan.FromMinutes(5));

        return new PartialViewResult
               {
                   ViewName = "Dialogs/MoveTopic", ViewData = new ViewDataDictionary<MoveTopicModal>(this.ViewData, new MoveTopicModal())
               };
    }

    /// <summary>
    /// Gets the message ID
    /// </summary>
    /// <param name="showDeleted">
    /// The show Deleted.
    /// </param>
    /// <param name="messagePosition">
    /// The message Position.
    /// </param>
    /// <param name="p">
    /// The page index
    /// </param>
    /// <param name="m">
    /// The Message Id
    /// </param>
    /// <returns>
    /// The get find message id.
    /// </returns>
    private int GetFindMessageId(
        bool showDeleted,
        out int messagePosition,
        int? p = null,
        int? m = null)
    {
        var findMessageId = 0;
        messagePosition = -1;

        try
        {
            if (p.HasValue && !m.HasValue)
            {
                return findMessageId;
            }

            if (m.HasValue)
            {
                var unreadFirst = this.GetRepository<Message>().FindUnread(
                    this.PageBoardContext.PageTopicID,
                    m.Value,
                    DateTimeHelper.SqlDbMinTime(),
                    showDeleted);

                findMessageId = unreadFirst.MessageID;
                messagePosition = unreadFirst.MessagePosition;
            }
            else
            {
                if (!this.PageBoardContext.IsCrawler)
                {
                    // find first unread message
                    var lastRead = this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                        this.PageBoardContext.PageForumID,
                        this.PageBoardContext.PageTopicID,
                        null,
                        null);

                    var unreadFirst = this.GetRepository<Message>().FindUnread(
                        this.PageBoardContext.PageTopicID,
                        null,
                        lastRead,
                        showDeleted);

                    findMessageId = unreadFirst.MessageID;
                    messagePosition = unreadFirst.MessagePosition;

                    if (this.PageBoardContext.PageUser.Activity)
                    {
                        this.GetRepository<Activity>().UpdateNotification(this.PageBoardContext.PageUserID, findMessageId);
                    }
                }
            }
        }
        catch (Exception x)
        {
            this.Get<ILogger<PostsModel>>().Log(this.PageBoardContext.PageUserID, this, x);
        }

        return findMessageId;
    }

    /// <summary>
    /// The handle watch topic.
    /// </summary>
    /// <returns>
    /// Returns The handle watch topic.
    /// </returns>
    public async Task<bool> HandleWatchTopicAsync()
    {
        if (this.PageBoardContext.IsGuest)
        {
            return false;
        }

        var watchTopicId = await this.GetRepository<WatchTopic>().CheckAsync(
            this.PageBoardContext.PageUserID,
            this.PageBoardContext.PageTopicID);

        // check if this forum is being watched by this user
        return watchTopicId.HasValue;
    }
}