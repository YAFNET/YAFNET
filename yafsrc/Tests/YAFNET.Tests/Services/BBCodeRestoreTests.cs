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
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Core.Services.Import;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The BBCode extensions of the forum (BBCodeExtensions.xml): the required ones and the restore of an extension.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BBCodeRestoreTests
{
    private const int Board = TestDatabase.BoardId;

    private TestBoardContext board;

    private IDataImporter importer;

    private string webRoot;

    [SetUp]
    public void SetUp()
    {
        // the web root with the resources/BBCodeExtensions.xml of the forum
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-bbcode-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(this.webRoot, "resources"));

        File.WriteAllText(
            Path.Combine(this.webRoot, "resources", "BBCodeExtensions.xml"),
            CodeList(("ATTACH", "attachment"), ("MEDIA", "media")));

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<DataImporter>().As<IDataImporter>();
            },
            useDatabase: true);

        this.importer = this.board.Resolve<IDataImporter>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();

        if (Directory.Exists(this.webRoot))
        {
            Directory.Delete(this.webRoot, true);
        }
    }

    [Test]
    [Description("The forum needs the attachments, the media, the member links and the album images: they can't be deleted")]
    public void Required_Extensions_Test()
    {
        BBCodeHelper.IsRequiredExtension("ATTACH").Should().BeTrue();
        BBCodeHelper.IsRequiredExtension("attach ").Should().BeTrue();
        BBCodeHelper.IsRequiredExtension("USERLINK").Should().BeTrue();
        BBCodeHelper.IsRequiredExtension("ALBUMIMG").Should().BeTrue();
        BBCodeHelper.IsRequiredExtension("MEDIA").Should().BeTrue("the media button of the editor inserts it");
        BBCodeHelper.IsRequiredExtension("YOUTUBE").Should().BeFalse();
        BBCodeHelper.IsRequiredExtension(null).Should().BeFalse();
    }

    [Test]
    [Description("The names of the extensions of the forum are read from BBCodeExtensions.xml")]
    public void Default_Names_Test()
    {
        this.importer.DefaultBBCodeExtensionNames().Should().Equal("ATTACH", "MEDIA");
    }

    [Test]
    [Description("The upgrade adds an extension the admin deleted again")]
    public async Task Upgrade_Adds_A_Deleted_Extension_Test()
    {
        await this.InstallAsync();

        var codes = this.board.Current.GetRepository<BBCode>();
        await codes.DeleteAsync(c => c.Name == "ATTACH");

        await this.InstallAsync();

        (await codes.GetSingleAsync(c => c.Name == "ATTACH")).ReplaceRegex.Should().Be("attachment");
    }

    [Test]
    [Description("The restore replaces the changes of the admin (the upgrade keeps them), the other extensions are not touched")]
    public async Task Restore_Replaces_The_Changes_Of_The_Admin_Test()
    {
        await this.InstallAsync();

        var codes = this.board.Current.GetRepository<BBCode>();

        foreach (var code in await codes.GetAsync(c => c.BoardID == Board))
        {
            code.ReplaceRegex = "admin";
            await codes.UpdateAsync(code);
        }

        (await this.importer.BBCodeExtensionRestoreAsync(Board, "attach")).Should().BeTrue();

        (await codes.GetSingleAsync(c => c.Name == "ATTACH")).ReplaceRegex.Should().Be("attachment");
        (await codes.GetSingleAsync(c => c.Name == "MEDIA")).ReplaceRegex.Should().Be("admin");

        // the restored version is the installed one again: the next upgrade updates it
        File.WriteAllText(
            Path.Combine(this.webRoot, "resources", "BBCodeExtensions.xml"),
            CodeList(("ATTACH", "attachment2"), ("MEDIA", "media2")));

        await this.InstallAsync();

        (await codes.GetSingleAsync(c => c.Name == "ATTACH")).ReplaceRegex.Should().Be("attachment2");
        (await codes.GetSingleAsync(c => c.Name == "MEDIA")).ReplaceRegex.Should().Be("admin");
    }

    [Test]
    [Description("An extension the forum does not have can't be restored")]
    public async Task Restore_Of_An_Own_Extension_Test()
    {
        (await this.importer.BBCodeExtensionRestoreAsync(Board, "MYCODE")).Should().BeFalse();
        (await this.importer.BBCodeExtensionRestoreAsync(Board, " ")).Should().BeFalse();
    }

    private async Task InstallAsync()
    {
        await using var stream = File.OpenRead(Path.Combine(this.webRoot, "resources", "BBCodeExtensions.xml"));

        await this.importer.BBCodeExtensionImportAsync(Board, stream, true);
    }

    private static string CodeList(params (string Name, string Replace)[] codes)
    {
        return "<YafBBCodeList>" + string.Concat(
                   codes.Select(c => $"<YafBBCode><Name>{c.Name}</Name><Description>d</Description><SearchRegex>c</SearchRegex><ReplaceRegex>{c.Replace}</ReplaceRegex><ExecOrder>1</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>false</UseToolbar></YafBBCode>"))
               + "</YafBBCodeList>";
    }
}
