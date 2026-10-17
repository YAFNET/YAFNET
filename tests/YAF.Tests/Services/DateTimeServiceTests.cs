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
}
