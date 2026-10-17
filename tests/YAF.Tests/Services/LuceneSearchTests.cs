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
using System.Web;

using Autofac;

using NSubstitute;

using YAF.Core.Services;
using YAF.Lucene.Net.Analysis.Standard;
using YAF.Lucene.Net.Documents;
using YAF.Lucene.Net.Index;
using YAF.Lucene.Net.Util;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Objects;

/// <summary>
/// The real Lucene <see cref="Search"/> service against an index in a temp folder (the data directory of
/// the application), with forum access read from the access views of the in-memory database.
/// </summary>
[TestFixture]
[NonParallelizable]
public class LuceneSearchTests
{
    /// <summary>
    /// The index folder of the search (the data directory is read once by the search).
    /// </summary>
    private static string indexFolder;

    private readonly List<Search> searches = [];

    private TestBoardContext board;

    private int alice;

    private int eve;

    private int forumA;

    private int forumB;

    private int hiddenForum;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        if (AppDomain.CurrentDomain.GetData("DataDirectory") == null)
        {
            AppDomain.CurrentDomain.SetData(
                "DataDirectory",
                Path.Combine(Path.GetTempPath(), $"yaf-search-{Guid.NewGuid():N}"));
        }

        indexFolder = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "search_index");
    }

    [SetUp]
    public void SetUp()
    {
        DeleteIndex();

        var formatMessage = Substitute.For<IFormatMessage>();
        formatMessage.Format(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<MessageFlags>(), Arg.Any<bool>(), Arg.Any<DateTime>())
            .Returns(c => c.ArgAt<string>(1));

        // the real removal of hidden content (it is not indexed)
        formatMessage.RemoveHiddenBBCodeContent(Arg.Any<string>())
            .Returns(c => new FormatMessage(null, null, null).RemoveHiddenBBCodeContent(c.ArgAt<string>(0)));

        var bbCode = Substitute.For<IBBCode>();
        bbCode.FormatMessageWithCustomBBCode(Arg.Any<string>(), Arg.Any<MessageFlags>(), Arg.Any<int?>(), Arg.Any<int?>())
            .Returns(c => c.ArgAt<string>(0));

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(formatMessage).As<IFormatMessage>();
                b.RegisterInstance(bbCode).As<IBBCode>();
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
        this.searches.ForEach(s => s.Dispose());
        this.searches.Clear();

        this.board?.Dispose();

        DeleteIndex();
    }

    [Test]
    [Description("A posted message can be found by its text, with the match highlighted")]
    public void Add_And_Search_Test()
    {
        this.Index(1, 10, this.forumA, "Lucene rocks", "the quick brown fox");

        var results = this.NewSearch().SearchPaged(out var total, 0, "fox", 0, 10);

        total.Should().Be(1);
        var hit = results.Single();
        hit.MessageId.Should().Be(1);
        hit.TopicId.Should().Be(10);
        hit.ForumId.Should().Be(this.forumA);
        hit.Message.Should().Contain("<mark>fox</mark>");
        hit.ForumUrl.Should().NotBeNullOrEmpty();
    }

    [Test]
    [Description("Hidden content (hide, hide-reply, hide-thanks, ...) is not indexed: it can't be found and is never in a result")]
    public void Hidden_Content_Is_Not_Indexed_Test()
    {
        var hidden = string.Join(" ", Enumerable.Repeat("filler", 80));

        this.Index(
            1,
            10,
            this.forumA,
            "Hidden",
            $"public introduction [hide-reply]{hidden} secretingredient {hidden}[/hide-reply] [hide-thanks]thankword[/hide-thanks] public end");

        this.NewSearch().SearchPaged(out _, 0, "secretingredient", 0, 10).Should().BeEmpty("the hidden word must not be found");
        this.NewSearch().SearchPaged(out _, 0, "thankword", 0, 10).Should().BeEmpty();

        this.NewSearch().SearchPaged(out _, 0, "introduction", 0, 10).Should().ContainSingle()
            .Which.Message.Should().NotContain("secretingredient").And.NotContain("thankword");
    }

    [Test]
    [Description("Hidden content is not indexed by the index task either (it adds the messages in bulk)")]
    public void Hidden_Content_Is_Not_Indexed_By_The_Task_Test()
    {
        var message = this.Message(1, 10, this.forumA, "Hidden", "public text [hide-reply]secretingredient[/hide-reply]");

        this.NewSearch().AddSearchIndexAsync([message]).Wait();

        this.NewSearch().SearchPaged(out _, 0, "secretingredient", 0, 10).Should().BeEmpty();
        this.NewSearch().SearchPaged(out _, 0, "public", 0, 10).Should().ContainSingle();
    }

    [Test]
    [Description("Search hits in forums without read access are not returned")]
    public void Search_Respects_Access_Test()
    {
        this.Index(1, 10, this.forumA, "Public", "secret sauce");
        this.Index(2, 20, this.hiddenForum, "Staff", "secret plans");

        var results = this.NewSearch().SearchPaged(out var total, 0, "secret", 0, 10);
        results.Select(r => r.MessageId).Should().Equal(1);
        total.Should().Be(1, "the hits that can't be seen are not counted (the pager showed empty pages)");

        // a search inside the hidden forum does not tell the number of hits
        this.NewSearch().SearchPaged(out var hiddenTotal, this.hiddenForum, "secret", 0, 10);
        hiddenTotal.Should().Be(0);

        this.board.LoginAs(this.eve);
        this.NewSearch().SearchPaged(out var noneTotal, 0, "secret", 0, 10).Should().BeEmpty();
        noneTotal.Should().Be(0);
    }

    [Test]
    [Description("Posts that are not approved (yet) or deleted are only found by moderators of the forum")]
    public void Search_Unapproved_Only_For_Moderators_Test()
    {
        this.Index(1, 10, this.forumA, "Approved", "moderation word");
        this.Index(2, 20, this.forumA, "Waiting", "moderation word", new MessageFlags { IsApproved = false });
        this.Index(3, 30, this.forumA, "Deleted", "moderation word", new MessageFlags { IsApproved = true, IsDeleted = true });

        var results = this.NewSearch().SearchPaged(out var total, 0, "moderation", 0, 10);
        results.Select(r => r.MessageId).Should().Equal(1);
        total.Should().Be(1);

        this.board.GrantAccess(this.alice, this.forumA, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        this.NewSearch().SearchPaged(out _, 0, "moderation", 0, 10).Select(r => r.MessageId).Should()
            .BeEquivalentTo([1, 2, 3]);
    }

    [Test]
    [Description("Searching inside one forum only returns messages of that forum")]
    public void Search_Forum_Filter_Test()
    {
        this.Index(1, 10, this.forumA, "One", "shared word");
        this.Index(2, 20, this.forumB, "Two", "shared word");

        this.NewSearch().SearchPaged(out _, this.forumB, "shared", 0, 10).Select(r => r.MessageId).Should().Equal(2);
    }

    [Test]
    [Description("Results are ordered newest first and paged with a zero based page index")]
    public void Search_Paging_Test()
    {
        var search = this.NewSearch();

        for (var i = 1; i <= 5; i++)
        {
            search.UpdateSearchIndexItem(this.Message(i, 100 + i, this.forumA, $"Topic {i}", "paged content"));
        }

        search.Optimize();

        var first = this.NewSearch().SearchPaged(out var total, 0, "paged", 0, 2);
        var second = this.NewSearch().SearchPaged(out _, 0, "paged", 1, 2);
        var last = this.NewSearch().SearchPaged(out _, 0, "paged", 2, 2);

        total.Should().Be(5);
        first.Select(r => r.MessageId).Should().Equal(5, 4);
        second.Select(r => r.MessageId).Should().Equal(3, 2);
        last.Select(r => r.MessageId).Should().Equal(1);
    }

    [Test]
    [Description("Message ids are sorted as numbers, not as strings, so newer messages are not cut off")]
    public void Search_Sorts_Ids_Numerically_Test()
    {
        this.board.BoardSettings.ReturnSearchMax = 2;

        this.Index(9, 10, this.forumA, "Nine", "sorting");
        this.Index(10, 11, this.forumA, "Ten", "sorting");
        this.Index(11, 12, this.forumA, "Eleven", "sorting");

        this.NewSearch().SearchPaged(out _, 0, "sorting", 0, 10).Select(r => r.MessageId).Should().Equal(11, 10);
    }

    [Test]
    [Description("A single field search only looks into that field")]
    public void Search_Single_Field_Test()
    {
        this.Index(1, 10, this.forumA, "Gardening", "about tomatoes");
        this.Index(2, 20, this.forumA, "Tomatoes", "about gardening");

        var results = this.NewSearch().SearchPaged(out _, 0, "tomatoes", 0, 10, "Topic");

        results.Select(r => r.MessageId).Should().Equal(2);
        results[0].Topic.Should().Be("<mark>Tomatoes</mark>");
    }

    [Test]
    [Description("Author and tags are searched too")]
    public void Search_Author_And_Tags_Test()
    {
        var message = this.Message(1, 10, this.forumA, "Hello", "body");
        message.UserName = "Zebedee";
        message.TopicTags = "astronomy,telescopes";
        this.NewSearch().AddSearchIndexAsync([message]).Wait();

        this.NewSearch().SearchPaged(out _, 0, "zebedee", 0, 10).Should().ContainSingle();
        this.NewSearch().SearchPaged(out _, 0, "astronomy", 0, 10).Should().ContainSingle();
    }

    [Test]
    [Description("Editing a message replaces its index entry")]
    public void Update_Replaces_Entry_Test()
    {
        this.Index(1, 10, this.forumA, "Topic", "original text");

        this.NewSearch().UpdateSearchIndexItem(this.Message(1, 10, this.forumA, "Topic", "edited text"), true);

        this.NewSearch().SearchPaged(out var original, 0, "original", 0, 10);
        this.NewSearch().SearchPaged(out var edited, 0, "edited", 0, 10);

        original.Should().Be(0);
        edited.Should().Be(1);
    }

    [Test]
    [Description("Editing a message keeps the topic tags searchable")]
    public void Update_Keeps_Topic_Tags_Test()
    {
        var message = this.Message(1, 10, this.forumA, "Topic", "text");
        message.TopicTags = "astronomy";
        this.NewSearch().AddSearchIndexItem(message);

        message.Message = "edited text";
        this.NewSearch().UpdateSearchIndexItem(message, true);

        this.NewSearch().SearchPaged(out _, 0, "astronomy", 0, 10).Should().ContainSingle();
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
    public void No_Index_Test()
    {
        var search = this.NewSearch();

        search.CountHits("anything").Should().Be(0);
        search.SearchSimilar(string.Empty, "anything", "Topic").Should().BeEmpty();
        search.SearchPaged(out var total, 0, "anything", 0, 10);
        total.Should().Be(0);
        search.Invoking(s => s.Optimize()).Should().NotThrow();
    }

    [Test]
    [Description("Empty and wildcard only input returns no results")]
    public void Empty_Input_Test()
    {
        this.Index(1, 10, this.forumA, "One", "content");
        var search = this.NewSearch();

        search.SearchPaged(out var empty, 0, string.Empty, 0, 10);
        empty.Should().Be(0);
        search.SearchPaged(out var wildcard, 0, "*?", 0, 10);
        wildcard.Should().Be(0);
        search.SearchDefault(0, null).Should().BeEmpty();
        search.SearchSimilar(string.Empty, string.Empty).Should().BeEmpty();
        search.SearchSimilar(string.Empty, "**").Should().BeEmpty();
    }

    [Test]
    [Description("Invalid query syntax is escaped instead of failing")]
    public void Invalid_Syntax_Test()
    {
        this.Index(1, 10, this.forumA, "Brackets", "foo bar");

        this.NewSearch().SearchPaged(out _, 0, "foo AND (", 0, 10).Should().ContainSingle();
        this.NewSearch().CountHits("bar)").Should().Be(1);
    }

    [Test]
    [Description("The default search returns all results on the first page")]
    public void Search_Default_Test()
    {
        this.Index(1, 10, this.forumA, "One", "default search");

        this.NewSearch().SearchDefault(0, "default").Should().ContainSingle();
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
    public void Results_Are_Html_Encoded_Test()
    {
        const string EvilTopic = "Cake <img src=x onerror=alert(1) x";

        var message = this.Message(1, 10, this.forumA, EvilTopic, "cake recipe");
        message.Description = "<b>bold</b>";
        message.TopicTags = "<i>tag</i>";
        this.NewSearch().AddSearchIndexAsync([message]).Wait();

        var hit = this.NewSearch().SearchPaged(out _, 0, "cake", 0, 10).Single();

        hit.Topic.Should().NotContain("<img").And.Contain("&lt;img").And.Contain("<mark>Cake</mark>");
        hit.Description.Should().Be("&lt;b&gt;bold&lt;/b&gt;");
        hit.TopicTags.Should().Be("&lt;i&gt;tag&lt;/i&gt;");

        var similar = this.NewSearch().SearchSimilar(string.Empty, "cake", "Topic").Single();

        similar.Topic.Should().Be(HttpUtility.HtmlEncode(EvilTopic));
    }

    [Test]
    [Description("The same service instance can search after it has written to the index")]
    public void Search_After_Write_Same_Instance_Test()
    {
        var search = this.NewSearch();

        search.AddSearchIndexItem(this.Message(1, 10, this.forumA, "One", "written"));

        search.CountHits("written").Should().Be(1);
        search.SearchSimilar(string.Empty, "one", "Topic").Should().ContainSingle();
        search.SearchPaged(out var total, 0, "written", 0, 10);
        total.Should().Be(1);

        search.AddSearchIndexItem(this.Message(2, 20, this.forumA, "Two", "written"));
        search.CountHits("written").Should().Be(2);
    }

    [Test]
    [Description("Documents of older versions have no flags: they are found in readable forums, an unreadable date is shown as stored")]
    public void Document_Without_Flags_Test()
    {
        this.IndexWithoutFlags(1, this.forumA, "an old fox");
        this.IndexWithoutFlags(2, this.hiddenForum, "a hidden fox");

        var results = this.NewSearch().SearchPaged(out var total, 0, "fox", 0, 10);

        total.Should().Be(1);
        var hit = results.Single();
        hit.MessageId.Should().Be(1);
        hit.Posted.Should().Be("some day");
    }

    [Test]
    [Description("A write lock left by a crashed process is removed, the index can be written again")]
    public void Stale_Write_Lock_Test()
    {
        this.Index(1, 10, this.forumA, "first", "the first fox");

        File.WriteAllText(Path.Combine(indexFolder, "write.lock"), string.Empty);

        this.Index(2, 11, this.forumA, "second", "the second fox");

        this.NewSearch().SearchPaged(out var total, 0, "fox", 0, 10);
        total.Should().Be(2);
    }

    [Test]
    [Description("The date of a result is shown relative when the board shows relative times")]
    public void Relative_Time_Test()
    {
        this.Index(1, 10, this.forumA, "Lucene rocks", "the quick brown fox");

        this.board.BoardSettings.ShowRelativeTime = true;

        var hit = this.NewSearch().SearchPaged(out _, 0, "fox", 0, 10).Single();

        hit.Posted.Should().NotBe(this.Message(1, 10, this.forumA, "Lucene rocks", "fox").Posted)
            .And.NotBeNullOrEmpty();
    }

    private static void DeleteIndex()
    {
        if (indexFolder != null && Directory.Exists(indexFolder))
        {
            Directory.Delete(indexFolder, true);
        }
    }

    /// <summary>
    /// Indexes a message like older versions did, without the flags.
    /// </summary>
    private void IndexWithoutFlags(int messageId, int forumId, string text)
    {
        using var directory = YAF.Lucene.Net.Store.FSDirectory.Open(indexFolder);
        using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        using var writer = new IndexWriter(directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer));

        writer.AddDocument(
        [
            new StringField("MessageId", messageId.ToString(), Field.Store.YES),
            new TextField("Message", text, Field.Store.YES),
            new StoredField("Posted", "some day"),
            new StringField("UserId", this.alice.ToString(), Field.Store.YES),
            new StringField("TopicId", "10", Field.Store.YES),
            new TextField("Topic", "Old topic", Field.Store.YES),
            new StringField("ForumName", "Forum", Field.Store.YES),
            new StringField("ForumId", forumId.ToString(), Field.Store.YES),
            new TextField("Author", "Alice", Field.Store.YES),
            new TextField("AuthorDisplay", "Alice", Field.Store.YES),
            new StoredField("AuthorStyle", string.Empty),
            new TextField("Description", string.Empty, Field.Store.YES),
            new TextField("TopicTags", string.Empty, Field.Store.YES)
        ]);

        writer.Commit();
    }

    private Search NewSearch()
    {
        var search = new Search(this.board.Current.ServiceLocator);

        this.searches.Add(search);

        return search;
    }

    private void Index(int messageId, int topicId, int forumId, string topic, string text, MessageFlags flags = null)
    {
        var search = new Search(this.board.Current.ServiceLocator);
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
