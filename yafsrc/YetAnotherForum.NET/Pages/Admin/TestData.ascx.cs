/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

#if DEBUG
namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Linq;
    using System.Text;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;
    using YAF.Web.Extensions;

    using ListItem = System.Web.UI.WebControls.ListItem;

    #endregion

    /// <summary>
    /// The control generates test data for different data layers.
    /// </summary>
    public partial class TestData : AdminPage
    {
        #region Constants and Fields

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

        #endregion

        #region Methods

        /// <summary>
        /// The cancel click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_TestData);
        }

        /// <summary>
        /// The create test data click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CreateTestData_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine("Test Data Generator reports: ");

            sb.AppendLine("Created:");

            sb.Append(this.CreateUsers());
            sb.Append(this.CreateBoards());
            sb.Append(this.CreateCategories());
            sb.Append("; ");

            sb.AppendFormat("{0} Forums, ", this.CreateForums());

            sb.AppendFormat(
                "{0} Topics, ",
                this.CreateTopics(
                    this.TopicsForum.SelectedValue.ToType<int>(),
                    this.TopicsNumber.Text.ToType<int>(),
                    this.TopicsMessagesNumber.Text.ToType<int>()));

            sb.AppendFormat(
                "{0} Messages, ",
                this.CreatePosts(
                    this.PostsForum.Text.ToType<int>(),
                    this.PostsTopic.SelectedValue.ToType<int>(),
                    this.PostsNumber.Text.ToType<int>()));

            sb.AppendFormat("{0} Private Messages, ", this.CreatePMessages());

            var mesRetStr = sb.ToString();

            this.Logger.Log(this.PageContext.PageUserID, this, mesRetStr, EventLogTypes.Information);

            this.PageContext.AddLoadMessage(mesRetStr, MessageTypes.success);
        }

        /// <summary>
        /// The forums category on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ForumsCategory_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var forums_category = this.GetRepository<Forum>().ListAllFromCategory(
                this.ForumsCategory.SelectedValue.ToType<int>());

            this.ForumsParent.DataSource = forums_category;
            this.ForumsParent.DataBind();
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and Jquery Ui Tabs.
            this.PageContext.PageElements.RegisterJsBlock(
                "yafPmTabsJs",
                JavaScriptBlocks.BootstrapTabsLoadJs(this.TestDataTabs.ClientID, this.hidLastTab.ClientID));

            base.OnPreRender(e);
        }

        /// <summary>
        /// The page load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink("TEST DATA GENERATOR", string.Empty);

            var categories = this.GetRepository<Category>().List();

            this.ForumsCategory.DataSource = categories;
            this.TopicsCategory.DataSource = categories;
            this.PostsCategory.DataSource = categories;

            // Access Mask Lists
            this.ForumsStartMask.DataSource = this.GetRepository<AccessMask>().GetByBoardId();

            // Board lists
            this.UsersBoardsList.DataSource = this.GetRepository<Board>().GetAll();
            this.UsersBoardsList.SelectedValue = this.PageContext.PageBoardID.ToString();

            this.CategoriesBoardsList.DataSource = this.UsersBoardsList.DataSource;
            this.CategoriesBoardsList.SelectedValue = this.PageContext.PageBoardID.ToString();

            this.PMessagesBoardsList.DataSource = this.UsersBoardsList.DataSource;
            this.PMessagesBoardsList.SelectedValue = this.PageContext.PageBoardID.ToString();

            this.DataBind();

            if (this.ForumsStartMask.Items.Count > 1)
            {
                this.ForumsStartMask.SelectedIndex = 2;
            }

            this.TopicsCategory.ClearSelection();
            this.PostsCategory.ClearSelection();

            this.ForumsCategory.SelectedIndex = -1;

            this.From.Text = this.PageContext.User.Name;
            this.To.Text = this.PageContext.User.Name;

            this.TopicsPriorityList.Items.Add(new ListItem("Normal", "0"));
            this.TopicsPriorityList.Items.Add(new ListItem("Sticky", "1"));
            this.TopicsPriorityList.Items.Add(new ListItem("Announcement", "2"));

            this.TopicsPriorityList.SelectedIndex = 0;
        }

        /// <summary>
        /// The posts category_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PostsCategory_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var posts_category = this.GetRepository<Forum>().ListAllFromCategory(
                this.PostsCategory.SelectedValue.ToType<int>());

            this.PostsForum.DataSource = posts_category;
            this.PostsForum.DataBind();
        }

        /// <summary>
        /// The posts forum_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PostsForum_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var topics = this.GetRepository<Topic>().ListPaged(
                this.PostsForum.SelectedValue.ToType<int>(),
                this.PageContext.PageUserID,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                0,
                100,
                false,
                false);

            this.PostsTopic.DataSource = topics;
            this.PostsTopic.DataBind();
        }

        /// <summary>
        /// The topics category_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void TopicsCategory_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var topic_forums = this.GetRepository<Forum>().ListAllFromCategory(
                this.TopicsCategory.SelectedValue.ToType<int>());

            this.TopicsForum.DataSource = topic_forums;
            this.TopicsForum.DataBind();
        }

        /// <summary>
        /// The create boards.
        /// </summary>
        /// <returns>
        /// The number of created boards.
        /// </returns>
        private string CreateBoards()
        {
            var boardNumber = this.BoardNumber.Text.ToType<int>();
            var usersNumber = this.BoardsUsersNumber.Text.ToType<int>();

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
                var boardName = this.BoardPrefixTB.Text.Trim() + Guid.NewGuid();

                var newBoardId = this.GetRepository<Board>().Create(
                    boardName,
                    this.PageContext.BoardSettings.ForumEmail,
                    "en-US",
                    "english.xml",
                    this.PageContext.User.Name,
                    this.PageContext.User.Email,
                    this.PageContext.User.ProviderUserKey,
                    this.PageContext.User.UserFlags.IsHostAdmin,
                    Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty);

                this.CreateUsers(newBoardId, usersNumber);
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

            var numForums = this.CategoriesForumsNumber.Text.ToType<int>();
            var numTopics = this.CategoriesTopicsNumber.Text.ToType<int>();
            var numMessages = this.CategoriesMessagesNumber.Text.ToType<int>();
            var numCategories = this.CategoriesNumber.Text.ToType<int>();

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
                this.CategoriesBoardsList.SelectedValue.ToType<int>(),
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
        private string CreateCategoriesBase(
            int boardId,
            int numForums,
            int numTopics,
            int numMessages,
            int numCategories)
        {
            int i;

            for (i = 0; i < numCategories; i++)
            {
                var catName = this.CategoryPrefixTB.Text.Trim() + Guid.NewGuid();

                var newCategoryId = this.GetRepository<Category>().Save(null, catName, null, 100, boardId);

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
            int? parentID = null;
            if (int.TryParse(this.ForumsParent.Text.Trim(), out var parentIDInt))
            {
                parentID = parentIDInt;
            }

            var numTopics = this.ForumsTopicsNumber.Text.ToType<int>();
            var numPosts = this.ForumsMessagesNumber.Text.ToType<int>();
            var numForums = this.ForumsNumber.Text.ToType<int>();

            var categoryID = this.ForumsCategory.SelectedValue.ToType<int>();

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
                this.PageContext.PageBoardID,
                categoryID,
                parentID,
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
            [NotNull] int boardId,
            [NotNull] int categoryId,
            [CanBeNull] int? parentId,
            [NotNull] int numForums,
            [NotNull] int topicsToCreate,
            [NotNull] int messagesToCreate)
        {
            var countMessagesInStatistics = this.ForumsCountMessages.Checked;

            int forums;

            for (forums = 0; forums < numForums; forums++)
            {
                this.randomGuid = Guid.NewGuid().ToString();

                var newForumId = this.GetRepository<Forum>().Save(
                    null,
                    categoryId,
                    parentId,
                    this.ForumPrefixTB.Text.Trim() + this.randomGuid,
                    $"Description of {this.ForumPrefixTB.Text.Trim()}{this.randomGuid}",
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
                    m => m.BoardID == boardId && m.Name == this.ForumsStartMask.SelectedItem.Text);

                groups.ForEach(
                    group => this.GetRepository<ForumAccess>().Create(
                        newForumId,
                        group.ID,
                        mask.ID));


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
        private int CreatePMessages()
        {
            var numPMessages = this.PMessagesNumber.Text.ToType<int>();

            if (numPMessages <= 0)
            {
                return 0;
            }

            var fromUser = this.GetRepository<User>().GetSingle(u => u.Name == this.From.Text.Trim());

            var toUser = this.GetRepository<User>().GetSingle(u => u.Name == this.To.Text.Trim());

            if (fromUser == null)
            {
                this.PageContext.AddLoadMessage("You should enter valid 'from' user name.", MessageTypes.warning);
                return 0;
            }

            if (toUser == null)
            {
                this.PageContext.AddLoadMessage("You should enter valid 'to' user name.", MessageTypes.warning);

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

                var messageFlags = new MessageFlags { IsHtml = false, IsBBCode = true };

                this.GetRepository<PMessage>().SendMessage(
                    fromUser.ID,
                    toUser.ID,
                    this.TopicPrefixTB.Text.Trim() + this.randomGuid,
                    $"{PMessagePrefix}{this.randomGuid}   {this.PMessageText.Text.Trim()}",
                    messageFlags.BitValue,
                    -1);
            }

            if (!this.MarkRead.Checked)
            {
                return i;
            }

            this.GetRepository<UserPMessage>().Get(m => m.UserID == toUser.ID).ForEach(
                x => this.GetRepository<UserPMessage>().MarkAsRead(x.PMessageID, new PMessageFlags(x.Flags)));

            // Clearing cache with old permissions data...
            this.Get<IDataCache>().Remove(string.Format(Constants.Cache.ActiveUserLazyData, toUser.ID));

            return i;
        }

        /// <summary>
        /// The create posts.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="numMessages">
        /// The num messages.
        /// </param>
        /// <returns>
        /// The number of created posts.
        /// </returns>
        private int CreatePosts(int forumID, int topicID, int numMessages)
        {
            if (numMessages <= 0)
            {
                return 0;
            }

            if (forumID <= 0)
            {
                return 0;
            }

            if (topicID <= 0)
            {
                return 0;
            }

            int posts;

            const int ReplyTo = -1;

            for (posts = 0; posts < numMessages; posts++)
            {
                this.randomGuid = Guid.NewGuid().ToString();

                this.GetRepository<Message>().SaveNew(
                    forumID,
                    topicID,
                    this.PageContext.PageUserID,
                    $"msgd-{this.randomGuid}  {this.MyMessage.Text.Trim()}",
                    this.PageContext.User.Name,
                    this.Request.GetUserRealIPAddress(),
                    DateTime.UtcNow,
                    ReplyTo,
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
            var priority = forumId <= 0 ? this.TopicsPriorityList.SelectedValue.ToType<short>() : (short)0;

            if (numTopics <= 0)
            {
                return 0;
            }

            if (messagesToCreate < 0)
            {
                return 0;
            }

            int topics;
            for (topics = 0; topics < numTopics; topics++)
            {
                this.randomGuid = Guid.NewGuid().ToString();

                var topicId = this.GetRepository<Topic>().SaveNew(
                    forumId,
                    this.TopicPrefixTB.Text.Trim() + this.randomGuid,
                    string.Empty,
                    string.Empty,
                    $"{this.TopicPrefixTB.Text.Trim()}{this.randomGuid}descr",
                    $"{this.MessageContentPrefixTB.Text.Trim()}{this.randomGuid}",
                    this.PageContext.PageUserID,
                    priority,
                    this.PageContext.User.Name,
                    this.PageContext.User.DisplayName,
                    this.Request.GetUserRealIPAddress(),
                    DateTime.UtcNow,
                    this.GetMessageFlags(),
                    null,
                    out _);

                if (this.PollCreate.Checked)
                {
                    var pollId = this.GetRepository<Poll>().Create(
                        this.PageContext.PageUserID,
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
                    this.GetRepository<Topic>().AttachPoll(topicId, pollId);
                }

                if (messagesToCreate > 0)
                {
                    this.CreatePosts(forumId, topicId, messagesToCreate);
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
        private string CreateUsers()
        {
            var usersNumber = this.UsersNumber.Text.ToType<int>();

            return usersNumber <= 0
                ? null
                : this.CreateUsers(this.UsersBoardsList.SelectedValue.ToType<int>(), usersNumber);
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
        private string CreateUsers(int boardId, int countLimit)
        {
            var outCounter = 0;

            for (var i = 0; i < countLimit; i++)
            {
                this.randomGuid = Guid.NewGuid().ToString();
                var newEmail = $"{this.UserPrefixTB.Text.Trim()}{this.randomGuid}@test.info";
                var newUsername = this.UserPrefixTB.Text.Trim() + this.randomGuid;

                if (this.Get<IAspNetUsersHelper>().UserExists(newUsername, newEmail))
                {
                    continue;
                }

                var user = new AspNetUsers
                {
                    Id = Guid.NewGuid().ToString(),
                    ApplicationId = this.Get<BoardSettings>().ApplicationId,
                    UserName = newUsername,
                    LoweredUserName = newUsername.ToLower(),
                    Email = newEmail,
                    IsApproved = true,
                    EmailConfirmed = true
                };

                var result = this.Get<IAspNetUsersHelper>().Create(user, this.Password.Text.Trim());

                if (!result.Succeeded)
                {
                    this.PageContext.AddLoadMessage(result.Errors.FirstOrDefault(), MessageTypes.warning);

                    continue;
                }

                // setup initial roles (if any) for this user
                this.Get<IAspNetRolesHelper>().SetupUserRoles(boardId, user);

                // create the user in the YAF DB as well as sync roles...
                this.Get<IAspNetRolesHelper>().CreateForumUser(
                    user,
                    newUsername,
                    boardId);

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
            var messageFlags = new MessageFlags
            {
                IsHtml = false, IsBBCode = true, IsPersistent = false, IsApproved = this.PageContext.IsAdmin
            };

            // Bypass Approval if Admin or Moderator.
            return messageFlags;
        }

        #endregion
    }
}

#endif