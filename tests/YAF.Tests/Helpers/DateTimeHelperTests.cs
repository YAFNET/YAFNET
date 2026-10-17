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

namespace YAF.Tests.Helpers;

/// <summary>
/// YAF.Core.Helpers DateTimeHelper Tests (the parts that don't need a BoardContext)
/// </summary>
[TestFixture]
public class DateTimeHelperTests
{
    [Test]
    [Description("SqlDbMinTime is 1903-01-01, above the SQL Server datetime minimum")]
    public void SqlDbMinTime_Test()
    {
        DateTimeHelper.SqlDbMinTime().Should().Be(new DateTime(1903, 1, 1));
    }

    [Test]
    [Description("DateDiffDay counts calendar days and ignores the time of day")]
    public void DateDiffDay_Test()
    {
        DateTimeHelper.DateDiffDay(new DateTime(2026, 1, 1, 23, 59, 0), new DateTime(2026, 1, 2, 0, 1, 0))
            .Should().Be(1);
        DateTimeHelper.DateDiffDay(new DateTime(2026, 3, 1), new DateTime(2026, 2, 1)).Should().Be(-28);
        DateTimeHelper.DateDiffDay(new DateTime(2026, 1, 1, 8, 0, 0), new DateTime(2026, 1, 1, 20, 0, 0))
            .Should().Be(0);
    }

    [TestCase("UTC", 0)]
    [TestCase("India Standard Time", 330)]
    [TestCase("Nepal Standard Time", 345)]
    [TestCase("Hawaiian Standard Time", -600)]
    [Description("GetTimeZoneOffset returns the base offset in minutes, including half/quarter hour zones")]
    public void GetTimeZoneOffset_Test(string timeZoneId, int expectedMinutes)
    {
        DateTimeHelper.GetTimeZoneOffset(timeZoneId).Should().Be(expectedMinutes);
        DateTimeHelper.GetTimeZoneOffset(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)).Should().Be(expectedMinutes);
    }

    [Test]
    [Description("GetTimeZoneOffset falls back to the local zone for unknown ids")]
    public void GetTimeZoneOffset_Unknown_Test()
    {
        DateTimeHelper.GetTimeZoneOffset("Not/A_Zone").Should()
            .Be((int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes);
    }

    [TestCase("-60")]
    [TestCase("+120")]
    [TestCase("0")]
    [TestCase("Not/A_Zone")]
    [Description("GetTimeZoneInfo returns the local zone for legacy numeric offsets and unknown ids")]
    public void GetTimeZoneInfo_Local_Test(string input)
    {
        DateTimeHelper.GetTimeZoneInfo(input).Should().Be(TimeZoneInfo.Local);
    }

    [Test]
    [Description("GetTimeZoneInfo resolves known zone ids")]
    public void GetTimeZoneInfo_Known_Test()
    {
        DateTimeHelper.GetTimeZoneInfo("India Standard Time").BaseUtcOffset.Should().Be(TimeSpan.FromMinutes(330));
    }
}
