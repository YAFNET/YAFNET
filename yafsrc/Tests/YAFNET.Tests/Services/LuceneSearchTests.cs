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

namespace YAF.Tests.Services;

using System.IO;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Services;
using YAF.Types.Objects;

/// <summary>
/// The real Lucene <see cref="Search"/> service against an index in a temp web root, with forum
/// access read from the access views of the in-memory database.
/// </summary>
[TestFixture]
[NonParallelizable]
public class LuceneSearchTests
{
    private string webRoot;

    private TestBoardContext board;

    private int alice;

    private int eve;

    private int forumA;

    private int forumB;

    private int hiddenForum;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-search-{Guid.NewGuid():N}");
        Directory.CreateDirectory(this.webRoot);

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        var formatMessage = Substitute.For<IFormatMessage>();
        formatMessage.FormatMessageWithAllBBCodesAsync(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int?>(),
                Arg.Any<bool?>(),
                Arg.Any<DateTime?>())
            .Returns(c => Task.FromResult(c.ArgAt<string>(0)));

        // the real removal of hidden content (it is not indexed)
        formatMessage.RemoveHiddenBBCodeContent(Arg.Any<string>())
            .Returns(c => new FormatMessage(null, null).RemoveHiddenBBCodeContent(c.ArgAt<string>(0)));

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterInstance(formatMessage).As<IFormatMessage>();
                b.RegisterInstance(Substitute.For<IDateTimeService>()).As<IDateTimeService>();
            },
            useDatabase: true);

        this.board.BoardSettings.ShowRelativeTime = false;
        this.board.BoardSettings.ReturnSearchMax = 100;

        var db = this.board.Database;

        this.alice = db.AddUser("Alice");
        this.eve = db.AddUser("Eve");

        var category = db.AddCategory();
        this.forumA = db.AddForum("Forum A", category);
        this.forumB = db.AddForum("Forum B", category);
        this.hiddenForum = db.AddForum("Staff", category);

        this.board.GrantAccess(this.alice, this.forumA);
        this.board.GrantAccess(this.alice, this.forumB);
        this.board.LoginAs(this.alice);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();

        try
        {
            Directory.Delete(this.webRoot, true);
        }
        catch (IOException)
        {
            // index files may still be mapped, the temp folder is cleaned up eventually
        }
    }

    [Test]
    [Description("A posted message can be found by its text, with the match highlighted")]
    public async Task Add_And_Search_Test()
    {
        this.Index(1, 10, this.forumA, "Lucene rocks", "the quick brown fox");

        var (results, total) = await this.NewSearch().SearchPagedAsync(0, "fox", 0, 10);

        total.Should().Be(1);
        var hit = results.Single();
        hit.MessageId.Should().Be(1);
        hit.TopicId.Should().Be(10);
        hit.ForumId.Should().Be(this.forumA);
        hit.Message.Should().Contain("<mark>fox</mark>");
        hit.ForumUrl.Should().Be($"/forum/{this.forumA}");
    }

    [Test]
    [Description("Hidden content (hide, hide-reply, hide-thanks, ...) is not indexed: it can't be found and is never in a result")]
    public async Task Hidden_Content_Is_Not_Indexed_Test()
    {
        var hidden = string.Join(" ", Enumerable.Repeat("filler", 80));

        this.Index(
            1,
            10,
            this.forumA,
            "Hidden",
            $"public introduction [hide-reply]{hidden} secretingredient {hidden}[/hide-reply] [hide-thanks]thankword[/hide-thanks] public end");

        var (results, _) = await this.NewSearch().SearchPagedAsync(0, "secretingredient", 0, 10);
        results.Should().BeEmpty("the hidden word must not be found");

        (await this.NewSearch().SearchPagedAsync(0, "thankword", 0, 10)).Item1.Should().BeEmpty();

        var (found, _) = await this.NewSearch().SearchPagedAsync(0, "introduction", 0, 10);
        found.Should().ContainSingle().Which.Message.Should().NotContain("secretingredient").And.NotContain("thankword");
    }

    [Test]
    [Description("Search hits in forums without read access are not returned")]
    public async Task Search_Respects_Access_Test()
    {
        this.Index(1, 10, this.forumA, "Public", "secret sauce");
        this.Index(2, 20, this.hiddenForum, "Staff", "secret plans");

        var (results, total) = await this.NewSearch().SearchPagedAsync(0, "secret", 0, 10);
        results.Select(r => r.MessageId).Should().Equal(1);
        total.Should().Be(1, "the hits that can't be seen are not counted (the pager showed empty pages)");

        // a search inside the hidden forum does not tell the number of hits
        (await this.NewSearch().SearchPagedAsync(this.hiddenForum, "secret", 0, 10)).Item2.Should().Be(0);

        this.board.LoginAs(this.eve);
        var (none, noneTotal) = await this.NewSearch().SearchPagedAsync(0, "secret", 0, 10);
        none.Should().BeEmpty();
        noneTotal.Should().Be(0);
    }

    [Test]
    [Description("Posts that are not approved (yet) or deleted are only found by moderators of the forum")]
    public async Task Search_Unapproved_Only_For_Moderators_Test()
    {
        this.Index(1, 10, this.forumA, "Approved", "moderation word");
        this.Index(2, 20, this.forumA, "Waiting", "moderation word", new MessageFlags { IsApproved = false });
        this.Index(3, 30, this.forumA, "Deleted", "moderation word", new MessageFlags { IsApproved = true, IsDeleted = true });

        var (results, total) = await this.NewSearch().SearchPagedAsync(0, "moderation", 0, 10);
        results.Select(r => r.MessageId).Should().Equal(1);
        total.Should().Be(1);

        this.board.GrantAccess(this.alice, this.forumA, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        var (moderator, _) = await this.NewSearch().SearchPagedAsync(0, "moderation", 0, 10);
        moderator.Select(r => r.MessageId).Should().BeEquivalentTo([1, 2, 3]);
    }

    [Test]
    [Description("Searching inside one forum only returns messages of that forum")]
    public async Task Search_Forum_Filter_Test()
    {
        this.Index(1, 10, this.forumA, "One", "shared word");
        this.Index(2, 20, this.forumB, "Two", "shared word");

        var (results, _) = await this.NewSearch().SearchPagedAsync(this.forumB, "shared", 0, 10);

        results.Select(r => r.MessageId).Should().Equal(2);
    }

    [Test]
    [Description("Results are ordered newest first and paged with a zero based page index")]
    public async Task Search_Paging_Test()
    {
        var search = this.NewSearch();

        for (var i = 1; i <= 5; i++)
        {
            search.UpdateSearchIndexItem(this.Message(i, 100 + i, this.forumA, $"Topic {i}", "paged content"));
        }

        search.Optimize();

        var (first, total) = await this.NewSearch().SearchPagedAsync(0, "paged", 0, 2);
        var (second, _) = await this.NewSearch().SearchPagedAsync(0, "paged", 1, 2);
        var (last, _) = await this.NewSearch().SearchPagedAsync(0, "paged", 2, 2);

        total.Should().Be(5);
        first.Select(r => r.MessageId).Should().Equal(5, 4);
        second.Select(r => r.MessageId).Should().Equal(3, 2);
        last.Select(r => r.MessageId).Should().Equal(1);
    }

    [Test]
    [Description("Message ids are sorted as numbers, not as strings, so newer messages are not cut off")]
    public async Task Search_Sorts_Ids_Numerically_Test()
    {
        this.board.BoardSettings.ReturnSearchMax = 2;

        this.Index(9, 10, this.forumA, "Nine", "sorting");
        this.Index(10, 11, this.forumA, "Ten", "sorting");
        this.Index(11, 12, this.forumA, "Eleven", "sorting");

        var (results, _) = await this.NewSearch().SearchPagedAsync(0, "sorting", 0, 10);

        results.Select(r => r.MessageId).Should().Equal(11, 10);
    }

    [Test]
    [Description("A single field search only looks into that field")]
    public async Task Search_Single_Field_Test()
    {
        this.Index(1, 10, this.forumA, "Gardening", "about tomatoes");
        this.Index(2, 20, this.forumA, "Tomatoes", "about gardening");

        var (results, _) = await this.NewSearch().SearchPagedAsync(0, "tomatoes", 0, 10, "Topic");

        results.Select(r => r.MessageId).Should().Equal(2);
        results[0].Topic.Should().Be("<mark>Tomatoes</mark>");
    }

    [Test]
    [Description("Author and tags are searched too")]
    public async Task Search_Author_And_Tags_Test()
    {
        var search = this.NewSearch();
        var message = this.Message(1, 10, this.forumA, "Hello", "body");
        message.UserName = "Zebedee";
        message.TopicTags = "astronomy,telescopes";
        search.AddSearchIndexAsync([message]);

        (await this.NewSearch().SearchPagedAsync(0, "zebedee", 0, 10)).Item1.Should().ContainSingle();
        (await this.NewSearch().SearchPagedAsync(0, "astronomy", 0, 10)).Item1.Should().ContainSingle();
    }

    [Test]
    [Description("Editing a message replaces its index entry")]
    public async Task Update_Replaces_Entry_Test()
    {
        this.Index(1, 10, this.forumA, "Topic", "original text");

        this.NewSearch().UpdateSearchIndexItem(this.Message(1, 10, this.forumA, "Topic", "edited text"), true);

        var search = this.NewSearch();
        (await search.SearchPagedAsync(0, "original", 0, 10)).Item2.Should().Be(0);
        (await this.NewSearch().SearchPagedAsync(0, "edited", 0, 10)).Item2.Should().Be(1);
    }

    [Test]
    [Description("Editing a message keeps the topic tags searchable")]
    public async Task Update_Keeps_Topic_Tags_Test()
    {
        var message = this.Message(1, 10, this.forumA, "Topic", "text");
        message.TopicTags = "astronomy";
        this.NewSearch().AddSearchIndexItem(message);

        message.Message = "edited text";
        this.NewSearch().UpdateSearchIndexItem(message, true);

        (await this.NewSearch().SearchPagedAsync(0, "astronomy", 0, 10)).Item1.Should().ContainSingle();
    }

    [Test]
    [Description("Messages without topic tags can be added to the index")]
    public void Add_Without_Tags_Test()
    {
        var message = this.Message(1, 10, this.forumA, "Topic", "text");
        message.TopicTags = null;

        this.NewSearch().Invoking(s => s.AddSearchIndexItem(message)).Should().NotThrow();
        this.NewSearch().CountHits("text").Should().Be(1);
    }

    [Test]
    [Description("Deleting by message or topic removes the entries")]
    public void Delete_Test()
    {
        this.Index(1, 10, this.forumA, "One", "delete me");
        this.Index(2, 10, this.forumA, "One", "delete me too");
        this.Index(3, 20, this.forumA, "Two", "delete me later");

        this.NewSearch().CountHits("delete").Should().Be(3);

        this.NewSearch().DeleteSearchIndexRecordByMessageId(3);
        this.NewSearch().CountHits("delete").Should().Be(2);

        this.NewSearch().DeleteSearchIndexRecordByTopicId(10);
        this.NewSearch().CountHits("delete").Should().Be(0);
    }

    [Test]
    [Description("Clearing the index removes everything")]
    public void Clear_Test()
    {
        this.Index(1, 10, this.forumA, "One", "content");

        var search = this.NewSearch();
        search.ClearSearchIndex().Should().BeTrue();
        search.Optimize();

        this.NewSearch().CountHits("content").Should().Be(0);
    }

    [Test]
    [Description("Without an index nothing is found and nothing throws")]
    public async Task No_Index_Test()
    {
        var search = this.NewSearch();

        search.CountHits("anything").Should().Be(0);
        search.SearchSimilar(string.Empty, "anything", "Topic").Should().BeEmpty();
        (await search.SearchPagedAsync(0, "anything", 0, 10)).Item2.Should().Be(0);
        search.Invoking(s => s.Optimize()).Should().NotThrow();
    }

    [Test]
    [Description("Empty and wildcard only input returns no results")]
    public async Task Empty_Input_Test()
    {
        this.Index(1, 10, this.forumA, "One", "content");
        var search = this.NewSearch();

        (await search.SearchPagedAsync(0, string.Empty, 0, 10)).Item2.Should().Be(0);
        (await search.SearchPagedAsync(0, "*?", 0, 10)).Item2.Should().Be(0);
        (await search.SearchDefaultAsync(0, null)).Item2.Should().Be(0);
        search.SearchSimilar(string.Empty, string.Empty).Should().BeEmpty();
        search.SearchSimilar(string.Empty, "**").Should().BeEmpty();
    }

    [Test]
    [Description("Invalid query syntax is escaped instead of failing")]
    public async Task Invalid_Syntax_Test()
    {
        this.Index(1, 10, this.forumA, "Brackets", "foo bar");

        var (results, _) = await this.NewSearch().SearchPagedAsync(0, "foo AND (", 0, 10);

        results.Should().ContainSingle();
        this.NewSearch().CountHits("bar)").Should().Be(1);
    }

    [Test]
    [Description("The default search returns all results on the first page")]
    public async Task Search_Default_Test()
    {
        this.Index(1, 10, this.forumA, "One", "default search");

        var (results, total) = await this.NewSearch().SearchDefaultAsync(0, "default");

        total.Should().Be(1);
        results.Should().ContainSingle();
    }

    [Test]
    [Description("Similar topics exclude the current topic and are grouped by topic")]
    public void Search_Similar_Test()
    {
        this.Index(1, 10, this.forumA, "Bread baking", "a");
        this.Index(2, 20, this.forumA, "Baking bread at home", "b");
        this.Index(3, 20, this.forumA, "Baking bread at home", "c");
        this.Index(4, 30, this.hiddenForum, "Bread for staff", "d");

        var similar = this.NewSearch().SearchSimilar("10", "bread", "Topic");

        similar.Select(s => s.TopicId).Should().Equal(20);
        similar[0].TopicUrl.Should().NotBeNull();

        this.NewSearch().SearchSimilar(string.Empty, "bread", "Topic").Select(s => s.TopicId).Should()
            .BeEquivalentTo([10, 20]);
    }

    [Test]
    [Description("The results are shown with javascript (innerHTML): topic, description and tags are html encoded, the highlighting stays html")]
    public async Task Results_Are_Html_Encoded_Test()
    {
        const string evilTopic = "Cake <img src=x onerror=alert(1) x";

        var search = this.NewSearch();
        var message = this.Message(1, 10, this.forumA, evilTopic, "cake recipe");
        message.Description = "<b>bold</b>";
        message.TopicTags = "<i>tag</i>";
        search.AddSearchIndexAsync([message]);

        var hit = (await this.NewSearch().SearchPagedAsync(0, "cake", 0, 10)).Item1.Single();

        hit.Topic.Should().NotContain("<img").And.Contain("&lt;img").And.Contain("<mark>Cake</mark>");
        hit.Description.Should().Be("&lt;b&gt;bold&lt;/b&gt;");
        hit.TopicTags.Should().Be("&lt;i&gt;tag&lt;/i&gt;");

        var similar = this.NewSearch().SearchSimilar(string.Empty, "cake", "Topic").Single();

        similar.Topic.Should().Be("Cake &lt;img src=x onerror=alert(1) x");
    }

    [Test]
    [Description("The same service instance can search after it has written to the index")]
    public async Task Search_After_Write_Same_Instance_Test()
    {
        var search = this.NewSearch();

        search.AddSearchIndexItem(this.Message(1, 10, this.forumA, "One", "written"));

        search.CountHits("written").Should().Be(1);
        search.SearchSimilar(string.Empty, "one", "Topic").Should().ContainSingle();
        (await search.SearchPagedAsync(0, "written", 0, 10)).Item2.Should().Be(1);

        search.AddSearchIndexItem(this.Message(2, 20, this.forumA, "Two", "written"));
        search.CountHits("written").Should().Be(2);
    }

    private Search NewSearch()
    {
        return new Search(this.board.Current.ServiceLocator);
    }

    private void Index(int messageId, int topicId, int forumId, string topic, string text, MessageFlags flags = null)
    {
        var search = this.NewSearch();
        var message = this.Message(messageId, topicId, forumId, topic, text);
        message.Flags = (flags ?? new MessageFlags { IsApproved = true }).BitValue;
        search.AddSearchIndexItem(message);
        search.Dispose();
    }

    private SearchMessage Message(int messageId, int topicId, int forumId, string topic, string text)
    {
        return new SearchMessage
        {
            MessageId = messageId,
            Message = text,
            Flags = new MessageFlags { IsApproved = true }.BitValue,
            Posted = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            UserName = "Alice",
            UserDisplayName = "Alice A.",
            UserId = this.alice,
            TopicId = topicId,
            Topic = topic,
            TopicTags = string.Empty,
            ForumId = forumId,
            ForumName = $"Forum {forumId}",
            Description = string.Empty
        };
    }
}
