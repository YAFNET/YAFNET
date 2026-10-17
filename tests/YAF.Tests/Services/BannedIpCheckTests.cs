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

using System.Collections.Specialized;
using System.Web;

using NSubstitute;

using YAF.Core.Model;
using YAF.Core.Services.Startup;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The check of the banned IPs at the start of a request (<see cref="StartupCheckBannedIps"/>).
/// </summary>
[TestFixture]
[NonParallelizable]
public class BannedIpCheckTests
{
    private TestBoardContext board;

    private HttpResponseBase response;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.response = Substitute.For<HttpResponseBase>();

        this.board.Current.GetRepository<BannedIP>().Save(null, "203.0.113.10", "spam", 2);
        this.board.Current.GetRepository<BannedIP>().Save(null, "198.51.100.*", "spam", 2);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [TestCase("203.0.113.10")]
    [TestCase("198.51.100.77")]
    [Description("Requests from banned IPs (or masks) are ended")]
    public void Banned_Ip_Test(string address)
    {
        this.Check(address).Should().BeFalse();

        this.response.Received().End();
    }

    [Test]
    [Description("Other requests pass")]
    public void Not_Banned_Test()
    {
        this.Check("192.0.2.1").Should().BeTrue();

        this.response.DidNotReceive().End();
    }

    [Test]
    [Description("Behind a reverse proxy the client address of the forwarded header is checked, not the proxy address")]
    public void Reverse_Proxy_Test()
    {
        this.Check("10.0.0.5", "203.0.113.10").Should().BeFalse();

        this.response.Received().End();
    }

    [Test]
    [Description("A client (not a trusted proxy) can not escape a ban with a forwarded header of another address")]
    public void Forged_Forwarded_Header_Test()
    {
        this.Check("203.0.113.10", "192.0.2.1").Should().BeFalse();

        this.response.Received().End();
    }

    [Test]
    [Description("Requests without a remote address are not banned and do not fail")]
    public void No_Remote_Address_Test()
    {
        this.Check(null).Should().BeTrue();

        this.response.DidNotReceive().End();
    }

    /// <summary>
    /// Runs the check for a request.
    /// </summary>
    /// <returns>Returns if the request may continue.</returns>
    private bool Check(string remoteAddress, string forwardedFor = null)
    {
        var request = Substitute.For<HttpRequestBase>();
        request.UserHostAddress.Returns(remoteAddress);
        request.ServerVariables.Returns(new NameValueCollection { ["REMOTE_ADDR"] = remoteAddress });
        request.Headers.Returns(new NameValueCollection { ["X-Forwarded-For"] = forwardedFor });

        var check = new StartupCheckBannedIps(
            this.board.Cache,
            this.response,
            request,
            this.board.Current.GetRepository<BannedIP>(),
            this.board.Resolve<ILoggerService>());

        check.Run();

        return check.Initialized;
    }
}
