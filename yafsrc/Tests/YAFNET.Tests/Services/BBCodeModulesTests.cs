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

using System.Threading.Tasks;

using Autofac;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.BBCode;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models.Identity;
using YAF.Web.BBCodes;

using BBCodeModel = YAF.Types.Models.BBCode;

/// <summary>
/// The BBCode modules of the forum (hidden content and attachments) with the search expressions of the
/// installation, rendered like in a post.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BBCodeModulesTests
{
    private const string HidePosts = @"\[hide-posts=(?<posts>([0-9]*))\](?<inner>(.|\n)*?)\[\/hide-posts\]";

    private const string HideThanks = @"\[hide-thanks=(?<thanks>([0-9]*))\](?<inner>(.|\n)*?)\[\/hide-thanks\]";

    private const string AttachRegex = @"\[attach\](?<inner>.+?)\[\/attach\]";

    private const string AlbumImageRegex = @"\[albumimg\](?<inner>.+?)\[\/albumimg\]";

    private const string GroupHideRegex =
        @"\[group-hide\](?<inner>(.|\n)*?)\[\/group-hide\]|\[group-hide=(?<group>([^""\r\n\]\[]*?))\](?<inner>(.|\n)*?)\[\/group-hide\]";

    private readonly List<string> readerRoles = [];

    private TestBoardContext board;

    private TestDatabase db;

    private int author;

    private int reader;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.Register<Func<IEnumerable<bool>, IProcessReplaceRules>>(_ => _ => new ProcessReplaceRules());
                b.RegisterType<BBCodeService>().As<IBBCodeService>();

                // the roles of the reader
                var roles = Substitute.For<IAspNetRolesHelper>();
                roles.GetRolesForUserAsync(Arg.Any<AspNetUsers>())
                    .Returns(_ => Task.FromResult<IList<string>>(this.readerRoles));
                b.RegisterInstance(roles).As<IAspNetRolesHelper>();
            },
            useDatabase: true);

        this.readerRoles.Clear();

        this.db = this.board.Database;
        this.author = this.db.AddUser("Author");
        this.reader = this.db.AddUser("Reader");
        this.board.LoginAs(this.reader);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [TestCase(null, 0)]
    [TestCase("", 0)]
    [TestCase("5", 5)]
    [TestCase("99999999999", int.MaxValue)]
    [Description("The number of a hide BBCode can be empty (0) or too big (nobody but the author and the admins)")]
    public void ParseCount_Test(string value, int expected)
    {
        HideBBCodeModule.ParseCount(value).Should().Be(expected);
    }

    [TestCase("[hide-posts=]hidden text[/hide-posts]", true)]
    [TestCase("[hide-posts=99999999999]hidden text[/hide-posts]", false)]
    [TestCase("[hide-thanks=]hidden text[/hide-thanks]", true)]
    [Description("A hide BBCode with an empty (0) or too big number (nobody) does not break the post")]
    public async Task Hide_Invalid_Number_Test(string message, bool shown)
    {
        this.AddCode("HIDE-POSTS", HidePosts, typeof(HideBBCodeModule), "posts");
        this.AddCode("HIDE-THANKS", HideThanks, typeof(HideBBCodeModule), "thanks");

        var html = await this.FormatAsync($"before {message} after");

        html.Should().StartWith("before ").And.EndWith(" after");

        if (shown)
        {
            html.Should().Contain("hidden text");
        }
        else
        {
            html.Should().NotContain("hidden text");
        }
    }

    [Test]
    [Description("Hide-thanks shows the content to users who received enough thanks (not who gave them)")]
    public async Task Hide_Thanks_Counts_Received_Thanks_Test()
    {
        this.AddCode("HIDE-THANKS", HideThanks, typeof(HideBBCodeModule), "thanks");

        var topic = this.db.AddTopic(this.db.AddForum(), this.author);
        var authorsPost = this.db.AddMessage(topic, this.author);
        var readersPost = this.db.AddMessage(topic, this.reader);

        // the reader gave a thanks
        this.board.Current.GetRepository<YAF.Types.Models.Thanks>().AddMessageThanks(this.reader, this.author, authorsPost);

        (await this.FormatAsync("[hide-thanks=1]secret[/hide-thanks]")).Should().NotContain("secret", "the reader received no thanks");

        // the reader received a thanks
        this.board.Current.GetRepository<YAF.Types.Models.Thanks>().AddMessageThanks(this.author, this.reader, readersPost);

        (await this.FormatAsync("[hide-thanks=1]secret[/hide-thanks]")).Should().Contain("secret");
    }

    [TestCase("1.5")]
    [TestCase("99999999999")]
    [TestCase("(5)")]
    [TestCase("1e3")]
    [Description("An attachment or album image id that is no whole number does not break the post")]
    public async Task Invalid_Id_Test(string id)
    {
        this.AddCode("ATTACH", AttachRegex, typeof(Attach));
        this.AddCode("ALBUMIMG", AlbumImageRegex, typeof(AlbumImage));

        var html = await this.FormatAsync($"before [attach]{id}[/attach] [albumimg]{id}[/albumimg] after");

        html.Should().StartWith("before ").And.EndWith(" after");
    }

    private Task<string> FormatAsync(string message)
    {
        return this.board.Resolve<IBBCodeService>().FormatMessageWithCustomBBCodeAsync(message, 1, this.author);
    }

    private void AddCode(string name, string search, Type module, string variables = null)
    {
        using var connection = this.db.Open();

        connection.Insert(
            new BBCodeModel
            {
                BoardID = TestDatabase.BoardId,
                Name = name,
                SearchRegex = search,
                UseModule = true,
                ModuleClass = module.AssemblyQualifiedName,
                Variables = variables,
                ExecOrder = 1
            });
    }
}
