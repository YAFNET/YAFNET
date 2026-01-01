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

using LighthousePlaywright.Net;
using LighthousePlaywright.Net.Objects;

namespace YAF.Tests.LighthouseTests;

/// <summary>
/// The pages test
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PageTests : Setup
{
    /// <summary>
    /// Basic test to check if all admin pages load without error
    /// </summary>
    [Test]
    public async Task PagePerformanceTest()
    {
        var lh = new Lighthouse(new Options
        {
            Reports = new Reports
            {
                Formats = new Formats
                {
                    Html = true
                }
            }
        });

        var res = await lh.RunAsync($"{this.TestSettings.TestForumUrl}");

        Assert.That(res.Accessibility > 0.9m, Is.True, $"Value was: {res.Accessibility}");

        Assert.That(res.BestPractices > 0.9m, Is.True, $"Value was: {res.BestPractices}");

        Assert.That(res.Seo > 0.9m, Is.True, $"Value was: {res.Seo}");

        Assert.That(res.Performance >= 0.6m, Is.True, $"Value was: {res.Performance}");
    }
}