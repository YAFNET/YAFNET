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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using ServiceStack.OrmLite;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models.Identity;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The control generates test data for different data layers.
/// </summary>
public class TestDataModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public TestDataInputModel Input { get; set; }

    public SelectList Categories { get; set; }

    public SelectList ForumsStartMasks { get; set; }

    public SelectList BoardList { get; set; }

    public SelectList ForumsParentList { get; set; }

    public SelectList TopicsForumList { get; set; }

    public SelectList PostsForumList { get; set; }

    public SelectList PostsTopicsList { get; set; }

    public List<SelectListItem> TopicPriorities { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestDataModel"/> class.
    /// </summary>
    public TestDataModel()
        : base("ADMIN_TESTDATA", ForumPages.Admin_TestData)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink("TEST DATA GENERATOR", string.Empty);

        this.PageTitle = "TEST DATA GENERATOR";
    }

    /// <summary>
    ///   The board create limit.
    /// </summary>
    private const int BoardCreateLimit = 100;

    /// <summary>
    ///   The category create limit.
    /// </summary>
    private const int CategoryCreateLimit = 100;

    /// <summary>
    ///   The create common limit.
    /// </summary>
    private const int CreateCommonLimit = 9999;

    /// <summary>
    ///   The Private Message prefix.
    /// </summary>
    private const string PMessagePrefix = "pmsg-";

    /// <summary>
    ///   The random GUID.
    /// </summary>
    private string randomGuid = Guid.NewGuid().ToString();

    /// <summary>
    /// The create test data click.
    /// </summary>
    public async Task<IActionResult> OnPostCreateTestDataAsync()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Test Data Generator reports: ");

        sb.AppendLine("Created:");

        sb.Append(await this.CreateUsersAsync());
        sb.Append(await this.CreateBoardsAsync());
        sb.Append(this.CreateCategories());
        sb.Append("; ");

        sb.AppendFormat("{0} Forums, ", this.CreateForums());

        sb.AppendFormat(
            "{0} Topics, ",
            this.CreateTopics(
                this.Input.TopicsForum,
                this.Input.TopicsNumber,
                this.Input.TopicsMessagesNumber));

        var topic = await this.GetRepository<Topic>().GetByIdAsync(this.Input.PostsTopic);

        sb.AppendFormat(
            "{0} Messages, ",
            this.CreatePosts(this.Input.PostsForum, topic, this.Input.PostsNumber));

        sb.AppendFormat("{0} Private Messages, ", await this.CreatePMessagesAsync());

        var mesRetStr = sb.ToString();

        this.Get<ILogger<TestDataModel>>().Log(this.PageBoardContext.PageUserID, this, mesRetStr, EventLogTypes.Information);

        return this.PageBoardContext.Notify(mesRetStr, MessageTypes.success);
    }

    /// <summary>
    /// The page load.
    /// </summary>
    public IActionResult OnGet()
    {
#if RELEASE
        return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
#endif
        this.Input = new TestDataInputModel {
                                        UsersBoardsList = this.PageBoardContext.PageBoardID,
                                        CategoriesBoardsList = this.PageBoardContext.PageBoardID,
                                        PMessagesBoardsList = this.PageBoardContext.PageBoardID,
                                        ForumsStartMask = this.PageBoardContext.BoardSettings.ForumDefaultAccessMask,
                                        From = this.PageBoardContext.PageUser.Name,
                                        To = this.PageBoardContext.PageUser.Name,
                                        TopicsPriorityList = 0
                                    };

        this.BindData();

        return this.Page();
    }

    public void OnPost()
    {
        this.BindData();
    }

    private void BindData()
    {
        this.Categories = new SelectList(
            this.GetRepository<Category>().List(),
            nameof(Category.ID),
            nameof(Category.Name));

        // Access Mask Lists
        this.ForumsStartMasks = new SelectList(
            this.GetRepository<AccessMask>().GetByBoardId(),
            nameof(AccessMask.ID),
            nameof(AccessMask.Name));

        // Board lists
        this.BoardList = new SelectList(this.GetRepository<Board>().GetAll(), nameof(Board.ID), nameof(Board.Name));

        this.TopicPriorities = [
            new SelectListItem("Normal", "0"),
            new SelectListItem("Sticky", "1"),
            new SelectListItem("Announcement", "2")
        ];

        if (this.Input.ForumsCategory != 0)
        {
            this.ForumsParentList = new SelectList(
                this.GetRepository<Forum>().ListAllFromCategory(this.Input.ForumsCategory),
                nameof(ForumSorted.ForumID),
                nameof(ForumSorted.Forum));
        }

        if (this.Input.TopicsCategory != 0)
        {
            this.TopicsForumList = new SelectList(
                this.GetRepository<Forum>().ListAllFromCategory(this.Input.TopicsCategory),
                nameof(ForumSorted.ForumID),
                nameof(ForumSorted.Forum));
        }

        if (this.Input.PostsCategory != 0)
        {
            this.PostsForumList = new SelectList(
                this.GetRepository<Forum>().ListAllFromCategory(this.Input.PostsCategory),
                nameof(ForumSorted.ForumID),
                nameof(ForumSorted.Forum));
        }

        if (this.Input.PostsForum != 0)
        {
            var topics = this.GetRepository<Topic>().ListPaged(
                this.Input.PostsForum,
                this.PageBoardContext.PageUserID,
                DateTimeHelper.SqlDbMinTime(),
                0,
                100,
                false);

            this.PostsTopicsList = new SelectList(topics, nameof(PagedTopic.TopicID), nameof(PagedTopic.Subject));
        }
    }

    /// <summary>
    /// The create boards.
    /// </summary>
    /// <returns>
    /// The number of created boards.
    /// </returns>
    private async Task<string> CreateBoardsAsync()
    {
        var boardNumber = this.Input.BoardNumber;
        var usersNumber = this.Input.BoardsUsersNumber;

        if (boardNumber <= 0)
        {
            return null;
        }

        if (usersNumber < 0)
        {
            return null;
        }

        if (boardNumber > BoardCreateLimit)
        {
            boardNumber = BoardCreateLimit;
        }

        int i;

        for (i = 0; i < boardNumber; i++)
        {
            var boardName = this.Input.BoardPrefixTB + Guid.NewGuid();

            var newBoardId = this.GetRepository<Board>().Create(
                boardName,
                this.PageBoardContext.BoardSettings.ForumEmail,
                "en-US",
                "english.json",
                this.PageBoardContext.PageUser.Name,
                this.PageBoardContext.PageUser.Email,
                this.PageBoardContext.PageUser.ProviderUserKey,
                this.PageBoardContext.PageUser.UserFlags.IsHostAdmin,
                string.Empty);

            await this.CreateUsersAsync(newBoardId, usersNumber);
        }

        return $"{i} Boards, {usersNumber} Users in each Board; ";
    }

    /// <summary>
    /// Create categories from Categories
    /// </summary>
    /// <returns>
    /// The create categories.
    /// </returns>
    private string CreateCategories()
    {
        const string NoCategories = "0 categories";

        var numForums = this.Input.CategoriesForumsNumber;
        var numTopics = this.Input.CategoriesTopicsNumber;
        var numMessages = this.Input.CategoriesMessagesNumber;
        var numCategories = this.Input.CategoriesNumber;

        if (numForums < 0)
        {
            return NoCategories;
        }

        if (numTopics < 0)
        {
            return NoCategories;
        }

        if (numMessages < 0)
        {
            return NoCategories;
        }

        switch (numCategories)
        {
            case <= 0:
                return NoCategories;
            case > CategoryCreateLimit:
                numCategories = CategoryCreateLimit;
                break;
        }

        return this.CreateCategoriesBase(
            this.Input.CategoriesBoardsList,
            numForums,
            numTopics,
            numMessages,
            numCategories);
    }

    /// <summary>
    /// The create categories base.
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="numForums">
    /// The num forums.
    /// </param>
    /// <param name="numTopics">
    /// The num topics.
    /// </param>
    /// <param name="numMessages">
    /// The num messages.
    /// </param>
    /// <param name="numCategories">
    /// The num categories.
    /// </param>
    /// <returns>
    /// The create categories base.
    /// </returns>
    private string CreateCategoriesBase(int boardId, int numForums, int numTopics, int numMessages, int numCategories)
    {
        int i;

        for (i = 0; i < numCategories; i++)
        {
            var catName = this.Input.CategoryPrefixTB + Guid.NewGuid();

            var categoryFlags = new CategoryFlags {IsActive = true};

            var newCategoryId = this.GetRepository<Category>().Save(null, catName, null, 100, categoryFlags, boardId);

            this.CreateForums(boardId, newCategoryId, null, numForums, numTopics, numMessages);
        }

        return $"{i} Categories, ";
    }

    /// <summary>
    /// Create forums from Forums page
    /// </summary>
    /// <returns>
    /// The create forums.
    /// </returns>
    private int CreateForums()
    {
        int? parentId = null;

        if (this.Input.ForumsParent > 0)
        {
            parentId = this.Input.ForumsParent;
        }

        var numTopics = this.Input.ForumsTopicsNumber;
        var numPosts = this.Input.ForumsMessagesNumber;
        var numForums = this.Input.ForumsNumber;

        var categoryId = this.Input.ForumsCategory;

        if (numTopics < 0)
        {
            return 0;
        }

        if (numPosts < 0)
        {
            return 0;
        }

        switch (numForums)
        {
            case <= 0:
                return 0;
            case > CreateCommonLimit:
                numForums = CreateCommonLimit;
                break;
        }

        return this.CreateForums(
            this.PageBoardContext.PageBoardID,
            categoryId,
            parentId,
            numForums,
            numTopics,
            numPosts);
    }

    /// <summary>
    /// Create forums from Categories
    /// </summary>
    /// <param name="boardId">
    /// The Board ID
    /// </param>
    /// <param name="categoryId">
    /// </param>
    /// <param name="parentId">
    /// The parent ID.
    /// </param>
    /// <param name="numForums">
    /// The num Forums.
    /// </param>
    /// <param name="topicsToCreate">
    /// Number of topics to create.
    /// </param>
    /// <param name="messagesToCreate">
    /// Number of messages to create.
    /// </param>
    /// <returns>
    /// The create forums.
    /// </returns>
    private int CreateForums(
        int boardId,
        int categoryId,
        int? parentId,
        int numForums,
        int topicsToCreate,
        int messagesToCreate)
    {
        var countMessagesInStatistics = this.Input.ForumsCountMessages;

        int forums;

        for (forums = 0; forums < numForums; forums++)
        {
            this.randomGuid = Guid.NewGuid().ToString();

            var newForumId = this.GetRepository<Forum>().Save(
                null,
                categoryId,
                parentId,
                this.Input.ForumPrefixTB + this.randomGuid,
                $"Description of {this.Input.ForumPrefixTB}{this.randomGuid}",
                100,
                false,
                true,
                countMessagesInStatistics,
                false,
                null,
                false,
                null,
                null,
                null,
                null);

            var groups = this.GetRepository<Group>().List(null, boardId);

            var mask = this.GetRepository<AccessMask>().GetSingle(
                m => m.BoardID == boardId && m.ID == this.Input.ForumsStartMask);

            groups.ForEach(group => this.GetRepository<ForumAccess>().Create(newForumId, group.ID, mask.ID));

            if (topicsToCreate <= 0)
            {
                continue;
            }

            this.CreateTopics(newForumId, topicsToCreate, messagesToCreate);
        }

        return forums;
    }

    /// <summary>
    /// The create p messages.
    /// </summary>
    /// <returns>
    /// The number of created p messages.
    /// </returns>
    private async Task<int> CreatePMessagesAsync()
    {
        var numPMessages = this.Input.PMessagesNumber;

        if (numPMessages <= 0)
        {
            return 0;
        }

        var fromUser = await this.GetRepository<User>().GetSingleAsync(u => u.Name == this.Input.From);

        var toUser = await this.GetRepository<User>().GetSingleAsync(u => u.Name == this.Input.To);

        if (fromUser is null)
        {
            this.PageBoardContext.Notify("You should enter valid 'from' user name.", MessageTypes.warning);
            return 0;
        }

        if (toUser is null)
        {
            this.PageBoardContext.Notify("You should enter valid 'to' user name.", MessageTypes.warning);

            return 0;
        }

        if (numPMessages > CreateCommonLimit)
        {
            numPMessages = CreateCommonLimit;
        }

        int i;
        for (i = 0; i < numPMessages; i++)
        {
            this.randomGuid = Guid.NewGuid().ToString();

            _ = await this.GetRepository<PrivateMessage>().InsertAsync(
                new PrivateMessage
                {
                    Created = DateTime.UtcNow,
                    Flags = 0,
                    FromUserId = fromUser.ID,
                    ToUserId = toUser.ID,
                    Body = $"{PMessagePrefix}{this.randomGuid}   {this.Input.PMessageText}"
                });
        }

        if (!this.Input.MarkRead)
        {
            return i;
        }

        await BoardContext.Current.GetRepository<PrivateMessage>().DbAccess.ExecuteAsync(
            db =>
            {
                var updateExpression = OrmLiteConfig.DialectProvider.SqlExpression<PrivateMessage>();

                return db.ExecuteSqlAsync(
                    $" update {updateExpression.Table<PrivateMessage>()} set Flags = Flags | 1 where ToUserId = {toUser.ID}");
            });

        // Clearing cache with old permissions data...
        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.ActiveUserLazyData, toUser.ID));

        return i;
    }

    /// <summary>
    /// The create posts.
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <param name="numMessages">
    /// The num messages.
    /// </param>
    /// <returns>
    /// The number of created posts.
    /// </returns>
    private int CreatePosts(int forumId, Topic topic, int numMessages)
    {
        var forum = this.GetRepository<Forum>().GetById(forumId);

        if (numMessages <= 0)
        {
            return 0;
        }

        if (forumId <= 0)
        {
            return 0;
        }

        if (topic.ID <= 0)
        {
            return 0;
        }

        int posts;

        const int replyTo = -1;

        for (posts = 0; posts < numMessages; posts++)
        {
            this.randomGuid = Guid.NewGuid().ToString();

            this.GetRepository<Message>().SaveNew(
                forum,
                topic,
                this.PageBoardContext.PageUser,
                $"msgd-{this.randomGuid}  {this.Input.MyMessage}",
                this.PageBoardContext.PageUser.Name,
                this.Request.GetUserRealIPAddress(),
                DateTime.UtcNow,
                replyTo,
                this.GetMessageFlags());
        }

        return posts;
    }

    /// <summary>
    /// The create topics.
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="numTopics">
    /// The num topics.
    /// </param>
    /// <param name="messagesToCreate">
    /// The _messages to create.
    /// </param>
    /// <returns>
    /// Number of created topics.
    /// </returns>
    private int CreateTopics(int forumId, int numTopics, int messagesToCreate)
    {
        var forum = this.GetRepository<Forum>().GetById(forumId);

        var priority = forumId <= 0 ? this.Input.TopicsPriorityList : 0;

        if (numTopics <= 0)
        {
            return 0;
        }

        if (messagesToCreate < 0)
        {
            return 0;
        }

        var topicName = this.Input.TopicPrefixTB + this.randomGuid;

        int topics;

        for (topics = 0; topics < numTopics; topics++)
        {
            this.randomGuid = Guid.NewGuid().ToString();

            var topic = this.GetRepository<Topic>().SaveNew(
                forum,
                topicName,
                string.Empty,
                string.Empty,
                $"{this.Input.TopicPrefixTB}{this.randomGuid}descr",
                $"{this.Input.MessageContentPrefixTB}{this.randomGuid}",
                this.PageBoardContext.PageUser,
                priority.ToType<short>(),
                this.PageBoardContext.PageUser.Name,
                this.PageBoardContext.PageUser.DisplayName,
                this.Request.GetUserRealIPAddress(),
                DateTime.UtcNow,
                this.GetMessageFlags(),
                out _);

            if (this.Input.PollCreate)
            {
                var pollId = this.GetRepository<Poll>().Create(
                    this.PageBoardContext.PageUserID,
                    $"quest-{this.randomGuid}",
                    null,
                    false,
                    false,
                    true,
                    null);

                this.GetRepository<Choice>().AddChoice(pollId, $"ans1-{this.randomGuid}", null);
                this.GetRepository<Choice>().AddChoice(pollId, $"ans2-{this.randomGuid}", null);
                this.GetRepository<Choice>().AddChoice(pollId, $"ans3-{this.randomGuid}", null);

                // Attach Poll to topic
                this.GetRepository<Topic>().AttachPoll(topic.ID, pollId);
            }

            if (messagesToCreate > 0)
            {
                this.CreatePosts(forumId, topic, messagesToCreate);
            }
        }

        return topics;
    }

    /// <summary>
    /// From Users Tab
    /// </summary>
    /// <returns>
    /// The create users.
    /// </returns>
    private async Task<string> CreateUsersAsync()
    {
        var usersNumber = this.Input.UsersNumber;

        return usersNumber <= 0
                   ? null
                   : await this.CreateUsersAsync(this.Input.UsersBoardsList, usersNumber);
    }

    /// <summary>
    /// The create users.
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="countLimit">
    /// The count limit.
    /// </param>
    /// <returns>
    /// The string with number of created users.
    /// </returns>
    private async Task<string> CreateUsersAsync(int boardId, int countLimit)
    {
        var outCounter = 0;

        for (var i = 0; i < countLimit; i++)
        {
            this.randomGuid = Guid.NewGuid().ToString();
            var newEmail = $"{this.Input.UserPrefixTB}{this.randomGuid}@test.info";
            var newUsername = this.Input.UserPrefixTB + this.randomGuid;

            if (this.Get<IAspNetUsersHelper>().UserExists(newUsername, newEmail))
            {
                continue;
            }

            var user = new AspNetUsers {
                Id = Guid.NewGuid().ToString(),
                ApplicationId = this.Get<BoardSettings>().ApplicationId,
                UserName = newUsername,
                LoweredUserName = newUsername.ToLower(),
                Email = newEmail,
                LoweredEmail = newEmail.ToLower(),
                IsApproved = true,
                EmailConfirmed = true
            };

            var result = await this.Get<IAspNetUsersHelper>().CreateUserAsync(user, this.Input.Password);

            if (!result.Succeeded)
            {
                this.PageBoardContext.Notify(result.Errors.FirstOrDefault()?.Description, MessageTypes.warning);

                continue;
            }

            // setup initial roles (if any) for this user
            await this.Get<IAspNetRolesHelper>().SetupUserRolesAsync(boardId, user);

            // create the user in the YAF DB as well as sync roles...
            await this.Get<IAspNetRolesHelper>().CreateForumUserAsync(user, newUsername, boardId);

            outCounter++;
        }

        return $"{outCounter} Users in {boardId} Board(s); ";
    }

    /// <summary>
    /// The get message flags.
    /// </summary>
    /// <returns>
    /// The method returns message flags.
    /// </returns>
    private MessageFlags GetMessageFlags()
    {
        var messageFlags = new MessageFlags {
                                                IsHtml = false, IsBBCode = true, IsPersistent = false,
                                                IsApproved = this.PageBoardContext.IsAdmin
                                            };

        // Bypass Approval if Admin or Moderator.
        return messageFlags;
    }
}