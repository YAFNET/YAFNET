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

namespace YAF.Tests.Helpers;

using YAF.Configuration;

/// <summary>
/// Tests for the text related helpers: StringHelper, HtmlTagHelper, UrlRewriteHelper and BBCodeHelper
/// </summary>
[TestFixture]
[NonParallelizable]
public class TextHelperTests
{
    private string originalUrlRewritingMode;

    [SetUp]
    public void SetUp()
    {
        this.originalUrlRewritingMode = Config.UrlRewritingMode;
    }

    [TearDown]
    public void TearDown()
    {
        Config.UrlRewritingMode = this.originalUrlRewritingMode;
    }

    [TestCase("Yet Another", "YA")]
    [TestCase("Yet Another Forum", "YA")]
    [TestCase("YetAnotherForum", "YAF")]
    [TestCase("forum", "FOR")]
    [TestCase("ab", "AB")]
    [TestCase("  admin  ", "ADM")]
    [TestCase("Ärger Bob", "AB")]
    [TestCase("Alice  Smith", "AS")]
    [TestCase("Иван Петров", "IP")]
    [TestCase("Tom & Jerry", "T&")]
    [TestCase("😀😀", "?")]
    [TestCase("", "?")]
    [TestCase(null, "?")]
    [Description("GetAbbreviation builds a two or three letter abbreviation")]
    public void GetAbbreviation_Test(string input, string expected)
    {
        input.GetAbbreviation().Should().Be(expected);
    }

    [TestCase("<b>bold</b> text", "bold text")]
    [TestCase("<a href=\"x\">link</a><br />", "link")]
    [TestCase("<p\nclass=\"a\">multi</p>", "multi")]
    [TestCase("no html", "no html")]
    [TestCase("", "")]
    [TestCase(null, null)]
    [Description("StripHtml removes all tags")]
    public void StripHtml_Test(string input, string expected)
    {
        HtmlTagHelper.StripHtml(input).Should().Be(expected);
    }

    [Test]
    [Description("CleanHtmlString replaces line breaks and common entities")]
    public void CleanHtmlString_Test()
    {
        HtmlTagHelper.CleanHtmlString("a<br />&quot;b&quot;&nbsp;c").Should().Be("a \"b\" c");
    }

    [TestCase("b", true)]
    [TestCase("/b", true)]
    [TestCase("b class=\"x\"", true)]
    [TestCase("br/", true)]
    [TestCase("i>", true)]
    [TestCase("script", false)]
    [TestCase("bold", false)]
    [TestCase("a href=\"javascript:alert(1)\"", false)]
    [TestCase("a href=\"vbscript:x\"", false)]
    [TestCase("b onclick=\"x\"", false)]
    [Description("IsValidTag only allows whitelisted tags without script handlers")]
    public void IsValidTag_Test(string tag, bool expected)
    {
        HtmlTagHelper.IsValidTag(tag, ["b", "i", "a", "br"]).Should().Be(expected);
    }

    [TestCase("Hello World & Friends!", "Hello-World-and-Friends")]
    [TestCase("Über Café", "Uber-Cafe")]
    [TestCase("  trimmed  ", "trimmed")]
    [TestCase("a&amp;b", "a-b")]
    [TestCase("Привет мир", "Default")]
    [TestCase("!!!", "Default")]
    [Description("CleanStringForUrl (default mode) produces ASCII slugs")]
    public void CleanStringForUrl_Default_Test(string input, string expected)
    {
        Config.UrlRewritingMode = null;

        UrlRewriteHelper.CleanStringForUrl(input).Should().Be(expected);
    }

    [TestCase("Привет мир", "Privet-mir")]
    [TestCase("Hello World", "Hello-World")]
    [TestCase("???", "Default")]
    [Description("CleanStringForUrl (Translit mode) transliterates non latin text")]
    public void CleanStringForUrl_Translit_Test(string input, string expected)
    {
        Config.UrlRewritingMode = "Translit";

        UrlRewriteHelper.CleanStringForUrl(input).Should().Be(expected);
    }

    [TestCase("Привет мир", "Привет-мир")]
    [TestCase("Hello%20World", "Hello-World")]
    [TestCase("...", "Default")]
    [Description("CleanStringForUrl (Unicode mode) keeps non latin letters")]
    public void CleanStringForUrl_Unicode_Test(string input, string expected)
    {
        Config.UrlRewritingMode = "Unicode";

        UrlRewriteHelper.CleanStringForUrl(input).Should().Be(expected);
    }

    [Test]
    [Description("CleanStringForUrl throws on null")]
    public void CleanStringForUrl_Null_Throws_Test()
    {
        FluentActions.Invoking(() => UrlRewriteHelper.CleanStringForUrl(null)).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("FindUserQuoting returns the quoted user names")]
    public void FindUserQuoting_Test()
    {
        const string message = "[quote=Admin;12]hello[/quote] reply [quote=Bob;3]\nworld[/quote] [quote]anonymous[/quote]";

        BBCodeHelper.FindUserQuoting(message).Should().Equal("Admin", "Bob");
    }

    [Test]
    [Description("FindMentions returns the mentioned user names")]
    public void FindMentions_Test()
    {
        const string message = "Hi @[userlink]Admin[/userlink] and @[USERLINK]Bob[/USERLINK], not @Carl";

        BBCodeHelper.FindMentions(message).Should().Equal("Admin", "Bob");
        BBCodeHelper.FindMentions("nothing here").Should().BeEmpty();
    }

    [Test]
    [Description("EncodeCodeBlocks html-encodes code blocks and DecodeCodeBlocks reverses it")]
    public void Encode_Decode_CodeBlocks_Test()
    {
        const string message = "<b>outside</b> [code]<script>a && b</script>[/code]";

        var encoded = BBCodeHelper.EncodeCodeBlocks(message);

        encoded.Should().Be("<b>outside</b> [code]&lt;script&gt;a &amp;&amp; b&lt;/script&gt;[/code]");
        BBCodeHelper.DecodeCodeBlocks(encoded).Should().Be(message);
    }
}
