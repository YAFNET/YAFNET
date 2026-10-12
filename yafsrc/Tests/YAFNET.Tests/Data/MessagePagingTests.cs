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

namespace YAF.Tests.Data;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The posts page queries: PostListPaged and FindUnread.
/// </summary>
[TestFixture]
[NonParallelizable]
public class MessagePagingTests
{
    private static readonly DateTime Since = DateTimeHelper.SqlDbMinTime();

    private readonly DateTime start = DateTime.UtcNow.AddDays(-1);

    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.topic = this.db.AddTopic(this.db.AddForum("Forum"), this.alice, "Topic");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Posts are listed oldest first with author, rank and topic data")]
    public void PostListPaged_Content_Test()
    {
        var first = this.AddPost(this.alice, 0);
        var second = this.AddPost(this.bob, 1);

        var posts = this.List(pageSize: 10);

        posts.Select(p => p.MessageID).Should().Equal(first, second);
        posts.Should().OnlyContain(p => p.TotalRows == 2 && p.PageIndex == 0);

        var post = posts[1];
        post.Message.Should().Be("Post 1");
        post.UserName.Should().Be("Bob");
        post.RankName.Should().Be("Member");
        post.Topic.Should().Be("Topic");
        post.ForumName.Should().Be("Forum");
        post.Position.Should().Be(1);
        post.IsDeleted.Should().BeFalse();
    }

    [Test]
    [Description("Deleted posts are only listed when requested, unapproved posts never")]
    public void PostListPaged_Deleted_And_Unapproved_Test()
    {
        var visible = this.AddPost(this.alice, 0);
        var deleted = this.AddPost(this.alice, 1, new MessageFlags { IsApproved = true, IsDeleted = true });
        this.AddPost(this.alice, 2, new MessageFlags { IsApproved = false });

        this.List(showDeleted: false).Select(p => p.MessageID).Should().Equal(visible);

        var withDeleted = this.List(showDeleted: true);

        withDeleted.Select(p => p.MessageID).Should().Equal(visible, deleted);
        withDeleted.Single(p => p.MessageID == deleted).IsDeleted.Should().BeTrue();
        withDeleted.Should().OnlyContain(p => p.TotalRows == 2);
    }

    [Test]
    [Description("Opening a page counts a view only when requested")]
    public void PostListPaged_ViewCount_Test()
    {
        this.AddPost(this.alice, 0);

        this.List(updateViewCount: true);
        this.List(updateViewCount: true);
        this.List(updateViewCount: false);

        this.board.Current.GetRepository<Topic>().GetById(this.topic).Views.Should().Be(2);
    }

    [Test]
    [Description("Thanks and reputation votes of the current user are included")]
    public void PostListPaged_Thanks_And_Reputation_Test()
    {
        var message = this.AddPost(this.bob, 0);
        var voteDate = DateTime.UtcNow.AddHours(-2);

        using (var connection = this.db.Open())
        {
            connection.Insert(new Thanks { ThanksFromUserID = this.alice, ThanksToUserID = this.bob, MessageID = message, ThanksDate = DateTime.UtcNow });
            connection.Insert(new Thanks { ThanksFromUserID = this.db.AddUser("Carol"), ThanksToUserID = this.bob, MessageID = message, ThanksDate = DateTime.UtcNow });
            connection.Insert(new ReputationVote { ReputationFromUserID = this.alice, ReputationToUserID = this.bob, VoteDate = voteDate });
        }

        var post = this.List(userId: this.alice).Single();

        post.ThanksNumber.Should().Be(2);
        post.IsThankedByUser.Should().BeTrue();
        post.ReputationVoteDate.Should().BeCloseTo(voteDate, TimeSpan.FromSeconds(1));

        this.List(userId: this.bob).Single().IsThankedByUser.Should().BeFalse();
    }

    [Test]
    [Description("Pages are cut by the page size and a too high page shows the last page")]
    public void PostListPaged_Paging_Test()
    {
        var ids = Enumerable.Range(0, 25).Select(i => this.AddPost(this.alice, i)).ToList();

        this.List(pageIndex: 1, pageSize: 10).Select(p => p.MessageID).Should().Equal(ids.Skip(10).Take(10));
        this.List(pageIndex: 1, pageSize: 10).Should().OnlyContain(p => p.TotalRows == 25);

        this.List(pageIndex: 9, pageSize: 10).Select(p => p.MessageID).Should().Equal(ids.Skip(20));
    }

    [TestCase(5, 0, 0)]
    [TestCase(9, 0, 0)]
    [TestCase(10, 1, 2)]
    [TestCase(15, 1, 2)]
    [TestCase(25, 2, 3)]
    [TestCase(34, 3, 4)]
    [Description("Jumping to a message shows the page that contains it; PageIndex is the page for the URL")]
    public void PostListPaged_Jump_To_Position_Test(int position, int expectedPage, int expectedUrlPage)
    {
        var ids = Enumerable.Range(0, 35).Select(i => this.AddPost(this.alice, i)).ToList();

        var posts = this.List(pageSize: 10, messagePosition: position);

        posts.Select(p => p.MessageID).Should().Contain(ids[position]);
        posts.Select(p => p.MessageID).Should().Equal(ids.Skip(expectedPage * 10).Take(10));
        posts.Should().OnlyContain(p => p.PageIndex == expectedUrlPage);
    }

    [Test]
    [Description("The posted date range limits the posts")]
    public void PostListPaged_Date_Range_Test()
    {
        this.AddPost(this.alice, 0);
        var inRange = this.AddPost(this.alice, 1);
        this.AddPost(this.alice, 2);

        this.board.Current.GetRepository<Message>()
            .PostListPaged(this.topic, this.alice, false, false, this.start.AddMinutes(0.5), this.start.AddMinutes(1.5), 0, 10, 0)
            .Select(p => p.MessageID).Should().Equal(inRange);
    }

    [Test]
    [Description("FindUnread by message ID returns that message's position")]
    public void FindUnread_By_Message_Test()
    {
        this.AddPost(this.alice, 0);
        var second = this.AddPost(this.alice, 1);
        var deleted = this.AddPost(this.alice, 2, new MessageFlags { IsApproved = true, IsDeleted = true });
        var last = this.AddPost(this.alice, 3);

        this.Messages.FindUnread(this.topic, second, Since, false).Should().Be((1, second));
        this.Messages.FindUnread(this.topic, deleted, Since, true).Should().Be((2, deleted));

        // a deleted message the user can't see: falls back to the first post
        this.Messages.FindUnread(this.topic, deleted, Since, false).MessagePosition.Should().Be(0);

        // a message of another topic: falls back as well
        this.Messages.FindUnread(this.topic, -1, this.start.AddMinutes(10), false).Should().Be((3, last));
    }

    [Test]
    [Description("FindUnread finds the first post after the last visit, or the last post")]
    public void FindUnread_By_Last_Read_Test()
    {
        var first = this.AddPost(this.alice, 0);
        this.AddPost(this.alice, 1);
        var third = this.AddPost(this.alice, 2);
        this.AddPost(this.alice, 3);
        var last = this.AddPost(this.alice, 4);

        // read up to the second post: the third one is the first unread
        this.Messages.FindUnread(this.topic, null, this.start.AddMinutes(1.5), false).Should().Be((2, third));

        // everything read: jump to the last post
        this.Messages.FindUnread(this.topic, null, this.start.AddDays(1), false).Should().Be((4, last));

        // never read: the first post
        this.Messages.FindUnread(this.topic, null, Since, false).Should().Be((0, first));
    }

    private IRepository<Message> Messages => this.board.Current.GetRepository<Message>();

    /// <summary>
    /// Adds a post at position <paramref name="position"/>, posted <paramref name="position"/> minutes after the start.
    /// </summary>
    private int AddPost(int userId, int position, MessageFlags flags = null)
    {
        return this.db.AddMessage(
            this.topic,
            userId,
            $"Post {position}",
            posted: this.start.AddMinutes(position),
            configure: m =>
            {
                m.Position = position;
                m.UserName = userId == this.alice ? "Alice" : "Bob";

                if (flags != null)
                {
                    m.Flags = flags.BitValue;
                }
            });
    }

    private List<PagedMessage> List(
        int? userId = null,
        bool updateViewCount = false,
        bool showDeleted = false,
        int pageIndex = 0,
        int pageSize = 20,
        int messagePosition = 0)
    {
        return this.Messages.PostListPaged(
            this.topic,
            userId ?? this.alice,
            updateViewCount,
            showDeleted,
            Since,
            DateTime.UtcNow,
            pageIndex,
            pageSize,
            messagePosition);
    }
}
