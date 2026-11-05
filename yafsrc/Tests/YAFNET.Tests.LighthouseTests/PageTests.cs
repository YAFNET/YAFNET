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

using LighthousePlaywright.Net;
using LighthousePlaywright.Net.Objects;

namespace YAF.Tests.LighthouseTests;

/// <summary>
/// Runs a lighthouse audit on all forum pages a guest can open.
/// </summary>
/// <remarks>
/// The audits run one after another (not in parallel), because parallel audits slow each other down
/// and that would distort the performance score.
/// </remarks>
[TestFixture]
public class PageTests : Setup
{
    /// <summary>
    /// The minimum score (0 - 1) of the accessibility, best practices and seo category.
    /// </summary>
    private const decimal MinScore = 0.9m;

    /// <summary>
    /// The minimum performance score (0 - 1), lighthouse simulates a slow mobile device and network.
    /// </summary>
    private const decimal MinPerformanceScore = 0.9m;

    /// <summary>
    /// Gets the pages to audit, as relative url, the placeholders {forumId} and {topicId}
    /// are replaced with the test forum and topic of the test settings.
    /// </summary>
    /// <remarks>
    /// Only pages a guest can open, the audit runs in a new browser without login. Pages like the
    /// members list, active users or search redirect a guest (depending on the board permissions)
    /// to the login page.
    /// The second argument is <c>true</c> for pages that can be indexed, all others have a
    /// <c>noindex</c> robots meta tag on purpose (see CanonicalMetaTag) which fails the SEO category.
    /// The topic is opened with a page number, without it a (non crawler) visitor is redirected to the first unread post.
    /// </remarks>
    public static IEnumerable<TestCaseData> Pages()
    {
        yield return new TestCaseData("", true).SetName("Lighthouse_Index");
        yield return new TestCaseData("forum/{forumId}/test", true).SetName("Lighthouse_Topics");
        yield return new TestCaseData("topic/{topicId}/test/1", true).SetName("Lighthouse_Posts");
        yield return new TestCaseData("Help", false).SetName("Lighthouse_Help");
        yield return new TestCaseData("Privacy", false).SetName("Lighthouse_Privacy");
        yield return new TestCaseData("Cookies", false).SetName("Lighthouse_Cookies");
        yield return new TestCaseData("Account/Login", false).SetName("Lighthouse_Login");
        yield return new TestCaseData("Account/Register", false).SetName("Lighthouse_Register");
        yield return new TestCaseData("Account/ForgotPassword", false).SetName("Lighthouse_ForgotPassword");
    }

    /// <summary>
    /// Audits the page and checks the scores of all categories.
    /// </summary>
    /// <param name="path">The relative url of the page.</param>
    /// <param name="indexable">If the page can be indexed by search engines, only then the seo score is checked.</param>
    [TestCaseSource(nameof(Pages))]
    public async Task PagePerformanceTest(string path, bool indexable)
    {
        path = path.Replace("{forumId}", this.TestSettings.TestForumId.ToString())
            .Replace("{topicId}", this.TestSettings.TestTopicId.ToString());

        var reportName = $"lighthouse-{(path.Length == 0 ? "Index" : path.Replace('/', '-').Replace('?', '-').Replace('=', '-'))}";

        var lh = new Lighthouse(new Options
        {
            Reports = new Reports
            {
                Name = reportName,
                Formats = new Formats
                {
                    Html = true
                }
            }
        });

        var res = await lh.RunAsync($"{this.TestSettings.TestForumUrl}{path}");

        await TestContext.Progress.WriteLineAsync(
            $"{reportName}: Performance {res.Performance}, Accessibility {res.Accessibility}, Best Practices {res.BestPractices}, SEO {res.Seo}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(res.Accessibility, Is.GreaterThanOrEqualTo(MinScore), $"Accessibility of /{path}");

            Assert.That(res.BestPractices, Is.GreaterThanOrEqualTo(MinScore), $"Best Practices of /{path}");

            if (indexable)
            {
                Assert.That(res.Seo, Is.GreaterThanOrEqualTo(MinScore), $"SEO of /{path}");
            }

            Assert.That(res.Performance, Is.GreaterThanOrEqualTo(MinPerformanceScore), $"Performance of /{path}");
        }
    }
}
