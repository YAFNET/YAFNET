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

using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces.Services;

/// <summary>
/// The country lookup of the GeoIP database (the database of the web project is used).
/// </summary>
[TestFixture]
[NonParallelizable]
public class GeoIpCountryServiceTests
{
    private TestBoardContext board;

    private IWebHostEnvironment environment;

    private static string WebRoot =>
        Path.GetFullPath(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..", "..", "YetAnotherForum.NET", "wwwroot"));

    [SetUp]
    public void SetUp()
    {
        this.environment = Substitute.For<IWebHostEnvironment>();
        this.environment.WebRootPath.Returns(WebRoot);

        this.board = new TestBoardContext(b => b.RegisterInstance(this.environment).As<IWebHostEnvironment>());
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("IPv4 and IPv6 addresses are found, private and invalid addresses have no country")]
    public void Get_Country_Test()
    {
        Assume.That(File.Exists(Path.Combine(WebRoot, "GeoIP.dat")), "the GeoIP database of the web project is missing");

        var service = this.CreateService();

        service.DatabaseExists().Should().BeTrue();

        var google = service.GetCountry("8.8.8.8");
        google.CountryCode.Should().Be("US");
        google.CountryName.Should().Be("United States");

        service.GetCountry(IPAddress.Parse("2001:4860:4860::8888")).CountryCode.Should().Be("US");

        service.GetCountry("192.168.1.10").CountryCode.Should().Be("--");
        service.GetCountry("127.0.0.1").CountryCode.Should().Be("--");

        var invalid = service.GetCountry("not an ip");
        invalid.CountryCode.Should().Be("--");
        invalid.CountryName.Should().Be("N/A");
    }

    [Test]
    [Description("Requests share the cached database, parallel lookups of different services return the right countries")]
    public void Parallel_Lookups_Test()
    {
        Assume.That(File.Exists(Path.Combine(WebRoot, "GeoIP.dat")), "the GeoIP database of the web project is missing");

        // one service per request, all use the same cached database
        var first = this.CreateService();
        var second = this.CreateService();

        var expected = new Dictionary<string, string>
                       {
                           ["8.8.8.8"] = first.GetCountry("8.8.8.8").CountryCode,
                           ["81.169.145.1"] = first.GetCountry("81.169.145.1").CountryCode,
                           ["133.242.0.3"] = first.GetCountry("133.242.0.3").CountryCode,
                           ["192.168.1.10"] = "--"
                       };

        expected.Values.Distinct().Should().HaveCountGreaterThan(2, "the addresses are from different countries");

        var ips = expected.Keys.ToArray();
        var wrong = 0;

        Parallel.For(
            0,
            4000,
            i =>
            {
                var service = i % 2 == 0 ? first : second;
                var ip = ips[i % ips.Length];

                if (service.GetCountry(ip).CountryCode != expected[ip])
                {
                    Interlocked.Increment(ref wrong);
                }
            });

        wrong.Should().Be(0);
    }

    [Test]
    [Description("Without the database file nothing is found, the lookup does not fail")]
    public void Missing_Database_Test()
    {
        this.environment.WebRootPath.Returns(Path.Combine(Path.GetTempPath(), $"yaf-no-geoip-{Guid.NewGuid():N}"));

        var service = this.CreateService();

        service.DatabaseExists().Should().BeFalse();
        service.GetCountry("8.8.8.8").CountryCode.Should().Be("--");
    }

    private IGeoIpCountryService CreateService()
    {
        return new GeoIpCountryService(this.board.Current.ServiceLocator);
    }
}
