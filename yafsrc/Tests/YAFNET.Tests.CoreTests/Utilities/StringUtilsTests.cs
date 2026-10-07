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

namespace YAF.Tests.CoreTests.Utilities;

using Microsoft.AspNetCore.Http;

using YAF.Core.Utilities;
using YAF.Core.Utilities.MinifyUtils;
using YAF.Core.Utilities.StringUtils;

/// <summary>
/// Tests for CrossSiteScriptingValidation, UnicodeEncoder, Unidecoder and JsMinify
/// </summary>
[TestFixture]
public class StringUtilsTests
{
    [TestCase("<script>alert(1)</script>", true, 0)]
    [TestCase("hello <b>", true, 6)]
    [TestCase("</div>", true, 0)]
    [TestCase("<!-- comment -->", true, 0)]
    [TestCase("<?xml", true, 0)]
    [TestCase("x &#60;", true, 2)]
    [TestCase("a < b && c > d", false, 0)]
    [TestCase("1<2", false, 0)]
    [TestCase("ends with <", false, 0)]
    [TestCase("ends with &", false, 0)]
    [TestCase("Tom & Jerry", false, 0)]
    [TestCase("plain text", false, 0)]
    [TestCase("", false, 0)]
    [Description("IsDangerousString flags tag openers and numeric entities")]
    public void IsDangerousString_Test(string input, bool expected, int expectedIndex)
    {
        var result = CrossSiteScriptingValidation.IsDangerousString(input, out var matchIndex);

        result.Should().Be(expected);

        if (expected)
        {
            matchIndex.Should().Be(expectedIndex);
        }
    }

    [Test]
    [Description("AddHeaders adds the P3P header only once")]
    public void AddHeaders_Test()
    {
        var headers = new HeaderDictionary();

        headers.AddHeaders();
        headers.AddHeaders();

        headers["P3P"].Should().ContainSingle().Which.Should().StartWith("CP=");
    }

    [Test]
    [Description("ToJson serializes objects with Newtonsoft")]
    public void ToJson_Test()
    {
        new { Id = 1, Name = "yaf" }.ToJson().Should().Be("{\"Id\":1,\"Name\":\"yaf\"}");
    }

    [Test]
    [Description("XSSEncode encodes quotes and angle brackets")]
    public void XSSEncode_Test()
    {
        new UnicodeEncoder().XSSEncode("<a href=\"x\" title='y'>")
            .Should().Be("&lt;a href=&quot;x&quot; title=&#39;y&#39;&gt;");
    }

    [Test]
    [Description("EncodeWithoutUnicode html-encodes but keeps latin-1 letters readable")]
    public void EncodeWithoutUnicode_Test()
    {
        new UnicodeEncoder().EncodeWithoutUnicode("Ärger & <Grüße>")
            .Should().Be("Ärger &amp; &lt;Grüße&gt;");
    }

    [TestCase("Äpfel und Öl", "Apfel und Ol")]
    [TestCase("Привет", "Privet")]
    [TestCase("crème brûlée", "creme brulee")]
    [TestCase("plain ascii", "plain ascii")]
    [TestCase("", "")]
    [TestCase(null, null)]
    [Description("Unidecode transliterates to ASCII")]
    public void Unidecode_Test(string input, string expected)
    {
        input.Unidecode().Should().Be(expected);
    }

    [Test]
    [Description("JsMinify removes comments and needless whitespace")]
    public void JsMinify_Test()
    {
        const string script = """
                              // a comment
                              function add(a, b) {
                                  /* block comment */
                                  return a + b;
                              }

                              var text = "keep  // this";
                              """;

        var minified = JsMinify.Minify(script);

        minified.Should().NotContain("comment");
        minified.Should().Contain("function add(a,b){return a+b;}");
        minified.Should().Contain("var text=\"keep  // this\";");
        minified.Length.Should().BeLessThan(script.Length);
    }

    [Test]
    [Description("JsMinify keeps regex literals intact")]
    public void JsMinify_Regex_Literal_Test()
    {
        JsMinify.Minify("var r = /a b\\/c/g;").Should().Contain("var r=/a b\\/c/g;");
    }

    [Test]
    [Description("JsMinify rejects unterminated comments")]
    public void JsMinify_Unterminated_Comment_Throws_Test()
    {
        FluentActions.Invoking(() => JsMinify.Minify("var a = 1; /* never closed"))
            .Should().Throw<FormatException>();
    }
}
