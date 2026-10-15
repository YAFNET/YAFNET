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

using System.Net;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Middleware;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The ban middlewares (IP, country, user agent): blocked requests are logged when enabled, a ban list that can
/// not be read lets the requests pass instead of breaking the forum.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BanMiddlewareTests
{
    private const string BannedIp = "203.0.113.9";

    private const string BannedAgent = "EvilBot/1.0";

    private TestBoardContext board;

    private ILogger logger;

    public enum Ban
    {
        Ip,
        Country,
        UserAgent
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [TestCase(Ban.Ip)]
    [TestCase(Ban.Country)]
    [TestCase(Ban.UserAgent)]
    [Description("A blocked request is written to the event log when banned requests are logged")]
    public async Task Blocked_Request_Is_Logged_Test(Ban ban)
    {
        this.SetUpBoard(ban);
        this.board.BoardSettings.LogBannedIP = true;

        var (next, status) = await this.RunAsync(ban);

        next.Should().BeFalse();
        status.Should().Be((int)HttpStatusCode.Forbidden);

        this.logger.ReceivedCalls().Select(c => c.GetArguments()[0]).OfType<EventLog>()
            .Should().ContainSingle(e => e.Type == (int)EventLogTypes.IpBanDetected);
    }

    [TestCase(Ban.Ip)]
    [TestCase(Ban.Country)]
    [TestCase(Ban.UserAgent)]
    [Description("A blocked request is not logged when banned requests are not logged")]
    public async Task Blocked_Request_Not_Logged_Test(Ban ban)
    {
        this.SetUpBoard(ban);
        this.board.BoardSettings.LogBannedIP = false;

        (await this.RunAsync(ban)).Status.Should().Be((int)HttpStatusCode.Forbidden);

        this.logger.ReceivedCalls().Should().BeEmpty();
    }

    [TestCase(Ban.Ip)]
    [TestCase(Ban.Country)]
    [TestCase(Ban.UserAgent)]
    [Description("When the ban list can not be read (e.g. during an upgrade) the requests pass")]
    public async Task Ban_List_Error_Test(Ban ban)
    {
        this.SetUpBoard(ban);

        using (var db = this.board.Database.Open())
        {
            switch (ban)
            {
                case Ban.Ip:
                    db.DropTable<BannedIP>();
                    break;
                case Ban.Country:
                    db.DropTable<BannedCountry>();
                    break;
                default:
                    db.DropTable<BannedUserAgent>();
                    break;
            }
        }

        var (next, status) = await this.RunAsync(ban);

        next.Should().BeTrue();
        status.Should().Be(200);
    }

    private void SetUpBoard(Ban ban)
    {
        var geoIp = Substitute.For<IGeoIpCountryService>();
        geoIp.DatabaseExists().Returns(true);
        geoIp.GetCountry(Arg.Any<string>()).Returns(new GeoIpData { CountryCode = "XX", CountryName = "Testland" });

        this.logger = ban switch
        {
            Ban.Ip => Substitute.For<ILogger<CheckBannedIps>>(),
            Ban.Country => Substitute.For<ILogger<CheckBannedCountries>>(),
            _ => Substitute.For<ILogger<CheckBannedUserAgents>>()
        };

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(geoIp).As<IGeoIpCountryService>();
                b.RegisterInstance(this.logger).As(this.logger.GetType().GetInterfaces().First(i => i.IsGenericType));
            },
            useDatabase: true);

        this.board.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse(BannedIp);
        this.board.HttpContext.Request.Headers.UserAgent = BannedAgent;

        using var db = this.board.Database.Open();

        switch (ban)
        {
            case Ban.Ip:
                db.Insert(new BannedIP { BoardID = TestDatabase.BoardId, Mask = BannedIp, Since = DateTime.UtcNow });
                break;
            case Ban.Country:
                db.Insert(new BannedCountry { BoardID = TestDatabase.BoardId, Country = "XX" });
                break;
            default:
                db.Insert(new BannedUserAgent { BoardID = TestDatabase.BoardId, UserAgent = BannedAgent });
                break;
        }
    }

    private async Task<(bool Next, int Status)> RunAsync(Ban ban)
    {
        var nextCalled = false;

        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var locator = this.board.Resolve<IServiceLocator>();

        switch (ban)
        {
            case Ban.Ip:
                await new CheckBannedIps(next, locator).InvokeAsync(this.board.HttpContext);
                break;
            case Ban.Country:
                await new CheckBannedCountries(next, locator).InvokeAsync(this.board.HttpContext);
                break;
            default:
                await new CheckBannedUserAgents(next, locator).InvokeAsync(this.board.HttpContext);
                break;
        }

        return (nextCalled, this.board.HttpContext.Response.StatusCode);
    }
}
