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

namespace YAF.Tests.UITests.Tests;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Follows the links of the forum (only GET requests) as admin, member and guest and reports the pages that fail
/// (server error, error page, an error in the event log) and the pages where a script of the user input runs.
/// </summary>
[TestFixture]
public class CrawlTests : ForumTestBase
{
    /// <summary>
    /// The maximum number of pages per role.
    /// </summary>
    private const int MaxPages = 400;

    /// <summary>
    /// Html with a script of the user input (the crawl reports the pages where it runs).
    /// </summary>
    private const string Canary = "<img src=x onerror=window.__xss=1>";

    /// <summary>
    /// The numbers (ids, page numbers) of the addresses.
    /// </summary>
    private static readonly Regex Numbers = new(@"(?<=[/=])\d+(?=[/?&-]|$)");

    /// <summary>
    /// The pages where the script of the canary ran.
    /// </summary>
    private readonly ConcurrentQueue<string> scriptPages = new();

    /// <summary>
    /// The pages of the last crawl.
    /// </summary>
    private readonly List<string> lastVisited = [];

    [Test]
    [Description("The pages of the forum don't fail for admins, members and guests, the user input never runs as script")]
    public async Task Crawled_Pages_Do_Not_Fail_Test()
    {
        var failures = new List<string>();

        // the canary reports itself on the console of every page
        await this.Context.AddInitScriptAsync(
            """
            Object.defineProperty(window, '__xss', { set() { console.log('__XSS__ ' + location.href); } });
            """);

        this.Page.Console += (_, message) =>
            {
                if (message.Text.StartsWith("__XSS__ ", StringComparison.Ordinal))
                {
                    this.scriptPages.Enqueue(message.Text.Substring("__XSS__ ".Length));
                }
            };

        await this.CreateContentAsync();

        ClearEventLog();

        await this.LoginAsAdminAsync();
        failures.AddRange(await this.CrawlAsync("admin"));
        failures.AddRange(await this.CheckInvalidIdsAsync("admin"));

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        failures.AddRange(await this.CrawlAsync("member"));
        failures.AddRange(await this.CheckInvalidIdsAsync("member"));

        await this.LogoutAsync();
        failures.AddRange(await this.CrawlAsync("guest"));
        failures.AddRange(await this.CheckInvalidIdsAsync("guest"));

        // the errors that were caught and logged on the way
        failures.AddRange(LoggedErrors().Select(e => $"event log: {e}"));

        failures.AddRange(this.scriptPages.Distinct().Select(page => $"script of the user input runs at {page}"));

        // the crawl reports the failures itself (the server errors of the pages are part of them)
        this.ClearServerErrors();

        failures.Should().BeEmpty(string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// Deletes the event log (only the errors of the crawl are checked, other tests log errors on purpose).
    /// </summary>
    private static void ClearEventLog()
    {
        using var connection = new SqlConnection(TestSite.ConnectionString);
        connection.Open();

        using var command = new SqlCommand("DELETE FROM [dbo].[yaf_EventLog]", connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Gets the errors with an exception of the event log.
    /// </summary>
    private static List<string> LoggedErrors()
    {
        var errors = new List<string>();

        using var connection = new SqlConnection(TestSite.ConnectionString);
        connection.Open();

        using var command = new SqlCommand(
            "SELECT [Source], [Description] FROM [dbo].[yaf_EventLog] WHERE [Description] LIKE '%ExceptionMessage%'",
            connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var description = reader.GetString(1);
            errors.Add($"{reader.GetString(0)}: {description.Substring(0, Math.Min(description.Length, 600))}");
        }

        return errors;
    }

    /// <summary>
    /// Creates the content of the crawl: a topic with a reply and a poll, an album, a profile and a signature,
    /// the texts of the users contain the canary.
    /// </summary>
    private async Task CreateContentAsync()
    {
        // the member: profile, signature, album and a topic
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("Profile/EditProfile");
        await this.Control("ProfileEditor_Realname").FillAsync($"Real {Canary}");
        await this.Control("ProfileEditor_City").FillAsync($"City {Canary}");
        await this.Control("ProfileEditor_Location").FillAsync($"Location {Canary}");
        await this.Control("ProfileEditor_Interests").FillAsync($"Interests {Canary}");
        await this.Control("ProfileEditor_Occupation").FillAsync($"Occupation {Canary}");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        await this.GotoAsync("Profile/EditSignature");
        await this.Control("SignatureEditor_YafTextEditor").FillAsync($"Signature {Canary}");
        await this.ClickAndWaitAsync(this.Control("SignatureEditor_save"));

        await this.GotoAsync("EditAlbumImages?a=new");
        await this.Control("txtTitle").FillAsync($"Album {Canary}");
        await this.Control("File").SetInputFilesAsync(
            new FilePayload
            {
                Name = "image.png",
                MimeType = "image/png",
                Buffer = Convert.FromBase64String(
                    "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==")
            });
        await this.ClickAndWaitAsync(this.Control("Upload"));

        var topicHref = await this.CreateTopicAsync(
            $"Crawl {Canary} & <b>x</b> \"q\" 'a' / ? #h %20",
            $"[quote]Quote[/quote] [b]bold[/b] {Canary} [url]https://example.com/a?b=1&c=2[/url]");

        // a reply of the admin and a poll
        await this.LoginAsAdminAsync();

        await this.GotoAsync(topicHref);
        var topicId = Regex.Match(topicHref, @"Posts/t(\d+)").Groups[1].Value;

        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.Control("YafTextEditor").FillAsync($"A reply {Canary}");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync($"PollEdit?t={topicId}");
        await this.Control("Question").FillAsync($"Question {Canary}");
        await this.Page.Locator("[id$='_ChoiceRepeater_ctl00_PollChoice']").FillAsync($"Choice {Canary}");
        await this.Page.Locator("[id$='_ChoiceRepeater_ctl01_PollChoice']").FillAsync("Second choice");
        await this.ClickAndWaitAsync(this.Control("SavePoll"));
    }

    /// <summary>
    /// Creates a topic in the test forum.
    /// </summary>
    /// <returns>Returns the address of the topic.</returns>
    private async Task<string> CreateTopicAsync(string subject, string text)
    {
        await this.GotoAsync("PostTopic?f=1");

        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync(text);

        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Topics/1-Test Forum");

        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = "Crawl" }).First
                       .GetAttributeAsync("href");

        return href.TrimStart('/');
    }

    /// <summary>
    /// Visits the pages reachable from the start page (breadth first) and returns the failed ones.
    /// </summary>
    private async Task<List<string>> CrawlAsync(string role)
    {
        var baseUri = new Uri(TestConfig.BaseUrl);
        var failures = new List<string>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue = new Queue<string>();

        queue.Enqueue(baseUri.ToString());

        while (queue.Count > 0 && visited.Count < MaxPages)
        {
            var url = queue.Dequeue();

            if (!visited.Add(url))
            {
                continue;
            }

            var failure = await this.VisitAsync(role, url, notFoundIsValid: false);

            if (failure != null)
            {
                failures.Add(failure);
                continue;
            }

            // a logout by a link would end the crawl of the role
            if (role != "guest" && await this.Page.Locator("[id$='_UserDropDown']").CountAsync() == 0)
            {
                failures.Add($"{role}: logged out at {url}");
                break;
            }

            string[] links;

            try
            {
                links = await this.Page.Locator("a[href]").EvaluateAllAsync<string[]>("links => links.map(l => l.href)");
            }
            catch (PlaywrightException)
            {
                // a page that is not html (e.g. a feed)
                continue;
            }

            foreach (var link in links)
            {
                if (!Uri.TryCreate(link, UriKind.Absolute, out var uri) || uri.Host != baseUri.Host || uri.Port != baseUri.Port)
                {
                    continue;
                }

                var target = uri.GetLeftPart(UriPartial.Query);

                if (IsExcluded(target))
                {
                    continue;
                }

                if (!visited.Contains(target))
                {
                    queue.Enqueue(target);
                }
            }
        }

        await TestContext.Progress.WriteLineAsync($"[crawl] {role}: {visited.Count} pages, {failures.Count} failures");

        foreach (var failure in failures)
        {
            await TestContext.Progress.WriteLineAsync($"[crawl] {failure}");
        }

        this.ClearServerErrors();

        var kinds = visited.Select(url => new Uri(url).AbsolutePath.Trim('/'))
            .Select(path => path.StartsWith("Admin/", StringComparison.OrdinalIgnoreCase)
                            || path.StartsWith("Moderate/", StringComparison.OrdinalIgnoreCase)
                            || path.StartsWith("Profile/", StringComparison.OrdinalIgnoreCase)
                            || path.StartsWith("Account/", StringComparison.OrdinalIgnoreCase)
                                ? string.Join("/", path.Split('/').Take(2))
                                : path.Split('/')[0])
            .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(k => k);

        await TestContext.Progress.WriteLineAsync($"[crawl] {role} pages: {string.Join(", ", kinds)}");

        this.lastVisited.Clear();
        this.lastVisited.AddRange(visited);

        return failures;
    }

    /// <summary>
    /// Calls the pages of the last crawl with invalid numbers (ids, page numbers) in the address: one address of
    /// each kind, every number replaced by 0, -1, a missing id and a number too big for an int.
    /// </summary>
    private async Task<List<string>> CheckInvalidIdsAsync(string role)
    {
        var failures = new List<string>();

        var shapes = this.lastVisited.GroupBy(url => Numbers.Replace(url, "#")).Select(group => group.First())
            .Where(url => Numbers.IsMatch(url)).ToList();

        var checkedUrls = 0;

        foreach (var url in shapes)
        {
            foreach (Match match in Numbers.Matches(url))
            {
                foreach (var value in new[] { "0", "-1", "999999999", "2147483648" })
                {
                    var target = $"{url.Substring(0, match.Index)}{value}{url.Substring(match.Index + match.Length)}";

                    checkedUrls++;

                    var failure = await this.VisitAsync(role, target, notFoundIsValid: true);

                    if (failure != null)
                    {
                        failures.Add(failure);
                    }
                }
            }
        }

        await TestContext.Progress.WriteLineAsync(
            $"[invalid ids] {role}: {shapes.Count} kinds of pages, {checkedUrls} addresses, {failures.Count} failures");

        foreach (var failure in failures)
        {
            await TestContext.Progress.WriteLineAsync($"[invalid ids] {failure}");
        }

        // the failures are reported by the crawl (not by the checks of the next page)
        this.ClearServerErrors();

        // the user is still logged in (an invalid id may lead to the login page)
        return failures;
    }

    /// <summary>
    /// Opens a page, returns the failure or <c>null</c>. A link of the forum must not be "not found", an address
    /// with an invalid id may be.
    /// </summary>
    private async Task<string> VisitAsync(string role, string url, bool notFoundIsValid)
    {
        IResponse response;

        try
        {
            response = await this.Page.GotoAsync(url, new PageGotoOptions { Timeout = 30000 });
        }
        catch (PlaywrightException)
        {
            // downloads (files, feeds) don't open a page
            return null;
        }
        catch (TimeoutException)
        {
            return $"{role}: timeout {url}";
        }

        if (response is null)
        {
            return null;
        }

        if (response.Status == 404)
        {
            return notFoundIsValid ? null : $"{role}: not found {url}";
        }

        if (response.Status >= 500)
        {
            return $"{role}: {response.Status} {url}";
        }

        var contentType = response.Headers.TryGetValue("content-type", out var type) ? type : string.Empty;

        if (!contentType.Contains("text/html"))
        {
            return null;
        }

        var content = await this.Page.ContentAsync();

        return content.Contains("Server Error in '/' Application") ? $"{role}: error page {url}" : null;
    }

    /// <summary>
    /// The links that are not followed: logout, installation, files, the api and links that change something.
    /// </summary>
    private static bool IsExcluded(string url)
    {
        string[] excluded =
        [
            "/Account/Logout", "logout", "/install/", "/api/", "resource.ashx", "FileUploader.ashx", "/Feed",
            "/Admin/RestartApp", "/Admin/TestData", "/Account/DeleteAccount", "/Profile/DeleteAccount", "javascript:"
        ];

        return excluded.Any(part => url.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0);
    }
}
