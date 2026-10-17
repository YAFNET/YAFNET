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

using System.Web.UI;

using Autofac;

using ServiceStack.OrmLite;

using YAF.Core.BBCode;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;

using BBCodeModel = YAF.Types.Models.BBCode;

/// <summary>
/// Custom BBCodes with a module class (rendered by a <see cref="BBCodeControl"/>).
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
                b.RegisterType<YAF.Core.BBCode.BBCode>().As<IBBCode>();
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
    public void Module_BBCode_Test()
    {
        this.AddCode("SHOUT", @"\[shout=(?<level>\d)\](?<inner>.*?)\[/shout\]", typeof(ShoutModule), "level");

        var html = this.Format("a [shout=2]hi[/shout] b [shout=3]yo[/shout]", 5);

        html.Should().Be("a <b data-level=\"2\" data-message=\"5\">HI</b> b <b data-level=\"3\" data-message=\"5\">YO</b>");
    }

    [Test]
    [Description("One custom BBCode with an invalid search expression does not break the other BBCodes or the post")]
    public void Invalid_Search_Regex_Test()
    {
        this.AddCode("BROKEN", "([", typeof(ShoutModule));
        this.AddCode("SHOUT", @"\[shout\](?<inner>.*?)\[/shout\]", typeof(ShoutModule));

        var html = this.Format("text [shout]hi[/shout]", 1);

        html.Should().Contain("HI").And.StartWith("text ");
    }

    [Test]
    [Description("A custom BBCode whose module class does not exist leaves the text as it is")]
    public void Missing_Module_Class_Test()
    {
        this.AddCode("GONE", @"\[gone\](?<inner>.*?)\[/gone\]", moduleClass: "YAF.Does.Not.Exist, YAF.Nowhere");

        var html = this.Format("keep [gone]this[/gone]", 1);

        html.Should().Be("keep [gone]this[/gone]");
    }

    private string Format(string message, int messageId)
    {
        return this.board.Resolve<IBBCode>().FormatMessageWithCustomBBCode(
            message,
            new MessageFlags { IsBBCode = true },
            7,
            messageId);
    }

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
        protected override void Render(HtmlTextWriter writer)
        {
            this.Parameters.TryGetValue("level", out var level);

            writer.Write(
                level.IsSet()
                    ? $"<b data-level=\"{level}\" data-message=\"{this.MessageID}\">{this.Parameters["inner"].ToUpperInvariant()}</b>"
                    : this.Parameters["inner"].ToUpperInvariant());
        }
    }
}
