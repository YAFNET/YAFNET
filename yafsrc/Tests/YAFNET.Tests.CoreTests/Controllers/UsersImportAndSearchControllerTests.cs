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

namespace YAF.Tests.CoreTests.Controllers;

using System.IO;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using YAF.Core.Controllers;
using YAF.Core.Controllers.Modals;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Objects;

/// <summary>
/// The admin user import dialog and the search api.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UsersImportAndSearchControllerTests
{
    private TestBoardContext board;

    private IDataImporter importer;

    private ISearch search;

    private IPermissions permissions;

    [SetUp]
    public void SetUp()
    {
        this.importer = Substitute.For<IDataImporter>();
        this.search = Substitute.For<ISearch>();
        this.permissions = Substitute.For<IPermissions>();
        this.permissions.Check(Arg.Any<ViewPermissions>()).Returns(true);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(this.importer).As<IDataImporter>();
                b.RegisterInstance(this.search).As<ISearch>();
                b.RegisterInstance(this.permissions).As<IPermissions>();
            });

        this.board.PageLoad.IsAdmin = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Xml files are imported as xml, the number of new users is reported")]
    public async Task Import_Xml_Test()
    {
        this.importer.ImportingUsersAsync(Arg.Any<Stream>(), true).Returns(3);

        var result = await this.Import("text/xml");

        result.MessageType.Should().Be(MessageTypes.success.ToString());
        await this.importer.Received(1).ImportingUsersAsync(Arg.Any<Stream>(), true);
    }

    [TestCase("text/csv")]
    [TestCase("application/vnd.ms-excel")]
    [TestCase("text/comma-separated-values")]
    [Description("Csv files are imported as csv, whatever csv content type the browser sends")]
    public async Task Import_Csv_Test(string contentType)
    {
        this.importer.ImportingUsersAsync(Arg.Any<Stream>(), false).Returns(2);

        (await this.Import(contentType)).MessageType.Should().Be(MessageTypes.success.ToString());
        await this.importer.Received(1).ImportingUsersAsync(Arg.Any<Stream>(), false);
    }

    [Test]
    [Description("Xml files sent as application/xml are imported too")]
    public async Task Import_Application_Xml_Test()
    {
        this.importer.ImportingUsersAsync(Arg.Any<Stream>(), true).Returns(1);

        (await this.Import("application/xml")).MessageType.Should().Be(MessageTypes.success.ToString());
    }

    [Test]
    [Description("Other files are rejected, nothing new is an info, import errors are reported")]
    public async Task Import_Other_Results_Test()
    {
        (await this.Import("image/png")).MessageType.Should().Be(MessageTypes.danger.ToString());
        await this.importer.DidNotReceiveWithAnyArgs().ImportingUsersAsync(default, default);

        this.importer.ImportingUsersAsync(Arg.Any<Stream>(), true).Returns(0);
        (await this.Import("text/xml")).MessageType.Should().Be(MessageTypes.info.ToString());

        this.importer.ImportingUsersAsync(Arg.Any<Stream>(), true).ThrowsAsync(new InvalidDataException("bad file"));
        var failed = await this.Import("text/xml");
        failed.MessageType.Should().Be(MessageTypes.danger.ToString());
        failed.Message.Should().Contain("ADMIN_USERS_IMPORT.IMPORT_FAILED");

        this.importer.ImportingUsersAsync(Arg.Any<Stream>(), true).ThrowsAsync(new FormatException("no users"));
        var wrongFormat = await this.Import("text/xml");
        wrongFormat.MessageType.Should().Be(MessageTypes.danger.ToString());
        wrongFormat.Message.Should().Contain("IMPORT_FAILED_FORMAT");
    }

    [Test]
    [Description("Similar topic titles: the matches, or an empty result when the search has none")]
    public async Task Similar_Titles_Test()
    {
        var controller = this.board.CreateController<SearchController>();

        this.search.SearchSimilar(string.Empty, "hello", "Topic").Returns([new SearchMessage { Topic = "hello world" }]);
        var found = Value(await controller.GetSimilarTitles(new SearchTopic { SearchTerm = "hello" }));
        found.TotalRecords.Should().Be(1);
        found.SearchResults.Single().Topic.Should().Be("hello world");

        this.search.SearchSimilar(string.Empty, "nothing", "Topic").Returns((List<SearchMessage>)null);
        var none = Value(await controller.GetSimilarTitles(new SearchTopic { SearchTerm = "nothing" }));
        none.TotalRecords.Should().Be(0);
    }

    [Test]
    [Description("Search results are paged as requested")]
    public async Task Search_Results_Test()
    {
        this.search.SearchPagedAsync(5, "term", 2, 10).Returns(
            Tuple.Create(new List<SearchMessage> { new() { Topic = "a" }, new() { Topic = "b" } }, 12));

        var result = Value(
            await this.board.CreateController<SearchController>()
                .GetSearchResults(new SearchTopic { ForumId = 5, SearchTerm = "term", Page = 2, PageSize = 10 }));

        result.TotalRecords.Should().Be(12);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.SearchResults.Select(r => r.Topic).Should().Equal("a", "b");
    }

    [Test]
    [Description("Searching needs the search permission of the board, the page size is limited")]
    public async Task Search_Permission_And_Page_Size_Test()
    {
        this.search.SearchPagedAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(Tuple.Create(new List<SearchMessage>(), 0));

        var result = Value(
            await this.board.CreateController<SearchController>()
                .GetSearchResults(new SearchTopic { SearchTerm = "term", Page = -3, PageSize = 100000 }));

        result.PageSize.Should().Be(50);
        await this.search.Received(1).SearchPagedAsync(0, "term", 0, 50);

        this.permissions.Check(Arg.Any<ViewPermissions>()).Returns(false);

        (await this.board.CreateController<SearchController>()
             .GetSearchResults(new SearchTopic { SearchTerm = "term", Page = 0, PageSize = 10 })).Result
            .Should().BeOfType<NotFoundResult>();
    }

    private static SearchGridDataSet Value(ActionResult<SearchGridDataSet> result)
    {
        return (SearchGridDataSet)((OkObjectResult)result.Result).Value;
    }

    private async Task<MessageModalNotification> Import(string contentType)
    {
        var bytes = Encoding.UTF8.GetBytes("data");
        var file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "users")
        {
            Headers = new HeaderDictionary(), ContentType = contentType
        };

        var result = await this.board.CreateController<UsersImportController>().ImportAsync(file);

        return (MessageModalNotification)((OkObjectResult)result).Value;
    }
}
