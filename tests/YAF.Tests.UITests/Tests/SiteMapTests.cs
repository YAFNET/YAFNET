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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.UITests.Tests;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The sitemap of the forum (Sitemap.xml) for the search engines.
/// </summary>
[TestFixture]
public class SiteMapTests : ForumTestBase
{
    [Test]
    [Description("The sitemap lists the forums the guests can read with the address of the forum and the elements of the sitemap schema")]
    public async Task SiteMap_Lists_Forums_Test()
    {
        var response = await this.Page.APIRequest.GetAsync($"{TestConfig.BaseUrl}Sitemap.xml");

        response.Status.Should().Be(200);

        var body = await response.BodyAsync();
        var xml = Encoding.UTF8.GetString(body);

        // utf-8 (google refuses utf-16)
        xml.Should().Contain("encoding=\"utf-8\"");

        var document = XDocument.Parse(xml.TrimStart('﻿'));
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        document.Root.Name.Should().Be(ns + "urlset");

        var urls = document.Root.Elements(ns + "url").ToList();
        urls.Should().NotBeEmpty();

        var testForum = urls.Select(u => u.Element(ns + "loc").Value).FirstOrDefault(l => l.Contains("/Topics/1-"));
        testForum.Should().NotBeNull(
            $"the test forum is listed with the address of the forum, but the addresses were {string.Join(" ", urls.Select(u => u.Element(ns + "loc").Value))}");
        testForum.Should().StartWith(TestConfig.BaseUrl);
        testForum.Substring(TestConfig.BaseUrl.Length).Should().NotContain("//");

        foreach (var url in urls)
        {
            // the order of the schema: loc, lastmod, changefreq, priority
            url.Elements().Select(e => e.Name.LocalName).Should().Equal("loc", "lastmod", "changefreq", "priority");

            // a w3c date with the time zone
            url.Element(ns + "lastmod").Value.Should().MatchRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(Z|[+-]\d{2}:\d{2})$");
        }
    }
}
