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

namespace YAF.Tests.Pages;

/// <summary>
/// Shared helpers for every wizard page.
/// </summary>
public abstract class BasePage(IPage page)
{
    private ILocator NextButton => this.Page.Locator(".btn-primary");

    protected IPage Page { get; } = page;

    /// <summary>Wait until the page URL matches the expected path segment.</summary>
    public async Task WaitForUrlContainsAsync(string pathSegment, int timeoutMs = 15_000)
    {
        await this.Page.WaitForURLAsync($"**{pathSegment}**",
            new PageWaitForURLOptions { Timeout = timeoutMs });
    }

    public async Task<string> GetHeadingAsync()
    {
        return await this.Page.Locator("h4").First.InnerTextAsync();
    }

    public async Task ClickNextAsync()
    {
        await this.NextButton.ClickAsync();
    }
}
