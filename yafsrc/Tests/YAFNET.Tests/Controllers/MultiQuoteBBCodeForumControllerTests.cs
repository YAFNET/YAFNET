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

namespace YAF.Tests.Controllers;

using System.IO;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YAF.Core.Controllers;
using YAF.Core.Controllers.Modals;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Core.Services.Import;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The multi quote toggle, the admin BBCode extension import and the forum select list.
/// </summary>
[TestFixture]
[NonParallelizable]
public class MultiQuoteBBCodeForumControllerTests
{
    private TestBoardContext board;

    private int alice;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<SessionService>().As<ISessionService>();
                b.RegisterType<DataImporter>().As<IDataImporter>();
            },
            useDatabase: true);

        this.alice = this.board.Database.AddUser("Alice");
        this.board.LoginAs(this.alice);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Checking a quote button adds the message once, the css class shows the checked state")]
    public void MultiQuote_Add_Test()
    {
        var controller = this.board.CreateController<MultiQuoteController>();

        var result = Value<ReturnClass>(controller.HandleMultiQuote(Button(5, true)));
        controller.HandleMultiQuote(Button(5, true));
        controller.HandleMultiQuote(Button(6, true));

        result.Id.Should().Be("btn-5");
        result.NewTitle.Should().Be("btn-multiquote Checked");
        this.Session.MultiQuoteIds.Select(m => m.MessageID).Should().Equal(5, 6);
        this.Session.MultiQuoteIds[0].TopicID.Should().Be(10);
    }

    [Test]
    [Description("Unchecking a quote button removes the message from the multi quote list")]
    public void MultiQuote_Remove_Test()
    {
        var controller = this.board.CreateController<MultiQuoteController>();

        controller.HandleMultiQuote(Button(5, true));
        controller.HandleMultiQuote(Button(6, true));

        var result = Value<ReturnClass>(controller.HandleMultiQuote(Button(5, false)));

        result.NewTitle.Should().Be("btn-multiquote form-check btn btn-link");
        this.Session.MultiQuoteIds.Select(m => m.MessageID).Should().Equal(6);

        controller.HandleMultiQuote(Button(7, false));
        this.Session.MultiQuoteIds.Select(m => m.MessageID).Should().Equal(6);
    }

    [Test]
    [Description("BBCode import: only text files, valid exports are imported, broken files are reported")]
    public async Task BBCode_Import_Test()
    {
        this.board.PageLoad.IsAdmin = true;
        var controller = this.board.CreateController<BBCodeController>();

        Notification(await controller.Import(File("x", "application/pdf"))).MessageType.Should()
            .Be(MessageTypes.danger.ToString());

        const string Export = """
                              <YafBBCodeList>
                                <YafBBCode><Name>NEW</Name><Description>d</Description><SearchRegex>a</SearchRegex><ReplaceRegex>b</ReplaceRegex><ExecOrder>1</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>true</UseToolbar></YafBBCode>
                              </YafBBCodeList>
                              """;

        var imported = Notification(await controller.Import(File(Export, "text/xml")));
        imported.MessageType.Should().Be(MessageTypes.success.ToString());
        imported.Message.Should().Contain("IMPORT_SUCESS");
        (await this.board.Current.GetRepository<BBCode>().CountAsync(b => b.Name == "NEW")).Should().Be(1);

        Notification(await controller.Import(File(Export, "text/xml"))).Message.Should()
            .Contain("IMPORT_NOTHING", "the extension exists already");

        Notification(await controller.Import(File("not xml <", "text/plain"))).MessageType.Should()
            .Be(MessageTypes.danger.ToString());
    }

    [Test]
    [Description("The forum list only contains forums the user can read, optionally with the 'all forums' entry first")]
    public async Task Forum_List_Test()
    {
        var db = this.board.Database;
        var category = db.AddCategory("Category");
        var readable = db.AddForum("Readable", category);
        var other = db.AddForum("Another one", category);
        db.AddForum("Hidden", category);

        this.board.GrantAccess(this.alice, readable);
        this.board.GrantAccess(this.alice, other);

        var controller = this.board.CreateController<ForumController>();

        var all = Value<SelectPagedGroupOptions>(await controller.GetForums(new SearchTopic { Page = 0 }));
        all.Total.Should().Be(2);
        all.Results.SelectMany(g => g.children).Select(c => c.text).Should().BeEquivalentTo(["Readable", "Another one"]);

        var withAll = Value<SelectPagedGroupOptions>(
            await controller.GetForums(new SearchTopic { Page = 0, AllForumsOption = true }));
        withAll.Results[0].id.Should().Be(0);
        withAll.Results[0].children.Single().id.Should().Be("0");

        var searched = Value<SelectPagedGroupOptions>(
            await controller.GetForums(new SearchTopic { SearchTerm = "READ" }));
        searched.Total.Should().Be(1);
        searched.Results.Single().children.Single().text.Should().Be("Readable");
    }

    [Test]
    [Description("Without readable forums the list is empty, even with the 'all forums' option")]
    public async Task Forum_List_Empty_Test()
    {
        var controller = this.board.CreateController<ForumController>();

        var result = Value<SelectPagedGroupOptions>(await controller.GetForums(new SearchTopic { Page = 0 }));

        result.Total.Should().Be(0);
        result.Results.Should().BeEmpty();
    }

    private ISessionService Session => this.board.Resolve<ISessionService>();

    private static MultiQuoteButton Button(int messageId, bool isChecked)
    {
        return new MultiQuoteButton
        {
            ButtonId = $"btn-{messageId}",
            ButtonCssClass = "btn-multiquote",
            IsMultiQuoteButton = isChecked,
            MessageId = messageId,
            TopicId = 10
        };
    }

    private static T Value<T>(IActionResult result)
    {
        return (T)((OkObjectResult)result).Value;
    }

    private static T Value<T>(ActionResult<T> result)
    {
        return (T)((OkObjectResult)result.Result).Value;
    }

    private static MessageModalNotification Notification(IActionResult result)
    {
        return Value<MessageModalNotification>(result);
    }

    private static FormFile File(string content, string contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(content);

        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "import.xml")
        {
            Headers = new HeaderDictionary(), ContentType = contentType
        };
    }
}
