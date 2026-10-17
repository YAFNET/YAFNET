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

namespace YAF.Tests.Helpers;

using System.Collections.Specialized;
using System.Net;
using System.Web;

using NSubstitute;

/// <summary>
/// YAF.Core.Helpers IPHelper Tests
/// </summary>
[TestFixture]
[NonParallelizable]
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
    [TestCase("1.2.3.*", "1.2.3.99", true)]
    [TestCase("10.0.0.1", "110.0.0.15", false)]
    [TestCase("10.0.0.1", "10.0.0.12", false)]
    [TestCase("10.0.0.1", "210.0.0.1", false)]
    [TestCase("1.2.3.*", "1.2.30.4", false)]
    [TestCase("1.2.3.*", "41.2.3.9", false)]
    [TestCase("2001:db8::1", "2001:db8::1", true)]
    [TestCase("2001:db8::1", "2001:db8::10", false)]
    [TestCase("(", "1.2.3.4", false)]
    [TestCase("not an ip", "1.2.3.4", false)]
    [Description("IsBanned matches IPs and wildcard masks exactly, treating ::1 as localhost; invalid masks match nothing")]
    public void IsBanned_Test(string ban, string ip, bool expected)
    {
        IPHelper.IsBanned(ban, ip).Should().Be(expected);
    }

    [TestCase("192.168.1.1", true)]
    [TestCase(" 192.168.1.* ", true)]
    [TestCase("*.*.*.*", true)]
    [TestCase("2001:db8::1", true)]
    [TestCase("1.2.3", false)]
    [TestCase("300.1.1.1", false)]
    [TestCase("1.2.3.4.5", false)]
    [TestCase("1.2.*", false)]
    [TestCase("(", false)]
    [TestCase("", false)]
    [Description("Only ip addresses and ipv4 masks with * parts are valid ban masks")]
    public void IsValidBanMask_Test(string mask, bool expected)
    {
        IPHelper.IsValidBanMask(mask).Should().Be(expected);
    }

    [Test]
    [Description("An ipv4 address in ipv6 notation (::ffff:a.b.c.d) is matched like the ipv4 address")]
    public void IsBanned_Mapped_IpV4_Test()
    {
        IPHelper.IsBanned("203.0.113.10", "::ffff:203.0.113.10").Should().BeTrue();
        IPHelper.IsBanned("203.0.113.*", "::ffff:203.0.113.10").Should().BeTrue();
    }

    [TestCase("1.2.3.*", "2001:db8::1")]
    [TestCase("1.2.3.4", "2001:db8::1")]
    [TestCase("1.2.3.*", "")]
    [TestCase("1.2.3.4", "")]
    [TestCase("1.2.3.*", "1.2.3")]
    [Description("An ipv6 address, an empty or incomplete address never matches an ipv4 ban")]
    public void IsBanned_Not_IpV4_Test(string ban, string ip)
    {
        IPHelper.IsBanned(ban, ip).Should().BeFalse();
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
    [Description("GetUserRealIPAddress ignores X-Forwarded-For and X-Real-IP of clients, a client could send any address")]
    public void GetUserRealIPAddress_Ignores_Headers_Of_Clients_Test()
    {
        CreateRequest("203.0.113.7", forwardedFor: "8.8.8.8").GetUserRealIPAddress().Should().Be("203.0.113.7");
        CreateRequest("203.0.113.7", realIp: "9.9.9.9").GetUserRealIPAddress().Should().Be("203.0.113.7");
    }

    [Test]
    [Description("GetUserRealIPAddress uses X-Forwarded-For of loopback and private proxies")]
    public void GetUserRealIPAddress_Forwarded_For_Test()
    {
        CreateRequest("10.0.0.5", forwardedFor: "8.8.8.8").GetUserRealIPAddress().Should().Be("8.8.8.8", "a proxy in the private network");
        CreateRequest("127.0.0.1", forwardedFor: "8.8.8.8").GetUserRealIPAddress().Should().Be("8.8.8.8", "a proxy on the same host");
        CreateRequest("::1", forwardedFor: "8.8.8.8").GetUserRealIPAddress().Should().Be("8.8.8.8", "a proxy on the same host (ipv6)");
        CreateRequest("10.0.0.5", forwardedFor: "8.8.8.8, 192.168.1.2").GetUserRealIPAddress().Should().Be("8.8.8.8", "a chain of trusted proxies");
        CreateRequest("10.0.0.5", forwardedFor: "1.1.1.1, 8.8.8.8").GetUserRealIPAddress()
            .Should().Be("8.8.8.8", "the address in front of the last untrusted address was sent by the client");
        CreateRequest("10.0.0.5", forwardedFor: "not-an-ip").GetUserRealIPAddress().Should().Be("10.0.0.5");
    }

    [Test]
    [Description("GetUserRealIPAddress uses X-Real-IP of trusted proxies")]
    public void GetUserRealIPAddress_Real_Ip_Test()
    {
        CreateRequest("10.0.0.5", realIp: "9.9.9.9").GetUserRealIPAddress().Should().Be("9.9.9.9");
        CreateRequest("10.0.0.5", realIp: "not-an-ip").GetUserRealIPAddress().Should().Be("10.0.0.5");
    }

    [TestCase("8.8.8.8", false)]
    [TestCase("10.1.2.3", true)]
    [TestCase("172.16.5.4", true)]
    [TestCase("192.168.178.1", true)]
    [TestCase("127.0.0.1", true)]
    [TestCase("::1", true)]
    [TestCase("fd12:3456::1", true)]
    [TestCase("::ffff:10.1.2.3", true)]
    [TestCase("2001:4860:4860::8888", false)]
    [Description("Loopback and private networks are trusted proxies")]
    public void IsTrustedProxy_Test(string ip, bool expected)
    {
        IPHelper.IsTrustedProxy(IPAddress.Parse(ip)).Should().Be(expected);
    }

    [Test]
    [Description("GetUserRealIPAddress falls back to the connection address")]
    public void GetUserRealIPAddress_Remote_Address_Test()
    {
        CreateRequest("203.0.113.7").GetUserRealIPAddress().Should().Be("203.0.113.7");
    }

    [Test]
    [Description("A connection address of an ipv4 client in ipv6 notation is returned as ipv4 address (the bans are ipv4)")]
    public void GetUserRealIPAddress_Mapped_IpV4_Test()
    {
        CreateRequest("::ffff:203.0.113.7").GetUserRealIPAddress().Should().Be("203.0.113.7");
    }

    [Test]
    [Description("GetUserRealIPAddress returns an empty address for requests without a remote address instead of failing")]
    public void GetUserRealIPAddress_No_Remote_Address_Test()
    {
        CreateRequest(null).GetUserRealIPAddress().Should().BeEmpty();
    }

    [Test]
    [Description("GetUserRealIPAddress throws on null")]
    public void GetUserRealIPAddress_Null_Throws_Test()
    {
        HttpRequestBase request = null;

        FluentActions.Invoking(() => request.GetUserRealIPAddress()).Should().Throw<ArgumentNullException>();
    }

    [TestCase("192.168.*.*")]
    [TestCase("127.0.0.1")]
    [Description("GetIpAddressAsString returns wildcard masks and IP literals unchanged")]
    public void GetIpAddressAsString_Test(string input)
    {
        IPHelper.GetIpAddressAsString(input).Should().Be(input);
    }

    private static HttpRequestBase CreateRequest(string remoteIp, string forwardedFor = null, string realIp = null)
    {
        var headers = new NameValueCollection();

        if (forwardedFor != null)
        {
            headers["X-Forwarded-For"] = forwardedFor;
        }

        if (realIp != null)
        {
            headers["X-Real-IP"] = realIp;
        }

        var request = Substitute.For<HttpRequestBase>();
        request.UserHostAddress.Returns(remoteIp);
        request.Headers.Returns(headers);

        return request;
    }
}
