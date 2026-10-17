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

using System.Net.Http;
using System.Text;

using YAF.Core.Context.Start;

/// <summary>
/// The api refuses requests that a form of another web site can send with the login cookie of the user.
/// </summary>
[TestFixture]
public class CrossSiteRequestFilterTests
{
    [TestCase("application/x-www-form-urlencoded")]
    [TestCase("multipart/form-data")]
    [TestCase("text/plain")]
    [Description("Posts with the content types of a form (of any web site) are refused")]
    public void Form_Post_Is_Refused_Test(string contentType)
    {
        CrossSiteRequestFilter.IsForgeable(Post(new StringContent("id=1&value=x", Encoding.UTF8, contentType)))
            .Should().BeTrue();
    }

    [Test]
    [Description("A post without content (e.g. a form without fields) is refused")]
    public void Empty_Post_Is_Refused_Test()
    {
        CrossSiteRequestFilter.IsForgeable(Post(null)).Should().BeTrue();
    }

    [Test]
    [Description("The json requests of the forum scripts pass")]
    public void Json_Post_Passes_Test()
    {
        CrossSiteRequestFilter.IsForgeable(Post(new StringContent("{}", Encoding.UTF8, "application/json")))
            .Should().BeFalse();
    }

    [TestCase("RequestVerificationToken")]
    [TestCase("X-Requested-With")]
    [Description("Posts with an own header of the forum scripts pass (e.g. the album title editor sends form data)")]
    public void Own_Header_Passes_Test(string header)
    {
        var request = Post(new StringContent("id=1&value=x", Encoding.UTF8, "multipart/form-data"));
        request.Headers.Add(header, "1");

        CrossSiteRequestFilter.IsForgeable(request).Should().BeFalse();
    }

    [Test]
    [Description("Reading requests pass")]
    public void Get_Passes_Test()
    {
        CrossSiteRequestFilter.IsForgeable(new HttpRequestMessage(HttpMethod.Get, "https://forum.example.com/api/x"))
            .Should().BeFalse();
    }

    private static HttpRequestMessage Post(HttpContent content)
    {
        return new HttpRequestMessage(HttpMethod.Post, "https://forum.example.com/api/ThankYou/AddThanks/1") { Content = content };
    }
}
