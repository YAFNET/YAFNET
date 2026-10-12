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

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using NSubstitute;

using YAF.Core.Services;
using YAF.Core.Services.CheckForSpam;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;

/// <summary>
/// The web service clients (bot checks, ip info, latest version) use the http client factory,
/// the requests are answered by a fake handler.
/// </summary>
[TestFixture]
[NonParallelizable]
public class WebServiceClientTests
{
    private const string NotListed = """{"success":1,"username":{"appears":0,"frequency":0},"email":{"appears":0,"frequency":0},"ip":{"appears":0,"frequency":0}}""";

    private TestBoardContext board;

    private FakeHandler handler;

    [SetUp]
    public void SetUp()
    {
        this.handler = new FakeHandler();

        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient(Arg.Any<string>()).Returns(_ => new HttpClient(this.handler, false));

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(factory).As<IHttpClientFactory>();
                b.RegisterType<SpamCheck>().As<ISpamCheck>();
                b.RegisterType<IpInfoService>().As<IIpInfoService>();
                b.RegisterType<LatestInformationService>().As<ILatestInformationService>();
            },
            useDatabase: true);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        this.handler?.Dispose();
    }

    [TestCase(1, 1, 0, true, TestName = "StopForumSpam_Name_And_Email_Test")]
    [TestCase(1, 0, 1, true, TestName = "StopForumSpam_Name_And_Ip_Test")]
    [TestCase(0, 1, 1, true, TestName = "StopForumSpam_Email_And_Ip_Test")]
    [TestCase(1, 0, 0, false, TestName = "StopForumSpam_Only_Name_Test")]
    [TestCase(0, 0, 1, false, TestName = "StopForumSpam_Only_Ip_Test")]
    [Description("StopForumSpam reports a bot when two of name, e-mail and ip are listed")]
    public async Task StopForumSpam_Test(int name, int email, int ip, bool isBot)
    {
        this.handler.Respond(
            $$$"""{"success":1,"username":{"appears":{{{name}}}},"email":{"appears":{{{email}}}},"ip":{"appears":{{{ip}}}}}""");

        (await new StopForumSpam().IsBotAsync("10.0.0.1", "bot@example.com", "bot")).IsBot.Should().Be(isBot);
    }

    [Test]
    [Description("The values are url encoded, a plus in an e-mail address is not turned into a space")]
    public async Task StopForumSpam_Url_Encoded_Test()
    {
        this.handler.Respond(NotListed);

        await new StopForumSpam().IsBotAsync("10.0.0.1", "first+last@example.com", "Tom & Jerry");

        var query = this.handler.Requests.Single().RequestUri!.Query;
        query.Should().Contain("email=first%2Blast%40example.com")
            .And.Contain("username=Tom%20%26%20Jerry")
            .And.Contain("ip=10.0.0.1")
            .And.EndWith("&f=json");
    }

    [Test]
    [Description("A failed request, a failed check or a response without a value is no bot")]
    public async Task StopForumSpam_Errors_Test()
    {
        this.handler.Respond("""{"success":0}""");
        (await new StopForumSpam().IsBotAsync("10.0.0.1", "bot@example.com", "bot")).IsBot.Should().BeFalse();

        // only the ip was checked, the response has no username or email
        this.handler.Respond("""{"success":1,"ip":{"appears":1}}""");
        (await new StopForumSpam().IsBotAsync("10.0.0.1", null, null)).IsBot.Should().BeFalse();

        this.handler.Respond("not json");
        (await new StopForumSpam().IsBotAsync("10.0.0.1", "bot@example.com", "bot")).IsBot.Should().BeFalse();

        this.handler.Respond(HttpStatusCode.InternalServerError, "error");
        (await new StopForumSpam().IsBotAsync("10.0.0.1", "bot@example.com", "bot")).IsBot.Should().BeFalse();
    }

    [Test]
    [Description("A bot is reported to StopForumSpam with the api key")]
    public async Task StopForumSpam_Report_Test()
    {
        this.board.BoardSettings.StopForumSpamApiKey = "secret key";

        this.handler.Respond("<response success=\"true\"></response>");
        (await new StopForumSpam().ReportUserAsBotAsync("10.0.0.1", "bot@example.com", "bot")).Should().BeTrue();

        this.handler.Requests.Single().RequestUri!.Query.Should().Contain("api_key=secret%20key");

        this.handler.Respond("<response><error>invalid key</error></response>");
        (await new StopForumSpam().ReportUserAsBotAsync("10.0.0.1", "bot@example.com", "bot")).Should().BeFalse();
    }

    [TestCase("Y|MULTI|IP|1|MAIL|1|NAME|0", true, TestName = "BotScout_Ip_And_Mail_Test")]
    [TestCase("Y|MULTI|IP|0|MAIL|3|NAME|2", true, TestName = "BotScout_Name_And_Mail_Test")]
    [TestCase("Y|MULTI|IP|4|MAIL|0|NAME|1", true, TestName = "BotScout_Name_And_Ip_Test")]
    [TestCase("Y|MULTI|IP|0|MAIL|0|NAME|5", false, TestName = "BotScout_Only_Name_Test")]
    [TestCase("N|MULTI|IP|0|MAIL|0|NAME|0", false, TestName = "BotScout_Not_Found_Test")]
    [Description("BotScout reports a bot when two of name, e-mail and ip are listed")]
    public async Task BotScout_Test(string response, bool isBot)
    {
        this.handler.Respond(response);

        var result = await new BotScout().IsBotAsync("10.0.0.1", "bot@example.com", "bot");

        result.IsBot.Should().Be(isBot);
        result.ResponseText.Should().Be(response);
    }

    [Test]
    [Description("BotScout gets the encoded values and the api key, a failed request is no bot")]
    public async Task BotScout_Request_Test()
    {
        this.board.BoardSettings.BotScoutApiKey = "key&1";
        this.handler.Respond("N|MULTI|IP|0|MAIL|0|NAME|0");

        await new BotScout().IsBotAsync("10.0.0.1", "first+last@example.com", "bot");

        this.handler.Requests.Single().RequestUri!.Query.Should().Contain("&mail=first%2Blast%40example.com")
            .And.Contain("&key=key%261");

        this.handler.Fail();
        (await new BotScout().IsBotAsync("10.0.0.1", "bot@example.com", "bot")).IsBot.Should().BeFalse();
    }

    [Test]
    [Description("The board setting decides which bot services are asked and how their answers are combined")]
    public async Task Bot_Service_Settings_Test()
    {
        var spamCheck = this.board.Resolve<ISpamCheck>();

        // StopForumSpam finds a bot, BotScout does not
        this.handler.Respond(
            request => request.RequestUri!.Host.Contains("stopforumspam")
                           ? """{"success":1,"username":{"appears":1},"email":{"appears":1},"ip":{"appears":0}}"""
                           : "N|MULTI|IP|0|MAIL|0|NAME|0");

        this.board.BoardSettings.BotSpamServiceType = (int)BotSpamService.NoService;
        (await spamCheck.CheckUserForSpamBotAsync("bot", "bot@example.com", "10.0.0.1")).IsBot.Should().BeFalse();
        this.handler.Requests.Should().BeEmpty("no service is asked");

        this.board.BoardSettings.BotSpamServiceType = (int)BotSpamService.StopForumSpam;
        (await spamCheck.CheckUserForSpamBotAsync("bot", "bot@example.com", "10.0.0.1")).IsBot.Should().BeTrue();

        // without an api key BotScout is replaced by StopForumSpam
        this.board.BoardSettings.BotSpamServiceType = (int)BotSpamService.BotScout;
        (await spamCheck.CheckUserForSpamBotAsync("bot", "bot@example.com", "10.0.0.1")).IsBot.Should().BeTrue();

        this.board.BoardSettings.BotScoutApiKey = "key";
        (await spamCheck.CheckUserForSpamBotAsync("bot", "bot@example.com", "10.0.0.1")).IsBot.Should().BeFalse();

        this.board.BoardSettings.BotSpamServiceType = (int)BotSpamService.BothServiceMatch;
        (await spamCheck.CheckUserForSpamBotAsync("bot", "bot@example.com", "10.0.0.1")).IsBot.Should().BeFalse();

        this.board.BoardSettings.BotSpamServiceType = (int)BotSpamService.OneServiceMatch;
        (await spamCheck.CheckUserForSpamBotAsync("bot", "bot@example.com", "10.0.0.1")).IsBot.Should().BeTrue();

        // without a key both modes only ask StopForumSpam
        this.board.BoardSettings.BotScoutApiKey = string.Empty;
        this.handler.Requests.Clear();
        this.board.BoardSettings.BotSpamServiceType = (int)BotSpamService.BothServiceMatch;
        (await spamCheck.CheckUserForSpamBotAsync("bot", "bot@example.com", "10.0.0.1")).IsBot.Should().BeTrue();
        this.board.BoardSettings.BotSpamServiceType = (int)BotSpamService.OneServiceMatch;
        (await spamCheck.CheckUserForSpamBotAsync("bot", "bot@example.com", "10.0.0.1")).IsBot.Should().BeTrue();
        this.handler.Requests.Should().OnlyContain(r => r.RequestUri!.Host.Contains("stopforumspam"));
    }

    [Test]
    [Description("The ip locator service is only asked when it is enabled, an error status gives nothing")]
    public async Task Ip_Locator_Test()
    {
        var service = this.board.Resolve<IIpInfoService>();

        this.board.BoardSettings.IPLocatorUrlPath = "https://ip.example.com/?ip={0}";
        this.board.BoardSettings.EnableIPInfoService = false;
        (await service.GetUserIpLocatorAsync("10.0.0.1")).Should().BeNull();
        (await service.GetUserIpLocatorAsync(null)).Should().BeNull();
        this.handler.Requests.Should().BeEmpty();

        this.board.BoardSettings.EnableIPInfoService = true;
        this.handler.Respond("""{"statusCode":"OK","countryCode":"DE","countryName":"Germany"}""");

        var locator = await service.GetUserIpLocatorAsync("10.0.0.1");
        locator.CountryCode.Should().Be("DE");
        this.handler.Requests.Single().RequestUri!.ToString().Should().Be("https://ip.example.com/?ip=10.0.0.1&format=json");

        this.handler.Respond("""{"statusCode":"ERROR","statusMessage":"limit reached"}""");
        (await service.GetUserIpLocatorAsync("10.0.0.1")).Should().BeNull();

        this.handler.Fail();
        (await service.GetUserIpLocatorAsync("10.0.0.1")).Should().BeNull();
    }

    [Test]
    [Description("AbuseIPDB requests send the api key, the ip is url encoded")]
    public async Task Abuse_Ip_Db_Test()
    {
        var service = this.board.Resolve<IIpInfoService>();

        this.handler.Respond("""{"data":{"ipAddress":"10.0.0.1","abuseConfidenceScore":87,"totalReports":3}}""");

        var report = await service.CheckAbuseIpdDbAsync("10.0.0.1", "api-key");
        report.Data.AbuseConfidenceScore.Should().Be(87);

        var request = this.handler.Requests.Single();
        request.Headers.GetValues("Key").Should().Equal("api-key");
        request.RequestUri!.ToString().Should().Be("https://api.abuseipdb.com/api/v2/check?ipAddress=10.0.0.1&maxAgeInDays=30");

        (await service.CheckAbuseIpdDbAsync(null, "api-key")).Should().BeNull();

        this.handler.Respond("""{"meta":{"generatedAt":"2026-10-09T10:00:00Z"},"data":[{"ipAddress":"203.0.113.5","abuseConfidenceScore":100}]}""");

        var blackList = await service.GetAbuseIpDbBlackListAsync("api-key", 90, 5);
        blackList.Data.Single().IpAddress.Should().Be("203.0.113.5");
        this.handler.Requests.Last().RequestUri!.Query.Should().Be("?confidenceMinimum=90&limit=5");
    }

    [Test]
    [Description("The latest v4 release is read from GitHub, an error gives no version")]
    public async Task Latest_Version_Test()
    {
        var service = this.board.Resolve<ILatestInformationService>();

        this.handler.Respond(
            """[{"tag_name":"v5.0.0","published_at":"2026-10-01T00:00:00Z"},{"tag_name":"v4.0.10","published_at":"2026-09-01T00:00:00Z"}]""");

        var version = await service.GetLatestVersionAsync();

        ((string)version.Version).Should().Be("4.0.10");
        this.handler.Requests.Single().Headers.UserAgent.ToString().Should().Be("YAF.NET");

        this.handler.Fail();
        ((IDictionary<string, object>)await service.GetLatestVersionAsync()).Should().NotContainKey("Version");
    }

    /// <summary>
    /// Answers the requests with a configured response and records them.
    /// </summary>
    private sealed class FakeHandler : HttpMessageHandler
    {
        private Func<HttpRequestMessage, HttpResponseMessage> respond = _ => new HttpResponseMessage(HttpStatusCode.NotFound);

        public List<HttpRequestMessage> Requests { get; } = [];

        public void Respond(string content)
        {
            this.Respond(HttpStatusCode.OK, content);
        }

        public void Respond(HttpStatusCode statusCode, string content)
        {
            this.respond = _ => new HttpResponseMessage(statusCode)
                                {
                                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                                };
        }

        public void Respond(Func<HttpRequestMessage, string> content)
        {
            this.respond = request => new HttpResponseMessage(HttpStatusCode.OK)
                                      {
                                          Content = new StringContent(content(request), Encoding.UTF8, "application/json")
                                      };
        }

        public void Fail()
        {
            this.respond = _ => throw new HttpRequestException("network down");
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.Requests.Add(request);

            return Task.FromResult(this.respond(request));
        }
    }
}
