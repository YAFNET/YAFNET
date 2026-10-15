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

using YAF.Core.BBCode;
using YAF.Tests.TestInfrastructure;

/// <summary>
/// The base class of the BBCode modules (custom BBCodes rendered by code).
/// </summary>
[TestFixture]
[NonParallelizable]
public class BBCodeControlTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Unique ids start with the prefix, without prefix they are 10 characters long")]
    public void Unique_Id_Test()
    {
        BBCodeControl.GetUniqueId("spoiler").Should().StartWith("spoiler").And.HaveLength(12);
        BBCodeControl.GetUniqueId(null).Should().HaveLength(10);
        BBCodeControl.GetUniqueId("x").Should().NotBe(BBCodeControl.GetUniqueId("x"));
    }

    [Test]
    [Description("Values are html encoded outside of a forum page too")]
    public void Html_Encode_Test()
    {
        BBCodeControl.HtmlEncode(null).Should().BeNull();
        BBCodeControl.HtmlEncode("<b>\"x\"</b>").Should().NotContain("<b>").And.NotContain("\"x\"");
        BBCodeControl.HtmlEncode(42).Should().Be("42");
    }

    [Test]
    [Description("Texts of the module come from the BBCODEMODULE page, else the default text is used")]
    public async Task Localized_String_Test()
    {
        this.board.Localization.Add("BBCODEMODULE", "SHOW", "Anzeigen");

        var module = new TestModule();
        var html = new StringBuilder();

        await module.RenderAsync(html);

        html.ToString().Should().Be("Anzeigen|Hide");
        module.ServiceLocator.Should().BeSameAs(this.board.Current.ServiceLocator);
        module.Localization.Should().BeSameAs(this.board.Localization);
    }

    [Test]
    [Description("The base module renders nothing")]
    public async Task Render_Nothing_Test()
    {
        var html = new StringBuilder();

        await new BBCodeControl().RenderAsync(html);

        html.Length.Should().Be(0);
    }

    private sealed class TestModule : BBCodeControl
    {
        public override Task RenderAsync(StringBuilder stringBuilder)
        {
            stringBuilder.Append(this.LocalizedString("SHOW", "Show")).Append('|')
                .Append(this.LocalizedString("HIDE", "Hide"));

            return Task.CompletedTask;
        }
    }
}
