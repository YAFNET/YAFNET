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

namespace YAF.Tests.Helpers;

using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Objects.Model;

/// <summary>
/// The rules of a shown post (<see cref="PostDataHelperWrapper"/>): who may edit, delete, restore, attach,
/// reply and thank.
/// </summary>
[TestFixture]
[NonParallelizable]
public class PostDataHelperTests
{
    private const int Author = 2;

    private const int Other = 3;

    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();

        this.board.PageLoad.UserID = Author;
        this.board.PageLoad.EditAccess = true;
        this.board.PageLoad.DeleteAccess = true;
        this.board.PageLoad.UploadAccess = true;
        this.board.PageLoad.ReplyAccess = true;
        this.board.BoardSettings.LockPosts = 0;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Authors can edit, delete and attach to their own posts, not to the posts of others")]
    public void Own_Posts_Test()
    {
        var own = Post(Author);

        own.CanEditPost.Should().BeTrue();
        own.CanDeletePost.Should().BeTrue();
        own.CanAttach.Should().BeTrue();
        own.CanThankPost.Should().BeFalse("nobody thanks the own post");

        var other = Post(Other);

        other.CanEditPost.Should().BeFalse();
        other.CanDeletePost.Should().BeFalse();
        other.CanAttach.Should().BeFalse();
        other.CanThankPost.Should().BeTrue();
    }

    [Test]
    [Description("Without edit / delete / upload / reply access nothing is allowed, also on own posts")]
    public void No_Access_Test()
    {
        this.board.PageLoad.EditAccess = false;
        this.board.PageLoad.DeleteAccess = false;
        this.board.PageLoad.UploadAccess = false;
        this.board.PageLoad.ReplyAccess = false;
        this.board.PageLoad.ModeratorAccess = true;

        var own = Post(Author);

        own.CanEditPost.Should().BeFalse();
        own.CanDeletePost.Should().BeFalse();
        own.CanAttach.Should().BeFalse();
        own.CanReply.Should().BeFalse();
    }

    [TestCase(true, false, false)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    [Description("Locked posts, topics and forums can not be edited by the author, moderators still can")]
    public void Locked_Test(bool postLocked, bool topicLocked, bool forumLocked)
    {
        var post = Post(
            Author,
            new MessageFlags { IsLocked = postLocked, IsApproved = true }.BitValue,
            new TopicFlags { IsLocked = topicLocked }.BitValue,
            new ForumFlags { IsLocked = forumLocked }.BitValue);

        post.CanEditPost.Should().BeFalse();
        post.CanDeletePost.Should().BeFalse();
        post.CanAttach.Should().BeFalse();
        post.CanReply.Should().BeFalse();

        this.board.PageLoad.ModeratorAccess = true;

        post.CanEditPost.Should().BeTrue();
        post.CanDeletePost.Should().BeTrue();
        post.CanAttach.Should().BeTrue();
        post.CanReply.Should().BeTrue();
    }

    [Test]
    [Description("Posts are locked automatically after the lock period, not for admins")]
    public void Lock_Period_Test()
    {
        this.board.BoardSettings.LockPosts = 2;

        Post(Author, posted: DateTime.UtcNow.AddDays(-1)).PostLocked.Should().BeFalse();

        var old = Post(Author, posted: DateTime.UtcNow.AddDays(-3));
        old.PostLocked.Should().BeTrue();
        old.CanEditPost.Should().BeFalse();

        this.board.PageLoad.IsAdmin = true;
        old.PostLocked.Should().BeFalse();
    }

    [Test]
    [Description("Editing a post does not extend the lock period, it starts when the post was written")]
    public void Lock_Period_Starts_At_Posted_Test()
    {
        this.board.BoardSettings.LockPosts = 2;

        var post = Post(Author, posted: DateTime.UtcNow.AddDays(-3));
        post.DataRow.Edited = DateTime.UtcNow;

        post.PostLocked.Should().BeTrue();
    }

    [Test]
    [Description("Deleted posts can be restored by who may delete them, a post deleted by a moderator only by moderators")]
    public void Undelete_Test()
    {
        var deleted = Post(Author, new MessageFlags { IsDeleted = true, IsApproved = true }.BitValue);
        deleted.PostDeleted.Should().BeTrue();
        deleted.CanUnDeletePost.Should().BeTrue();

        deleted.DataRow.IsModeratorChanged = true;
        deleted.CanUnDeletePost.Should().BeFalse("a moderator deleted it");

        this.board.PageLoad.ModeratorAccess = true;
        deleted.CanUnDeletePost.Should().BeTrue();

        Post(Author).CanUnDeletePost.Should().BeFalse("the post is not deleted");
    }

    [Test]
    [Description("The flags and ids of the post row are read, an empty row has none")]
    public void Row_Values_Test()
    {
        var post = Post(Author, new MessageFlags { IsAnswer = true, IsLocked = true }.BitValue);

        post.PostIsAnswer.Should().BeTrue();
        post.IsLocked.Should().BeTrue();
        post.MessageId.Should().Be(7);
        post.TopicId.Should().Be(5);

        var empty = new PostDataHelperWrapper(null);
        empty.MessageId.Should().Be(0);
        empty.TopicId.Should().Be(0);
        empty.IsLocked.Should().BeFalse();
    }

    private static PostDataHelperWrapper Post(
        int userId,
        int messageFlags = 2,
        int topicFlags = 0,
        int forumFlags = 0,
        DateTime? posted = null)
    {
        return new PostDataHelperWrapper(
            new PagedMessage
            {
                MessageID = 7,
                TopicID = 5,
                UserID = userId,
                Flags = messageFlags,
                TopicFlags = topicFlags,
                ForumFlags = forumFlags,
                Posted = posted ?? DateTime.UtcNow,
                Edited = posted ?? DateTime.UtcNow
            });
    }
}
