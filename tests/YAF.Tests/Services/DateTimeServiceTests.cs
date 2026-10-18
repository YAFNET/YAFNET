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

namespace YAF.Tests.Services;

using System;
using System.Globalization;

using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;

/// <summary>
/// The <see cref="DateTimeService"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DateTimeServiceTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();

        this.board.Localization.Add("DEFAULT", "FORMAT_DATE_SHORT", "yyyy-MM-dd");
        this.board.Localization.Add("DEFAULT", "FORMAT_DATE_LONG", "dd MMMM yyyy");
        this.board.Localization.Add("DEFAULT", "FORMAT_DATE_TIME_SHORT", "dd.MM.yyyy HH:mm");
        this.board.Localization.Add("DEFAULT", "FORMAT_TIME", "HH:mm");
        this.board.Localization.Add("DEFAULT", "TodayAt", "Today at {0:HH:mm}");
        this.board.Localization.Add("DEFAULT", "YesterdayAt", "Yesterday at {0:HH:mm}");
        this.board.Localization.Add("TIMEZONES", "NAME_UTC", "(UTC{0})");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Guests see the offset of the time zone of the date (summer and winter time), members only the date")]
    public void Guest_Time_Zone_Suffix_Test()
    {
        var service = new DateTimeService(this.board.Current.ServiceLocator);

        this.board.User.IsGuest = true;
        this.board.User.TimeZoneUser = "UTC";

        service.FormatDateShort(new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc)).Should().Be("2026-01-15 (UTC)");

        this.board.User.TimeZoneUser = "W. Europe Standard Time";

        service.FormatDateShort(new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc)).Should()
            .Be("2026-01-15 (UTC+01:00)");
        service.FormatDateShort(new DateTime(2026, 7, 15, 12, 0, 0, DateTimeKind.Utc)).Should()
            .Be("2026-07-15 (UTC+02:00)");

        this.board.User.IsGuest = false;

        service.FormatDateShort(new DateTime(2026, 7, 15, 12, 0, 0, DateTimeKind.Utc)).Should().Be("2026-07-15");
    }

    [Test]
    [Description("The dates of members are formatted with the formats of the language file")]
    public void Formats_Of_The_Language_Test()
    {
        var service = new DateTimeService(this.board.Current.ServiceLocator);
        var date = new DateTime(2003, 2, 7, 13, 45, 0, DateTimeKind.Utc);

        service.FormatDateLong(date).Should().Be(date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture));
        service.FormatDateTimeShort(date).Should().Be("07.02.2003 13:45");
        service.FormatTime(date).Should().Be("13:45");
    }

    [Test]
    [Description("A broken format of the language file falls back to the standard format of the culture")]
    public void Broken_Format_Test()
    {
        foreach (var tag in new[] { "FORMAT_DATE_LONG", "FORMAT_DATE_SHORT", "FORMAT_DATE_TIME_LONG", "FORMAT_DATE_TIME_SHORT", "FORMAT_TIME" })
        {
            this.board.Localization.Add("DEFAULT", tag, "%");
        }

        var service = new DateTimeService(this.board.Current.ServiceLocator);
        var date = new DateTime(2003, 2, 7, 13, 45, 0, DateTimeKind.Utc);

        service.FormatDateLong(date).Should().Be(date.ToString("D"));
        service.FormatDateShort(date).Should().Be(date.ToString("d"));
        service.FormatDateTime(date).Should().Be(date.ToString("F"));
        service.FormatDateTimeShort(date).Should().Be(date.ToString("G"));
        service.FormatTime(date).Should().Be(date.ToString("T"));
    }

    [Test]
    [Description("The topic lists show today and yesterday, older dates with the short date and time format")]
    public void Topic_Date_Test()
    {
        var service = new DateTimeService(this.board.Current.ServiceLocator);
        var now = DateTime.UtcNow;

        service.FormatDateTimeTopic(now).Should().StartWith("Today at");
        service.FormatDateTimeTopic(now.AddDays(-1)).Should().StartWith("Yesterday at");

        var older = now.AddDays(-5);
        service.FormatDateTimeTopic(older).Should().Be(older.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture));

        // a local date is taken as it is
        service.FormatDateTimeTopic(DateTime.SpecifyKind(older, DateTimeKind.Local)).Should()
            .Be(older.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture));
    }
}
