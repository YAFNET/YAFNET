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

namespace YAF.Tests.Services;

using YAF.Tests.TestInfrastructure;

/// <summary>
/// DateTimeHelper.ToRelativeTime Tests (uses ILocalization through BoardContext)
/// </summary>
[TestFixture]
[NonParallelizable]
public class RelativeTimeTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();

        // the english.json texts
        this.board.Localization
            .Add("RELATIVE_TIME", "NOW", "Right now")
            .Add("RELATIVE_TIME", "PAST", "{0} ago")
            .Add("RELATIVE_TIME", "FUTURE", "in {0}")
            .Add("RELATIVE_TIME", "S", "a few seconds")
            .Add("RELATIVE_TIME", "SS", "{0} seconds")
            .Add("RELATIVE_TIME", "M", "a minute")
            .Add("RELATIVE_TIME", "MM", "{0} minutes")
            .Add("RELATIVE_TIME", "H", "an hour")
            .Add("RELATIVE_TIME", "HH", "{0} hours")
            .Add("RELATIVE_TIME", "D", "a day")
            .Add("RELATIVE_TIME", "DD", "{0} days")
            .Add("RELATIVE_TIME", "MO", "a month")
            .Add("RELATIVE_TIME", "MOMO", "{0} months")
            .Add("RELATIVE_TIME", "Y", "a year")
            .Add("RELATIVE_TIME", "YY", "{0} years");

        // ToRelativeTime formats PAST/SS/... without a page, so it must not depend on TransPage
        this.board.Localization.TransPage = "POSTS";
    }

    [TearDown]
    public void TearDown()
    {
        this.board.Dispose();
    }

    [TestCase(30, "30 seconds ago")]
    [TestCase(90, "a minute ago")]
    [TestCase(10 * 60, "10 minutes ago")]
    [TestCase(60 * 60, "an hour ago")]
    [TestCase(5 * 60 * 60, "5 hours ago")]
    [TestCase(30 * 60 * 60, "a day ago")]
    [TestCase(10 * 24 * 60 * 60, "10 days ago")]
    [TestCase(35 * 24 * 60 * 60, "a month ago")]
    [TestCase(100 * 24 * 60 * 60, "3 months ago")]
    [TestCase(400 * 24 * 60 * 60, "a year ago")]
    [TestCase(1000 * 24 * 60 * 60, "2 years ago")]
    [Description("Past dates are rendered as '... ago'")]
    public void ToRelativeTime_Past_Test(int secondsAgo, string expected)
    {
        // add a little slack so the boundary isn't crossed while the test runs
        DateTime.UtcNow.AddSeconds(-secondsAgo - 0.2).ToRelativeTime().Should().Be(expected);
    }

    [TestCase(10 * 60, "in 10 minutes")]
    [TestCase(5 * 60 * 60, "in 5 hours")]
    [TestCase(10 * 24 * 60 * 60, "in 10 days")]
    [Description("Future dates are rendered as 'in ...'")]
    public void ToRelativeTime_Future_Test(int secondsAhead, string expected)
    {
        DateTime.UtcNow.AddSeconds(secondsAhead + 0.5).ToRelativeTime().Should().Be(expected);
    }

    [Test]
    [Description("One second uses the singular text")]
    public void ToRelativeTime_One_Second_Test()
    {
        DateTime.UtcNow.AddSeconds(-1.2).ToRelativeTime().Should().Be("a few seconds ago");
    }
}
