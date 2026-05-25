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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Playwright;

public static class IBrowserContextExtensions
{
    extension(IBrowserContext context)
    {
        /// <summary>
        /// Open a Browser page and navigate to the given URL before
        /// applying the given test handler.
        /// </summary>
        /// <param name="url">URL to navigate to.</param>
        /// <param name="testHandler">Test handler to apply on the page.
        /// </param>
        /// <returns>The GotoPage task.</returns>
        public async Task GotoPageAsync([StringSyntax(StringSyntaxAttribute.Uri)] string url, Func<IPage, Task> testHandler)
        {
            var page = await context.NewPageAsync().ConfigureAwait(false);

            Assert.That(page, Is.Not.Null);

            // Navigate to the given URL and wait until loading
            // network activity is done.
            var gotoResult = await page.GotoAsync(url);

            Assert.That(gotoResult, Is.Not.Null);

            await gotoResult.FinishedAsync();

            Assert.That(gotoResult.Ok, Is.True);

            var closeButton = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close" })
                .Filter(new LocatorFilterOptions { HasText = "Close" });

            if (await closeButton.IsVisibleAsync())
            {
                await closeButton.ClickAsync();
            }

            // Run the actual test logic.
            await testHandler(page);
        }
    }
}