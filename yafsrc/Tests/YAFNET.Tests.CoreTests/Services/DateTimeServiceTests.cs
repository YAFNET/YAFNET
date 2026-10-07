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

namespace YAF.Tests.CoreTests.Services;

using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
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
            .Add("TIMEZONES", "NAME_UTC", " (UTC)");
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
    [Description("Guests get the time zone name appended")]
    public void Format_Guest_Appends_TimeZone_Test()
    {
        this.board.User.IsGuest = true;

        this.CreateService().FormatTime(this.summerNoonUtc).Should().Be("14:00 (UTC)");
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

    private DateTimeService CreateService()
    {
        return new DateTimeService(this.board.Resolve<IServiceLocator>());
    }
}
