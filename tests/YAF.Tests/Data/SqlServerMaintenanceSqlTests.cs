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

/// <summary>
/// The maintenance statements of SQL Server (database names that need quotes).
/// </summary>
[TestFixture]
public class SqlServerMaintenanceSqlTests
{
    [Test]
    [Description("The name of the database is quoted when the recovery mode is changed (a name with a - failed)")]
    public void Change_Recovery_Mode_Quotes_The_Database_Test()
    {
        SqlServerDialect.Instance.ChangeRecoveryMode("yaf-forum", "SIMPLE")
            .Should().Be("ALTER DATABASE [yaf-forum] SET RECOVERY SIMPLE");

        SqlServerDialect.Instance.ChangeRecoveryMode("odd]name", "FULL")
            .Should().Be("ALTER DATABASE [odd]]name] SET RECOVERY FULL");
    }

    [Test]
    [Description("The name of the database is a string when the database is shrunk")]
    public void Shrink_Quotes_The_Database_Test()
    {
        SqlServerDialect.Instance.ShrinkDatabase("o'forum").Should().Be("DBCC SHRINKDATABASE(N'o''forum')");
    }
}
