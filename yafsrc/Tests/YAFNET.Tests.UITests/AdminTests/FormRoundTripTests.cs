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
using System.Linq;
using System.Text.RegularExpressions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.AdminTests;

/// <summary>
/// Saves the forms of the edit pages without changes: the page must not fail and the values must stay
/// the same (a value that is not sent back or saved differently is lost or changed by every save).
/// </summary>
public class FormRoundTripTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Saves the edit pages of the admin and the profile without changes.
    /// </summary>
    [Test]
    public async Task UnchangedFormsKeepTheirValuesTest()
    {
        var failures = new List<string>();

        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    var pages = await this.CollectPagesAsync(page);

                    await TestContext.Progress.WriteLineAsync($"[forms] {pages.Count} pages");

                    var saved = 0;

                    foreach (var url in pages)
                    {
                        await page.GotoAsync(url);

                        var forms = await page.EvaluateAsync<string[]>(
                                        """
                                        () => [...document.querySelectorAll('form')].map((form, index) => {
                                            const buttons = [...form.querySelectorAll('button[type=submit], button:not([type]), input[type=submit]')]
                                                .filter(b => b.offsetParent !== null)
                                                .map(b => (b.getAttribute('formaction') || form.getAttribute('action') || '') + '|' + (b.id || '') + '|' + b.textContent.trim());
                                            return index + '#' + buttons.join('¦');
                                        })
                                        """);

                        foreach (var form in forms)
                        {
                            var index = int.Parse(form[..form.IndexOf('#')]);

                            var save = form[(form.IndexOf('#') + 1)..].Split('¦', StringSplitOptions.RemoveEmptyEntries)
                                .FirstOrDefault(
                                    b =>
                                        {
                                            var parts = b.Split('|');
                                            var handler = Regex.Match(parts[0], @"(?:handler=|/)(?<h>[A-Za-z]+)(?:\?|$)").Groups["h"].Value;

                                            // the save handlers, or the default handler of a form with a save button
                                            return Regex.IsMatch(handler, "^(Save|Update|UpdateProfile|SaveSettings)$", RegexOptions.IgnoreCase)
                                                   || ((parts[0].Length == 0 || parts[0].Split('?')[0].TrimEnd('/')
                                                           .Equals(new Uri(url).AbsolutePath.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
                                                       && Regex.IsMatch(parts[2], "^(Save|Update)", RegexOptions.IgnoreCase));
                                        });

                            if (save is null)
                            {
                                continue;
                            }

                            var before = await ReadFormAsync(page, index);

                            var action = save.Split('|')[0];

                            // a stored value the form does not accept (e.g. a sort order below the minimum of the field):
                            // the item can't be saved without changing it
                            var rejected = await page.EvaluateAsync<string>(
                                               """
                                               index => [...document.querySelectorAll('form')[index].elements]
                                                   .filter(e => e.willValidate && !e.checkValidity() && !e.validity.valueMissing)
                                                   .map(e => (e.name || e.id) + '=' + e.value + ': ' + e.validationMessage).join('; ')
                                               """,
                                               index);

                            if (!string.IsNullOrEmpty(rejected))
                            {
                                failures.Add($"{url} ({action}) the stored value is not accepted: {rejected}");
                                continue;
                            }

                            // a form with an empty required field is not sent by the browser
                            var invalid = await page.EvaluateAsync<string>(
                                              """
                                              index => [...document.querySelectorAll('form')[index].elements]
                                                  .filter(e => e.willValidate && !e.checkValidity())
                                                  .map(e => (e.name || e.id) + ': ' + e.validationMessage).join('; ')
                                              """,
                                              index);

                            if (!string.IsNullOrEmpty(invalid))
                            {
                                await TestContext.Progress.WriteLineAsync($"[forms] not valid as loaded: {url} ({action}) {invalid}");
                                continue;
                            }

                            IResponse response;

                            try
                            {
                                response = await page.RunAndWaitForNavigationAsync(
                                               () => page.EvaluateAsync(
                                                   """
                                                   ([index, action]) => {
                                                       const form = document.querySelectorAll('form')[index];
                                                       const button = [...form.querySelectorAll('button, input[type=submit]')]
                                                           .find(b => (b.getAttribute('formaction') || form.getAttribute('action') || '') === action);
                                                       form.requestSubmit(button);
                                                   }
                                                   """,
                                                   new object[] { index, action }),
                                               new PageRunAndWaitForNavigationOptions { Timeout = 15000 });
                            }
                            catch (TimeoutException)
                            {
                                failures.Add($"no response saving {url} ({action})");
                                continue;
                            }

                            saved++;
                            await TestContext.Progress.WriteLineAsync($"[forms] saved {url} ({action})");

                            if (response is { Status: >= 500 } || await page.Locator("h1", new PageLocatorOptions { HasText = "Forum Error" }).CountAsync() > 0)
                            {
                                failures.Add($"error ({response?.Status}) saving {url} ({action})");
                                continue;
                            }

                            // the same page again, the values must be the same
                            await page.GotoAsync(url);

                            var forms2 = await page.Locator("form").CountAsync();

                            if (forms2 <= index)
                            {
                                continue;
                            }

                            var after = await ReadFormAsync(page, index);

                            var changed = before.Keys.Union(after.Keys)
                                .Where(k => !k.Contains("RequestVerificationToken") && before.GetValueOrDefault(k) != after.GetValueOrDefault(k))

                                // the minimum posts of a rank only count for ladder ranks (it is removed for the others)
                                .Where(k => k != "Input.MinPosts" || before.GetValueOrDefault("Input.IsLadder", string.Empty).Contains("true"))
                                .Select(k => $"{k}: '{before.GetValueOrDefault(k)}' -> '{after.GetValueOrDefault(k)}'")
                                .ToList();

                            if (changed.Count > 0)
                            {
                                failures.Add($"{url} ({action}) changed: {string.Join("; ", changed.Take(10))}");
                            }
                        }
                    }

                    await TestContext.Progress.WriteLineAsync($"[forms] {saved} forms saved");
                });

        Assert.That(failures, Is.Empty, string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// Changes the text fields of the edit forms (the value with a "Z" at the end), the changed values
    /// must be saved (a field that is ignored by the save keeps its old value), the old values are saved again.
    /// </summary>
    [Test]
    public async Task ChangedTextFieldsAreSavedTest()
    {
        var failures = new List<string>();

        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    // the edit pages of single items (not the settings of the board and the host, the other tests use them)
                    var pages = (await this.CollectPagesAsync(page))
                        .Where(url => Regex.IsMatch(url, @"/Admin/Edit(Category|Forum|Group|Rank|Medal|AccessMask|BBCode)/|/Profile/(EditProfile|EditSignature)|/Admin/EditUser/Users(Info|Profile|Signature)", RegexOptions.IgnoreCase))
                        .ToList();

                    foreach (var url in pages)
                    {
                        await page.GotoAsync(url);

                        var forms = await page.Locator("form").CountAsync();

                        for (var index = 0; index < forms; index++)
                        {
                            var action = await SaveActionAsync(page, index, url);

                            if (action is null)
                            {
                                continue;
                            }

                            // the visible text and number fields with a value (not at their maximum length), with the changed value
                            // (the minimum posts of a rank are only saved for ladder ranks)
                            var fields = await page.EvaluateAsync<string[][]>(
                                             """
                                             index => [...document.querySelectorAll('form')[index].querySelectorAll('input[type=text], input[type=number], input:not([type]), textarea')]
                                                 .filter(e => e.name && e.name !== 'Input.MinPosts' && !e.readOnly && !e.disabled && e.offsetParent !== null
                                                              && e.value.trim().length > 0 && !(e.maxLength > 0 && e.value.length >= e.maxLength))
                                                 .map(e => [e.name, e.value, e.type === 'number' ? String(Number(e.value) + 1) : e.value + 'Z'])
                                             """,
                                             index);

                            // a form for a new item (an empty required field) is not saved
                            var empty = await page.EvaluateAsync<bool>(
                                            "index => [...document.querySelectorAll('form')[index].elements].some(e => e.willValidate && e.validity.valueMissing)",
                                            index);

                            if (fields.Length == 0 || empty)
                            {
                                continue;
                            }

                            await TestContext.Progress.WriteLineAsync($"[persist] {url}: {string.Join(", ", fields.Select(f => f[0]))}");

                            await SetFieldsAsync(page, index, fields.Select(f => new[] { f[0], f[2] }).ToArray());

                            var error = await SubmitAsync(page, index, action);

                            if (error != null)
                            {
                                failures.Add($"not saved {url} ({action}): {error}");
                                continue;
                            }

                            await page.GotoAsync(url);

                            var after = await ReadFormAsync(page, index);

                            failures.AddRange(
                                fields.Where(f => after.GetValueOrDefault(f[0]) != f[2])
                                    .Select(f => $"{url} ({action}) {f[0]}: '{f[2]}' was saved as '{after.GetValueOrDefault(f[0])}'"));

                            // the old values again
                            await SetFieldsAsync(page, index, fields.Select(f => new[] { f[0], f[1] }).ToArray());
                            await SubmitAsync(page, index, action);
                            await page.GotoAsync(url);
                        }
                    }
                });

        Assert.That(failures, Is.Empty, string.Join(Environment.NewLine, failures));
    }

    /// <summary>
    /// The action of the save button of a form, null if the form has none.
    /// </summary>
    private static async Task<string> SaveActionAsync(IPage page, int index, string url)
    {
        var buttons = await page.EvaluateAsync<string[]>(
                          """
                          index => {
                              const form = document.querySelectorAll('form')[index];
                              return [...form.querySelectorAll('button[type=submit], button:not([type]), input[type=submit]')]
                                  .filter(b => b.offsetParent !== null)
                                  .map(b => (b.getAttribute('formaction') || form.getAttribute('action') || '') + '|' + b.textContent.trim());
                          }
                          """,
                          index);

        return buttons.Select(b => b.Split('|'))
            .FirstOrDefault(
                parts => Regex.IsMatch(
                             Regex.Match(parts[0], @"(?:handler=|/)(?<h>[A-Za-z]+)(?:\?|$)").Groups["h"].Value,
                             "^(Save|Update|UpdateProfile|SaveSettings|Add)$",
                             RegexOptions.IgnoreCase)
                         || ((parts[0].Length == 0 || parts[0].Split('?')[0].TrimEnd('/')
                                 .Equals(new Uri(url).AbsolutePath.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
                             && Regex.IsMatch(parts[1], "^(Save|Update)", RegexOptions.IgnoreCase)))?[0];
    }

    private static Task SetFieldsAsync(IPage page, int index, string[][] fields)
    {
        return page.EvaluateAsync(
            """
            ([index, fields]) => {
                const form = document.querySelectorAll('form')[index];
                fields.forEach(([name, value]) => { form.elements[name].value = value; });
            }
            """,
            new object[] { index, fields });
    }

    /// <summary>
    /// Submits the form with the button of the action, returns the error (null if the form was saved).
    /// </summary>
    private static async Task<string> SubmitAsync(IPage page, int index, string action)
    {
        try
        {
            var response = await page.RunAndWaitForNavigationAsync(
                               () => page.EvaluateAsync(
                                   """
                                   ([index, action]) => {
                                       const form = document.querySelectorAll('form')[index];
                                       const button = [...form.querySelectorAll('button, input[type=submit]')]
                                           .find(b => (b.getAttribute('formaction') || form.getAttribute('action') || '') === action);
                                       form.requestSubmit(button);
                                   }
                                   """,
                                   new object[] { index, action }),
                               new PageRunAndWaitForNavigationOptions { Timeout = 15000 });

            if (response is { Status: >= 500 })
            {
                return $"status {response.Status}";
            }

            return await page.Locator("h1", new PageLocatorOptions { HasText = "Forum Error" }).CountAsync() == 0
                       ? null
                       : $"forum error {page.Url}";
        }
        catch (TimeoutException)
        {
            // no navigation (e.g. a field is invalid in the browser)
            var invalid = await page.EvaluateAsync<string[]>(
                              "index => [...document.querySelectorAll('form')[index].elements].filter(e => e.willValidate && !e.checkValidity()).map(e => e.name + '=' + e.value + ' (' + e.validationMessage + ')')",
                              index);

            return $"no navigation, invalid fields: {string.Join(", ", invalid)}";
        }
    }

    /// <summary>
    /// The values of a form (name = value, several values of a name joined).
    /// </summary>
    private static async Task<Dictionary<string, string>> ReadFormAsync(IPage page, int index)
    {
        var entries = await page.EvaluateAsync<string[][]>(
                          """
                          index => {
                              const form = document.querySelectorAll('form')[index];
                              return [...new FormData(form).entries()]
                                  .filter(([, v]) => typeof v === 'string')
                                  .map(([k, v]) => [k, v]);
                          }
                          """,
                          index);

        return entries.GroupBy(e => e[0]).ToDictionary(g => g.Key, g => string.Join(",", g.Select(e => e[1])));
    }

    /// <summary>
    /// The admin and profile pages (links of the admin menu and the edit pages they link).
    /// </summary>
    private async Task<List<string>> CollectPagesAsync(IPage page)
    {
        var baseUri = new Uri(this.TestSettings.TestForumUrl);
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue = new Queue<string>();

        queue.Enqueue($"{this.TestSettings.TestForumUrl}Admin/Admin");
        queue.Enqueue($"{this.TestSettings.TestForumUrl}Profile/EditProfile");
        queue.Enqueue($"{this.TestSettings.TestForumUrl}Profile/EditSettings");
        queue.Enqueue($"{this.TestSettings.TestForumUrl}Profile/BlockOptions");
        queue.Enqueue($"{this.TestSettings.TestForumUrl}Profile/EditSignature");
        queue.Enqueue($"{this.TestSettings.TestForumUrl}Profile/Subscriptions");

        while (queue.Count > 0 && visited.Count < 150)
        {
            var url = queue.Dequeue();

            if (!visited.Add(url))
            {
                continue;
            }

            await page.GotoAsync(url);

            var links = await page.Locator("a[href]").EvaluateAllAsync<string[]>("links => links.map(l => l.href)");

            foreach (var link in links)
            {
                if (!Uri.TryCreate(link, UriKind.Absolute, out var uri) || uri.Host != baseUri.Host || uri.Port != baseUri.Port)
                {
                    continue;
                }

                var target = uri.GetLeftPart(UriPartial.Query);

                // the admin pages, without the pages of the host that change the whole application
                // and the copy of a forum (it creates a new forum)
                if (!target.Contains("/Admin/", StringComparison.OrdinalIgnoreCase)
                    || Regex.IsMatch(target, "handler=|RestartApp|RunSql|ReIndex|TestData|Install|Logout|EditForum/true", RegexOptions.IgnoreCase))
                {
                    continue;
                }

                if (!visited.Contains(target))
                {
                    queue.Enqueue(target);
                }
            }

            // the access masks are opened with a button (a post that redirects to the edit page)
            var maskIds = await page.Locator("button[formaction*='/Edit?maskId=']").EvaluateAllAsync<string[]>(
                              "buttons => buttons.map(b => new URL(b.formAction).searchParams.get('maskId'))");

            foreach (var maskId in maskIds.Distinct())
            {
                var target = $"{this.TestSettings.TestForumUrl}Admin/EditAccessMask/{maskId}";

                if (!visited.Contains(target))
                {
                    queue.Enqueue(target);
                }
            }
        }

        // the tabs of the user editor are pages of their own (loaded by a script)
        var userIds = visited.Select(url => Regex.Match(url, @"Admin/EditUser/(\d+)", RegexOptions.IgnoreCase))
            .Where(m => m.Success).Select(m => m.Groups[1].Value).Distinct().Take(3).ToList();

        foreach (var userId in userIds)
        {
            foreach (var tab in new[] { "UsersInfo", "UsersGroups", "UsersProfile", "UsersSettings", "UsersSignature" })
            {
                visited.Add($"{this.TestSettings.TestForumUrl}Admin/EditUser/{tab}?userId={userId}");
            }
        }

        return [.. visited];
    }
}
