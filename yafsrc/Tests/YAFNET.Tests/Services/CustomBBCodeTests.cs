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

using System.Text;
using System.Threading.Tasks;

using Autofac;

using ServiceStack.OrmLite;

using YAF.Core.BBCode;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;

using BBCodeModel = YAF.Types.Models.BBCode;

/// <summary>
/// Custom BBCodes with a module class (rendered by a <see cref="BBCodeControl"/>) and their inline scripts and styles.
/// </summary>
[TestFixture]
[NonParallelizable]
public class CustomBBCodeTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.Register<Func<IEnumerable<bool>, IProcessReplaceRules>>(_ => _ => new ProcessReplaceRules());
                b.RegisterType<BBCodeService>().As<IBBCodeService>();
            },
            useDatabase: true);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A module BBCode is rendered by its module with the inner text and variables, every occurrence")]
    public async Task Module_BBCode_Test()
    {
        this.AddCode("SHOUT", @"\[shout=(?<level>\d)\](?<inner>.*?)\[/shout\]", typeof(ShoutModule), "level");

        var html = await this.Service.FormatMessageWithCustomBBCodeAsync("a [shout=2]hi[/shout] b [shout=3]yo[/shout]", 5, 7);

        html.Should().Be("a <b data-level=\"2\" data-message=\"5\">HI</b> b <b data-level=\"3\" data-message=\"5\">YO</b>");
    }

    [Test]
    [Description("One custom BBCode with an invalid search expression does not break the other BBCodes or the post")]
    public async Task Invalid_Search_Regex_Test()
    {
        this.AddCode("BROKEN", "([", typeof(ShoutModule));
        this.AddCode("SHOUT", @"\[shout\](?<inner>.*?)\[/shout\]", typeof(ShoutModule));

        var html = await this.Service.FormatMessageWithCustomBBCodeAsync("text [shout]hi[/shout]", 1, 1);

        html.Should().Contain("HI").And.StartWith("text ");
    }

    [Test]
    [Description("A custom BBCode whose module class does not exist leaves the text as it is")]
    public async Task Missing_Module_Class_Test()
    {
        this.AddCode("GONE", @"\[gone\](?<inner>.*?)\[/gone\]", moduleClass: "YAF.Does.Not.Exist, YAF.Nowhere");

        var html = await this.Service.FormatMessageWithCustomBBCodeAsync("keep [gone]this[/gone]", 1, 1);

        html.Should().Be("keep [gone]this[/gone]");
    }

    [Test]
    [Description("Display scripts, edit scripts (with the editor id) and css of custom BBCodes are registered on the page")]
    public void Inline_Elements_Test()
    {
        using (var connection = this.board.Database.Open())
        {
            connection.Insert(
                new BBCodeModel
                {
                    BoardID = TestDatabase.BoardId,
                    Name = "STYLED",
                    SearchRegex = "x",
                    ExecOrder = 1,
                    DisplayJS = "show();",
                    EditJS = "edit('{editorid}');",
                    DisplayCSS = ".styled { color: red; }"
                });
        }

        this.Service.RegisterCustomBBCodeInlineElements("editor1");

        string.Join("\n", this.board.Current.InlineElements.Items.Select(i => i.Code))
            .Should().Contain("show();").And.Contain("edit('editor1');").And.Contain(".styled");
    }

    private IBBCodeService Service => this.board.Resolve<IBBCodeService>();

    private void AddCode(string name, string search, Type module = null, string variables = null, string moduleClass = null)
    {
        using var connection = this.board.Database.Open();

        connection.Insert(
            new BBCodeModel
            {
                BoardID = TestDatabase.BoardId,
                Name = name,
                SearchRegex = search,
                UseModule = true,
                ModuleClass = moduleClass ?? module!.AssemblyQualifiedName,
                Variables = variables,
                ExecOrder = 1
            });
    }

    /// <summary>
    /// A test module: shouts the inner text.
    /// </summary>
    public class ShoutModule : BBCodeControl
    {
        public override Task RenderAsync(StringBuilder stringBuilder)
        {
            this.Parameters.TryGetValue("level", out var level);

            stringBuilder.Append(
                level.IsSet()
                    ? $"<b data-level=\"{level}\" data-message=\"{this.MessageID}\">{this.Parameters["inner"].ToUpperInvariant()}</b>"
                    : this.Parameters["inner"].ToUpperInvariant());

            return Task.CompletedTask;
        }
    }
}
