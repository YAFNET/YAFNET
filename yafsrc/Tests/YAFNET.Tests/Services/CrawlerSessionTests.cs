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

using System.Text;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using YAF.Core.Middleware;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;

/// <summary>
/// "Abandon Sessions for Don't Track Users": search engines without a session cookie get a session only for the
/// request, nothing of it is kept (every crawler request started a session that stayed in the memory).
/// </summary>
[TestFixture]
[NonParallelizable]
public class CrawlerSessionTests
{
    private const string CookieName = ".YAFNET.Session";

    private const string Crawler = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";

    private const string Browser = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/131.0 Safari/537.36";

    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b => b.RegisterInstance(Options.Create(new SessionOptions { Cookie = { Name = CookieName } }))
                .As<IOptions<SessionOptions>>());
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A crawler without a session cookie gets a session only for the request when the setting is on")]
    public async Task Crawler_Gets_Request_Session_Test()
    {
        this.board.BoardSettings.AbandonSessionsForDontTrack = true;

        var session = await this.RunAsync(Crawler);

        session.Should().BeOfType<CrawlerSession.RequestSession>();

        // the pages can still use it during the request
        session.Set("Forums", Encoding.UTF8.GetBytes("list"));
        session.TryGetValue("Forums", out var value).Should().BeTrue();
        value.Should().Equal(Encoding.UTF8.GetBytes("list"));
    }

    [TestCase(false, Crawler, false, Description = "the setting is off")]
    [TestCase(true, Browser, false, Description = "a browser")]
    [TestCase(true, Crawler, true, Description = "a crawler that keeps its session cookie")]
    [Description("Browsers, crawlers with a session cookie and every request when the setting is off keep the normal session")]
    public async Task Normal_Session_Test(bool abandon, string userAgent, bool hasCookie)
    {
        this.board.BoardSettings.AbandonSessionsForDontTrack = abandon;

        var original = this.board.HttpContext.Session;

        (await this.RunAsync(userAgent, hasCookie)).Should().BeSameAs(original);
    }

    private async Task<ISession> RunAsync(string userAgent, bool hasCookie = false)
    {
        var context = this.board.HttpContext;

        context.Request.Headers.UserAgent = userAgent;

        if (hasCookie)
        {
            context.Request.Headers.Cookie = $"{CookieName}=abc";
        }

        ISession seen = null;

        await new CrawlerSession(
            c =>
            {
                seen = c.Session;
                return Task.CompletedTask;
            },
            this.board.Resolve<IServiceLocator>()).InvokeAsync(context);

        return seen;
    }
}
