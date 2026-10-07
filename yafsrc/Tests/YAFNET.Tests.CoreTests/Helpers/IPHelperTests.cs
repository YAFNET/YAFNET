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

namespace YAF.Tests.CoreTests.Helpers;

using System.Net;

using Microsoft.AspNetCore.Http;

/// <summary>
/// YAF.Core.Helpers IPHelper Tests
/// </summary>
[TestFixture]
public class IPHelperTests
{
    [TestCase("43.*.*.*", "43.134.127.238", true)]
    [TestCase("43.*.*.*", "44.134.127.238", false)]
    [TestCase("192.168.1.1", "192.168.1.1", true)]
    [TestCase(" 192.168.1.1 ", "192.168.1.1", true)]
    [TestCase("10.0.0.1", "10.0.0.2", false)]
    [TestCase("::1", "::1", true)]
    [TestCase("127.0.0.1", "::1", true)]
    [TestCase("::1", "127.0.0.1", true)]
    [Description("IsBanned matches IPs and wildcard masks, treating ::1 as localhost")]
    public void IsBanned_Test(string ban, string ip, bool expected)
    {
        IPHelper.IsBanned(ban, ip).Should().Be(expected);
    }

    [Test]
    [Description("IsBanned throws on null arguments")]
    public void IsBanned_Null_Throws_Test()
    {
        FluentActions.Invoking(() => IPHelper.IsBanned(null, "1.1.1.1")).Should().Throw<ArgumentNullException>();
        FluentActions.Invoking(() => IPHelper.IsBanned("1.1.1.1", null)).Should().Throw<ArgumentNullException>();
    }

    [TestCase("8.8.8.8", true)]
    [TestCase("1.1.1.1", true)]
    [TestCase("172.32.0.1", true)]
    [TestCase("10.1.2.3", false)]
    [TestCase("172.16.5.4", false)]
    [TestCase("172.31.255.255", false)]
    [TestCase("192.168.178.1", false)]
    [TestCase("169.254.10.10", false)]
    [TestCase("127.0.0.1", false)]
    [TestCase("0.0.0.0", false)]
    [TestCase("2001:4860:4860::8888", true)]
    [TestCase("2a00:1450:4001:82a::200e", true)]
    [TestCase("::1", false)]
    [TestCase("::", false)]
    [TestCase("fc00::1", false)]
    [TestCase("fd12:3456::1", false)]
    [TestCase("2001:db8::1", false)]
    [Description("IsRoutable rejects private, loopback, link-local and documentation ranges")]
    public void IsRoutable_Test(string ip, bool expected)
    {
        IPAddress.Parse(ip).IsRoutable().Should().Be(expected);
    }

    [Test]
    [Description("IsRoutable throws on null")]
    public void IsRoutable_Null_Throws_Test()
    {
        IPAddress ip = null;

        FluentActions.Invoking(() => ip.IsRoutable()).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("GetUserRealIPAddress returns the first routable X-Forwarded-For entry")]
    public void GetUserRealIPAddress_Forwarded_For_Test()
    {
        var context = CreateContext("10.0.0.5");
        context.Request.Headers["X-Forwarded-For"] = "192.168.1.10, 8.8.8.8, 1.1.1.1";

        context.GetUserRealIPAddress().Should().Be("8.8.8.8");
        context.Request.GetUserRealIPAddress().Should().Be("8.8.8.8");
    }

    [Test]
    [Description("GetUserRealIPAddress falls back to X-Real-IP when X-Forwarded-For is only private")]
    public void GetUserRealIPAddress_Real_Ip_Test()
    {
        var context = CreateContext("10.0.0.5");
        context.Request.Headers["X-Forwarded-For"] = "192.168.1.10";
        context.Request.Headers["X-Real-IP"] = "9.9.9.9";

        context.GetUserRealIPAddress().Should().Be("9.9.9.9");
    }

    [Test]
    [Description("GetUserRealIPAddress falls back to the connection address")]
    public void GetUserRealIPAddress_Remote_Address_Test()
    {
        CreateContext("203.0.113.7").GetUserRealIPAddress().Should().Be("203.0.113.7");

        var invalidRealIp = CreateContext("203.0.113.7");
        invalidRealIp.Request.Headers["X-Real-IP"] = "not-an-ip";

        invalidRealIp.GetUserRealIPAddress().Should().Be("203.0.113.7");
    }

    [Test]
    [Description("GetUserRealIPAddress throws on null")]
    public void GetUserRealIPAddress_Null_Throws_Test()
    {
        HttpContext context = null;
        HttpRequest request = null;

        FluentActions.Invoking(() => context.GetUserRealIPAddress()).Should().Throw<ArgumentNullException>();
        FluentActions.Invoking(() => request.GetUserRealIPAddress()).Should().Throw<ArgumentNullException>();
    }

    [TestCase("192.168.*.*")]
    [TestCase("127.0.0.1")]
    [Description("GetIpAddressAsString returns wildcard masks and IP literals unchanged")]
    public void GetIpAddressAsString_Test(string input)
    {
        IPHelper.GetIpAddressAsString(input).Should().Be(input);
    }

    private static DefaultHttpContext CreateContext(string remoteIp)
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(remoteIp);
        return context;
    }
}
