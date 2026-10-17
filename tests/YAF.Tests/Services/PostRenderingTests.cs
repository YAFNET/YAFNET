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

using Autofac;

using YAF.Core.BBCode;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// Post rendering: FormatMessage + BBCodeService with the real rule engine.
/// </summary>
[TestFixture]
[NonParallelizable]
public class PostRenderingTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                // a fresh rule engine per call (production caches one per flag combination)
                b.Register<Func<IEnumerable<bool>, IProcessReplaceRules>>(_ => _ => new ProcessReplaceRules());
                b.RegisterType<YAF.Core.BBCode.BBCode>().As<IBBCode>();
                b.RegisterType<FormatMessage>().As<IFormatMessage>();
                b.RegisterType<BadWordReplace>().As<IBadWordReplace>();
            },
            useDatabase: true);

        this.board.BoardSettings.UseNoFollowLinks = false;
        this.board.BoardSettings.BlankLinks = false;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [TestCase("[b]bold[/b]", "<strong>bold</strong>")]
    [TestCase("[B]bold[/B]", "<strong>bold</strong>")]
    [TestCase("[i]italic[/i]", "<em>italic</em>")]
    [TestCase("[u]underline[/u]", "<u>underline</u>")]
    [TestCase("[s]strike[/s]", "<s>strike</s>")]
    [TestCase("[h]mark[/h]", "<mark>mark</mark>")]
    [TestCase("[size=5]big[/size]", "<span style=\"font-size:")]
    [TestCase("[size=1]small[/size]", "<span style=\"font-size:50%\">small</span>")]
    [TestCase("[size=9]large[/size]", "<span style=\"font-size:180%\">large</span>")]
    [TestCase("[size=150]big[/size]", "<span style=\"font-size:150%\">big</span>")]
    [TestCase("[size=99999]huge[/size]", "<span style=\"font-size:400%\">huge</span>")]
    [TestCase("[color=red]red[/color]", "<span style=\"color:red\">red</span>")]
    [TestCase("[color=#ff0000]red[/color]", "<span style=\"color:#ff0000\">red</span>")]
    [TestCase("[font=arial]text[/font]", "<span style=\"font-family:arial\">text</span>")]
    [TestCase("[center]c[/center]", "<div align=\"center\">c</div>")]
    [TestCase("[email]john@example.com[/email]", "<a href=\"mailto:john@example.com\">john@example.com</a>")]
    [Description("Basic BBCodes are turned into HTML")]
    public void Basic_BBCodes_Test(string input, string expected)
    {
        (this.Format(input)).Should().Contain(expected);
    }

    [Test]
    [Description("Links get href and title, the external icon, and no target/nofollow by default")]
    public void Url_Test()
    {
        var html = this.Format("[url=https://www.yetanotherforum.net]YAF[/url]");

        html.Should().Contain("href=\"https://www.yetanotherforum.net\"");
        html.Should().Contain(">YAF&nbsp;<i class=\"fa fa-external-link-alt\"></i></a>");
        html.Should().NotContain("target=\"_blank\"").And.NotContain("nofollow");

        (this.Format("[url]https://example.com/page[/url]")).Should()
            .Contain("href=\"https://example.com/page\"");
    }

    [Test]
    [Description("BlankLinks and the target override open links in a new window")]
    public void Url_Target_Blank_Test()
    {
        (this.Format("[url]https://example.com[/url]", targetBlank: true)).Should().Contain("target=\"_blank\"");

        this.board.BoardSettings.BlankLinks = true;

        (this.Format("[url]https://example.com[/url]")).Should().Contain("target=\"_blank\"");
    }

    [Test]
    [Description("Links get rel=nofollow, unless the post is older than DisableNoFollowLinksAfterDay")]
    public void Url_NoFollow_Test()
    {
        this.board.BoardSettings.UseNoFollowLinks = true;
        this.board.BoardSettings.DisableNoFollowLinksAfterDay = 7;

        (this.Format("[url]https://example.com[/url]", lastEdited: DateTime.UtcNow.AddDays(-1)))
            .Should().Contain("rel=\"nofollow\"");

        (this.Format("[url]https://example.com[/url]", lastEdited: DateTime.UtcNow.AddDays(-30)))
            .Should().NotContain("nofollow", "the post is older than 7 days");
    }

    [Test]
    [Description("Images and quotes are rendered")]
    public void Image_And_Quote_Test()
    {
        (this.Format("[img]https://example.com/picture.png[/img]")).Should()
            .Contain("<img src=\"https://example.com/picture.png\"");

        var quote = this.Format("[quote]quoted text[/quote]");
        quote.Should().Contain("<div class=\"card").And.Contain("quoted text");
        System.Text.RegularExpressions.Regex.Matches(quote, "<div[\\s>]").Count
            .Should().Be(System.Text.RegularExpressions.Regex.Matches(quote, "</div>").Count);
    }

    [Test]
    [Description("Code blocks keep their content encoded (posts are saved with BBCodeHelper.EncodeCodeBlocks)")]
    public void Code_Test()
    {
        var html = this.Format(BBCodeHelper.EncodeCodeBlocks("[code]<b>not bold</b> row[td] [b][/code]"));

        html.Should().Contain("row[td] [b]", "emoji short codes are not applied inside code blocks");

        html.Should().Contain("&lt;b&gt;not bold&lt;/b&gt;").And.NotContain("<b>");
    }

    [Test]
    [Description("BBCodes inside [noparse] are shown as text, tabs and line breaks are kept")]
    public void NoParse_Test()
    {
        var html = this.Format("before [noparse][b]not bold[/b]\tx\r\n[url]y[/url][/noparse] [b]bold[/b]");

        html.Should().Contain("&#91;b&#93;not bold&#91;/b&#93;&nbsp; &nbsp;&nbsp;x<br />&#91;url&#93;y&#91;/url&#93;");
        html.Should().NotContain("<strong>not bold</strong>").And.Contain("<strong>bold</strong>");
        html.Should().NotContain("[noparse]");
    }

    [Test]
    [Description("Html inside [noparse] is shown as text")]
    public void NoParse_Html_Test()
    {
        var html = this.Format("[noparse]<script>alert(1)</script>[/noparse] [noparse][i]two[/i][/noparse]");

        html.Should().NotContain("<script>").And.Contain("&#91;i&#93;two&#91;/i&#93;");
    }

    [Test]
    [Description("Code blocks with a language get the highlighter markup, the content stays encoded")]
    [TestCase("cs", "language-csharp")]
    [TestCase("xml", "language-markup")]
    [TestCase("plain", "language-markup")]
    [TestCase("javascript", "language-javascript")]
    public void Code_Language_Test(string language, string cssClass)
    {
        var html = this.Format(BBCodeHelper.EncodeCodeBlocks($"[code={language}]<script>alert(1)</script>[/code]"));

        html.Should().Contain($"<pre class=\"line-numbers {cssClass}\">").And.Contain($"<code class=\"{cssClass}\">");
        html.Should().Contain("&lt;script&gt;").And.NotContain("<script>");
    }

    [Test]
    [Description("Lines of a code block can be highlighted")]
    public void Code_Highlight_Lines_Test()
    {
        var html = this.Format(BBCodeHelper.EncodeCodeBlocks("[code=cs;2-3]var a = 1;[/code]"));

        html.Should().Contain("<pre class=\"line-numbers language-csharp\" data-line=\"2-3\">");
    }

    [Test]
    [Description("The language of a code block can not break out of the html attribute")]
    [TestCase("x\" onmouseover=\"alert(1)")]
    [TestCase("x;1\" onmouseover=\"alert(1)")]
    [TestCase("x><script>alert(1)</script>")]
    public void Code_Language_Injection_Test(string language)
    {
        var html = this.Format(BBCodeHelper.EncodeCodeBlocks($"[code={language}]text[/code]"));

        html.Should().NotContain("<script>").And.NotContain("onmouseover", "only language and line characters are kept");
        html.Should().MatchRegex("<pre class=\"line-numbers language-[a-z0-9#+-]*\"( data-line=\"[0-9,-]*\")?>");
    }

    [Test]
    [Description("Bad words from the database are replaced")]
    public void Bad_Words_Test()
    {
        this.board.Current.GetRepository<Replace_Words>().Save(null, "darn", "****");

        (this.Format("Oh darn it")).Should().Contain("Oh **** it");
    }

    [Test]
    [Description("The good words are text: html in a good word is encoded in the message (it was added as html)")]
    public void Bad_Words_Html_Good_Word_Test()
    {
        this.board.Current.GetRepository<Replace_Words>().Save(null, "darn", "<b onmouseover=alert(1)>nice</b>");

        var html = this.Format("Oh darn it");

        html.Should().NotContain("<b onmouseover").And.Contain("&lt;b onmouseover=alert(1)&gt;nice&lt;/b&gt;");
    }

    [Test]
    [Description("Ascii smileys become emoji")]
    public void Emoji_Test()
    {
        (this.Format("Hello :D")).Should().Contain("😃");
    }

    [TestCase("<script>alert(1)</script>")]
    [TestCase("<img src=x onerror=alert(1)>")]
    [TestCase("[url=javascript:alert(1)]click[/url]")]
    [TestCase("[url]javascript:alert(1)[/url]")]
    [TestCase("[url=JaVaScRiPt:alert(1)]click[/url]")]
    [TestCase("[url= javascript:alert(1)]click[/url]")]
    [TestCase("[url=data:text/html;base64,PHNjcmlwdD5hbGVydCgxKTwvc2NyaXB0Pg==]click[/url]")]
    [TestCase("[url=java\tscript:alert(1)]click[/url]")]
    [TestCase("[url]java\tscript:alert(1)[/url]")]
    [TestCase("[url=\u0001javascript:alert(1)]click[/url]")]
    [TestCase("[url=https://example.com\" onmouseover=\"alert(1)]click[/url]")]
    [TestCase("[email=x\" onmouseover=\"alert(1)]mail[/email]")]
    [TestCase("[img]https://example.com/a.png\" onload=\"alert(1)[/img]")]
    [TestCase("[color=red;background:url(javascript:alert(1))]x[/color]")]
    [TestCase("[img]javascript:alert(1)//x.png[/img]")]
    [TestCase("[img=javascript:alert(1)//x.png]description[/img]")]
    [TestCase("[img]java\tscript:alert(1)//x.png[/img]")]
    [TestCase("[url]\u0001javascript:alert(1)[/url]")]
    [TestCase("[url=\u000Bjavascript:alert(1)]click[/url]")]
    [Description("Script injection attempts never produce executable HTML")]
    public void Xss_Test(string input)
    {
        var html = this.Format(input);

        html.Should().NotContainEquivalentOf("<script");
        html.Should().NotMatchRegex("(?i)<[^<>]*\\son[a-z]+\\s*=\\s*[\"']?alert", "no injected event handler attributes");
        html.Should().NotMatchRegex(
            "(?i)(href|src)\\s*=\\s*\"[\\x00-\\x20]*(j[\\x00-\\x20]*a[\\x00-\\x20]*v[\\x00-\\x20]*a[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t|data|vbscript)[\\x00-\\x20]*:",
            "no script urls, also when browsers would strip whitespace/control characters");
    }

    [Test]
    [Description("RemoveNestedQuotes strips quotes from a reply")]
    public void RemoveNestedQuotes_Test()
    {
        this.Formatter.RemoveNestedQuotes("[quote=Bob;1]old[/quote] new reply").Should().Be("new reply");
    }

    [Test]
    [Description("RemoveHiddenBBCodeContent strips every kind of hidden content")]
    public void RemoveHiddenBBCodeContent_Test()
    {
        const string body = "visible [hide]secret[/hide] [hide-reply]r[/hide-reply] [hide-thanks]t[/hide-thanks] "
                            + "[hide-posts=10]p[/hide-posts] [group-hide=Admins]g[/group-hide] end";

        var result = this.Formatter.RemoveHiddenBBCodeContent(body);

        result.Should().StartWith("visible").And.EndWith("end");
        result.Should().NotContain("secret").And.NotContain("hide");
    }

    [Test]
    [Description("RemoveCustomBBCodes replaces spoilers with their content and keeps the rest of the post")]
    public void RemoveCustomBBCodes_Test()
    {
        this.Formatter.RemoveCustomBBCodes("Before [spoiler]first[/spoiler] middle [SPOILER]second[/SPOILER] after")
            .Should().Be("Before first middle second after");

        this.Formatter.RemoveCustomBBCodes("no spoiler").Should().Be("no spoiler");
    }

    [Test]
    [Description("RepairHtml encodes HTML")]
    public void RepairHtml_Test()
    {
        this.Formatter.RepairHtml("<b>x</b> & \"y\"", false).Should().Be("&lt;b&gt;x&lt;/b&gt; &amp; &quot;y&quot;");
    }

    [Test]
    [Description("SurroundWordList highlights search words longer than 3 characters, case-insensitively")]
    public void SurroundWordList_Test()
    {
        this.Formatter.SurroundWordList("Install the Forum and enjoy the forum", ["forum", "the"], "<mark>", "</mark>")
            .Should().Be("Install the <mark>Forum</mark> and enjoy the <mark>forum</mark>");
    }

    [Test]
    [Description("BBCodeForbiddenDetector lists the tags that are not allowed")]
    public void BBCodeForbiddenDetector_Test()
    {
        this.Formatter.BBCodeForbiddenDetector("[b]x[/b] [url=https://a]y[/url] [img]z[/img] [IMG]w[/IMG]", "b,i", ',')
            .Should().Be("img,url");

        this.Formatter.BBCodeForbiddenDetector("[b]ok[/b]", "b", ',').Should().BeEmpty();
    }

    [Test]
    [Description("Syndication messages drop hidden content and spoiler tags")]
    public void FormatSyndicationMessageAsync_Test()
    {
        var html = this.Formatter.FormatSyndicationMessage(
            "[b]Hello[/b] [hide]secret[/hide] [spoiler]revealed[/spoiler] bye",
            1,
            2,
            new MessageFlags { IsBBCode = true, IsHtml = false });

        html.Should().Contain("<strong>Hello</strong>").And.Contain("revealed").And.Contain("bye");
        html.Should().NotContain("secret");
    }

    [Test]
    [Description("A plain url becomes a link, the text before it on the line is kept (it was removed)")]
    public void MakeHtml_Plain_Url_Keeps_Text_Test()
    {
        var service = this.board.Resolve<IBBCode>();

        var html = service.MakeHtml("Please see https://forum.example/moderate for details", true, false);

        html.Should().StartWith("Please see <a ").And.Contain("href=\"https://forum.example/moderate\"").And.EndWith("</a> for details");

        html = service.MakeHtml("first https://a.example/x and second https://b.example/y end", true, false);

        html.Should().StartWith("first <a ").And.Contain("href=\"https://a.example/x\"").And.Contain(" and second <a ")
            .And.Contain("href=\"https://b.example/y\"").And.EndWith("</a> end");

        html = service.MakeHtml("line one\nline two https://a.example/x", true, false);

        html.Should().Contain("line one").And.Contain("line two <a ");
    }

    [Test]
    [Description("A youtube url is embedded, not linked (also with text before it)")]
    public void MakeHtml_Youtube_Url_Test()
    {
        var html = this.board.Resolve<IBBCode>().MakeHtml(
            "Watch https://www.youtube.com/watch?v=abc123 now",
            true,
            false);

        html.Should().StartWith("Watch ").And.Contain("youtube.com/embed/abc123").And.NotContain("href=\"https://www.youtube.com");
    }

    [Test]
    [Description("MakeHtml without formatting only applies the structural rules")]
    public void MakeHtml_Test()
    {
        var service = this.board.Resolve<IBBCode>();

        (service.MakeHtml("[b]x[/b]", true, false)).Should().Contain("<strong>x</strong>");
        (service.MakeHtml("[b]x[/b]", false, false)).Should().Contain("[b]x[/b]");
    }

    [Test]
    [Description("Localization elements in custom BBCodes use the BBCODEMODULE texts")]
    public void LocalizeCustomBBCodeElement_Test()
    {
        this.board.Localization.Add("BBCODEMODULE", "SPOILER_TITLE", "Show spoiler");

        var service = this.board.Resolve<IBBCode>();

        service.LocalizeCustomBBCodeElement("[localization=SPOILER_TITLE]Spoiler[/localization]: [localization=UNKNOWN]Fallback[/localization]")
            .Should().Be("Show spoiler: Fallback");
    }

    private IFormatMessage Formatter => this.board.Resolve<IFormatMessage>();

    private string Format(string message, bool targetBlank = false, DateTime? lastEdited = null)
    {
        // like a post: the message is formatted, then the custom bb codes are applied
        // a post of the bb code editor (no html)
        var flags = new MessageFlags { IsBBCode = true, IsHtml = false };

        var html = this.Formatter.Format(1, message, flags, targetBlank, lastEdited ?? DateTime.UtcNow);

        return this.board.Resolve<IBBCode>().FormatMessageWithCustomBBCode(html, flags, 2, 1);
    }
}
