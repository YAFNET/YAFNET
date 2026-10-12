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

using System.Collections.Generic;
using System.IO;
using System.Linq;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Features;

/// <summary>
/// Follows the links of the forum (only GET requests) as admin, member and guest and reports
/// the pages that fail (server error or the error page).
/// </summary>
public class CrawlTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The maximum number of pages per role.
    /// </summary>
    private const int MaxPages = 250;

    /// <summary>
    /// Html with a script of the user input (the crawl reports the pages where it runs).
    /// </summary>
    private const string Canary = "<img src=x onerror=window.__xss=1>";

    /// <summary>
    /// The pages where the script of the canary ran (also later, e.g. in content loaded by a script).
    /// </summary>
    private readonly System.Collections.Concurrent.ConcurrentQueue<string> scriptPages = new();

    /// <summary>
    /// The name of the board before the crawl (the docker stack is shared by the test classes).
    /// </summary>
    private string boardName;

    /// <summary>
    /// The user with the display name, an other language and time zone.
    /// </summary>
    private string localeUserName;

    /// <summary>
    /// A member of a group with moderator access to all forums (no admin).
    /// </summary>
    private string moderatorUserName;

    /// <summary>
    /// The pages of the last crawl.
    /// </summary>
    private readonly List<string> lastVisited = [];

    /// <summary>
    /// The pages of the forum don't fail for admins, members and guests.
    /// </summary>
    [Test]
    public async Task CrawledPagesDoNotFailTest()
    {
        var failures = new List<string>();

        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // the canary reports itself on the console of every page
                    await page.AddInitScriptAsync(
                        """
                        Object.defineProperty(window, '__xss', { set() {
                            const target = window.event && window.event.target;
                            const html = target && target.parentElement ? target.parentElement.outerHTML.slice(0, 300) : '';
                            console.log('__XSS__ ' + location.href + ' ' + html.replace(/\s+/g, ' '));
                        } });
                        """);

                    page.Console += (_, message) =>
                        {
                            if (message.Text.StartsWith("__XSS__ ", StringComparison.Ordinal))
                            {
                                this.scriptPages.Enqueue(message.Text["__XSS__ ".Length..]);
                            }
                        };

                    // a user with html in the display name (and a post of the user)
                    await this.CreateCanaryDisplayNameAsync(page);

                    // a forum moderator (not an admin)
                    await this.CreateModeratorAsync(page);

                    try
                    {
                        // some content: a topic with a reply, so topic and post pages exist
                        Assert.That(
                            await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                            Is.True,
                            "Login failed");

                        await page.CreateTopicAsync(
                            this.TestSettings,
                            this.TestSettings.TestForumId,
                            $"Crawl topic {Random.Shared.Next()}",
                            "A topic of the crawl test https://example.com/x");

                        // a name with characters of urls and html, and an emoji (links, slugs and lists of topics)
                        await page.CreateTopicAsync(
                            this.TestSettings,
                            this.TestSettings.TestForumId,
                            "C# & .NET <b>x</b> \"q\" 'a' / ? #h %20 😀",
                            "[quote]Quote[/quote] [b]bold[/b] [url]https://example.com/a?b=1&c=2[/url]");

                        // bb codes with html and scripts in the values (names, urls, colors ...)
                        await this.CreateBBCodeTopicAsync(page);

                        // a topic with a poll and an album with an image (poll, album pages)
                        await this.CreatePollTopicAsync(page);
                        await this.CreateAlbumAsync(page);

                        // a reply moved to a new topic (the subject of the new topic is entered on the move page)
                        await this.SplitReplyAsync(page);

                        // the canary in the texts of the admin and the profile
                        await this.CreateCanaryTextsAsync(page);
                        await this.CreateAdminCanaryTextsAsync(page);

                        // the check of the script works (the canary as html runs)
                        await this.GoHomeAsync(page);
                        await page.EvaluateAsync("html => document.body.insertAdjacentHTML('beforeend', `<div id=\"canary-control\">${html}</div>`)", Canary);

                        for (var wait = 0; wait < 50 && this.scriptPages.IsEmpty; wait++)
                        {
                            await page.WaitForTimeoutAsync(100);
                        }

                        Assert.That(this.scriptPages, Is.Not.Empty, "The check of the canary does not work");

                        // the message of the check can still arrive
                        await page.WaitForTimeoutAsync(500);
                        this.scriptPages.Clear();

                        // the search results are loaded by a script
                        foreach (var term in new[] { "Moved", "Choice" })
                        {
                            await page.GotoAsync($"{this.TestSettings.TestForumUrl}Search?search={term}");
                            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                        }

                        failures.AddRange(await this.CrawlAsync(page, "admin"));
                        failures.AddRange(await this.CheckInvalidIdsAsync(page, "admin"));

                        // the last page can be a not found page (without the user menu), or still loading
                        await this.GoHomeAsync(page);

                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                        Assert.That(
                            await page.LoginUserAsync(this.TestSettings, this.TestSettings.TestUserName, this.TestSettings.TestUserPassword),
                            Is.True,
                            "Login failed");

                        failures.AddRange(await this.CrawlAsync(page, "member"));
                        failures.AddRange(await this.CheckInvalidIdsAsync(page, "member"));

                        await this.GoHomeAsync(page);

                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                        failures.AddRange(await this.CrawlAsync(page, "guest"));
                        failures.AddRange(await this.CheckInvalidIdsAsync(page, "guest"));

                        await this.GoHomeAsync(page);

                        // a member with an other language and time zone (formats of dates and numbers)
                        Assert.That(
                            await page.LoginUserAsync(this.TestSettings, this.localeUserName, this.TestSettings.TestUserPassword),
                            Is.True,
                            "Login failed");

                        failures.AddRange(await this.CrawlAsync(page, "locale"));

                        await this.GoHomeAsync(page);

                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                        // a moderator of all forums who is no admin
                        Assert.That(
                            await page.LoginUserAsync(this.TestSettings, this.moderatorUserName, this.TestSettings.TestUserPassword),
                            Is.True,
                            "Login failed");

                        failures.AddRange(await this.CrawlAsync(page, "moderator"));
                        failures.AddRange(await this.CheckInvalidIdsAsync(page, "moderator"));

                        await this.GoHomeAsync(page);

                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                        // the errors that were logged (and caught) on the way
                        Assert.That(
                            await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                            Is.True,
                            "Login failed");

                        // the name of the board is used by other tests
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");
                        await page.Locator("#Input_Name").FillAsync(this.boardName);
                        await page.SubmitAndWaitAsync(page.Locator("button[formaction$='/Save'], button[formaction*='handler=Save']").First);

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EventLog");

                        var logged = await page.Locator(".list-group-item").EvaluateAllAsync<string[]>(
                                         "items => items.map(i => i.textContent.replace(/\\s+/g, ' ').trim()).filter(t => t.includes('Exception'))");

                        failures.AddRange(logged.Select(entry => $"event log: {entry[..Math.Min(entry.Length, 600)]}"));
                    }
                    finally
                    {
                        // the other tests run without display names
                        await page.SetDisplayNamesAsync(this.TestSettings, false);
                    }
                });

        failures.AddRange(
            this.scriptPages.Where(message => !message.Contains("canary-control")).Distinct()
                .Select(message => $"script of the user input runs at {message}"));

        Assert.That(failures, Is.Empty, string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// Visits the pages reachable from the start page (breadth first) and returns the failed ones.
    /// </summary>
    private async Task<List<string>> CrawlAsync(IPage page, string role)
    {
        var baseUri = new Uri(this.TestSettings.TestForumUrl);
        var failures = new List<string>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue = new Queue<string>();

        // the first page of each kind waits for the content loaded by scripts (e.g. the forum jump)
        var numbers = new System.Text.RegularExpressions.Regex(@"(?<=[/=])\d+(?=[/?&]|$)");
        var waitedKinds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        queue.Enqueue(baseUri.ToString());

        while (queue.Count > 0 && visited.Count < MaxPages)
        {
            var url = queue.Dequeue();

            if (!visited.Add(url))
            {
                continue;
            }

            IResponse response;

            try
            {
                response = await page.GotoAsync(url, new PageGotoOptions { Timeout = 15000 });
            }
            catch (Exception ex) when (ex is PlaywrightException or TimeoutException)
            {
                // downloads (files, feeds) don't open a page
                continue;
            }

            if (response is null)
            {
                continue;
            }

            var contentType = response.Headers.TryGetValue("content-type", out var type) ? type : string.Empty;

            if (response.Status >= 500)
            {
                failures.Add($"{role}: {response.Status} {url}");
                continue;
            }

            if (!contentType.Contains("text/html"))
            {
                continue;
            }

            if (waitedKinds.Add(numbers.Replace(new Uri(url).AbsolutePath, "#")))
            {
                try
                {
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 3000 });
                }
                catch (TimeoutException)
                {
                    // a page that keeps a connection open (e.g. the chat)
                }
            }

            if (await page.Locator("h1", new PageLocatorOptions { HasText = "Forum Error" }).CountAsync() > 0)
            {
                failures.Add($"{role}: error page ({response.Status}) {url}");
                continue;
            }

            // links with a script of the user input (they run when they are clicked)
            var scriptLinks = await page.EvaluateAsync<string[]>(
                """
                () => [...document.querySelectorAll('[href*="__xss"], [src*="__xss"], [action*="__xss"], [formaction*="__xss"]')]
                    .map(e => e.getAttribute('href') || e.getAttribute('src') || e.getAttribute('action') || e.getAttribute('formaction'))
                    .filter(url => /^\s*(javascript|data|vbscript):/i.test(url))
                """);

            failures.AddRange(scriptLinks.Select(link => $"{role}: script link {link} at {url}"));

            // a logout by a link would end the crawl of the role
            if (await page.Locator("//li[contains(@class,'dropdown-notify')]").CountAsync() == 0 && role != "guest")
            {
                failures.Add($"{role}: logged out at {url}");
                break;
            }

            var links = await page.Locator("a[href]").EvaluateAllAsync<string[]>("links => links.map(l => l.href)");

            foreach (var link in links)
            {
                if (!Uri.TryCreate(link, UriKind.Absolute, out var uri) || uri.Host != baseUri.Host || uri.Port != baseUri.Port)
                {
                    continue;
                }

                var target = uri.GetLeftPart(UriPartial.Query);

                // no files, feeds, apis and links that change something (all changes are posts, but to be sure)
                if (target.Contains("/api/", StringComparison.OrdinalIgnoreCase)
                    || target.Contains("/Logout", StringComparison.OrdinalIgnoreCase)
                    || target.Contains("handler=", StringComparison.OrdinalIgnoreCase)
                    || target.Contains("/Install", StringComparison.OrdinalIgnoreCase)
                    || target.Contains("/RestartApp", StringComparison.OrdinalIgnoreCase)
                    || target.Contains("/ReIndex", StringComparison.OrdinalIgnoreCase)
                    || target.Contains("/RunSql", StringComparison.OrdinalIgnoreCase)
                    || target.Contains("/DeleteAccount", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!visited.Contains(target))
                {
                    queue.Enqueue(target);
                }
            }
        }

        TestContext.Progress.WriteLine($"[crawl] {role}: {visited.Count} pages, {failures.Count} failures");

        // the kinds of pages that were reached (to see what the crawl covers)
        var kinds = visited.Select(url => new Uri(url).AbsolutePath.Trim('/'))
            .Select(path => path.StartsWith("Admin/", StringComparison.OrdinalIgnoreCase) || path.StartsWith("Moderate/", StringComparison.OrdinalIgnoreCase)
                                ? string.Join('/', path.Split('/').Take(2))
                                : path.Split('/')[0])
            .Distinct(StringComparer.OrdinalIgnoreCase).Order();

        TestContext.Progress.WriteLine($"[crawl] {role} pages: {string.Join(", ", kinds)}");

        this.lastVisited.Clear();
        this.lastVisited.AddRange(visited);

        return failures;
    }

    /// <summary>
    /// Calls the pages of the last crawl with invalid numbers (ids, page numbers) in the address:
    /// one address of each kind, every number replaced by 0, -1, a missing id and a number too big for an int.
    /// </summary>
    private async Task<List<string>> CheckInvalidIdsAsync(IPage page, string role)
    {
        var failures = new List<string>();
        var numbers = new System.Text.RegularExpressions.Regex(@"(?<=[/=])\d+(?=[/?&]|$)");

        // pages without a link on the forum pages (e.g. the moderation of a forum without reported posts)
        if (role == "admin")
        {
            this.lastVisited.AddRange(
                new[] { "Moderate/UnapprovedPosts/1", "Moderate/ReportedPosts/1", "Moderate/Forums/1" }.Select(
                    path => $"{this.TestSettings.TestForumUrl}{path}"));
        }

        var shapes = this.lastVisited
            .Where(url => !url.Contains("/Admin/", StringComparison.OrdinalIgnoreCase) || role == "admin")
            .GroupBy(url => numbers.Replace(url, "#"))
            .Select(group => group.First())
            .Where(url => numbers.IsMatch(url))
            .ToList();

        var checkedUrls = 0;

        foreach (var url in shapes)
        {
            foreach (System.Text.RegularExpressions.Match match in numbers.Matches(url))
            {
                foreach (var value in new[] { "0", "-1", "999999999", "2147483648" })
                {
                    var target = $"{url[..match.Index]}{value}{url[(match.Index + match.Length)..]}";

                    checkedUrls++;

                    IResponse response;

                    try
                    {
                        response = await page.GotoAsync(target, new PageGotoOptions { Timeout = 15000 });
                    }
                    catch (Exception ex) when (ex is PlaywrightException or TimeoutException)
                    {
                        continue;
                    }

                    if (response is null)
                    {
                        continue;
                    }

                    if (response.Status >= 500)
                    {
                        failures.Add($"{role}: {response.Status} {target}");
                        continue;
                    }

                    // a number that is no int does not match the route (not found)
                    if (response.Status != 404
                        && await page.Locator("h1", new PageLocatorOptions { HasText = "Forum Error" }).CountAsync() > 0)
                    {
                        failures.Add($"{role}: error page ({response.Status}) {target}");
                    }
                }
            }
        }

        TestContext.Progress.WriteLine($"[invalid ids] {role}: {shapes.Count} kinds of pages, {checkedUrls} addresses, {failures.Count} failures");

        return failures;
    }

    /// <summary>
    /// Creates a topic with bb codes that contain html and scripts in their values, the text is set
    /// directly in the editor (as bb code source).
    /// </summary>
    private async Task CreateBBCodeTopicAsync(IPage page)
    {
        const string Focus = "\" autofocus onfocus=\"window.__xss=1";
        const string Error = "\" onerror=\"window.__xss=1";

        var bbCode = string.Join(
            "\n",
            "[quote=Name" + Focus + ";1]another quote[/quote]",
            "[url=https://example.com/" + Focus + "]a link[/url]",
            "[url]https://example.com/" + Focus + "[/url]",
            "[email=a@example.com" + Focus + "]mail[/email]",
            "[img]https://example.com/x.png" + Error + "[/img]",
            "[img=https://example.com/x.png" + Error + "]image[/img]",
            "[color=red" + Focus + "]color[/color]",
            "[size=5" + Focus + "]size[/size]",
            "[font=Arial" + Focus + "]font[/font]",
            "https://example.com/" + Focus,
            "[youtube]https://www.youtube.com/watch?v=x" + Focus + "[/youtube]",
            "[albumimg]1" + Error + "[/albumimg]",
            "[attach]1" + Error + "[/attach]",
            "[quote=Name" + Error + "]quote[/quote]",
            "[userlink]Name" + Focus + "[/userlink]",
            "[code=html" + Focus + "]code[/code]",
            "[code=html]" + Canary + "[/code]");

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

        await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Crawl bb codes {Random.Shared.Next()}");

        await page.EvaluateAsync(
            "text => sceditor.instance([...document.querySelectorAll('textarea')].find(t => sceditor.instance(t))).val(text)",
            bbCode);

        await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

        Assert.That(page.Url, Does.Contain("/Posts/"), "The bb code topic was not created");
    }

    /// <summary>
    /// Creates a topic with a poll of two choices.
    /// </summary>
    private async Task CreatePollTopicAsync(IPage page)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

        await page.Locator("//input[contains(@id,'_AddPoll')]").CheckAsync();
        await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Crawl poll {Random.Shared.Next()}");

        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic with a poll");

        // tags (the list of the tags is filled by the tags select), one is html without the end of the tag
        await page.Locator("#Input_TagsValue").EvaluateAsync(
            "(input, canary) => input.value = 'crawltag,' + canary.slice(0, -1) + ' x='",
            Canary);

        await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

        await page.Locator("//input[contains(@id,'Question')]").First.FillAsync("Crawl question?");
        await page.Locator("//input[contains(@id,'Input_Choices_0__ChoiceName')]").FillAsync("Choice 1");
        await page.Locator("//input[contains(@id,'Input_Choices_1__ChoiceName')]").FillAsync($"Choice 2 {Canary}");
        await page.Locator("//input[contains(@id,'_ShowVotersCheckBox')]").CheckAsync();

        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'SavePoll')]"));

        Assert.That(await page.ContentAsync(), Does.Contain("Choice 1"), "The poll was not created");
    }

    /// <summary>
    /// Creates a reply in the test topic and moves it to a new topic.
    /// </summary>
    private async Task SplitReplyAsync(IPage page)
    {
        Assert.That(await page.TryCreateNewReplyInTestTopicAsync(this.TestSettings, "A reply to move"), Is.True, "The reply was not created");

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Posts/{this.TestSettings.TestTopicId}/test");

        var moveLink = await page.Locator("a[href*='MoveMessage/']").Last.GetAttributeAsync("href");

        await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), moveLink!).ToString());

        await page.Locator("#Input_TopicSubject").FillAsync($"Moved reply {Canary}");

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='CreateAndMove']"));

        Assert.That(await page.ContentAsync(), Does.Contain("Moved reply"), "The reply was not moved to a new topic");
    }

    /// <summary>
    /// Turns on the display names and creates a user with the canary in the display name who replies
    /// in the test topic (the names are shown in the lists of topics, the posts and the user lists).
    /// </summary>
    private async Task CreateCanaryDisplayNameAsync(IPage page)
    {
        var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

        await page.GotoAsync(this.TestSettings.TestForumUrl);

        if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
        {
            await page.LogOutUserAsync();
        }

        await page.SetDisplayNamesAsync(this.TestSettings, true);

        // the users can choose a language (like the language test of the profile, the setting stays on)
        Assert.That(
            await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
            Is.True,
            "Login failed");

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings/Permission");

        var allowLanguage = page.Locator("#Input_AllowUserLanguage");

        if (!await allowLanguage.IsCheckedAsync())
        {
            await allowLanguage.CheckAsync();
            await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']:visible").First);
        }

        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

        Assert.That(
            await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
            Is.True,
            "Login failed");

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");
        await page.Locator("#Input_DisplayName").FillAsync($"Crawl {Canary}");
        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateProfile')]"));

        Assert.That(
            await page.TryCreateNewReplyInTestTopicAsync(this.TestSettings, "A reply of a user with a display name"),
            Is.True,
            "The reply was not created");

        // german and a time zone far from utc (the selects are Choices.js selects, only the selected option is in the form)
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

        await page.EvaluateAsync(
            """
            () => [['#Language', 'de-DE'], ['#TimeZone', 'Pacific/Kiritimati']].forEach(([id, value]) => {
                const select = document.querySelector(id);
                select.add(new Option(value, value, true, true));
                select.value = value;
            })
            """);

        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@type,'submit')]").Last);

        this.localeUserName = userName;

        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
    }

    /// <summary>
    /// Creates a group with the moderator access to all forums and a user in the group.
    /// </summary>
    private async Task CreateModeratorAsync(IPage page)
    {
        var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

        await page.GotoAsync(this.TestSettings.TestForumUrl);

        if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
        {
            await page.LogOutUserAsync();
        }

        Assert.That(
            await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
            Is.True,
            "Login failed");

        var groupName = $"Crawl Moderators {Random.Shared.Next()}";

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditGroup");
        await page.Locator("#Input_Name").FillAsync(groupName);
        await page.Locator("#Input_IsModeratorX").CheckAsync();
        await page.Locator("#Input_NewAccessMaskID").SelectOptionAsync(new SelectOptionValue { Label = "Moderator Access" });
        await page.SubmitAndWaitAsync(page.Locator("button[formaction$='/Save'], button[formaction*='handler=Save']").First);

        // the user joins the group
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

        var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
        await searchNameInput.FillAsync(userName);
        await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

        var editLink = await page.Locator("//a[contains(@href,'EditUser')]", new PageLocatorOptions { HasText = userName }).First
                           .GetAttributeAsync("href");

        await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), editLink!).ToString());
        await page.Locator("//button[contains(@data-bs-target,'#View2')]").ClickAsync();
        await page.GetByLabel(groupName).First.CheckAsync();
        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UsersGroups?handler=Save')]"));

        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

        this.moderatorUserName = userName;
    }

    /// <summary>
    /// Creates a category and a forum and fills the profile of the admin with the canary.
    /// </summary>
    private async Task CreateCanaryTextsAsync(IPage page)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditCategory");
        // a new name in each run (the docker stack is shared by the test classes)
        var categoryName = $"Crawl category {Random.Shared.Next()}";

        await page.Locator("#Input_Name").FillAsync($"{categoryName} {Canary}");

        if (await page.Locator("#Input_SortOrder").CountAsync() > 0)
        {
            await page.Locator("#Input_SortOrder").FillAsync("99");
        }

        await page.Locator("#Input_Active").CheckAsync();
        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

        // the forum is created in the new category (a category is only listed with forums),
        // the id is taken from the category select (a Choices.js select)
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

        var categoryId = await page.Locator(".choices__item[data-value]", new PageLocatorOptions { HasText = categoryName }).First
                             .GetAttributeAsync("data-value");

        Assert.That(categoryId, Is.Not.Null, "The category was not created");
        await page.Locator("#Input_Name").FillAsync($"Crawl forum {Canary}");

        // the select is a Choices.js select (only the selected option is in the form)
        await page.EvaluateAsync(
            """
            id => {
                const select = document.querySelector('#Input_CategoryID');
                select.add(new Option('', id, true, true));
                select.value = id;
            }
            """,
            categoryId!);
        await page.Locator("#Input_Description").FillAsync($"Crawl description {Canary}");
        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

        // a forum that only the admins can read, but that is listed for the others (without a link)
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");
        await page.Locator("#Input_Name").FillAsync($"Crawl no access {Canary}");
        await page.Locator("#Input_Description").FillAsync("A forum without access");
        await page.Locator("#Input_HideNoAccess").UncheckAsync();

        await page.EvaluateAsync(
            """
            () => document.querySelectorAll("select[name$='AccessMaskID']").forEach(select => {
                const label = document.querySelector(`label[for='${select.id}']`).textContent.trim();

                if (label !== 'Administrators') {
                    select.value = [...select.options].find(o => o.text.trim() === 'No Access').value;
                }
            })
            """);

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

        await page.GotoAsync(this.TestSettings.TestForumUrl);

        Assert.That(await page.ContentAsync(), Does.Contain("Crawl forum").And.Contain("Crawl category"), "The forum or the category was not created");

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

        foreach (var field in new[] { "RealName", "Location", "Interests", "Occupation", "City" })
        {
            await page.Locator($"#Input_{field}").FillAsync($"{field} {Canary}");
        }

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='UpdateProfile']"));
    }

    /// <summary>
    /// Creates a group, a rank, an access mask, a medal, a banned ip and a profile field with the canary
    /// (the texts of the admins of a board are shown to the host admin).
    /// </summary>
    private async Task CreateAdminCanaryTextsAsync(IPage page)
    {
        foreach (var (adminPage, required) in new[]
                                                  {
                                                      ("EditGroup", new Dictionary<string, string>()),
                                                      ("EditRank", new Dictionary<string, string> { ["#Input_RankPriority"] = "1" }),
                                                      ("EditAccessMask", new Dictionary<string, string> { ["#Input_SortOrder"] = "9" })
                                                  })
        {
            await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/{adminPage}");

            await page.Locator("#Input_Name").FillAsync($"Crawl {adminPage} {Canary}");

            if (await page.Locator("#Input_Description").CountAsync() > 0)
            {
                await page.Locator("#Input_Description").FillAsync($"Crawl description {Canary}");
            }

            foreach (var (field, value) in required)
            {
                await page.Locator(field).FillAsync(value);
            }

            await page.SubmitAndWaitAsync(page.Locator("button[formaction$='/Save'], button[formaction*='handler=Save']").First);

            Assert.That(page.Url, Does.Not.Contain($"Admin/{adminPage}"), $"{adminPage} was not saved");
        }

        // the name of the board (shown on every page)
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");
        this.boardName = await page.Locator("#Input_Name").InputValueAsync();
        await page.Locator("#Input_Name").FillAsync($"Crawl board {Canary}");
        await page.SubmitAndWaitAsync(page.Locator("button[formaction$='/Save'], button[formaction*='handler=Save']").First);

        // a medal
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditMedal");

        await page.Locator("#Input_Name").FillAsync($"Crawl medal {Canary}");
        await page.Locator("#Input_Category").FillAsync($"Crawl {Canary}");
        await page.Locator("#Input_Description").FillAsync($"Crawl description {Canary}");
        await page.Locator("#Input_Message").FillAsync($"Crawl message {Canary}");
        await page.Locator("#Input_MedalImage").EvaluateAsync("select => select.value = 'medal.webp'");

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

        // a banned ip and a profile field (saved by the dialogs)
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Admin");

        var statuses = await page.EvaluateAsync<int[]>(
            """
            async ([baseUrl, canary]) => {
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                const post = (url, body) => fetch(baseUrl + url, {
                    method: 'POST',
                    headers: { 'RequestVerificationToken': token, 'Content-Type': 'application/json' },
                    body: JSON.stringify(body)
                }).then(response => response.status);

                return [
                    await post('api/BannedIp/Edit', { mask: '203.0.113.99', banReason: 'Crawl ' + canary }),
                    await post('api/ProfileDefinition/Edit', {
                        name: 'Crawl field ' + canary, dataType: 'Text', length: 50, required: false,
                        showInUserInfo: true, showOnRegisterPage: true, defaultValue: ''
                    })
                ];
            }
            """,
            new[] { this.TestSettings.TestForumUrl, Canary });

        Assert.That(statuses, Is.All.EqualTo(200), "The banned ip or the profile field was not saved");
    }

    /// <summary>
    /// Enables the albums and creates an album with an image.
    /// </summary>
    private async Task CreateAlbumAsync(IPage page)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings");
        await page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Album Settings" }).ClickAsync();
        await page.GetByRole(AriaRole.Checkbox, new PageGetByRoleOptions { Name = "Enable Album Feature:" }).CheckAsync();
        await page.SubmitAndWaitAsync(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save Settings" }));

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");
        await page.GetByText("Edit Albums").First.ClickAsync();
        await page.GetByText("Add New Album").ClickAsync();

        await page.Locator("#AlbumTitle").FillAsync($"Crawl album & \"q\" {Canary}");
        await page.Locator("#ImageFiles").SetInputFilesAsync(Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png"));

        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Upload')]"));

        Assert.That(await page.Locator("img[src*='GetImagePreview']").CountAsync(), Is.GreaterThan(0), "The album image was not uploaded");
    }

    /// <summary>
    /// Goes to the start page (a navigation of the checks can still be running).
    /// </summary>
    private async Task GoHomeAsync(IPage page)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                await page.GotoAsync(this.TestSettings.TestForumUrl);
                return;
            }
            catch (PlaywrightException) when (attempt < 3)
            {
                await page.WaitForTimeoutAsync(1000);
            }
        }
    }
}
