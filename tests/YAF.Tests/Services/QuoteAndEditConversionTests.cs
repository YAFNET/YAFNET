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

using System.Text.RegularExpressions;

using Autofac;

using YAF.Core.BBCode;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Services;

/// <summary>
/// Quote rendering and the HTML to BBCode conversion used when editing / quoting HTML posts.
/// </summary>
[TestFixture]
[NonParallelizable]
public class QuoteAndEditConversionTests
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
                b.RegisterType<FormatMessage>().As<IFormatMessage>();
                b.RegisterType<BadWordReplace>().As<IBadWordReplace>();
            },
            useDatabase: true);

        this.board.Localization
            .Add("COMMON", "BBCODE_QUOTEWROTE", "{0} wrote:")
            .Add("COMMON", "BBCODE_QUOTEPOSTED", "Posted by {0}");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A plain quote is rendered as a card")]
    public void Quote_Plain_Test()
    {
        var html = this.Format("before [quote]quoted text[/quote] after");

        html.Should().Contain("<div class=\"card").And.Contain("quoted text");
        html.Should().StartWith("before").And.EndWith("after");
        AssertBalancedDivs(html);
    }

    [Test]
    [Description("A quote with a name shows who wrote it")]
    public void Quote_With_Name_Test()
    {
        var html = this.Format("[quote=Bob]hello[/quote]");

        html.Should().Contain("hello").And.Contain("Bob wrote:");
        AssertBalancedDivs(html);
    }

    [Test]
    [Description("A quote with a post id links to the quoted post")]
    public void Quote_With_Post_Link_Test()
    {
        var db = this.board.Database;
        var bob = db.AddUser("Bob");
        var topic = db.AddTopic(db.AddForum(), bob);
        var message = db.AddMessage(topic, bob);

        var html = this.Format($"[quote=Bob;{message}]hello[/quote]");

        html.Should().Contain("Posted by Bob").And.Contain($"m={message}");
        html.Should().NotContain($";{message}");
    }

    [TestCase("[quote=Bob;999999]hello[/quote]")]
    [TestCase("[quote=Bob;abc]hello[/quote]")]
    [Description("A quote with a missing or invalid post id falls back to 'wrote'")]
    public void Quote_With_Invalid_Post_Test(string input)
    {
        var html = this.Format(input);

        html.Should().Contain("Bob wrote:").And.Contain("hello").And.NotContain("card-link");
    }

    [Test]
    [Description("Nested named quotes produce correctly nested HTML in the original order")]
    public void Quote_Nested_Named_Test()
    {
        var html = this.Format("[quote=Alice]outer start [quote=Bob]inner[/quote] outer end[/quote] reply");

        AssertBalancedDivs(html);
        AssertOrder(html, "outer start", "inner", "Bob wrote:", "outer end", "Alice wrote:", "reply");
    }

    [Test]
    [Description("A plain quote inside a named quote stays inside it")]
    public void Quote_Nested_Mixed_Test()
    {
        var html = this.Format("[quote=Alice]outer [quote]inner[/quote] end[/quote]");

        AssertBalancedDivs(html);
        AssertOrder(html, "outer", "inner", "end", "Alice wrote:");
    }

    [Test]
    [Description("Several quotes in a row are each rendered")]
    public void Quote_Sequential_Test()
    {
        var html = this.Format("[quote=Alice]one[/quote] middle [quote=Bob]two[/quote]");

        AssertBalancedDivs(html);
        AssertOrder(html, "one", "Alice wrote:", "middle", "two", "Bob wrote:");
    }

    [TestCase("[quote=<script>alert(1)</script>]x[/quote]")]
    [TestCase("[quote=Bob\" onmouseover=\"alert(1)]x[/quote]")]
    [TestCase("[quote=Bob;1\" onmouseover=\"alert(1)]x[/quote]")]
    [Description("Quote names can't inject HTML")]
    public void Quote_Xss_Test(string input)
    {
        var html = this.Format(input);

        html.Should().NotContainEquivalentOf("<script");
        html.Should().NotMatchRegex("(?i)<[^<>]*\\son[a-z]+\\s*=\\s*[\"']?alert");
    }

    [TestCase("<strong>bold</strong> <b>also</b>", "[b]bold[/b] [b]also[/b]")]
    [TestCase("<em>italic</em>", "[i]italic[/i]")]
    [TestCase("<u>under</u>", "[u]under[/u]")]
    [TestCase("<span style=\"text-decoration: underline;\">under</span>", "[u]under[/u]")]
    [TestCase("<s>strike</s>", "[s]strike[/s]")]
    [TestCase("<span class=\"highlight\">mark</span>", "[h]mark[/h]")]
    [TestCase("<div align=\"center\">c</div>", "[center]c[/center]")]
    [TestCase("<p style=\"text-align: right;\">r</p>", "[right]r[/right]")]
    [TestCase("<div style=\"margin-left:40px\">i</div>", "[indent]i[/indent]")]
    [TestCase("<span style=\"font-size:20px;\">big</span>", "[size=20]big[/size]")]
    [TestCase("<span style=\"font-family: arial;\">font</span>", "[font=arial]font[/font]")]
    [TestCase("<span style=\"color: #ff0000;\">red</span>", "[color=#ff0000]red[/color]")]
    [TestCase("<a href=\"https://example.com\" target=\"_blank\">link</a>", "[url=https://example.com]link[/url]")]
    [TestCase("<a href=\"mailto:john@example.com\">John</a>", "[email=john@example.com]John[/email]")]
    [TestCase("<img src=\"https://example.com/a.png\" alt=\"pic\" />", "[img=https://example.com/a.png]pic[/img]")]
    [TestCase("line 1<br />line 2<br>line 3", "line 1\r\nline 2\r\nline 3")]
    [TestCase("<span class=\"unknown\">kept</span>&nbsp;text", "kepttext")]
    [Description("HTML is converted back to BBCode for editing")]
    public void ConvertHtmlToBBCode_Test(string html, string expected)
    {
        (this.Service.ConvertHtmlToBBCodeForEdit(html)).Should().Be(expected);
    }

    [Test]
    [Description("Lists become BBCode lists")]
    public void ConvertHtmlToBBCode_Lists_Test()
    {
        var bulleted = this.Service.ConvertHtmlToBBCodeForEdit("<ul><li>a</li><li>b</li></ul>");

        bulleted.Should().Be("[list][*]a\r\n[*]b\r\n[/list]");
        (this.Service.MakeHtml(bulleted, true, false)).Should().Contain("<li>a</li>").And.Contain("<li>b</li>");

        (this.Service.ConvertHtmlToBBCodeForEdit("<ol type=\"1\"><li>a</li></ol>")).Should().Be("[list=1][*]a\r\n[/list]");
        (this.Service.ConvertHtmlToBBCodeForEdit("<ol type=\"a\"><li>a</li></ol>")).Should().Be("[list=a][*]a\r\n[/list]");
        (this.Service.ConvertHtmlToBBCodeForEdit("<ol style=\"list-style-type:upper-roman\"><li>a</li></ol>"))
            .Should().Be("[list=I][*]a\r\n[/list]");
    }

    [Test]
    [Description("Formatting inside links, list items and alignment blocks is converted too")]
    public void ConvertHtmlToBBCode_Nested_Formatting_Test()
    {
        (this.Service.ConvertHtmlToBBCodeForEdit("<a href=\"https://example.com\"><strong>bold link</strong></a>"))
            .Should().Be("[url=https://example.com][b]bold link[/b][/url]");

        (this.Service.ConvertHtmlToBBCodeForEdit("<ul><li><em>item</em><br />second line</li></ul>"))
            .Should().Be("[list][*][i]item[/i]\r\nsecond line\r\n[/list]");

        (this.Service.ConvertHtmlToBBCodeForEdit("<div align=\"center\"><u>title</u></div>"))
            .Should().Be("[center][u]title[/u][/center]");
    }

    [Test]
    [Description("Code blocks become [code] with their language")]
    public void ConvertHtmlToBBCode_Code_Test()
    {
        (this.Service.ConvertHtmlToBBCodeForEdit("<div class=\"code\"><div class=\"innercode\">var x;</div></div>"))
            .Should().Be("[code]var x;[/code]");

        (this.Service.ConvertHtmlToBBCodeForEdit(
                "<div class=\"code\"><div class=\"innercode\"><pre class=\"brush:csharp;\">var x;</pre></div></div>"))
            .Should().Be("[code=csharp]var x;[/code]");
    }

    [Test]
    [Description("Notes keep their type")]
    public void ConvertHtmlToBBCode_Note_Test()
    {
        (this.Service.ConvertHtmlToBBCodeForEdit("<div class=\"alert alert-warning\" role=\"alert\">careful</div>"))
            .Should().Be("[note=warning]careful[/note]");
    }

    [Test]
    [Description("Paragraphs become line breaks, no [br] codes (there is no such BBCode, [br] is an emoji short code)")]
    public void ConvertHtmlToBBCode_Paragraphs_Test()
    {
        var bbCode = this.Service.ConvertHtmlToBBCodeForEdit("<p>first</p><p>second</p>");

        bbCode.Should().NotContain("[br]");
        bbCode.Should().Contain("first").And.Contain("second");
        bbCode.IndexOf("second", StringComparison.Ordinal).Should()
            .BeGreaterThan(bbCode.IndexOf("\r\n", bbCode.IndexOf("first", StringComparison.Ordinal), StringComparison.Ordinal));
    }

    [Test]
    [Description("Typical HTML editor output becomes BBCode that renders the same way again")]
    public void ConvertHtmlToBBCode_Editor_Html_Test()
    {
        const string editorHtml = "<p>Hello <strong>World</strong></p><p>See <a href=\"https://example.com\">this</a></p>";

        var bbCode = this.Service.ConvertHtmlToBBCodeForEdit(editorHtml);

        bbCode.Should().Be("\r\n\r\nHello [b]World[/b]\r\n\r\nSee [url=https://example.com]this[/url]");

        var html = this.Service.MakeHtml(bbCode, true, false);
        html.Should().Contain("<strong>World</strong>").And.Contain("href=\"https://example.com\"");
    }

    private IBBCode Service => this.board.Resolve<IBBCode>();

    private static void AssertBalancedDivs(string html)
    {
        Regex.Matches(html, "<div[\\s>]").Count.Should().Be(Regex.Matches(html, "</div>").Count, html);

        // never close a div that wasn't opened yet
        var depth = 0;

        foreach (Match tag in Regex.Matches(html, "<div[\\s>]|</div>"))
        {
            depth += tag.Value.StartsWith("</") ? -1 : 1;
            depth.Should().BeGreaterThanOrEqualTo(0, html);
        }
    }

    private static void AssertOrder(string html, params string[] parts)
    {
        var position = -1;

        foreach (var part in parts)
        {
            var next = html.IndexOf(part, position + 1, StringComparison.Ordinal);
            next.Should().BeGreaterThan(position, $"'{part}' should come after the previous part in:\n{html}");
            position = next;
        }
    }

    private string Format(string message)
    {
        // like a post of the bb code editor
        return this.board.Resolve<IFormatMessage>().Format(
            1,
            message,
            new MessageFlags { IsBBCode = true, IsHtml = false },
            false,
            DateTime.UtcNow);
    }
}
