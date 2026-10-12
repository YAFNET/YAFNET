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
using System.Threading.Tasks;

using Autofac;

using NSubstitute;

using YAF.Core.BBCode;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

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
                b.RegisterType<BBCodeService>().As<IBBCodeService>();
                b.RegisterType<FormatMessage>().As<IFormatMessage>();
                b.RegisterType<BadWordReplace>().As<IBadWordReplace>();
            },
            useDatabase: true);

        this.board.Localization
            .Add("COMMON", "BBCODE_QUOTEWROTE", "{0} wrote:")
            .Add("COMMON", "BBCODE_QUOTEPOSTED", "Posted by {0}");

        // like the real link builder: a missing topic throws
        this.board.LinkBuilder.GetMessageLink(Arg.Any<Topic>(), Arg.Any<int>()).Returns(
            c => c.Arg<Topic>() is null
                     ? throw new NullReferenceException()
                     : $"/post/{c.ArgAt<int>(1)}");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A plain quote is rendered as a card")]
    public async Task Quote_Plain_Test()
    {
        var html = await this.FormatAsync("before [quote]quoted text[/quote] after");

        html.Should().Contain("<div class=\"card").And.Contain("quoted text");
        html.Should().StartWith("before").And.EndWith("after");
        AssertBalancedDivs(html);
    }

    [Test]
    [Description("A quote with a name shows who wrote it")]
    public async Task Quote_With_Name_Test()
    {
        var html = await this.FormatAsync("[quote=Bob]hello[/quote]");

        html.Should().Contain("hello").And.Contain("Bob wrote:");
        AssertBalancedDivs(html);
    }

    [Test]
    [Description("A quote with a post id links to the quoted post")]
    public async Task Quote_With_Post_Link_Test()
    {
        var db = this.board.Database;
        var bob = db.AddUser("Bob");
        var topic = db.AddTopic(db.AddForum(), bob);
        var message = db.AddMessage(topic, bob);

        var html = await this.FormatAsync($"[quote=Bob;{message}]hello[/quote]");

        html.Should().Contain("Posted by Bob").And.Contain($"href=\"/post/{message}\"");
        html.Should().NotContain($";{message}");
    }

    [TestCase("[quote=Bob;999999]hello[/quote]")]
    [TestCase("[quote=Bob;abc]hello[/quote]")]
    [Description("A quote with a missing or invalid post id falls back to 'wrote'")]
    public async Task Quote_With_Invalid_Post_Test(string input)
    {
        var html = await this.FormatAsync(input);

        html.Should().Contain("Bob wrote:").And.Contain("hello").And.NotContain("href=\"/post/");
    }

    [Test]
    [Description("Nested named quotes produce correctly nested HTML in the original order")]
    public async Task Quote_Nested_Named_Test()
    {
        var html = await this.FormatAsync("[quote=Alice]outer start [quote=Bob]inner[/quote] outer end[/quote] reply");

        AssertBalancedDivs(html);
        AssertOrder(html, "outer start", "inner", "Bob wrote:", "outer end", "Alice wrote:", "reply");
    }

    [Test]
    [Description("A plain quote inside a named quote stays inside it")]
    public async Task Quote_Nested_Mixed_Test()
    {
        var html = await this.FormatAsync("[quote=Alice]outer [quote]inner[/quote] end[/quote]");

        AssertBalancedDivs(html);
        AssertOrder(html, "outer", "inner", "end", "Alice wrote:");
    }

    [Test]
    [Description("Several quotes in a row are each rendered")]
    public async Task Quote_Sequential_Test()
    {
        var html = await this.FormatAsync("[quote=Alice]one[/quote] middle [quote=Bob]two[/quote]");

        AssertBalancedDivs(html);
        AssertOrder(html, "one", "Alice wrote:", "middle", "two", "Bob wrote:");
    }

    [Test]
    [Description("The quote rule loads the quoted post asynchronously and has no blocking synchronous path")]
    public async Task Quote_Rule_Is_Async_Only_Test()
    {
        var rule = new YAF.Core.BBCode.ReplaceRules.QuoteRegexReplaceRule(
            @"\[quote(?:=(?<quote>[^\]]*))?\](?<inner>(?:(?!\[quote[=\]]).)*?)\[/quote\]",
            "<q>${quoteText}</q>${quote}",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        var text = "[quote=Bob]hi[/quote]";

        FluentActions.Invoking(() => rule.Replace(ref text, new ReplaceBlocksCollection()))
            .Should().Throw<NotSupportedException>();

        // a rule's output holds placeholders for its html, the engine puts the html back in
        var engine = new ProcessReplaceRules();
        engine.AddRule(rule);

        (await engine.ProcessAsync(text)).Should().StartWith("<q>hi</q>").And.Contain("Bob wrote:");
    }

    [TestCase("[quote=<script>alert(1)</script>]x[/quote]")]
    [TestCase("[quote=Bob\" onmouseover=\"alert(1)]x[/quote]")]
    [TestCase("[quote=Bob;1\" onmouseover=\"alert(1)]x[/quote]")]
    [Description("Quote names can't inject HTML")]
    public async Task Quote_Xss_Test(string input)
    {
        var html = await this.FormatAsync(input);

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
    public async Task ConvertHtmlToBBCode_Test(string html, string expected)
    {
        (await this.Service.ConvertHtmlToBBCodeForEditAsync(html)).Should().Be(expected);
    }

    [Test]
    [Description("Lists become BBCode lists")]
    public async Task ConvertHtmlToBBCode_Lists_Test()
    {
        var bulleted = await this.Service.ConvertHtmlToBBCodeForEditAsync("<ul><li>a</li><li>b</li></ul>");

        bulleted.Should().Be("[list][*]a\r\n[*]b\r\n[/list]");
        (await this.Service.MakeHtmlAsync(bulleted, true, false)).Should().Contain("<li>a</li>").And.Contain("<li>b</li>");

        (await this.Service.ConvertHtmlToBBCodeForEditAsync("<ol type=\"1\"><li>a</li></ol>")).Should().Be("[list=1][*]a\r\n[/list]");
        (await this.Service.ConvertHtmlToBBCodeForEditAsync("<ol type=\"a\"><li>a</li></ol>")).Should().Be("[list=a][*]a\r\n[/list]");
        (await this.Service.ConvertHtmlToBBCodeForEditAsync("<ol style=\"list-style-type:upper-roman\"><li>a</li></ol>"))
            .Should().Be("[list=I][*]a\r\n[/list]");
    }

    [Test]
    [Description("Formatting inside links, list items and alignment blocks is converted too")]
    public async Task ConvertHtmlToBBCode_Nested_Formatting_Test()
    {
        (await this.Service.ConvertHtmlToBBCodeForEditAsync("<a href=\"https://example.com\"><strong>bold link</strong></a>"))
            .Should().Be("[url=https://example.com][b]bold link[/b][/url]");

        (await this.Service.ConvertHtmlToBBCodeForEditAsync("<ul><li><em>item</em><br />second line</li></ul>"))
            .Should().Be("[list][*][i]item[/i]\r\nsecond line\r\n[/list]");

        (await this.Service.ConvertHtmlToBBCodeForEditAsync("<div align=\"center\"><u>title</u></div>"))
            .Should().Be("[center][u]title[/u][/center]");
    }

    [Test]
    [Description("Code blocks become [code] with their language")]
    public async Task ConvertHtmlToBBCode_Code_Test()
    {
        (await this.Service.ConvertHtmlToBBCodeForEditAsync("<div class=\"code\"><div class=\"innercode\">var x;</div></div>"))
            .Should().Be("[code]var x;[/code]");

        (await this.Service.ConvertHtmlToBBCodeForEditAsync(
                "<div class=\"code\"><div class=\"innercode\"><pre class=\"brush:csharp;\">var x;</pre></div></div>"))
            .Should().Be("[code=csharp]var x;[/code]");
    }

    [Test]
    [Description("Notes keep their type")]
    public async Task ConvertHtmlToBBCode_Note_Test()
    {
        (await this.Service.ConvertHtmlToBBCodeForEditAsync("<div class=\"alert alert-warning\" role=\"alert\">careful</div>"))
            .Should().Be("[note=warning]careful[/note]");
    }

    [Test]
    [Description("Paragraphs become line breaks, no [br] codes (there is no such BBCode, [br] is an emoji short code)")]
    public async Task ConvertHtmlToBBCode_Paragraphs_Test()
    {
        var bbCode = await this.Service.ConvertHtmlToBBCodeForEditAsync("<p>first</p><p>second</p>");

        bbCode.Should().NotContain("[br]");
        bbCode.Should().Contain("first").And.Contain("second");
        bbCode.IndexOf("second", StringComparison.Ordinal).Should()
            .BeGreaterThan(bbCode.IndexOf("\r\n", bbCode.IndexOf("first", StringComparison.Ordinal), StringComparison.Ordinal));
    }

    [Test]
    [Description("Typical HTML editor output becomes BBCode that renders the same way again")]
    public async Task ConvertHtmlToBBCode_Editor_Html_Test()
    {
        const string editorHtml = "<p>Hello <strong>World</strong></p><p>See <a href=\"https://example.com\">this</a></p>";

        var bbCode = await this.Service.ConvertHtmlToBBCodeForEditAsync(editorHtml);

        bbCode.Should().Be("\r\n\r\nHello [b]World[/b]\r\n\r\nSee [url=https://example.com]this[/url]");

        var html = await this.Service.MakeHtmlAsync(bbCode, true, false);
        html.Should().Contain("<strong>World</strong>").And.Contain("href=\"https://example.com\"");
    }

    private IBBCodeService Service => this.board.Resolve<IBBCodeService>();

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

    private Task<string> FormatAsync(string message)
    {
        return this.board.Resolve<IFormatMessage>().FormatMessageWithAllBBCodesAsync(message, 1, 2, false, null);
    }
}
