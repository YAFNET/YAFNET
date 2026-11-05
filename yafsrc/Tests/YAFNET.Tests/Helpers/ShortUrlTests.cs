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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Rewrite;

using YAF.Configuration;
using YAF.Core.Helpers;

/// <summary>
/// The short urls of the public pages: the routes, the redirects of the old urls and the slugs.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ShortUrlTests
{
    private string originalUrlRewritingMode;

    [SetUp]
    public void SetUp()
    {
        this.originalUrlRewritingMode = Config.UrlRewritingMode;
    }

    [TearDown]
    public void TearDown()
    {
        Config.UrlRewritingMode = this.originalUrlRewritingMode;
    }

    [TestCase("/Posts", null, "Posts/{t:int?}/{name}/{handler?}", "topic/{t:int?}/{name}/{handler?}")]
    [TestCase("/Topics", null, "Topics/{f:int}/{name}/{p:int=1}/{handler?}", "forum/{f:int}/{name}/{p:int=1}/{handler?}")]
    [TestCase("/Post", null, "Post/{m:int}/{name}/{handler?}", "post/{m:int}/{name}/{handler?}")]
    [TestCase("/UserProfile", null, "UserProfile/{u:int}/{name}/{handler?}", "user/{u:int}/{name}/{handler?}")]
    [TestCase("/ActiveUsers", null, "ActiveUsers/{v:int?}/{handler?}", "active-users/{v:int?}/{handler?}")]
    [TestCase("/Posts", "Forums", "Forums/Posts/{t:int?}/{name}/{handler?}", "Forums/topic/{t:int?}/{name}/{handler?}")]
    [TestCase("/PostMessage", null, "PostMessage", "PostMessage")]
    [TestCase("/Admin/Users", null, "Admin/Users/{p:int=1}/{handler?}", "Admin/Users/{p:int=1}/{handler?}")]
    [Description("The convention replaces only the page name of the public pages, also in an area")]
    public void Convention_Test(string page, string area, string template, string expected)
    {
        var model = new PageRouteModel($"/Pages{page}.cshtml", page, area);

        model.Selectors.Add(new SelectorModel { AttributeRouteModel = new AttributeRouteModel { Template = template } });

        new ShortRouteConvention().Apply(model);

        model.Selectors[0].AttributeRouteModel!.Template.Should().Be(expected);
    }

    [TestCase("/Posts/5/hello", null, "/topic/5/hello")]
    [TestCase("/posts/5/hello", null, "/topic/5/hello")]
    [TestCase("/Topics/5/general/2", null, "/forum/5/general/2")]
    [TestCase("/UserProfile/1/ingo", null, "/user/1/ingo")]
    [TestCase("/Post/9/hello", null, "/post/9/hello")]
    [TestCase("/Category/2/news", null, "/category/2/news")]
    [TestCase("/Albums/1", null, "/albums/1")]
    [TestCase("/Album/1/2", null, "/album/1/2")]
    [TestCase("/Members", null, "/members")]
    [TestCase("/ActiveUsers/0", null, "/active-users/0")]
    [TestCase("/Team", null, "/team")]
    [TestCase("/Forums/Posts/5/hello", "Forums", "/Forums/topic/5/hello")]
    [Description("The old urls have a short path")]
    public void Old_Url_Test(string path, string area, string expected)
    {
        ForumRouteHelper.GetShortPath(path, area).Should().Be(expected);
    }

    [TestCase("/topic/5/hello", null)]
    [TestCase("/post/9/hello", null)]
    [TestCase("/category/2/news", null)]
    [TestCase("/members", null)]
    [TestCase("/", null)]
    [TestCase("/Posts/MarkAsAnswer/5", null)]
    [TestCase("/Posts", null)]
    [TestCase("/PostMessage", null)]
    [TestCase("/Admin/Users", null)]
    [TestCase("/PostsAndMore/5", null)]
    [TestCase("//example.com/Posts/5", null)]
    [TestCase("/Posts/5/hello", "Forums")]
    [Description("Short urls, controller actions and other pages are not redirected, so there is no loop")]
    public void Not_An_Old_Url_Test(string path, string area)
    {
        ForumRouteHelper.GetShortPath(path, area).Should().BeNull();
    }

    [Test]
    [Description("The redirect is permanent, keeps the path base and the query and encodes the name")]
    public void Redirect_Test()
    {
        var context = NewContext("GET", "/Posts/5/Привет");

        context.HttpContext.Request.PathBase = "/board";
        context.HttpContext.Request.QueryString = new QueryString("?p=2&m=9");

        new LegacyUrlRedirectRule(null).ApplyRule(context);

        context.Result.Should().Be(RuleResult.EndResponse);
        context.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status301MovedPermanently);
        context.HttpContext.Response.Headers.Location.ToString()
            .Should().Be("/board/topic/5/%D0%9F%D1%80%D0%B8%D0%B2%D0%B5%D1%82?p=2&m=9");
    }

    [TestCase("POST", "/Posts/5/hello")]
    [TestCase("GET", "/topic/5/hello")]
    [Description("Posted data is not redirected, short urls are passed on")]
    public void No_Redirect_Test(string method, string path)
    {
        var context = NewContext(method, path);

        new LegacyUrlRedirectRule(null).ApplyRule(context);

        context.Result.Should().Be(RuleResult.ContinueRules);
        context.HttpContext.Response.Headers.ContainsKey("Location").Should().BeFalse();
    }

    [TestCase("Hello   -  World", "hello-world")]
    [TestCase("-- Hello --", "hello")]
    [TestCase("UPPER case", "upper-case")]
    [Description("Slugs are lowercase, without repeated or outer dashes")]
    public void Slug_Test(string input, string expected)
    {
        Config.UrlRewritingMode = null;

        UrlRewriteHelper.CleanStringForUrl(input).Should().Be(expected);
    }

    [Test]
    [Description("Long slugs are cut at the end of a word, cleaning a slug again doesn't change it")]
    public void Long_Slug_Test()
    {
        Config.UrlRewritingMode = null;

        var slug = UrlRewriteHelper.CleanStringForUrl(string.Join(' ', Enumerable.Repeat("wordy", 40)));

        slug.Length.Should().BeLessThanOrEqualTo(80);
        slug.Should().EndWith("wordy");
        UrlRewriteHelper.CleanStringForUrl(slug).Should().Be(slug);
    }

    [Test]
    [Description("A long name without spaces is cut at the maximum length")]
    public void Long_Word_Slug_Test()
    {
        Config.UrlRewritingMode = null;

        UrlRewriteHelper.CleanStringForUrl(new string('a', 200)).Should().Be(new string('a', 80));
    }

    [TestCase(null, "topic/{t:int}/{name}/{p:int}/{handler?}")]
    [TestCase("Forums", "Forums/topic/{t:int}/{name}/{p:int}/{handler?}")]
    [Description("A topic has a route with the page number, it is matched first, other pages don't get one")]
    public void Topic_Page_Route_Test(string area, string expected)
    {
        var model = new PageRouteModel("/Pages/Posts.cshtml", "/Posts", area);

        new ShortRouteConvention().Apply(model);

        model.Selectors.Should().ContainSingle();
        model.Selectors[0].AttributeRouteModel!.Template.Should().Be(expected);
        model.Selectors[0].AttributeRouteModel!.Order.Should().Be(-1);

        var other = new PageRouteModel("/Pages/Post.cshtml", "/Post", area);

        new ShortRouteConvention().Apply(other);

        other.Selectors.Should().BeEmpty();
    }

    private static RewriteContext NewContext(string method, string path)
    {
        var httpContext = new DefaultHttpContext();

        httpContext.Request.Method = method;
        httpContext.Request.Path = path;

        return new RewriteContext { HttpContext = httpContext, Result = RuleResult.ContinueRules };
    }
}
