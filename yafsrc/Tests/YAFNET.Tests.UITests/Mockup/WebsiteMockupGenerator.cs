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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.Mockup;

/// <summary>
/// Creates the multi device mockup image of the website (monitor, laptop, tablet and phone).
/// Installs a new board in docker, fills it with the demo community of <see cref="MockupContent"/>,
/// takes a screenshot per device and puts them into the device frames of mockup.html.
/// <para>
/// Run with <c>dotnet test --filter "Category=Mockup"</c>. The image is written to the file of the
/// YAF_MOCKUP_OUTPUT environment variable (e.g. the Img1.png of the website), without it to
/// <c>mockup-output/Img1.png</c> in the test output folder. The single screenshots are kept next to the image.
/// The scenario (database) is Sqlite, set YAF_MOCKUP_SCENARIO to use another one (e.g. SqlServer).
/// Uses the same port as the ui tests, so it can not run at the same time.
/// </para>
/// </summary>
[TestFixture]
[Explicit("Creates the website mockup image, run with --filter Category=Mockup")]
[Category("Mockup")]
public class WebsiteMockupGenerator
{
    /// <summary>
    /// The password of all demo users.
    /// </summary>
    private const string Password = "Mockup1234!Demo";

    private readonly Dictionary<string, IPage> userPages = new(StringComparer.OrdinalIgnoreCase);

    private DatabaseFixture fixture;

    private TestConfig settings;

    private string workDirectory;

    [Test]
    public async Task CreateMockupImageAsync()
    {
        this.settings = LoadSettings();

        // not "mockup", that is the folder of the template (the same folder on windows)
        this.workDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "mockup-output");
        Directory.CreateDirectory(this.workDirectory);

        this.fixture = CreateFixture();

        try
        {
            // installs the board and registers the first two users
            await this.fixture.InitializeAsync(this.settings);

            await this.CreateUsersAsync();
            await this.CreateForumsAsync();
            var showcaseTopic = await this.CreateTopicsAsync();

            await this.TakeScreenshotsAsync(showcaseTopic);

            var output = await this.ComposeImageAsync();

            await TestContext.Progress.WriteLineAsync($"🖼️ Mockup image created: {output}");
        }
        finally
        {
            foreach (var page in this.userPages.Values)
            {
                await page.Context.DisposeAsync();
            }

            await this.fixture.DisposeAsync();
        }
    }

    /// <summary>
    /// The settings of the ui tests, with the board and the users of the demo community.
    /// Always a new board in docker, the demo content is not created in a local forum.
    /// </summary>
    private static TestConfig LoadSettings()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath($"{Directory.GetCurrentDirectory()}..//..//..//.//..//")
            .AddJsonFile("appSettings.json", false, false)
            .AddEnvironmentVariables()
            .Build();

        var settings = configuration.GetSection("TestSettings").Get<TestConfig>();

        settings.UseDocker = true;
        settings.NewInstall = true;
        settings.RecordVideos = false;

        settings.TestApplicationName = MockupContent.BoardName;
        settings.AdminUserName = MockupContent.Admin;
        settings.AdminEmail = $"{MockupContent.Admin.ToLower()}@example.com";
        settings.AdminPassword = Password;

        // registered by the fixture after the install
        settings.TestUserName = MockupContent.Members[0];
        settings.TestUserPassword = Password;
        settings.TestUserName2 = MockupContent.Members[1];
        settings.TestUser2Password = Password;

        return settings;
    }

    private static DatabaseFixture CreateFixture()
    {
        var scenario = Environment.GetEnvironmentVariable("YAF_MOCKUP_SCENARIO") ?? "Sqlite";

        return scenario.ToLowerInvariant() switch
        {
            "sqlserver" => new SqlServerFixture(),
            "mysql" => new MySqlFixture(),
            "postgresql" => new PostgreSQLFixture(),
            _ => new SqliteFixture()
        };
    }

    /// <summary>
    /// Registers the other members and logs in every user in an own browser context,
    /// so all of them are shown as active users.
    /// </summary>
    private async Task CreateUsersAsync()
    {
        foreach (var member in MockupContent.Members.Skip(2))
        {
            Assert.That(
                await this.fixture.Page.RegisterUserAsync(this.settings, this.fixture.SmtpServer, member, Password),
                Is.True,
                $"Registering {member} failed");
        }

        foreach (var user in MockupContent.Members.Prepend(MockupContent.Admin))
        {
            var context = await this.fixture.Context.Browser!.NewContextAsync(
                              new BrowserNewContextOptions
                              {
                                  BaseURL = this.settings.TestForumUrl,
                                  ViewportSize = new ViewportSize { Width = 1440, Height = 900 }
                              });

            // the cookie banner covers the buttons (and the screenshots)
            await context.AddCookiesAsync(
                [new Cookie { Name = "YAF-AcceptCookies", Value = "true", Url = this.settings.TestForumUrl }]);

            var page = await context.NewPageAsync();

            Assert.That(await page.LoginUserAsync(this.settings, user, Password), Is.True, $"Login of {user} failed");

            this.userPages[user] = page;
        }

        await TestContext.Progress.WriteLineAsync("👥 users created");
    }

    /// <summary>
    /// Renames the test category and forum of the install and creates the other categories and forums.
    /// </summary>
    private async Task CreateForumsAsync()
    {
        var page = this.userPages[MockupContent.Admin];

        // the members are not flood controlled while the content is created
        await page.GotoAsync($"{this.settings.TestForumUrl}Admin/HostSettings");
        await page.EvaluateAsync("() => document.querySelector('#Input_PostFloodDelay').value = '0'");
        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Save')]").First);

        // all categories first, the list of the admin page is paged (the categories are not found after the forums)
        var categoryIds = new List<int>();

        for (var i = 0; i < MockupContent.Categories.Length; i++)
        {
            var name = MockupContent.Categories[i].Name;

            if (i == 0)
            {
                // the category of the install
                await this.SaveCategoryAsync(page, 1, name, 1);
                categoryIds.Add(1);
            }
            else
            {
                await this.SaveCategoryAsync(page, 0, name, i + 1);
                categoryIds.Add(await this.CategoryIdAsync(page, name));
            }
        }

        for (var i = 0; i < MockupContent.Categories.Length; i++)
        {
            var forumSortOrder = 1;

            foreach (var forum in MockupContent.Categories[i].Forums)
            {
                // the first forum is the forum of the install
                await this.SaveForumAsync(
                    page,
                    i == 0 && forumSortOrder == 1 ? 1 : null,
                    categoryIds[i],
                    forum,
                    forumSortOrder++);
            }
        }

        await TestContext.Progress.WriteLineAsync("🗂️ forums created");
    }

    private async Task SaveCategoryAsync(IPage page, int categoryId, string name, int sortOrder)
    {
        await page.GotoAsync($"{this.settings.TestForumUrl}Admin/EditCategory/{categoryId}");
        await page.Locator("#Input_Name").FillAsync(name);
        await page.Locator("#Input_SortOrder").FillAsync(sortOrder.ToString());
        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
    }

    private async Task<int> CategoryIdAsync(IPage page, string name)
    {
        await page.GotoAsync($"{this.settings.TestForumUrl}Admin/Forums");

        var categoryId = await page.EvaluateAsync<string>(
                             """
                             name => {
                                 const item = [...document.querySelectorAll('li.list-group-item')]
                                     .find(li => li.querySelector('h5')?.textContent.trim() === name);
                                 const href = item?.querySelector("a[href*='EditCategory/']")?.getAttribute('href');
                                 return href ? href.match(/EditCategory\/(\d+)/)[1] : null;
                             }
                             """,
                             name);

        Assert.That(categoryId, Is.Not.Null, $"The category {name} was not created");

        return int.Parse(categoryId);
    }

    private async Task SaveForumAsync(IPage page, int? forumId, int categoryId, MockupForum forum, int sortOrder)
    {
        await page.GotoAsync(
            $"{this.settings.TestForumUrl}Admin/EditForum/false{(forumId.HasValue ? $"/{forumId}" : string.Empty)}");

        await page.Locator("#Input_Name").FillAsync(forum.Name);
        await page.Locator("#Input_Description").FillAsync(forum.Description);
        await page.Locator("#Input_SortOrder").FillAsync(sortOrder.ToString());

        // the select is a Choices.js select (only the selected option is in the form)
        await page.EvaluateAsync(
            """
            id => {
                const select = document.querySelector('#Input_CategoryID');
                select.add(new Option('', id, true, true));
                select.value = id;
            }
            """,
            categoryId.ToString());

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

        Assert.That(page.Url, Does.Contain("Admin/Forums"), $"The forum {forum.Name} was not saved");
    }

    /// <summary>
    /// Creates the topics and replies, returns the id of the topic shown on the phone.
    /// </summary>
    private async Task<int> CreateTopicsAsync()
    {
        var forumIds = await this.ForumIdsAsync();
        var showcaseTopic = 0;

        foreach (var topic in MockupContent.Topics)
        {
            var topicId = await this.userPages[topic.Author].CreateTopicAsync(
                              this.settings,
                              forumIds[topic.Forum],
                              topic.Subject,
                              topic.Message);

            Assert.That(topicId, Is.Not.Null, $"The topic {topic.Subject} was not created");

            foreach (var (author, message) in topic.Replies)
            {
                await this.ReplyAsync(this.userPages[author], topicId!.Value, message);
            }

            if (topic.Showcase)
            {
                showcaseTopic = topicId!.Value;
            }
        }

        await TestContext.Progress.WriteLineAsync("💬 topics created");

        return showcaseTopic;
    }

    private async Task<Dictionary<string, int>> ForumIdsAsync()
    {
        var page = this.userPages[MockupContent.Admin];

        await page.GotoAsync(this.settings.TestForumUrl);

        var links = await page.EvaluateAsync<string[][]>(
                        """
                        () => [...document.querySelectorAll("a[href*='/forum/']")]
                            .map(a => [a.textContent.trim(), a.getAttribute('href')])
                        """);

        var forumIds = new Dictionary<string, int>();

        foreach (var link in links)
        {
            var match = Regex.Match(link[1], @"/forum/(\d+)/");

            if (match.Success)
            {
                forumIds.TryAdd(link[0], int.Parse(match.Groups[1].Value));
            }
        }

        foreach (var forum in MockupContent.Categories.SelectMany(c => c.Forums))
        {
            Assert.That(forumIds, Does.ContainKey(forum.Name), $"The forum {forum.Name} is not shown on the index");
        }

        return forumIds;
    }

    private async Task ReplyAsync(IPage page, int topicId, string message)
    {
        await page.GotoAsync($"{this.settings.TestForumUrl}topic/{topicId}/topic");

        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'ReplyLink')]").First);

        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(message);

        await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

        // a new post is shown with its id (not approved posts are not)
        Assert.That(page.Url, Does.Contain("/post/"), $"The reply in the topic {topicId} was not created");
    }

    /// <summary>
    /// Takes the screenshots of the devices as the admin.
    /// </summary>
    private async Task TakeScreenshotsAsync(int showcaseTopic)
    {
        var admin = this.userPages[MockupContent.Admin];

        // the statistics of the dashboard are cached since the install
        await admin.GotoAsync($"{this.settings.TestForumUrl}Admin/HostSettings");
        await admin.RunAndWaitForNextPageAsync(
            () => admin.Locator("button[formaction*='ResetCacheAll']").First
                .EvaluateAsync("b => b.form.requestSubmit(b)"));

        var storageState = await admin.Context.StorageStateAsync();

        // Monitor: the admin dashboard
        await this.ScreenshotAsync(
            storageState,
            "monitor.png",
            new ViewportSize { Width = 1600, Height = 900 },
            false,
            async page => await page.GotoAsync($"{this.settings.TestForumUrl}Admin/Admin"));

        // Laptop: the board index
        await this.ScreenshotAsync(
            storageState,
            "laptop.png",
            new ViewportSize { Width = 1440, Height = 900 },
            false,
            async page => await page.GotoAsync(this.settings.TestForumUrl));

        // Tablet: a new topic is written
        await this.ScreenshotAsync(
            storageState,
            "tablet.png",
            new ViewportSize { Width = 820, Height = 1180 },
            true,
            async page =>
            {
                var forumIds = await this.ForumIdsAsync();

                await page.GotoAsync($"{this.settings.TestForumUrl}PostTopic/{forumIds[MockupContent.Draft.Forum]}");
                await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync(MockupContent.Draft.Subject);
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
                    .FillAsync(MockupContent.Draft.Message);

                // no focus outline in the screenshot
                await page.EvaluateAsync("() => document.activeElement?.blur()");
            });

        // Phone: the showcase topic
        await this.ScreenshotAsync(
            storageState,
            "phone.png",
            new ViewportSize { Width = 390, Height = 844 },
            true,
            async page => await page.GotoAsync($"{this.settings.TestForumUrl}topic/{showcaseTopic}/topic"));

        await TestContext.Progress.WriteLineAsync("📸 screenshots taken");
    }

    private async Task ScreenshotAsync(
        string storageState,
        string fileName,
        ViewportSize viewport,
        bool mobile,
        Func<IPage, Task> show)
    {
        await using var context = await this.fixture.Context.Browser!.NewContextAsync(
                                      new BrowserNewContextOptions
                                      {
                                          BaseURL = this.settings.TestForumUrl,
                                          StorageState = storageState,
                                          ViewportSize = viewport,
                                          DeviceScaleFactor = mobile ? 2 : 1,
                                          IsMobile = mobile,
                                          HasTouch = mobile
                                      });

        var page = await context.NewPageAsync();

        await show(page);

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.EvaluateAsync("() => document.fonts.ready");

        // the topic jumps to the last post, the screenshot shows the top of the page
        await page.EvaluateAsync("() => window.scrollTo(0, 0)");
        await page.WaitForTimeoutAsync(500);

        // the numbers of the dashboard count up to their value
        await page.WaitForFunctionAsync(
            """
            () => [...document.querySelectorAll('.stats[data-count-target]')]
                .every(el => el.textContent.trim().replace(/\D/g, '') === el.dataset.countTarget.replace(/\D/g, ''))
            """);

        // no blinking caret, no scrollbar
        await page.AddStyleTagAsync(
            new PageAddStyleTagOptions
            {
                Content = "* { caret-color: transparent !important; } ::-webkit-scrollbar { display: none; }"
            });

        await page.ScreenshotAsync(
            new PageScreenshotOptions { Path = Path.Combine(this.workDirectory, fileName), Animations = ScreenshotAnimations.Disabled });
    }

    /// <summary>
    /// Puts the screenshots into the device frames and saves the image with a transparent background.
    /// </summary>
    /// <returns>The path of the image.</returns>
    private async Task<string> ComposeImageAsync()
    {
        var template = Path.Combine(TestContext.CurrentContext.TestDirectory, "Mockup", "mockup.html");
        var html = Path.Combine(this.workDirectory, "mockup.html");

        File.Copy(template, html, true);

        var output = Environment.GetEnvironmentVariable("YAF_MOCKUP_OUTPUT");

        if (string.IsNullOrWhiteSpace(output))
        {
            output = Path.Combine(this.workDirectory, "Img1.png");
        }

        await using var context = await this.fixture.Context.Browser!.NewContextAsync(
                                      new BrowserNewContextOptions
                                      {
                                          ViewportSize = new ViewportSize { Width = 2000, Height = 1160 },
                                          DeviceScaleFactor = 1
                                      });

        var page = await context.NewPageAsync();

        await page.GotoAsync(new Uri(html).AbsoluteUri);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await page.Locator("#stage").ScreenshotAsync(
            new LocatorScreenshotOptions { Path = output, OmitBackground = true });

        return output;
    }
}
