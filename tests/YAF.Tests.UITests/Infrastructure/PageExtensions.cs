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

namespace YAF.Tests.UITests.Infrastructure;

using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

/// <summary>
/// Extensions for the Playwright page.
/// </summary>
public static class PageExtensions
{
    /// <summary>
    /// Runs the action and waits for the post back (the navigation) it starts.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="action">The action that starts the post back (e.g. a click or an auto post back control).</param>
    /// <returns>The task.</returns>
    /// <remarks>
    /// Playwright marks RunAndWaitForNavigationAsync as obsolete in favour of WaitForURLAsync, but a WebForms
    /// post back returns to the same address, so there is no address to wait for. The waiting is kept in this
    /// one place, so it can be changed for all tests at once.
    /// </remarks>
    public static async Task RunAndWaitForPostBackAsync(this IPage page, Func<Task> action)
    {
#pragma warning disable CS0612 // no replacement for a post back to the same address (see remarks)
        await page.RunAndWaitForNavigationAsync(action);
#pragma warning restore CS0612
    }
}
