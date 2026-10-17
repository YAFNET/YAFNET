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

namespace YAF.Tests.Utilities;

using YAF.Core.Utilities.StringUtils;

/// <summary>
/// The string utilities: html encoding and transliteration.
/// </summary>
[TestFixture]
public class StringUtilsTests
{
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
}
