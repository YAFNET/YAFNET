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

using System.Threading.Tasks;

using Autofac;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Middleware;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// <see cref="CheckBannedCountries"/>: requests from banned countries get 403.
/// </summary>
[TestFixture]
[NonParallelizable]
public class CheckBannedCountriesTests
{
    private TestBoardContext board;

    private IGeoIpCountryService geoIp;

    [SetUp]
    public void SetUp()
    {
        this.geoIp = Substitute.For<IGeoIpCountryService>();
        this.geoIp.DatabaseExists().Returns(true);
        this.geoIp.GetCountry(Arg.Any<string>()).Returns(new GeoIpData { CountryCode = "XX", CountryName = "Testland" });

        this.board = new TestBoardContext(b => b.RegisterInstance(this.geoIp).As<IGeoIpCountryService>(), useDatabase: true);

        using var connection = this.board.Database.Open();
        connection.Insert(new BannedCountry { BoardID = TestDatabase.BoardId, Country = " xx " });
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A request from a banned country is ended with 403 (country codes are trimmed and case insensitive)")]
    public async Task Banned_Country_Test()
    {
        var (next, status) = await this.RunAsync();

        next.Should().BeFalse();
        status.Should().Be(403);
    }

    [Test]
    [Description("Other countries, unknown addresses and boards without a GeoIP database pass")]
    [TestCase("DE", true)]
    [TestCase("", true)]
    [TestCase("XX", false)]
    public async Task Allowed_Test(string country, bool databaseExists)
    {
        this.geoIp.GetCountry(Arg.Any<string>()).Returns(new GeoIpData { CountryCode = country });
        this.geoIp.DatabaseExists().Returns(databaseExists);

        var (next, status) = await this.RunAsync();

        next.Should().BeTrue();
        status.Should().Be(200);
    }

    private async Task<(bool Next, int Status)> RunAsync()
    {
        var nextCalled = false;

        await new CheckBannedCountries(
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            this.board.Resolve<IServiceLocator>()).InvokeAsync(this.board.HttpContext);

        return (nextCalled, this.board.HttpContext.Response.StatusCode);
    }
}
