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

namespace YAF.Tests.Data;

using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;
using ServiceStack.OrmLite.PostgreSQL;

/// <summary>
/// The date functions of the dialects, PostgreSQL and MySQL are not covered by the database tests (they run on SQLite).
/// </summary>
[TestFixture]
public class DialectDateDiffTests
{
    [Test]
    [Description("PostgreSQL: the minutes are the whole difference (EXTRACT(Minute) is only the minutes part of the interval, 2:05 hours were 5 minutes)")]
    public void PostgreSql_Minutes_Are_Whole_Difference_Test()
    {
        var sql = PostgreSqlDialect.Provider.DateDiffFunction("minute", "a", "b");

        sql.Should().Be("FLOOR(EXTRACT(EPOCH FROM (b::timestamp - a::timestamp)) / 60)::integer");

        // used in where fragments (e.g. the clean up of the active users)
        FluentActions.Invoking(() => $"{sql} > 5".SqlVerifyFragment()).Should().NotThrow();
    }

    [Test]
    [Description("PostgreSQL: the days of an interval of two timestamps are the whole days")]
    public void PostgreSql_Days_Test()
    {
        PostgreSqlDialect.Provider.DateDiffFunction("dd", "a", "b")
            .Should().Be("EXTRACT(Day from (b::timestamp - a::timestamp))");
    }

    [Test]
    [Description("MySQL: the current time is the time, not the date (UTC_DATE() is midnight: the active users of the day were never removed)")]
    public void MySql_Utc_Now_Is_Time_Test()
    {
        MySqlDialect.Provider.GetUtcDateFunction().Should().Be("UTC_TIMESTAMP()");
    }
}
