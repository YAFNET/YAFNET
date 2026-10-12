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

namespace YAF.Tests.CoreTests.Services;

using System.IO;
using System.Xml.Serialization;

using Google.Authenticator;

using YAF.Core.BBCode;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Two factor authentication codes, the sitemap xml and the cached BBCode rule engines.
/// </summary>
[TestFixture]
[NonParallelizable]
public class TwoFactorSitemapRulesProviderTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();
        this.board.BoardSettings.Name = "Test Forum";
        this.board.PageUser.Email = "alice@example.com";
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The setup code is for the board and the user, the current pin of the key is valid, others are not")]
    public void Two_Factor_Test()
    {
        var service = new TwoFactorAuthService(this.board.Current.ServiceLocator);

        var key = service.CreateAccountSecretKey(new User { Email = "alice@example.com" });
        key.Should().EndWith("+alice@example.com");
        service.CreateAccountSecretKey(new User { Email = "alice@example.com" }).Should().NotBe(key, "every key is random");

        var setup = service.GenerateSetupCode(key);
        setup.ManualEntryKey.Should().NotBeNullOrEmpty();
        setup.QrCodeSetupImageUrl.Should().StartWith("data:image/png;base64,");
        Uri.UnescapeDataString(setup.Account).Should().Be("alice@example.com");

        var pin = new TwoFactorAuthenticator().GetCurrentPIN(key);

        service.ValidatePin(key, pin).Should().BeTrue();
        service.ValidatePin(key, "abc").Should().BeFalse();
        service.ValidatePin($"{key}-other", pin).Should().BeFalse();
    }

    [Test]
    [Description("The sitemap is written in the sitemaps.org format, empty optional values are left out")]
    public void Sitemap_Xml_Test()
    {
        var map = new SiteMap();
        map.Add(new UrlLocation
        {
            Url = "https://forum.example/Posts/t1/hello",
            LastModified = "2026-10-09",
            ChangeFrequency = UrlLocation.ChangeFrequencies.daily,
            Priority = 0.8
        });
        map.Add(new UrlLocation { Url = "https://forum.example/Index" });

        using var writer = new StringWriter();
        new XmlSerializer(typeof(SiteMap)).Serialize(writer, map);
        var xml = writer.ToString();

        xml.Should().Contain("<urlset").And.Contain("http://www.sitemaps.org/schemas/sitemap/0.9");
        xml.Should().Contain("<loc>https://forum.example/Posts/t1/hello</loc>");
        xml.Should().Contain("<changefreq>daily</changefreq>").And.Contain("<priority>0.8</priority>");
        xml.Split("<priority>").Length.Should().Be(2, "the url without priority has none");

        var copy = (SiteMap)new XmlSerializer(typeof(SiteMap)).Deserialize(new StringReader(xml));
        copy.Locations.Select(l => l.Url).Should().Equal("https://forum.example/Posts/t1/hello", "https://forum.example/Index");

        copy.Locations = null;
        copy.Locations.Should().HaveCount(2, "null is ignored");
    }

    [Test]
    [Description("The rule engine is created once per combination of flags and gets its services injected")]
    public void Replace_Rules_Provider_Test()
    {
        var injector = this.board.Resolve<IInjectServices>();

        var html = new ProcessReplaceRulesProvider(this.board.Cache, this.board.Current.ServiceLocator, injector, [true, false]);
        var htmlAgain = new ProcessReplaceRulesProvider(this.board.Cache, this.board.Current.ServiceLocator, injector, [true, false]);
        var other = new ProcessReplaceRulesProvider(this.board.Cache, this.board.Current.ServiceLocator, injector, [false, true]);

        html.Instance.Should().BeSameAs(htmlAgain.Instance);
        html.Instance.Should().NotBeSameAs(other.Instance);
        this.board.Cache.Get(string.Format(Constants.Cache.ReplaceRules, 1)).Should().BeSameAs(html.Instance);
    }
}
