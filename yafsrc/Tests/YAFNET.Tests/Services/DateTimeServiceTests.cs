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

using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;

/// <summary>
/// DateTimeService Tests
/// </summary>
[TestFixture]
[NonParallelizable]
public class DateTimeServiceTests
{
    private const string Berlin = "W. Europe Standard Time";

    // 12:00 UTC is 14:00 in Berlin (summer time)
    private readonly DateTime summerNoonUtc = new(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc);

    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();
        this.board.User.TimeZoneUser = Berlin;

        this.board.Localization
            .Add("DEFAULT", "FORMAT_DATE_LONG", "dd MMMM yyyy")
            .Add("DEFAULT", "FORMAT_DATE_SHORT", "yyyy-MM-dd")
            .Add("DEFAULT", "FORMAT_DATE_TIME_LONG", "yyyy-MM-dd HH:mm:ss")
            .Add("DEFAULT", "FORMAT_DATE_TIME_SHORT", "yyyy-MM-dd HH:mm")
            .Add("DEFAULT", "FORMAT_TIME", "HH:mm")
            .Add("DEFAULT", "TodayAt", "Today at {0:HH:mm}")
            .Add("DEFAULT", "YesterdayAt", "Yesterday at {0:HH:mm}")
            .Add("TIMEZONES", "NAME_UTC", "(UTC{0})");
    }

    [TearDown]
    public void TearDown()
    {
        this.board.Dispose();
    }

    [Test]
    [Description("The format methods convert to the user time zone and use the localized format")]
    public void Format_Methods_Test()
    {
        var service = this.CreateService();

        service.FormatDateLong(this.summerNoonUtc).Should().Be("01 July 2026");
        service.FormatDateShort(this.summerNoonUtc).Should().Be("2026-07-01");
        service.FormatDateTime(this.summerNoonUtc).Should().Be("2026-07-01 14:00:00");
        service.FormatDateTimeShort(this.summerNoonUtc).Should().Be("2026-07-01 14:00");
        service.FormatTime(this.summerNoonUtc).Should().Be("14:00");
    }

    [Test]
    [Description("A calendar date (a birthday) is the same day in every time zone, also for guests (no time zone appended)")]
    public void Format_Calendar_Date_Test()
    {
        var birthday = new DateTime(1990, 5, 10);

        foreach (var timeZone in new[] { Berlin, "UTC", "Hawaiian Standard Time", "Tokyo Standard Time" })
        {
            this.board.User.TimeZoneUser = timeZone;

            this.CreateService().FormatCalendarDate(birthday).Should().Be("10 May 1990", timeZone);
        }

        this.board.User.IsGuest = true;
        this.CreateService().FormatCalendarDate(birthday).Should().Be("10 May 1990");
    }

    [Test]
    [Description("Guests get the offset of their time zone at that date appended, registered users not")]
    public void Format_Guest_Appends_TimeZone_Test()
    {
        this.board.User.IsGuest = true;
        var service = this.CreateService();

        // Berlin: summer time +2, winter time +1
        service.FormatTime(this.summerNoonUtc).Should().Be("14:00 (UTC+02:00)");
        service.FormatTime(new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc)).Should().Be("13:00 (UTC+01:00)");
        service.FormatDateShort(this.summerNoonUtc).Should().Be("2026-07-01 (UTC+02:00)");

        this.board.User.TimeZoneUser = "Newfoundland Standard Time";
        service.FormatTime(new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc)).Should().Be("08:30 (UTC-03:30)");

        this.board.User.TimeZoneUser = "UTC";
        service.FormatTime(this.summerNoonUtc).Should().Be("12:00 (UTC)");

        this.board.User.IsGuest = false;
        service.FormatTime(this.summerNoonUtc).Should().Be("12:00");
    }

    [Test]
    [Description("Invalid localized formats fall back to the standard .NET format")]
    public void Format_Invalid_Format_Falls_Back_Test()
    {
        this.board.Localization.Add("DEFAULT", "FORMAT_TIME", "%");

        var berlinTime = TimeZoneInfo.ConvertTimeFromUtc(this.summerNoonUtc, TimeZoneInfo.FindSystemTimeZoneById(Berlin));

        this.CreateService().FormatTime(this.summerNoonUtc).Should().Be(berlinTime.ToString("T"));
    }

    [Test]
    [Description("FormatDateTimeTopic uses 'today' and 'yesterday' texts for recent dates")]
    public void FormatDateTimeTopic_Test()
    {
        this.board.User.TimeZoneUser = "UTC";

        var service = this.CreateService();
        var now = DateTime.UtcNow;

        // keep away from midnight so 'today' doesn't turn into 'yesterday' while the test runs
        var today = now.Date.AddHours(now.Hour < 12 ? 0 : 12).AddMinutes(1);
        var yesterday = today.AddDays(-1);
        var older = today.AddDays(-10);

        service.FormatDateTimeTopic(today).Should().Be($"Today at {today:HH:mm}");
        service.FormatDateTimeTopic(yesterday).Should().Be($"Yesterday at {yesterday:HH:mm}");
        service.FormatDateTimeTopic(older).Should().Be(older.ToString("yyyy-MM-dd HH:mm"));
    }

    [Test]
    [Description("The Farsi calendar setting switches to Persian dates")]
    public void Format_Farsi_Calendar_Test()
    {
        this.board.BoardSettings.UseFarsiCalender = true;

        // 1 July 2026 is 10 Tir 1405
        this.CreateService().FormatDateShort(this.summerNoonUtc).Should().Contain("1405");
    }

    [Test]
    [Description("TimeOffset combines the user offset and the server time correction")]
    public void TimeOffset_Test()
    {
        this.board.BoardSettings.ServerTimeCorrection = 5;

        // base offset of Berlin is +60 minutes
        this.CreateService().TimeOffset.Should().Be(new TimeSpan(1, 5, 0));
    }

    [Test]
    [Description("GetUserDateTime converts from UTC to the user time zone")]
    public void GetUserDateTime_Test()
    {
        var service = this.CreateService();

        service.GetUserDateTime(this.summerNoonUtc).Should().Be(new DateTime(2026, 7, 1, 14, 0, 0));
        service.GetUserDateTime(this.summerNoonUtc, TimeZoneInfo.Utc).Should().Be(this.summerNoonUtc);
    }

    [Test]
    [Description("Every format method falls back to the standard .NET format when the localized one is invalid")]
    public void All_Formats_Fall_Back_Test()
    {
        foreach (var tag in new[] { "FORMAT_DATE_LONG", "FORMAT_DATE_SHORT", "FORMAT_DATE_TIME_LONG", "FORMAT_DATE_TIME_SHORT" })
        {
            this.board.Localization.Add("DEFAULT", tag, "%");
        }

        this.board.Localization.Add("DEFAULT", "TodayAt", "{0:%}").Add("DEFAULT", "YesterdayAt", "{0:%}");

        var berlin = TimeZoneInfo.ConvertTimeFromUtc(this.summerNoonUtc, TimeZoneInfo.FindSystemTimeZoneById(Berlin));
        var service = this.CreateService();

        service.FormatDateLong(this.summerNoonUtc).Should().Be(berlin.ToString("D"));
        service.FormatDateShort(this.summerNoonUtc).Should().Be(berlin.ToString("d"));
        service.FormatDateTime(this.summerNoonUtc).Should().Be(berlin.ToString("F"));
        service.FormatDateTimeShort(this.summerNoonUtc).Should().Be(berlin.ToString("G"));
        service.FormatDateTimeTopic(this.summerNoonUtc).Should().Be(berlin.ToString("G"));
    }

    [Test]
    [Description("All format methods use the Persian calendar when the Farsi calendar is enabled")]
    public void All_Formats_Farsi_Calendar_Test()
    {
        this.board.BoardSettings.UseFarsiCalender = true;
        var service = this.CreateService();

        // 1 July 2026 is 10 Tir 1405
        service.FormatDateLong(this.summerNoonUtc).Should().Contain("1405");
        service.FormatDateTime(this.summerNoonUtc).Should().Contain("1405");
        service.FormatDateTimeShort(this.summerNoonUtc).Should().Contain("1405");
        service.FormatDateTimeTopic(this.summerNoonUtc).Should().Contain("1405");
        service.FormatDateLong(this.summerNoonUtc).Should().NotContain("2026");
    }

    [Test]
    [Description("Local dates are treated as UTC by the topic format instead of failing")]
    public void FormatDateTimeTopic_Local_Kind_Test()
    {
        this.board.User.TimeZoneUser = "UTC";
        var older = DateTime.SpecifyKind(new DateTime(2020, 3, 4, 5, 6, 0), DateTimeKind.Local);

        this.CreateService().FormatDateTimeTopic(older).Should().Be("2020-03-04 05:06");
    }

    [Test]
    [Description("A negative user offset with minutes is split into hours and minutes")]
    public void TimeOffset_Negative_Test()
    {
        // Newfoundland: -3:30 in winter, -2:30 in summer
        this.board.User.TimeZoneUser = "Newfoundland Standard Time";

        var offset = this.CreateService().TimeOffset;

        offset.Minutes.Should().Be(-30);
        offset.Hours.Should().BeOneOf(-2, -3);
    }

    private DateTimeService CreateService()
    {
        return new DateTimeService(this.board.Resolve<IServiceLocator>());
    }
}
