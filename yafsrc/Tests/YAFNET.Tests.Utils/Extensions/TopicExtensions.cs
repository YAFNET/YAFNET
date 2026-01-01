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
            await page.GotoAsync($"{testSettings.TestForumUrl}PostTopic/{testSettings.TestForumId}");

            var pageSource = await page.ContentAsync();

            if (!pageSource.Contains("Post New Topic"))
            {
                return false;
            }

            // Create New Topic
            await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Auto Created Test Topic - {DateTime.UtcNow}");

            await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
                .FillAsync("This is a Test Message Created by an automated Unit Test");

            // Post New Topic
            await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

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
            // Go to Post New Topic
            await page.GotoAsync(
                $"{testSettings.TestForumUrl}Posts/{testSettings.TestTopicId}/test");

            var pageSource = await page.ContentAsync();

            if (pageSource.Contains("You've passed an invalid value to the forum."))
            {
                return false;
            }

            await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

            pageSource = await page.ContentAsync();

            if (!pageSource.Contains("Post a reply"))
            {
                return false;
            }

            // Create New Reply
            await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(message);

            // Post New Reply
            await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

            pageSource = await page.ContentAsync();

            return pageSource.Contains(message);
        }
    }
}