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

using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

using YAF.Types.Extensions;

namespace YAF.Tests.MiddlewareTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SiteMapTests : Setup
{
    [OneTimeSetUp]
    public async Task LoadXml()
    {
        this.client = WebTestingHostFactoryUtils.CreateClient(this.TestSettings, "127.0.0.1");

        this.client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

        this.siteMapXmlFile = await this.client.GetStringAsync($"{this.TestSettings.TestForumUrl}Sitemap.xml");
    }

    [OneTimeTearDown]
    public void BaseTearDown()
    {
        this.client.Dispose();
    }

    private HttpClient client;

    private string siteMapXmlFile;

    private const string SitemapNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
    private const int MaxUrlCount = 50_000;
    private const int MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB
    private const int MaxUrlLength = 2048;

    private readonly static string[] ValidChangeFrequencies =
        ["always", "hourly", "daily", "weekly", "monthly", "yearly", "never"];

    //// <summary>Load sitemap from an XML string (handy for inline test data).</summary>
    private static XDocument LoadSitemapFromString(string xml)
    {
        return XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
    }

    [Test]
    public void SiteMap_ShouldBeValidXml()
    {
        var xml = this.siteMapXmlFile;
        var act = () => LoadSitemapFromString(xml);
        act.Should().NotThrow();
    }

    [Test]
    public void SiteMap_ShouldHaveCorrectNamespace()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var root = doc.Root;

        root.Should().NotBeNull();
        root!.Name.NamespaceName
            .Should().Be(SitemapNamespace,
                "the root element must declare the sitemaps.org namespace");
    }

    [Test]
    public void SiteMap_ShouldBeValidUtf8()
    {
        var declaration = XDocument.Parse(this.siteMapXmlFile).Declaration;

        declaration?.Encoding?.Should().BeEquivalentTo("UTF-8",
            "sitemap content must be valid UTF-8");
    }

    [Test]
    public void SiteMap_RootElement_ShouldBeUrlset()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        doc.Root!.Name.Should().Be(ns + "urlset",
            "root element must be <urlset>");
    }

    [Test]
    public void SiteMap_ShouldNotExceedMaxUrlCount()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        var urlCount = doc.Root!.Elements(ns + "url").Count();

        urlCount.Should().BeLessThanOrEqualTo(MaxUrlCount,
            $"Google allows a maximum of {MaxUrlCount} URLs per sitemap file");
    }

    [Test]
    public void SiteMap_FileSizeInBytes_ShouldNotExceed50MB()
    {
        var xml = this.siteMapXmlFile;
        var bytes = Encoding.UTF8.GetByteCount(xml);

        bytes.Should().BeLessThanOrEqualTo(MaxFileSizeBytes,
            "uncompressed sitemap must not exceed 50 MB");
    }

    [Test]
    public void SiteMap_EachUrlEntry_ShouldContainLocElement()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        var urlsWithoutLoc = doc.Root!
            .Elements(ns + "url")
            .Where(u => u.Element(ns + "loc") is null)
            .ToList();

        urlsWithoutLoc.Should().BeEmpty(
            "every <url> entry must contain a <loc> child element");
    }

    [Test]
    public void SiteMap_LocValues_ShouldBeAbsoluteUrls()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        var invalidLocs = doc.Root!
            .Elements(ns + "url")
            .Select(u => u.Element(ns + "loc")?.Value.Trim())
            .Where(loc => !IsAbsoluteUrl(loc))
            .ToList();

        invalidLocs.Should().BeEmpty(
            "all <loc> values must be absolute URLs starting with http:// or https://");
    }

    [Test]
    public void SiteMap_LocValues_ShouldNotExceedMaxLength()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        var tooLong = doc.Root!
            .Elements(ns + "url")
            .Select(u => u.Element(ns + "loc")?.Value.Trim() ?? "")
            .Where(loc => loc.Length > MaxUrlLength)
            .ToList();

        tooLong.Should().BeEmpty(
            $"URLs must not exceed {MaxUrlLength} characters");
    }

    [Test]
    public void SiteMap_LocValues_ShouldBeUnique()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        var locs = doc.Root!
            .Elements(ns + "url")
            .Select(u => u.Element(ns + "loc")?.Value.Trim() ?? "")
            .ToList();

        var duplicates = locs
            .GroupBy(l => l, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        duplicates.Should().BeEmpty(
            "each URL in <loc> must appear only once");
    }

    [Test]
    public void SiteMap_LastmodValues_WhenPresent_ShouldBeValidW3CDatetime()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        var invalidDates = doc.Root!
            .Elements(ns + "url")
            .Select(u => u.Element(ns + "lastmod")?.Value.Trim())
            .Where(v => v is not null)
            .Where(v => !IsValidW3CDatetime(v!))
            .ToList();

        invalidDates.Should().BeEmpty(
            "<lastmod> must be a valid W3C/ISO 8601 date (e.g. 2024-06-01 or 2024-06-01T12:00:00+00:00)");
    }

    [Test]
    public void SiteMap_ChangefreqValues_WhenPresent_ShouldBeValidKeywords()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        var invalidFreqs = doc.Root!
            .Elements(ns + "url")
            .Select(u => u.Element(ns + "changefreq")?.Value.Trim().ToLowerInvariant())
            .Where(v => v is not null)
            .Where(v => !ValidChangeFrequencies.Contains(v))
            .ToList();

        invalidFreqs.Should().BeEmpty(
            $"<changefreq> must be one of: {string.Join(", ", ValidChangeFrequencies)}");
    }

    [Test]
    public void SiteMap_PriorityValues_WhenPresent_ShouldBeBetween0And1()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        var invalidPriorities = doc.Root!
            .Elements(ns + "url")
            .Select(u => u.Element(ns + "priority")?.Value.Trim())
            .Where(v => v is not null)
            .Where(v => !IsValidPriority(v!))
            .ToList();

        invalidPriorities.Should().BeEmpty(
            "<priority> must be a decimal value between 0.0 and 1.0 inclusive");
    }

    [Test]
    public void SiteMap_LocValues_ShouldBeProperlyEncoded()
    {
        var doc = LoadSitemapFromString(this.siteMapXmlFile);
        var ns = XNamespace.Get(SitemapNamespace);

        // Spaces in URLs are a common mistake — they must be %20-encoded
        var unencoded = doc.Root!
            .Elements(ns + "url")
            .Select(u => u.Element(ns + "loc")?.Value.Trim() ?? "")
            .Where(loc => loc.Contains(' '))
            .ToList();

        unencoded.Should().BeEmpty(
            "URLs must be properly percent-encoded (e.g. spaces must be %20)");
    }

    private static bool IsAbsoluteUrl(string url)
    {
        return url.IsSet() &&
               (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) &&
               Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    private static bool IsValidW3CDatetime(string value)
    {
        // W3C datetime profile used by sitemaps: date-only or full datetime with offset
        string[] formats =
        [
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:ss.fffzzz",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss.fffZ"
        ];

        return DateTime.TryParseExact(
            value,
            formats,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out _);
    }

    private static bool IsValidPriority(string value)
    {
        return double.TryParse(value,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var d) && d is >= 0.0 and <= 1.0;
    }
}