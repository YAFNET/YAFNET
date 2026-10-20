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
using Autofac.Features.Indexed;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using YAF.Core.Controllers.Modals;
using YAF.Core.Data;
using YAF.Core.Events;
using YAF.Core.Extensions;
using YAF.Core.Services.Import;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Exceptions;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Services;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The admin import dialogs (banned e-mails, user agents, spam words), the banned user agent dialog
/// and the database provider lookup.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AdminImportControllerTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(b => b.RegisterType<DataImporter>().As<IDataImporter>(), useDatabase: true);
        this.board.LoginAs(this.board.Database.AddUser("Admin"));
        this.board.PageLoad.IsAdmin = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Banned e-mail import: only text files, the new addresses are imported and reported")]
    public async Task Banned_Email_Import_Test()
    {
        var controller = this.board.CreateController<BannedEmailController>();

        Notification(await controller.Import(File("x", "application/pdf"))).MessageType.Should().Be(MessageTypes.danger.ToString());
        Notification(await controller.Import(File("a@spam.example\nb@spam.example", "text/plain"))).MessageType.Should().Be(MessageTypes.success.ToString());
        Notification(await controller.Import(File("a@spam.example", "text/plain"))).MessageType.Should().Be(MessageTypes.success.ToString());

        (await this.Repository<BannedEmail>().CountAsync(_ => true)).Should().Be(2);
    }

    [Test]
    [Description("Banned user agent import: only text files, valid expressions are imported")]
    public async Task Banned_User_Agent_Import_Test()
    {
        var controller = this.board.CreateController<BannedUserAgentController>();

        Notification(await controller.Import(File("x", "application/octet-stream"))).MessageType.Should().Be(MessageTypes.danger.ToString());
        Notification(await controller.Import(File("EvilBot\nBadBot", "text/plain"))).MessageType.Should().Be(MessageTypes.success.ToString());
        Notification(await controller.Import(File("EvilBot", "text/plain"))).MessageType.Should().Be(MessageTypes.success.ToString());

        (await this.Repository<BannedUserAgent>().GetAsync(_ => true)).Select(b => b.UserAgent).Should().BeEquivalentTo(["EvilBot", "BadBot"]);
    }

    [Test]
    [Description("Spam word import: only text files, the xml export is imported, broken files are reported")]
    public void Spam_Words_Import_Test()
    {
        var controller = this.board.CreateController<SpamWordsController>();

        Notification(controller.Import(File("x", "image/png"))).MessageType.Should().Be(MessageTypes.danger.ToString());
        Notification(
                controller.Import(
                    File(
                        "<YafSpamWordsList><YafSpamWords><SpamWord>casino</SpamWord></YafSpamWords></YafSpamWordsList>",
                        "text/xml")))
            .MessageType.Should().Be(MessageTypes.success.ToString());
        Notification(controller.Import(File("<root><x>1</x></root>", "text/xml"))).MessageType
            .Should().Be(MessageTypes.success.ToString(), "nothing to import");
        Notification(controller.Import(File("not xml", "text/xml"))).MessageType.Should().Be(MessageTypes.danger.ToString());

        this.Repository<SpamWords>().Get(_ => true).Select(s => s.SpamWord).Should().Equal("casino");
    }

    [Test]
    [Description("Import errors are reported in the dialog")]
    public async Task Import_Error_Test()
    {
        var importer = Substitute.For<IDataImporter>();
        importer.BannedEmailAddressesImportAsync(0, null).ThrowsAsyncForAnyArgs(new IOException("broken"));
        importer.BannedUserAgentsImportAsync(0, null).ThrowsAsyncForAnyArgs(new IOException("broken"));

        this.board.Dispose();
        this.board = new TestBoardContext(b => b.RegisterInstance(importer).As<IDataImporter>());

        Notification(await this.board.CreateController<BannedEmailController>().Import(File("x", "text/plain")))
            .MessageType.Should().Be(MessageTypes.danger.ToString());
        Notification(await this.board.CreateController<BannedUserAgentController>().Import(File("x", "text/plain")))
            .MessageType.Should().Be(MessageTypes.danger.ToString());
    }

    [Test]
    [Description("Banned user agents: valid expressions are saved (trimmed), invalid ones and duplicates rejected, existing ones changed")]
    public void Banned_User_Agent_Edit_Test()
    {
        var controller = this.board.CreateController<BannedUserAgentController>();

        controller.Edit(new BannedEmailEditModal { Id = 0, Mask = " EvilBot " }).Should().BeOfType<OkResult>();
        Notification(controller.Edit(new BannedEmailEditModal { Mask = "EvilBot" })).MessageType.Should().Be(MessageTypes.warning.ToString());
        Notification(controller.Edit(new BannedEmailEditModal { Mask = "Bot(" })).MessageType.Should().Be(MessageTypes.warning.ToString());
        Notification(controller.Edit(new BannedEmailEditModal { Mask = null })).MessageType.Should().Be(MessageTypes.warning.ToString());

        var ban = this.Repository<BannedUserAgent>().GetSingle(_ => true);
        ban.UserAgent.Should().Be("EvilBot");

        controller.Edit(new BannedEmailEditModal { Id = ban.ID, Mask = "BadBot" }).Should().BeOfType<OkResult>();
        this.Repository<BannedUserAgent>().GetSingle(_ => true).UserAgent.Should().Be("BadBot");
    }

    [Test]
    [Description("The database provider is looked up by the configured provider name once, unknown providers fail with a clear message")]
    public void Db_Access_Provider_Test()
    {
        var dbAccess = Substitute.For<IDbAccess>();
        var providers = Substitute.For<IIndex<string, IDbAccess>>();
        providers.TryGetValue("sqlite", out Arg.Any<IDbAccess>()).Returns(
            c =>
            {
                c[1] = dbAccess;
                return true;
            });

        var provider = this.CreateProvider(providers, "sqlite");

        provider.ProviderName.Should().Be("sqlite");
        provider.Instance.Should().BeSameAs(dbAccess);
        provider.Instance.Should().BeSameAs(dbAccess);
        this.board.Events.OfType<InitDatabaseProviderEvent>().Should().ContainSingle("the provider is initialized once");

        provider.ProviderName = "unknown";
        provider.Invoking(p => p.Instance).Should().Throw<NoValidDbAccessProviderFoundException>().WithMessage("*unknown*");
    }

    [Test]
    [Description("A database access set directly is used (and its provider name taken over)")]
    public void Db_Access_Provider_Set_Instance_Test()
    {
        var configured = Substitute.For<IDbAccess>();
        var other = Substitute.For<IDbAccess>();
        other.Information.ProviderName.Returns("other");

        var providers = Substitute.For<IIndex<string, IDbAccess>>();
        providers.TryGetValue(Arg.Any<string>(), out Arg.Any<IDbAccess>()).Returns(
            c =>
            {
                c[1] = c.ArgAt<string>(0) == "other" ? Substitute.For<IDbAccess>() : configured;
                return true;
            });

        var provider = this.CreateProvider(providers, "sqlite");

        provider.Instance = other;

        provider.ProviderName.Should().Be("other");
        provider.Instance.Should().BeSameAs(other);
    }

    [Test]
    [Description("Banned IP import: only text files, new addresses are imported, broken files are reported")]
    public async Task Banned_Ip_Import_Test()
    {
        this.board.Localization.Add("ADMIN_BANNEDIP_IMPORT", "IMPORT_SUCESS", "{0} imported")
            .Add("ADMIN_BANNEDIP_IMPORT", "IMPORT_NOTHING", "nothing")
            .Add("ADMIN_BANNEDIP_IMPORT", "IMPORT_FAILED", "failed {0}");
        var controller = this.board.CreateController<BannedIpController>();

        Notification(await controller.Import(File("1.2.3.4", "application/pdf"))).MessageType.Should().Be(MessageTypes.danger.ToString());
        Notification(await controller.Import(File("1.2.3.4\n5.6.7.*", "text/plain"))).Message.Should().Be("2 imported");
        Notification(await controller.Import(File("1.2.3.4", "text/plain"))).Message.Should().Be("nothing");

        (await this.Repository<BannedIP>().GetAsync(_ => true)).Select(b => b.Mask).Should().BeEquivalentTo(["1.2.3.4", "5.6.7.*"]);
    }

    [Test]
    [Description("Banned IP edit: invalid masks are rejected, a new id of 0 adds the mask, duplicates are reported")]
    public void Banned_Ip_Edit_Test()
    {
        this.board.Localization.Add("ADMIN_BANNEDIP_EDIT", "INVALID_ADRESS", "invalid").Add("ADMIN_BANNEDIP", "MSG_EXIST", "exists");
        this.board.BoardSettings.LogBannedIP = true;
        var controller = this.board.CreateController<BannedIpController>();

        Notification(controller.Edit(new BannedEmailEditModal { Id = 0, Mask = "not an ip" })).Message.Should().Be("invalid");
        controller.Edit(new BannedEmailEditModal { Id = 0, Mask = " 10.0.0.1 ", BanReason = " spam " }).Should().BeOfType<OkResult>();
        Notification(controller.Edit(new BannedEmailEditModal { Mask = "10.0.0.1" })).Message.Should().Be("exists");

        var ban = this.Repository<BannedIP>().GetSingle(b => b.Mask == "10.0.0.1");
        ban.Reason.Should().Be("spam");
    }

    [Test]
    [Description("Banned name edit: the mask must be a valid regular expression, duplicates are reported")]
    public void Banned_Name_Edit_Test()
    {
        this.board.Localization.Add("ADMIN_REPLACEWORDS_EDIT", "MSG_REGEX_BAD", "bad regex").Add("ADMIN_BANNEDNAME", "MSG_EXIST", "exists");
        var controller = this.board.CreateController<BannedNameController>();

        Notification(controller.Edit(new BannedEmailEditModal { Id = 0, Mask = "spam[" })).Message.Should().Be("bad regex");
        Notification(controller.Edit(new BannedEmailEditModal { Mask = null })).Message.Should().Be("bad regex");
        controller.Edit(new BannedEmailEditModal { Id = 0, Mask = " ^spammer$ " }).Should().BeOfType<OkResult>();
        Notification(controller.Edit(new BannedEmailEditModal { Mask = "^spammer$" })).Message.Should().Be("exists");

        this.Repository<BannedName>().Get(_ => true).Select(b => b.Mask).Should().Equal("^spammer$");
    }

    [Test]
    [Description("Banned name import: names are imported once, broken files are reported")]
    public async Task Banned_Name_Import_Test()
    {
        this.board.Localization.Add("ADMIN_BANNEDNAME_IMPORT", "IMPORT_SUCESS", "{0} imported")
            .Add("ADMIN_BANNEDNAME_IMPORT", "IMPORT_NOTHING", "nothing");
        var controller = this.board.CreateController<BannedNameController>();

        Notification(await controller.Import(File("x", "image/png"))).MessageType.Should().Be(MessageTypes.danger.ToString());
        Notification(await controller.Import(File("spammer\nbot", "text/plain"))).Message.Should().Be("2 imported");
        Notification(await controller.Import(File("bot", "text/plain"))).Message.Should().Be("nothing");
    }

    private static MessageModalNotification Notification(IActionResult result)
    {
        return (result as OkObjectResult)?.Value as MessageModalNotification;
    }

    private static FormFile File(string content, string contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(content);

        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "import.txt")
        {
            Headers = new HeaderDictionary(), ContentType = contentType
        };
    }

    private DbAccessProvider CreateProvider(IIndex<string, IDbAccess> providers, string configuredName)
    {
        var locator = Substitute.For<IServiceLocator>();
        locator.Get(typeof(BoardConfiguration)).Returns(new BoardConfiguration { ConnectionProviderName = configuredName });
        locator.Get(typeof(IRaiseEvent)).Returns(this.board.Events);

        return new DbAccessProvider(providers, locator);
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
