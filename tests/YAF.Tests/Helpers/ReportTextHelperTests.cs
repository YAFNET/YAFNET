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

namespace YAF.Tests.Helpers;

using YAF.Core.Helpers;

/// <summary>
/// The report text of a user and a post ("date??text" joined with "|").
/// </summary>
[TestFixture]
public class ReportTextHelperTests
{
    [Test]
    [Description("Every report is a date and a text")]
    public void Parse_Reports_Test()
    {
        var reports = ReportTextHelper.Parse("11/21/2026 08:15:00??first|11/22/2026 09:30:05??second");

        reports.Should().HaveCount(2);
        reports[0].Date.Should().Be(new DateTime(2026, 11, 21, 8, 15, 0, DateTimeKind.Utc));
        reports[0].Text.Should().Be("first");
        reports[1].Text.Should().Be("second");
    }

    [Test]
    [Description("A | in the text of a report is part of the text (the text was split into a report without a date)")]
    public void Parse_Text_With_Pipe_Test()
    {
        var reports = ReportTextHelper.Parse("11/21/2026 08:15:00??before | after|11/22/2026 09:30:05??a|b");

        reports.Select(r => r.Text).Should().Equal("before | after", "a|b");
        reports.Should().OnlyContain(r => r.Date.HasValue);
    }

    [Test]
    [Description("A text without a date is kept, an empty text has no reports")]
    public void Parse_Without_Date_Test()
    {
        ReportTextHelper.Parse("an old report").Should().ContainSingle().Which.Should().Be(((DateTime?)null, "an old report"));
        ReportTextHelper.Parse(null).Should().BeEmpty();
        ReportTextHelper.Parse("  ").Should().BeEmpty();
    }
}
