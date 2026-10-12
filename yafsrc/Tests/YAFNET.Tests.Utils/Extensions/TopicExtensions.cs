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

namespace YAF.Tests.Utils.Extensions;

using System;

using Microsoft.Playwright;

/// <summary>
/// Topic Extensions
/// </summary>
public static class TopicExtensions
{
    /// <param name="page">
    /// The Page Instance
    /// </param>
    extension(IPage page)
    {
        /// <summary>
        /// Creates a topic in a forum and returns its id (the page shows the topic afterwards).
        /// No retry after the flood delay, use a new user (e.g. <c>RegisterNewTestUserAsync</c>).
        /// </summary>
        /// <param name="testSettings">The test Settings</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns>Returns the topic id, or null if the topic was not created.</returns>
        public async Task<int?> CreateTopicAsync(TestConfig testSettings, int forumId, string subject, string message)
        {
            await page.GotoAsync($"{testSettings.TestForumUrl}PostTopic/{forumId}");

            await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync(subject);

            await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
            await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(message);

            await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

            var match = System.Text.RegularExpressions.Regex.Match(page.Url, @"/Posts/(\d+)/");

            return match.Success ? int.Parse(match.Groups[1].Value) : null;
        }

        /// <summary>
        /// Creates the new test topic.
        /// </summary>
        /// <param name="testSettings">
        /// The test Settings
        /// </param>
        /// <returns>
        /// Returns if Creating of the New Topic was
        /// successfully or not.
        /// </returns>
        public async Task<bool> CreateNewTestTopicAsync(TestConfig testSettings)
        {
            // members have to wait between two posts (flood control), the test before may just have posted
            for (var attempt = 0; attempt < 2; attempt++)
            {
                if (attempt > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(31));
                }

                if (await page.TryCreateNewTestTopicAsync(testSettings))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the new test topic once.
        /// </summary>
        /// <param name="testSettings">The test Settings</param>
        /// <returns>Returns if the topic was created.</returns>
        public async Task<bool> TryCreateNewTestTopicAsync(TestConfig testSettings)
        {
            await page.GotoAsync($"{testSettings.TestForumUrl}PostTopic/{testSettings.TestForumId}");

            var pageSource = await page.ContentAsync();

            if (!pageSource.Contains("Post New Topic"))
            {
                return false;
            }

            // Create New Topic
            await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Auto Created Test Topic - {DateTime.UtcNow}");

            await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
            await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
                .FillAsync("This is a Test Message Created by an automated Unit Test");

            // Post New Topic
            await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

            pageSource = await page.ContentAsync();

            return pageSource.Contains("Next Topic");
        }

        /// <summary>
        /// Creates a new reply in the test topic.
        /// </summary>
        /// <param name="testSettings">
        /// The test Settings
        /// </param>
        /// <param name="message">The Reply message.</param>
        /// <returns>
        /// Returns if Reply was Created or not
        /// </returns>
        public async Task<bool> CreateNewReplyInTestTopicAsync(TestConfig testSettings, string message)
        {
            // members have to wait between two posts (flood control), the test before may just have posted
            for (var attempt = 0; attempt < 2; attempt++)
            {
                if (attempt > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(31));
                }

                if (await page.TryCreateNewReplyInTestTopicAsync(testSettings, message))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a new reply in the test topic once.
        /// </summary>
        /// <param name="testSettings">The test Settings</param>
        /// <param name="message">The Reply message.</param>
        /// <returns>Returns if the reply was created.</returns>
        public async Task<bool> TryCreateNewReplyInTestTopicAsync(TestConfig testSettings, string message)
        {
            // Go to Post New Topic
            await page.GotoAsync(
                $"{testSettings.TestForumUrl}Posts/{testSettings.TestTopicId}/test");

            var pageSource = await page.ContentAsync();

            if (pageSource.Contains("You've passed an invalid value to the forum."))
            {
                return false;
            }

            await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'ReplyLink')]").First);

            pageSource = await page.ContentAsync();

            if (!pageSource.Contains("Post a reply"))
            {
                return false;
            }

            // Create New Reply
            await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
            await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(message);

            // Post New Reply
            await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

            pageSource = await page.ContentAsync();

            // bb codes are rendered (e.g. hidden content), the text before the first one is shown as written
            var visibleText = message.Split('[')[0].Trim();

            return pageSource.Contains(visibleText.Length > 0 ? visibleText : message);
        }
    }
}