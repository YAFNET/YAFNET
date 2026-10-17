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
/// The platform and the search engines of the user agents (active users list).
/// </summary>
[TestFixture]
public class UserAgentHelperTests
{
    [TestCase("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36", "Windows 10")]
    [TestCase("Mozilla/5.0 (Windows NT 6.3; Win64; x64)", "Windows 8.1")]
    [TestCase("Mozilla/5.0 (Windows NT 6.2; Win64; x64)", "Windows 8")]
    [TestCase("Mozilla/5.0 (Windows NT 6.1; Win64; x64)", "Windows 7")]
    [TestCase("Mozilla/5.0 (Windows NT 6.0)", "Windows Vista")]
    [TestCase("Mozilla/5.0 (Windows NT 5.1)", "Windows XP")]
    [TestCase("Mozilla/5.0 (Windows NT 5.2)", "Windows 2003")]
    [TestCase("Mozilla/5.0 (X11; Linux x86_64; rv:120.0) Gecko/20100101 Firefox/120.0", "Linux")]
    [TestCase("Mozilla/5.0 (X11; FreeBSD amd64)", "FreeBSD")]
    [TestCase("Mozilla/5.0 (iPad; CPU OS 17_0 like Mac OS X)", "iPad(iOS)")]
    [TestCase("Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X)", "iPhone(iOS)")]
    [TestCase("Mozilla/5.0 (iPod touch; CPU iPhone OS 12_0 like Mac OS X)", "iPod(iOS)")]
    [TestCase("Mozilla/5.0 (Linux; Android 14; Pixel 8) AppleWebKit/537.36 Chrome/120.0 Mobile Safari/537.36", "Android")]
    [TestCase("Mozilla/5.0 (Macintosh; Intel Mac OS X 14_0) AppleWebKit/605.1.15 Safari/605.1.15", "Mac OS X")]
    [TestCase("Mozilla/5.0 (webOS/1.4.0; U; en-US)", "WebOS")]
    [TestCase("Mozilla/4.0 (compatible; MSIE 7.0; Windows Phone OS 7.0)", "Windows Phone")]
    [TestCase("SomeUnknownAgent/1.0", "unknown")]
    [Description("The platform of the user agent, Android phones are no Linux computers")]
    public void Platform_Test(string userAgent, string expected)
    {
        var platform = "unknown";

        UserAgentHelper.Platform(userAgent, false, ref platform, out var isSearchEngine);

        platform.Should().Be(expected);
        isSearchEngine.Should().BeFalse();
    }

    [Test]
    [Description("Without a user agent the platform is empty")]
    public void Platform_Empty_Test()
    {
        var platform = "unknown";

        UserAgentHelper.Platform(null, true, ref platform, out var isSearchEngine);

        platform.Should().Be("[Empty]");
        isSearchEngine.Should().BeFalse();
    }

    [TestCase("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)", true)]
    [TestCase("Mozilla/5.0 (compatible; YandexBot/3.0)", true)]
    [TestCase("Baiduspider+(+http://www.baidu.com/search/spider.htm)", true)]
    [TestCase("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0", false)]
    [TestCase("", false)]
    [TestCase(null, false)]
    [Description("Search engines are found by their user agent or by the crawler flag")]
    public void Search_Engine_Test(string userAgent, bool expected)
    {
        UserAgentHelper.IsSearchEngineSpider(userAgent).Should().Be(expected);

        var platform = string.Empty;

        UserAgentHelper.Platform("Mozilla/5.0 (Windows NT 10.0)", true, ref platform, out var isCrawler);
        isCrawler.Should().BeTrue("the crawler flag of the browser capabilities");
    }
}
