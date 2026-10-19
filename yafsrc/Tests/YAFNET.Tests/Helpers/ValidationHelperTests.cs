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

/// <summary>
/// YAF.Utils.Helpers ValidationHelper Tests
/// </summary>
[TestFixture]
public class ValidationHelperTests
{
    /// <summary>
    /// Determines whether [is valid email test].
    /// </summary>
    [Test]
    [Description("Determines whether [is valid email test]")]
    public void IsValidEmail_Test()
    {
        const string testEmail = "yaf@co.geauga.oh.us";

        ValidationHelper.IsValidEmail(testEmail).Should().BeTrue("Email address is not valid");
    }

    [TestCase("john.doe@example.com", true)]
    [TestCase("john+tag@example.co.uk", true)]
    [TestCase("JOHN@EXAMPLE.COM", true)]
    [TestCase("john@example", false)]
    [TestCase("john@@example.com", false)]
    [TestCase("@example.com", false)]
    [TestCase("john.@example.com", false)]
    [TestCase("john doe@example.com", false)]
    [TestCase("", false)]
    [Description("IsValidEmail accepts common addresses and rejects malformed ones")]
    public void IsValidEmail_Cases_Test(string email, bool expected)
    {
        ValidationHelper.IsValidEmail(email).Should().Be(expected);
    }

    [TestCase("https://www.yetanotherforum.net/", true)]
    [TestCase("ftp://files.example.com/a.zip", true)]
    [TestCase("/relative/path", false)]
    [TestCase("not a url", false)]
    [TestCase(null, false)]
    [Description("IsValidUrl only accepts absolute URIs")]
    public void IsValidUrl_Test(string url, bool expected)
    {
        ValidationHelper.IsValidUrl(url).Should().Be(expected);
    }

    [TestCase("https://www.example.com/member", true)]
    [TestCase("http://example.com", true)]
    [TestCase("ftp://example.com/file", true)]
    [TestCase("javascript:alert(1)", false)]
    [TestCase(" JavaScript:alert(1)", false)]
    [TestCase("data:text/html,<script>alert(1)</script>", false)]
    [TestCase("vbscript:msgbox(1)", false)]
    [TestCase("www.example.com", false)]
    [TestCase("", false)]
    [TestCase(null, false)]
    [Description("Only web addresses can be used as links of the profile")]
    public void IsLinkUrl_Test(string url, bool expected)
    {
        ValidationHelper.IsLinkUrl(url).Should().Be(expected);
    }

    [TestCase("42", true)]
    [TestCase("-7", true)]
    [TestCase("4.2", false)]
    [TestCase("99999999999", false)]
    [TestCase("abc", false)]
    [TestCase(null, false)]
    [Description("IsValidInt checks for 32 bit integers")]
    public void IsValidInt_Test(string input, bool expected)
    {
        ValidationHelper.IsValidInt(input).Should().Be(expected);
    }

    [TestCase("42", true)]
    [TestCase("99999999999", true)]
    [TestCase("1e5", true)]
    [TestCase("abc", false)]
    [TestCase("", false)]
    [Description("IsNumeric accepts any number format")]
    public void IsNumeric_Test(string input, bool expected)
    {
        ValidationHelper.IsNumeric(input).Should().Be(expected);
    }

    [TestCase(@"^\d+$", true)]
    [TestCase("[a-z]+", true)]
    [TestCase("[a-z", false)]
    [TestCase("(abc", false)]
    [TestCase("", false)]
    [TestCase(null, false)]
    [Description("IsValidRegex detects invalid patterns")]
    public void IsValidRegex_Test(string pattern, bool expected)
    {
        ValidationHelper.IsValidRegex(pattern).Should().Be(expected);
    }

    /// <summary>
    /// Test the IPHelper if the ip address matches the pattern.
    /// </summary>
    [Test]
    [Description("Test the IPHelper if the ip address matches the pattern.")]
    public void IpHelperTest()
    {
        IPHelper.IsBanned( "43.*.*.*", "43.134.127.238").Should().BeTrue("true");
    }
}
